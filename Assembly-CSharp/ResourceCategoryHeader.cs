using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResourceCategoryHeader : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	private void Awake()
	{
		this.EntryContainer.SetParent(base.transform.parent);
		this.EntryContainer.SetSiblingIndex(base.transform.GetSiblingIndex() + 1);
		this.EntryContainer.localScale = Vector3.one;
		this.mButton = base.GetComponent<Button>();
		this.mButton.onClick.AddListener(delegate
		{
			this.ToggleOpen();
		});
		this.SetInteractable(false);
		this.SetActiveColor(false);
		this.tooltip = base.GetComponent<ToolTip>();
	}

	private void Start()
	{
		this.UpdateContents();
	}

	private void SetInteractable(bool state)
	{
		if (state)
		{
			if (!this.IsOpen)
			{
				this.expandArrow.SetInactive();
				if (this.tooltip != null)
				{
					this.tooltip.toolTip = "Click to expand";
				}
			}
			else
			{
				this.expandArrow.SetActive();
				if (this.tooltip != null)
				{
					this.tooltip.toolTip = "Click to collapse";
				}
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

	public void SetTag(Tag t)
	{
		this.ResourceCategoryTag = t;
		this.elements.LabelText.text = t.ProperName();
	}

	private void ToggleOpen()
	{
		if (!this.anyDiscovered)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
			return;
		}
		if (!this.IsOpen)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Open", false));
			this.SetOpen(true);
			this.elements.LabelText.fontSize = (float)this.maximizedFontSize;
			this.elements.QuantityText.fontSize = (float)this.maximizedFontSize;
		}
		else
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
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
		List<Pickupable> pickupables = WorldInventory.Instance.GetPickupables(this.ResourceCategoryTag);
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
		}
		else
		{
			this.expandArrow.SetInactive();
		}
		this.EntryContainer.gameObject.SetActive(this.IsOpen);
	}

	public void UpdateContents()
	{
		float num = 0f;
		foreach (KeyValuePair<Tag, ResourceEntry> keyValuePair in this.ResourcesDiscovered)
		{
			if (WorldInventory.Instance.IsDiscovered(keyValuePair.Key))
			{
				this.anyDiscovered = true;
			}
			if (this.measure == ResourceCategoryHeader.MeasureUnit.quantity || this.measure == ResourceCategoryHeader.MeasureUnit.mass)
			{
				num += WorldInventory.Instance.GetAmount(keyValuePair.Key);
			}
		}
		List<Pickupable> pickupables = WorldInventory.Instance.GetPickupables(this.ResourceCategoryTag);
		if ((this.measure == ResourceCategoryHeader.MeasureUnit.kcal || (!this.anyDiscovered && this.isPickupableCategory)) && pickupables != null)
		{
			foreach (Pickupable pickupable in pickupables)
			{
				if (this.measure == ResourceCategoryHeader.MeasureUnit.kcal)
				{
					Edible component = pickupable.GetComponent<Edible>();
					if (component != null)
					{
						num += component.rations * 100000f;
					}
				}
				else if (pickupable != null)
				{
					this.anyDiscovered = true;
					num += pickupable.TotalAmount;
				}
			}
		}
		this.SetActiveColor(num > 0f);
		string text = string.Empty;
		switch (this.measure)
		{
		case ResourceCategoryHeader.MeasureUnit.mass:
			text = GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.None, true, "F1");
			break;
		case ResourceCategoryHeader.MeasureUnit.kcal:
			text = GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true);
			break;
		case ResourceCategoryHeader.MeasureUnit.quantity:
			text = num.ToString();
			break;
		}
		this.elements.QuantityText.text = text;
		if (!this.anyDiscovered)
		{
			this.SetInteractable(false);
		}
		else
		{
			this.SetInteractable(true);
		}
		if (!this.isPickupableCategory)
		{
			foreach (KeyValuePair<Tag, Tag> keyValuePair2 in WorldInventory.Instance.GetDiscoveredResourceTags())
			{
				if (keyValuePair2.Value == this.ResourceCategoryTag)
				{
					if (!this.ResourcesDiscovered.ContainsKey(keyValuePair2.Key))
					{
						this.ResourcesDiscovered.Add(keyValuePair2.Key, this.NewResourceEntry(keyValuePair2.Key));
					}
					else
					{
						this.ResourcesDiscovered[keyValuePair2.Key].UpdateValue(this.isPickupableCategory, this.measure);
					}
				}
			}
		}
		if (this.isPickupableCategory && pickupables != null)
		{
			foreach (Pickupable pickupable2 in pickupables)
			{
				if (!(pickupable2 == null))
				{
					Tag tag = new Tag(pickupable2.GetComponent<KPrefabID>().Tags[0]);
					if (!this.ResourcesDiscovered.ContainsKey(tag))
					{
						Element element = ElementLoader.GetElement(tag);
						if (element != null)
						{
							this.ResourcesDiscovered.Add(tag, this.NewResourceEntry(tag));
						}
						else
						{
							Tag tag2 = pickupable2.GetComponent<KPrefabID>().PrefabID();
							GameObject prefab = Assets.GetPrefab(tag2);
							if (prefab != null)
							{
								this.ResourcesDiscovered.Add(tag, this.NewResourceEntry(tag, Assets.GetPrefab(tag2)));
							}
						}
					}
					else
					{
						this.ResourcesDiscovered[tag].UpdateValue(true, this.measure);
					}
				}
			}
		}
	}

	private ResourceEntry NewResourceEntry(Tag resourceTag)
	{
		GameObject gameObject = Util.KInstantiateUI(this.Prefab_ResourceEntry, this.EntryContainer.gameObject, true);
		ResourceEntry component = gameObject.GetComponent<ResourceEntry>();
		component.SetTag(resourceTag);
		return component;
	}

	private ResourceEntry NewResourceEntry(Tag resourceTag, GameObject prefab)
	{
		return this.NewResourceEntry(resourceTag);
	}

	public GameObject Prefab_ResourceEntry;

	public Transform EntryContainer;

	public Tag ResourceCategoryTag;

	public bool IsOpen;

	public ImageToggleState expandArrow;

	private Button mButton;

	public bool isPickupableCategory;

	public Dictionary<Tag, ResourceEntry> ResourcesDiscovered = new Dictionary<Tag, ResourceEntry>();

	public ResourceCategoryHeader.ElementReferences elements;

	public Color TextColor_Interactable;

	public Color TextColor_NonInteractable;

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

	public ResourceCategoryHeader.MeasureUnit measure;

	private bool anyDiscovered;

	public enum MeasureUnit
	{
		mass,
		kcal,
		quantity
	}

	[Serializable]
	public struct ElementReferences
	{
		public LocText LabelText;

		public LocText QuantityText;
	}
}
