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

	private void OnClickRead()
	{
		InfoDialogScreen infoDialogScreen = (InfoDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
		infoDialogScreen.SetHeader(Strings.Get("STRINGS.BUILDINGS.PREFABS." + base.gameObject.name.ToUpper() + ".NAME"));
		if (base.gameObject.name == "PropDesk" || base.gameObject.name == "PropFacilityDesk" || base.gameObject.name == "PropReceptionDesk")
		{
			if (!this.BeenClicked)
			{
				if (Game.Instance.unlocks.UnlockNext("emails"))
				{
					this.BeenClicked = true;
					string text = "SEARCH" + global::UnityEngine.Random.Range(1, 6);
					infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_SUCCESS." + text));
				}
				else
				{
					string text2 = "SEARCH" + global::UnityEngine.Random.Range(1, 8);
					infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_COMPUTER_FAIL." + text2));
				}
			}
			else
			{
				infoDialogScreen.AddPlainText(this.BeenSearched);
			}
		}
		else if (base.gameObject.name == "GeneShuffler" || base.gameObject.name == "MassiveHeatSink")
		{
			if (!this.BeenClicked)
			{
				if (Game.Instance.unlocks.UnlockNext("researchnotes"))
				{
					this.BeenClicked = true;
					string text3 = "SEARCH" + global::UnityEngine.Random.Range(1, 3);
					infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_TECHNOLOGY_SUCCESS." + text3));
				}
				else
				{
					string text4 = "SEARCH1";
					infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_FAIL." + text4));
				}
			}
			else
			{
				infoDialogScreen.AddPlainText(this.BeenSearched);
			}
		}
		else if (base.gameObject.name == "HeadquartersComplete")
		{
			if (!this.BeenClicked)
			{
				Game.Instance.unlocks.Unlock("pod_evacuation");
				this.BeenClicked = true;
				infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_POD);
			}
			else
			{
				infoDialogScreen.AddPlainText(this.BeenSearched);
			}
		}
		else if (base.gameObject.name == "PropFacilityDisplay")
		{
			if (!this.BeenClicked)
			{
				Game.Instance.unlocks.Unlock("display_prop1");
				this.BeenClicked = true;
				infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_DISPLAY);
			}
			else
			{
				infoDialogScreen.AddPlainText(this.BeenSearched);
			}
		}
		else if (base.gameObject.name == "PropFacilityDisplay2")
		{
			if (!this.BeenClicked)
			{
				Game.Instance.unlocks.Unlock("display_prop2");
				this.BeenClicked = true;
				infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_DISPLAY);
			}
			else
			{
				infoDialogScreen.AddPlainText(this.BeenSearched);
			}
		}
		else if (base.gameObject.name == "PropFacilityDisplay3")
		{
			if (!this.BeenClicked)
			{
				Game.Instance.unlocks.Unlock("display_prop3");
				this.BeenClicked = true;
				infoDialogScreen.AddPlainText(UI.USERMENUACTIONS.READLORE.SEARCH_DISPLAY);
			}
			else
			{
				infoDialogScreen.AddPlainText(this.BeenSearched);
			}
		}
		else if (!this.BeenClicked)
		{
			if (Game.Instance.unlocks.UnlockNext("journals"))
			{
				this.BeenClicked = true;
				string text5 = "SEARCH" + global::UnityEngine.Random.Range(1, 6);
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_SUCCESS." + text5));
			}
			else
			{
				string text6 = "SEARCH1";
				infoDialogScreen.AddPlainText(Strings.Get("STRINGS.UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_FAIL." + text6));
			}
		}
		else
		{
			infoDialogScreen.AddPlainText(this.BeenSearched);
		}
	}

	private bool BeenClicked;

	public string BeenSearched = UI.USERMENUACTIONS.READLORE.ALREADY_SEARCHED;
}
