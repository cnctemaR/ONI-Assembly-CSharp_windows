using System;
using System.Collections.Generic;
using System.Diagnostics;
using Klei.AI;
using ProcGen;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class SimpleInfoScreen : TargetScreen, ISim4000ms, ISim1000ms
{
	public GameObject StoragePanel { get; private set; }

	public SimpleInfoScreen()
	{
		this.onStorageChangeDelegate = new Action<object>(this.OnStorageChange);
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return true;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.processConditionContainer = global::Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.processConditionContainer.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.PROCESS_CONDITIONS.NAME;
		this.statusItemPanel = global::Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.statusItemPanel.Content.GetComponent<VerticalLayoutGroup>().padding.bottom = 10;
		this.statusItemPanel.HeaderLabel.text = UI.DETAILTABS.SIMPLEINFO.GROUPNAME_STATUS;
		this.statusItemPanel.scalerMask.hoverLock = true;
		this.statusItemsFolder = this.statusItemPanel.Content.gameObject;
		this.spaceSimpleInfoPOIPanel = new SpacePOISimpleInfoPanel(this);
		this.spacePOIPanel = global::Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.rocketSimpleInfoPanel = new RocketSimpleInfoPanel(this);
		this.rocketStatusContainer = global::Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.rocketStatusContainer.SetTitle(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_ROCKET);
		this.vitalsPanel = global::Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.vitalsPanel.SetTitle(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_CONDITION);
		this.vitalsContainer = global::Util.KInstantiateUI(this.VitalsPanelTemplate, this.vitalsPanel.Content.gameObject, false).GetComponent<MinionVitalsPanel>();
		this.fertilityPanel = global::Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.fertilityPanel.SetTitle(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_FERTILITY);
		this.infoPanel = global::Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.infoPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.SIMPLEINFO.GROUPNAME_DESCRIPTION;
		GameObject gameObject = this.infoPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject;
		this.descriptionContainer = global::Util.KInstantiateUI<DescriptionContainer>(this.DescriptionContainerTemplate, gameObject, false);
		this.worldLifePanel = global::Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.worldLifePanel.SetTitle(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_LIFE);
		this.worldTraitsPanel = global::Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.worldTraitsPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.SIMPLEINFO.GROUPNAME_WORLDTRAITS;
		this.worldElementsPanel = global::Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.worldElementsPanel.SetTitle(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_ELEMENTS);
		this.worldGeysersPanel = global::Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.worldGeysersPanel.SetTitle(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_GEYSERS);
		this.worldBiomesPanel = global::Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.worldBiomesPanel.SetTitle(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_BIOMES);
		this.StoragePanel = global::Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.stressPanel = global::Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.stressDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, this.stressPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
		this.stampContainer = global::Util.KInstantiateUI(this.StampContainerTemplate, gameObject, false);
		base.Subscribe<SimpleInfoScreen>(-1514841199, SimpleInfoScreen.OnRefreshDataDelegate);
	}

	public override void OnSelectTarget(GameObject target)
	{
		base.OnSelectTarget(target);
		base.Subscribe(target, -1697596308, this.onStorageChangeDelegate);
		base.Subscribe(target, -1197125120, this.onStorageChangeDelegate);
		this.RefreshStorage();
		base.Subscribe(target, 1059811075, new Action<object>(this.OnBreedingChanceChanged));
		this.RefreshBreedingChance();
		this.vitalsPanel.SetTitle((target.GetComponent<WiltCondition>() == null) ? UI.DETAILTABS.SIMPLEINFO.GROUPNAME_CONDITION : UI.DETAILTABS.SIMPLEINFO.GROUPNAME_REQUIREMENTS);
		KSelectable component = target.GetComponent<KSelectable>();
		if (component != null)
		{
			StatusItemGroup statusItemGroup = component.GetStatusItemGroup();
			if (statusItemGroup != null)
			{
				StatusItemGroup statusItemGroup2 = statusItemGroup;
				statusItemGroup2.OnAddStatusItem = (Action<StatusItemGroup.Entry, StatusItemCategory>)Delegate.Combine(statusItemGroup2.OnAddStatusItem, new Action<StatusItemGroup.Entry, StatusItemCategory>(this.OnAddStatusItem));
				StatusItemGroup statusItemGroup3 = statusItemGroup;
				statusItemGroup3.OnRemoveStatusItem = (Action<StatusItemGroup.Entry, bool>)Delegate.Combine(statusItemGroup3.OnRemoveStatusItem, new Action<StatusItemGroup.Entry, bool>(this.OnRemoveStatusItem));
				foreach (StatusItemGroup.Entry entry in statusItemGroup)
				{
					if (entry.category != null && entry.category.Id == "Main")
					{
						this.DoAddStatusItem(entry, entry.category, false);
					}
				}
				foreach (StatusItemGroup.Entry entry2 in statusItemGroup)
				{
					if (entry2.category == null || entry2.category.Id != "Main")
					{
						this.DoAddStatusItem(entry2, entry2.category, false);
					}
				}
			}
		}
		this.statusItemPanel.gameObject.SetActive(true);
		this.statusItemPanel.scalerMask.UpdateSize();
		this.Refresh(true);
		this.RefreshWorld();
		this.spaceSimpleInfoPOIPanel.Refresh(this.spacePOIPanel, this.selectedTarget);
	}

	public override void OnDeselectTarget(GameObject target)
	{
		base.OnDeselectTarget(target);
		if (target != null)
		{
			base.Unsubscribe(target, -1697596308, this.onStorageChangeDelegate);
			base.Unsubscribe(target, -1197125120, this.onStorageChangeDelegate);
			base.Unsubscribe(target, 1059811075, new Action<object>(this.OnBreedingChanceChanged));
		}
		KSelectable component = target.GetComponent<KSelectable>();
		if (component != null)
		{
			StatusItemGroup statusItemGroup = component.GetStatusItemGroup();
			if (statusItemGroup != null)
			{
				StatusItemGroup statusItemGroup2 = statusItemGroup;
				statusItemGroup2.OnAddStatusItem = (Action<StatusItemGroup.Entry, StatusItemCategory>)Delegate.Remove(statusItemGroup2.OnAddStatusItem, new Action<StatusItemGroup.Entry, StatusItemCategory>(this.OnAddStatusItem));
				StatusItemGroup statusItemGroup3 = statusItemGroup;
				statusItemGroup3.OnRemoveStatusItem = (Action<StatusItemGroup.Entry, bool>)Delegate.Remove(statusItemGroup3.OnRemoveStatusItem, new Action<StatusItemGroup.Entry, bool>(this.OnRemoveStatusItem));
				foreach (SimpleInfoScreen.StatusItemEntry statusItemEntry in this.statusItems)
				{
					statusItemEntry.Destroy(true);
				}
				this.statusItems.Clear();
				foreach (SimpleInfoScreen.StatusItemEntry statusItemEntry2 in this.oldStatusItems)
				{
					statusItemEntry2.onDestroy = null;
					statusItemEntry2.Destroy(true);
				}
				this.oldStatusItems.Clear();
			}
		}
	}

	private void OnStorageChange(object data)
	{
		this.RefreshStorage();
	}

	private void OnBreedingChanceChanged(object data)
	{
		this.RefreshBreedingChance();
	}

	private void OnAddStatusItem(StatusItemGroup.Entry status_item, StatusItemCategory category)
	{
		this.DoAddStatusItem(status_item, category, false);
	}

	private void DoAddStatusItem(StatusItemGroup.Entry status_item, StatusItemCategory category, bool show_immediate = false)
	{
		GameObject gameObject = this.statusItemsFolder;
		Color color;
		if (status_item.item.notificationType == NotificationType.BadMinor || status_item.item.notificationType == NotificationType.Bad || status_item.item.notificationType == NotificationType.DuplicantThreatening)
		{
			color = GlobalAssets.Instance.colorSet.statusItemBad;
		}
		else if (status_item.item.notificationType == NotificationType.Event)
		{
			color = GlobalAssets.Instance.colorSet.statusItemEvent;
		}
		else
		{
			color = this.statusItemTextColor_regular;
		}
		TextStyleSetting textStyleSetting = ((category == Db.Get().StatusItemCategories.Main) ? this.StatusItemStyle_Main : this.StatusItemStyle_Other);
		SimpleInfoScreen.StatusItemEntry statusItemEntry = new SimpleInfoScreen.StatusItemEntry(status_item, category, this.StatusItemPrefab, gameObject.transform, this.ToolTipStyle_Property, color, textStyleSetting, show_immediate, new Action<SimpleInfoScreen.StatusItemEntry>(this.OnStatusItemDestroy));
		statusItemEntry.SetSprite(status_item.item.sprite);
		if (category != null)
		{
			int num = -1;
			foreach (SimpleInfoScreen.StatusItemEntry statusItemEntry2 in this.oldStatusItems.FindAll((SimpleInfoScreen.StatusItemEntry e) => e.category == category))
			{
				num = statusItemEntry2.GetIndex();
				statusItemEntry2.Destroy(true);
				this.oldStatusItems.Remove(statusItemEntry2);
			}
			if (category == Db.Get().StatusItemCategories.Main)
			{
				num = 0;
			}
			if (num != -1)
			{
				statusItemEntry.SetIndex(num);
			}
		}
		this.statusItems.Add(statusItemEntry);
	}

	private void OnRemoveStatusItem(StatusItemGroup.Entry status_item, bool immediate = false)
	{
		this.DoRemoveStatusItem(status_item, immediate);
	}

	private void DoRemoveStatusItem(StatusItemGroup.Entry status_item, bool destroy_immediate = false)
	{
		for (int i = 0; i < this.statusItems.Count; i++)
		{
			if (this.statusItems[i].item.item == status_item.item)
			{
				SimpleInfoScreen.StatusItemEntry statusItemEntry = this.statusItems[i];
				this.statusItems.RemoveAt(i);
				this.oldStatusItems.Add(statusItemEntry);
				statusItemEntry.Destroy(destroy_immediate);
				return;
			}
		}
	}

	private void OnStatusItemDestroy(SimpleInfoScreen.StatusItemEntry item)
	{
		this.oldStatusItems.Remove(item);
	}

	private void Update()
	{
		this.Refresh(false);
	}

	private void OnRefreshData(object obj)
	{
		this.Refresh(false);
	}

	public void Refresh(bool force = false)
	{
		if (this.selectedTarget != this.lastTarget || force)
		{
			this.lastTarget = this.selectedTarget;
			if (this.selectedTarget != null)
			{
				this.SetPanels(this.selectedTarget);
				this.SetStamps(this.selectedTarget);
			}
		}
		int count = this.statusItems.Count;
		this.statusItemPanel.gameObject.SetActive(count > 0);
		for (int i = 0; i < count; i++)
		{
			this.statusItems[i].Refresh();
		}
		if (this.vitalsContainer.isActiveAndEnabled)
		{
			this.vitalsContainer.Refresh();
		}
		this.RefreshStress();
		this.RefreshStorage();
		this.rocketSimpleInfoPanel.Refresh(this.rocketStatusContainer, this.selectedTarget);
	}

	private void SetPanels(GameObject target)
	{
		MinionIdentity component = target.GetComponent<MinionIdentity>();
		Amounts amounts = target.GetAmounts();
		PrimaryElement component2 = target.GetComponent<PrimaryElement>();
		BuildingComplete component3 = target.GetComponent<BuildingComplete>();
		BuildingUnderConstruction component4 = target.GetComponent<BuildingUnderConstruction>();
		CellSelectionObject component5 = target.GetComponent<CellSelectionObject>();
		InfoDescription component6 = target.GetComponent<InfoDescription>();
		Edible component7 = target.GetComponent<Edible>();
		bool component8 = target.GetComponent<IProcessConditionSet>() != null;
		this.attributeLabels.ForEach(delegate(LocText x)
		{
			global::UnityEngine.Object.Destroy(x.gameObject);
		});
		this.attributeLabels.Clear();
		this.vitalsPanel.gameObject.SetActive(amounts != null);
		string text = "";
		string text2 = "";
		if (amounts != null)
		{
			this.vitalsContainer.selectedEntity = this.selectedTarget;
			Uprootable component9 = this.selectedTarget.gameObject.GetComponent<Uprootable>();
			if (component9 != null)
			{
				this.vitalsPanel.gameObject.SetActive(component9.GetPlanterStorage != null);
			}
			if (this.selectedTarget.gameObject.GetComponent<WiltCondition>() != null)
			{
				this.vitalsPanel.gameObject.SetActive(true);
			}
		}
		if (component8)
		{
			this.processConditionContainer.SetActive(true);
			this.RefreshProcessConditions();
		}
		else
		{
			this.processConditionContainer.SetActive(false);
		}
		if (component)
		{
			text = "";
		}
		else if (component6)
		{
			text = component6.description;
		}
		else if (component3 != null)
		{
			text = component3.Def.Effect;
			text2 = component3.Def.Desc;
		}
		else if (component4 != null)
		{
			text = component4.Def.Effect;
			text2 = component4.Def.Desc;
		}
		else if (component7 != null)
		{
			EdiblesManager.FoodInfo foodInfo = component7.FoodInfo;
			text += string.Format(UI.GAMEOBJECTEFFECTS.CALORIES, GameUtil.GetFormattedCalories(foodInfo.CaloriesPerUnit, GameUtil.TimeSlice.None, true));
		}
		else if (component5 != null)
		{
			text = component5.element.FullDescription(false);
		}
		else if (component2 != null)
		{
			Element element = ElementLoader.FindElementByHash(component2.ElementID);
			text = ((element != null) ? element.FullDescription(false) : "");
		}
		List<Descriptor> gameObjectEffects = GameUtil.GetGameObjectEffects(target, true);
		bool flag = gameObjectEffects.Count > 0;
		this.descriptionContainer.gameObject.SetActive(flag);
		this.descriptionContainer.descriptors.gameObject.SetActive(flag);
		if (flag)
		{
			this.descriptionContainer.descriptors.SetDescriptors(gameObjectEffects);
		}
		this.descriptionContainer.description.text = text;
		this.descriptionContainer.flavour.text = text2;
		bool flag2 = text.IsNullOrWhiteSpace() && text2.IsNullOrWhiteSpace() && !flag;
		this.infoPanel.gameObject.SetActive(component == null && !flag2);
		this.descriptionContainer.gameObject.SetActive(this.infoPanel.activeSelf);
		this.descriptionContainer.flavour.gameObject.SetActive(text2 != "" && text2 != "\n");
		if (this.vitalsPanel.gameObject.activeSelf && amounts.Count == 0)
		{
			this.vitalsPanel.gameObject.SetActive(false);
		}
	}

	private void RefreshBreedingChance()
	{
		if (this.selectedTarget == null)
		{
			this.fertilityPanel.gameObject.SetActive(false);
			return;
		}
		FertilityMonitor.Instance smi = this.selectedTarget.GetSMI<FertilityMonitor.Instance>();
		if (smi == null)
		{
			this.fertilityPanel.gameObject.SetActive(false);
			return;
		}
		int num = 0;
		foreach (FertilityMonitor.BreedingChance breedingChance in smi.breedingChances)
		{
			List<FertilityModifier> forTag = Db.Get().FertilityModifiers.GetForTag(breedingChance.egg);
			if (forTag.Count > 0)
			{
				string text = "";
				foreach (FertilityModifier fertilityModifier in forTag)
				{
					text += string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_MOD_FORMAT, fertilityModifier.GetTooltip());
				}
				this.fertilityPanel.SetLabel("breeding_" + num++.ToString(), string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT, breedingChance.egg.ProperName(), GameUtil.GetFormattedPercent(breedingChance.weight * 100f, GameUtil.TimeSlice.None)), string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT_TOOLTIP, breedingChance.egg.ProperName(), GameUtil.GetFormattedPercent(breedingChance.weight * 100f, GameUtil.TimeSlice.None), text));
			}
			else
			{
				this.fertilityPanel.SetLabel("breeding_" + num++.ToString(), string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT, breedingChance.egg.ProperName(), GameUtil.GetFormattedPercent(breedingChance.weight * 100f, GameUtil.TimeSlice.None)), string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT_TOOLTIP_NOMOD, breedingChance.egg.ProperName(), GameUtil.GetFormattedPercent(breedingChance.weight * 100f, GameUtil.TimeSlice.None)));
			}
		}
		this.fertilityPanel.Commit();
	}

	private void RefreshStorage()
	{
		if (this.selectedTarget == null)
		{
			this.StoragePanel.gameObject.SetActive(false);
			return;
		}
		IStorage[] array = this.selectedTarget.GetComponentsInChildren<IStorage>();
		if (array == null)
		{
			this.StoragePanel.gameObject.SetActive(false);
			return;
		}
		array = Array.FindAll<IStorage>(array, (IStorage n) => n.ShouldShowInUI());
		if (array.Length == 0)
		{
			this.StoragePanel.gameObject.SetActive(false);
			return;
		}
		this.StoragePanel.gameObject.SetActive(true);
		string text = ((this.selectedTarget.GetComponent<MinionIdentity>() != null) ? UI.DETAILTABS.DETAILS.GROUPNAME_MINION_CONTENTS : UI.DETAILTABS.DETAILS.GROUPNAME_CONTENTS);
		this.StoragePanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = text;
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.storageLabels)
		{
			keyValuePair.Value.SetActive(false);
		}
		int num = 0;
		foreach (IStorage storage in array)
		{
			ListPool<global::Tuple<string, TextStyleSetting>, SimpleInfoScreen>.PooledList pooledList = ListPool<global::Tuple<string, TextStyleSetting>, SimpleInfoScreen>.Allocate();
			foreach (GameObject gameObject in storage.GetItems())
			{
				if (!(gameObject == null))
				{
					PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
					if (!(component != null) || component.Mass != 0f)
					{
						Rottable.Instance smi = gameObject.GetSMI<Rottable.Instance>();
						HighEnergyParticleStorage component2 = gameObject.GetComponent<HighEnergyParticleStorage>();
						string text2 = "";
						pooledList.Clear();
						if (component != null && component2 == null)
						{
							text2 = GameUtil.GetUnitFormattedName(gameObject, false);
							text2 = string.Format(UI.DETAILTABS.DETAILS.CONTENTS_MASS, text2, GameUtil.GetFormattedMass(component.Mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
							text2 = string.Format(UI.DETAILTABS.DETAILS.CONTENTS_TEMPERATURE, text2, GameUtil.GetFormattedTemperature(component.Temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
						}
						if (component2 != null)
						{
							text2 = ITEMS.RADIATION.HIGHENERGYPARITCLE.NAME;
							text2 = string.Format(UI.DETAILTABS.DETAILS.CONTENTS_MASS, text2, GameUtil.GetFormattedHighEnergyParticles(component2.Particles, GameUtil.TimeSlice.None, true));
						}
						if (smi != null)
						{
							string text3 = smi.StateString();
							if (!string.IsNullOrEmpty(text3))
							{
								text2 += string.Format(UI.DETAILTABS.DETAILS.CONTENTS_ROTTABLE, text3);
							}
							pooledList.Add(new global::Tuple<string, TextStyleSetting>(smi.GetToolTip(), PluginAssets.Instance.defaultTextStyleSetting));
						}
						if (component.DiseaseIdx != 255)
						{
							text2 += string.Format(UI.DETAILTABS.DETAILS.CONTENTS_DISEASED, GameUtil.GetFormattedDisease(component.DiseaseIdx, component.DiseaseCount, false));
							string formattedDisease = GameUtil.GetFormattedDisease(component.DiseaseIdx, component.DiseaseCount, true);
							pooledList.Add(new global::Tuple<string, TextStyleSetting>(formattedDisease, PluginAssets.Instance.defaultTextStyleSetting));
						}
						GameObject gameObject2 = this.AddOrGetStorageLabel(this.storageLabels, this.StoragePanel, "storage_" + num.ToString());
						num++;
						gameObject2.GetComponentInChildren<LocText>().text = text2;
						gameObject2.GetComponentInChildren<ToolTip>().ClearMultiStringTooltip();
						foreach (global::Tuple<string, TextStyleSetting> tuple in pooledList)
						{
							gameObject2.GetComponentInChildren<ToolTip>().AddMultiStringTooltip(tuple.first, tuple.second);
						}
						KButton component3 = gameObject2.GetComponent<KButton>();
						GameObject select_target = gameObject;
						component3.onClick += delegate
						{
							SelectTool.Instance.Select(select_target.GetComponent<KSelectable>(), false);
						};
						if (storage.allowUIItemRemoval)
						{
							Transform transform = gameObject2.transform.Find("removeAttributeButton");
							if (transform != null)
							{
								KButton component4 = transform.GetComponent<KButton>();
								component4.enabled = true;
								component4.gameObject.SetActive(true);
								GameObject select_item = gameObject;
								IStorage selected_storage = storage;
								component4.onClick += delegate
								{
									selected_storage.Drop(select_item, true);
								};
							}
						}
					}
				}
			}
			pooledList.Recycle();
		}
		if (num == 0)
		{
			this.AddOrGetStorageLabel(this.storageLabels, this.StoragePanel, "empty").GetComponentInChildren<LocText>().text = UI.DETAILTABS.DETAILS.STORAGE_EMPTY;
		}
	}

	private void CreateWorldTraitRow()
	{
		GameObject gameObject = global::Util.KInstantiateUI(this.iconLabelRow, this.worldTraitsPanel.Content.gameObject, true);
		this.worldTraitRows.Add(gameObject);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		component.GetReference<Image>("Icon").gameObject.SetActive(false);
		component.GetReference<LocText>("ValueLabel").gameObject.SetActive(false);
	}

	private void RefreshWorld()
	{
		WorldContainer worldContainer = ((this.selectedTarget == null) ? null : this.selectedTarget.GetComponent<WorldContainer>());
		AsteroidGridEntity asteroidGridEntity = ((this.selectedTarget == null) ? null : this.selectedTarget.GetComponent<AsteroidGridEntity>());
		bool flag = worldContainer != null && asteroidGridEntity != null;
		this.worldBiomesPanel.gameObject.SetActive(flag);
		this.worldGeysersPanel.gameObject.SetActive(flag);
		this.worldTraitsPanel.gameObject.SetActive(flag);
		if (!flag)
		{
			return;
		}
		foreach (KeyValuePair<Tag, GameObject> keyValuePair in this.biomeRows)
		{
			keyValuePair.Value.SetActive(false);
		}
		if (worldContainer.Biomes != null)
		{
			using (List<string>.Enumerator enumerator2 = worldContainer.Biomes.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					string text = enumerator2.Current;
					Sprite biomeSprite = GameUtil.GetBiomeSprite(text);
					if (!this.biomeRows.ContainsKey(text))
					{
						this.biomeRows.Add(text, global::Util.KInstantiateUI(this.bigIconLabelRow, this.worldBiomesPanel.Content.gameObject, true));
						HierarchyReferences component = this.biomeRows[text].GetComponent<HierarchyReferences>();
						component.GetReference<Image>("Icon").sprite = biomeSprite;
						component.GetReference<LocText>("NameLabel").SetText(UI.FormatAsLink(Strings.Get("STRINGS.SUBWORLDS." + text.ToUpper() + ".NAME"), "BIOME" + text.ToUpper()));
						component.GetReference<LocText>("DescriptionLabel").SetText(Strings.Get("STRINGS.SUBWORLDS." + text.ToUpper() + ".DESC"));
					}
					this.biomeRows[text].SetActive(true);
				}
				goto IL_021A;
			}
		}
		this.worldBiomesPanel.gameObject.SetActive(false);
		IL_021A:
		List<Tag> list = new List<Tag>();
		foreach (Geyser geyser in global::UnityEngine.Object.FindObjectsOfType<Geyser>())
		{
			if (geyser.GetMyWorldId() == worldContainer.id)
			{
				list.Add(geyser.PrefabID());
			}
		}
		list.AddRange(SaveGame.Instance.worldGenSpawner.GetUnspawnedWithType<Geyser>(worldContainer.id));
		list.AddRange(SaveGame.Instance.worldGenSpawner.GetSpawnersWithTag("OilWell", worldContainer.id, true));
		foreach (KeyValuePair<Tag, GameObject> keyValuePair2 in this.geyserRows)
		{
			keyValuePair2.Value.SetActive(false);
		}
		foreach (Tag tag in list)
		{
			global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(tag, "ui", false);
			if (!this.geyserRows.ContainsKey(tag))
			{
				this.geyserRows.Add(tag, global::Util.KInstantiateUI(this.iconLabelRow, this.worldGeysersPanel.Content.gameObject, true));
				HierarchyReferences component2 = this.geyserRows[tag].GetComponent<HierarchyReferences>();
				component2.GetReference<Image>("Icon").sprite = uisprite.first;
				component2.GetReference<Image>("Icon").color = uisprite.second;
				component2.GetReference<LocText>("NameLabel").SetText(Assets.GetPrefab(tag).GetProperName());
				component2.GetReference<LocText>("ValueLabel").gameObject.SetActive(false);
			}
			this.geyserRows[tag].SetActive(true);
		}
		int count = SaveGame.Instance.worldGenSpawner.GetSpawnersWithTag("GeyserGeneric", worldContainer.id, false).Count;
		if (count > 0)
		{
			global::Tuple<Sprite, Color> uisprite2 = Def.GetUISprite("GeyserGeneric", "ui", false);
			Tag tag2 = "GeyserGeneric";
			if (!this.geyserRows.ContainsKey(tag2))
			{
				this.geyserRows.Add(tag2, global::Util.KInstantiateUI(this.iconLabelRow, this.worldGeysersPanel.Content.gameObject, true));
				HierarchyReferences component3 = this.geyserRows[tag2].GetComponent<HierarchyReferences>();
				component3.GetReference<Image>("Icon").sprite = uisprite2.first;
				component3.GetReference<Image>("Icon").color = uisprite2.second;
				component3.GetReference<LocText>("NameLabel").SetText(UI.DETAILTABS.SIMPLEINFO.UNKNOWN_GEYSERS.Replace("{num}", count.ToString()));
				component3.GetReference<LocText>("ValueLabel").gameObject.SetActive(false);
			}
			this.geyserRows[tag2].SetActive(true);
		}
		Tag tag3 = "NoGeysers";
		if (!this.geyserRows.ContainsKey(tag3))
		{
			this.geyserRows.Add(tag3, global::Util.KInstantiateUI(this.iconLabelRow, this.worldGeysersPanel.Content.gameObject, true));
			HierarchyReferences component4 = this.geyserRows[tag3].GetComponent<HierarchyReferences>();
			component4.GetReference<Image>("Icon").sprite = Assets.GetSprite("icon_action_cancel");
			component4.GetReference<LocText>("NameLabel").SetText(UI.DETAILTABS.SIMPLEINFO.NO_GEYSERS);
			component4.GetReference<LocText>("ValueLabel").gameObject.SetActive(false);
		}
		this.geyserRows[tag3].gameObject.SetActive(list.Count == 0);
		List<string> worldTraitIds = worldContainer.WorldTraitIds;
		if (worldTraitIds != null)
		{
			for (int j = 0; j < worldTraitIds.Count; j++)
			{
				if (j > this.worldTraitRows.Count - 1)
				{
					this.CreateWorldTraitRow();
				}
				WorldTrait cachedTrait = SettingsCache.GetCachedTrait(worldTraitIds[j], false);
				if (cachedTrait != null)
				{
					this.worldTraitRows[j].GetComponent<HierarchyReferences>().GetReference<LocText>("NameLabel").SetText(Strings.Get(cachedTrait.name));
					this.worldTraitRows[j].AddOrGet<ToolTip>().SetSimpleTooltip(Strings.Get(cachedTrait.description));
				}
				else
				{
					this.worldTraitRows[j].GetComponent<HierarchyReferences>().GetReference<LocText>("NameLabel").SetText(WORLD_TRAITS.MISSING_TRAIT);
					this.worldTraitRows[j].AddOrGet<ToolTip>().SetSimpleTooltip("");
				}
			}
			for (int k = 0; k < this.worldTraitRows.Count; k++)
			{
				this.worldTraitRows[k].SetActive(k < worldTraitIds.Count);
			}
			if (worldTraitIds.Count == 0)
			{
				if (this.worldTraitRows.Count < 1)
				{
					this.CreateWorldTraitRow();
				}
				this.worldTraitRows[0].GetComponent<HierarchyReferences>().GetReference<LocText>("NameLabel").SetText(WORLD_TRAITS.NO_TRAITS.NAME_SHORTHAND);
				this.worldTraitRows[0].AddOrGet<ToolTip>().SetSimpleTooltip(WORLD_TRAITS.NO_TRAITS.DESCRIPTION);
				this.worldTraitRows[0].SetActive(true);
			}
		}
	}

	private void RefreshProcessConditions()
	{
		foreach (GameObject gameObject in this.processConditionRows)
		{
			global::Util.KDestroyGameObject(gameObject);
		}
		this.processConditionRows.Clear();
		if (!DlcManager.FeatureClusterSpaceEnabled())
		{
			if (this.selectedTarget.GetComponent<LaunchableRocket>() != null)
			{
				this.RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.RocketPrep);
				this.RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.RocketStorage);
				this.RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.RocketBoard);
				return;
			}
			this.RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.All);
			return;
		}
		else
		{
			if (this.selectedTarget.GetComponent<LaunchPad>() != null || this.selectedTarget.GetComponent<RocketProcessConditionDisplayTarget>() != null)
			{
				this.RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.RocketFlight);
				this.RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.RocketPrep);
				this.RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.RocketStorage);
				this.RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.RocketBoard);
				return;
			}
			this.RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.All);
			return;
		}
	}

	private void RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType conditionType)
	{
		List<ProcessCondition> conditionSet = this.selectedTarget.GetComponent<IProcessConditionSet>().GetConditionSet(conditionType);
		if (conditionSet.Count == 0)
		{
			return;
		}
		HierarchyReferences hierarchyReferences = global::Util.KInstantiateUI<HierarchyReferences>(this.processConditionHeader.gameObject, this.processConditionContainer.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject, true);
		hierarchyReferences.GetReference<LocText>("Label").text = Strings.Get("STRINGS.UI.DETAILTABS.PROCESS_CONDITIONS." + conditionType.ToString().ToUpper());
		hierarchyReferences.GetComponent<ToolTip>().toolTip = Strings.Get("STRINGS.UI.DETAILTABS.PROCESS_CONDITIONS." + conditionType.ToString().ToUpper() + "_TOOLTIP");
		this.processConditionRows.Add(hierarchyReferences.gameObject);
		List<ProcessCondition> list = new List<ProcessCondition>();
		using (List<ProcessCondition>.Enumerator enumerator = conditionSet.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ProcessCondition condition = enumerator.Current;
				if (condition.ShowInUI() && (condition.GetType() == typeof(RequireAttachedComponent) || list.Find((ProcessCondition match) => match.GetType() == condition.GetType()) == null))
				{
					list.Add(condition);
					GameObject gameObject = global::Util.KInstantiateUI(this.processConditionRow, this.processConditionContainer.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject, true);
					this.processConditionRows.Add(gameObject);
					ConditionListSideScreen.SetRowState(gameObject, condition);
				}
			}
		}
	}

	public GameObject AddOrGetStorageLabel(Dictionary<string, GameObject> labels, GameObject panel, string id)
	{
		GameObject gameObject;
		if (labels.ContainsKey(id))
		{
			gameObject = labels[id];
			gameObject.GetComponent<KButton>().ClearOnClick();
			Transform transform = gameObject.transform.Find("removeAttributeButton");
			if (transform != null)
			{
				KButton kbutton = transform.FindComponent<KButton>();
				kbutton.enabled = false;
				kbutton.gameObject.SetActive(false);
				kbutton.ClearOnClick();
			}
		}
		else
		{
			gameObject = global::Util.KInstantiate(this.attributesLabelButtonTemplate, panel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject, null);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			labels[id] = gameObject;
		}
		gameObject.SetActive(true);
		return gameObject;
	}

	private void RefreshStress()
	{
		MinionIdentity identity = ((this.selectedTarget != null) ? this.selectedTarget.GetComponent<MinionIdentity>() : null);
		if (identity == null)
		{
			this.stressPanel.SetActive(false);
			return;
		}
		List<ReportManager.ReportEntry.Note> stressNotes = new List<ReportManager.ReportEntry.Note>();
		this.stressPanel.SetActive(true);
		this.stressPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.STATS.GROUPNAME_STRESS;
		ReportManager.ReportEntry reportEntry = ReportManager.Instance.TodaysReport.reportEntries.Find((ReportManager.ReportEntry entry) => entry.reportType == ReportManager.ReportType.StressDelta);
		this.stressDrawer.BeginDrawing();
		float num = 0f;
		stressNotes.Clear();
		int num2 = reportEntry.contextEntries.FindIndex((ReportManager.ReportEntry entry) => entry.context == identity.GetProperName());
		ReportManager.ReportEntry reportEntry2 = ((num2 != -1) ? reportEntry.contextEntries[num2] : null);
		if (reportEntry2 != null)
		{
			reportEntry2.IterateNotes(delegate(ReportManager.ReportEntry.Note note)
			{
				stressNotes.Add(note);
			});
			stressNotes.Sort((ReportManager.ReportEntry.Note a, ReportManager.ReportEntry.Note b) => a.value.CompareTo(b.value));
			for (int i = 0; i < stressNotes.Count; i++)
			{
				this.stressDrawer.NewLabel(string.Concat(new string[]
				{
					(stressNotes[i].value > 0f) ? UIConstants.ColorPrefixRed : "",
					stressNotes[i].note,
					": ",
					global::Util.FormatTwoDecimalPlace(stressNotes[i].value),
					"%",
					(stressNotes[i].value > 0f) ? UIConstants.ColorSuffix : ""
				}));
				num += stressNotes[i].value;
			}
		}
		this.stressDrawer.NewLabel(((num > 0f) ? UIConstants.ColorPrefixRed : "") + string.Format(UI.DETAILTABS.DETAILS.NET_STRESS, global::Util.FormatTwoDecimalPlace(num)) + ((num > 0f) ? UIConstants.ColorSuffix : ""));
		this.stressDrawer.EndDrawing();
	}

	private void ShowAttributes(GameObject target)
	{
		Attributes attributes = target.GetAttributes();
		if (attributes == null)
		{
			return;
		}
		List<AttributeInstance> list = attributes.AttributeTable.FindAll((AttributeInstance a) => a.Attribute.ShowInUI == Klei.AI.Attribute.Display.General);
		if (list.Count > 0)
		{
			this.descriptionContainer.descriptors.gameObject.SetActive(true);
			List<Descriptor> list2 = new List<Descriptor>();
			foreach (AttributeInstance attributeInstance in list)
			{
				Descriptor descriptor = new Descriptor(string.Format("{0}: {1}", attributeInstance.Name, attributeInstance.GetFormattedValue()), attributeInstance.GetAttributeValueTooltip(), Descriptor.DescriptorType.Effect, false);
				descriptor.IncreaseIndent();
				list2.Add(descriptor);
			}
			this.descriptionContainer.descriptors.SetDescriptors(list2);
		}
	}

	private void SetStamps(GameObject target)
	{
		for (int i = 0; i < this.stampContainer.transform.childCount; i++)
		{
			global::UnityEngine.Object.Destroy(this.stampContainer.transform.GetChild(i).gameObject);
		}
		target.GetComponent<BuildingComplete>() != null;
	}

	public void Sim1000ms(float dt)
	{
		if (this.selectedTarget != null && this.selectedTarget.GetComponent<IProcessConditionSet>() != null)
		{
			this.RefreshProcessConditions();
		}
	}

	public void Sim4000ms(float dt)
	{
		this.RefreshWorld();
		this.spaceSimpleInfoPOIPanel.Refresh(this.spacePOIPanel, this.selectedTarget);
	}

	public GameObject attributesLabelTemplate;

	public GameObject attributesLabelButtonTemplate;

	public GameObject DescriptionContainerTemplate;

	private DescriptionContainer descriptionContainer;

	public GameObject StampContainerTemplate;

	public GameObject StampPrefab;

	public GameObject VitalsPanelTemplate;

	public Sprite DefaultPortraitIcon;

	public Text StatusPanelCurrentActionLabel;

	public GameObject StatusItemPrefab;

	public Sprite statusWarningIcon;

	private RocketSimpleInfoPanel rocketSimpleInfoPanel;

	private SpacePOISimpleInfoPanel spaceSimpleInfoPOIPanel;

	[SerializeField]
	private HierarchyReferences processConditionHeader;

	[SerializeField]
	private GameObject processConditionRow;

	private CollapsibleDetailContentPanel statusItemPanel;

	private CollapsibleDetailContentPanel vitalsPanel;

	private CollapsibleDetailContentPanel fertilityPanel;

	private CollapsibleDetailContentPanel rocketStatusContainer;

	private CollapsibleDetailContentPanel worldLifePanel;

	private CollapsibleDetailContentPanel worldElementsPanel;

	private CollapsibleDetailContentPanel worldBiomesPanel;

	private CollapsibleDetailContentPanel worldGeysersPanel;

	private CollapsibleDetailContentPanel spacePOIPanel;

	private CollapsibleDetailContentPanel worldTraitsPanel;

	[SerializeField]
	public GameObject iconLabelRow;

	[SerializeField]
	public GameObject bigIconLabelRow;

	private Dictionary<Tag, GameObject> lifeformRows = new Dictionary<Tag, GameObject>();

	private Dictionary<Tag, GameObject> biomeRows = new Dictionary<Tag, GameObject>();

	private Dictionary<Tag, GameObject> geyserRows = new Dictionary<Tag, GameObject>();

	private List<GameObject> worldTraitRows = new List<GameObject>();

	[SerializeField]
	public GameObject spacerRow;

	private GameObject infoPanel;

	private GameObject stampContainer;

	private MinionVitalsPanel vitalsContainer;

	private GameObject InfoFolder;

	private GameObject statusItemsFolder;

	public GameObject TextContainerPrefab;

	private GameObject processConditionContainer;

	private GameObject stressPanel;

	private DetailsPanelDrawer stressDrawer;

	private Dictionary<string, GameObject> storageLabels = new Dictionary<string, GameObject>();

	public TextStyleSetting ToolTipStyle_Property;

	public TextStyleSetting StatusItemStyle_Main;

	public TextStyleSetting StatusItemStyle_Other;

	public Color statusItemTextColor_regular = Color.black;

	public Color statusItemTextColor_old = new Color(0.8235294f, 0.8235294f, 0.8235294f);

	private GameObject lastTarget;

	private bool TargetIsMinion;

	private List<SimpleInfoScreen.StatusItemEntry> statusItems = new List<SimpleInfoScreen.StatusItemEntry>();

	private List<SimpleInfoScreen.StatusItemEntry> oldStatusItems = new List<SimpleInfoScreen.StatusItemEntry>();

	private List<LocText> attributeLabels = new List<LocText>();

	private List<GameObject> processConditionRows = new List<GameObject>();

	private Action<object> onStorageChangeDelegate;

	private static readonly EventSystem.IntraObjectHandler<SimpleInfoScreen> OnRefreshDataDelegate = new EventSystem.IntraObjectHandler<SimpleInfoScreen>(delegate(SimpleInfoScreen component, object data)
	{
		component.OnRefreshData(data);
	});

	[DebuggerDisplay("{item.item.Name}")]
	public class StatusItemEntry : IRenderEveryTick
	{
		public Image GetImage
		{
			get
			{
				return this.image;
			}
		}

		public StatusItemEntry(StatusItemGroup.Entry item, StatusItemCategory category, GameObject status_item_prefab, Transform parent, TextStyleSetting tooltip_style, Color color, TextStyleSetting style, bool skip_fade, Action<SimpleInfoScreen.StatusItemEntry> onDestroy)
		{
			this.item = item;
			this.category = category;
			this.tooltipStyle = tooltip_style;
			this.onDestroy = onDestroy;
			this.color = color;
			this.style = style;
			this.widget = global::Util.KInstantiateUI(status_item_prefab, parent.gameObject, false);
			this.text = this.widget.GetComponentInChildren<LocText>(true);
			SetTextStyleSetting.ApplyStyle(this.text, style);
			this.toolTip = this.widget.GetComponentInChildren<ToolTip>(true);
			this.image = this.widget.GetComponentInChildren<Image>(true);
			item.SetIcon(this.image);
			this.widget.SetActive(true);
			this.toolTip.OnToolTip = new Func<string>(this.OnToolTip);
			this.button = this.widget.GetComponentInChildren<KButton>();
			if (item.item.statusItemClickCallback != null)
			{
				this.button.onClick += this.OnClick;
			}
			else
			{
				this.button.enabled = false;
			}
			this.fadeStage = (skip_fade ? SimpleInfoScreen.StatusItemEntry.FadeStage.WAIT : SimpleInfoScreen.StatusItemEntry.FadeStage.IN);
			SimAndRenderScheduler.instance.Add(this, false);
			this.Refresh();
			this.SetColor(1f);
		}

		internal void SetSprite(TintedSprite sprite)
		{
			if (sprite != null)
			{
				this.image.sprite = sprite.sprite;
			}
		}

		public int GetIndex()
		{
			return this.widget.transform.GetSiblingIndex();
		}

		public void SetIndex(int index)
		{
			this.widget.transform.SetSiblingIndex(index);
		}

		public void RenderEveryTick(float dt)
		{
			switch (this.fadeStage)
			{
			case SimpleInfoScreen.StatusItemEntry.FadeStage.IN:
			{
				this.fade = Mathf.Min(this.fade + Time.deltaTime / this.fadeInTime, 1f);
				float num = this.fade;
				this.SetColor(num);
				if (this.fade >= 1f)
				{
					this.fadeStage = SimpleInfoScreen.StatusItemEntry.FadeStage.WAIT;
					return;
				}
				break;
			}
			case SimpleInfoScreen.StatusItemEntry.FadeStage.WAIT:
				break;
			case SimpleInfoScreen.StatusItemEntry.FadeStage.OUT:
			{
				float num2 = this.fade;
				this.SetColor(num2);
				this.fade = Mathf.Max(this.fade - Time.deltaTime / this.fadeOutTime, 0f);
				if (this.fade <= 0f)
				{
					this.Destroy(true);
				}
				break;
			}
			default:
				return;
			}
		}

		private string OnToolTip()
		{
			this.item.ShowToolTip(this.toolTip, this.tooltipStyle);
			return "";
		}

		private void OnClick()
		{
			this.item.OnClick();
		}

		public void Refresh()
		{
			string name = this.item.GetName();
			if (name != this.text.text)
			{
				this.text.text = name;
				this.SetColor(1f);
			}
		}

		private void SetColor(float alpha = 1f)
		{
			Color color = new Color(this.color.r, this.color.g, this.color.b, alpha);
			this.image.color = color;
			this.text.color = color;
		}

		public void Destroy(bool immediate)
		{
			if (immediate)
			{
				if (this.onDestroy != null)
				{
					this.onDestroy(this);
				}
				SimAndRenderScheduler.instance.Remove(this);
				this.toolTip.OnToolTip = null;
				global::UnityEngine.Object.Destroy(this.widget);
				return;
			}
			this.fade = 0.5f;
			this.fadeStage = SimpleInfoScreen.StatusItemEntry.FadeStage.OUT;
		}

		public StatusItemGroup.Entry item;

		public StatusItemCategory category;

		private LayoutElement spacerLayout;

		private GameObject widget;

		private ToolTip toolTip;

		private TextStyleSetting tooltipStyle;

		public Action<SimpleInfoScreen.StatusItemEntry> onDestroy;

		private Image image;

		private LocText text;

		private KButton button;

		public Color color;

		public TextStyleSetting style;

		private SimpleInfoScreen.StatusItemEntry.FadeStage fadeStage;

		private float fade;

		private float fadeInTime;

		private float fadeOutTime = 1.8f;

		private enum FadeStage
		{
			IN,
			WAIT,
			OUT
		}
	}
}
