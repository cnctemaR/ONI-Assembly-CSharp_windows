using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BreakdownListRow : KMonoBehaviour
{
	public void ShowData(string name, string value)
	{
		base.gameObject.transform.localScale = Vector2.one;
		this.nameLabel.text = name;
		this.valueLabel.text = value;
		this.dotOutlineImage.gameObject.SetActive(true);
		this.dotOutlineImage.rectTransform.localScale = Vector2.one * 0.6f;
		this.dotInsideImage.gameObject.SetActive(true);
		this.dotInsideImage.color = new Color(0.34117648f, 0.36862746f, 0.45882353f, 1f);
		this.iconImage.gameObject.SetActive(false);
		this.checkmarkImage.gameObject.SetActive(false);
		this.SetHighlighted(false);
		this.SetImportant(false);
	}

	public void ShowStatusData(string name, string value, Color dotColor)
	{
		this.ShowData(name, value);
		this.dotOutlineImage.gameObject.SetActive(true);
		this.dotInsideImage.gameObject.SetActive(true);
		this.dotInsideImage.color = dotColor;
		this.iconImage.gameObject.SetActive(false);
		this.checkmarkImage.gameObject.SetActive(false);
	}

	public void SetStatusColor(Color dotColor)
	{
		this.dotInsideImage.color = dotColor;
	}

	public void ShowCheckmarkData(string name, string value, bool completed)
	{
		this.ShowData(name, value);
		this.dotOutlineImage.gameObject.SetActive(true);
		this.dotOutlineImage.rectTransform.localScale = Vector2.one;
		this.dotInsideImage.gameObject.SetActive(true);
		this.dotInsideImage.color = ((!completed) ? new Color(0.1882353f, 0.20392157f, 0.2627451f, 1f) : new Color(0.34117648f, 0.36862746f, 0.45882353f, 1f));
		this.iconImage.gameObject.SetActive(false);
		this.checkmarkImage.gameObject.SetActive(true);
		this.checkmarkImage.color = ((!completed) ? new Color(1f, 1f, 1f, 0.15f) : new Color(0.38431373f, 0.72156864f, 0f, 1f));
	}

	public void ShowIconData(string name, string value, Sprite sprite)
	{
		this.ShowData(name, value);
		this.dotOutlineImage.gameObject.SetActive(false);
		this.dotInsideImage.gameObject.SetActive(false);
		this.iconImage.gameObject.SetActive(true);
		this.checkmarkImage.gameObject.SetActive(false);
		this.iconImage.sprite = sprite;
		this.iconImage.color = Color.white;
	}

	public void ShowIconData(string name, string value, Sprite sprite, Color spriteColor)
	{
		this.ShowIconData(name, value, sprite);
		this.iconImage.color = spriteColor;
	}

	public void SetHighlighted(bool highlighted)
	{
		this.isHighlighted = highlighted;
		this.dotOutlineImage.rectTransform.localScale = Vector2.one * 0.8f;
		this.nameLabel.alpha = ((!this.isHighlighted) ? 0.5f : 0.9f);
		this.valueLabel.alpha = ((!this.isHighlighted) ? 0.5f : 0.9f);
	}

	public void SetDisabled(bool disabled)
	{
		this.isDisabled = disabled;
		this.nameLabel.alpha = ((!this.isDisabled) ? 0.5f : 0.4f);
		this.valueLabel.alpha = ((!this.isDisabled) ? 0.5f : 0.4f);
	}

	public void SetImportant(bool important)
	{
		this.isImportant = important;
		this.dotOutlineImage.rectTransform.localScale = Vector2.one;
		this.nameLabel.alpha = ((!this.isImportant) ? 0.5f : 1f);
		this.valueLabel.alpha = ((!this.isImportant) ? 0.5f : 1f);
		this.nameLabel.fontStyle = ((!this.isImportant) ? FontStyles.Normal : FontStyles.Bold);
		this.valueLabel.fontStyle = ((!this.isImportant) ? FontStyles.Normal : FontStyles.Bold);
	}

	public void HideIcon()
	{
		this.dotOutlineImage.gameObject.SetActive(false);
		this.dotInsideImage.gameObject.SetActive(false);
		this.iconImage.gameObject.SetActive(false);
		this.checkmarkImage.gameObject.SetActive(false);
	}

	public void AddTooltip(string tooltipText)
	{
		if (this.tooltip == null)
		{
			this.tooltip = base.gameObject.AddComponent<ToolTip>();
		}
		this.tooltip.SetSimpleTooltip(tooltipText);
	}

	public void SetValue(string value)
	{
		this.valueLabel.text = value;
	}

	public Image dotOutlineImage;

	public Image dotInsideImage;

	public Image iconImage;

	public Image checkmarkImage;

	public LocText nameLabel;

	public LocText valueLabel;

	private bool isHighlighted;

	private bool isDisabled;

	private bool isImportant;

	private ToolTip tooltip;
}
