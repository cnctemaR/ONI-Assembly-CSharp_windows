using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Klei;
using Newtonsoft.Json;
using UnityEngine;

namespace KMod
{
	[JsonObject(MemberSerialization.OptIn)]
	[DebuggerDisplay("{title}")]
	public class Mod
	{
		public Content available_content { get; private set; }

		[JsonProperty]
		public string staticID { get; private set; }

		public LocString manage_tooltip { get; private set; }

		public global::System.Action on_managed { get; private set; }

		public bool is_managed
		{
			get
			{
				return this.manage_tooltip != null;
			}
		}

		public string title
		{
			get
			{
				return this.label.title;
			}
		}

		public string description { get; private set; }

		public Content loaded_content { get; private set; }

		public IFileSource file_source
		{
			get
			{
				return this._fileSource;
			}
			set
			{
				if (this._fileSource != null)
				{
					this._fileSource.Dispose();
				}
				this._fileSource = value;
			}
		}

		public bool DevModCrashTriggered { get; private set; }

		[JsonConstructor]
		public Mod()
		{
		}

		public void CopyPersistentDataTo(Mod other_mod)
		{
			other_mod.status = this.status;
			other_mod.enabledForDlc = ((this.enabledForDlc != null) ? new List<string>(this.enabledForDlc) : new List<string>());
			other_mod.crash_count = this.crash_count;
			other_mod.loaded_content = this.loaded_content;
			other_mod.loaded_mod_data = this.loaded_mod_data;
			other_mod.reinstall_path = this.reinstall_path;
		}

		public Mod(Label label, string staticID, string description, IFileSource file_source, LocString manage_tooltip, global::System.Action on_managed)
		{
			this.label = label;
			this.status = Mod.Status.NotInstalled;
			this.staticID = staticID;
			this.description = description;
			this.file_source = file_source;
			this.manage_tooltip = manage_tooltip;
			this.on_managed = on_managed;
			this.loaded_content = (Content)0;
			this.available_content = (Content)0;
			this.ScanContent();
		}

		public bool IsEnabledForActiveDlc()
		{
			return this.IsEnabledForDlc(DlcManager.GetHighestActiveDlcId());
		}

		public bool IsEnabledForDlc(string dlcId)
		{
			return this.enabledForDlc != null && this.enabledForDlc.Contains(dlcId);
		}

		public void SetEnabledForActiveDlc(bool enabled)
		{
			this.SetEnabledForDlc(DlcManager.GetHighestActiveDlcId(), enabled);
		}

		public void SetEnabledForDlc(string dlcId, bool set_enabled)
		{
			if (this.enabledForDlc == null)
			{
				this.enabledForDlc = new List<string>();
			}
			bool flag = this.enabledForDlc.Contains(dlcId);
			if (set_enabled && !flag)
			{
				this.enabledForDlc.Add(dlcId);
				return;
			}
			if (!set_enabled && flag)
			{
				this.enabledForDlc.Remove(dlcId);
			}
		}

		public void ScanContent()
		{
			this.ModDevLog(string.Format("{0} ({1}): Setting up mod.", this.label, this.label.id));
			this.available_content = (Content)0;
			if (this.file_source == null)
			{
				if (this.label.id.EndsWith(".zip"))
				{
					DebugUtil.DevAssert(false, "Does this actually get used ever?", null);
					this.file_source = new ZipFile(this.label.install_path);
				}
				else
				{
					this.file_source = new Directory(this.label.install_path);
				}
			}
			if (!this.file_source.Exists())
			{
				global::Debug.LogWarning(string.Format("{0}: File source does not appear to be valid, skipping. ({1})", this.label, this.label.install_path));
				return;
			}
			KModHeader header = KModUtil.GetHeader(this.file_source, this.label.defaultStaticID, this.label.title, this.description, this.IsDev);
			if (this.label.title != header.title)
			{
				global::Debug.Log(string.Concat(new string[]
				{
					"\t",
					this.label.title,
					" has a mod.yaml with the title `",
					header.title,
					"`, using that from now on."
				}));
			}
			if (this.label.defaultStaticID != header.staticID)
			{
				global::Debug.Log(string.Concat(new string[]
				{
					"\t",
					this.label.title,
					" has a mod.yaml with a staticID `",
					header.staticID,
					"`, using that from now on."
				}));
			}
			this.label.title = header.title;
			this.staticID = header.staticID;
			this.description = header.description;
			Mod.ArchivedVersion mostSuitableArchive = this.GetMostSuitableArchive();
			if (mostSuitableArchive == null)
			{
				global::Debug.LogWarning(string.Format("{0}: No archive supports this game version, skipping content.", this.label));
				this.contentCompatability = ModContentCompatability.DoesntSupportDLCConfig;
				this.available_content = (Content)0;
				this.SetEnabledForActiveDlc(false);
				return;
			}
			this.packagedModInfo = mostSuitableArchive.info;
			Content content;
			this.ScanContentFromSource(mostSuitableArchive.relativePath, out content);
			if (content == (Content)0)
			{
				global::Debug.LogWarning(string.Format("{0}: No supported content for mod, skipping content.", this.label));
				this.contentCompatability = ModContentCompatability.NoContent;
				this.available_content = (Content)0;
				this.SetEnabledForActiveDlc(false);
				return;
			}
			bool flag = mostSuitableArchive.info.APIVersion == 2;
			if ((content & Content.DLL) != (Content)0 && !flag)
			{
				global::Debug.LogWarning(string.Format("{0}: DLLs found but not using the correct API version.", this.label));
				this.contentCompatability = ModContentCompatability.OldAPI;
				this.available_content = (Content)0;
				this.SetEnabledForActiveDlc(false);
				return;
			}
			this.contentCompatability = ModContentCompatability.OK;
			this.available_content = content;
			this.relative_root = mostSuitableArchive.relativePath;
			global::Debug.Assert(this.content_source == null);
			this.content_source = new Directory(this.ContentPath);
			string text = (string.IsNullOrEmpty(this.relative_root) ? "root" : this.relative_root);
			global::Debug.Log(string.Format("{0}: Successfully loaded from path '{1}' with content '{2}'.", this.label, text, this.available_content.ToString()));
		}

		private Mod.ArchivedVersion GetMostSuitableArchive()
		{
			Mod.PackagedModInfo packagedModInfo = this.GetModInfoForFolder("");
			if (packagedModInfo == null)
			{
				packagedModInfo = new Mod.PackagedModInfo
				{
					supportedContent = "vanilla_id",
					minimumSupportedBuild = 0
				};
				if (this.ScanContentFromSourceForTranslationsOnly(""))
				{
					this.ModDevLogWarning(string.Format("{0}: No mod_info.yaml found, but since it contains a translation, default its supported content to 'ALL'", this.label));
					packagedModInfo.supportedContent = "all";
				}
				else
				{
					this.ModDevLogWarning(string.Format("{0}: No mod_info.yaml found, default its supported content to 'VANILLA_ID'", this.label));
				}
			}
			Mod.ArchivedVersion archivedVersion = new Mod.ArchivedVersion
			{
				relativePath = "",
				info = packagedModInfo
			};
			if (!this.file_source.Exists("archived_versions"))
			{
				this.ModDevLog(string.Format("\t{0}: No archived_versions for this mod, using root version directly.", this.label));
				if (!this.DoesModSupportCurrentContent(packagedModInfo))
				{
					return null;
				}
				return archivedVersion;
			}
			else
			{
				List<FileSystemItem> list = new List<FileSystemItem>();
				this.file_source.GetTopLevelItems(list, "archived_versions");
				if (list.Count == 0)
				{
					this.ModDevLog(string.Format("\t{0}: No archived_versions for this mod, using root version directly.", this.label));
					if (!this.DoesModSupportCurrentContent(packagedModInfo))
					{
						return null;
					}
					return archivedVersion;
				}
				else
				{
					List<Mod.ArchivedVersion> list2 = new List<Mod.ArchivedVersion>();
					list2.Add(archivedVersion);
					foreach (FileSystemItem fileSystemItem in list)
					{
						string text = Path.Combine("archived_versions", fileSystemItem.name);
						Mod.PackagedModInfo modInfoForFolder = this.GetModInfoForFolder(text);
						if (modInfoForFolder != null)
						{
							list2.Add(new Mod.ArchivedVersion
							{
								relativePath = text,
								info = modInfoForFolder
							});
						}
					}
					list2 = list2.Where<Mod.ArchivedVersion>((Mod.ArchivedVersion v) => this.DoesModSupportCurrentContent(v.info)).ToList<Mod.ArchivedVersion>();
					list2 = list2.Where<Mod.ArchivedVersion>((Mod.ArchivedVersion v) => v.info.APIVersion == 2 || v.info.APIVersion == 0).ToList<Mod.ArchivedVersion>();
					Mod.ArchivedVersion archivedVersion2 = (from v in list2
						where (long)v.info.minimumSupportedBuild <= 587147L
						orderby v.info.minimumSupportedBuild descending
						select v).FirstOrDefault<Mod.ArchivedVersion>();
					if (archivedVersion2 == null)
					{
						return null;
					}
					return archivedVersion2;
				}
			}
		}

		private Mod.PackagedModInfo GetModInfoForFolder(string relative_root)
		{
			List<FileSystemItem> list = new List<FileSystemItem>();
			this.file_source.GetTopLevelItems(list, relative_root);
			bool flag = false;
			foreach (FileSystemItem fileSystemItem in list)
			{
				if (fileSystemItem.type == FileSystemItem.ItemType.File && fileSystemItem.name.ToLower() == "mod_info.yaml")
				{
					flag = true;
					break;
				}
			}
			string text = (string.IsNullOrEmpty(relative_root) ? "root" : relative_root);
			if (!flag)
			{
				this.ModDevLogWarning(string.Concat(new string[] { "\t", this.title, ": has no mod_info.yaml in folder '", text, "'" }));
				return null;
			}
			string text2 = this.file_source.Read(Path.Combine(relative_root, "mod_info.yaml"));
			if (string.IsNullOrEmpty(text2))
			{
				this.ModDevLogError(string.Format("\t{0}: Failed to read {1} in folder '{2}', skipping", this.label, "mod_info.yaml", text));
				return null;
			}
			YamlIO.ErrorHandler errorHandler = delegate(YamlIO.Error e, bool force_warning)
			{
				YamlIO.LogError(e, !this.IsDev);
			};
			Mod.PackagedModInfo packagedModInfo = YamlIO.Parse<Mod.PackagedModInfo>(text2, default(FileHandle), errorHandler, null);
			if (packagedModInfo == null)
			{
				this.ModDevLogError(string.Format("\t{0}: Failed to parse {1} in folder '{2}', text is {3}", new object[] { this.label, "mod_info.yaml", text, text2 }));
				return null;
			}
			if (packagedModInfo.supportedContent == null)
			{
				this.ModDevLogError(string.Format("\t{0}: {1} in folder '{2}' does not specify supportedContent. Make sure you spelled it correctly in your mod_info!", this.label, "mod_info.yaml", text));
				return null;
			}
			if (packagedModInfo.lastWorkingBuild != 0)
			{
				this.ModDevLogError(string.Format("\t{0}: {1} in folder '{2}' is using `{3}`, please upgrade this to `{4}`", new object[] { this.label, "mod_info.yaml", text, "lastWorkingBuild", "minimumSupportedBuild" }));
				if (packagedModInfo.minimumSupportedBuild == 0)
				{
					packagedModInfo.minimumSupportedBuild = packagedModInfo.lastWorkingBuild;
				}
			}
			this.ModDevLog(string.Format("\t{0}: Found valid mod_info.yaml in folder '{1}': {2} at {3}", new object[] { this.label, text, packagedModInfo.supportedContent, packagedModInfo.minimumSupportedBuild }));
			return packagedModInfo;
		}

		private bool DoesModSupportCurrentContent(Mod.PackagedModInfo mod_info)
		{
			string text = DlcManager.GetHighestActiveDlcId();
			if (text == "")
			{
				text = "vanilla_id";
			}
			text = text.ToLower();
			string text2 = mod_info.supportedContent.ToLower();
			return text2.Contains(text) || text2.Contains("all");
		}

		private bool ScanContentFromSourceForTranslationsOnly(string relativeRoot)
		{
			this.available_content = (Content)0;
			List<FileSystemItem> list = new List<FileSystemItem>();
			this.file_source.GetTopLevelItems(list, relativeRoot);
			foreach (FileSystemItem fileSystemItem in list)
			{
				if (fileSystemItem.type == FileSystemItem.ItemType.File && fileSystemItem.name.ToLower().EndsWith(".po"))
				{
					this.available_content |= Content.Translation;
				}
			}
			return this.available_content > (Content)0;
		}

		private bool ScanContentFromSource(string relativeRoot, out Content available)
		{
			available = (Content)0;
			List<FileSystemItem> list = new List<FileSystemItem>();
			this.file_source.GetTopLevelItems(list, relativeRoot);
			foreach (FileSystemItem fileSystemItem in list)
			{
				if (fileSystemItem.type == FileSystemItem.ItemType.Directory)
				{
					string text = fileSystemItem.name.ToLower();
					available |= this.AddDirectory(text);
				}
				else
				{
					string text2 = fileSystemItem.name.ToLower();
					available |= this.AddFile(text2);
				}
			}
			return available > (Content)0;
		}

		public string ContentPath
		{
			get
			{
				return Path.Combine(this.label.install_path, this.relative_root);
			}
		}

		public bool IsEmpty()
		{
			return this.available_content == (Content)0;
		}

		private Content AddDirectory(string directory)
		{
			Content content = (Content)0;
			string text = directory.TrimEnd(new char[] { '/' });
			if (text != null)
			{
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 1519694028U)
				{
					if (num != 948591336U)
					{
						if (num != 1318520008U)
						{
							if (num == 1519694028U)
							{
								if (text == "elements")
								{
									content |= Content.LayerableFiles;
								}
							}
						}
						else if (text == "buildingfacades")
						{
							content |= Content.Animation;
						}
					}
					else if (text == "templates")
					{
						content |= Content.LayerableFiles;
					}
				}
				else if (num <= 3037049615U)
				{
					if (num != 2960291089U)
					{
						if (num == 3037049615U)
						{
							if (text == "worldgen")
							{
								content |= Content.LayerableFiles;
							}
						}
					}
					else if (text == "strings")
					{
						content |= Content.Strings;
					}
				}
				else if (num != 3319670096U)
				{
					if (num == 3570262116U)
					{
						if (text == "codex")
						{
							content |= Content.LayerableFiles;
						}
					}
				}
				else if (text == "anim")
				{
					content |= Content.Animation;
				}
			}
			return content;
		}

		private Content AddFile(string file)
		{
			Content content = (Content)0;
			if (file.EndsWith(".dll"))
			{
				content |= Content.DLL;
			}
			if (file.EndsWith(".po"))
			{
				content |= Content.Translation;
			}
			return content;
		}

		private static void AccumulateExtensions(Content content, List<string> extensions)
		{
			if ((content & Content.DLL) != (Content)0)
			{
				extensions.Add(".dll");
			}
			if ((content & (Content.Strings | Content.Translation)) != (Content)0)
			{
				extensions.Add(".po");
			}
		}

		[Conditional("DEBUG")]
		private void Assert(bool condition, string failure_message)
		{
			if (string.IsNullOrEmpty(this.title))
			{
				DebugUtil.Assert(condition, string.Format("{2}\n\t{0}\n\t{1}", this.title, this.label.ToString(), failure_message));
				return;
			}
			DebugUtil.Assert(condition, string.Format("{1}\n\t{0}", this.label.ToString(), failure_message));
		}

		public void Install()
		{
			if (this.IsLocal)
			{
				this.status = Mod.Status.Installed;
				return;
			}
			this.status = Mod.Status.ReinstallPending;
			if (this.file_source == null)
			{
				return;
			}
			if (!FileUtil.DeleteDirectory(this.label.install_path, 0))
			{
				return;
			}
			if (!FileUtil.CreateDirectory(this.label.install_path, 0))
			{
				return;
			}
			this.file_source.CopyTo(this.label.install_path, null);
			this.file_source = new Directory(this.label.install_path);
			this.status = Mod.Status.Installed;
		}

		public bool Uninstall()
		{
			this.SetEnabledForActiveDlc(false);
			if (this.loaded_content != (Content)0)
			{
				global::Debug.Log(string.Format("Can't uninstall {0}: still has loaded content: {1}", this.label.ToString(), this.loaded_content.ToString()));
				this.status = Mod.Status.UninstallPending;
				return false;
			}
			if (!this.IsLocal && !FileUtil.DeleteDirectory(this.label.install_path, 0))
			{
				global::Debug.Log(string.Format("Can't uninstall {0}: directory deletion failed", this.label.ToString()));
				this.status = Mod.Status.UninstallPending;
				return false;
			}
			this.status = Mod.Status.NotInstalled;
			return true;
		}

		private bool LoadStrings()
		{
			string text = FileSystem.Normalize(Path.Combine(this.ContentPath, "strings"));
			if (!Directory.Exists(text))
			{
				return false;
			}
			int num = 0;
			foreach (FileInfo fileInfo in new DirectoryInfo(text).GetFiles())
			{
				if (!(fileInfo.Extension.ToLower() != ".po"))
				{
					num++;
					Localization.OverloadStrings(Localization.LoadStringsFile(fileInfo.FullName, false));
				}
			}
			return true;
		}

		private bool LoadTranslations()
		{
			return false;
		}

		private bool LoadAnimation()
		{
			string text = FileSystem.Normalize(Path.Combine(this.ContentPath, "anim"));
			if (!Directory.Exists(text))
			{
				return false;
			}
			int num = 0;
			DirectoryInfo[] directories = new DirectoryInfo(text).GetDirectories();
			for (int i = 0; i < directories.Length; i++)
			{
				foreach (DirectoryInfo directoryInfo in directories[i].GetDirectories())
				{
					KAnimFile.Mod mod = new KAnimFile.Mod();
					foreach (FileInfo fileInfo in directoryInfo.GetFiles())
					{
						if (fileInfo.Extension == ".png")
						{
							byte[] array = File.ReadAllBytes(fileInfo.FullName);
							Texture2D texture2D = new Texture2D(2, 2);
							texture2D.LoadImage(array);
							mod.textures.Add(texture2D);
						}
						else if (fileInfo.Extension == ".bytes")
						{
							string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
							byte[] array2 = File.ReadAllBytes(fileInfo.FullName);
							if (fileNameWithoutExtension.EndsWith("_anim"))
							{
								mod.anim = array2;
							}
							else if (fileNameWithoutExtension.EndsWith("_build"))
							{
								mod.build = array2;
							}
							else
							{
								DebugUtil.LogWarningArgs(new object[] { string.Format("Unhandled TextAsset ({0})...ignoring", fileInfo.FullName) });
							}
						}
						else
						{
							DebugUtil.LogWarningArgs(new object[] { string.Format("Unhandled asset ({0})...ignoring", fileInfo.FullName) });
						}
					}
					string text2 = directoryInfo.Name + "_kanim";
					if (mod.IsValid() && ModUtil.AddKAnimMod(text2, mod))
					{
						num++;
					}
				}
			}
			return true;
		}

		public void Load(Content content)
		{
			content &= this.available_content & ~this.loaded_content;
			if (content > (Content)0)
			{
				global::Debug.Log(string.Format("Loading mod content {2} [{0}:{1}] (provides {3})", new object[]
				{
					this.title,
					this.label.id,
					content.ToString(),
					this.available_content.ToString()
				}));
			}
			if ((content & Content.Strings) != (Content)0 && this.LoadStrings())
			{
				this.loaded_content |= Content.Strings;
			}
			if ((content & Content.Translation) != (Content)0 && this.LoadTranslations())
			{
				this.loaded_content |= Content.Translation;
			}
			if ((content & Content.DLL) != (Content)0)
			{
				this.loaded_mod_data = DLLLoader.LoadDLLs(this, this.staticID, this.ContentPath, this.IsDev);
				if (this.loaded_mod_data != null)
				{
					this.loaded_content |= Content.DLL;
				}
			}
			if ((content & Content.LayerableFiles) != (Content)0)
			{
				global::Debug.Assert(this.content_source != null, "Attempting to Load layerable files with content_source not initialized");
				FileSystem.file_sources.Insert(0, this.content_source.GetFileSystem());
				this.loaded_content |= Content.LayerableFiles;
			}
			if ((content & Content.Animation) != (Content)0 && this.LoadAnimation())
			{
				this.loaded_content |= Content.Animation;
			}
		}

		public void PostLoad(IReadOnlyList<Mod> mods)
		{
			if ((this.loaded_content & Content.DLL) != (Content)0 && this.loaded_mod_data != null)
			{
				DLLLoader.PostLoadDLLs(this.staticID, this.loaded_mod_data, mods);
			}
		}

		public void Unload(Content content)
		{
			content &= this.loaded_content;
			if ((content & Content.LayerableFiles) != (Content)0)
			{
				FileSystem.file_sources.Remove(this.content_source.GetFileSystem());
				this.loaded_content &= ~Content.LayerableFiles;
			}
		}

		private void SetCrashCount(int new_crash_count)
		{
			this.crash_count = MathUtil.Clamp(0, 3, new_crash_count);
		}

		public bool IsDev
		{
			get
			{
				return this.label.distribution_platform == Label.DistributionPlatform.Dev;
			}
		}

		public bool IsLocal
		{
			get
			{
				return this.label.distribution_platform == Label.DistributionPlatform.Dev || this.label.distribution_platform == Label.DistributionPlatform.Local;
			}
		}

		public void SetCrashed()
		{
			this.SetCrashCount(this.crash_count + 1);
			if (!this.IsDev)
			{
				this.SetEnabledForActiveDlc(false);
			}
		}

		public void Uncrash()
		{
			this.SetCrashCount(this.IsDev ? (this.crash_count - 1) : 0);
		}

		public bool IsActive()
		{
			return this.loaded_content > (Content)0;
		}

		public bool AllActive(Content content)
		{
			return (this.loaded_content & content) == content;
		}

		public bool AllActive()
		{
			return (this.loaded_content & this.available_content) == this.available_content;
		}

		public bool AnyActive(Content content)
		{
			return (this.loaded_content & content) > (Content)0;
		}

		public bool HasContent()
		{
			return this.available_content > (Content)0;
		}

		public bool HasAnyContent(Content content)
		{
			return (this.available_content & content) > (Content)0;
		}

		public bool HasOnlyTranslationContent()
		{
			return this.available_content == Content.Translation;
		}

		public Texture2D GetPreviewImage()
		{
			string text = null;
			foreach (string text2 in Mod.PREVIEW_FILENAMES)
			{
				if (Directory.Exists(this.ContentPath) && File.Exists(Path.Combine(this.ContentPath, text2)))
				{
					text = text2;
					break;
				}
			}
			if (text == null)
			{
				return null;
			}
			Texture2D texture2D2;
			try
			{
				byte[] array = File.ReadAllBytes(Path.Combine(this.ContentPath, text));
				Texture2D texture2D = new Texture2D(2, 2);
				texture2D.LoadImage(array);
				texture2D2 = texture2D;
			}
			catch
			{
				global::Debug.LogWarning(string.Format("Mod {0} seems to have a preview.png but it didn't load correctly.", this.label));
				texture2D2 = null;
			}
			return texture2D2;
		}

		public void ModDevLog(string msg)
		{
			if (this.IsDev)
			{
				global::Debug.Log(msg);
			}
		}

		public void ModDevLogWarning(string msg)
		{
			if (this.IsDev)
			{
				global::Debug.LogWarning(msg);
			}
		}

		public void ModDevLogError(string msg)
		{
			if (this.IsDev)
			{
				this.DevModCrashTriggered = true;
				global::Debug.LogError(msg);
			}
		}

		public const int MOD_API_VERSION_NONE = 0;

		public const int MOD_API_VERSION_HARMONY1 = 1;

		public const int MOD_API_VERSION_HARMONY2 = 2;

		public const int MOD_API_VERSION = 2;

		[JsonProperty]
		public Label label;

		[JsonProperty]
		public Mod.Status status;

		[JsonProperty]
		public bool enabled;

		[JsonProperty]
		public List<string> enabledForDlc;

		[JsonProperty]
		public int crash_count;

		[JsonProperty]
		public string reinstall_path;

		public bool foundInStackTrace;

		public string relative_root = "";

		public Mod.PackagedModInfo packagedModInfo;

		public LoadedModData loaded_mod_data;

		private IFileSource _fileSource;

		public IFileSource content_source;

		public bool is_subscribed;

		private const string VANILLA_ID = "vanilla_id";

		private const string ALL_ID = "all";

		private const string ARCHIVED_VERSIONS_FOLDER = "archived_versions";

		private const string MOD_INFO_FILENAME = "mod_info.yaml";

		public ModContentCompatability contentCompatability;

		public const int MAX_CRASH_COUNT = 3;

		private static readonly List<string> PREVIEW_FILENAMES = new List<string> { "preview.png", "Preview.png", "PREVIEW.PNG" };

		public enum Status
		{
			NotInstalled,
			Installed,
			UninstallPending,
			ReinstallPending
		}

		public class ArchivedVersion
		{
			public string relativePath;

			public Mod.PackagedModInfo info;
		}

		public class PackagedModInfo
		{
			public string supportedContent { get; set; }

			[Obsolete("Use minimumSupportedBuild instead!")]
			public int lastWorkingBuild { get; set; }

			public int minimumSupportedBuild { get; set; }

			public int APIVersion { get; set; }

			public string version { get; set; }
		}
	}
}
