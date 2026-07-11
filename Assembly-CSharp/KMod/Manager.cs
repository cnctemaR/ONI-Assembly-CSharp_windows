using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Klei;
using Newtonsoft.Json;
using STRINGS;
using UnityEngine;

namespace KMod
{
	public class Manager
	{
		public Manager()
		{
			Manager $this = this;
			global::Debug.Log("Load mod database");
			string filename = this.GetFilename();
			try
			{
				FileUtil.DoIOAction(delegate
				{
					if (File.Exists(filename))
					{
						string text = File.ReadAllText(filename);
						Manager.PersistentData persistentData = JsonConvert.DeserializeObject<Manager.PersistentData>(text);
						$this.mods = persistentData.mods;
					}
				}, 5);
			}
			catch (Exception)
			{
				global::Debug.LogWarningFormat(UI.FRONTEND.MODS.DB_CORRUPT, new object[] { filename });
				this.mods = new List<Mod>();
			}
			List<Mod> list = new List<Mod>();
			bool flag = false;
			foreach (Mod mod in this.mods)
			{
				Mod.Status status = mod.status;
				if (status != Mod.Status.UninstallPending)
				{
					if (status == Mod.Status.ReinstallPending)
					{
						global::Debug.LogFormat("Latent reinstall of mod {0}", new object[] { mod.title });
						if (!string.IsNullOrEmpty(mod.reinstall_path) && File.Exists(mod.reinstall_path))
						{
							bool enabled = mod.enabled;
							mod.file_source = new ZipFile(mod.reinstall_path);
							mod.enabled = false;
							if (mod.Uninstall())
							{
								mod.Install();
								if (mod.status == Mod.Status.Installed)
								{
									mod.enabled = enabled;
								}
							}
							flag = true;
						}
						else if (mod.enabled)
						{
							mod.enabled = false;
							flag = true;
						}
					}
				}
				else
				{
					global::Debug.LogFormat("Latent uninstall of mod {0} from {1}", new object[]
					{
						mod.title,
						mod.label.install_path
					});
					if (mod.Uninstall())
					{
						list.Add(mod);
					}
					else
					{
						DebugUtil.Assert(mod.status == Mod.Status.UninstallPending);
						global::Debug.LogFormat("\t...failed to uninstall mod {0}", new object[] { mod.title });
					}
					if (mod.status != Mod.Status.UninstallPending)
					{
						flag = true;
					}
				}
				if (!string.IsNullOrEmpty(mod.reinstall_path))
				{
					mod.reinstall_path = null;
					flag = true;
				}
			}
			foreach (Mod mod2 in list)
			{
				this.mods.Remove(mod2);
			}
			foreach (Mod mod3 in this.mods)
			{
				mod3.ScanContent();
			}
			if (flag)
			{
				this.Save();
			}
		}

		public static string GetDirectory()
		{
			string text = Util.RootFolder();
			return Path.Combine(text, "mods/");
		}

		public void Shutdown()
		{
			foreach (Mod mod in this.mods)
			{
				mod.Unload(Content.LayerableFiles);
			}
		}

		public void Sanitize(GameObject parent)
		{
			ListPool<Label, Manager>.PooledList pooledList = ListPool<Label, Manager>.Allocate();
			foreach (Mod mod in this.mods)
			{
				if (!mod.is_subscribed)
				{
					pooledList.Add(mod.label);
				}
			}
			foreach (Label label in pooledList)
			{
				this.Unsubscribe(label, this);
			}
			pooledList.Recycle();
			this.Report(parent);
		}

		public bool HaveMods()
		{
			foreach (Mod mod in this.mods)
			{
				if (mod.status == Mod.Status.Installed && mod.HasContent())
				{
					return true;
				}
			}
			return false;
		}

		public bool HaveLoadedMods()
		{
			foreach (Mod mod in this.mods)
			{
				if (mod.status != Mod.Status.NotInstalled && mod.IsActive())
				{
					return true;
				}
			}
			return false;
		}

		private void Install(Mod mod)
		{
			if (mod.status != Mod.Status.NotInstalled)
			{
				return;
			}
			global::Debug.LogFormat("\tInstalling mod: {0}", new object[] { mod.title });
			mod.Install();
			if (mod.status == Mod.Status.Installed)
			{
				global::Debug.Log("\tSuccessfully installed.");
				this.events.Add(new Event
				{
					event_type = EventType.Installed,
					mod = mod.label
				});
			}
			else
			{
				global::Debug.Log("\tFailed install. Will install on restart.");
				this.events.Add(new Event
				{
					event_type = EventType.InstallFailed,
					mod = mod.label
				});
				this.events.Add(new Event
				{
					event_type = EventType.RestartRequested,
					mod = mod.label
				});
			}
		}

		private void Uninstall(Mod mod)
		{
			if (mod.status == Mod.Status.NotInstalled)
			{
				return;
			}
			global::Debug.LogFormat("\tUninstalling mod {0}", new object[] { mod.title });
			mod.Uninstall();
			if (mod.status == Mod.Status.UninstallPending)
			{
				global::Debug.Log("\tFailed. Will re-install on restart.");
				mod.status = Mod.Status.ReinstallPending;
				this.events.Add(new Event
				{
					event_type = EventType.RestartRequested,
					mod = mod.label
				});
			}
		}

		public void Subscribe(Mod mod, object caller)
		{
			global::Debug.LogFormat("Subscribe to mod {0}", new object[] { mod.title });
			Mod mod2 = this.mods.Find((Mod candidate) => mod.label.Match(candidate.label));
			mod.is_subscribed = true;
			if (mod2 == null)
			{
				this.mods.Add(mod);
				this.Install(mod);
			}
			else
			{
				global::Debug.LogFormat("\tAlready subscribed mod: {0}", new object[] { mod.title });
				if (mod2.status == Mod.Status.UninstallPending)
				{
					mod2.status = Mod.Status.Installed;
					this.events.Add(new Event
					{
						event_type = EventType.Installed,
						mod = mod2.label
					});
				}
				bool flag = mod2.label.version != mod.label.version;
				bool flag2 = mod2.available_content != mod.available_content;
				bool flag3 = flag || flag2 || mod2.status == Mod.Status.ReinstallPending;
				if (flag)
				{
					this.events.Add(new Event
					{
						event_type = EventType.VersionUpdate,
						mod = mod.label
					});
				}
				if (flag2)
				{
					this.events.Add(new Event
					{
						event_type = EventType.AvailableContentChanged,
						mod = mod.label
					});
				}
				string root = mod.file_source.GetRoot();
				mod2.CopyPersistentDataTo(mod);
				int num = this.mods.IndexOf(mod2);
				this.mods.RemoveAt(num);
				this.mods.Insert(num, mod);
				if (flag3 || mod.status == Mod.Status.NotInstalled)
				{
					if (mod.enabled)
					{
						mod.reinstall_path = root;
						mod.status = Mod.Status.ReinstallPending;
						this.events.Add(new Event
						{
							event_type = EventType.RestartRequested,
							mod = mod.label
						});
					}
					else
					{
						if (flag3)
						{
							this.Uninstall(mod);
						}
						this.Install(mod);
					}
				}
				else
				{
					mod.file_source = mod2.file_source;
				}
			}
			this.dirty = true;
			this.Update(caller);
		}

		public void Update(Mod mod, object caller)
		{
			global::Debug.LogFormat("Update mod {0}", new object[] { mod.title });
			Mod mod2 = this.mods.Find((Mod candidate) => mod.label.Match(candidate.label));
			DebugUtil.DevAssert(!string.IsNullOrEmpty(mod2.label.id), "Should be subscribed to a mod we are getting an Update notification for");
			if (mod2.status == Mod.Status.UninstallPending)
			{
				return;
			}
			this.events.Add(new Event
			{
				event_type = EventType.VersionUpdate,
				mod = mod.label
			});
			string root = mod.file_source.GetRoot();
			mod2.CopyPersistentDataTo(mod);
			mod.is_subscribed = mod2.is_subscribed;
			int num = this.mods.IndexOf(mod2);
			this.mods.RemoveAt(num);
			this.mods.Insert(num, mod);
			if (mod.enabled)
			{
				mod.reinstall_path = root;
				mod.status = Mod.Status.ReinstallPending;
				this.events.Add(new Event
				{
					event_type = EventType.RestartRequested,
					mod = mod.label
				});
			}
			else
			{
				this.Uninstall(mod);
				this.Install(mod);
			}
			this.dirty = true;
			this.Update(caller);
		}

		public void Unsubscribe(Label label, object caller)
		{
			global::Debug.LogFormat("Unsubscribe from mod {0}", new object[] { label.ToString() });
			int num = 0;
			foreach (Mod mod in this.mods)
			{
				if (mod.label.Match(label))
				{
					global::Debug.LogFormat("\t...found it: {0}", new object[] { mod.title });
					break;
				}
				num++;
			}
			if (num == this.mods.Count)
			{
				global::Debug.LogFormat("\t...not found", new object[0]);
				return;
			}
			Mod mod2 = this.mods[num];
			mod2.enabled = false;
			mod2.Unload(Content.LayerableFiles);
			this.events.Add(new Event
			{
				event_type = EventType.Uninstalled,
				mod = mod2.label
			});
			if (mod2.IsActive())
			{
				global::Debug.LogFormat("\tCould not unload all content provided by mod {0} : {1}\nUninstall will likely fail", new object[]
				{
					mod2.title,
					mod2.label.ToString()
				});
				this.events.Add(new Event
				{
					event_type = EventType.RestartRequested,
					mod = mod2.label
				});
			}
			if (mod2.status == Mod.Status.Installed)
			{
				global::Debug.LogFormat("\tUninstall mod {0} : {1}", new object[]
				{
					mod2.title,
					mod2.label.ToString()
				});
				mod2.Uninstall();
			}
			if (mod2.status == Mod.Status.NotInstalled)
			{
				global::Debug.LogFormat("\t...success. Removing from management list {0} : {1}", new object[]
				{
					mod2.title,
					mod2.label.ToString()
				});
				this.mods.RemoveAt(num);
			}
			this.dirty = true;
			this.Update(caller);
		}

		public bool IsInDevMode()
		{
			return this.mods.Exists((Mod mod) => mod.enabled && mod.label.distribution_platform == Label.DistributionPlatform.Dev);
		}

		public void Load(Content content)
		{
			if ((byte)(content & Content.DLL) != 0 && this.load_user_mod_loader_dll)
			{
				if (!DLLLoader.LoadUserModLoaderDLL())
				{
					global::Debug.Log("ModLoader.dll failed to load. Either it is not present or it encountered an error");
				}
				this.load_user_mod_loader_dll = false;
			}
			global::Debug.LogFormat("Load content ({0}) for mods:", new object[] { content });
			foreach (Mod mod in this.mods)
			{
				if (mod.enabled)
				{
					mod.Load(content);
					if (mod.IsActive())
					{
						global::Debug.LogFormat("\t{0}", new object[] { mod.title });
					}
				}
			}
			bool flag = false;
			bool flag2 = this.IsInDevMode();
			foreach (Mod mod2 in this.mods)
			{
				Content content2 = mod2.loaded_content & content;
				Content content3 = mod2.available_content & content;
				if (mod2.enabled && content2 != content3)
				{
					mod2.Crash(!flag2);
					if (!mod2.enabled)
					{
						flag = true;
						this.events.Add(new Event
						{
							event_type = EventType.Deactivated,
							mod = mod2.label
						});
					}
					global::Debug.LogFormat("Failed to load mod {0}...disabling", new object[] { mod2.title });
					this.events.Add(new Event
					{
						event_type = EventType.LoadError,
						mod = mod2.label
					});
				}
			}
			if (flag)
			{
				this.Save();
			}
		}

		public void Unload(Content content)
		{
			foreach (Mod mod in this.mods)
			{
				mod.Unload(content);
			}
		}

		public void Update(object change_source)
		{
			if (!this.dirty)
			{
				return;
			}
			this.dirty = false;
			this.Save();
			if (this.on_update != null)
			{
				this.on_update(change_source);
			}
		}

		public bool MatchFootprint(List<Label> footprint, Content relevant_content)
		{
			if (footprint == null)
			{
				return true;
			}
			bool flag = true;
			bool flag2 = true;
			bool flag3 = false;
			int num = -1;
			Func<Label, Mod, bool> is_match = (Label label, Mod mod) => mod.label.Match(label);
			using (List<Label>.Enumerator enumerator = footprint.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Label label = enumerator.Current;
					bool flag4 = false;
					for (int num2 = num + 1; num2 != this.mods.Count; num2++)
					{
						Mod mod3 = this.mods[num2];
						num = num2;
						Content content = mod3.available_content & relevant_content;
						bool flag5 = content != (Content)0;
						if (is_match(label, mod3))
						{
							if (flag5)
							{
								if (!mod3.enabled)
								{
									this.events.Add(new Event
									{
										event_type = EventType.ExpectedActive,
										mod = label
									});
									flag = false;
								}
								else if (!mod3.AllActive(content))
								{
									this.events.Add(new Event
									{
										event_type = EventType.LoadError,
										mod = label
									});
								}
							}
							flag4 = true;
							break;
						}
						if (flag5 && mod3.enabled)
						{
							this.events.Add(new Event
							{
								event_type = EventType.ExpectedInactive,
								mod = mod3.label
							});
							flag3 = true;
						}
					}
					if (!flag4)
					{
						this.events.Add(new Event
						{
							event_type = ((!this.mods.Exists((Mod candidate) => is_match(label, candidate))) ? EventType.NotFound : EventType.OutOfOrder),
							mod = label
						});
						flag2 = false;
					}
				}
			}
			for (int num3 = num + 1; num3 != this.mods.Count; num3++)
			{
				Mod mod2 = this.mods[num3];
				bool flag6 = (byte)(mod2.available_content & relevant_content) != 0;
				if (flag6 && mod2.enabled)
				{
					this.events.Add(new Event
					{
						event_type = EventType.ExpectedInactive,
						mod = mod2.label
					});
					flag3 = true;
				}
			}
			return flag2 && flag && !flag3;
		}

		private string GetFilename()
		{
			return FileSystem.Normalize(Path.Combine(Manager.GetDirectory(), "mods.json"));
		}

		public static void Dialog(GameObject parent = null, string title = null, string text = null, string confirm_text = null, global::System.Action on_confirm = null, string cancel_text = null, global::System.Action on_cancel = null, string configurable_text = null, global::System.Action on_configurable_clicked = null, Sprite image_sprite = null, bool activateBlackBackground = true)
		{
			ConfirmDialogScreen confirmDialogScreen = (ConfirmDialogScreen)KScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, parent ?? Global.Instance.globalCanvas);
			confirmDialogScreen.PopupConfirmDialog(text, on_confirm, on_cancel, configurable_text, on_configurable_clicked, title, confirm_text, cancel_text, image_sprite, activateBlackBackground);
		}

		private static string MakeModList(List<Event> events, EventType event_type)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine();
			foreach (Event @event in events)
			{
				if (@event.event_type == event_type)
				{
					stringBuilder.AppendLine(@event.mod.title);
				}
			}
			return stringBuilder.ToString();
		}

		private static string MakeEventList(List<Event> events)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine();
			string text = null;
			string text2 = null;
			foreach (Event @event in events)
			{
				Event.GetUIStrings(@event.event_type, out text, out text2);
				stringBuilder.AppendFormat("{0}: {1}", text, @event.mod.title);
				if (!string.IsNullOrEmpty(@event.details))
				{
					stringBuilder.AppendFormat(" ({0})", @event.details);
				}
				stringBuilder.Append("\n");
			}
			return stringBuilder.ToString();
		}

		private static string MakeModList(List<Event> events)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine();
			HashSetPool<string, Manager>.PooledHashSet pooledHashSet = HashSetPool<string, Manager>.Allocate();
			foreach (Event @event in events)
			{
				if (pooledHashSet.Add(@event.mod.title))
				{
					stringBuilder.AppendLine(@event.mod.title);
				}
			}
			pooledHashSet.Recycle();
			return stringBuilder.ToString();
		}

		private void LoadFailureDialog(GameObject parent)
		{
			if (this.events.Count == 0)
			{
				return;
			}
			foreach (Event @event in this.events)
			{
				if (@event.event_type == EventType.LoadError)
				{
					foreach (Mod mod in this.mods)
					{
						if (mod.label.distribution_platform != Label.DistributionPlatform.Local && mod.label.distribution_platform != Label.DistributionPlatform.Dev)
						{
							if (mod.label.Match(@event.mod))
							{
								mod.status = Mod.Status.ReinstallPending;
							}
						}
					}
				}
			}
			this.dirty = true;
			this.Update(this);
			string text = UI.FRONTEND.MOD_DIALOGS.LOAD_FAILURE.TITLE;
			string text2 = string.Format(UI.FRONTEND.MOD_DIALOGS.LOAD_FAILURE.MESSAGE, Manager.MakeModList(this.events, EventType.LoadError));
			string text3 = UI.FRONTEND.MOD_DIALOGS.RESTART.OK;
			string text4 = UI.FRONTEND.MOD_DIALOGS.RESTART.CANCEL;
			Manager.Dialog(parent, text, text2, text3, new global::System.Action(App.instance.Restart), text4, delegate
			{
			}, null, null, null, true);
			this.events.Clear();
		}

		private void DevRestartDialog(GameObject parent, bool is_crash)
		{
			if (this.events.Count == 0)
			{
				return;
			}
			if (is_crash)
			{
				string text = UI.FRONTEND.MOD_DIALOGS.MOD_ERRORS_ON_BOOT.TITLE;
				string text2 = string.Format(UI.FRONTEND.MOD_DIALOGS.MOD_ERRORS_ON_BOOT.DEV_MESSAGE, Manager.MakeEventList(this.events));
				string text3 = UI.FRONTEND.MOD_DIALOGS.RESTART.OK;
				string text4 = UI.FRONTEND.MOD_DIALOGS.RESTART.CANCEL;
				Manager.Dialog(parent, text, text2, text3, delegate
				{
					foreach (Mod mod in this.mods)
					{
						mod.enabled = false;
					}
					this.dirty = true;
					this.Update(this);
					App.instance.Restart();
				}, text4, delegate
				{
				}, null, null, null, true);
			}
			else
			{
				string text4 = UI.FRONTEND.MOD_DIALOGS.MOD_EVENTS.TITLE;
				string text3 = string.Format(UI.FRONTEND.MOD_DIALOGS.RESTART.DEV_MESSAGE, Manager.MakeEventList(this.events));
				string text2 = UI.FRONTEND.MOD_DIALOGS.RESTART.OK;
				string text = UI.FRONTEND.MOD_DIALOGS.RESTART.CANCEL;
				Manager.Dialog(parent, text4, text3, text2, delegate
				{
					App.instance.Restart();
				}, text, delegate
				{
				}, null, null, null, true);
			}
			this.events.Clear();
		}

		public void RestartDialog(string title, string message_format, global::System.Action on_cancel, bool with_details, GameObject parent, string cancel_text = null)
		{
			if (this.events.Count == 0)
			{
				return;
			}
			string text = string.Format(message_format, (!with_details) ? Manager.MakeModList(this.events) : Manager.MakeEventList(this.events));
			string text2 = UI.FRONTEND.MOD_DIALOGS.RESTART.OK;
			string text3 = cancel_text ?? UI.FRONTEND.MOD_DIALOGS.RESTART.CANCEL;
			Manager.Dialog(parent, title, text, text2, new global::System.Action(App.instance.Restart), text3, on_cancel, null, null, null, true);
			this.events.Clear();
		}

		public void NotifyDialog(string title, string message_format, GameObject parent)
		{
			if (this.events.Count == 0)
			{
				return;
			}
			Manager.Dialog(parent, title, string.Format(message_format, Manager.MakeEventList(this.events)), null, null, null, null, null, null, null, true);
			this.events.Clear();
		}

		public void HandleCrash()
		{
			global::Debug.Log("Error occurred with mods active. Disabling all mods (unless dev mods active).");
			bool flag = this.IsInDevMode();
			foreach (Mod mod in this.mods)
			{
				if (mod.enabled)
				{
					this.events.Add(new Event
					{
						event_type = EventType.ActiveDuringCrash,
						mod = mod.label
					});
					mod.Crash(!flag);
					if (!flag)
					{
						this.events.Add(new Event
						{
							event_type = EventType.Deactivated,
							mod = mod.label
						});
					}
				}
			}
			this.dirty = true;
			this.Update(this);
		}

		public void HandleErrors(List<YamlIO.Error> world_gen_errors)
		{
			string text = FileSystem.Normalize(Manager.GetDirectory());
			ListPool<Mod, Manager>.PooledList pooledList = ListPool<Mod, Manager>.Allocate();
			foreach (YamlIO.Error error in world_gen_errors)
			{
				string text2 = ((error.file.source == null) ? string.Empty : FileSystem.Normalize(error.file.source.GetRoot()));
				YamlIO.LogError(error, text2.Contains(text));
				if (error.severity != YamlIO.Error.Severity.Recoverable)
				{
					if (text2.Contains(text))
					{
						foreach (Mod mod in this.mods)
						{
							if (mod.enabled)
							{
								if (text2.Contains(mod.label.install_path))
								{
									this.events.Add(new Event
									{
										event_type = EventType.BadWorldGen,
										mod = mod.label,
										details = Path.GetFileName(error.file.full_path)
									});
									break;
								}
							}
						}
					}
				}
			}
			bool flag = this.IsInDevMode();
			foreach (Mod mod2 in pooledList)
			{
				mod2.Crash(!flag);
				if (!flag)
				{
					this.events.Add(new Event
					{
						event_type = EventType.Deactivated,
						mod = mod2.label
					});
				}
				this.dirty = true;
			}
			pooledList.Recycle();
			this.Update(this);
		}

		public void Report(GameObject parent)
		{
			if (this.events.Count == 0)
			{
				return;
			}
			for (int i = 0; i < this.events.Count; i++)
			{
				Event @event = this.events[i];
				for (int num = this.events.Count - 1; num != i; num--)
				{
					if (this.events[num].event_type == @event.event_type && this.events[num].mod.Match(@event.mod) && this.events[num].details == @event.details)
					{
						this.events.RemoveAt(num);
					}
				}
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			foreach (Event event2 in this.events)
			{
				EventType event_type = event2.event_type;
				switch (event_type)
				{
				case EventType.RestartRequested:
					flag3 = true;
					break;
				default:
					if (event_type != EventType.LoadError)
					{
						if (event_type == EventType.ActiveDuringCrash)
						{
							flag = true;
						}
					}
					else
					{
						flag2 = true;
					}
					break;
				case EventType.Deactivated:
					if ((byte)(this.FindMod(event2.mod).available_content & (Content.Strings | Content.DLL | Content.Translation | Content.Animation)) != 0)
					{
						flag3 = true;
					}
					break;
				}
			}
			flag3 = flag || flag2 || flag3;
			bool flag4 = this.IsInDevMode();
			if (flag3 && flag4)
			{
				this.DevRestartDialog(parent, flag);
			}
			else if (flag2)
			{
				this.LoadFailureDialog(parent);
			}
			else if (flag)
			{
				this.RestartDialog(UI.FRONTEND.MOD_DIALOGS.MOD_ERRORS_ON_BOOT.TITLE, UI.FRONTEND.MOD_DIALOGS.MOD_ERRORS_ON_BOOT.MESSAGE, null, false, parent, null);
			}
			else if (flag3)
			{
				this.RestartDialog(UI.FRONTEND.MOD_DIALOGS.MOD_EVENTS.TITLE, UI.FRONTEND.MOD_DIALOGS.RESTART.MESSAGE, null, true, parent, null);
			}
			else
			{
				this.NotifyDialog(UI.FRONTEND.MOD_DIALOGS.MOD_EVENTS.TITLE, (!flag4) ? UI.FRONTEND.MOD_DIALOGS.MOD_EVENTS.MESSAGE : UI.FRONTEND.MOD_DIALOGS.MOD_EVENTS.DEV_MESSAGE, parent);
			}
		}

		public bool Save()
		{
			if (!FileUtil.CreateDirectory(Manager.GetDirectory(), 5))
			{
				return false;
			}
			using (FileStream stream = FileUtil.Create(this.GetFilename(), 5))
			{
				if (stream == null)
				{
					return false;
				}
				using (StreamWriter streamWriter = FileUtil.DoIODialog<StreamWriter>(() => new StreamWriter(stream), this.GetFilename(), null, 5))
				{
					if (streamWriter == null)
					{
						return false;
					}
					string text = JsonConvert.SerializeObject(new Manager.PersistentData(this.current_version, this.mods), Formatting.Indented);
					streamWriter.Write(text);
				}
			}
			return true;
		}

		public Mod FindMod(Label label)
		{
			foreach (Mod mod in this.mods)
			{
				if (mod.label.Equals(label))
				{
					return mod;
				}
			}
			return null;
		}

		public bool IsModEnabled(Label id)
		{
			Mod mod = this.FindMod(id);
			return mod != null && mod.enabled;
		}

		public bool EnableMod(Label id, bool enabled, object caller)
		{
			Mod mod = this.FindMod(id);
			if (mod == null)
			{
				return false;
			}
			if (mod.enabled == enabled)
			{
				return false;
			}
			mod.enabled = enabled;
			if (enabled)
			{
				mod.Load(Content.LayerableFiles);
			}
			else
			{
				mod.Unload(Content.LayerableFiles);
			}
			this.dirty = true;
			this.Update(caller);
			return true;
		}

		public void Reinsert(int source_index, int target_index, object caller)
		{
			DebugUtil.Assert(source_index != target_index);
			if (source_index < -1 || this.mods.Count <= source_index)
			{
				return;
			}
			if (target_index < -1 || this.mods.Count < target_index)
			{
				return;
			}
			Mod mod = this.mods[source_index];
			this.mods.RemoveAt(source_index);
			if (source_index < target_index)
			{
				target_index--;
			}
			if (target_index == this.mods.Count)
			{
				this.mods.Add(mod);
			}
			else
			{
				this.mods.Insert(target_index, mod);
			}
			this.dirty = true;
			this.Update(caller);
		}

		public void SendMetricsEvent()
		{
			ListPool<string, Manager>.PooledList pooledList = ListPool<string, Manager>.Allocate();
			foreach (Mod mod in this.mods)
			{
				if (mod.enabled)
				{
					pooledList.Add(mod.title);
				}
			}
			DictionaryPool<string, object, Manager>.PooledDictionary pooledDictionary = DictionaryPool<string, object, Manager>.Allocate();
			pooledDictionary["ModCount"] = pooledList.Count;
			pooledDictionary["Mods"] = pooledList;
			ThreadedHttps<KleiMetrics>.Instance.SendEvent(pooledDictionary);
			pooledDictionary.Recycle();
			pooledList.Recycle();
			KCrashReporter.haveActiveMods = pooledList.Count > 0;
		}

		public const Content all_content = Content.LayerableFiles | Content.Strings | Content.DLL | Content.Translation | Content.Animation;

		public const Content boot_content = Content.Strings | Content.DLL | Content.Translation | Content.Animation;

		public const Content install_content = Content.DLL;

		public const Content on_demand_content = Content.LayerableFiles;

		public List<IDistributionPlatform> distribution_platforms = new List<IDistributionPlatform>();

		public List<Mod> mods = new List<Mod>();

		public List<Event> events = new List<Event>();

		private bool dirty = true;

		public Manager.OnUpdate on_update;

		private const int IO_OP_RETRY_COUNT = 5;

		private bool load_user_mod_loader_dll = true;

		private int current_version = 1;

		public delegate void OnUpdate(object change_source);

		private class PersistentData
		{
			public PersistentData()
			{
			}

			public PersistentData(int version, List<Mod> mods)
			{
				this.version = version;
				this.mods = mods;
			}

			public int version;

			public List<Mod> mods;
		}
	}
}
