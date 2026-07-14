using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LoreBearer")]
public class LoreBearer : KMonoBehaviour
{
	public string content
	{
		get
		{
			return Strings.Get("STRINGS.LORE.BUILDINGS." + base.gameObject.name + ".ENTRY");
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.displayContentAction == null && !string.IsNullOrEmpty(this.poiOverrideLoreUnlockId))
		{
			if (!string.IsNullOrEmpty(this.poiOverrideNextCollectionId))
			{
				this.displayContentAction = LoreBearerUtil.UnlockSpecificEntryThenNext(this.poiOverrideLoreUnlockId, Strings.Get(this.poiOverrideLoreDisplayText), LoreBearerUtil.GetUnlockActionForCollection(this.poiOverrideNextCollectionId), false);
				return;
			}
			this.displayContentAction = LoreBearerUtil.UnlockSpecificEntry(this.poiOverrideLoreUnlockId, Strings.Get(this.poiOverrideLoreDisplayText), false);
		}
	}

	public LoreBearer Internal_SetContent(LoreBearerAction action)
	{
		this.displayContentAction = action;
		return this;
	}

	public LoreBearer Internal_SetContent(LoreBearerAction action, string[] collectionsToUnlockFrom)
	{
		this.displayContentAction = action;
		this.collectionsToUnlockFrom = collectionsToUnlockFrom;
		return this;
	}

	public static InfoDialogScreen ShowPopupDialog()
	{
		return (InfoDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
	}

	private void OnClickRead()
	{
		InfoDialogScreen infoDialogScreen = LoreBearer.ShowPopupDialog().SetHeader(base.gameObject.GetComponent<KSelectable>().GetProperName()).AddDefaultOK(true);
		if (this.BeenClicked)
		{
			infoDialogScreen.AddPlainText(this.BeenSearched);
			return;
		}
		this.BeenClicked = true;
		if (DlcManager.IsExpansion1Active())
		{
			Scenario.SpawnPrefab(Grid.PosToCell(base.gameObject), 0, 1, "OrbitalResearchDatabank", Grid.SceneLayer.Front).SetActive(true);
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, Assets.GetPrefab("OrbitalResearchDatabank".ToTag()).GetProperName(), base.gameObject.transform, 1.5f, false);
		}
		if (this.displayContentAction != null)
		{
			this.displayContentAction(infoDialogScreen);
			return;
		}
		if (this.useDefaultLore)
		{
			LoreBearerUtil.UnlockNextJournalEntry(infoDialogScreen);
		}
	}

	public string SidescreenButtonText
	{
		get
		{
			return this.BeenClicked ? UI.USERMENUACTIONS.READLORE.ALREADYINSPECTED : UI.USERMENUACTIONS.READLORE.NAME;
		}
	}

	public string SidescreenButtonTooltip
	{
		get
		{
			return this.BeenClicked ? UI.USERMENUACTIONS.READLORE.TOOLTIP_ALREADYINSPECTED : UI.USERMENUACTIONS.READLORE.TOOLTIP;
		}
	}

	public void OnSidescreenButtonPressed()
	{
		this.OnClickRead();
	}

	public bool SidescreenButtonInteractable()
	{
		return !this.BeenClicked;
	}

	public int GetSideScreenSortOrder()
	{
		if (this.GetSidescreenSortOrder != null)
		{
			return this.GetSidescreenSortOrder();
		}
		return -100;
	}

	public void Debug_ResetSearched()
	{
		this.BeenClicked = false;
	}

	[Serialize]
	private bool BeenClicked;

	public string BeenSearched = UI.USERMENUACTIONS.READLORE.ALREADY_SEARCHED;

	private string[] collectionsToUnlockFrom;

	public Func<int> GetSidescreenSortOrder;

	[Serialize]
	public string poiOverrideLoreUnlockId;

	[Serialize]
	public string poiOverrideLoreDisplayText;

	[Serialize]
	public string poiOverrideNextCollectionId;

	[Tooltip("Controls if the lore should be active. The Inspect button will also disappear if set to true.")]
	public bool hideLore;

	public bool useDefaultLore = true;

	private LoreBearerAction displayContentAction;
}
