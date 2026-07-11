using System;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ModeSelectScreen : KScreen
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		HierarchyReferences component = this.survivalButton.GetComponent<HierarchyReferences>();
		this.survivalButtonHeader = component.GetReference<RectTransform>("HeaderBackground").GetComponent<Image>();
		this.survivalButtonSelectionFrame = component.GetReference<RectTransform>("SelectionFrame").GetComponent<Image>();
		MultiToggle multiToggle = this.survivalButton;
		multiToggle.onEnter = (global::System.Action)Delegate.Combine(multiToggle.onEnter, new global::System.Action(this.OnHoverEnterSurvival));
		MultiToggle multiToggle2 = this.survivalButton;
		multiToggle2.onExit = (global::System.Action)Delegate.Combine(multiToggle2.onExit, new global::System.Action(this.OnHoverExitSurvival));
		MultiToggle multiToggle3 = this.survivalButton;
		multiToggle3.onClick = (global::System.Action)Delegate.Combine(multiToggle3.onClick, new global::System.Action(this.OnClickSurvival));
		HierarchyReferences component2 = this.nosweatButton.GetComponent<HierarchyReferences>();
		this.nosweatButtonHeader = component2.GetReference<RectTransform>("HeaderBackground").GetComponent<Image>();
		this.nosweatButtonSelectionFrame = component2.GetReference<RectTransform>("SelectionFrame").GetComponent<Image>();
		MultiToggle multiToggle4 = this.nosweatButton;
		multiToggle4.onEnter = (global::System.Action)Delegate.Combine(multiToggle4.onEnter, new global::System.Action(this.OnHoverEnterNosweat));
		MultiToggle multiToggle5 = this.nosweatButton;
		multiToggle5.onExit = (global::System.Action)Delegate.Combine(multiToggle5.onExit, new global::System.Action(this.OnHoverExitNosweat));
		MultiToggle multiToggle6 = this.nosweatButton;
		multiToggle6.onClick = (global::System.Action)Delegate.Combine(multiToggle6.onClick, new global::System.Action(this.OnClickNosweat));
		this.closeButton.onClick += this.Deactivate;
	}

	private void OnHoverEnterSurvival()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover", false));
		this.survivalButtonSelectionFrame.SetAlpha(1f);
		this.survivalButtonHeader.color = new Color(0.7019608f, 0.3647059f, 0.53333336f, 1f);
		this.descriptionArea.text = UI.FRONTEND.MODESELECTSCREEN.SURVIVAL_DESC;
	}

	private void OnHoverExitSurvival()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover", false));
		this.survivalButtonSelectionFrame.SetAlpha(0f);
		this.survivalButtonHeader.color = new Color(0.30980393f, 0.34117648f, 0.38431373f, 1f);
		this.descriptionArea.text = UI.FRONTEND.MODESELECTSCREEN.BLANK_DESC;
	}

	private void OnClickSurvival()
	{
		this.Deactivate();
		CustomGameSettings.Instance.SetSurvivalDefaults();
		GameObject gameObject = Util.KInstantiateUI(ScreenPrefabs.Instance.NewGameSettingsScreen.gameObject, base.transform.parent.gameObject, true);
		gameObject.GetComponent<KScreen>().Activate();
	}

	private void OnHoverEnterNosweat()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover", false));
		this.nosweatButtonSelectionFrame.SetAlpha(1f);
		this.nosweatButtonHeader.color = new Color(0.7019608f, 0.3647059f, 0.53333336f, 1f);
		this.descriptionArea.text = UI.FRONTEND.MODESELECTSCREEN.NOSWEAT_DESC;
	}

	private void OnHoverExitNosweat()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover", false));
		this.nosweatButtonSelectionFrame.SetAlpha(0f);
		this.nosweatButtonHeader.color = new Color(0.30980393f, 0.34117648f, 0.38431373f, 1f);
		this.descriptionArea.text = UI.FRONTEND.MODESELECTSCREEN.BLANK_DESC;
	}

	private void OnClickNosweat()
	{
		this.Deactivate();
		CustomGameSettings.Instance.SetNosweatDefaults();
		GameObject gameObject = Util.KInstantiateUI(ScreenPrefabs.Instance.NewGameSettingsScreen.gameObject, base.transform.parent.gameObject, true);
		gameObject.GetComponent<KScreen>().Activate();
	}

	[SerializeField]
	private MultiToggle nosweatButton;

	private Image nosweatButtonHeader;

	private Image nosweatButtonSelectionFrame;

	[SerializeField]
	private MultiToggle survivalButton;

	private Image survivalButtonHeader;

	private Image survivalButtonSelectionFrame;

	[SerializeField]
	private LocText descriptionArea;

	[SerializeField]
	private KButton closeButton;
}
