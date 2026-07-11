using System;
using System.Collections.Generic;
using Steamworks;
using STRINGS;
using UnityEngine;

public class ModsScreen : KModalScreen, SteamUGCService.IUGCEventHandler
{
	private static void OpenDetailsPage(string key)
	{
		Application.OpenURL("https://steamcommunity.com/sharedfiles/filedetails/?id=" + key);
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		this.closeButtonTitle.onClick += this.Exit;
		this.closeButton.onClick += this.Exit;
		Global.Instance.modManager.UpdateSteamCodeModSubscriptions();
		if (SteamUGCService.Instance != null)
		{
			SteamUGCService.Instance.ugcEventHandlers.Add(this);
		}
		this.RebuildDisplay();
	}

	protected override void OnDeactivate()
	{
		if (SteamUGCService.Instance != null)
		{
			SteamUGCService.Instance.ugcEventHandlers.Remove(this);
		}
		base.OnDeactivate();
	}

	private void Exit()
	{
		if (this.triggerGameRestart)
		{
			Global.Instance.modManager.Save();
			ConfirmDialogScreen component = Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.transform.parent.gameObject, true).GetComponent<ConfirmDialogScreen>();
			component.PopupConfirmDialog(UI.FRONTEND.MODS.REQUIRES_RESTART.ToString(), new global::System.Action(App.instance.Restart), new global::System.Action(this.Deactivate), null, null, null, null, null, null);
		}
		else
		{
			this.Deactivate();
		}
	}

	private void RebuildDisplay()
	{
		foreach (RectTransform rectTransform in this.displayedMods)
		{
			if (rectTransform != null)
			{
				global::UnityEngine.Object.Destroy(rectTransform.gameObject);
			}
		}
		this.displayedMods.Clear();
		ModsScreen.ModOrderingDragListener modOrderingDragListener = new ModsScreen.ModOrderingDragListener(this, this.displayedMods);
		ICollection<ModInfo> installedMods = Global.Instance.modManager.GetInstalledMods();
		using (IEnumerator<ModInfo> enumerator2 = installedMods.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				ModInfo mod = enumerator2.Current;
				ModsScreen $this = this;
				bool flag = true;
				string text = null;
				string text2 = null;
				bool flag2 = true;
				ModInfo.Source source = mod.source;
				if (source != ModInfo.Source.Steam)
				{
					if (source == ModInfo.Source.Local)
					{
						flag = true;
						text = mod.description;
						flag2 = false;
					}
				}
				else
				{
					flag = false;
					if (SteamUGCService.Instance != null)
					{
						ulong num = ulong.Parse(mod.assetID);
						PublishedFileId_t publishedFileId_t = new PublishedFileId_t(num);
						SteamUGCService.Subscribed subscribed = SteamUGCService.Instance.GetSubscribed(publishedFileId_t);
						if (subscribed != null)
						{
							flag = true;
							text = subscribed.title;
							text2 = subscribed.description;
						}
					}
				}
				if (flag)
				{
					HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(this.entryPrefab, this.entryParent.gameObject, false);
					this.displayedMods.Add(hierarchyReferences.gameObject.GetComponent<RectTransform>());
					DragMe component = hierarchyReferences.GetComponent<DragMe>();
					component.listener = modOrderingDragListener;
					LocText reference = hierarchyReferences.GetReference<LocText>("Title");
					reference.text = text;
					if (text2 != null)
					{
						reference.GetComponent<ToolTip>().toolTip = text2;
					}
					KButton reference2 = hierarchyReferences.GetReference<KButton>("ManageButton");
					reference2.isInteractable = flag2;
					reference2.GetComponent<ToolTip>().toolTip = UI.FRONTEND.MODS.TOOLTIPS.MANAGE_STEAM_SUBSCRIPTION;
					reference2.onClick += delegate
					{
						Application.OpenURL("https://steamcommunity.com/sharedfiles/filedetails/?id=" + mod.assetID);
					};
					MultiToggle toggle = hierarchyReferences.GetReference<MultiToggle>("EnabledToggle");
					toggle.ChangeState((!mod.enabled) ? 0 : 1);
					MultiToggle toggle2 = toggle;
					toggle2.onClick = (global::System.Action)Delegate.Combine(toggle2.onClick, new global::System.Action(delegate
					{
						$this.OnToggleClicked(toggle, mod);
					}));
					toggle.GetComponent<ToolTip>().OnToolTip = () => (!Global.Instance.modManager.IsModEnabled(mod)) ? UI.FRONTEND.MODS.TOOLTIPS.DISABLED : UI.FRONTEND.MODS.TOOLTIPS.ENABLED;
					hierarchyReferences.gameObject.SetActive(true);
				}
				else
				{
					Global.Instance.modManager.UninstallMod(mod);
				}
			}
		}
		foreach (RectTransform rectTransform2 in this.displayedMods)
		{
			rectTransform2.gameObject.SetActive(true);
		}
	}

	public void OnUGCItemInstalled(ItemInstalled_t pCallback)
	{
	}

	public void OnUGCItemUpdated(RemoteStoragePublishedFileUpdated_t pCallback)
	{
	}

	public void OnUGCItemUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t pCallback)
	{
		string text = pCallback.m_nPublishedFileId.m_PublishedFileId.ToString();
		ModInfo modInfo = new ModInfo(ModInfo.Source.Steam, ModInfo.ModType.Mod, text, "UNSUBSCRIBED", string.Empty, 0UL);
		Global.Instance.modManager.UninstallMod(modInfo);
	}

	public void OnUGCItemDownloaded(DownloadItemResult_t pCallback)
	{
	}

	public void OnUGCRefresh()
	{
		this.RebuildDisplay();
	}

	private void OnToggleClicked(MultiToggle toggle, ModInfo info)
	{
		ModManager modManager = Global.Instance.modManager;
		bool flag = modManager.IsModEnabled(info);
		flag = !flag;
		toggle.ChangeState((!flag) ? 0 : 1);
		if (flag)
		{
			modManager.EnableMod(info);
		}
		else
		{
			modManager.DisableMod(info);
		}
		this.triggerGameRestart = true;
	}

	public const string TAG_MOD = "mod";

	[SerializeField]
	private KButton closeButtonTitle;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private GameObject entryPrefab;

	[SerializeField]
	private Transform entryParent;

	private List<RectTransform> displayedMods = new List<RectTransform>();

	private bool triggerGameRestart;

	private class ModOrderingDragListener : DragMe.IDragListener
	{
		public ModOrderingDragListener(ModsScreen screen, List<RectTransform> displayed_items)
		{
			this.screen = screen;
			this.rectTransforms = displayed_items;
		}

		public void OnBeginDrag(Vector2 pos)
		{
			this.startDragIdx = this.GetDragIdx(pos);
		}

		public void OnEndDrag(Vector2 pos)
		{
			if (this.startDragIdx >= 0)
			{
				int dragIdx = this.GetDragIdx(pos);
				if (dragIdx >= 0 && dragIdx != this.startDragIdx)
				{
					Global.Instance.modManager.Reorder(this.startDragIdx, dragIdx);
					this.screen.RebuildDisplay();
				}
			}
		}

		private int GetDragIdx(Vector2 pos)
		{
			int num = -1;
			for (int i = 0; i < this.rectTransforms.Count; i++)
			{
				if (RectTransformUtility.RectangleContainsScreenPoint(this.rectTransforms[i], pos))
				{
					num = i;
					break;
				}
			}
			return num;
		}

		private List<RectTransform> rectTransforms;

		private ModsScreen screen;

		private int startDragIdx = -1;
	}
}
