using System;
using System.Collections.Generic;
using System.Diagnostics;
using Klei.AI;
using ProcGen;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleInfoScreen : DetailScreenTab, ISim4000ms, ISim1000ms
{
	public CollapsibleDetailContentPanel StoragePanel { get; private set; }

	public override bool IsValidForTarget(GameObject target)
	{
		return true;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.processConditionContainer = base.CreateCollapsableSection(UI.DETAILTABS.PROCESS_CONDITIONS.NAME);
		this.statusItemPanel = base.CreateCollapsableSection(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_STATUS);
		this.statusItemPanel.Content.GetComponent<VerticalLayoutGroup>().padding.bottom = 10;
		this.statusItemPanel.scalerMask.hoverLock = true;
		this.statusItemsFolder = this.statusItemPanel.Content.gameObject;
		this.spaceSimpleInfoPOIPanel = new SpacePOISimpleInfoPanel(this);
		this.spacePOIPanel = base.CreateCollapsableSection(null);
		this.starmapHexCellStorageInfoPanel = new StarmapHexCellInventoryInfoPanel(this);
		this.spaceHexCellStoragePanel = base.CreateCollapsableSection(null);
		this.rocketSimpleInfoPanel = new RocketSimpleInfoPanel(this);
		this.rocketStatusContainer = base.CreateCollapsableSection(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_ROCKET);
		this.vitalsPanel = global::Util.KInstantiateUI(this.VitalsPanelTemplate, base.gameObject, false).GetComponent<MinionVitalsPanel>();
		this.fertilityPanel = base.CreateCollapsableSection(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_FERTILITY);
		this.mooFertilityPanel = base.CreateCollapsableSection(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_MOO_FERTILITY);
		this.infoPanel = base.CreateCollapsableSection(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_DESCRIPTION);
		this.requirementsPanel = base.CreateCollapsableSection(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_REQUIREMENTS);
		this.requirementContent = global::Util.KInstantiateUI<DescriptorPanel>(this.DescriptorContentPrefab.gameObject, this.requirementsPanel.Content.gameObject, false);
		this.effectsPanel = base.CreateCollapsableSection(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_EFFECTS);
		this.effectsContent = global::Util.KInstantiateUI<DescriptorPanel>(this.DescriptorContentPrefab.gameObject, this.effectsPanel.Content.gameObject, false);
		this.worldMeteorShowersPanel = base.CreateCollapsableSection(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_METEORSHOWERS);
		this.worldElementsPanel = base.CreateCollapsableSection(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_ELEMENTS);
		this.worldGeysersPanel = base.CreateCollapsableSection(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_GEYSERS);
		this.worldTraitsPanel = base.CreateCollapsableSection(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_WORLDTRAITS);
		this.worldBiomesPanel = base.CreateCollapsableSection(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_BIOMES);
		this.worldLifePanel = base.CreateCollapsableSection(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_LIFE);
		this.StoragePanel = base.CreateCollapsableSection(null);
		this.stressPanel = base.CreateCollapsableSection(null);
		this.stressDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, this.stressPanel.Content.gameObject);
		this.movePanel = base.CreateCollapsableSection(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_MOVABLE);
		base.Subscribe<SimpleInfoScreen>(-1514841199, SimpleInfoScreen.OnRefreshDataDelegate);
	}

	protected override void OnSelectTarget(GameObject target)
	{
		base.OnSelectTarget(target);
		base.Subscribe(target, -1697596308, new Action<object>(this.TriggerRefreshStorage));
		base.Subscribe(target, -1197125120, new Action<object>(this.TriggerRefreshStorage));
		base.Subscribe(target, 1059811075, new Action<object>(this.OnBreedingChanceChanged));
		base.Subscribe(target, 1105317911, new Action<object>(this.OnMooSongChanceChanged));
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
		this.RefreshWorldPanel();
		this.RefreshProcessConditionsPanel();
		this.spaceSimpleInfoPOIPanel.Refresh(this.spacePOIPanel, this.selectedTarget);
		this.starmapHexCellStorageInfoPanel.Refresh(this.spaceHexCellStoragePanel, this.selectedTarget);
	}

	public override void OnDeselectTarget(GameObject target)
	{
		base.OnDeselectTarget(target);
		if (target != null)
		{
			base.Unsubscribe(target, -1697596308, new Action<object>(this.TriggerRefreshStorage));
			base.Unsubscribe(target, -1197125120, new Action<object>(this.TriggerRefreshStorage));
			base.Unsubscribe(target, 1059811075, new Action<object>(this.OnBreedingChanceChanged));
			base.Unsubscribe(target, 1105317911, new Action<object>(this.OnMooSongChanceChanged));
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
		SimpleInfoScreen.RefreshStoragePanel(this.StoragePanel, this.selectedTarget);
	}

	private void OnBreedingChanceChanged(object data)
	{
		SimpleInfoScreen.RefreshFertilityPanel(this.fertilityPanel, this.selectedTarget);
	}

	private void OnMooSongChanceChanged(object data)
	{
		SimpleInfoScreen.RefreshMooSongPanel(this.fertilityPanel, this.selectedTarget);
	}

	private void OnAddStatusItem(StatusItemGroup.Entry status_item, StatusItemCategory category)
	{
		this.DoAddStatusItem(status_item, category, false);
	}

	private void DoAddStatusItem(StatusItemGroup.Entry status_item, StatusItemCategory category, bool show_immediate = false)
	{
		if (status_item.item.showInHoverCardOnly)
		{
			return;
		}
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
		else if (status_item.item.notificationType == NotificationType.MessageImportant)
		{
			color = GlobalAssets.Instance.colorSet.statusItemMessageImportant;
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

	private void OnRefreshData(object obj)
	{
		this.Refresh(false);
	}

	protected override void Refresh(bool force = false)
	{
		if (this.selectedTarget != this.lastTarget || force)
		{
			this.lastTarget = this.selectedTarget;
		}
		int count = this.statusItems.Count;
		this.statusItemPanel.gameObject.SetActive(count > 0);
		for (int i = 0; i < count; i++)
		{
			this.statusItems[i].Refresh();
		}
		SimpleInfoScreen.RefreshStressPanel(this.stressPanel, this.selectedTarget);
		SimpleInfoScreen.RefreshStoragePanel(this.StoragePanel, this.selectedTarget);
		SimpleInfoScreen.RefreshMovePanel(this.movePanel, this.selectedTarget);
		SimpleInfoScreen.RefreshFertilityPanel(this.fertilityPanel, this.selectedTarget);
		SimpleInfoScreen.RefreshMooSongPanel(this.mooFertilityPanel, this.selectedTarget);
		SimpleInfoScreen.RefreshRequirementsAndEffectsPanels(this.requirementsPanel, this.effectsPanel, this.selectedTarget, this.requirementContent, this.effectsContent);
		SimpleInfoScreen.RefreshInfoPanel(this.infoPanel, this.selectedTarget);
		this.vitalsPanel.Refresh(this.selectedTarget);
		this.rocketSimpleInfoPanel.Refresh(this.rocketStatusContainer, this.selectedTarget);
	}

	public void Sim1000ms(float dt)
	{
		if (this.selectedTarget != null && this.selectedTarget.GetComponent<IProcessConditionSet>() != null)
		{
			this.RefreshProcessConditionsPanel();
		}
	}

	public void Sim4000ms(float dt)
	{
		this.RefreshWorldPanel();
		this.spaceSimpleInfoPOIPanel.Refresh(this.spacePOIPanel, this.selectedTarget);
		this.starmapHexCellStorageInfoPanel.Refresh(this.spaceHexCellStoragePanel, this.selectedTarget);
	}

	private static void RefreshInfoPanel(CollapsibleDetailContentPanel targetPanel, GameObject targetEntity)
	{
		string text = "";
		string text2 = "";
		global::UnityEngine.Object component = targetEntity.GetComponent<MinionIdentity>();
		InfoDescription component2 = targetEntity.GetComponent<InfoDescription>();
		BuildingComplete component3 = targetEntity.GetComponent<BuildingComplete>();
		BuildingUnderConstruction component4 = targetEntity.GetComponent<BuildingUnderConstruction>();
		Edible component5 = targetEntity.GetComponent<Edible>();
		PrimaryElement component6 = targetEntity.GetComponent<PrimaryElement>();
		ICellSelectionProxy component7 = targetEntity.GetComponent<ICellSelectionProxy>();
		if (!component)
		{
			if (component2)
			{
				text = component2.description;
				text2 = component2.effect;
			}
			else if (component3 != null)
			{
				text = component3.DescFlavour + "\n\n" + component3.Desc;
			}
			else if (component4 != null)
			{
				text = component4.DescFlavour + "\n\n" + component4.Desc;
			}
			else if (component5 != null)
			{
				EdiblesManager.FoodInfo foodInfo = component5.FoodInfo;
				text += string.Format(UI.GAMEOBJECTEFFECTS.CALORIES, GameUtil.GetFormattedCalories(foodInfo.CaloriesPerUnit, GameUtil.TimeSlice.None, true));
			}
			else if (component7 != null && component7.Element != null)
			{
				text = component7.Element.FullDescription(false);
			}
			else if (component6 != null)
			{
				Element element = ElementLoader.FindElementByHash(component6.ElementID);
				text = ((element != null) ? element.FullDescription(false) : "");
			}
			if (!string.IsNullOrEmpty(text))
			{
				targetPanel.SetLabel("Description", text, "");
			}
			bool flag = !string.IsNullOrEmpty(text2) && text2 != "\n";
			string text3 = "\n" + text2;
			if (flag)
			{
				targetPanel.SetLabel("Flavour", text3, "");
			}
			string[] roomClassForObject = CodexEntryGenerator.GetRoomClassForObject(targetEntity);
			if (roomClassForObject != null)
			{
				string text4 = "\n" + CODEX.HEADERS.BUILDINGTYPE + ":";
				foreach (string text5 in roomClassForObject)
				{
					text4 = text4 + "\n    • " + text5;
				}
				targetPanel.SetLabel("RoomClass", text4, "");
			}
		}
		targetPanel.Commit();
	}

	private static void RefreshRequirementsAndEffectsPanels(CollapsibleDetailContentPanel requirementsTargetPanel, CollapsibleDetailContentPanel effectsTargetPanel, GameObject targetEntity, DescriptorPanel requirementContent, DescriptorPanel effectsContent)
	{
		if (targetEntity.GetComponent<MinionIdentity>() != null)
		{
			requirementsTargetPanel.SetActive(false);
			effectsTargetPanel.SetActive(false);
			return;
		}
		global::UnityEngine.Object component = targetEntity.GetComponent<WiltCondition>();
		CreatureBrain component2 = targetEntity.GetComponent<CreatureBrain>();
		bool flag = component != null || component2 != null;
		BuildingUnderConstruction component3 = targetEntity.GetComponent<BuildingUnderConstruction>();
		GameObject gameObject = (component3 ? component3.Def.BuildingComplete : targetEntity);
		ListPool<ValueTuple<ElementConverter, List<Descriptor>>, SimpleInfoScreen>.PooledList pooledList = ListPool<ValueTuple<ElementConverter, List<Descriptor>>, SimpleInfoScreen>.Allocate();
		ListPool<Descriptor, SimpleInfoScreen>.PooledList pooledList2 = ListPool<Descriptor, SimpleInfoScreen>.Allocate();
		ListPool<Descriptor, SimpleInfoScreen>.PooledList pooledList3 = ListPool<Descriptor, SimpleInfoScreen>.Allocate();
		List<Descriptor> list;
		bool flag2;
		GameUtil.PartitionBuildingDescriptors(gameObject, true, out list, pooledList, pooledList2, pooledList3, out flag2);
		if (flag)
		{
			requirementsTargetPanel.SetActive(false);
		}
		else
		{
			ListPool<Descriptor, SimpleInfoScreen>.PooledList pooledList4 = ListPool<Descriptor, SimpleInfoScreen>.Allocate();
			GameUtil.BuildPartitionedRequirements(pooledList4, pooledList2, pooledList, flag2);
			bool flag3 = pooledList4.Count > 0;
			requirementContent.gameObject.SetActive(flag3);
			if (flag3)
			{
				requirementContent.SetDescriptors(pooledList4);
			}
			requirementsTargetPanel.SetActive(flag3);
			pooledList4.Recycle();
		}
		ListPool<Descriptor, SimpleInfoScreen>.PooledList pooledList5 = ListPool<Descriptor, SimpleInfoScreen>.Allocate();
		GameUtil.BuildPartitionedEffects(pooledList5, pooledList3, pooledList);
		bool flag4 = pooledList5.Count > 0;
		effectsContent.gameObject.SetActive(flag4);
		if (flag4)
		{
			effectsContent.SetDescriptors(pooledList5);
		}
		effectsTargetPanel.SetActive(targetEntity != null && flag4);
		pooledList5.Recycle();
		pooledList2.Recycle();
		pooledList3.Recycle();
		pooledList.Recycle();
	}

	private static void RefreshFertilityPanel(CollapsibleDetailContentPanel targetPanel, GameObject targetEntity)
	{
		FertilityMonitor.Instance smi = targetEntity.GetSMI<FertilityMonitor.Instance>();
		if (smi != null)
		{
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
					targetPanel.SetLabel("breeding_" + num++.ToString(), string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT, breedingChance.egg.ProperName(), GameUtil.GetFormattedPercent(breedingChance.weight * 100f, GameUtil.TimeSlice.None)), string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT_TOOLTIP, breedingChance.egg.ProperName(), GameUtil.GetFormattedPercent(breedingChance.weight * 100f, GameUtil.TimeSlice.None), text));
				}
				else
				{
					targetPanel.SetLabel("breeding_" + num++.ToString(), string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT, breedingChance.egg.ProperName(), GameUtil.GetFormattedPercent(breedingChance.weight * 100f, GameUtil.TimeSlice.None)), string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT_TOOLTIP_NOMOD, breedingChance.egg.ProperName(), GameUtil.GetFormattedPercent(breedingChance.weight * 100f, GameUtil.TimeSlice.None)));
				}
			}
		}
		targetPanel.Commit();
	}

	private static void RefreshMooSongPanel(CollapsibleDetailContentPanel targetPanel, GameObject targetEntity)
	{
		BeckoningMonitor.Instance smi = targetEntity.GetSMI<BeckoningMonitor.Instance>();
		if (smi != null)
		{
			int num = 0;
			foreach (BeckoningMonitor.SongChance songChance in smi.songChances)
			{
				List<MooSongModifier> forTag = Db.Get().MooSongModifiers.GetForTag(songChance.meteorID);
				if (forTag.Count > 0)
				{
					string text = "";
					foreach (MooSongModifier mooSongModifier in forTag)
					{
						text += string.Format(UI.DETAILTABS.MOO_SONG_CHANCES.CHANCE_MOD_FORMAT, mooSongModifier.GetTooltip());
					}
					targetPanel.SetLabel("breeding_" + num++.ToString(), string.Format(UI.DETAILTABS.MOO_SONG_CHANCES.CHANCE_FORMAT, songChance.meteorID.ProperName(), GameUtil.GetFormattedPercent(songChance.weight * 100f, GameUtil.TimeSlice.None)), string.Format(UI.DETAILTABS.MOO_SONG_CHANCES.CHANCE_FORMAT_TOOLTIP, songChance.meteorID.ProperName(), GameUtil.GetFormattedPercent(songChance.weight * 100f, GameUtil.TimeSlice.None), text));
				}
				else
				{
					targetPanel.SetLabel("breeding_" + num++.ToString(), string.Format(UI.DETAILTABS.MOO_SONG_CHANCES.CHANCE_FORMAT, songChance.meteorID.ProperName(), GameUtil.GetFormattedPercent(songChance.weight * 100f, GameUtil.TimeSlice.None)), string.Format(UI.DETAILTABS.MOO_SONG_CHANCES.CHANCE_FORMAT_TOOLTIP_NOMOD, songChance.meteorID.ProperName(), GameUtil.GetFormattedPercent(songChance.weight * 100f, GameUtil.TimeSlice.None)));
				}
			}
		}
		targetPanel.Commit();
	}

	private void TriggerRefreshStorage(object data = null)
	{
		SimpleInfoScreen.RefreshStoragePanel(this.StoragePanel, this.selectedTarget);
	}

	private static void RefreshStoragePanel(CollapsibleDetailContentPanel targetPanel, GameObject targetEntity)
	{
		if (targetEntity == null)
		{
			targetPanel.gameObject.SetActive(false);
			targetPanel.Commit();
			return;
		}
		IStorage[] array = targetEntity.GetComponentsInChildren<IStorage>();
		if (array == null)
		{
			targetPanel.gameObject.SetActive(false);
			targetPanel.Commit();
			return;
		}
		array = Array.FindAll<IStorage>(array, (IStorage n) => n.ShouldShowInUI());
		if (array.Length == 0)
		{
			targetPanel.gameObject.SetActive(false);
			targetPanel.Commit();
			return;
		}
		string text = ((targetEntity.GetComponent<MinionIdentity>() != null) ? UI.DETAILTABS.DETAILS.GROUPNAME_MINION_CONTENTS : UI.DETAILTABS.DETAILS.GROUPNAME_CONTENTS);
		targetPanel.gameObject.SetActive(true);
		targetPanel.SetTitle(text);
		DictionaryPool<int, SimpleInfoScreen.StoredItemCategoryData, SimpleInfoScreen>.PooledDictionary pooledDictionary = DictionaryPool<int, SimpleInfoScreen.StoredItemCategoryData, SimpleInfoScreen>.Allocate();
		SimpleInfoScreen.storageItemPrefabDataIndexesFound.Clear();
		IStorage[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			foreach (GameObject gameObject in array2[i].GetItems())
			{
				if (!(gameObject == null))
				{
					KPrefabID component = gameObject.GetComponent<KPrefabID>();
					PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
					if (!(component2 != null) || component2.Mass != 0f)
					{
						int hashCode = component.GetHashCode();
						float mass = component2.Mass;
						SimpleInfoScreen.StoredItemCategoryData storedItemCategoryData;
						if (!pooledDictionary.TryGetValue(hashCode, out storedItemCategoryData))
						{
							storedItemCategoryData = new SimpleInfoScreen.StoredItemCategoryData(component.GetProperName(), 0f, component2.MassPerUnit);
							pooledDictionary[hashCode] = storedItemCategoryData;
							SimpleInfoScreen.storageItemPrefabDataIndexesFound.Add(hashCode);
						}
						storedItemCategoryData.mass += mass;
						storedItemCategoryData.temperatureRanges.x = Mathf.Min(storedItemCategoryData.temperatureRanges.x, component2.Temperature);
						storedItemCategoryData.temperatureRanges.y = Mathf.Max(storedItemCategoryData.temperatureRanges.y, component2.Temperature);
						storedItemCategoryData.instancesFound++;
						storedItemCategoryData.lastInstance = component;
						storedItemCategoryData.lastPEInstance = component2;
					}
				}
			}
		}
		int num = 0;
		foreach (int num2 in SimpleInfoScreen.storageItemPrefabDataIndexesFound)
		{
			SimpleInfoScreen.StoredItemCategoryData storedItemCategoryData2 = pooledDictionary[num2];
			int num3 = num2;
			string text2 = "";
			string text3 = "";
			string text4 = "";
			string text5 = "";
			text2 = storedItemCategoryData2.name;
			if (storedItemCategoryData2.instancesFound == 1)
			{
				SimpleInfoScreen.ForgeNameAndTooltipForStoredItem(storedItemCategoryData2.lastInstance, storedItemCategoryData2.lastPEInstance, out text2, out text4, out text5, out text3, false);
				text2 = "• " + text2;
				KSelectable itemSelectable = storedItemCategoryData2.lastInstance.GetComponent<KSelectable>();
				targetPanel.SetLabelWithButton("storage_" + num.ToString(), text2, text4, text5, text3, delegate
				{
					SelectTool.Instance.Select(itemSelectable, false);
				}).transform.SetAsFirstSibling();
			}
			else
			{
				text5 = (storedItemCategoryData2.usingUnits ? GameUtil.GetFormattedUnits(storedItemCategoryData2.mass / storedItemCategoryData2.massPerUnit, GameUtil.TimeSlice.None, true, "") : GameUtil.GetFormattedMass(storedItemCategoryData2.mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				targetPanel.SetCollapsableLabel("storage_group_" + num3.ToString(), text2, text5, text3, new SimpleInfoScreen.StorageCollapsibleRowData
				{
					prefabHashCode = num3,
					storages = array
				}, new Action<DetailCollapsableLabel>(SimpleInfoScreen.OnStorageCollapsibleRowExpanded), new Action<DetailCollapsableLabel>(SimpleInfoScreen.OnStorageCollapsibleRowCollapsed));
			}
			storedItemCategoryData2.ClearData();
			num++;
		}
		if (num == 0)
		{
			targetPanel.SetLabel("storage_empty", UI.DETAILTABS.DETAILS.STORAGE_EMPTY, "");
		}
		pooledDictionary.Recycle();
		targetPanel.Commit();
	}

	private static string GetIconsForItemName(string itemName, KPrefabID item, PrimaryElement pe, int maxIconsAllowed)
	{
		string text = itemName;
		bool flag = maxIconsAllowed <= 0;
		char c = ' ';
		if (!flag && item.HasTag(GameTags.RotModifierTags.Refrigerated))
		{
			text = text + c.ToString() + UI.DETAILTABS.TEXTICONDATA.REFRIGERATED.ICON;
			flag = --maxIconsAllowed <= 0;
		}
		else if (!flag && item.HasTag(GameTags.RotModifierTags.DeepFrozen))
		{
			text = text + c.ToString() + UI.DETAILTABS.TEXTICONDATA.DEEPFROZEN.ICON;
			flag = --maxIconsAllowed <= 0;
		}
		if (!flag && item.HasTag(GameTags.SpicedFood))
		{
			text = text + c.ToString() + UI.DETAILTABS.TEXTICONDATA.SPICEDFOOD.ICON;
			flag = --maxIconsAllowed <= 0;
		}
		if (!flag && item.HasTag(GameTags.RotModifierTags.Fresh))
		{
			text = text + c.ToString() + UI.DETAILTABS.TEXTICONDATA.FRESH.ICON;
			flag = --maxIconsAllowed <= 0;
		}
		else if (!flag && item.HasTag(GameTags.RotModifierTags.Stale))
		{
			text = text + c.ToString() + UI.DETAILTABS.TEXTICONDATA.STALE.ICON;
			flag = --maxIconsAllowed <= 0;
		}
		if (!flag && pe.DiseaseIdx != 255)
		{
			Disease disease = Db.Get().Diseases[(int)pe.DiseaseIdx];
			text = text + c.ToString() + UI.DETAILTABS.TEXTICONDATA.GERMS.ICON;
			flag = --maxIconsAllowed <= 0;
		}
		if (flag)
		{
			text += "…";
		}
		return text;
	}

	private static string GetIconsLegendForItem(KPrefabID item, PrimaryElement pe, Rottable.Instance rottable, bool addTabs = false)
	{
		string text = "";
		string text2 = (addTabs ? "\n  " : "\n");
		char c = ' ';
		if (item.HasTag(GameTags.SpicedFood))
		{
			text = string.Concat(new string[]
			{
				text,
				text2,
				UI.DETAILTABS.TEXTICONDATA.SPICEDFOOD.ICON,
				c.ToString(),
				UI.DETAILTABS.TEXTICONDATA.SPICEDFOOD.NAME
			});
		}
		if (item.HasTag(GameTags.RotModifierTags.Refrigerated))
		{
			text = string.Concat(new string[]
			{
				text,
				text2,
				UI.DETAILTABS.TEXTICONDATA.REFRIGERATED.ICON,
				c.ToString(),
				UI.DETAILTABS.TEXTICONDATA.REFRIGERATED.NAME
			});
		}
		else if (item.HasTag(GameTags.RotModifierTags.DeepFrozen))
		{
			text = string.Concat(new string[]
			{
				text,
				text2,
				UI.DETAILTABS.TEXTICONDATA.DEEPFROZEN.ICON,
				c.ToString(),
				UI.DETAILTABS.TEXTICONDATA.DEEPFROZEN.NAME
			});
		}
		if (item.HasTag(GameTags.RotModifierTags.Fresh))
		{
			string text3 = ((rottable == null) ? UI.DETAILTABS.TEXTICONDATA.FRESH.NAME : rottable.StateString());
			text = string.Concat(new string[]
			{
				text,
				text2,
				UI.DETAILTABS.TEXTICONDATA.FRESH.ICON,
				c.ToString(),
				text3
			});
		}
		else if (item.HasTag(GameTags.RotModifierTags.Stale))
		{
			string text4 = ((rottable == null) ? UI.DETAILTABS.TEXTICONDATA.STALE.NAME : rottable.StateString());
			text = string.Concat(new string[]
			{
				text,
				text2,
				UI.DETAILTABS.TEXTICONDATA.STALE.ICON,
				c.ToString(),
				text4
			});
		}
		if (pe.DiseaseIdx != 255)
		{
			Disease disease = Db.Get().Diseases[(int)pe.DiseaseIdx];
			text = string.Concat(new string[]
			{
				text,
				text2,
				UI.DETAILTABS.TEXTICONDATA.GERMS.ICON,
				c.ToString(),
				GameUtil.SafeStringFormat(UI.OVERLAYS.DISEASE.DISEASE_FORMAT, new object[]
				{
					disease.Name,
					GameUtil.GetFormattedDiseaseAmount(pe.DiseaseCount, GameUtil.TimeSlice.None)
				})
			});
		}
		return text.TrimStart('\n');
	}

	private static string GetTrimmedString(string value, int maxCharacterCount)
	{
		if (value.Length <= maxCharacterCount)
		{
			return value;
		}
		return value.Substring(0, maxCharacterCount) + "…";
	}

	private static void ForgeNameAndTooltipForStoredItem(KPrefabID itemPrefabID, PrimaryElement pe, out string nameText, out string temperatureText, out string massText, out string tooltip, bool trim)
	{
		GameObject gameObject = itemPrefabID.gameObject;
		Rottable.Instance smi = gameObject.GetSMI<Rottable.Instance>();
		HighEnergyParticleStorage component = gameObject.GetComponent<HighEnergyParticleStorage>();
		pe = ((pe == null) ? gameObject.GetComponent<PrimaryElement>() : pe);
		string text = UI.StripLinkFormatting(gameObject.GetProperName());
		nameText = (trim ? SimpleInfoScreen.GetTrimmedString(text, 15) : text);
		tooltip = "";
		massText = "";
		temperatureText = "";
		if (pe != null && component == null)
		{
			massText = ((pe.MassPerUnit > 1f) ? GameUtil.GetFormattedUnits(pe.Mass / pe.MassPerUnit, GameUtil.TimeSlice.None, true, "") : GameUtil.GetFormattedMass(pe.Mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
			temperatureText = GameUtil.GetFormattedTemperature(pe.Temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false);
			nameText = SimpleInfoScreen.GetIconsForItemName(nameText, itemPrefabID, pe, trim ? (15 - nameText.Length) : 99999);
			string iconsLegendForItem = SimpleInfoScreen.GetIconsLegendForItem(itemPrefabID, pe, smi, true);
			tooltip = text + "\n";
			tooltip += iconsLegendForItem;
			tooltip += (string.IsNullOrEmpty(iconsLegendForItem) ? "" : "\n\n");
		}
		if (component != null)
		{
			nameText = ITEMS.RADIATION.HIGHENERGYPARITCLE.NAME;
			temperatureText = GameUtil.GetFormattedHighEnergyParticles(component.Particles, GameUtil.TimeSlice.None, true);
		}
		if (smi != null)
		{
			tooltip += smi.GetToolTip();
		}
		tooltip.TrimEnd('\n');
	}

	private static void OnStorageCollapsibleRowExpanded(DetailCollapsableLabel collapsableLabel)
	{
		collapsableLabel.MarkAllRowsUnused();
		SimpleInfoScreen.StorageCollapsibleRowData storageCollapsibleRowData = collapsableLabel.Data as SimpleInfoScreen.StorageCollapsibleRowData;
		IStorage[] storages = storageCollapsibleRowData.storages;
		for (int i = 0; i < storages.Length; i++)
		{
			using (List<GameObject>.Enumerator enumerator = storages[i].GetItems().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					GameObject go = enumerator.Current;
					if (!(go == null))
					{
						KPrefabID component = go.GetComponent<KPrefabID>();
						if (component.GetHashCode() == storageCollapsibleRowData.prefabHashCode)
						{
							string text = "";
							string text2 = "";
							string text3 = "";
							string text4 = "";
							SimpleInfoScreen.ForgeNameAndTooltipForStoredItem(component, null, out text, out text3, out text2, out text4, false);
							DetailLabelWithButton detailLabelWithButton = collapsableLabel.AddOrGetAvailableContentRow();
							detailLabelWithButton.label.SetText(text);
							detailLabelWithButton.label2.SetText(text3);
							detailLabelWithButton.label3.SetText(text2);
							detailLabelWithButton.RefreshLabelsVisibility();
							detailLabelWithButton.toolTip.SetSimpleTooltip(text4.TrimEnd('\n'));
							detailLabelWithButton.button.ClearOnClick();
							detailLabelWithButton.button.onClick += delegate
							{
								SelectTool.Instance.Select(go.GetComponent<KSelectable>(), false);
							};
						}
					}
				}
			}
		}
		collapsableLabel.RefreshRowVisibilityState();
	}

	private static void OnStorageCollapsibleRowCollapsed(DetailCollapsableLabel collapsableLabel)
	{
		collapsableLabel.MarkAllRowsUnused();
		collapsableLabel.RefreshRowVisibilityState();
	}

	private void CreateWorldTraitRow()
	{
		GameObject gameObject = global::Util.KInstantiateUI(this.iconLabelRow, this.worldTraitsPanel.Content.gameObject, true);
		this.worldTraitRows.Add(gameObject);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		component.GetReference<Image>("Icon").gameObject.SetActive(false);
		component.GetReference<LocText>("ValueLabel").gameObject.SetActive(false);
	}

	private static void RefreshMovePanel(CollapsibleDetailContentPanel targetPanel, GameObject targetEntity)
	{
		CancellableMove component = targetEntity.GetComponent<CancellableMove>();
		Movable moving = targetEntity.GetComponent<Movable>();
		if (component != null)
		{
			List<Ref<Movable>> movingObjects = component.movingObjects;
			int num = 0;
			using (List<Ref<Movable>>.Enumerator enumerator = movingObjects.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Ref<Movable> @ref = enumerator.Current;
					Movable movable = @ref.Get();
					GameObject go = ((movable != null) ? movable.gameObject : null);
					if (!(go == null))
					{
						PrimaryElement component2 = go.GetComponent<PrimaryElement>();
						if (!(component2 != null) || component2.Mass != 0f)
						{
							Rottable.Instance smi = go.GetSMI<Rottable.Instance>();
							HighEnergyParticleStorage component3 = go.GetComponent<HighEnergyParticleStorage>();
							string text = "";
							string text2 = "";
							if (component2 != null && component3 == null)
							{
								text = GameUtil.GetUnitFormattedName(go, false);
								text = string.Format(UI.DETAILTABS.DETAILS.CONTENTS_MASS, text, GameUtil.GetFormattedMass(component2.Mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
								text = string.Format(UI.DETAILTABS.DETAILS.CONTENTS_TEMPERATURE, text, GameUtil.GetFormattedTemperature(component2.Temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
							}
							if (component3 != null)
							{
								text = ITEMS.RADIATION.HIGHENERGYPARITCLE.NAME;
								text = string.Format(UI.DETAILTABS.DETAILS.CONTENTS_MASS, text, GameUtil.GetFormattedHighEnergyParticles(component3.Particles, GameUtil.TimeSlice.None, true));
							}
							if (smi != null)
							{
								string text3 = smi.StateString();
								if (!string.IsNullOrEmpty(text3))
								{
									text += string.Format(UI.DETAILTABS.DETAILS.CONTENTS_ROTTABLE, text3);
								}
								text2 += smi.GetToolTip();
							}
							if (component2.DiseaseIdx != 255)
							{
								text += string.Format(UI.DETAILTABS.DETAILS.CONTENTS_DISEASED, GameUtil.GetFormattedDisease(component2.DiseaseIdx, component2.DiseaseCount, false));
								string formattedDisease = GameUtil.GetFormattedDisease(component2.DiseaseIdx, component2.DiseaseCount, true);
								text2 += formattedDisease;
							}
							targetPanel.SetLabelWithButton("move_" + num.ToString(), text, text2, delegate
							{
								SelectTool.Instance.SelectAndFocus(go.transform.GetPosition(), go.GetComponent<KSelectable>(), new Vector3(5f, 0f, 0f));
							});
							num++;
						}
					}
				}
				goto IL_029C;
			}
		}
		if (moving != null && moving.IsMarkedForMove)
		{
			targetPanel.SetLabelWithButton("moveplacer", MISC.PLACERS.MOVEPICKUPABLEPLACER.PLACER_STATUS, MISC.PLACERS.MOVEPICKUPABLEPLACER.PLACER_STATUS_TOOLTIP, delegate
			{
				SelectTool.Instance.SelectAndFocus(moving.StorageProxy.transform.GetPosition(), moving.StorageProxy.GetComponent<KSelectable>(), new Vector3(5f, 0f, 0f));
			});
		}
		IL_029C:
		targetPanel.Commit();
	}

	private void RefreshWorldPanel()
	{
		WorldContainer worldContainer = ((this.selectedTarget == null) ? null : this.selectedTarget.GetComponent<WorldContainer>());
		AsteroidGridEntity asteroidGridEntity = ((this.selectedTarget == null) ? null : this.selectedTarget.GetComponent<AsteroidGridEntity>());
		bool flag = ManagementMenu.Instance.IsScreenOpen(ClusterMapScreen.Instance) && worldContainer != null && asteroidGridEntity != null;
		this.worldBiomesPanel.gameObject.SetActive(flag);
		this.worldGeysersPanel.gameObject.SetActive(flag);
		this.worldMeteorShowersPanel.gameObject.SetActive(flag);
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
				goto IL_023C;
			}
		}
		this.worldBiomesPanel.gameObject.SetActive(false);
		IL_023C:
		List<Tag> list = new List<Tag>();
		foreach (Geyser geyser in Components.Geysers.GetItems(worldContainer.id))
		{
			list.Add(geyser.PrefabID());
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
			}
			HierarchyReferences component3 = this.geyserRows[tag2].GetComponent<HierarchyReferences>();
			component3.GetReference<Image>("Icon").sprite = uisprite2.first;
			component3.GetReference<Image>("Icon").color = uisprite2.second;
			component3.GetReference<LocText>("NameLabel").SetText(UI.DETAILTABS.SIMPLEINFO.UNKNOWN_GEYSERS.Replace("{num}", count.ToString()));
			component3.GetReference<LocText>("ValueLabel").gameObject.SetActive(false);
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
		this.geyserRows[tag3].gameObject.SetActive(list.Count == 0 && count == 0);
		foreach (KeyValuePair<Tag, GameObject> keyValuePair3 in this.meteorShowerRows)
		{
			keyValuePair3.Value.SetActive(false);
		}
		bool flag2 = false;
		foreach (string text2 in worldContainer.GetSeasonIds())
		{
			GameplaySeason gameplaySeason = Db.Get().GameplaySeasons.TryGet(text2);
			if (gameplaySeason != null)
			{
				foreach (GameplayEvent gameplayEvent in gameplaySeason.events)
				{
					if (gameplayEvent.tags.Contains(GameTags.SpaceDanger) && gameplayEvent is MeteorShowerEvent)
					{
						flag2 = true;
						MeteorShowerEvent meteorShowerEvent = gameplayEvent as MeteorShowerEvent;
						string id = meteorShowerEvent.Id;
						global::Tuple<Sprite, Color> uisprite3 = Def.GetUISprite(meteorShowerEvent.GetClusterMapMeteorShowerID(), "ui", false);
						if (!this.meteorShowerRows.ContainsKey(id))
						{
							this.meteorShowerRows.Add(id, global::Util.KInstantiateUI(this.iconLabelRow, this.worldMeteorShowersPanel.Content.gameObject, true));
							HierarchyReferences component5 = this.meteorShowerRows[id].GetComponent<HierarchyReferences>();
							component5.GetReference<Image>("Icon").sprite = uisprite3.first;
							component5.GetReference<Image>("Icon").color = uisprite3.second;
							component5.GetReference<LocText>("NameLabel").SetText(Assets.GetPrefab(meteorShowerEvent.GetClusterMapMeteorShowerID()).GetProperName());
							component5.GetReference<LocText>("ValueLabel").gameObject.SetActive(false);
						}
						this.meteorShowerRows[id].SetActive(true);
					}
				}
			}
		}
		Tag tag4 = "NoMeteorShowers";
		if (!this.meteorShowerRows.ContainsKey(tag4))
		{
			this.meteorShowerRows.Add(tag4, global::Util.KInstantiateUI(this.iconLabelRow, this.worldMeteorShowersPanel.Content.gameObject, true));
			HierarchyReferences component6 = this.meteorShowerRows[tag4].GetComponent<HierarchyReferences>();
			component6.GetReference<Image>("Icon").sprite = Assets.GetSprite("icon_action_cancel");
			component6.GetReference<LocText>("NameLabel").SetText(UI.DETAILTABS.SIMPLEINFO.NO_METEORSHOWERS);
			component6.GetReference<LocText>("ValueLabel").gameObject.SetActive(false);
		}
		this.meteorShowerRows[tag4].gameObject.SetActive(!flag2);
		List<string> worldTraitIds = worldContainer.WorldTraitIds;
		if (worldTraitIds != null)
		{
			for (int i = 0; i < worldTraitIds.Count; i++)
			{
				if (i > this.worldTraitRows.Count - 1)
				{
					this.CreateWorldTraitRow();
				}
				WorldTrait cachedWorldTrait = SettingsCache.GetCachedWorldTrait(worldTraitIds[i], false);
				Image reference = this.worldTraitRows[i].GetComponent<HierarchyReferences>().GetReference<Image>("Icon");
				if (cachedWorldTrait != null)
				{
					Sprite sprite = Assets.GetSprite(cachedWorldTrait.filePath.Substring(cachedWorldTrait.filePath.LastIndexOf("/") + 1));
					reference.gameObject.SetActive(true);
					reference.sprite = ((sprite == null) ? Assets.GetSprite("unknown") : sprite);
					reference.color = global::Util.ColorFromHex(cachedWorldTrait.colorHex);
					this.worldTraitRows[i].GetComponent<HierarchyReferences>().GetReference<LocText>("NameLabel").SetText(Strings.Get(cachedWorldTrait.name));
					this.worldTraitRows[i].AddOrGet<ToolTip>().SetSimpleTooltip(Strings.Get(cachedWorldTrait.description));
				}
				else
				{
					Sprite sprite2 = Assets.GetSprite("NoTraits");
					reference.gameObject.SetActive(true);
					reference.sprite = sprite2;
					reference.color = Color.white;
					this.worldTraitRows[i].GetComponent<HierarchyReferences>().GetReference<LocText>("NameLabel").SetText(WORLD_TRAITS.MISSING_TRAIT);
					this.worldTraitRows[i].AddOrGet<ToolTip>().SetSimpleTooltip("");
				}
			}
			for (int j = 0; j < this.worldTraitRows.Count; j++)
			{
				this.worldTraitRows[j].SetActive(j < worldTraitIds.Count);
			}
			if (worldTraitIds.Count == 0)
			{
				if (this.worldTraitRows.Count < 1)
				{
					this.CreateWorldTraitRow();
				}
				Image reference2 = this.worldTraitRows[0].GetComponent<HierarchyReferences>().GetReference<Image>("Icon");
				Sprite sprite3 = Assets.GetSprite("NoTraits");
				reference2.gameObject.SetActive(true);
				reference2.sprite = sprite3;
				reference2.color = Color.black;
				this.worldTraitRows[0].GetComponent<HierarchyReferences>().GetReference<LocText>("NameLabel").SetText(WORLD_TRAITS.NO_TRAITS.NAME_SHORTHAND);
				this.worldTraitRows[0].AddOrGet<ToolTip>().SetSimpleTooltip(WORLD_TRAITS.NO_TRAITS.DESCRIPTION);
				this.worldTraitRows[0].SetActive(true);
			}
		}
		for (int k = this.surfaceConditionRows.Count - 1; k >= 0; k--)
		{
			global::Util.KDestroyGameObject(this.surfaceConditionRows[k]);
		}
		this.surfaceConditionRows.Clear();
		GameObject gameObject = global::Util.KInstantiateUI(this.iconLabelRow, this.worldTraitsPanel.Content.gameObject, true);
		HierarchyReferences component7 = gameObject.GetComponent<HierarchyReferences>();
		component7.GetReference<Image>("Icon").sprite = Assets.GetSprite("overlay_lights");
		component7.GetReference<LocText>("NameLabel").SetText(UI.CLUSTERMAP.ASTEROIDS.SURFACE_CONDITIONS.LIGHT);
		component7.GetReference<LocText>("ValueLabel").SetText(GameUtil.GetFormattedLux(worldContainer.SunlightFixedTraits[worldContainer.sunlightFixedTrait]));
		component7.GetReference<LocText>("ValueLabel").alignment = TextAlignmentOptions.MidlineRight;
		this.surfaceConditionRows.Add(gameObject);
		GameObject gameObject2 = global::Util.KInstantiateUI(this.iconLabelRow, this.worldTraitsPanel.Content.gameObject, true);
		HierarchyReferences component8 = gameObject2.GetComponent<HierarchyReferences>();
		component8.GetReference<Image>("Icon").sprite = Assets.GetSprite("overlay_radiation");
		component8.GetReference<LocText>("NameLabel").SetText(UI.CLUSTERMAP.ASTEROIDS.SURFACE_CONDITIONS.RADIATION);
		component8.GetReference<LocText>("ValueLabel").SetText(GameUtil.GetFormattedRads((float)worldContainer.CosmicRadiationFixedTraits[worldContainer.cosmicRadiationFixedTrait], GameUtil.TimeSlice.None));
		component8.GetReference<LocText>("ValueLabel").alignment = TextAlignmentOptions.MidlineRight;
		this.surfaceConditionRows.Add(gameObject2);
	}

	private void RefreshProcessConditionsPanel()
	{
		foreach (GameObject gameObject in this.processConditionRows)
		{
			global::Util.KDestroyGameObject(gameObject);
		}
		this.processConditionRows.Clear();
		this.processConditionContainer.SetActive(this.selectedTarget.GetComponent<IProcessConditionSet>() != null);
		if (!DlcManager.FeatureClusterSpaceEnabled())
		{
			if (this.selectedTarget.GetComponent<LaunchableRocket>() != null)
			{
				this.RefreshProcessConditionsForType(this.selectedTarget, ProcessCondition.ProcessConditionType.RocketPrep);
				this.RefreshProcessConditionsForType(this.selectedTarget, ProcessCondition.ProcessConditionType.RocketStorage);
				this.RefreshProcessConditionsForType(this.selectedTarget, ProcessCondition.ProcessConditionType.RocketBoard);
				return;
			}
			this.RefreshProcessConditionsForType(this.selectedTarget, ProcessCondition.ProcessConditionType.All);
			return;
		}
		else
		{
			if (this.selectedTarget.GetComponent<LaunchPad>() != null || this.selectedTarget.GetComponent<RocketProcessConditionDisplayTarget>() != null)
			{
				this.RefreshProcessConditionsForType(this.selectedTarget, ProcessCondition.ProcessConditionType.RocketFlight);
				this.RefreshProcessConditionsForType(this.selectedTarget, ProcessCondition.ProcessConditionType.RocketPrep);
				this.RefreshProcessConditionsForType(this.selectedTarget, ProcessCondition.ProcessConditionType.RocketStorage);
				this.RefreshProcessConditionsForType(this.selectedTarget, ProcessCondition.ProcessConditionType.RocketBoard);
				return;
			}
			this.RefreshProcessConditionsForType(this.selectedTarget, ProcessCondition.ProcessConditionType.All);
			return;
		}
	}

	private static void RefreshStressPanel(CollapsibleDetailContentPanel targetPanel, GameObject targetEntity)
	{
		if (((targetEntity != null) ? targetEntity.GetComponent<MinionIdentity>() : null) != null)
		{
			List<ReportManager.ReportEntry.Note> stressNotes = new List<ReportManager.ReportEntry.Note>();
			targetPanel.gameObject.SetActive(true);
			targetPanel.SetTitle(UI.DETAILTABS.STATS.GROUPNAME_STRESS);
			ReportManager.ReportEntry reportEntry = ReportManager.Instance.TodaysReport.reportEntries.Find((ReportManager.ReportEntry entry) => entry.reportType == ReportManager.ReportType.StressDelta);
			float num = 0f;
			stressNotes.Clear();
			ReportManager.ReportEntry reportEntry2 = reportEntry.FindEntryFromGameObject(targetEntity);
			if (reportEntry2 != null)
			{
				reportEntry2.IterateNotes(delegate(ReportManager.ReportEntry.Note note)
				{
					stressNotes.Add(note);
				});
				stressNotes.Sort((ReportManager.ReportEntry.Note a, ReportManager.ReportEntry.Note b) => a.value.CompareTo(b.value));
				for (int i = 0; i < stressNotes.Count; i++)
				{
					string text = (float.IsNegativeInfinity(stressNotes[i].value) ? UI.NEG_INFINITY.ToString() : global::Util.FormatTwoDecimalPlace(stressNotes[i].value));
					targetPanel.SetLabel("stressNotes_" + i.ToString(), string.Concat(new string[]
					{
						(stressNotes[i].value > 0f) ? UIConstants.ColorPrefixRed : "",
						stressNotes[i].note,
						": ",
						text,
						"%",
						(stressNotes[i].value > 0f) ? UIConstants.ColorSuffix : ""
					}), "");
					num += stressNotes[i].value;
				}
			}
			string text2 = (float.IsNegativeInfinity(num) ? UI.NEG_INFINITY.ToString() : global::Util.FormatTwoDecimalPlace(num));
			targetPanel.SetLabel("net_stress", ((num > 0f) ? UIConstants.ColorPrefixRed : "") + string.Format(UI.DETAILTABS.DETAILS.NET_STRESS, text2) + ((num > 0f) ? UIConstants.ColorSuffix : ""), "");
		}
		targetPanel.Commit();
	}

	private void RefreshProcessConditionsForType(GameObject target, ProcessCondition.ProcessConditionType conditionType)
	{
		IProcessConditionSet component = target.GetComponent<IProcessConditionSet>();
		if (component == null)
		{
			return;
		}
		List<ProcessCondition> list;
		using (ProcessCondition.ListPool.Get(out list))
		{
			if (component.PopulateConditionSet(conditionType, list) != 0)
			{
				HierarchyReferences hierarchyReferences = global::Util.KInstantiateUI<HierarchyReferences>(this.processConditionHeader.gameObject, this.processConditionContainer.Content.gameObject, true);
				hierarchyReferences.GetReference<LocText>("Label").text = Strings.Get("STRINGS.UI.DETAILTABS.PROCESS_CONDITIONS." + conditionType.ToString().ToUpper());
				hierarchyReferences.GetComponent<ToolTip>().toolTip = Strings.Get("STRINGS.UI.DETAILTABS.PROCESS_CONDITIONS." + conditionType.ToString().ToUpper() + "_TOOLTIP");
				this.processConditionRows.Add(hierarchyReferences.gameObject);
				List<ProcessCondition> list2 = new List<ProcessCondition>();
				using (List<ProcessCondition>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ProcessCondition condition = enumerator.Current;
						if (condition.ShowInUI() && (condition.GetType() == typeof(RequireAttachedComponent) || list2.Find((ProcessCondition match) => match.GetType() == condition.GetType()) == null))
						{
							list2.Add(condition);
							GameObject gameObject = global::Util.KInstantiateUI(this.processConditionRow, this.processConditionContainer.Content.gameObject, true);
							this.processConditionRows.Add(gameObject);
							ConditionListSideScreen.SetRowState(gameObject, condition);
						}
					}
				}
			}
		}
	}

	public GameObject iconLabelRow;

	public GameObject spacerRow;

	[SerializeField]
	private GameObject attributesLabelTemplate;

	[SerializeField]
	private GameObject attributesLabelButtonTemplate;

	[SerializeField]
	private DescriptorPanel DescriptorContentPrefab;

	[SerializeField]
	private GameObject VitalsPanelTemplate;

	[SerializeField]
	private GameObject StatusItemPrefab;

	[SerializeField]
	private Sprite statusWarningIcon;

	[SerializeField]
	private HierarchyReferences processConditionHeader;

	[SerializeField]
	private GameObject processConditionRow;

	[SerializeField]
	private Text StatusPanelCurrentActionLabel;

	[SerializeField]
	private GameObject bigIconLabelRow;

	[SerializeField]
	private TextStyleSetting ToolTipStyle_Property;

	[SerializeField]
	private TextStyleSetting StatusItemStyle_Main;

	[SerializeField]
	private TextStyleSetting StatusItemStyle_Other;

	[SerializeField]
	private Color statusItemTextColor_regular = Color.black;

	[SerializeField]
	private Color statusItemTextColor_old = new Color(0.8235294f, 0.8235294f, 0.8235294f);

	private CollapsibleDetailContentPanel statusItemPanel;

	private MinionVitalsPanel vitalsPanel;

	private CollapsibleDetailContentPanel fertilityPanel;

	private CollapsibleDetailContentPanel mooFertilityPanel;

	private CollapsibleDetailContentPanel rocketStatusContainer;

	private CollapsibleDetailContentPanel worldLifePanel;

	private CollapsibleDetailContentPanel worldElementsPanel;

	private CollapsibleDetailContentPanel worldBiomesPanel;

	private CollapsibleDetailContentPanel worldGeysersPanel;

	private CollapsibleDetailContentPanel worldMeteorShowersPanel;

	private CollapsibleDetailContentPanel spacePOIPanel;

	private CollapsibleDetailContentPanel spaceHexCellStoragePanel;

	private CollapsibleDetailContentPanel worldTraitsPanel;

	private CollapsibleDetailContentPanel processConditionContainer;

	private CollapsibleDetailContentPanel requirementsPanel;

	private CollapsibleDetailContentPanel effectsPanel;

	private CollapsibleDetailContentPanel stressPanel;

	private CollapsibleDetailContentPanel infoPanel;

	private CollapsibleDetailContentPanel movePanel;

	private DescriptorPanel effectsContent;

	private DescriptorPanel requirementContent;

	private RocketSimpleInfoPanel rocketSimpleInfoPanel;

	private SpacePOISimpleInfoPanel spaceSimpleInfoPOIPanel;

	private StarmapHexCellInventoryInfoPanel starmapHexCellStorageInfoPanel;

	private DetailsPanelDrawer stressDrawer;

	private bool TargetIsMinion;

	private GameObject lastTarget;

	private GameObject statusItemsFolder;

	private Dictionary<Tag, GameObject> lifeformRows = new Dictionary<Tag, GameObject>();

	private Dictionary<Tag, GameObject> biomeRows = new Dictionary<Tag, GameObject>();

	private Dictionary<Tag, GameObject> geyserRows = new Dictionary<Tag, GameObject>();

	private Dictionary<Tag, GameObject> meteorShowerRows = new Dictionary<Tag, GameObject>();

	private List<GameObject> worldTraitRows = new List<GameObject>();

	private List<GameObject> surfaceConditionRows = new List<GameObject>();

	private List<SimpleInfoScreen.StatusItemEntry> statusItems = new List<SimpleInfoScreen.StatusItemEntry>();

	private List<SimpleInfoScreen.StatusItemEntry> oldStatusItems = new List<SimpleInfoScreen.StatusItemEntry>();

	private List<GameObject> processConditionRows = new List<GameObject>();

	private static readonly EventSystem.IntraObjectHandler<SimpleInfoScreen> OnRefreshDataDelegate = new EventSystem.IntraObjectHandler<SimpleInfoScreen>(delegate(SimpleInfoScreen component, object data)
	{
		component.OnRefreshData(data);
	});

	private const string STORAGE_ROW_ID_PREFIX = "storage_";

	private const string STORAGE_GROUP_ROW_ID_PREFIX = "storage_group_";

	private const int MAXStoreItemNameCharacterCount = 15;

	private const string TRIMMED_STRING = "…";

	private static List<int> storageItemPrefabDataIndexesFound = new List<int>();

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
			if (this.toolTip != null)
			{
				this.toolTip.OnToolTip = null;
			}
			if (this.button != null && this.button.enabled)
			{
				this.button.onClick -= this.OnClick;
			}
			if (immediate)
			{
				if (this.onDestroy != null)
				{
					this.onDestroy(this);
				}
				SimAndRenderScheduler.instance.Remove(this);
				global::UnityEngine.Object.Destroy(this.widget);
				return;
			}
			this.fade = 0.5f;
			this.fadeStage = SimpleInfoScreen.StatusItemEntry.FadeStage.OUT;
		}

		public StatusItemGroup.Entry item;

		public StatusItemCategory category;

		public Color color;

		public TextStyleSetting style;

		public Action<SimpleInfoScreen.StatusItemEntry> onDestroy;

		private LayoutElement spacerLayout;

		private GameObject widget;

		private ToolTip toolTip;

		private TextStyleSetting tooltipStyle;

		private Image image;

		private LocText text;

		private KButton button;

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

	private class StorageCollapsibleRowData
	{
		public int prefabHashCode;

		public IStorage[] storages;
	}

	private class StoredItemCategoryData
	{
		public bool usingUnits
		{
			get
			{
				return this.massPerUnit > 1f;
			}
		}

		public StoredItemCategoryData(string name, float m, float massPerUnit)
		{
			this.temperatureRanges = new Vector2(float.MaxValue, float.MinValue);
			this.name = name;
			this.mass = m;
			this.massPerUnit = massPerUnit;
		}

		public void ClearData()
		{
			this.name = null;
			this.mass = 0f;
			this.massPerUnit = 1f;
			this.temperatureRanges = new Vector2(float.MaxValue, float.MinValue);
			this.instancesFound = 0;
			this.lastInstance = null;
			this.lastPEInstance = null;
		}

		public float mass;

		public float massPerUnit;

		public string name;

		public Vector2 temperatureRanges;

		public int instancesFound;

		public KPrefabID lastInstance;

		public PrimaryElement lastPEInstance;
	}
}
