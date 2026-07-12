using System;
using STRINGS;
using UnityEngine;

public class DLCToggle : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.expansion1Active = DlcManager.IsExpansion1Active();
	}

	public void ToggleExpansion1Cicked()
	{
		Util.KInstantiateUI<InfoDialogScreen>(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, base.GetComponentInParent<Canvas>().gameObject, true).AddDefaultCancel().SetHeader(this.expansion1Active ? UI.FRONTEND.MAINMENU.DLC.DEACTIVATE_EXPANSION1 : UI.FRONTEND.MAINMENU.DLC.ACTIVATE_EXPANSION1)
			.AddSprite(this.expansion1Active ? GlobalResources.Instance().baseGameLogoSmall : GlobalResources.Instance().expansion1LogoSmall)
			.AddPlainText(this.expansion1Active ? UI.FRONTEND.MAINMENU.DLC.DEACTIVATE_EXPANSION1_DESC : UI.FRONTEND.MAINMENU.DLC.ACTIVATE_EXPANSION1_DESC)
			.AddOption(UI.CONFIRMDIALOG.OK, delegate(InfoDialogScreen screen)
			{
				DlcManager.ToggleDLC("EXPANSION1_ID");
			}, true);
	}

	private bool expansion1Active;
}
