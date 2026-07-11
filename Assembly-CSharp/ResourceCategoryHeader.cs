using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResourceCategoryHeader : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.EntryContainer.SetParent(base.transform.parent);
		this.EntryContainer.SetSiblingIndex(base.transform.GetSiblingIndex() + 1);
		this.EntryContainer.localScale = Vector3.one;
		this.mButton = base.GetComponent<Button>();
		this.mButton.onClick.AddListener(delegate
		{
			this.ToggleOpen(true);
		});
		this.SetInteractable(false);
		this.SetActiveColor(false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.tooltip.OnToolTip = new Func<string>(this.OnTooltip);
		this.UpdateContents();
	}

	private void SetInteractable(bool state)
	{
		if (state)
		{
			if (!this.IsOpen)
			{
				this.expandArrow.SetInactive();
			}
			else
			{
				this.expandArrow.SetActive();
			}
		}
		else
		{
			this.SetOpen(false);
			this.expandArrow.SetDisabled();
		}
	}

	private void SetActiveColor(bool state)
	{
		if (state)
		{
			this.elements.QuantityText.color = this.TextColor_Interactable;
			this.elements.LabelText.color = this.TextColor_Interactable;
			this.expandArrow.ActiveColour = this.TextColor_Interactable;
			this.expandArrow.InactiveColour = this.TextColor_Interactable;
			this.expandArrow.TargetImage.color = this.TextColor_Interactable;
		}
		else
		{
			this.elements.LabelText.color = this.TextColor_NonInteractable;
			this.elements.QuantityText.color = this.TextColor_NonInteractable;
			this.expandArrow.ActiveColour = this.TextColor_NonInteractable;
			this.expandArrow.InactiveColour = this.TextColor_NonInteractable;
			this.expandArrow.TargetImage.color = this.TextColor_NonInteractable;
		}
	}

	public void SetTag(Tag t, GameUtil.MeasureUnit measure)
	{
		this.ResourceCategoryTag = t;
		this.Measure = measure;
		this.elements.LabelText.text = t.ProperName();
		if (SaveGame.Instance.expandedResourceTags.Contains(this.ResourceCategoryTag))
		{
			this.anyDiscovered = true;
			this.ToggleOpen(false);
		}
	}

	private void ToggleOpen(bool play_sound)
	{
		if (!this.anyDiscovered)
		{
			if (play_sound)
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
			}
			return;
		}
		if (!this.IsOpen)
		{
			if (play_sound)
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Open", false));
			}
			this.SetOpen(true);
			this.elements.LabelText.fontSize = (float)this.maximizedFontSize;
			this.elements.QuantityText.fontSize = (float)this.maximizedFontSize;
		}
		else
		{
			if (play_sound)
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
			}
			this.SetOpen(false);
			this.elements.LabelText.fontSize = (float)this.minimizedFontSize;
			this.elements.QuantityText.fontSize = (float)this.minimizedFontSize;
		}
	}

	private void Hover(bool is_hovering)
	{
		if (is_hovering)
		{
			this.Background.color = this.BackgroundHoverColor;
		}
		else
		{
			this.Background.color = new Color(0f, 0f, 0f, 0f);
		}
		List<Pickupable> list = null;
		if (WorldInventory.Instance != null)
		{
			list = WorldInventory.Instance.GetPickupables(this.ResourceCategoryTag);
		}
		if (list == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (!(list[i] == null))
			{
				KAnimControllerBase component = list[i].GetComponent<KAnimControllerBase>();
				if (!(component == null))
				{
					if (is_hovering)
					{
						component.HighlightColour = this.highlightColour;
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

	public void SetOpen(bool open)
	{
		this.IsOpen = open;
		if (open)
		{
			this.expandArrow.SetActive();
			if (!SaveGame.Instance.expandedResourceTags.Contains(this.ResourceCategoryTag))
			{
				SaveGame.Instance.expandedResourceTags.Add(this.ResourceCategoryTag);
			}
		}
		else
		{
			this.expandArrow.SetInactive();
			SaveGame.Instance.expandedResourceTags.Remove(this.ResourceCategoryTag);
		}
		this.EntryContainer.gameObject.SetActive(this.IsOpen);
	}

	private void GetAmounts(bool doExtras, out float available, out float total, out float reserved)
	{
		available = 0f;
		total = 0f;
		reserved = 0f;
		HashSet<Tag> hashSet = null;
		if (!WorldInventory.Instance.TryGetDiscoveredResourcesFromTag(this.ResourceCategoryTag, out hashSet))
		{
			return;
		}
		foreach (Tag tag in hashSet)
		{
			this.anyDiscovered = true;
			if (!this.ResourcesDiscovered.ContainsKey(tag))
			{
				this.ResourcesDiscovered.Add(tag, this.NewResourceEntry(tag, this.Measure));
			}
			float num = WorldInventory.Instance.GetAmount(tag);
			float num2 = ((!doExtras) ? 0f : WorldInventory.Instance.GetTotalAmount(tag));
			float num3 = ((!doExtras) ? 0f : MaterialNeeds.Instance.GetAmount(tag));
			if (this.Measure == GameUtil.MeasureUnit.kcal)
			{
				EdiblesManager.FoodInfo foodInfo = EdiblesManager.instance.GetFoodInfo(tag.Name);
				num *= foodInfo.CaloriesPerUnit;
				num2 *= foodInfo.CaloriesPerUnit;
				num3 *= foodInfo.CaloriesPerUnit;
			}
			available += num;
			total += num2;
			reserved += num3;
		}
	}

	public void UpdateContents()
	{
		float num;
		float num2;
		float num3;
		this.GetAmounts(false, out num, out num2, out num3);
		if (num != this.cachedAvailable || num2 != this.cachedTotal || num3 != this.cachedReserved)
		{
			if (this.quantityString == null || this.currentQuantity != num)
			{
				GameUtil.MeasureUnit measure = this.Measure;
				if (measure != GameUtil.MeasureUnit.mass)
				{
					if (measure != GameUtil.MeasureUnit.quantity)
					{
						if (measure == GameUtil.MeasureUnit.kcal)
						{
							this.quantityString = GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true);
						}
					}
					else
					{
						this.quantityString = num.ToString();
					}
				}
				else
				{
					this.quantityString = GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
				}
				this.elements.QuantityText.text = this.quantityString;
				this.currentQuantity = num;
			}
			this.cachedAvailable = num;
			this.cachedTotal = num2;
			this.cachedReserved = num3;
		}
		foreach (KeyValuePair<Tag, ResourceEntry> keyValuePair in this.ResourcesDiscovered)
		{
			keyValuePair.Value.UpdateValue();
		}
		this.SetActiveColor(num > 0f);
		if (!this.anyDiscovered)
		{
			this.SetInteractable(false);
		}
		else
		{
			this.SetInteractable(true);
		}
	}

	private string OnTooltip()
	{
		float num;
		float num2;
		float num3;
		this.GetAmounts(true, out num, out num2, out num3);
		string text = this.elements.LabelText.text + "\n";
		return text + string.Format(UI.RESOURCESCREEN.AVAILABLE_TOOLTIP, ResourceCategoryScreen.QuantityTextForMeasure(num, this.Measure), ResourceCategoryScreen.QuantityTextForMeasure(num3, this.Measure), ResourceCategoryScreen.QuantityTextForMeasure(num2, this.Measure));
	}

	private ResourceEntry NewResourceEntry(Tag resourceTag, GameUtil.MeasureUnit measure)
	{
		GameObject gameObject = Util.KInstantiateUI(this.Prefab_ResourceEntry, this.EntryContainer.gameObject, true);
		ResourceEntry component = gameObject.GetComponent<ResourceEntry>();
		component.SetTag(resourceTag, measure);
		return component;
	}

	public GameObject Prefab_ResourceEntry;

	public Transform EntryContainer;

	public Tag ResourceCategoryTag;

	public GameUtil.MeasureUnit Measure;

	public bool IsOpen;

	public ImageToggleState expandArrow;

	private Button mButton;

	public Dictionary<Tag, ResourceEntry> ResourcesDiscovered = new Dictionary<Tag, ResourceEntry>();

	public ResourceCategoryHeader.ElementReferences elements;

	public Color TextColor_Interactable;

	public Color TextColor_NonInteractable;

	private string quantityString;

	private float currentQuantity;

	private bool anyDiscovered;

	[MyCmpGet]
	private ToolTip tooltip;

	[SerializeField]
	private int minimizedFontSize;

	[SerializeField]
	private int maximizedFontSize;

	[SerializeField]
	private Color highlightColour;

	[SerializeField]
	private Color BackgroundHoverColor;

	[SerializeField]
	private Image Background;

	private float cachedAvailable = float.MinValue;

	private float cachedTotal = float.MinValue;

	private float cachedReserved = float.MinValue;

	[Serializable]
	public struct ElementReferences
	{
		public LocText LabelText;

		public LocText QuantityText;
	}
}
