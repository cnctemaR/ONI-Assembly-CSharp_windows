using System;
using STRINGS;
using UnityEngine;

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
		base.Subscribe(493375141, new Action<object>(this.RefreshUserMenu));
	}

	private void RefreshUserMenu(object data = null)
	{
		UserMenu userMenu = Game.Instance.userMenu;
		GameObject gameObject = base.gameObject;
		string text = "action_follow_cam";
		string text2 = UI.USERMENUACTIONS.READLORE.NAME;
		global::System.Action action = new global::System.Action(this.OnClickRead);
		string text3 = UI.USERMENUACTIONS.READLORE.TOOLTIP;
		userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true), 1f);
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
			}
			else
			{
				ManagementMenu.Instance.OpenCodexToEntry(entryForLock);
			}
		};
	}

	private void OnClickRead()
	{
		InfoDialogScreen infoDialogScreen = (InfoDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
		infoDialogScreen.SetHeader(base.gameObject.GetComponent<KSelectable>().GetProperName());
		if (this.BeenClicked)
		{
			infoDialogScreen.AddPlainText(this.BeenSearched);
			return;
		}
		this.BeenClicked = true;
		if (base.gameObject.name == "PropDesk" || base.gameObject.name == "PropFacilityDesk" || base.gameObject.name == "PropReceptionDesk")
		{
			string text = Game.Instance.unlocks.UnlockNext("emails");
			if (text != null)
			{
				string text2 = "SEARCH" + global::UnityEngine.Random.Range(1, 6);
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_SUCCESS." + text2));
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodex(text));
			}
			else
			{
				string text3 = "SEARCH" + global::UnityEngine.Random.Range(1, 8);
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_FAIL." + text3));
			}
		}
		else if (base.gameObject.name == "GeneShuffler" || base.gameObject.name == "MassiveHeatSink")
		{
			string text4 = Game.Instance.unlocks.UnlockNext("researchnotes");
			if (text4 != null)
			{
				string text5 = "SEARCH" + global::UnityEngine.Random.Range(1, 3);
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_TECHNOLOGY_SUCCESS." + text5));
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodex(text4));
			}
			else
			{
				string text6 = "SEARCH1";
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_FAIL." + text6));
			}
		}
		else if (base.gameObject.name == "HeadquartersComplete")
		{
			Game.Instance.unlocks.Unlock("pod_evacuation");
			infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_POD);
			infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodex("pod_evacuation"));
		}
		else if (base.gameObject.name == "PropFacilityDisplay")
		{
			Game.Instance.unlocks.Unlock("display_prop1");
			infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_DISPLAY);
			infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodex("display_prop1"));
		}
		else if (base.gameObject.name == "PropFacilityDisplay2")
		{
			Game.Instance.unlocks.Unlock("display_prop2");
			infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_DISPLAY);
			infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodex("display_prop2"));
		}
		else if (base.gameObject.name == "PropFacilityDisplay3")
		{
			Game.Instance.unlocks.Unlock("display_prop3");
			infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_DISPLAY);
			infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodex("display_prop3"));
		}
		else
		{
			string text7 = Game.Instance.unlocks.UnlockNext("journals");
			if (text7 != null)
			{
				string text8 = "SEARCH" + global::UnityEngine.Random.Range(1, 6);
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_SUCCESS." + text8));
				infoDialogScreen.AddOption(UI.USERMENUACTIONS.READLORE.GOTODATABASE, this.OpenCodex(text7));
			}
			else
			{
				string text9 = "SEARCH1";
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_FAIL." + text9));
			}
		}
	}

	private bool BeenClicked;

	public string BeenSearched = UI.USERMENUACTIONS.READLORE.ALREADY_SEARCHED;
}
