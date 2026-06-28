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
		this.vitalsPanel.HeaderLabel.text = UI.DETAILTABS.SIMPLEINFO.GROUPNAME_NEEDS;
		this.vitalsContainer = Util.KInstantiateUI(this.VitalsPanelTemplate, this.vitalsPanel.Content.gameObject, false).GetComponent<MinionVitalsPanel>();
		this.researchContainer = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.researchContainer.HeaderLabel.text = UI.DETAILTABS.SIMPLEINFO.GROUPNAME_RESEARCH;
		GameObject gameObject = Util.KInstantiateUI(this.TextContainerPrefab, this.researchContainer.Content.gameObject, true);
		this.researchPanel = gameObject.GetComponent<LocText>();
		this.infoPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.infoPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.SIMPLEINFO.GROUPNAME_DESCRIPTION;
		GameObject gameObject2 = this.infoPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject;
		this.descriptionContainer = Util.KInstantiateUI<DescriptionContainer>(this.DescriptionContainerTemplate, gameObject2, false);
		this.descriptionContainer.attributePrefab.gameObject.SetActive(false);
		this.attributeContainer = this.descriptionContainer.attributePrefab.transform.parent.gameObject;
		this.stampContainer = Util.KInstantiateUI(this.StampContainerTemplate, gameObject2, false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
	}

	public override void OnSelectTarget(GameObject target)
	{
		base.OnSelectTarget(target);
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
				statusItemGroup3.OnRemoveStatusItem = (Action<StatusItemGroup.Entry>)Delegate.Combine(statusItemGroup3.OnRemoveStatusItem, new Action<StatusItemGroup.Entry>(this.OnRemoveStatusItem));
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
		KSelectable component = target.GetComponent<KSelectable>();
		if (component != null)
		{
			StatusItemGroup statusItemGroup = component.GetStatusItemGroup();
			if (statusItemGroup != null)
			{
				StatusItemGroup statusItemGroup2 = statusItemGroup;
				statusItemGroup2.OnAddStatusItem = (Action<StatusItemGroup.Entry, StatusItemCategory>)Delegate.Remove(statusItemGroup2.OnAddStatusItem, new Action<StatusItemGroup.Entry, StatusItemCategory>(this.OnAddStatusItem));
				StatusItemGroup statusItemGroup3 = statusItemGroup;
				statusItemGroup3.OnRemoveStatusItem = (Action<StatusItemGroup.Entry>)Delegate.Remove(statusItemGroup3.OnRemoveStatusItem, new Action<StatusItemGroup.Entry>(this.OnRemoveStatusItem));
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

	private void OnRemoveStatusItem(StatusItemGroup.Entry status_item)
	{
		this.DoRemoveStatusItem(status_item, false);
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
		for (int i = 0; i < count; i++)
		{
			this.statusItems[i].Refresh();
		}
		if (this.vitalsContainer.isActiveAndEnabled)
		{
			this.vitalsContainer.Refresh(null);
		}
		if (this.researchContainer.isActiveAndEnabled)
		{
			this.RefreshResearchContainer(this.mTarget);
		}
	}

	private void SetTitle(GameObject target)
	{
		if (DetailsScreen.Instance != null)
		{
			DetailsScreen.Instance.SetTitle(this.mTarget.gameObject.GetProperName());
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
		this.attributeContainer.SetActive(false);
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
			this.ShowAttributes(target);
		}
		else if (component4 != null)
		{
			text = component4.Def.Effect;
			text2 = component4.Def.Desc;
		}
		else if (component7 != null)
		{
			EdiblesManager.FoodInfo foodInfo = component7.FoodInfo;
			text += string.Format(UI.GAMEOBJECTEFFECTS.CALORIES, GameUtil.GetFormattedCalories((float)foodInfo.Rations * 100000f, GameUtil.TimeSlice.None, true));
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
		else
		{
			text += GameUtil.GetGameObjectEffectsString(target);
		}
		this.RefreshResearchContainer(target);
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

	private void RefreshResearchContainer(GameObject target)
	{
		ResearchCenter component = target.GetComponent<ResearchCenter>();
		if (component != null)
		{
			this.researchContainer.gameObject.SetActive(true);
			this.researchPanel.enabled = true;
			string statusString = component.GetStatusString();
			if (this.researchPanel.text != statusString)
			{
				this.researchPanel.text = statusString;
			}
		}
		else
		{
			this.researchContainer.gameObject.SetActive(false);
		}
	}

	private void ShowAttributes(GameObject target)
	{
		this.attributeContainer.SetActive(true);
		Attributes attributes = target.GetAttributes();
		if (attributes != null && attributes.Count > 0)
		{
			foreach (AttributeInstance attributeInstance in attributes)
			{
				LocText locText = Util.KInstantiateUI<LocText>(this.descriptionContainer.attributePrefab.gameObject, this.attributeContainer, false);
				locText.transform.localScale = new Vector3(1f, 1f, 1f);
				locText.name = attributeInstance.Id;
				locText.text = attributeInstance.Name + " " + attributeInstance.GetTotalValue();
				locText.gameObject.SetActive(true);
				AttributeInstance attributeInstance2 = attributeInstance;
				locText.GetComponent<ToolTip>().SetSimpleTooltip(attributeInstance2.GetAttributeValueTooltip());
				this.attributeLabels.Add(locText);
			}
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

	public GameObject DescriptionContainerTemplate;

	private DescriptionContainer descriptionContainer;

	private GameObject attributeContainer;

	public GameObject StampContainerTemplate;

	public GameObject StampPrefab;

	public GameObject VitalsPanelTemplate;

	public Sprite DefaultPortraitIcon;

	public Text StatusPanelCurrentActionLabel;

	public GameObject StatusItemPrefab;

	public Sprite statusWarningIcon;

	private CollapsibleDetailContentPanel statusItemPanel;

	private CollapsibleDetailContentPanel vitalsPanel;

	private CollapsibleDetailContentPanel researchContainer;

	private LocText researchPanel;

	private GameObject infoPanel;

	private GameObject stampContainer;

	private MinionVitalsPanel vitalsContainer;

	private GameObject InfoFolder;

	private GameObject statusItemsFolder;

	public GameObject TextContainerPrefab;

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
			this.schedulerHandle = GameScheduler.Instance.SchedulePeriodic("StatusFader", 0f, new Action<object>(this.UpdateRoutine), null, null, 0f);
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
