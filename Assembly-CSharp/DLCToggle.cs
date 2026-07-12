using System;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class DLCToggle : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.expansion1Active = DistributionPlatform.Inst.IsExpansion1Active;
		this.button.onClick += this.ToggleExpansion1Cicked;
		this.label.text = (this.expansion1Active ? UI.FRONTEND.MAINMENU.DLC.DEACTIVATE_EXPANSION1 : UI.FRONTEND.MAINMENU.DLC.ACTIVATE_EXPANSION1);
		this.logo.sprite = (this.expansion1Active ? GlobalResources.Instance().baseGameLogoSmall : GlobalResources.Instance().expansion1LogoSmall);
	}

	private void ToggleExpansion1Cicked()
	{
		Util.KInstantiateUI<InfoDialogScreen>(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, base.GetComponentInParent<Canvas>().gameObject, true).AddDefaultCancel().SetHeader(this.expansion1Active ? UI.FRONTEND.MAINMENU.DLC.DEACTIVATE_EXPANSION1 : UI.FRONTEND.MAINMENU.DLC.ACTIVATE_EXPANSION1)
			.AddSprite(this.expansion1Active ? GlobalResources.Instance().baseGameLogoSmall : GlobalResources.Instance().expansion1LogoSmall)
			.AddPlainText(this.expansion1Active ? UI.FRONTEND.MAINMENU.DLC.DEACTIVATE_EXPANSION1_DESC : UI.FRONTEND.MAINMENU.DLC.ACTIVATE_EXPANSION1_DESC)
			.AddOption(UI.CONFIRMDIALOG.OK, delegate(InfoDialogScreen screen)
			{
				KPlayerPrefs.SetInt("EXPANSION1_ID.ENABLED", 1);
				DistributionPlatform.Inst.ToggleDLC();
			}, true);
	}

	[SerializeField]
	private KButton button;

	[SerializeField]
	private LocText label;

	[SerializeField]
	private Image logo;

	private bool expansion1Active;
}
