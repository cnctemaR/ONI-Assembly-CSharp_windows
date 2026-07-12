using System;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ClusterCategorySelectionScreen : NewGameFlowScreen
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		HierarchyReferences component = this.vanillaButton.GetComponent<HierarchyReferences>();
		this.vanillaButtonHeader = component.GetReference<RectTransform>("HeaderBackground").GetComponent<Image>();
		this.vanillalButtonSelectionFrame = component.GetReference<RectTransform>("SelectionFrame").GetComponent<Image>();
		MultiToggle multiToggle = this.vanillaButton;
		multiToggle.onEnter = (global::System.Action)Delegate.Combine(multiToggle.onEnter, new global::System.Action(this.OnHoverEnterVanilla));
		MultiToggle multiToggle2 = this.vanillaButton;
		multiToggle2.onExit = (global::System.Action)Delegate.Combine(multiToggle2.onExit, new global::System.Action(this.OnHoverExitVanilla));
		MultiToggle multiToggle3 = this.vanillaButton;
		multiToggle3.onClick = (global::System.Action)Delegate.Combine(multiToggle3.onClick, new global::System.Action(this.OnClickVanilla));
		HierarchyReferences component2 = this.spacedOutButton.GetComponent<HierarchyReferences>();
		this.spacedOutButtonHeader = component2.GetReference<RectTransform>("HeaderBackground").GetComponent<Image>();
		this.spacedOutButtonSelectionFrame = component2.GetReference<RectTransform>("SelectionFrame").GetComponent<Image>();
		MultiToggle multiToggle4 = this.spacedOutButton;
		multiToggle4.onEnter = (global::System.Action)Delegate.Combine(multiToggle4.onEnter, new global::System.Action(this.OnHoverEnterSpacedOut));
		MultiToggle multiToggle5 = this.spacedOutButton;
		multiToggle5.onExit = (global::System.Action)Delegate.Combine(multiToggle5.onExit, new global::System.Action(this.OnHoverExitSpacedOut));
		MultiToggle multiToggle6 = this.spacedOutButton;
		multiToggle6.onClick = (global::System.Action)Delegate.Combine(multiToggle6.onClick, new global::System.Action(this.OnClickSpacedOut));
		this.closeButton.onClick += base.NavigateBackward;
	}

	private void OnHoverEnterVanilla()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover", false));
		this.vanillalButtonSelectionFrame.SetAlpha(1f);
		this.vanillaButtonHeader.color = new Color(0.7019608f, 0.3647059f, 0.53333336f, 1f);
		this.descriptionArea.text = UI.FRONTEND.CLUSTERCATEGORYSELECTSCREEN.VANILLA_DESC;
	}

	private void OnHoverExitVanilla()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover", false));
		this.vanillalButtonSelectionFrame.SetAlpha(0f);
		this.vanillaButtonHeader.color = new Color(0.30980393f, 0.34117648f, 0.38431373f, 1f);
		this.descriptionArea.text = UI.FRONTEND.CLUSTERCATEGORYSELECTSCREEN.BLANK_DESC;
	}

	private void OnClickVanilla()
	{
		this.Deactivate();
		DestinationSelectPanel.ChosenClusterCategorySetting = 1;
		base.NavigateForward();
	}

	private void OnHoverEnterSpacedOut()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover", false));
		this.spacedOutButtonSelectionFrame.SetAlpha(1f);
		this.spacedOutButtonHeader.color = new Color(0.7019608f, 0.3647059f, 0.53333336f, 1f);
		this.descriptionArea.text = UI.FRONTEND.CLUSTERCATEGORYSELECTSCREEN.SPACEDOUT_DESC;
	}

	private void OnHoverExitSpacedOut()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover", false));
		this.spacedOutButtonSelectionFrame.SetAlpha(0f);
		this.spacedOutButtonHeader.color = new Color(0.30980393f, 0.34117648f, 0.38431373f, 1f);
		this.descriptionArea.text = UI.FRONTEND.CLUSTERCATEGORYSELECTSCREEN.BLANK_DESC;
	}

	private void OnClickSpacedOut()
	{
		this.Deactivate();
		DestinationSelectPanel.ChosenClusterCategorySetting = 2;
		base.NavigateForward();
	}

	[SerializeField]
	private MultiToggle spacedOutButton;

	private Image spacedOutButtonHeader;

	private Image spacedOutButtonSelectionFrame;

	[SerializeField]
	private MultiToggle vanillaButton;

	private Image vanillaButtonHeader;

	private Image vanillalButtonSelectionFrame;

	[SerializeField]
	private LocText descriptionArea;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KBatchedAnimController nosweatAnim;

	[SerializeField]
	private KBatchedAnimController survivalAnim;
}
