using System;
using Steamworks;
using STRINGS;
using UnityEngine;

namespace KMod
{
	public class Steam : IDistributionPlatform, SteamUGCService.IUGCEventHandler
	{
		public Steam()
		{
			this.UpdateSubscriptions();
		}

		private Mod MakeMod(SteamUGCService.Subscribed subscribed)
		{
			if (subscribed == null)
			{
				return null;
			}
			if ((SteamUGC.GetItemState(subscribed.fileId) & 4U) == 0U)
			{
				return null;
			}
			string id = subscribed.fileId.m_PublishedFileId.ToString();
			Label label = new Label
			{
				id = id,
				distribution_platform = Label.DistributionPlatform.Steam,
				version = subscribed.lastUpdateTime,
				title = subscribed.title
			};
			ulong num;
			string text;
			uint num2;
			if (!SteamUGC.GetItemInstallInfo(subscribed.fileId, out num, out text, 1024U, out num2))
			{
				Global.Instance.modManager.events.Add(new Event
				{
					event_type = EventType.InstallInfoInaccessible,
					mod = label
				});
				return null;
			}
			return new Mod(label, subscribed.description, new ZipFile(text), UI.FRONTEND.MODS.TOOLTIPS.MANAGE_STEAM_SUBSCRIPTION, delegate
			{
				Application.OpenURL("https://steamcommunity.com/sharedfiles/filedetails/?id=" + id);
			});
		}

		private void UpdateSubscriptions()
		{
			foreach (SteamUGCService.Subscribed subscribed in SteamUGCService.Instance.GetSubscribed())
			{
				Mod mod = this.MakeMod(subscribed);
				if (mod != null)
				{
					Global.Instance.modManager.Subscribe(mod, this);
				}
			}
		}

		public void OnUGCItemSubscribed(RemoteStoragePublishedFileSubscribed_t pCallback)
		{
			Mod mod = this.MakeMod(SteamUGCService.Instance.GetSubscribed(pCallback.m_nPublishedFileId));
			if (mod == null)
			{
				return;
			}
			Global.Instance.modManager.Subscribe(mod, this);
			Global.Instance.modManager.NotifyDialog(UI.FRONTEND.MOD_DIALOGS.STEAM_SUBSCRIBED.TITLE, UI.FRONTEND.MOD_DIALOGS.STEAM_SUBSCRIBED.MESSAGE, null);
		}

		public void OnUGCItemInstalled(ItemInstalled_t pCallback)
		{
		}

		public void OnUGCItemUpdated(RemoteStoragePublishedFileUpdated_t pCallback)
		{
			Mod mod = this.MakeMod(SteamUGCService.Instance.GetSubscribed(pCallback.m_nPublishedFileId));
			if (mod == null)
			{
				return;
			}
			Global.Instance.modManager.Subscribe(mod, this);
			Global.Instance.modManager.NotifyDialog(UI.FRONTEND.MOD_DIALOGS.STEAM_UPDATED.TITLE, UI.FRONTEND.MOD_DIALOGS.STEAM_UPDATED.MESSAGE, null);
		}

		public void OnUGCItemUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t pCallback)
		{
			Mod mod = this.MakeMod(SteamUGCService.Instance.GetSubscribed(pCallback.m_nPublishedFileId));
			if (mod == null)
			{
				return;
			}
			Global.Instance.modManager.Unsubscribe(mod.label, this);
			Global.Instance.modManager.NotifyDialog(UI.FRONTEND.MOD_DIALOGS.STEAM_UNSUBSCRIBED.TITLE, UI.FRONTEND.MOD_DIALOGS.STEAM_UNSUBSCRIBED.MESSAGE, null);
		}

		public void OnUGCItemDownloaded(DownloadItemResult_t pCallback)
		{
		}

		public void OnUGCRefresh()
		{
			this.UpdateSubscriptions();
			Global.Instance.modManager.NotifyDialog(UI.FRONTEND.MOD_DIALOGS.STEAM_REFRESH.TITLE, UI.FRONTEND.MOD_DIALOGS.STEAM_REFRESH.MESSAGE, null);
		}
	}
}
