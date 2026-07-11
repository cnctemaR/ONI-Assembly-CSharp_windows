using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/ResourceCategoryHeader")]
public class ResourceCategoryHeader : KMonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
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
		this.SetInteractable(this.anyDiscovered);
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
		if (!state)
		{
			this.SetOpen(false);
			this.expandArrow.SetDisabled();
			return;
		}
		if (!this.IsOpen)
		{
			this.expandArrow.SetInactive();
			return;
		}
		this.expandArrow.SetActive();
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
			return;
		}
		this.elements.LabelText.color = this.TextColor_NonInteractable;
		this.elements.QuantityText.color = this.TextColor_NonInteractable;
		this.expandArrow.ActiveColour = this.TextColor_NonInteractable;
		this.expandArrow.InactiveColour = this.TextColor_NonInteractable;
		this.expandArrow.TargetImage.color = this.TextColor_NonInteractable;
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
			return;
		}
		if (play_sound)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
		}
		this.SetOpen(false);
		this.elements.LabelText.fontSize = (float)this.minimizedFontSize;
		this.elements.QuantityText.fontSize = (float)this.minimizedFontSize;
	}

	private void Hover(bool is_hovering)
	{
		this.Background.color = (is_hovering ? this.BackgroundHoverColor : new Color(0f, 0f, 0f, 0f));
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
					component.HighlightColour = (is_hovering ? this.highlightColour : Color.black);
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
		ListPool<Tag, ResourceCategoryHeader>.PooledList pooledList = ListPool<Tag, ResourceCategoryHeader>.Allocate();
		foreach (Tag tag in hashSet)
		{
			EdiblesManager.FoodInfo foodInfo = null;
			if (this.Measure == GameUtil.MeasureUnit.kcal)
			{
				foodInfo = EdiblesManager.GetFoodInfo(tag.Name);
				if (foodInfo == null)
				{
					pooledList.Add(tag);
					continue;
				}
			}
			this.anyDiscovered = true;
			ResourceEntry resourceEntry = null;
			if (!this.ResourcesDiscovered.TryGetValue(tag, out resourceEntry))
			{
				resourceEntry = this.NewResourceEntry(tag, this.Measure);
				this.ResourcesDiscovered.Add(tag, resourceEntry);
			}
			float num;
			float num2;
			float num3;
			resourceEntry.GetAmounts(foodInfo, doExtras, out num, out num2, out num3);
			available += num;
			total += num2;
			reserved += num3;
		}
		foreach (Tag tag2 in pooledList)
		{
			hashSet.Remove(tag2);
		}
		pooledList.Recycle();
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
				switch (this.Measure)
				{
				case GameUtil.MeasureUnit.mass:
					this.quantityString = GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
					break;
				case GameUtil.MeasureUnit.kcal:
					this.quantityString = GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true);
					break;
				case GameUtil.MeasureUnit.quantity:
					this.quantityString = num.ToString();
					break;
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
		this.SetInteractable(this.anyDiscovered);
	}

	private string OnTooltip()
	{
		float num;
		float num2;
		float num3;
		this.GetAmounts(true, out num, out num2, out num3);
		return this.elements.LabelText.text + "\n" + string.Format(UI.RESOURCESCREEN.AVAILABLE_TOOLTIP, ResourceCategoryScreen.QuantityTextForMeasure(num, this.Measure), ResourceCategoryScreen.QuantityTextForMeasure(num3, this.Measure), ResourceCategoryScreen.QuantityTextForMeasure(num2, this.Measure));
	}

	private ResourceEntry NewResourceEntry(Tag resourceTag, GameUtil.MeasureUnit measure)
	{
		ResourceEntry component = Util.KInstantiateUI(this.Prefab_ResourceEntry, this.EntryContainer.gameObject, true).GetComponent<ResourceEntry>();
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
