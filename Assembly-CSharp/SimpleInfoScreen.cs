using System;
using System.Collections.Generic;
using System.Diagnostics;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class SimpleInfoScreen : TargetScreen
{
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
		this.statusItemPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.statusItemPanel.Content.GetComponent<VerticalLayoutGroup>().padding.bottom = 10;
		this.statusItemPanel.HeaderLabel.text = UI.DETAILTABS.SIMPLEINFO.GROUPNAME_STATUS;
		this.statusItemPanel.scalerMask.hoverLock = true;
		this.statusItemsFolder = this.statusItemPanel.Content.gameObject;
		this.vitalsPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.vitalsPanel.SetTitle(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_CONDITION);
		this.vitalsContainer = Util.KInstantiateUI(this.VitalsPanelTemplate, this.vitalsPanel.Content.gameObject, false).GetComponent<MinionVitalsPanel>();
		this.fertilityPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.fertilityPanel.SetTitle(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_FERTILITY);
		this.infoPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.infoPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.SIMPLEINFO.GROUPNAME_DESCRIPTION;
		GameObject gameObject = this.infoPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject;
		this.descriptionContainer = Util.KInstantiateUI<DescriptionContainer>(this.DescriptionContainerTemplate, gameObject, false);
		this.storagePanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.stressPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.stressDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, this.stressPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
		this.stampContainer = Util.KInstantiateUI(this.StampContainerTemplate, gameObject, false);
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
		this.vitalsPanel.SetTitle((!(target.GetComponent<WiltCondition>() == null)) ? UI.DETAILTABS.SIMPLEINFO.GROUPNAME_REQUIREMENTS : UI.DETAILTABS.SIMPLEINFO.GROUPNAME_CONDITION);
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
			color = this.statusItemTextColor_bad;
		}
		else
		{
			color = this.statusItemTextColor_regular;
		}
		TextStyleSetting textStyleSetting = ((category != Db.Get().StatusItemCategories.Main) ? this.StatusItemStyle_Other : this.StatusItemStyle_Main);
		SimpleInfoScreen.StatusItemEntry statusItemEntry = new SimpleInfoScreen.StatusItemEntry(status_item, category, this.StatusItemPrefab, gameObject.transform, this.ToolTipStyle_Property, color, textStyleSetting, show_immediate, new Action<SimpleInfoScreen.StatusItemEntry>(this.OnStatusItemDestroy));
		statusItemEntry.SetSprite(status_item.item.sprite);
		if (category != null)
		{
			int num = -1;
			List<SimpleInfoScreen.StatusItemEntry> list = this.oldStatusItems.FindAll((SimpleInfoScreen.StatusItemEntry e) => e.category == category);
			foreach (SimpleInfoScreen.StatusItemEntry statusItemEntry2 in list)
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
				break;
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
		this.attributeLabels.ForEach(delegate(LocText x)
		{
			global::UnityEngine.Object.Destroy(x.gameObject);
		});
		this.attributeLabels.Clear();
		this.vitalsPanel.gameObject.SetActive(amounts != null);
		string text = string.Empty;
		string text2 = string.Empty;
		if (amounts != null)
		{
			this.vitalsContainer.selectedEntity = this.selectedTarget;
			Uprootable component8 = this.selectedTarget.gameObject.GetComponent<Uprootable>();
			if (component8 != null)
			{
				this.vitalsPanel.gameObject.SetActive(component8.GetPlanterStorage != null);
			}
			Growing component9 = this.selectedTarget.gameObject.GetComponent<Growing>();
			if (component9 != null)
			{
				this.vitalsPanel.gameObject.SetActive(true);
			}
		}
		if (component)
		{
			text = string.Empty;
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
			text = ((element == null) ? string.Empty : element.FullDescription(false));
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
		this.infoPanel.gameObject.SetActive(component == null);
		this.descriptionContainer.gameObject.SetActive(this.infoPanel.activeSelf);
		this.descriptionContainer.flavour.gameObject.SetActive(text2 != string.Empty && text2 != "\n");
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
				string text = string.Empty;
				foreach (FertilityModifier fertilityModifier in forTag)
				{
					text += string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_MOD_FORMAT, fertilityModifier.GetTooltip());
				}
				this.fertilityPanel.SetLabel("breeding_" + num++, string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT, breedingChance.egg.ProperName(), GameUtil.GetFormattedPercent(breedingChance.weight * 100f, GameUtil.TimeSlice.None)), string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT_TOOLTIP, breedingChance.egg.ProperName(), GameUtil.GetFormattedPercent(breedingChance.weight * 100f, GameUtil.TimeSlice.None), text));
			}
			else
			{
				this.fertilityPanel.SetLabel("breeding_" + num++, string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT, breedingChance.egg.ProperName(), GameUtil.GetFormattedPercent(breedingChance.weight * 100f, GameUtil.TimeSlice.None)), string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT_TOOLTIP_NOMOD, breedingChance.egg.ProperName(), GameUtil.GetFormattedPercent(breedingChance.weight * 100f, GameUtil.TimeSlice.None)));
			}
		}
		this.fertilityPanel.Commit();
	}

	private void RefreshStorage()
	{
		if (this.selectedTarget == null)
		{
			this.storagePanel.gameObject.SetActive(false);
			return;
		}
		Storage[] array = this.selectedTarget.GetComponentsInChildren<Storage>();
		if (array == null)
		{
			this.storagePanel.gameObject.SetActive(false);
			return;
		}
		array = Array.FindAll<Storage>(array, (Storage n) => n.showInUI);
		if (array.Length == 0)
		{
			this.storagePanel.gameObject.SetActive(false);
			return;
		}
		this.storagePanel.gameObject.SetActive(true);
		string text = ((!(this.selectedTarget.GetComponent<MinionIdentity>() != null)) ? UI.DETAILTABS.DETAILS.GROUPNAME_CONTENTS : UI.DETAILTABS.DETAILS.GROUPNAME_MINION_CONTENTS);
		this.storagePanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = text;
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.storageLabels)
		{
			keyValuePair.Value.SetActive(false);
		}
		int num = 0;
		foreach (Storage storage in array)
		{
			foreach (GameObject gameObject in storage.items)
			{
				if (!(gameObject == null))
				{
					GameObject gameObject2 = this.AddOrGetStorageLabel(this.storageLabels, this.storagePanel, "storage_" + num.ToString());
					num++;
					if (storage.allowUIItemRemoval)
					{
						Transform transform = gameObject2.transform.Find("removeAttributeButton");
						if (transform != null)
						{
							KButton component = transform.GetComponent<KButton>();
							component.enabled = true;
							component.gameObject.SetActive(true);
							GameObject select_item = gameObject;
							Storage selected_storage = storage;
							component.onClick += delegate
							{
								selected_storage.Remove(select_item, true);
							};
						}
					}
					PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
					Rottable.Instance smi = gameObject.GetSMI<Rottable.Instance>();
					gameObject2.GetComponentInChildren<ToolTip>().ClearMultiStringTooltip();
					string text2 = GameUtil.GetUnitFormattedName(gameObject, false);
					text2 = string.Format(UI.DETAILTABS.DETAILS.CONTENTS_MASS, text2, GameUtil.GetFormattedMass(component2.Mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
					text2 = string.Format(UI.DETAILTABS.DETAILS.CONTENTS_TEMPERATURE, text2, GameUtil.GetFormattedTemperature(component2.Temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
					if (smi != null)
					{
						string text3 = smi.StateString();
						if (!string.IsNullOrEmpty(text3))
						{
							text2 += string.Format(UI.DETAILTABS.DETAILS.CONTENTS_ROTTABLE, text3);
						}
						gameObject2.GetComponentInChildren<ToolTip>().AddMultiStringTooltip(smi.GetToolTip(), PluginAssets.Instance.defaultTextStyleSetting);
					}
					if (component2.DiseaseIdx != 255)
					{
						text2 += string.Format(UI.DETAILTABS.DETAILS.CONTENTS_DISEASED, GameUtil.GetFormattedDisease(component2.DiseaseIdx, component2.DiseaseCount, false));
						string formattedDisease = GameUtil.GetFormattedDisease(component2.DiseaseIdx, component2.DiseaseCount, true);
						gameObject2.GetComponentInChildren<ToolTip>().AddMultiStringTooltip(formattedDisease, PluginAssets.Instance.defaultTextStyleSetting);
					}
					gameObject2.GetComponentInChildren<LocText>().text = text2;
					KButton component3 = gameObject2.GetComponent<KButton>();
					GameObject select_target = gameObject;
					component3.onClick += delegate
					{
						SelectTool.Instance.Select(select_target.GetComponent<KSelectable>(), false);
					};
				}
			}
		}
		if (num == 0)
		{
			GameObject gameObject3 = this.AddOrGetStorageLabel(this.storageLabels, this.storagePanel, "empty");
			gameObject3.GetComponentInChildren<LocText>().text = UI.DETAILTABS.DETAILS.STORAGE_EMPTY;
		}
	}

	private GameObject AddOrGetStorageLabel(Dictionary<string, GameObject> labels, GameObject panel, string id)
	{
		GameObject gameObject;
		if (labels.ContainsKey(id))
		{
			gameObject = labels[id];
			KButton component = gameObject.GetComponent<KButton>();
			component.ClearOnClick();
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
			gameObject = Util.KInstantiate(this.attributesLabelButtonTemplate, panel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject, null);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			labels[id] = gameObject;
		}
		gameObject.SetActive(true);
		return gameObject;
	}

	private void RefreshStress()
	{
		MinionIdentity identity = ((!(this.selectedTarget != null)) ? null : this.selectedTarget.GetComponent<MinionIdentity>());
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
		ReportManager.ReportEntry reportEntry2 = ((num2 == -1) ? null : reportEntry.contextEntries[num2]);
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
					(stressNotes[i].value <= 0f) ? string.Empty : UIConstants.ColorPrefixRed,
					stressNotes[i].note,
					": ",
					Util.FormatTwoDecimalPlace(stressNotes[i].value),
					"%",
					(stressNotes[i].value <= 0f) ? string.Empty : UIConstants.ColorSuffix
				}));
				num += stressNotes[i].value;
			}
		}
		this.stressDrawer.NewLabel(((num <= 0f) ? string.Empty : UIConstants.ColorPrefixRed) + string.Format(UI.DETAILTABS.DETAILS.NET_STRESS, Util.FormatTwoDecimalPlace(num)) + ((num <= 0f) ? string.Empty : UIConstants.ColorSuffix));
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
		BuildingComplete component = target.GetComponent<BuildingComplete>();
		if (component != null)
		{
		}
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

	private CollapsibleDetailContentPanel statusItemPanel;

	private CollapsibleDetailContentPanel vitalsPanel;

	private CollapsibleDetailContentPanel fertilityPanel;

	private GameObject storagePanel;

	private GameObject infoPanel;

	private GameObject stampContainer;

	private MinionVitalsPanel vitalsContainer;

	private GameObject InfoFolder;

	private GameObject statusItemsFolder;

	public GameObject TextContainerPrefab;

	private GameObject stressPanel;

	private DetailsPanelDrawer stressDrawer;

	private Dictionary<string, GameObject> storageLabels = new Dictionary<string, GameObject>();

	public TextStyleSetting ToolTipStyle_Property;

	public TextStyleSetting StatusItemStyle_Main;

	public TextStyleSetting StatusItemStyle_Other;

	public Color statusItemTextColor_regular = Color.black;

	public Color statusItemTextColor_bad = new Color(0.95686275f, 0.2901961f, 0.2784314f);

	public Color statusItemTextColor_old = new Color(0.8235294f, 0.8235294f, 0.8235294f);

	private GameObject lastTarget;

	private bool TargetIsMinion;

	private List<SimpleInfoScreen.StatusItemEntry> statusItems = new List<SimpleInfoScreen.StatusItemEntry>();

	private List<SimpleInfoScreen.StatusItemEntry> oldStatusItems = new List<SimpleInfoScreen.StatusItemEntry>();

	private List<LocText> attributeLabels = new List<LocText>();

	private Action<object> onStorageChangeDelegate;

	private static readonly EventSystem.IntraObjectHandler<SimpleInfoScreen> OnRefreshDataDelegate = new EventSystem.IntraObjectHandler<SimpleInfoScreen>(delegate(SimpleInfoScreen component, object data)
	{
		component.OnRefreshData(data);
	});

	[DebuggerDisplay("{item.item.Name}")]
	public class StatusItemEntry : IRenderEveryTick
	{
		public StatusItemEntry(StatusItemGroup.Entry item, StatusItemCategory category, GameObject status_item_prefab, Transform parent, TextStyleSetting tooltip_style, Color color, TextStyleSetting style, bool skip_fade, Action<SimpleInfoScreen.StatusItemEntry> onDestroy)
		{
			this.item = item;
			this.category = category;
			this.tooltipStyle = tooltip_style;
			this.onDestroy = onDestroy;
			this.color = color;
			this.style = style;
			this.widget = Util.KInstantiateUI(status_item_prefab, parent.gameObject, false);
			this.text = this.widget.GetComponentInChildren<LocText>(true);
			SetTextStyleSetting.ApplyStyle(this.text, style);
			this.toolTip = this.widget.GetComponentInChildren<ToolTip>(true);
			this.image = this.widget.GetComponentInChildren<Image>(true);
			item.SetIcon(this.image);
			this.widget.SetActive(true);
			this.toolTip.OnToolTip = new Func<string>(this.OnToolTip);
			this.fadeStage = ((!skip_fade) ? SimpleInfoScreen.StatusItemEntry.FadeStage.IN : SimpleInfoScreen.StatusItemEntry.FadeStage.WAIT);
			SimAndRenderScheduler.instance.Add(this, false);
			this.Refresh();
			this.SetColor(1f);
		}

		public Image GetImage
		{
			get
			{
				return this.image;
			}
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
			SimpleInfoScreen.StatusItemEntry.FadeStage fadeStage = this.fadeStage;
			if (fadeStage != SimpleInfoScreen.StatusItemEntry.FadeStage.IN)
			{
				if (fadeStage != SimpleInfoScreen.StatusItemEntry.FadeStage.WAIT)
				{
					if (fadeStage == SimpleInfoScreen.StatusItemEntry.FadeStage.OUT)
					{
						float num = this.fade;
						this.SetColor(num);
						this.fade = Mathf.Max(this.fade - Time.deltaTime / this.fadeOutTime, 0f);
						if (this.fade <= 0f)
						{
							this.Destroy(true);
						}
					}
				}
			}
			else
			{
				this.fade = Mathf.Min(this.fade + Time.deltaTime / this.fadeInTime, 1f);
				float num2 = this.fade;
				this.SetColor(num2);
				if (this.fade >= 1f)
				{
					this.fadeStage = SimpleInfoScreen.StatusItemEntry.FadeStage.WAIT;
				}
			}
		}

		private string OnToolTip()
		{
			this.item.ShowToolTip(this.toolTip, this.tooltipStyle);
			return string.Empty;
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
			}
			else
			{
				this.fade = 0.5f;
				this.fadeStage = SimpleInfoScreen.StatusItemEntry.FadeStage.OUT;
			}
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
