using System;
using System.Collections.Generic;
using Database;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class OutfitDescriptionPanel : KMonoBehaviour
{
	public void Refresh(PermitResource permitResource, ClothingOutfitUtility.OutfitType outfitType)
	{
		if (permitResource != null)
		{
			this.Refresh(permitResource.Name, new string[] { permitResource.Id }, outfitType);
			return;
		}
		this.Refresh(UI.OUTFIT_NAME.NONE, OutfitDescriptionPanel.NO_ITEMS, outfitType);
	}

	public void Refresh(Option<ClothingOutfitTarget> outfit, ClothingOutfitUtility.OutfitType outfitType)
	{
		if (outfit.HasValue)
		{
			this.Refresh(outfit.Value.ReadName(), outfit.Value.ReadItems(), outfitType);
			return;
		}
		if (outfitType == ClothingOutfitUtility.OutfitType.Clothing)
		{
			this.Refresh(UI.OUTFIT_NAME.NONE, OutfitDescriptionPanel.NO_ITEMS, outfitType);
			return;
		}
		if (outfitType != ClothingOutfitUtility.OutfitType.JoyResponse)
		{
			return;
		}
		this.Refresh(UI.OUTFIT_NAME.NONE_JOY_RESPONSE, OutfitDescriptionPanel.NO_ITEMS, outfitType);
	}

	public void Refresh(OutfitDesignerScreen_OutfitState outfitState, ClothingOutfitUtility.OutfitType outfitType)
	{
		this.Refresh(outfitState.name, outfitState.GetItems(), outfitType);
	}

	public void Refresh(string name, string[] itemIds, ClothingOutfitUtility.OutfitType outfitType)
	{
		this.ClearItemDescRows();
		using (DictionaryPool<PermitCategory, Option<PermitResource>, OutfitDescriptionPanel>.PooledDictionary pooledDictionary = PoolsFor<OutfitDescriptionPanel>.AllocateDict<PermitCategory, Option<PermitResource>>())
		{
			using (ListPool<PermitResource, OutfitDescriptionPanel>.PooledList pooledList = PoolsFor<OutfitDescriptionPanel>.AllocateList<PermitResource>())
			{
				ClothingOutfitUtility.OutfitType outfitType2 = outfitType;
				if (outfitType2 != ClothingOutfitUtility.OutfitType.Clothing)
				{
					if (outfitType2 == ClothingOutfitUtility.OutfitType.JoyResponse)
					{
						if (itemIds != null && itemIds.Length != 0)
						{
							if (Db.Get().Permits.BalloonArtistFacades.TryGet(itemIds[0]) != null)
							{
								this.outfitDescriptionLabel.gameObject.SetActive(true);
								string text = DUPLICANTS.TRAITS.BALLOONARTIST.NAME;
								this.outfitNameLabel.SetText(text);
								this.outfitDescriptionLabel.SetText(name);
							}
						}
						else
						{
							this.outfitNameLabel.SetText(name);
							this.outfitDescriptionLabel.gameObject.SetActive(false);
						}
						pooledDictionary.Add(PermitCategory.JoyResponse, Option.None);
					}
				}
				else
				{
					this.outfitNameLabel.SetText(name);
					this.outfitDescriptionLabel.gameObject.SetActive(false);
					pooledDictionary.Add(PermitCategory.DupeTops, Option.None);
					pooledDictionary.Add(PermitCategory.DupeGloves, Option.None);
					pooledDictionary.Add(PermitCategory.DupeBottoms, Option.None);
					pooledDictionary.Add(PermitCategory.DupeShoes, Option.None);
				}
				foreach (string text2 in itemIds)
				{
					PermitResource permitResource = Db.Get().Permits.Get(text2);
					Option<PermitResource> option;
					if (pooledDictionary.TryGetValue(permitResource.Category, out option) && !option.HasValue)
					{
						pooledDictionary[permitResource.Category] = permitResource;
					}
					else
					{
						pooledList.Add(permitResource);
					}
				}
				foreach (KeyValuePair<PermitCategory, Option<PermitResource>> keyValuePair in pooledDictionary)
				{
					PermitCategory permitCategory;
					Option<PermitResource> option2;
					keyValuePair.Deconstruct<PermitCategory, Option<PermitResource>>(out permitCategory, out option2);
					PermitCategory permitCategory2 = permitCategory;
					Option<PermitResource> option3 = option2;
					if (option3.HasValue)
					{
						this.AddItemDescRow(option3.Value);
					}
					else
					{
						this.AddItemDescRow(KleiItemsUI.GetNoneClothingItemIcon(permitCategory2), KleiItemsUI.GetNoneClothingItemString(permitCategory2), null, 1f);
					}
				}
				foreach (PermitResource permitResource2 in pooledList)
				{
					ClothingItemResource clothingItemResource = (ClothingItemResource)permitResource2;
					this.AddItemDescRow(clothingItemResource);
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
			this.Refresh(name, itemIds, outfitType);
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

	private void AddItemDescRow(PermitResource permit)
	{
		PermitPresentationInfo permitPresentationInfo = permit.GetPermitPresentationInfo();
		bool flag = permit.IsUnlocked();
		string text = (flag ? null : UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWN_NONE);
		this.AddItemDescRow(permitPresentationInfo.sprite, permit.Name, text, flag ? 1f : 0.7f);
	}

	private void AddItemDescRow(Sprite icon, string text, string tooltip = null, float alpha = 1f)
	{
		GameObject gameObject = Util.KInstantiateUI(this.itemDescriptionRowPrefab, this.itemDescriptionContainer, true);
		this.itemDescriptionRows.Add(gameObject);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		component.GetReference<Image>("Icon").sprite = icon;
		component.GetReference<LocText>("Label").SetText(text);
		gameObject.AddOrGet<CanvasGroup>().alpha = alpha;
		gameObject.AddOrGet<NonDrawingGraphic>();
		if (tooltip != null)
		{
			gameObject.AddOrGet<ToolTip>().SetSimpleTooltip(tooltip);
			return;
		}
		gameObject.AddOrGet<ToolTip>().ClearMultiStringTooltip();
	}

	[SerializeField]
	public LocText outfitNameLabel;

	[SerializeField]
	public LocText outfitDescriptionLabel;

	[SerializeField]
	private GameObject itemDescriptionRowPrefab;

	[SerializeField]
	private GameObject itemDescriptionContainer;

	[SerializeField]
	private LocText usesUnownedItemsLabel;

	private List<GameObject> itemDescriptionRows = new List<GameObject>();

	public static readonly string[] NO_ITEMS = new string[0];
}
