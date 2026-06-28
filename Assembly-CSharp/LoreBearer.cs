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
		this.Subscribe(493375141, new Action<object>(this.RefreshUserMenu));
	}

	private void RefreshUserMenu(object data = null)
	{
		UserMenu userMenu = this.userMenu;
		string text = UI.USERMENUACTIONS.READLORE.TOOLTIP;
		userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_follow_cam", UI.USERMENUACTIONS.READLORE.NAME, new global::System.Action(this.OnClickRead), global::Action.NumActions, null, null, null, text, true), 1f);
	}

	private void OnClickRead()
	{
		LoreDialogScreen loreDialogScreen = (LoreDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.LoreDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
		loreDialogScreen.PopupLoreDialog(this.content, Strings.Get("STRINGS.LORE.BUILDINGS." + base.gameObject.name + ".HEADER"), null);
	}

	private UserMenu userMenu;
}
