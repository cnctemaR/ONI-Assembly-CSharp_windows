using System;
using STRINGS;

public class LoreBearer : KMonoBehaviour
{
	public string content
	{
		get
		{
			return Strings.Get("STRINGS.LORE.BUILDINGS." + base.gameObject.name + ".ENTRY");
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.userMenu = base.GetComponent<UserMenu>();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe(493375141, new Action<object>(this.RefreshUserMenu));
	}

	private void RefreshUserMenu(object data = null)
	{
		UserMenu userMenu = this.userMenu;
		string text = "action_follow_cam";
		string text2 = UI.USERMENUACTIONS.READLORE.NAME;
		global::System.Action action = new global::System.Action(this.OnClickRead);
		string text3 = UI.USERMENUACTIONS.READLORE.TOOLTIP;
		userMenu.AddButton(new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true), 1f);
	}

	private void OnClickRead()
	{
		InfoDialogScreen infoDialogScreen = (InfoDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
		infoDialogScreen.SetHeader(Strings.Get("STRINGS.LORE.BUILDINGS." + base.gameObject.name + ".HEADER")).AddPlainText(this.content);
	}

	private UserMenu userMenu;
}
