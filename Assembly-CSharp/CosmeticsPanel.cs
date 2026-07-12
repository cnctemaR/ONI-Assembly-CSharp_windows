using System;
using System.Collections.Generic;
using Database;
using FMOD.Studio;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class CosmeticsPanel : TargetPanel
{
	public override bool IsValidForTarget(GameObject target)
	{
		return true;
	}

	protected override void OnSelectTarget(GameObject target)
	{
		base.OnSelectTarget(target);
		BuildingFacade buildingFacade = this.selectedTarget.GetComponent<BuildingFacade>();
		MinionIdentity component = this.selectedTarget.GetComponent<MinionIdentity>();
		this.selectionPanel.OnFacadeSelectionChanged = null;
		this.outfitCategoryButtonContainer.SetActive(component != null);
		if (component != null)
		{
			ClothingOutfitTarget outfitTarget = ClothingOutfitTarget.FromMinion(this.selectedOutfitCategory, component.gameObject);
			this.selectionPanel.SetOutfitTarget(outfitTarget, this.selectedOutfitCategory);
			FacadeSelectionPanel facadeSelectionPanel = this.selectionPanel;
			facadeSelectionPanel.OnFacadeSelectionChanged = (global::System.Action)Delegate.Combine(facadeSelectionPanel.OnFacadeSelectionChanged, new global::System.Action(delegate
			{
				if (this.selectionPanel.SelectedFacade == null || this.selectionPanel.SelectedFacade == "DEFAULT_FACADE")
				{
					outfitTarget.WriteItems(this.selectedOutfitCategory, new string[0]);
				}
				else
				{
					outfitTarget.WriteItems(this.selectedOutfitCategory, ClothingOutfitTarget.FromTemplateId(this.selectionPanel.SelectedFacade).ReadItems());
				}
				this.Refresh();
			}));
		}
		else if (buildingFacade != null)
		{
			this.selectionPanel.SetBuildingDef(this.selectedTarget.GetComponent<Building>().Def.PrefabID, this.selectedTarget.GetComponent<BuildingFacade>().CurrentFacade);
			this.selectionPanel.OnFacadeSelectionChanged = null;
			FacadeSelectionPanel facadeSelectionPanel2 = this.selectionPanel;
			facadeSelectionPanel2.OnFacadeSelectionChanged = (global::System.Action)Delegate.Combine(facadeSelectionPanel2.OnFacadeSelectionChanged, new global::System.Action(delegate
			{
				if (this.selectionPanel.SelectedFacade == null || this.selectionPanel.SelectedFacade == "DEFAULT_FACADE" || Db.GetBuildingFacades().TryGet(this.selectionPanel.SelectedFacade).IsNullOrDestroyed())
				{
					buildingFacade.ApplyDefaultFacade(true);
				}
				else
				{
					buildingFacade.ApplyBuildingFacade(Db.GetBuildingFacades().Get(this.selectionPanel.SelectedFacade), true);
				}
				this.Refresh();
			}));
		}
		this.Refresh();
	}

	public override void OnDeselectTarget(GameObject target)
	{
		base.OnDeselectTarget(target);
	}

	public void Refresh()
	{
		global::UnityEngine.Object component = this.selectedTarget.GetComponent<MinionIdentity>();
		BuildingFacade component2 = this.selectedTarget.GetComponent<BuildingFacade>();
		if (component != null)
		{
			ClothingOutfitTarget clothingOutfitTarget = ClothingOutfitTarget.FromMinion(this.selectedOutfitCategory, this.selectedTarget);
			this.editButton.gameObject.SetActive(true);
			this.mannequin.gameObject.SetActive(true);
			this.mannequin.SetOutfit(clothingOutfitTarget);
			this.buildingIcon.gameObject.SetActive(false);
			this.editButton.ClearOnClick();
			this.editButton.onClick += this.OnClickEditOutfit;
			this.nameLabel.SetText(clothingOutfitTarget.ReadName());
			this.descriptionLabel.SetText("");
		}
		else if (component2 != null)
		{
			this.editButton.gameObject.SetActive(false);
			this.mannequin.gameObject.SetActive(false);
			this.buildingIcon.gameObject.SetActive(true);
			if (component2.CurrentFacade != null && component2.CurrentFacade != "DEFAULT_FACADE" && !Db.GetBuildingFacades().TryGet(component2.CurrentFacade).IsNullOrDestroyed())
			{
				BuildingFacadeResource buildingFacadeResource = Db.GetBuildingFacades().Get(component2.CurrentFacade);
				this.nameLabel.SetText(buildingFacadeResource.Name);
				this.descriptionLabel.SetText(buildingFacadeResource.Description);
				this.buildingIcon.sprite = Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim(buildingFacadeResource.AnimFile), "ui", false, "");
			}
			else
			{
				string prefabID = component2.GetComponent<Building>().Def.PrefabID;
				StringEntry stringEntry;
				Strings.TryGet(string.Concat(new string[]
				{
					"STRINGS.BUILDINGS.PREFABS.",
					prefabID.ToString().ToUpperInvariant(),
					".FACADES.DEFAULT_",
					prefabID.ToString().ToUpperInvariant(),
					".NAME"
				}), out stringEntry);
				if (stringEntry == null)
				{
					Strings.TryGet("STRINGS.BUILDINGS.PREFABS." + prefabID.ToString().ToUpperInvariant() + ".NAME", out stringEntry);
				}
				StringEntry stringEntry2;
				Strings.TryGet(string.Concat(new string[]
				{
					"STRINGS.BUILDINGS.PREFABS.",
					prefabID.ToString().ToUpperInvariant(),
					".FACADES.DEFAULT_",
					prefabID.ToString().ToUpperInvariant(),
					".DESC"
				}), out stringEntry2);
				if (stringEntry2 == null)
				{
					Strings.TryGet("STRINGS.BUILDINGS.PREFABS." + prefabID.ToString().ToUpperInvariant() + ".DESC", out stringEntry2);
				}
				this.nameLabel.SetText((stringEntry != null) ? stringEntry : "");
				this.descriptionLabel.SetText((stringEntry2 != null) ? stringEntry2 : "");
				this.buildingIcon.sprite = Def.GetUISprite(prefabID, "ui", false).first;
			}
		}
		this.RefreshOutfitCategories();
		this.selectionPanel.Refresh();
	}

	public void OnClickEditOutfit()
	{
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndSupplyClosetSnapshot);
		MinionBrowserScreenConfig.MinionInstances(this.selectedTarget).ApplyAndOpenScreen(delegate
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndSupplyClosetSnapshot, STOP_MODE.ALLOWFADEOUT);
		});
	}

	private void RefreshOutfitCategories()
	{
		foreach (KeyValuePair<ClothingOutfitUtility.OutfitType, GameObject> keyValuePair in this.outfitCategories)
		{
			global::Util.KDestroyGameObject(keyValuePair.Value);
		}
		this.outfitCategories.Clear();
		string[] array = new string[] { "outfit", "atmosuit" };
		Dictionary<ClothingOutfitUtility.OutfitType, string> dictionary = new Dictionary<ClothingOutfitUtility.OutfitType, string>();
		dictionary.Add(ClothingOutfitUtility.OutfitType.Clothing, UI.UISIDESCREENS.BLUEPRINT_TAB.SUBCATEGORY_OUTFIT);
		dictionary.Add(ClothingOutfitUtility.OutfitType.AtmoSuit, UI.UISIDESCREENS.BLUEPRINT_TAB.SUBCATEGORY_ATMOSUIT);
		for (int i = 0; i < 3; i++)
		{
			if (i != 1)
			{
				int idx = i;
				GameObject gameObject = global::Util.KInstantiateUI(this.outfitCategoryButtonPrefab, this.outfitCategoryButtonContainer, true);
				this.outfitCategories.Add((ClothingOutfitUtility.OutfitType)idx, gameObject);
				gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("Label").SetText(dictionary[(ClothingOutfitUtility.OutfitType)i]);
				MultiToggle component = gameObject.GetComponent<MultiToggle>();
				component.onClick = (global::System.Action)Delegate.Combine(component.onClick, new global::System.Action(delegate
				{
					this.selectedOutfitCategory = (ClothingOutfitUtility.OutfitType)idx;
					this.Refresh();
					this.selectionPanel.SelectedOutfitCategory = this.selectedOutfitCategory;
				}));
				component.ChangeState((this.selectedOutfitCategory == (ClothingOutfitUtility.OutfitType)idx) ? 1 : 0);
			}
		}
	}

	[SerializeField]
	private GameObject cosmeticSlotContainer;

	[SerializeField]
	private FacadeSelectionPanel selectionPanel;

	[SerializeField]
	private LocText nameLabel;

	[SerializeField]
	private LocText descriptionLabel;

	[SerializeField]
	private KButton editButton;

	[SerializeField]
	private UIMannequin mannequin;

	[SerializeField]
	private Image buildingIcon;

	[SerializeField]
	private Dictionary<ClothingOutfitUtility.OutfitType, GameObject> outfitCategories = new Dictionary<ClothingOutfitUtility.OutfitType, GameObject>();

	[SerializeField]
	private GameObject outfitCategoryButtonPrefab;

	[SerializeField]
	private GameObject outfitCategoryButtonContainer;

	private ClothingOutfitUtility.OutfitType selectedOutfitCategory;
}
