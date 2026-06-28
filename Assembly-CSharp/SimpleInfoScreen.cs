using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class SimpleInfoScreen : TargetScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		SimpleInfoScreen.Instance = this;
		this.statusItemPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.statusItemPanel.Content.GetComponent<VerticalLayoutGroup>().padding.bottom = 10;
		this.statusItemPanel.HeaderLabel.text = UI.DETAILTABS.SIMPLEINFO.GROUPNAME_STATUS;
		this.statusItemPanel.scalerMask.hoverLock = true;
		this.statusItemsFolder = this.statusItemPanel.Content.gameObject;
		this.vitalsPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.vitalsPanel.HeaderLabel.text = UI.DETAILTABS.SIMPLEINFO.GROUPNAME_CONDITION;
		this.vitalsContainer = Util.KInstantiateUI(this.VitalsPanelTemplate, this.vitalsPanel.Content.gameObject, false).GetComponent<MinionVitalsPanel>();
		this.infoPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.infoPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.SIMPLEINFO.GROUPNAME_DESCRIPTION;
		GameObject gameObject = this.infoPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject;
		this.descriptionContainer = Util.KInstantiateUI<DescriptionContainer>(this.DescriptionContainerTemplate, gameObject, false);
		this.storagePanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.stampContainer = Util.KInstantiateUI(this.StampContainerTemplate, gameObject, false);
	}

	public override void OnSelectTarget(GameObject target)
	{
		base.OnSelectTarget(target);
		base.Subscribe(target, -1697596308, new Action<object>(this.OnStorageChange));
		base.Subscribe(target, -1197125120, new Action<object>(this.OnStorageChange));
		this.RefreshStorage();
		this.mTarget = target;
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
		CellSelectionObject component2 = target.GetComponent<CellSelectionObject>();
		if (component2)
		{
			component2.OnObjectSelected(null);
		}
		this.statusItemPanel.gameObject.SetActive(true);
		this.statusItemPanel.scalerMask.UpdateSize();
		this.Refresh(false);
	}

	public override void OnDeselectTarget(GameObject target)
	{
		base.OnDeselectTarget(target);
		if (target != null)
		{
			base.Unsubscribe(target, -1697596308, new Action<object>(this.OnStorageChange));
			base.Unsubscribe(target, -1197125120, new Action<object>(this.OnStorageChange));
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

	private void OnAddStatusItem(StatusItemGroup.Entry status_item, StatusItemCategory category)
	{
		this.DoAddStatusItem(status_item, category, false);
	}

	private void DoAddStatusItem(StatusItemGroup.Entry status_item, StatusItemCategory category, bool show_immediate = false)
	{
		GameObject gameObject = this.statusItemsFolder;
		SimpleInfoScreen.StatusItemEntry statusItemEntry = new SimpleInfoScreen.StatusItemEntry(status_item, category, this.StatusItemPrefab, gameObject.transform, this.ToolTipStyle_Property, show_immediate, new Action<SimpleInfoScreen.StatusItemEntry>(this.OnStatusItemDestroy));
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

	public void Refresh(bool force = false)
	{
		if (this.mTarget != this.lastTarget || force)
		{
			this.lastTarget = this.mTarget;
			if (this.mTarget != null)
			{
				this.SetTitle(this.mTarget);
				this.SetPanels(this.mTarget);
				this.SetStamps(this.mTarget);
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
			this.vitalsContainer.Refresh(null);
		}
		this.RefreshStorage();
	}

	private void SetTitle(GameObject target)
	{
		if (DetailsScreen.Instance != null)
		{
			DetailsScreen.Instance.SetTitle(GameUtil.GetUnitFormattedName(this.mTarget, false));
		}
	}

	private void SetPortrait(GameObject target)
	{
		if (DetailsScreen.Instance != null)
		{
			DetailsScreen.Instance.UpdatePortrait(this.mTarget);
		}
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
		this.vitalsPanel.gameObject.SetActive(false);
		this.infoPanel.gameObject.SetActive(true);
		this.descriptionContainer.descriptors.gameObject.SetActive(false);
		this.descriptionContainer.gameObject.SetActive(false);
		string text = string.Empty;
		string text2 = string.Empty;
		if (amounts != null)
		{
			this.vitalsPanel.gameObject.SetActive(true);
			this.vitalsContainer.selectedEntity = this.mTarget;
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
		if (gameObjectEffects.Count > 0)
		{
			this.descriptionContainer.descriptors.gameObject.SetActive(true);
			this.descriptionContainer.descriptors.SetDescriptors(gameObjectEffects);
		}
		this.descriptionContainer.description.text = text;
		this.descriptionContainer.flavour.text = text2;
		if (this.infoPanel.activeSelf && (text == string.Empty || text == "\n"))
		{
			this.infoPanel.SetActive(false);
		}
		this.descriptionContainer.gameObject.SetActive(this.infoPanel.activeSelf);
		this.descriptionContainer.flavour.gameObject.SetActive(text2 != string.Empty && text2 != "\n");
		if (this.vitalsPanel.gameObject.activeSelf && amounts.Count == 0)
		{
			this.vitalsPanel.gameObject.SetActive(false);
		}
	}

	private void CollectStorageItems(Storage storage, ref Dictionary<string, SimpleInfoScreen.StorageEntry> item_counts)
	{
		foreach (GameObject gameObject in storage.items)
		{
			if (!(gameObject == null))
			{
				string text = gameObject.name;
				KSelectable component = gameObject.GetComponent<KSelectable>();
				if (component != null)
				{
					text = component.GetName();
				}
				if (text != null)
				{
					float totalAmount = gameObject.GetComponent<Pickupable>().TotalAmount;
					if (totalAmount != 0f)
					{
						if (item_counts.ContainsKey(text))
						{
							item_counts[text].Amount = item_counts[text].Amount + totalAmount;
						}
						else
						{
							item_counts[text] = new SimpleInfoScreen.StorageEntry
							{
								Amount = totalAmount,
								gameObject = gameObject
							};
						}
					}
				}
			}
		}
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
		if (this.selectedTarget.GetComponent<MinionIdentity>())
		{
			this.storagePanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.DETAILS.GROUPNAME_MINION_CONTENTS;
		}
		else
		{
			this.storagePanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.DETAILS.GROUPNAME_CONTENTS;
		}
		Dictionary<string, SimpleInfoScreen.StorageEntry> dictionary = new Dictionary<string, SimpleInfoScreen.StorageEntry>();
		foreach (Storage storage in array)
		{
			this.CollectStorageItems(storage, ref dictionary);
		}
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.storageLabels)
		{
			keyValuePair.Value.SetActive(false);
		}
		if (dictionary.Count > 0)
		{
			foreach (KeyValuePair<string, SimpleInfoScreen.StorageEntry> keyValuePair2 in dictionary)
			{
				GameObject gameObject = this.AddOrGetStorageLabel(this.storageLabels, this.storagePanel, keyValuePair2.Key);
				PrimaryElement component = keyValuePair2.Value.gameObject.GetComponent<PrimaryElement>();
				Rottable.Instance smi = keyValuePair2.Value.gameObject.GetSMI<Rottable.Instance>();
				string text = GameUtil.GetUnitFormattedName(keyValuePair2.Value.gameObject, false);
				text = string.Format(UI.DETAILTABS.DETAILS.CONTENTS_MASS, text, GameUtil.GetFormattedMass(keyValuePair2.Value.Amount * component.MassPerUnit, GameUtil.TimeSlice.None, true, "{0:0.#}"));
				text = string.Format(UI.DETAILTABS.DETAILS.CONTENTS_TEMPERATURE, text, GameUtil.GetFormattedTemperature(component.Temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
				if (smi != null)
				{
					text += string.Format(UI.DETAILTABS.DETAILS.CONTENTS_ROTTABLE, smi.StateString());
					gameObject.GetComponent<ToolTip>().SetSimpleTooltip(smi.GetToolTip());
				}
				else
				{
					gameObject.GetComponent<ToolTip>().SetSimpleTooltip(string.Empty);
				}
				gameObject.GetComponent<LocText>().text = text;
			}
		}
		else
		{
			GameObject gameObject2 = this.AddOrGetStorageLabel(this.storageLabels, this.storagePanel, "empty");
			gameObject2.GetComponent<LocText>().text = UI.DETAILTABS.DETAILS.STORAGE_EMPTY;
		}
	}

	private GameObject AddOrGetStorageLabel(Dictionary<string, GameObject> labels, GameObject panel, string id)
	{
		GameObject gameObject;
		if (labels.ContainsKey(id))
		{
			gameObject = labels[id];
		}
		else
		{
			gameObject = Util.KInstantiate(this.attributesLabelTemplate, panel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject, null);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			labels[id] = gameObject;
		}
		gameObject.SetActive(true);
		return gameObject;
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
				Descriptor descriptor = new Descriptor(string.Format("{0}: {1}", attributeInstance.Name, attributeInstance.GetFormattedValue(false)), attributeInstance.GetAttributeValueTooltip(), Descriptor.DescriptorType.Effect, false);
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

	public static SimpleInfoScreen Instance;

	public GameObject attributesLabelTemplate;

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

	private GameObject storagePanel;

	private GameObject infoPanel;

	private GameObject stampContainer;

	private MinionVitalsPanel vitalsContainer;

	private GameObject InfoFolder;

	private GameObject statusItemsFolder;

	public GameObject TextContainerPrefab;

	private Dictionary<string, GameObject> storageLabels = new Dictionary<string, GameObject>();

	public TextStyleSetting ToolTipStyle_Property;

	public Color statusItemTextColor_regular = Color.black;

	public Color statusItemTextColor_bad = new Color(0.95686275f, 0.2901961f, 0.2784314f);

	public Color statusItemTextColor_old = new Color(0.8235294f, 0.8235294f, 0.8235294f);

	private GameObject lastTarget;

	private GameObject mTarget;

	private bool TargetIsMinion;

	private List<SimpleInfoScreen.StatusItemEntry> statusItems = new List<SimpleInfoScreen.StatusItemEntry>();

	private List<SimpleInfoScreen.StatusItemEntry> oldStatusItems = new List<SimpleInfoScreen.StatusItemEntry>();

	private List<LocText> attributeLabels = new List<LocText>();

	private class StorageEntry
	{
		public float Amount;

		public GameObject gameObject;
	}

	public class StatusItemEntry
	{
		public StatusItemEntry(StatusItemGroup.Entry item, StatusItemCategory category, GameObject status_item_prefab, Transform parent, TextStyleSetting tooltip_style, bool skip_fade, Action<SimpleInfoScreen.StatusItemEntry> onDestroy)
		{
			this.item = item;
			this.category = category;
			this.tooltipStyle = tooltip_style;
			this.onDestroy = onDestroy;
			this.spacer = new GameObject("Spacer");
			this.spacer.transform.SetParent(parent, false);
			this.spacerLayout = this.spacer.AddComponent<LayoutElement>();
			this.widget = Util.KInstantiateUI(status_item_prefab, this.spacer, false);
			this.text = this.widget.GetComponentInChildren<LocText>(true);
			this.toolTip = this.widget.GetComponentInChildren<ToolTip>(true);
			this.image = this.widget.GetComponentInChildren<Image>(true);
			this.widgetRect = this.widget.GetComponent<RectTransform>();
			item.SetIcon(this.image);
			this.widget.SetActive(true);
			this.toolTip.OnToolTip = new Func<string>(this.OnToolTip);
			this.fadeStage = ((!skip_fade) ? SimpleInfoScreen.StatusItemEntry.FadeStage.IN : SimpleInfoScreen.StatusItemEntry.FadeStage.WAIT);
			this.schedulerHandle = GameScheduler.Instance.SchedulePeriodic("StatusFader", 0f, new Action<object>(this.UpdateRoutine), null, null, 0f, null);
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.widgetRect);
			this.SetSize(this.widgetRect.rect.width, this.widgetRect.rect.height);
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
			return this.spacer.transform.GetSiblingIndex();
		}

		public void SetIndex(int index)
		{
			this.spacer.transform.SetSiblingIndex(index);
		}

		private void UpdateRoutine(object data)
		{
			switch (this.fadeStage)
			{
			case SimpleInfoScreen.StatusItemEntry.FadeStage.IN:
			{
				this.fade = Mathf.Min(this.fade + Time.deltaTime / this.fadeInTime, 1f);
				float num = this.fade;
				this.SetColor(num);
				this.SetSize(this.widgetRect.rect.width, this.widgetRect.rect.height);
				if (this.fade >= 1f)
				{
					this.fadeStage = SimpleInfoScreen.StatusItemEntry.FadeStage.WAIT;
				}
				break;
			}
			case SimpleInfoScreen.StatusItemEntry.FadeStage.WAIT:
				this.SetSize(this.widgetRect.rect.width, this.widgetRect.rect.height);
				break;
			case SimpleInfoScreen.StatusItemEntry.FadeStage.OUT:
			{
				float num2 = this.fade;
				this.SetColor(num2);
				this.SetSize(this.widgetRect.rect.width, this.widgetRect.rect.height);
				this.fade = Mathf.Max(this.fade - Time.deltaTime / this.fadeOutTime, 0f);
				if (this.fade <= 0f)
				{
					this.Destroy(true);
				}
				break;
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
			if (this.item.item.notificationType == NotificationType.BadMinor || this.item.item.notificationType == NotificationType.Bad)
			{
				Color color = new Color(SimpleInfoScreen.Instance.statusItemTextColor_bad.r, SimpleInfoScreen.Instance.statusItemTextColor_bad.g, SimpleInfoScreen.Instance.statusItemTextColor_bad.b, alpha);
				this.image.color = color;
				this.text.color = color;
			}
			else
			{
				Color color2 = new Color(SimpleInfoScreen.Instance.statusItemTextColor_regular.r, SimpleInfoScreen.Instance.statusItemTextColor_regular.g, SimpleInfoScreen.Instance.statusItemTextColor_regular.b, alpha);
				this.image.color = color2;
				this.text.color = color2;
			}
		}

		private void SetSize(float width, float height)
		{
			this.spacerLayout.minWidth = width;
			this.spacerLayout.minHeight = height;
		}

		public void Destroy(bool immediate)
		{
			if (immediate)
			{
				if (this.onDestroy != null)
				{
					this.onDestroy(this);
				}
				this.schedulerHandle.Clear();
				this.toolTip.OnToolTip = null;
				global::UnityEngine.Object.Destroy(this.spacer);
			}
			else
			{
				this.fade = 0.5f;
				this.fadeStage = SimpleInfoScreen.StatusItemEntry.FadeStage.OUT;
			}
		}

		public StatusItemGroup.Entry item;

		public StatusItemCategory category;

		private GameObject spacer;

		private LayoutElement spacerLayout;

		private GameObject widget;

		private RectTransform widgetRect;

		private ToolTip toolTip;

		private TextStyleSetting tooltipStyle;

		public Action<SimpleInfoScreen.StatusItemEntry> onDestroy;

		private Image image;

		private LocText text;

		private SimpleInfoScreen.StatusItemEntry.FadeStage fadeStage;

		private float fade;

		private SchedulerHandle schedulerHandle;

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
