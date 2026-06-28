using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CollapsibleDetailContentPanel : KMonoBehaviour
{
	public static void SetActiveBasedOnContentPresent(CollapsibleDetailContentPanel panelInstance)
	{
		if (panelInstance.ContentsEmpty())
		{
			if (panelInstance.gameObject.activeSelf)
			{
				panelInstance.gameObject.SetActive(false);
			}
		}
		else if (!panelInstance.gameObject.activeSelf)
		{
			panelInstance.gameObject.SetActive(true);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.CollapseButton.onClick.AddListener(new UnityAction(this.ToggleOpen));
		this.SetColors(this.colors);
		this.ArrowIcon.SetActive();
		this.Refresh();
	}

	public void SetTitle(string title)
	{
		this.HeaderLabel.SetText(title);
	}

	public bool ContentsEmpty()
	{
		int num = 0;
		for (int i = 0; i < this.Content.childCount; i++)
		{
			if (this.Content.GetChild(i).gameObject.activeSelf)
			{
				num++;
			}
		}
		return num <= 1;
	}

	private void Refresh()
	{
		if (this.ContentsEmpty())
		{
			base.gameObject.SetActive(false);
		}
	}

	public void SetColors(CollapsibleDetailContentPanel.PanelColors newColors)
	{
		this.colors = newColors;
		this.HeaderLabel.color = this.colors.TextColor;
		this.ArrowIcon.ActiveColour = this.colors.ArrowColor;
		this.ArrowIcon.InactiveColour = this.colors.ArrowColor;
		this.CollapseButton.transition = Selectable.Transition.None;
		ColorBlock colorBlock = default(ColorBlock);
		colorBlock.normalColor = new Color(this.colors.FrameColor.r, this.colors.FrameColor.g, this.colors.FrameColor.b, this.colors.FrameColor.a);
		colorBlock.highlightedColor = this.colors.FrameColor_Hover;
		colorBlock.pressedColor = this.colors.FrameColor_Press;
		colorBlock.disabledColor = colorBlock.normalColor;
		colorBlock.colorMultiplier = 1f;
		this.CollapseButton.colors = colorBlock;
		this.CollapseButton.transition = Selectable.Transition.ColorTint;
	}

	private void ToggleOpen()
	{
		bool flag = this.scalerMask.gameObject.activeSelf;
		flag = !flag;
		this.scalerMask.gameObject.SetActive(flag);
		if (flag)
		{
			this.ArrowIcon.SetActive();
			this.ForceLocTextsMeshRebuild();
		}
		else
		{
			this.ArrowIcon.SetInactive();
		}
	}

	public void SetCollapsible(bool bCollapsible)
	{
		this.ArrowIcon.gameObject.SetActive(bCollapsible);
		this.CollapseButton.interactable = bCollapsible;
	}

	public void ForceLocTextsMeshRebuild()
	{
		LocText[] componentsInChildren = base.GetComponentsInChildren<LocText>();
		foreach (LocText locText in componentsInChildren)
		{
			locText.ForceMeshUpdate();
		}
	}

	public ImageToggleState ArrowIcon;

	public LocText HeaderLabel;

	public Button CollapseButton;

	public Transform Content;

	public Image BGFrame;

	public ScalerMask scalerMask;

	public GameObject textContainerPrefab;

	public CollapsibleDetailContentPanel.PanelColors colors;

	[Serializable]
	public struct PanelColors
	{
		public Color FrameColor;

		public Color FrameColor_Hover;

		public Color FrameColor_Press;

		public Color ArrowColor;

		public Color TextColor;
	}
}
