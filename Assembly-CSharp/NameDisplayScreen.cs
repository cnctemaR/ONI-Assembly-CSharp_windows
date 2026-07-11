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

	protected override void OnSpawn()
	{
		base.OnSpawn();
		UIRegistry.nameDisplayScreen = this;
		foreach (Health health2 in Components.Health.Items)
		{
			this.RegisterComponent(health2.gameObject, health2, true);
		}
		Components.Health.Register(delegate(Health health)
		{
			this.RegisterComponent(health.gameObject, health);
		}, null);
		foreach (Equipment equipment2 in Components.Equipment.Items)
		{
			this.RegisterComponent(equipment2.gameObject, equipment2, false);
		}
		Components.Equipment.Register(delegate(Equipment equipment)
		{
			this.RegisterComponent(equipment.gameObject, equipment);
		}, null);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		NameDisplayScreen.Instance = this;
	}

	private bool ShouldShowName(GameObject representedObject)
	{
		return representedObject.GetComponent<MinionBrain>() != null;
	}

	public void AddNewEntry(GameObject representedObject)
	{
		NameDisplayScreen.Entry entry = new NameDisplayScreen.Entry();
		entry.world_go = representedObject;
		bool flag = this.ShouldShowName(representedObject);
		GameObject gameObject = ((!flag) ? this.barsPrefab : this.nameAndBarsPrefab);
		GameObject gameObject2 = Util.KInstantiateUI(gameObject, base.gameObject, true);
		entry.display_go = gameObject2;
		if (this.worldSpace)
		{
			entry.display_go.transform.localScale = Vector3.one * 0.01f;
		}
		gameObject2.name = representedObject.name + " character overlay";
		KSelectable component = representedObject.GetComponent<KSelectable>();
		FactionAlignment component2 = representedObject.GetComponent<FactionAlignment>();
		if (component != null && component2 != null && (component2.Alignment == FactionManager.FactionID.Friendly || component2.Alignment == FactionManager.FactionID.Duplicant))
		{
			this.UpdateName(representedObject);
		}
		entry.Name = representedObject.name;
		entry.refs = gameObject2.GetComponent<HierarchyReferences>();
		this.entries.Add(entry);
	}

	public void RegisterComponent(GameObject representedObject, object component)
	{
		this.RegisterComponent(representedObject, component, false);
	}

	public void RegisterComponent(GameObject representedObject, object component, bool force_new_entry)
	{
		NameDisplayScreen.Entry entry = ((!force_new_entry) ? this.GetEntry(representedObject) : null);
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
			Health health = (Health)component;
			GameObject gameObject = Util.KInstantiateUI(ProgressBarsConfig.Instance.healthBarPrefab, reference.gameObject, false);
			gameObject.name = "Health Bar";
			health.healthBar = gameObject.GetComponent<HealthBar>();
			health.healthBar.GetComponent<KSelectable>().entityName = UI.METERS.HEALTH.TOOLTIP;
			health.healthBar.GetComponent<KSelectableHealthBar>().IsSelectable = representedObject.GetComponent<MinionBrain>() != null;
			entry.healthBar = health.healthBar;
			gameObject.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("HealthBar");
		}
		else if (component is OxygenBreather)
		{
			GameObject gameObject2 = Util.KInstantiateUI(ProgressBarsConfig.Instance.progressBarUIPrefab, reference.gameObject, false);
			entry.breathBar = gameObject2.GetComponent<ProgressBar>();
			gameObject2.gameObject.GetComponent<ToolTip>().AddMultiStringTooltip("Breath", this.ToolTipStyle_Property);
			gameObject2.name = "Breath Bar";
			gameObject2.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("BreathBar");
			gameObject2.GetComponent<KSelectable>().entityName = UI.METERS.BREATH.TOOLTIP;
		}
		else if (component is Equipment)
		{
			GameObject gameObject3 = Util.KInstantiateUI(ProgressBarsConfig.Instance.progressBarUIPrefab, reference.gameObject, false);
			entry.suitBar = gameObject3.GetComponent<ProgressBar>();
			gameObject3.name = "Suit Tank Bar";
			gameObject3.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("OxygenTankBar");
			gameObject3.GetComponent<KSelectable>().entityName = UI.METERS.BREATH.TOOLTIP;
		}
	}

	private void LateUpdate()
	{
		if (App.isLoading || App.IsExiting)
		{
			return;
		}
		SimViewMode simViewMode = SimViewMode.None;
		if (OverlayScreen.Instance != null)
		{
			simViewMode = OverlayScreen.Instance.GetMode();
		}
		bool flag = !(Camera.main == null) && Camera.main.orthographicSize < this.HideDistance && simViewMode == SimViewMode.None;
		int num = this.entries.Count;
		int i = 0;
		while (i < num)
		{
			if (this.entries[i].world_go != null)
			{
				Vector3 vector = this.entries[i].world_go.transform.GetPosition();
				if (flag && CameraController.Instance.IsVisiblePos(vector))
				{
					RectTransform component = this.entries[i].display_go.GetComponent<RectTransform>();
					if (CameraController.Instance != null && CameraController.Instance.followTarget == this.entries[i].world_go.transform)
					{
						vector = CameraController.Instance.followTargetPos;
					}
					else
					{
						KAnimControllerBase component2 = this.entries[i].world_go.GetComponent<KAnimControllerBase>();
						if (component2 != null)
						{
							vector = component2.GetWorldPivot();
						}
					}
					component.anchoredPosition = ((!this.worldSpace) ? base.WorldToScreen(vector) : vector);
					this.entries[i].display_go.SetActive(true);
				}
				else if (this.entries[i].display_go.activeSelf)
				{
					this.entries[i].display_go.SetActive(false);
				}
				if (this.entries[i].world_go.HasTag(GameTags.Dead))
				{
					this.entries[i].bars_go.SetActive(false);
				}
				if (this.entries[i].bars_go != null)
				{
					GameObject bars_go = this.entries[i].bars_go;
					bars_go.GetComponentsInChildren<KCollider2D>(false, this.workingList);
					foreach (KCollider2D kcollider2D in this.workingList)
					{
						kcollider2D.MarkDirty(false);
					}
				}
				i++;
			}
			else
			{
				global::UnityEngine.Object.Destroy(this.entries[i].display_go);
				num--;
				this.entries[i] = this.entries[num];
			}
		}
		this.entries.RemoveRange(num, this.entries.Count - num);
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
		}
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

	public bool worldSpace = true;

	private List<KCollider2D> workingList = new List<KCollider2D>();

	[Serializable]
	public class Entry
	{
		public string Name;

		public GameObject world_go;

		public GameObject display_go;

		public GameObject bars_go;

		public HealthBar healthBar;

		public ProgressBar breathBar;

		public ProgressBar suitBar;

		public HierarchyReferences refs;
	}
}
