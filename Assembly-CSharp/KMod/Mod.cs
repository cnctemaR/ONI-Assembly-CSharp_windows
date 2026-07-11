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

		[JsonConstructor]
		public Mod()
		{
		}

		public void CopyPersistentDataTo(Mod other_mod)
		{
			other_mod.status = this.status;
			other_mod.enabled = this.enabled;
			other_mod.crash_count = this.crash_count;
			other_mod.loaded_content = this.loaded_content;
			other_mod.loaded_mod_data = this.loaded_mod_data;
			other_mod.reinstall_path = this.reinstall_path;
		}

		public Mod(Label label, string description, IFileSource file_source, LocString manage_tooltip, global::System.Action on_managed)
		{
			this.enabled = false;
			this.label = label;
			this.status = Mod.Status.NotInstalled;
			this.description = description;
			this.file_source = file_source;
			this.manage_tooltip = manage_tooltip;
			this.on_managed = on_managed;
			this.loaded_content = (Content)0;
			this.available_content = (Content)0;
			this.ScanContent();
		}

		public void ScanContent()
		{
			this.available_content = (Content)0;
			if (this.file_source == null)
			{
				this.file_source = new Directory(this.label.install_path);
			}
			if (!this.file_source.Exists())
			{
				return;
			}
			if (this.ScanContentFromSource(""))
			{
				if (this.content_source == null)
				{
					this.content_source = new Directory(this.ContentPath);
					return;
				}
			}
			else
			{
				global::Debug.LogWarning(string.Format("No supported content for mod: {0}, skipping content.", this.label));
				this.available_content = (Content)0;
				this.enabled = false;
			}
		}

		private bool ScanContentFromSource(string relativeRoot = "")
		{
			this.available_content = (Content)0;
			List<FileSystemItem> list = new List<FileSystemItem>();
			this.file_source.GetTopLevelItems(list, relativeRoot);
			bool flag = false;
			bool flag2 = false;
			foreach (FileSystemItem fileSystemItem in list)
			{
				if (fileSystemItem.type == FileSystemItem.ItemType.Directory)
				{
					string text = fileSystemItem.name.ToLower();
					this.AddDirectory(text);
				}
				else
				{
					string text2 = fileSystemItem.name.ToLower();
					if (text2 == "archived_versions.yaml")
					{
						flag = true;
					}
					else if (text2 == "mod_info.yaml")
					{
						flag2 = true;
					}
					else
					{
						this.AddFile(text2);
					}
				}
			}
			bool flag3 = true;
			if (flag2)
			{
				string text3 = this.file_source.Read(Path.Combine(relativeRoot, "mod_info.yaml"));
				if (!string.IsNullOrEmpty(text3))
				{
					Mod.PackagedModInfo packagedModInfo = YamlIO.Parse<Mod.PackagedModInfo>(text3, default(FileHandle), null, null);
					if (packagedModInfo != null)
					{
						string text4 = (string.IsNullOrEmpty(this.relative_root) ? "root" : this.relative_root);
						if (packagedModInfo.supportedContent != null)
						{
							string text5 = packagedModInfo.supportedContent.ToLower();
							if (!text5.Contains("vanilla_id") && !text5.Contains("all"))
							{
								global::Debug.Log(string.Format("Skipping mod {0} at {1} because it does not support vanilla content.", this.label, text4));
								flag3 = false;
							}
						}
					}
					else
					{
						global::Debug.LogWarning("Failed to parse mod_info.yaml, text is " + text3);
					}
				}
				else
				{
					global::Debug.LogWarning("Failed to read mod_info.yaml, skipping");
				}
			}
			if (!string.IsNullOrEmpty(this.relative_root))
			{
				return flag3;
			}
			if (!flag)
			{
				return flag3;
			}
			string text6 = this.file_source.Read("archived_versions.yaml");
			if (string.IsNullOrEmpty(text6))
			{
				global::Debug.LogWarning("Failed to read archived_versions.yaml, skipping");
				return flag3;
			}
			Mod.ArchivedVersionArray archivedVersionArray = YamlIO.Parse<Mod.ArchivedVersionArray>(text6, default(FileHandle), null, null);
			if (archivedVersionArray == null)
			{
				global::Debug.LogWarning("Failed to parse archived_versions.yaml, text is " + text6);
				return flag3;
			}
			List<Mod.ArchivedVersion> list2;
			if (flag3)
			{
				list2 = archivedVersionArray.archivedVersions.Where<Mod.ArchivedVersion>((Mod.ArchivedVersion versionInfo) => (long)versionInfo.lastWorkingBuild == 444111L).ToList<Mod.ArchivedVersion>();
			}
			else
			{
				list2 = (from versionInfo in archivedVersionArray.archivedVersions
					where (long)versionInfo.lastWorkingBuild >= 444111L
					orderby versionInfo.lastWorkingBuild
					select versionInfo).Concat<Mod.ArchivedVersion>(from versionInfo in archivedVersionArray.archivedVersions
					where (long)versionInfo.lastWorkingBuild < 444111L
					orderby versionInfo.lastWorkingBuild descending
					select versionInfo).ToList<Mod.ArchivedVersion>();
			}
			foreach (Mod.ArchivedVersion archivedVersion in list2)
			{
				this.relative_root = FileSystem.Normalize(archivedVersion.relativePath);
				if (!this.relative_root.StartsWith("archived_version"))
				{
					global::Debug.LogError("Archived version with path: " + archivedVersion.relativePath + ". For consistency among mods, please keep all old versions in a top-level directory called \"archived_versions\"");
					return false;
				}
				Content available_content = this.available_content;
				if (this.ScanContentFromSource(this.relative_root))
				{
					global::Debug.Log(string.Format("Found archived version for mod {0} with lastWorkingBuild: {1}, redirected content path to {2}", this.title, archivedVersion.lastWorkingBuild, this.relative_root));
					return true;
				}
				this.relative_root = null;
				this.available_content = available_content;
			}
			return flag3;
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

		private void AddDirectory(string directory)
		{
			string text = directory.TrimEnd(new char[] { '/' });
			if (text == "strings")
			{
				this.available_content |= Content.Strings;
				return;
			}
			if (text == "codex")
			{
				this.available_content |= Content.LayerableFiles;
				return;
			}
			if (text == "elements")
			{
				this.available_content |= Content.LayerableFiles;
				return;
			}
			if (text == "templates")
			{
				this.available_content |= Content.LayerableFiles;
				return;
			}
			if (text == "worldgen")
			{
				this.available_content |= Content.LayerableFiles;
				return;
			}
			if (!(text == "anim"))
			{
				return;
			}
			this.available_content |= Content.Animation;
		}

		private void AddFile(string file)
		{
			if (file.EndsWith(".dll"))
			{
				this.available_content |= Content.DLL;
			}
			if (file.EndsWith(".po"))
			{
				this.available_content |= Content.Translation;
			}
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
			this.content_source = new Directory(this.ContentPath);
			this.status = Mod.Status.Installed;
		}

		public bool Uninstall()
		{
			this.enabled = false;
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
				this.loaded_mod_data = DLLLoader.LoadDLLs(this.label.id + "." + this.label.distribution_platform, this.ContentPath);
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
				this.enabled = false;
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

		[JsonProperty]
		public Label label;

		[JsonProperty]
		public Mod.Status status;

		[JsonProperty]
		public bool enabled;

		[JsonProperty]
		public int crash_count;

		[JsonProperty]
		public string reinstall_path;

		public bool foundInStackTrace;

		public string relative_root = "";

		public LoadedModData loaded_mod_data;

		public IFileSource file_source;

		public IFileSource content_source;

		public bool is_subscribed;

		private const string ARCHIVED_VERSIONS_FILENAME = "archived_versions.yaml";

		private const string MOD_INFO_FILENAME = "mod_info.yaml";

		public const int MAX_CRASH_COUNT = 3;

		public enum Status
		{
			NotInstalled,
			Installed,
			UninstallPending,
			ReinstallPending
		}

		public class ArchivedVersionArray
		{
			public Mod.ArchivedVersion[] archivedVersions { get; set; }

			public ArchivedVersionArray()
			{
				this.archivedVersions = new Mod.ArchivedVersion[0];
			}
		}

		public class ArchivedVersion
		{
			public string relativePath { get; set; }

			public int lastWorkingBuild { get; set; }
		}

		public class PackagedModInfo
		{
			public string supportedContent { get; set; }

			public int lastWorkingBuild { get; set; }
		}
	}
}
