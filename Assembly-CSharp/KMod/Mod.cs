using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Klei;
using Newtonsoft.Json;
using UnityEngine;

namespace KMod
{
	[JsonObject(MemberSerialization.OptIn)]
	[DebuggerDisplay("{title}")]
	public class Mod
	{
		[JsonConstructor]
		public Mod()
		{
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

		public void CopyPersistentDataTo(Mod other_mod)
		{
			other_mod.status = this.status;
			other_mod.enabled = this.enabled;
			other_mod.crash_count = this.crash_count;
			other_mod.loaded_content = this.loaded_content;
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
			List<FileSystemItem> list = new List<FileSystemItem>();
			this.file_source.GetTopLevelItems(list);
			foreach (FileSystemItem fileSystemItem in list)
			{
				if (fileSystemItem.type == FileSystemItem.ItemType.Directory)
				{
					this.AddDirectory(fileSystemItem.name.ToLower());
				}
				else
				{
					this.AddFile(fileSystemItem.name.ToLower());
				}
			}
		}

		public bool IsEmpty()
		{
			return this.available_content == (Content)0;
		}

		private void AddDirectory(string directory)
		{
			string text = directory.TrimEnd(new char[] { '/' });
			if (text != null)
			{
				if (!(text == "strings"))
				{
					if (!(text == "codex"))
					{
						if (!(text == "elements"))
						{
							if (!(text == "templates"))
							{
								if (!(text == "worldgen"))
								{
									if (text == "anims")
									{
										this.available_content |= Content.Animation;
									}
								}
								else
								{
									this.available_content |= Content.LayerableFiles;
								}
							}
							else
							{
								this.available_content |= Content.LayerableFiles;
							}
						}
						else
						{
							this.available_content |= Content.LayerableFiles;
						}
					}
					else
					{
						this.available_content |= Content.LayerableFiles;
					}
				}
				else
				{
					this.available_content |= Content.Strings;
				}
			}
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
			if ((byte)(content & Content.DLL) != 0)
			{
				extensions.Add(".dll");
			}
			if ((byte)(content & (Content.Strings | Content.Translation)) != 0)
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
			}
			else
			{
				DebugUtil.Assert(condition, string.Format("{1}\n\t{0}", this.label.ToString(), failure_message));
			}
		}

		public void Install()
		{
			if (this.label.distribution_platform == Label.DistributionPlatform.Local || this.label.distribution_platform == Label.DistributionPlatform.Dev)
			{
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
			if (this.label.distribution_platform == Label.DistributionPlatform.Local || this.label.distribution_platform == Label.DistributionPlatform.Dev)
			{
				return false;
			}
			this.enabled = false;
			if (this.loaded_content != (Content)0)
			{
				global::Debug.Log(string.Format("Can't uninstall {0}: still has loaded content: {1}", this.label.ToString(), this.loaded_content.ToString()));
				this.status = Mod.Status.UninstallPending;
				return false;
			}
			if (!FileUtil.DeleteDirectory(this.label.install_path, 0))
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
			string text = FileSystem.Normalize(Path.Combine(this.label.install_path, "strings"));
			if (!Directory.Exists(text))
			{
				return false;
			}
			int num = 0;
			DirectoryInfo directoryInfo = new DirectoryInfo(text);
			foreach (FileInfo fileInfo in directoryInfo.GetFiles())
			{
				if (!(fileInfo.Extension.ToLower() != ".po"))
				{
					num++;
					Dictionary<string, string> dictionary = Localization.LoadStringsFile(fileInfo.FullName, false);
					Localization.OverloadStrings(dictionary);
				}
			}
			return true;
		}

		private bool LoadTranslations()
		{
			string text = FileSystem.Normalize(this.label.install_path);
			if (!Directory.Exists(text))
			{
				return false;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(text);
			HashSetPool<Localization.Locale, Mod>.PooledHashSet pooledHashSet = HashSetPool<Localization.Locale, Mod>.Allocate();
			foreach (FileInfo fileInfo in directoryInfo.GetFiles())
			{
				if (!(fileInfo.Extension.ToLower() != ".po"))
				{
					string[] array = File.ReadAllLines(fileInfo.FullName, Encoding.UTF8);
					pooledHashSet.Add(Localization.GetLocale(array));
					Dictionary<string, string> dictionary = Localization.ExtractTranslatedStrings(array, false);
					Localization.OverloadStrings(dictionary);
				}
			}
			if (pooledHashSet.Count == 0)
			{
				return false;
			}
			Localization.Locale new_locale = pooledHashSet.First<Localization.Locale>();
			if (!pooledHashSet.All<Localization.Locale>((Localization.Locale locale) => locale == new_locale))
			{
				return false;
			}
			Localization.SetLocale(new_locale);
			Localization.SwapToLocalizedFont(new_locale.FontName);
			KPlayerPrefs.SetString(Localization.SELECTED_LANGUAGE_TYPE_KEY, Localization.SelectedLanguageType.UGC.ToString());
			KPlayerPrefs.SetString(Localization.SELECTED_LANGUAGE_CODE_KEY, new_locale.Code);
			return true;
		}

		private bool LoadAnimation()
		{
			string text = FileSystem.Normalize(Path.Combine(this.label.install_path, "anims"));
			if (!Directory.Exists(text))
			{
				return false;
			}
			int num = 0;
			ListPool<Texture2D, Mod>.PooledList pooledList = ListPool<Texture2D, Mod>.Allocate();
			DirectoryInfo directoryInfo = new DirectoryInfo(text);
			foreach (FileInfo fileInfo in directoryInfo.GetFiles())
			{
				TextAsset textAsset = null;
				TextAsset textAsset2 = null;
				pooledList.Clear();
				AssetBundle assetBundle = AssetBundle.LoadFromFile(fileInfo.FullName);
				global::UnityEngine.Object[] array = assetBundle.LoadAllAssets();
				foreach (global::UnityEngine.Object @object in array)
				{
					Texture2D texture2D = @object as Texture2D;
					if (texture2D != null)
					{
						pooledList.Add(texture2D);
					}
					else if (@object.name.EndsWith("_anim"))
					{
						textAsset = @object as TextAsset;
					}
					else if (@object.name.EndsWith("_build"))
					{
						textAsset2 = @object as TextAsset;
					}
					else
					{
						DebugUtil.LogWarningArgs(new object[] { string.Format("Unhandled asset ({0}) in bundle ({1})...ignoring", @object.name, fileInfo.FullName) });
					}
				}
				if (ModUtil.AddKAnim(fileInfo.Name, textAsset, textAsset2, pooledList) != null)
				{
					num++;
				}
			}
			pooledList.Recycle();
			return true;
		}

		public void Load(Content content)
		{
			content &= this.available_content & ~this.loaded_content;
			if ((byte)(content & Content.Strings) != 0 && this.LoadStrings())
			{
				this.loaded_content |= Content.Strings;
			}
			if ((byte)(content & Content.Translation) != 0 && this.LoadTranslations())
			{
				this.loaded_content |= Content.Translation;
			}
			if ((byte)(content & Content.DLL) != 0 && DLLLoader.LoadDLLs(this.label.install_path))
			{
				this.loaded_content |= Content.DLL;
			}
			if ((byte)(content & Content.LayerableFiles) != 0)
			{
				FileSystem.file_sources.Insert(0, this.file_source.GetFileSystem());
				this.loaded_content |= Content.LayerableFiles;
			}
			if ((byte)(content & Content.Animation) != 0 && this.LoadAnimation())
			{
				this.loaded_content |= Content.Animation;
			}
		}

		public void Unload(Content content)
		{
			content &= this.loaded_content;
			if ((byte)(content & Content.LayerableFiles) != 0)
			{
				FileSystem.file_sources.Remove(this.file_source.GetFileSystem());
				this.loaded_content &= ~Content.LayerableFiles;
			}
		}

		private void SetCrashCount(int new_crash_count)
		{
			this.crash_count = MathUtil.Clamp(0, 3, new_crash_count);
		}

		public void Crash(bool do_disable)
		{
			this.SetCrashCount(this.crash_count + 1);
			if (do_disable)
			{
				this.enabled = false;
			}
		}

		public void Uncrash()
		{
			this.SetCrashCount((this.label.distribution_platform != Label.DistributionPlatform.Dev) ? 0 : (this.crash_count - 1));
		}

		public bool IsActive()
		{
			return this.loaded_content != (Content)0;
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
			return (byte)(this.loaded_content & content) != 0;
		}

		public bool HasContent()
		{
			return this.available_content != (Content)0;
		}

		public bool HasAnyContent(Content content)
		{
			return (byte)(this.available_content & content) != 0;
		}

		[JsonProperty]
		public Label label;

		[JsonProperty]
		public Mod.Status status;

		[JsonProperty]
		public bool enabled;

		[JsonProperty]
		public int crash_count;

		public IFileSource file_source;

		public bool is_subscribed;

		public const int MAX_CRASH_COUNT = 3;

		public enum Status
		{
			NotInstalled,
			Installed,
			UninstallPending,
			ReinstallPending
		}
	}
}
