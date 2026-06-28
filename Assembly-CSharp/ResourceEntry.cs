using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResourceEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	private void Awake()
	{
		this.QuantityLabel.color = this.AvailableColor;
		this.NameLabel.color = this.AvailableColor;
		base.GetComponent<Button>().onClick.AddListener(delegate
		{
			this.OnClick();
		});
		this.tooltip = base.GetComponent<ToolTip>();
	}

	private void OnClick()
	{
		List<Pickupable> pickupables = WorldInventory.Instance.GetPickupables(this.Resource);
		if (pickupables != null)
		{
			Pickupable pickupable = null;
			for (int i = 0; i < pickupables.Count; i++)
			{
				this.selectionIdx++;
				int num = this.selectionIdx % pickupables.Count;
				pickupable = pickupables[num];
				if (pickupable != null)
				{
					break;
				}
			}
			if (pickupable != null)
			{
				Transform transform = pickupable.transform;
				if (pickupable.storage != null)
				{
					transform = pickupable.storage.transform;
				}
				SelectTool.Instance.SelectAndFocus(transform.transform.position, transform.GetComponent<KSelectable>(), Vector3.zero);
				for (int j = 0; j < pickupables.Count; j++)
				{
					Pickupable pickupable2 = pickupables[j];
					if (pickupable2 != null)
					{
						KAnimControllerBase component = pickupable2.GetComponent<KAnimControllerBase>();
						if (component != null)
						{
							component.HighlightColour = this.HighlightColor;
						}
					}
				}
			}
		}
	}

	public void UpdateValue(ResourceCategoryHeader.MeasureUnit measure = ResourceCategoryHeader.MeasureUnit.mass)
	{
		this.SetName(this.Resource.ProperName());
		float num;
		if (measure == ResourceCategoryHeader.MeasureUnit.kcal)
		{
			EdiblesManager.FoodInfo foodInfo = EdiblesManager.instance.GetFoodInfo(this.Resource.Name);
			num = WorldInventory.Instance.GetAmount(this.Resource) * foodInfo.CaloriesPerUnit;
		}
		else
		{
			num = WorldInventory.Instance.GetAmount(this.Resource);
		}
		if (this.quantityText == null || this.currentQuantity != num)
		{
			if (measure != ResourceCategoryHeader.MeasureUnit.mass)
			{
				if (measure != ResourceCategoryHeader.MeasureUnit.quantity)
				{
					if (measure == ResourceCategoryHeader.MeasureUnit.kcal)
					{
						this.quantityText = GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true);
					}
				}
				else
				{
					this.quantityText = num.ToString();
				}
			}
			else
			{
				this.quantityText = GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
			}
			this.QuantityLabel.text = this.quantityText;
			this.currentQuantity = num;
			if (this.tooltip != null)
			{
				this.tooltip.ClearMultiStringTooltip();
				this.tooltip.AddMultiStringTooltip(this.NameLabel.text, this.tooltipStyle_Header);
				this.tooltip.AddMultiStringTooltip(string.Format(UI.RESOURCESCREEN.AVAILABLE_TOOLTIP, this.QuantityLabel.text), this.tooltipStyle_body);
			}
		}
		Color color = this.AvailableColor;
		if (num == 0f)
		{
			color = this.UnavailableColor;
		}
		if (this.QuantityLabel.color != color)
		{
			this.QuantityLabel.color = color;
		}
		if (this.NameLabel.color != color)
		{
			this.NameLabel.color = color;
		}
	}

	public void SetName(string name)
	{
		this.NameLabel.text = name;
	}

	public void SetTag(Tag t)
	{
		this.Resource = t;
	}

	private void Hover(bool is_hovering)
	{
		if (!(WorldInventory.Instance == null))
		{
			if (is_hovering)
			{
				this.Background.color = this.BackgroundHoverColor;
			}
			else
			{
				this.Background.color = new Color(0f, 0f, 0f, 0f);
			}
			List<Pickupable> pickupables = WorldInventory.Instance.GetPickupables(this.Resource);
			if (pickupables != null)
			{
				for (int i = 0; i < pickupables.Count; i++)
				{
					if (!(pickupables[i] == null))
					{
						KAnimControllerBase component = pickupables[i].GetComponent<KAnimControllerBase>();
						if (!(component == null))
						{
							if (is_hovering)
							{
								component.HighlightColour = this.HighlightColor;
							}
							else
							{
								component.HighlightColour = Color.black;
							}
						}
					}
				}
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		this.Hover(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		this.Hover(false);
	}

	public void SetSprite(Tag t)
	{
		Element element = ElementLoader.GetElement(this.Resource.Name);
		if (element != null)
		{
			Sprite uispriteFromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(element.substance.anim, "ui");
			if (uispriteFromMultiObjectAnim != null)
			{
				this.image.sprite = uispriteFromMultiObjectAnim;
			}
		}
	}

	public void SetSprite(Sprite sprite)
	{
		this.image.sprite = sprite;
	}

	public Tag Resource;

	public LocText NameLabel;

	public LocText QuantityLabel;

	public Image image;

	[SerializeField]
	private Color AvailableColor;

	[SerializeField]
	private Color UnavailableColor;

	[SerializeField]
	private Color HighlightColor;

	[SerializeField]
	private Color BackgroundHoverColor;

	[SerializeField]
	private Image Background;

	private ToolTip tooltip;

	private TextStyleSetting tooltipStyle_Header = null;

	private TextStyleSetting tooltipStyle_body = null;

	private int selectionIdx;

	private string quantityText = null;

	private float currentQuantity = 0f;
}
