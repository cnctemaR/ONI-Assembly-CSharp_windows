using System;
using UnityEngine;
using UnityEngine.UI;

public class MotdBox_ImageButtonLayoutElement : LayoutElement
{
	private void UpdateState()
	{
		MotdBox_ImageButtonLayoutElement.Style style = this.style;
		if (style == MotdBox_ImageButtonLayoutElement.Style.WidthExpandsBasedOnHeight)
		{
			this.flexibleHeight = 1f;
			this.preferredHeight = -1f;
			this.minHeight = -1f;
			this.flexibleWidth = 0f;
			this.preferredWidth = this.rectTransform().sizeDelta.y * this.heightToWidthRatio;
			this.minWidth = this.preferredWidth;
			this.ignoreLayout = false;
			return;
		}
		if (style != MotdBox_ImageButtonLayoutElement.Style.HeightExpandsBasedOnWidth)
		{
			return;
		}
		this.flexibleWidth = 1f;
		this.preferredWidth = -1f;
		this.minWidth = -1f;
		this.flexibleHeight = 0f;
		this.preferredHeight = this.rectTransform().sizeDelta.x / this.heightToWidthRatio;
		this.minHeight = this.preferredHeight;
		this.ignoreLayout = false;
	}

	protected override void OnTransformParentChanged()
	{
		this.UpdateState();
		base.OnTransformParentChanged();
	}

	protected override void OnRectTransformDimensionsChange()
	{
		this.UpdateState();
		base.OnRectTransformDimensionsChange();
	}

	[SerializeField]
	private float heightToWidthRatio;

	[SerializeField]
	private MotdBox_ImageButtonLayoutElement.Style style;

	private enum Style
	{
		WidthExpandsBasedOnHeight,
		HeightExpandsBasedOnWidth
	}
}
