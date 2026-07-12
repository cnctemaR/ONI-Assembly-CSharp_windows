using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class NameDisplayScreen : KScreen
{
	public static void DestroyInstance()
	{
		NameDisplayScreen.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		NameDisplayScreen.Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		UIRegistry.nameDisplayScreen = this;
		Components.Health.Register(new Action<Health>(this.OnHealthAdded), null);
		Components.Equipment.Register(new Action<Equipment>(this.OnEquipmentAdded), null);
		this.updateSectionIndex = 0;
		this.lateUpdateSections = new List<global::System.Action>
		{
			new global::System.Action(this.LateUpdatePart0),
			new global::System.Action(this.LateUpdatePart1),
			new global::System.Action(this.LateUpdatePart2)
		};
		this.bindOnOverlayChange();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this.isOverlayChangeBound && OverlayScreen.Instance != null)
		{
			OverlayScreen instance = OverlayScreen.Instance;
			instance.OnOverlayChanged = (Action<HashedString>)Delegate.Remove(instance.OnOverlayChanged, new Action<HashedString>(this.OnOverlayChanged));
			this.isOverlayChangeBound = false;
		}
	}

	private void bindOnOverlayChange()
	{
		if (this.isOverlayChangeBound)
		{
			return;
		}
		if (OverlayScreen.Instance != null)
		{
			OverlayScreen instance = OverlayScreen.Instance;
			instance.OnOverlayChanged = (Action<HashedString>)Delegate.Combine(instance.OnOverlayChanged, new Action<HashedString>(this.OnOverlayChanged));
			this.isOverlayChangeBound = true;
		}
	}

	private void OnOverlayChanged(HashedString new_mode)
	{
		HashedString hashedString = this.lastKnownOverlayID;
		this.lastKnownOverlayID = new_mode;
		if (hashedString != OverlayModes.None.ID && new_mode != OverlayModes.None.ID)
		{
			return;
		}
		bool flag = new_mode == OverlayModes.None.ID;
		int count = this.entries.Count;
		for (int i = 0; i < count; i++)
		{
			NameDisplayScreen.Entry entry = this.entries[i];
			if (!(entry.world_go == null))
			{
				entry.display_go.SetActive(flag);
			}
		}
	}

	private void OnHealthAdded(Health health)
	{
		this.RegisterComponent(health.gameObject, health, false);
	}

	private void OnEquipmentAdded(Equipment equipment)
	{
		MinionAssignablesProxy component = equipment.GetComponent<MinionAssignablesProxy>();
		GameObject targetGameObject = component.GetTargetGameObject();
		if (targetGameObject)
		{
			this.RegisterComponent(targetGameObject, equipment, false);
			return;
		}
		global::Debug.LogWarningFormat("OnEquipmentAdded proxy target {0} was null.", new object[] { component.TargetInstanceID });
	}

	private bool ShouldShowName(GameObject representedObject)
	{
		CharacterOverlay component = representedObject.GetComponent<CharacterOverlay>();
		return component != null && component.shouldShowName;
	}

	public Guid AddWorldText(string initialText, GameObject prefab)
	{
		NameDisplayScreen.TextEntry textEntry = new NameDisplayScreen.TextEntry();
		textEntry.guid = Guid.NewGuid();
		textEntry.display_go = Util.KInstantiateUI(prefab, base.gameObject, true);
		textEntry.display_go.GetComponentInChildren<LocText>().text = initialText;
		this.textEntries.Add(textEntry);
		return textEntry.guid;
	}

	public GameObject GetWorldText(Guid guid)
	{
		GameObject gameObject = null;
		foreach (NameDisplayScreen.TextEntry textEntry in this.textEntries)
		{
			if (textEntry.guid == guid)
			{
				gameObject = textEntry.display_go;
				break;
			}
		}
		return gameObject;
	}

	public void RemoveWorldText(Guid guid)
	{
		int num = -1;
		for (int i = 0; i < this.textEntries.Count; i++)
		{
			if (this.textEntries[i].guid == guid)
			{
				num = i;
				break;
			}
		}
		if (num >= 0)
		{
			global::UnityEngine.Object.Destroy(this.textEntries[num].display_go);
			this.textEntries.RemoveAt(num);
		}
	}

	public void AddNewEntry(GameObject representedObject)
	{
		NameDisplayScreen.Entry entry = new NameDisplayScreen.Entry();
		entry.world_go = representedObject;
		entry.world_go_anim_controller = representedObject.GetComponent<KAnimControllerBase>();
		GameObject gameObject = Util.KInstantiateUI(this.ShouldShowName(representedObject) ? this.nameAndBarsPrefab : this.barsPrefab, base.gameObject, true);
		entry.display_go = gameObject;
		entry.display_go_rect = gameObject.GetComponent<RectTransform>();
		if (this.worldSpace)
		{
			entry.display_go.transform.localScale = Vector3.one * 0.01f;
		}
		gameObject.name = representedObject.name + " character overlay";
		entry.Name = representedObject.name;
		entry.refs = gameObject.GetComponent<HierarchyReferences>();
		this.entries.Add(entry);
		global::UnityEngine.Object component = representedObject.GetComponent<KSelectable>();
		FactionAlignment component2 = representedObject.GetComponent<FactionAlignment>();
		if (component != null)
		{
			if (component2 != null)
			{
				if (component2.Alignment == FactionManager.FactionID.Friendly || component2.Alignment == FactionManager.FactionID.Duplicant)
				{
					this.UpdateName(representedObject);
					return;
				}
			}
			else
			{
				this.UpdateName(representedObject);
			}
		}
	}

	public void RegisterComponent(GameObject representedObject, object component, bool force_new_entry = false)
	{
		NameDisplayScreen.Entry entry = (force_new_entry ? null : this.GetEntry(representedObject));
		if (entry == null)
		{
			CharacterOverlay component2 = representedObject.GetComponent<CharacterOverlay>();
			if (component2 != null)
			{
				component2.Register();
				entry = this.GetEntry(representedObject);
			}
		}
		if (entry == null)
		{
			return;
		}
		Transform reference = entry.refs.GetReference<Transform>("Bars");
		entry.bars_go = reference.gameObject;
		if (component is Health)
		{
			if (!entry.healthBar)
			{
				Health health = (Health)component;
				GameObject gameObject = Util.KInstantiateUI(ProgressBarsConfig.Instance.healthBarPrefab, reference.gameObject, false);
				gameObject.name = "Health Bar";
				health.healthBar = gameObject.GetComponent<HealthBar>();
				health.healthBar.GetComponent<KSelectable>().entityName = UI.METERS.HEALTH.TOOLTIP;
				health.healthBar.GetComponent<KSelectableHealthBar>().IsSelectable = representedObject.GetComponent<MinionBrain>() != null;
				entry.healthBar = health.healthBar;
				entry.healthBar.autoHide = false;
				gameObject.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("HealthBar");
				return;
			}
			global::Debug.LogWarningFormat("Health added twice {0}", new object[] { component });
			return;
		}
		else if (component is OxygenBreather)
		{
			if (!entry.breathBar)
			{
				GameObject gameObject2 = Util.KInstantiateUI(ProgressBarsConfig.Instance.progressBarUIPrefab, reference.gameObject, false);
				entry.breathBar = gameObject2.GetComponent<ProgressBar>();
				entry.breathBar.autoHide = false;
				gameObject2.gameObject.GetComponent<ToolTip>().AddMultiStringTooltip("Breath", this.ToolTipStyle_Property);
				gameObject2.name = "Breath Bar";
				gameObject2.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("BreathBar");
				gameObject2.GetComponent<KSelectable>().entityName = UI.METERS.BREATH.TOOLTIP;
				return;
			}
			global::Debug.LogWarningFormat("OxygenBreather added twice {0}", new object[] { component });
			return;
		}
		else if (component is Equipment)
		{
			if (!entry.suitBar)
			{
				GameObject gameObject3 = Util.KInstantiateUI(ProgressBarsConfig.Instance.progressBarUIPrefab, reference.gameObject, false);
				entry.suitBar = gameObject3.GetComponent<ProgressBar>();
				entry.suitBar.autoHide = false;
				gameObject3.name = "Suit Tank Bar";
				gameObject3.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("OxygenTankBar");
				gameObject3.GetComponent<KSelectable>().entityName = UI.METERS.BREATH.TOOLTIP;
			}
			else
			{
				global::Debug.LogWarningFormat("SuitBar added twice {0}", new object[] { component });
			}
			if (!entry.suitFuelBar)
			{
				GameObject gameObject4 = Util.KInstantiateUI(ProgressBarsConfig.Instance.progressBarUIPrefab, reference.gameObject, false);
				entry.suitFuelBar = gameObject4.GetComponent<ProgressBar>();
				entry.suitFuelBar.autoHide = false;
				gameObject4.name = "Suit Fuel Bar";
				gameObject4.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("FuelTankBar");
				gameObject4.GetComponent<KSelectable>().entityName = UI.METERS.FUEL.TOOLTIP;
			}
			else
			{
				global::Debug.LogWarningFormat("FuelBar added twice {0}", new object[] { component });
			}
			if (!entry.suitBatteryBar)
			{
				GameObject gameObject5 = Util.KInstantiateUI(ProgressBarsConfig.Instance.progressBarUIPrefab, reference.gameObject, false);
				entry.suitBatteryBar = gameObject5.GetComponent<ProgressBar>();
				entry.suitBatteryBar.autoHide = false;
				gameObject5.name = "Suit Battery Bar";
				gameObject5.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("BatteryBar");
				gameObject5.GetComponent<KSelectable>().entityName = UI.METERS.BATTERY.TOOLTIP;
				return;
			}
			global::Debug.LogWarningFormat("CoolantBar added twice {0}", new object[] { component });
			return;
		}
		else
		{
			if (!(component is ThoughtGraph.Instance))
			{
				if (component is GameplayEventMonitor.Instance)
				{
					if (!entry.gameplayEventDisplay)
					{
						GameObject gameObject6 = Util.KInstantiateUI(EffectPrefabs.Instance.GameplayEventDisplay, entry.display_go, false);
						entry.gameplayEventDisplay = gameObject6.GetComponent<HierarchyReferences>();
						gameObject6.name = "Gameplay Event Display";
						return;
					}
					global::Debug.LogWarningFormat("GameplayEventDisplay added twice {0}", new object[] { component });
				}
				return;
			}
			if (!entry.thoughtBubble)
			{
				GameObject gameObject7 = Util.KInstantiateUI(EffectPrefabs.Instance.ThoughtBubble, entry.display_go, false);
				entry.thoughtBubble = gameObject7.GetComponent<HierarchyReferences>();
				gameObject7.name = "Thought Bubble";
				GameObject gameObject8 = Util.KInstantiateUI(EffectPrefabs.Instance.ThoughtBubbleConvo, entry.display_go, false);
				entry.thoughtBubbleConvo = gameObject8.GetComponent<HierarchyReferences>();
				gameObject8.name = "Thought Bubble Convo";
				return;
			}
			global::Debug.LogWarningFormat("ThoughtGraph added twice {0}", new object[] { component });
			return;
		}
	}

	private void LateUpdate()
	{
		if (App.isLoading || App.IsExiting)
		{
			return;
		}
		this.bindOnOverlayChange();
		Camera main = Camera.main;
		if (main == null)
		{
			return;
		}
		if (this.lastKnownOverlayID != OverlayModes.None.ID)
		{
			return;
		}
		int count = this.entries.Count;
		this.LateUpdatePos(main.orthographicSize < this.HideDistance);
		this.lateUpdateSections[this.updateSectionIndex]();
		this.updateSectionIndex = (this.updateSectionIndex + 1) % this.lateUpdateSections.Count;
	}

	private void LateUpdatePos(bool visibleToZoom)
	{
		CameraController instance = CameraController.Instance;
		Transform followTarget = instance.followTarget;
		int count = this.entries.Count;
		for (int i = 0; i < count; i++)
		{
			NameDisplayScreen.Entry entry = this.entries[i];
			GameObject world_go = entry.world_go;
			if (!(world_go == null))
			{
				Vector3 vector = world_go.transform.GetPosition();
				if (visibleToZoom && CameraController.Instance.IsVisiblePos(vector))
				{
					if (instance != null && followTarget == world_go.transform)
					{
						vector = instance.followTargetPos;
					}
					else if (entry.world_go_anim_controller != null)
					{
						vector = entry.world_go_anim_controller.GetWorldPivot();
					}
					entry.display_go_rect.anchoredPosition = (this.worldSpace ? vector : base.WorldToScreen(vector));
					entry.display_go.SetActive(true);
				}
				else if (entry.display_go.activeSelf)
				{
					entry.display_go.SetActive(false);
				}
			}
		}
	}

	private void LateUpdatePart0()
	{
		int num = this.entries.Count;
		int i = 0;
		while (i < num)
		{
			if (this.entries[i].world_go == null)
			{
				global::UnityEngine.Object.Destroy(this.entries[i].display_go);
				num--;
				this.entries[i] = this.entries[num];
			}
			else
			{
				i++;
			}
		}
		this.entries.RemoveRange(num, this.entries.Count - num);
	}

	private void LateUpdatePart1()
	{
		int count = this.entries.Count;
		for (int i = 0; i < count; i++)
		{
			if (!(this.entries[i].world_go == null) && this.entries[i].world_go.HasTag(GameTags.Dead))
			{
				this.entries[i].bars_go.SetActive(false);
			}
		}
	}

	private void LateUpdatePart2()
	{
		int count = this.entries.Count;
		for (int i = 0; i < count; i++)
		{
			if (this.entries[i].bars_go != null)
			{
				this.entries[i].bars_go.GetComponentsInChildren<KCollider2D>(false, this.workingList);
				foreach (KCollider2D kcollider2D in this.workingList)
				{
					kcollider2D.MarkDirty(false);
				}
			}
		}
	}

	public void UpdateName(GameObject representedObject)
	{
		NameDisplayScreen.Entry entry = this.GetEntry(representedObject);
		if (entry == null)
		{
			return;
		}
		KSelectable component = representedObject.GetComponent<KSelectable>();
		entry.display_go.name = component.GetProperName() + " character overlay";
		LocText componentInChildren = entry.display_go.GetComponentInChildren<LocText>();
		if (componentInChildren != null)
		{
			componentInChildren.text = component.GetProperName();
			if (representedObject.GetComponent<RocketModule>() != null)
			{
				componentInChildren.text = representedObject.GetComponent<RocketModule>().GetParentRocketName();
			}
		}
	}

	public void SetThoughtBubbleDisplay(GameObject minion_go, bool bVisible, string hover_text, Sprite bubble_sprite, Sprite topic_sprite)
	{
		NameDisplayScreen.Entry entry = this.GetEntry(minion_go);
		if (entry == null || entry.thoughtBubble == null)
		{
			return;
		}
		this.ApplyThoughtSprite(entry.thoughtBubble, bubble_sprite, "bubble_sprite");
		this.ApplyThoughtSprite(entry.thoughtBubble, topic_sprite, "icon_sprite");
		entry.thoughtBubble.GetComponent<KSelectable>().entityName = hover_text;
		entry.thoughtBubble.gameObject.SetActive(bVisible);
	}

	public void SetThoughtBubbleConvoDisplay(GameObject minion_go, bool bVisible, string hover_text, Sprite bubble_sprite, Sprite topic_sprite, Sprite mode_sprite)
	{
		NameDisplayScreen.Entry entry = this.GetEntry(minion_go);
		if (entry == null || entry.thoughtBubble == null)
		{
			return;
		}
		this.ApplyThoughtSprite(entry.thoughtBubbleConvo, bubble_sprite, "bubble_sprite");
		this.ApplyThoughtSprite(entry.thoughtBubbleConvo, topic_sprite, "icon_sprite");
		this.ApplyThoughtSprite(entry.thoughtBubbleConvo, mode_sprite, "icon_sprite_mode");
		entry.thoughtBubbleConvo.GetComponent<KSelectable>().entityName = hover_text;
		entry.thoughtBubbleConvo.gameObject.SetActive(bVisible);
	}

	private void ApplyThoughtSprite(HierarchyReferences active_bubble, Sprite sprite, string target)
	{
		active_bubble.GetReference<Image>(target).sprite = sprite;
	}

	public void SetGameplayEventDisplay(GameObject minion_go, bool bVisible, string hover_text, Sprite sprite)
	{
		NameDisplayScreen.Entry entry = this.GetEntry(minion_go);
		if (entry == null || entry.gameplayEventDisplay == null)
		{
			return;
		}
		entry.gameplayEventDisplay.GetReference<Image>("icon_sprite").sprite = sprite;
		entry.gameplayEventDisplay.GetComponent<KSelectable>().entityName = hover_text;
		entry.gameplayEventDisplay.gameObject.SetActive(bVisible);
	}

	public void SetBreathDisplay(GameObject minion_go, Func<float> updatePercentFull, bool bVisible)
	{
		NameDisplayScreen.Entry entry = this.GetEntry(minion_go);
		if (entry == null || entry.breathBar == null)
		{
			return;
		}
		entry.breathBar.SetUpdateFunc(updatePercentFull);
		entry.breathBar.gameObject.SetActive(bVisible);
	}

	public void SetHealthDisplay(GameObject minion_go, Func<float> updatePercentFull, bool bVisible)
	{
		NameDisplayScreen.Entry entry = this.GetEntry(minion_go);
		if (entry == null || entry.healthBar == null)
		{
			return;
		}
		entry.healthBar.OnChange();
		entry.healthBar.SetUpdateFunc(updatePercentFull);
		if (entry.healthBar.gameObject.activeSelf != bVisible)
		{
			entry.healthBar.gameObject.SetActive(bVisible);
		}
	}

	public void SetSuitTankDisplay(GameObject minion_go, Func<float> updatePercentFull, bool bVisible)
	{
		NameDisplayScreen.Entry entry = this.GetEntry(minion_go);
		if (entry == null || entry.suitBar == null)
		{
			return;
		}
		entry.suitBar.SetUpdateFunc(updatePercentFull);
		entry.suitBar.gameObject.SetActive(bVisible);
	}

	public void SetSuitFuelDisplay(GameObject minion_go, Func<float> updatePercentFull, bool bVisible)
	{
		NameDisplayScreen.Entry entry = this.GetEntry(minion_go);
		if (entry == null || entry.suitFuelBar == null)
		{
			return;
		}
		entry.suitFuelBar.SetUpdateFunc(updatePercentFull);
		entry.suitFuelBar.gameObject.SetActive(bVisible);
	}

	public void SetSuitBatteryDisplay(GameObject minion_go, Func<float> updatePercentFull, bool bVisible)
	{
		NameDisplayScreen.Entry entry = this.GetEntry(minion_go);
		if (entry == null || entry.suitBatteryBar == null)
		{
			return;
		}
		entry.suitBatteryBar.SetUpdateFunc(updatePercentFull);
		entry.suitBatteryBar.gameObject.SetActive(bVisible);
	}

	private NameDisplayScreen.Entry GetEntry(GameObject worldObject)
	{
		return this.entries.Find((NameDisplayScreen.Entry entry) => entry.world_go == worldObject);
	}

	[SerializeField]
	private float HideDistance;

	public static NameDisplayScreen Instance;

	public GameObject nameAndBarsPrefab;

	public GameObject barsPrefab;

	public TextStyleSetting ToolTipStyle_Property;

	[SerializeField]
	private Color selectedColor;

	[SerializeField]
	private Color defaultColor;

	public int fontsize_min = 14;

	public int fontsize_max = 32;

	public float cameraDistance_fontsize_min = 6f;

	public float cameraDistance_fontsize_max = 4f;

	public List<NameDisplayScreen.Entry> entries = new List<NameDisplayScreen.Entry>();

	public List<NameDisplayScreen.TextEntry> textEntries = new List<NameDisplayScreen.TextEntry>();

	public bool worldSpace = true;

	private int updateSectionIndex;

	private List<global::System.Action> lateUpdateSections = new List<global::System.Action>();

	private bool isOverlayChangeBound;

	private HashedString lastKnownOverlayID = OverlayModes.None.ID;

	private List<KCollider2D> workingList = new List<KCollider2D>();

	[Serializable]
	public class Entry
	{
		public string Name;

		public GameObject world_go;

		public GameObject display_go;

		public GameObject bars_go;

		public KAnimControllerBase world_go_anim_controller;

		public RectTransform display_go_rect;

		public HealthBar healthBar;

		public ProgressBar breathBar;

		public ProgressBar suitBar;

		public ProgressBar suitFuelBar;

		public ProgressBar suitBatteryBar;

		public HierarchyReferences thoughtBubble;

		public HierarchyReferences thoughtBubbleConvo;

		public HierarchyReferences gameplayEventDisplay;

		public HierarchyReferences refs;
	}

	public class TextEntry
	{
		public Guid guid;

		public GameObject display_go;
	}
}
