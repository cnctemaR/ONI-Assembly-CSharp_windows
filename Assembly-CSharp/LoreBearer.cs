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

	public static Action<InfoDialogScreen> OpenCodexByLockKeyID(string key)
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

	public static Action<InfoDialogScreen> OpenCodexByEntryID(string id)
	{
		return delegate(InfoDialogScreen dialog)
		{
			dialog.Deactivate();
			ManagementMenu.Instance.OpenCodexToEntry(id);
		};
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
		if (base.gameObject.name == "GeneShuffler")
		{
			Game.Instance.unlocks.Unlock("neuralvacillator", true);
		}
		if (base.gameObject.name == "PropDesk")
		{
			string text = Game.Instance.unlocks.UnlockNext("emails");
			if (text != null)
			{
				string text2 = "SEARCH" + global::UnityEngine.Random.Range(1, 6).ToString();
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_SUCCESS." + text2));
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearer.OpenCodexByLockKeyID(text), false);
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
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearer.OpenCodexByLockKeyID(text4), false);
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
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearer.OpenCodexByLockKeyID(text7), false);
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
				Game.Instance.unlocks.Unlock("email_pens", true);
				infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_ELLIESDESK);
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearer.OpenCodexByLockKeyID("email_pens"), false);
				return;
			}
			if (base.gameObject.name == "PropGravitasDeskPodium")
			{
				if (!Game.Instance.unlocks.IsUnlocked("story_trait_critter_manipulator_parking"))
				{
					Game.Instance.unlocks.Unlock("story_trait_critter_manipulator_parking", true);
					string text10 = "SEARCH" + global::UnityEngine.Random.Range(1, 1).ToString();
					infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_PODIUM." + text10));
					infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearer.OpenCodexByLockKeyID("story_trait_critter_manipulator_parking"), false);
					return;
				}
				string text11 = "SEARCH" + global::UnityEngine.Random.Range(1, 8).ToString();
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_FAIL." + text11));
				return;
			}
			else
			{
				if (base.gameObject.name == "PropFacilityDesk")
				{
					Game.Instance.unlocks.Unlock("journal_magazine", true);
					infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_STERNSDESK);
					infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearer.OpenCodexByLockKeyID("journal_magazine"), false);
					return;
				}
				if (base.gameObject.name == "HeadquartersComplete")
				{
					Game.Instance.unlocks.Unlock("pod_evacuation", true);
					infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_POD);
					infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearer.OpenCodexByLockKeyID("pod_evacuation"), false);
					return;
				}
				if (base.gameObject.name == "PropFacilityDisplay")
				{
					Game.Instance.unlocks.Unlock("display_prop1", true);
					infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_DISPLAY);
					infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearer.OpenCodexByLockKeyID("display_prop1"), false);
					return;
				}
				if (base.gameObject.name == "PropFacilityDisplay2")
				{
					Game.Instance.unlocks.Unlock("display_prop2", true);
					infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_DISPLAY);
					infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearer.OpenCodexByLockKeyID("display_prop2"), false);
					return;
				}
				if (base.gameObject.name == "PropFacilityDisplay3")
				{
					Game.Instance.unlocks.Unlock("display_prop3", true);
					infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_DISPLAY);
					infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearer.OpenCodexByLockKeyID("display_prop3"), false);
					return;
				}
				if (base.gameObject.name == "PropFacilityGlobeDroors")
				{
					Game.Instance.unlocks.Unlock("journal_newspaper", true);
					infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_CABINET"));
					infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearer.OpenCodexByLockKeyID("journal_newspaper"), false);
					return;
				}
				if (base.gameObject.GetProperName() == BUILDINGS.PREFABS.WARPRECEIVER.NAME)
				{
					Game.Instance.unlocks.Unlock("notes_AI", true);
					infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_TELEPORTER_RECEIVER"));
					infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearer.OpenCodexByLockKeyID("notes_AI"), false);
					return;
				}
				if (base.gameObject.GetProperName() == BUILDINGS.PREFABS.WARPPORTAL.NAME)
				{
					Game.Instance.unlocks.Unlock("notes_teleportation", true);
					infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_TELEPORTER_SENDER"));
					infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearer.OpenCodexByLockKeyID("notes_teleportation"), false);
					return;
				}
				if (base.gameObject.GetProperName() == BUILDINGS.PREFABS.CRYOTANK.NAME)
				{
					Game.Instance.unlocks.Unlock("cryotank_warning", true);
					infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_CRYO_TANK"));
					infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearer.OpenCodexByLockKeyID("cryotank_warning"), false);
					return;
				}
				if (base.gameObject.name.Contains("ArtifactSpacePOI"))
				{
					string text12 = Game.Instance.unlocks.UnlockNext("space");
					if (text12 != null)
					{
						string text13 = "SEARCH" + global::UnityEngine.Random.Range(1, 7).ToString();
						infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_SPACEPOI_SUCCESS." + text13));
						infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearer.OpenCodexByLockKeyID(text12), false);
						return;
					}
					string text14 = "SEARCH" + global::UnityEngine.Random.Range(1, 4).ToString();
					infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_SPACEPOI_FAIL." + text14));
					return;
				}
				else
				{
					if (base.gameObject.GetProperName() == BUILDINGS.PREFABS.PROPGRAVITASCREATUREPOSTER.NAME)
					{
						Game.Instance.unlocks.Unlock("storytrait_crittermanipulator_workiversary", true);
						infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_PROPGRAVITASCREATUREPOSTER"));
						infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearer.OpenCodexByLockKeyID("storytrait_crittermanipulator_workiversary"), false);
						return;
					}
					string text15 = Game.Instance.unlocks.UnlockNext("journals");
					if (text15 != null)
					{
						string text16 = "SEARCH" + global::UnityEngine.Random.Range(1, 6).ToString();
						infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_SUCCESS." + text16));
						infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, LoreBearer.OpenCodexByLockKeyID(text15), false);
						return;
					}
					string text17 = "SEARCH1";
					infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_FAIL." + text17));
					return;
				}
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
