using System;
using System.Collections.Generic;
using Klei;
using STRINGS;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResourceEntry : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.QuantityLabel.color = this.AvailableColor;
		this.NameLabel.color = this.AvailableColor;
		this.button.onClick.AddListener(new UnityAction(this.OnClick));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.tooltip.OnToolTip = new Func<string>(this.OnToolTip);
	}

	private void OnClick()
	{
		List<Pickupable> pickupables = WorldInventory.Instance.GetPickupables(this.Resource);
		if (pickupables == null)
		{
			return;
		}
		Pickupable pickupable = null;
		for (int i = 0; i < pickupables.Count; i++)
		{
			this.selectionIdx++;
			int num = this.selectionIdx % pickupables.Count;
			pickupable = pickupables[num];
			if (pickupable != null && !pickupable.HasTag(GameTags.StoredPrivate))
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
			SelectTool.Instance.SelectAndFocus(transform.transform.GetPosition(), transform.GetComponent<KSelectable>(), Vector3.zero);
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

	private void GetAmounts(bool doExtras, out float available, out float total, out float reserved)
	{
		available = WorldInventory.Instance.GetAmount(this.Resource);
		total = ((!doExtras) ? 0f : WorldInventory.Instance.GetTotalAmount(this.Resource));
		reserved = ((!doExtras) ? 0f : MaterialNeeds.Instance.GetAmount(this.Resource));
		if (this.Measure == GameUtil.MeasureUnit.kcal)
		{
			EdiblesManager.FoodInfo foodInfo = Game.Instance.ediblesManager.GetFoodInfo(this.Resource.Name);
			available *= foodInfo.CaloriesPerUnit;
			total *= foodInfo.CaloriesPerUnit;
			reserved *= foodInfo.CaloriesPerUnit;
		}
	}

	public void UpdateValue()
	{
		this.SetName(this.Resource.ProperName());
		bool allowInsufficientMaterialBuild = GenericGameSettings.instance.allowInsufficientMaterialBuild;
		float num;
		float num2;
		float num3;
		this.GetAmounts(allowInsufficientMaterialBuild, out num, out num2, out num3);
		if (this.currentQuantity != num)
		{
			this.currentQuantity = num;
			this.QuantityLabel.text = ResourceCategoryScreen.QuantityTextForMeasure(num, this.Measure);
		}
		Color color = this.AvailableColor;
		if (num3 > num2)
		{
			color = this.OverdrawnColor;
		}
		else if (num == 0f)
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

	private string OnToolTip()
	{
		float num;
		float num2;
		float num3;
		this.GetAmounts(true, out num, out num2, out num3);
		string text = this.NameLabel.text + "\n";
		return text + string.Format(UI.RESOURCESCREEN.AVAILABLE_TOOLTIP, ResourceCategoryScreen.QuantityTextForMeasure(num, this.Measure), ResourceCategoryScreen.QuantityTextForMeasure(num3, this.Measure), ResourceCategoryScreen.QuantityTextForMeasure(num2, this.Measure));
	}

	public void SetName(string name)
	{
		this.NameLabel.text = name;
	}

	public void SetTag(Tag t, GameUtil.MeasureUnit measure)
	{
		this.Resource = t;
		this.Measure = measure;
	}

	private void Hover(bool is_hovering)
	{
		if (WorldInventory.Instance == null)
		{
			return;
		}
		if (is_hovering)
		{
			this.Background.color = this.BackgroundHoverColor;
		}
		else
		{
			this.Background.color = new Color(0f, 0f, 0f, 0f);
		}
		List<Pickupable> pickupables = WorldInventory.Instance.GetPickupables(this.Resource);
		if (pickupables == null)
		{
			return;
		}
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
			Sprite uispriteFromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(element.substance.anim, "ui", false);
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

	public GameUtil.MeasureUnit Measure;

	public LocText NameLabel;

	public LocText QuantityLabel;

	public Image image;

	[SerializeField]
	private Color AvailableColor;

	[SerializeField]
	private Color UnavailableColor;

	[SerializeField]
	private Color OverdrawnColor;

	[SerializeField]
	private Color HighlightColor;

	[SerializeField]
	private Color BackgroundHoverColor;

	[SerializeField]
	private Image Background;

	[MyCmpGet]
	private ToolTip tooltip;

	[MyCmpReq]
	private Button button;

	private int selectionIdx;

	private float currentQuantity = float.MinValue;
}
