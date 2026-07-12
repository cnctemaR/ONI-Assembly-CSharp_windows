using System;
using System.Collections.Generic;
using ProcGen;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ClusterCategorySelectionScreen : NewGameFlowScreen
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.closeButton.onClick += base.NavigateBackward;
		int num = 0;
		using (Dictionary<string, ClusterLayout>.ValueCollection.Enumerator enumerator = SettingsCache.clusterLayouts.clusterCache.Values.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.clusterCategory == ClusterLayout.ClusterCategory.Special)
				{
					num++;
				}
			}
		}
		if (num > 0)
		{
			this.eventStyle.button.gameObject.SetActive(true);
			this.eventStyle.Init(this.descriptionArea, UI.FRONTEND.CLUSTERCATEGORYSELECTSCREEN.EVENT_DESC, UI.FRONTEND.CLUSTERCATEGORYSELECTSCREEN.EVENT_TITLE);
			MultiToggle button = this.eventStyle.button;
			button.onClick = (global::System.Action)Delegate.Combine(button.onClick, new global::System.Action(delegate
			{
				this.OnClickOption(ClusterLayout.ClusterCategory.Special);
			}));
		}
		if (DlcManager.IsExpansion1Active())
		{
			this.classicStyle.button.gameObject.SetActive(true);
			this.classicStyle.Init(this.descriptionArea, UI.FRONTEND.CLUSTERCATEGORYSELECTSCREEN.CLASSIC_DESC, UI.FRONTEND.CLUSTERCATEGORYSELECTSCREEN.CLASSIC_TITLE);
			MultiToggle button2 = this.classicStyle.button;
			button2.onClick = (global::System.Action)Delegate.Combine(button2.onClick, new global::System.Action(delegate
			{
				this.OnClickOption(ClusterLayout.ClusterCategory.SpacedOutVanillaStyle);
			}));
			this.spacedOutStyle.button.gameObject.SetActive(true);
			this.spacedOutStyle.Init(this.descriptionArea, UI.FRONTEND.CLUSTERCATEGORYSELECTSCREEN.SPACEDOUT_DESC, UI.FRONTEND.CLUSTERCATEGORYSELECTSCREEN.SPACEDOUT_TITLE);
			MultiToggle button3 = this.spacedOutStyle.button;
			button3.onClick = (global::System.Action)Delegate.Combine(button3.onClick, new global::System.Action(delegate
			{
				this.OnClickOption(ClusterLayout.ClusterCategory.SpacedOutStyle);
			}));
			this.panel.sizeDelta = ((num > 0) ? new Vector2(622f, this.panel.sizeDelta.y) : new Vector2(480f, this.panel.sizeDelta.y));
			return;
		}
		this.vanillaStyle.button.gameObject.SetActive(true);
		this.vanillaStyle.Init(this.descriptionArea, UI.FRONTEND.CLUSTERCATEGORYSELECTSCREEN.VANILLA_DESC, UI.FRONTEND.CLUSTERCATEGORYSELECTSCREEN.VANILLA_TITLE);
		MultiToggle button4 = this.vanillaStyle.button;
		button4.onClick = (global::System.Action)Delegate.Combine(button4.onClick, new global::System.Action(delegate
		{
			this.OnClickOption(ClusterLayout.ClusterCategory.Vanilla);
		}));
		this.panel.sizeDelta = new Vector2(480f, this.panel.sizeDelta.y);
		this.eventStyle.kanim.Play("lab_asteroid_standard", KAnim.PlayMode.Once, 1f, 0f);
	}

	private void OnClickOption(ClusterLayout.ClusterCategory clusterCategory)
	{
		this.Deactivate();
		DestinationSelectPanel.ChosenClusterCategorySetting = (int)clusterCategory;
		base.NavigateForward();
	}

	public ClusterCategorySelectionScreen.ButtonConfig vanillaStyle;

	public ClusterCategorySelectionScreen.ButtonConfig classicStyle;

	public ClusterCategorySelectionScreen.ButtonConfig spacedOutStyle;

	public ClusterCategorySelectionScreen.ButtonConfig eventStyle;

	[SerializeField]
	private LocText descriptionArea;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private RectTransform panel;

	[Serializable]
	public class ButtonConfig
	{
		public void Init(LocText descriptionArea, string hoverDescriptionText, string headerText)
		{
			this.descriptionArea = descriptionArea;
			this.hoverDescriptionText = hoverDescriptionText;
			this.headerLabel.SetText(headerText);
			MultiToggle multiToggle = this.button;
			multiToggle.onEnter = (global::System.Action)Delegate.Combine(multiToggle.onEnter, new global::System.Action(this.OnHoverEnter));
			MultiToggle multiToggle2 = this.button;
			multiToggle2.onExit = (global::System.Action)Delegate.Combine(multiToggle2.onExit, new global::System.Action(this.OnHoverExit));
			HierarchyReferences component = this.button.GetComponent<HierarchyReferences>();
			this.headerImage = component.GetReference<RectTransform>("HeaderBackground").GetComponent<Image>();
			this.selectionFrame = component.GetReference<RectTransform>("SelectionFrame").GetComponent<Image>();
		}

		private void OnHoverEnter()
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover", false));
			this.selectionFrame.SetAlpha(1f);
			this.headerImage.color = new Color(0.7019608f, 0.3647059f, 0.53333336f, 1f);
			this.descriptionArea.text = this.hoverDescriptionText;
		}

		private void OnHoverExit()
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover", false));
			this.selectionFrame.SetAlpha(0f);
			this.headerImage.color = new Color(0.30980393f, 0.34117648f, 0.38431373f, 1f);
			this.descriptionArea.text = UI.FRONTEND.CLUSTERCATEGORYSELECTSCREEN.BLANK_DESC;
		}

		public MultiToggle button;

		public Image headerImage;

		public LocText headerLabel;

		public Image selectionFrame;

		public KAnimControllerBase kanim;

		private string hoverDescriptionText;

		private LocText descriptionArea;
	}
}
