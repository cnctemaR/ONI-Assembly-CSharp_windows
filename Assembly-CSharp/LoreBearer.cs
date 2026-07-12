using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LoreBearer")]
public class LoreBearer : KMonoBehaviour, ISidescreenButtonControl
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
	}

	private Action<InfoDialogScreen> OpenCodexByLockKeyID(string key)
	{
		return delegate(InfoDialogScreen dialog)
		{
			dialog.Deactivate();
			string entryForLock = CodexCache.GetEntryForLock(key);
			if (entryForLock == null)
			{
				DebugUtil.DevLogError("Missing codex entry for lock: " + key);
				return;
			}
			ManagementMenu.Instance.OpenCodexToEntry(entryForLock);
		};
	}

	private void OnClickRead()
	{
		InfoDialogScreen infoDialogScreen = (InfoDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
		infoDialogScreen.SetHeader(base.gameObject.GetComponent<KSelectable>().GetProperName()).AddDefaultOK(true);
		if (this.BeenClicked)
		{
			infoDialogScreen.AddPlainText(this.BeenSearched);
			return;
		}
		this.BeenClicked = true;
		if (base.gameObject.name == "GeneShuffler")
		{
			Game.Instance.unlocks.Unlock("neuralvacillator");
		}
		if (base.gameObject.name == "PropDesk")
		{
			string text = Game.Instance.unlocks.UnlockNext("emails");
			if (text != null)
			{
				string text2 = "SEARCH" + global::UnityEngine.Random.Range(1, 6).ToString();
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_SUCCESS." + text2));
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodexByLockKeyID(text), false);
				return;
			}
			string text3 = "SEARCH" + global::UnityEngine.Random.Range(1, 8).ToString();
			infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_FAIL." + text3));
			return;
		}
		else if (base.gameObject.name == "GeneShuffler" || base.gameObject.name == "MassiveHeatSink")
		{
			string text4 = Game.Instance.unlocks.UnlockNext("researchnotes");
			if (text4 != null)
			{
				string text5 = "SEARCH" + global::UnityEngine.Random.Range(1, 3).ToString();
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_TECHNOLOGY_SUCCESS." + text5));
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodexByLockKeyID(text4), false);
				return;
			}
			string text6 = "SEARCH1";
			infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_FAIL." + text6));
			return;
		}
		else if (base.gameObject.GetProperName() == BUILDINGS.PREFABS.PROPGRAVITASJAR1.NAME || base.gameObject.name == BUILDINGS.PREFABS.PROPGRAVITASJAR2.NAME || base.gameObject.name == BUILDINGS.PREFABS.PROPGRAVITASDISPLAY4.NAME)
		{
			string text7 = Game.Instance.unlocks.UnlockNext("dimensionallore");
			if (text7 != null)
			{
				string text8 = "SEARCH" + global::UnityEngine.Random.Range(1, 6).ToString();
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_SUCCESS." + text8));
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodexByLockKeyID(text7), false);
				return;
			}
			string text9 = "SEARCH1";
			infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_FAIL." + text9));
			return;
		}
		else
		{
			if (base.gameObject.name == "PropReceptionDesk")
			{
				Game.Instance.unlocks.Unlock("email_pens");
				infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_ELLIESDESK);
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodexByLockKeyID("email_pens"), false);
				return;
			}
			if (base.gameObject.name == "PropFacilityDesk")
			{
				Game.Instance.unlocks.Unlock("journal_magazine");
				infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_STERNSDESK);
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodexByLockKeyID("journal_magazine"), false);
				return;
			}
			if (base.gameObject.name == "HeadquartersComplete")
			{
				Game.Instance.unlocks.Unlock("pod_evacuation");
				infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_POD);
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodexByLockKeyID("pod_evacuation"), false);
				return;
			}
			if (base.gameObject.name == "PropFacilityDisplay")
			{
				Game.Instance.unlocks.Unlock("display_prop1");
				infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_DISPLAY);
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodexByLockKeyID("display_prop1"), false);
				return;
			}
			if (base.gameObject.name == "PropFacilityDisplay2")
			{
				Game.Instance.unlocks.Unlock("display_prop2");
				infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_DISPLAY);
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodexByLockKeyID("display_prop2"), false);
				return;
			}
			if (base.gameObject.name == "PropFacilityDisplay3")
			{
				Game.Instance.unlocks.Unlock("display_prop3");
				infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_DISPLAY);
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodexByLockKeyID("display_prop3"), false);
				return;
			}
			if (base.gameObject.name == "PropFacilityGlobeDroors")
			{
				Game.Instance.unlocks.Unlock("journal_newspaper");
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_CABINET"));
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodexByLockKeyID("journal_newspaper"), false);
				return;
			}
			if (base.gameObject.GetProperName() == BUILDINGS.PREFABS.WARPRECEIVER.NAME)
			{
				Game.Instance.unlocks.Unlock("notes_AI");
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_TELEPORTER_RECEIVER"));
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodexByLockKeyID("notes_AI"), false);
				return;
			}
			if (base.gameObject.GetProperName() == BUILDINGS.PREFABS.WARPPORTAL.NAME)
			{
				Game.Instance.unlocks.Unlock("notes_teleportation");
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_TELEPORTER_SENDER"));
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodexByLockKeyID("notes_teleportation"), false);
				return;
			}
			if (base.gameObject.GetProperName() == BUILDINGS.PREFABS.CRYOTANK.NAME)
			{
				Game.Instance.unlocks.Unlock("cryotank_warning");
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_CRYO_TANK"));
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodexByLockKeyID("cryotank_warning"), false);
				return;
			}
			if (base.gameObject.name.Contains("ArtifactSpacePOI"))
			{
				string text10 = Game.Instance.unlocks.UnlockNext("space");
				if (text10 != null)
				{
					string text11 = "SEARCH" + global::UnityEngine.Random.Range(1, 7).ToString();
					infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_SPACEPOI_SUCCESS." + text11));
					infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodexByLockKeyID(text10), false);
					return;
				}
				string text12 = "SEARCH" + global::UnityEngine.Random.Range(1, 4).ToString();
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_SPACEPOI_FAIL." + text12));
				return;
			}
			else
			{
				string text13 = Game.Instance.unlocks.UnlockNext("journals");
				if (text13 != null)
				{
					string text14 = "SEARCH" + global::UnityEngine.Random.Range(1, 6).ToString();
					infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_SUCCESS." + text14));
					infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodexByLockKeyID(text13), false);
					return;
				}
				string text15 = "SEARCH1";
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_FAIL." + text15));
				return;
			}
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

	public bool SidescreenEnabled()
	{
		return true;
	}

	public void OnSidescreenButtonPressed()
	{
		this.OnClickRead();
	}

	public bool SidescreenButtonInteractable()
	{
		return !this.BeenClicked;
	}

	public int ButtonSideScreenSortOrder()
	{
		return 20;
	}

	[Serialize]
	private bool BeenClicked;

	public string BeenSearched = UI.USERMENUACTIONS.READLORE.ALREADY_SEARCHED;
}
