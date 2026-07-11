using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipScreen : KScreen
{
	public static ToolTipScreen Instance { get; private set; }

	protected override void OnActivate()
	{
		ToolTipScreen.Instance = this;
		this.toolTipWidget = Util.KInstantiate(this.ToolTipPrefab, base.gameObject, null);
		this.toolTipWidget.transform.SetParent(base.gameObject.transform, false);
		Util.Reset(this.toolTipWidget.transform);
		this.toolTipWidget.SetActive(false);
	}

	protected override void OnCleanUp()
	{
		ToolTipScreen.Instance = null;
	}

	public void SetToolTip(ToolTip tool_tip)
	{
		this.tooltipSetting = tool_tip;
		this.multiTooltipContainer = this.toolTipWidget.transform.Find("MultitooltipContainer").gameObject;
		this.ConfigureTooltip();
	}

	private void ConfigureTooltip()
	{
		if (this.tooltipSetting == null)
		{
			this.prevTooltip = null;
		}
		if (this.tooltipSetting != null && this.dirtyHoverTooltip != null && this.tooltipSetting == this.dirtyHoverTooltip)
		{
			this.ClearToolTip(this.dirtyHoverTooltip);
		}
		if (this.tooltipSetting != null)
		{
			this.tooltipSetting.RebuildDynamicTooltip();
			if (this.tooltipSetting.multiStringCount == 0)
			{
				this.clearMultiStringTooltip();
			}
			else if (this.prevTooltip != this.tooltipSetting || !this.multiTooltipContainer.activeInHierarchy)
			{
				this.prepareMultiStringTooltip(this.tooltipSetting);
				this.prevTooltip = this.tooltipSetting;
			}
			bool flag = this.multiTooltipContainer.transform.childCount != 0;
			this.toolTipWidget.SetActive(flag);
			if (flag)
			{
				RectTransform rectTransform;
				if (this.tooltipSetting.overrideParentObject == null)
				{
					rectTransform = this.tooltipSetting.GetComponent<RectTransform>();
				}
				else
				{
					rectTransform = this.tooltipSetting.overrideParentObject;
				}
				RectTransform component = this.toolTipWidget.GetComponent<RectTransform>();
				component.transform.SetParent(this.anchorRoot.transform);
				if (!this.tooltipSetting.worldSpace)
				{
					this.anchorRoot.anchoredPosition = rectTransform.transform.GetPosition();
				}
				else
				{
					this.anchorRoot.anchoredPosition = base.WorldToScreen(rectTransform.transform.GetPosition()) + new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f);
				}
				this.anchorRoot.anchoredPosition -= Vector2.up * (rectTransform.rectTransform().pivot.y * rectTransform.rectTransform().sizeDelta.y);
				this.anchorRoot.anchoredPosition -= Vector2.right * (rectTransform.rectTransform().pivot.x * rectTransform.rectTransform().sizeDelta.x);
				this.anchorRoot.anchoredPosition += Vector2.right * (rectTransform.sizeDelta.x * this.tooltipSetting.parentPositionAnchor.x);
				this.anchorRoot.anchoredPosition += Vector2.up * (rectTransform.sizeDelta.y * this.tooltipSetting.parentPositionAnchor.y);
				float num = 1f;
				CanvasScaler canvasScaler = base.transform.parent.GetComponent<CanvasScaler>();
				if (canvasScaler == null)
				{
					canvasScaler = base.transform.parent.parent.GetComponent<CanvasScaler>();
				}
				if (canvasScaler != null)
				{
					num = canvasScaler.scaleFactor;
				}
				this.anchorRoot.anchoredPosition = new Vector2(this.anchorRoot.anchoredPosition.x / num, this.anchorRoot.anchoredPosition.y / num);
				component.pivot = this.tooltipSetting.tooltipPivot;
				RectTransform rectTransform2 = component;
				Vector2 vector = new Vector2(0f, 0f);
				component.anchorMax = vector;
				rectTransform2.anchorMin = vector;
				component.anchoredPosition = this.tooltipSetting.tooltipPositionOffset * num;
				if (!this.tooltipSetting.worldSpace)
				{
					Rect rect = ((RectTransform)base.transform).rect;
					Vector2 vector2 = new Vector2(base.transform.GetPosition().x, base.transform.GetPosition().y) + this.ScreenEdgePadding;
					Vector2 vector3 = new Vector2(base.transform.GetPosition().x, base.transform.GetPosition().y) + rect.width * Vector2.right + rect.height * Vector2.up - this.ScreenEdgePadding * Mathf.Max(1f, num);
					vector3.x *= num;
					vector3.y *= num;
					Vector2 vector4;
					vector4.x = component.GetPosition().x - component.pivot.x * (component.sizeDelta.x * num);
					vector4.y = component.GetPosition().y - component.pivot.y * (component.sizeDelta.y * num);
					Vector2 vector5;
					vector5.x = component.GetPosition().x + (1f - component.pivot.x) * (component.sizeDelta.x * num);
					vector5.y = component.GetPosition().y + (1f - component.pivot.y) * (component.sizeDelta.y * num);
					Vector2 vector6 = Vector2.zero;
					if (vector4.x < vector2.x)
					{
						vector6.x = vector2.x - vector4.x;
					}
					if (vector5.x > vector3.x)
					{
						vector6.x = vector3.x - vector5.x;
					}
					if (vector4.y < vector2.y)
					{
						vector6.y = vector2.y - vector4.y;
					}
					if (vector5.y > vector3.y)
					{
						vector6.y = vector3.y - vector5.y;
					}
					vector6 /= num;
					component.anchoredPosition += vector6;
				}
			}
		}
		if (((RectTransform)base.transform).GetSiblingIndex() != base.transform.parent.childCount - 1)
		{
			((RectTransform)base.transform).SetAsLastSibling();
		}
	}

	private void prepareMultiStringTooltip(ToolTip setting)
	{
		int multiStringCount = this.tooltipSetting.multiStringCount;
		this.clearMultiStringTooltip();
		for (int i = 0; i < multiStringCount; i++)
		{
			GameObject gameObject = Util.KInstantiateUI(this.labelPrefab, null, true);
			gameObject.transform.SetParent(this.multiTooltipContainer.transform);
		}
		for (int j = 0; j < this.tooltipSetting.multiStringCount; j++)
		{
			Transform child = this.multiTooltipContainer.transform.GetChild(j);
			LayoutElement component = child.GetComponent<LayoutElement>();
			TextMeshProUGUI component2 = child.GetComponent<TextMeshProUGUI>();
			component2.text = this.tooltipSetting.GetMultiString(j);
			SetTextStyleSetting component3 = child.GetComponent<SetTextStyleSetting>();
			component3.SetStyle((TextStyleSetting)this.tooltipSetting.GetStyleSetting(j));
			if (setting.SizingSetting == ToolTip.ToolTipSizeSetting.MaxWidthWrapContent)
			{
				LayoutElement layoutElement = component;
				float num = setting.WrapWidth;
				component.preferredWidth = num;
				layoutElement.minWidth = num;
				component.rectTransform().sizeDelta = new Vector2(setting.WrapWidth, 1000f);
				LayoutElement layoutElement2 = component;
				num = component2.preferredHeight;
				component.preferredHeight = num;
				layoutElement2.minHeight = num;
				LayoutElement layoutElement3 = component;
				num = component2.preferredHeight;
				component.preferredHeight = num;
				layoutElement3.minHeight = num;
				component.rectTransform().sizeDelta = new Vector2(setting.WrapWidth, component.minHeight);
				base.GetComponentInChildren<ContentSizeFitter>(true).horizontalFit = ContentSizeFitter.FitMode.MinSize;
				this.multiTooltipContainer.GetComponent<LayoutElement>().minWidth = setting.WrapWidth;
			}
			else if (setting.SizingSetting == ToolTip.ToolTipSizeSetting.DynamicWidthNoWrap)
			{
				base.GetComponentInChildren<ContentSizeFitter>(true).horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
				Vector2 preferredValues = component2.GetPreferredValues();
				LayoutElement component4 = this.multiTooltipContainer.GetComponent<LayoutElement>();
				float num = preferredValues.x;
				component.preferredWidth = num;
				num = num;
				component.minWidth = num;
				component4.minWidth = num;
				LayoutElement layoutElement4 = component;
				num = preferredValues.y;
				component.preferredHeight = num;
				layoutElement4.minHeight = num;
				base.GetComponentInChildren<ContentSizeFitter>(true).SetLayoutHorizontal();
				base.GetComponentInChildren<ContentSizeFitter>(true).SetLayoutVertical();
				this.multiTooltipContainer.rectTransform().sizeDelta = new Vector2(component.minWidth, component.minHeight);
				this.multiTooltipContainer.transform.parent.rectTransform().sizeDelta = this.multiTooltipContainer.rectTransform().sizeDelta;
			}
			component2.ForceMeshUpdate();
		}
		this.tooltipIncubating = true;
	}

	private void Update()
	{
		if (this.tooltipSetting != null)
		{
			this.tooltipSetting.UpdateWhileHovered();
		}
		if (this.multiTooltipContainer == null || this.anchorRoot == null)
		{
			return;
		}
		if (this.dirtyHoverTooltip != null)
		{
			ToolTip toolTip = this.dirtyHoverTooltip;
			this.MakeDirtyTooltipClean(toolTip);
			this.ClearToolTip(toolTip);
		}
		if (this.tooltipIncubating)
		{
			this.tooltipIncubating = false;
			Image componentInChildren = this.anchorRoot.GetComponentInChildren<Image>();
			if (componentInChildren != null)
			{
				this.anchorRoot.GetComponentInChildren<Image>(true).enabled = false;
			}
			this.multiTooltipContainer.transform.localScale = Vector3.zero;
			this.toolTipIsBlank = true;
			for (int i = 0; i < this.multiTooltipContainer.transform.childCount; i++)
			{
				if (this.multiTooltipContainer.transform.GetChild(i).transform.localScale != Vector3.one)
				{
					this.multiTooltipContainer.transform.GetChild(i).transform.localScale = Vector3.one;
				}
				LayoutElement component = this.multiTooltipContainer.transform.GetChild(i).GetComponent<LayoutElement>();
				TextMeshProUGUI component2 = component.GetComponent<TextMeshProUGUI>();
				this.toolTipIsBlank = component2.text == string.Empty && this.toolTipIsBlank;
				if (component.minHeight != component2.preferredHeight)
				{
					component.minHeight = component2.preferredHeight;
				}
			}
		}
		else if (this.multiTooltipContainer.transform.localScale != Vector3.one && !this.toolTipIsBlank)
		{
			Image componentInChildren2 = this.anchorRoot.GetComponentInChildren<Image>();
			if (componentInChildren2 != null)
			{
				this.anchorRoot.GetComponentInChildren<Image>(true).enabled = true;
			}
			this.multiTooltipContainer.transform.localScale = Vector3.one;
		}
	}

	public void HotSwapTooltipString(string newString, int lineIndex)
	{
		if (this.multiTooltipContainer.transform.childCount > lineIndex)
		{
			Transform child = this.multiTooltipContainer.transform.GetChild(lineIndex);
			TextMeshProUGUI component = child.GetComponent<TextMeshProUGUI>();
			component.text = newString;
		}
	}

	private void clearMultiStringTooltip()
	{
		for (int i = this.multiTooltipContainer.transform.childCount - 1; i >= 0; i--)
		{
			global::UnityEngine.Object.DestroyImmediate(this.multiTooltipContainer.transform.GetChild(i).gameObject);
		}
	}

	public void ClearToolTip(ToolTip tt)
	{
		if (tt == this.tooltipSetting)
		{
			this.tooltipSetting = null;
			if (this.toolTipWidget != null)
			{
				this.clearMultiStringTooltip();
				this.toolTipWidget.SetActive(false);
			}
		}
	}

	public void MarkTooltipDirty(ToolTip tt)
	{
		if (tt == this.tooltipSetting)
		{
			this.dirtyHoverTooltip = tt;
		}
	}

	public void MakeDirtyTooltipClean(ToolTip tt)
	{
		if (tt == this.dirtyHoverTooltip)
		{
			this.dirtyHoverTooltip = null;
		}
	}

	public GameObject ToolTipPrefab;

	public RectTransform anchorRoot;

	private GameObject toolTipWidget;

	private ToolTip prevTooltip;

	private ToolTip tooltipSetting;

	public GameObject labelPrefab;

	private GameObject multiTooltipContainer;

	public TextStyleSetting defaultTooltipHeaderStyle;

	public TextStyleSetting defaultTooltipBodyStyle;

	private bool toolTipIsBlank;

	private Vector2 ScreenEdgePadding = new Vector2(8f, 8f);

	private ToolTip dirtyHoverTooltip;

	private bool tooltipIncubating = true;
}
