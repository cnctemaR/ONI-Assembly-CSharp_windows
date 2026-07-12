using System;
using System.Collections.Generic;
using Database;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class OutfitDescriptionPanel : KMonoBehaviour
{
	public void Refresh(Option<ClothingOutfitTarget> outfit)
	{
		if (outfit.HasValue)
		{
			this.Refresh(outfit.Value.ReadName(), outfit.Value.ReadItems());
			return;
		}
		this.Refresh(UI.OUTFIT_NAME.NONE, OutfitDescriptionPanel.NO_ITEMS);
	}

	public void Refresh(OutfitDesignerScreen_OutfitState outfitState)
	{
		this.Refresh(outfitState.name, outfitState.GetItems());
	}

	public void Refresh(string name, string[] itemIds)
	{
		this.outfitNameLabel.SetText(name);
		this.ClearItemDescRows();
		using (DictionaryPool<PermitCategory, Option<ClothingItemResource>, OutfitDescriptionPanel>.PooledDictionary pooledDictionary = PoolsFor<OutfitDescriptionPanel>.AllocateDict<PermitCategory, Option<ClothingItemResource>>())
		{
			using (ListPool<ClothingItemResource, OutfitDescriptionPanel>.PooledList pooledList = PoolsFor<OutfitDescriptionPanel>.AllocateList<ClothingItemResource>())
			{
				pooledDictionary.Add(PermitCategory.DupeTops, Option.None);
				pooledDictionary.Add(PermitCategory.DupeGloves, Option.None);
				pooledDictionary.Add(PermitCategory.DupeBottoms, Option.None);
				pooledDictionary.Add(PermitCategory.DupeShoes, Option.None);
				foreach (string text in itemIds)
				{
					ClothingItemResource clothingItemResource = (ClothingItemResource)Db.Get().Permits.Get(text);
					Option<ClothingItemResource> option;
					if (pooledDictionary.TryGetValue(clothingItemResource.PermitCategory, out option) && !option.HasValue)
					{
						pooledDictionary[clothingItemResource.PermitCategory] = clothingItemResource;
					}
					else
					{
						pooledList.Add(clothingItemResource);
					}
				}
				foreach (KeyValuePair<PermitCategory, Option<ClothingItemResource>> keyValuePair in pooledDictionary)
				{
					PermitCategory permitCategory;
					Option<ClothingItemResource> option2;
					keyValuePair.Deconstruct<PermitCategory, Option<ClothingItemResource>>(out permitCategory, out option2);
					PermitCategory permitCategory2 = permitCategory;
					Option<ClothingItemResource> option3 = option2;
					if (option3.HasValue)
					{
						this.AddItemDescRow(option3.Value.GetPermitPresentationInfo());
					}
					else
					{
						this.AddItemDescRow(KleiItemsUI.GetNoneClothingItemIcon(permitCategory2), KleiItemsUI.GetNoneClothingItemString(permitCategory2), default(Option<string>), 1f);
					}
				}
				foreach (ClothingItemResource clothingItemResource2 in pooledList)
				{
					this.AddItemDescRow(clothingItemResource2.GetPermitPresentationInfo());
				}
			}
		}
		if (!ClothingOutfitTarget.DoesContainNonOwnedItems(itemIds))
		{
			this.usesUnownedItemsLabel.gameObject.SetActive(false);
		}
		else
		{
			this.usesUnownedItemsLabel.transform.SetAsLastSibling();
			this.usesUnownedItemsLabel.SetText(KleiItemsUI.WrapWithColor(UI.OUTFIT_DESCRIPTION.CONTAINS_NON_OWNED_ITEMS, KleiItemsUI.TEXT_COLOR__PERMIT_NOT_OWNED));
			this.usesUnownedItemsLabel.gameObject.SetActive(true);
		}
		KleiItemsStatusRefresher.AddOrGetListener(this).OnRefreshUI(delegate
		{
			this.Refresh(name, itemIds);
		});
	}

	private void ClearItemDescRows()
	{
		for (int i = 0; i < this.itemDescriptionRows.Count; i++)
		{
			global::UnityEngine.Object.Destroy(this.itemDescriptionRows[i]);
		}
		this.itemDescriptionRows.Clear();
	}

	private void AddItemDescRow(PermitPresentationInfo presInfo)
	{
		Option<string> option = (presInfo.IsUnlocked() ? Option.None : Option.Some<string>(UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWN_NONE));
		this.AddItemDescRow(presInfo.sprite, presInfo.name, option, presInfo.IsUnlocked() ? 1f : 0.7f);
	}

	private void AddItemDescRow(Sprite icon, string text, Option<string> tooltip = default(Option<string>), float alpha = 1f)
	{
		GameObject gameObject = Util.KInstantiateUI(this.itemDescriptionRowPrefab, this.itemDescriptionContainer, true);
		this.itemDescriptionRows.Add(gameObject);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		component.GetReference<Image>("Icon").sprite = icon;
		component.GetReference<LocText>("Label").SetText(text);
		gameObject.AddOrGet<CanvasGroup>().alpha = alpha;
		gameObject.AddOrGet<NonDrawingGraphic>();
		if (tooltip.HasValue)
		{
			gameObject.AddOrGet<ToolTip>().SetSimpleTooltip(tooltip.Value);
			return;
		}
		gameObject.AddOrGet<ToolTip>().ClearMultiStringTooltip();
	}

	[SerializeField]
	public LocText outfitNameLabel;

	[SerializeField]
	private GameObject itemDescriptionRowPrefab;

	[SerializeField]
	private GameObject itemDescriptionContainer;

	[SerializeField]
	private LocText usesUnownedItemsLabel;

	private List<GameObject> itemDescriptionRows = new List<GameObject>();

	public static readonly string[] NO_ITEMS = new string[0];
}
