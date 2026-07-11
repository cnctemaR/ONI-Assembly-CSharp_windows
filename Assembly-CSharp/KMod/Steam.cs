using System;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using STRINGS;
using UnityEngine;

namespace KMod
{
	public class Steam : IDistributionPlatform, SteamUGCService.IClient
	{
		private Mod MakeMod(SteamUGCService.Mod subscribed)
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
				version = (long)subscribed.lastUpdateTime,
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

		public void UpdateMods(IEnumerable<PublishedFileId_t> added, IEnumerable<PublishedFileId_t> updated, IEnumerable<PublishedFileId_t> removed, IEnumerable<SteamUGCService.Mod> loaded_previews)
		{
			foreach (PublishedFileId_t publishedFileId_t in added)
			{
				SteamUGCService.Mod mod = SteamUGCService.Instance.FindMod(publishedFileId_t);
				if (mod == null)
				{
					DebugUtil.DevAssert(false, "SteamUGCService just told us this id was valid!");
				}
				else
				{
					Mod mod2 = this.MakeMod(mod);
					if (mod2 != null)
					{
						Global.Instance.modManager.Subscribe(mod2, this);
					}
				}
			}
			foreach (PublishedFileId_t publishedFileId_t2 in updated)
			{
				SteamUGCService.Mod mod3 = SteamUGCService.Instance.FindMod(publishedFileId_t2);
				if (mod3 == null)
				{
					DebugUtil.DevAssert(false, "SteamUGCService just told us this id was valid!");
				}
				else
				{
					Mod mod4 = this.MakeMod(mod3);
					if (mod4 != null)
					{
						Global.Instance.modManager.Update(mod4, this);
					}
				}
			}
			foreach (PublishedFileId_t publishedFileId_t3 in removed)
			{
				Global.Instance.modManager.Unsubscribe(new Label
				{
					id = publishedFileId_t3.m_PublishedFileId.ToString(),
					distribution_platform = Label.DistributionPlatform.Steam
				}, this);
			}
			if (added.Count<PublishedFileId_t>() != 0)
			{
				Global.Instance.modManager.Sanitize(null);
			}
			else
			{
				Global.Instance.modManager.Report(null);
			}
		}
	}
}
