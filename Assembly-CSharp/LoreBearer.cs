using System;
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
		base.Subscribe<LoreBearer>(493375141, LoreBearer.RefreshUserMenuDelegate);
	}

	private void RefreshUserMenu(object data = null)
	{
		Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("action_follow_cam", UI.USERMENUACTIONS.READLORE.NAME, new global::System.Action(this.OnClickRead), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.READLORE.TOOLTIP, true), 1f);
	}

	private Action<InfoDialogScreen> OpenCodex(string key)
	{
		return delegate(InfoDialogScreen dialog)
		{
			dialog.Deactivate();
			string entryForLock = CodexCache.GetEntryForLock(key);
			if (entryForLock == null)
			{
				KCrashReporter.Assert(false, "Missing codex entry: " + key);
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
				string text2 = "SEARCH" + global::UnityEngine.Random.Range(1, 6);
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_SUCCESS." + text2));
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodex(text), false);
				return;
			}
			string text3 = "SEARCH" + global::UnityEngine.Random.Range(1, 8);
			infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_FAIL." + text3));
			return;
		}
		else if (base.gameObject.name == "GeneShuffler" || base.gameObject.name == "MassiveHeatSink")
		{
			string text4 = Game.Instance.unlocks.UnlockNext("researchnotes");
			if (text4 != null)
			{
				string text5 = "SEARCH" + global::UnityEngine.Random.Range(1, 3);
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_TECHNOLOGY_SUCCESS." + text5));
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodex(text4), false);
				return;
			}
			string text6 = "SEARCH1";
			infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_FAIL." + text6));
			return;
		}
		else
		{
			if (base.gameObject.name == "PropReceptionDesk")
			{
				Game.Instance.unlocks.Unlock("email_pens");
				infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_ELLIESDESK);
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodex("email_pens"), false);
				return;
			}
			if (base.gameObject.name == "PropFacilityDesk")
			{
				Game.Instance.unlocks.Unlock("journal_magazine");
				infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_STERNSDESK);
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodex("journal_magazine"), false);
				return;
			}
			if (base.gameObject.name == "HeadquartersComplete")
			{
				Game.Instance.unlocks.Unlock("pod_evacuation");
				infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_POD);
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodex("pod_evacuation"), false);
				return;
			}
			if (base.gameObject.name == "PropFacilityDisplay")
			{
				Game.Instance.unlocks.Unlock("display_prop1");
				infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_DISPLAY);
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodex("display_prop1"), false);
				return;
			}
			if (base.gameObject.name == "PropFacilityDisplay2")
			{
				Game.Instance.unlocks.Unlock("display_prop2");
				infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_DISPLAY);
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodex("display_prop2"), false);
				return;
			}
			if (base.gameObject.name == "PropFacilityDisplay3")
			{
				Game.Instance.unlocks.Unlock("display_prop3");
				infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_DISPLAY);
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodex("display_prop3"), false);
				return;
			}
			if (base.gameObject.name == "PropFacilityGlobeDroors")
			{
				Game.Instance.unlocks.Unlock("journal_newspaper");
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_CABINET"));
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodex("journal_newspaper"), false);
				return;
			}
			string text7 = Game.Instance.unlocks.UnlockNext("journals");
			if (text7 != null)
			{
				string text8 = "SEARCH" + global::UnityEngine.Random.Range(1, 6);
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_SUCCESS." + text8));
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodex(text7), false);
				return;
			}
			string text9 = "SEARCH1";
			infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_FAIL." + text9));
			return;
		}
	}

	private bool BeenClicked;

	public string BeenSearched = UI.USERMENUACTIONS.READLORE.ALREADY_SEARCHED;

	private static readonly EventSystem.IntraObjectHandler<LoreBearer> RefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<LoreBearer>(delegate(LoreBearer component, object data)
	{
		component.RefreshUserMenu(data);
	});
}
