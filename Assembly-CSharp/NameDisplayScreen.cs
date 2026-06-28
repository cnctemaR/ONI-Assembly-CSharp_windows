using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class NameDisplayScreen : KScreen
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		UIRegistry.nameDisplayScreen = this;
		foreach (Health health2 in Components.Health)
		{
			this.RegisterComponent(health2.gameObject, health2);
		}
		Components.Health.Register(delegate(Health health)
		{
			this.RegisterComponent(health.gameObject, health);
		}, null);
		foreach (Equipment equipment2 in Components.Equipment)
		{
			this.RegisterComponent(equipment2.gameObject, equipment2);
		}
		Components.Equipment.Register(delegate(Equipment equipment)
		{
			this.RegisterComponent(equipment.gameObject, equipment);
		}, null);
		foreach (SuffocationMonitor.Instance instance in Components.SuffocationMonitorInstance)
		{
			this.RegisterComponent(instance.gameObject, instance);
		}
		Components.SuffocationMonitorInstance.Register(delegate(SuffocationMonitor.Instance SuffocationMonitorInstance)
		{
			this.RegisterComponent(SuffocationMonitorInstance.gameObject, SuffocationMonitorInstance);
		}, null);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		NameDisplayScreen.Instance = this;
		this.dispayRootPrefab.SetActive(false);
	}

	public void AddNewEntry(GameObject representedObject)
	{
		NameDisplayScreen.Entry entry = new NameDisplayScreen.Entry();
		entry.world_go = representedObject;
		GameObject gameObject = Util.KInstantiateUI(this.dispayRootPrefab, base.gameObject, true);
		entry.display_go = gameObject;
		if (this.worldSpace)
		{
			entry.display_go.transform.localScale = Vector3.one * 0.01f;
		}
		gameObject.name = representedObject.name + " character overlay";
		KSelectable component = representedObject.GetComponent<KSelectable>();
		FactionAlignment component2 = representedObject.GetComponent<FactionAlignment>();
		if (component != null && component2 != null && (component2.Alignment == FactionManager.FactionID.Friendly || component2.Alignment == FactionManager.FactionID.Duplicant))
		{
			this.UpdateName(representedObject);
		}
		entry.Name = representedObject.name;
		this.entries.Add(entry);
	}

	public void RegisterComponent(GameObject representedObject, object Component)
	{
		NameDisplayScreen.Entry entry = this.GetEntry(representedObject);
		if (entry == null)
		{
			CharacterOverlay component = representedObject.GetComponent<CharacterOverlay>();
			if (!(component != null))
			{
				return;
			}
			component.Register();
			entry = this.GetEntry(representedObject);
		}
		if (entry == null)
		{
			return;
		}
		Transform transform = entry.display_go.transform.FindChild("Bars");
		entry.bars_go = transform.gameObject;
		if (representedObject.GetComponent<MinionBrain>() == null)
		{
			Transform transform2 = entry.display_go.transform.FindChild("Name");
			transform2.gameObject.SetActive(false);
		}
		else
		{
			this.UpdateName(representedObject);
		}
		if (Component is Health)
		{
			Health health = (Health)Component;
			GameObject gameObject = Util.KInstantiateUI(ProgressBarsConfig.Instance.healthBarPrefab, transform.gameObject, false);
			gameObject.name = "Health Bar";
			health.healthBar = gameObject.GetComponent<HealthBar>();
			health.healthBar.GetComponent<KSelectable>().entityName = UI.METERS.HEALTH.TOOLTIP;
			entry.healthBar = health.healthBar;
			gameObject.transform.FindChild("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("HealthBar");
		}
		else if (Component is SuffocationMonitor.Instance)
		{
			GameObject gameObject2 = Util.KInstantiateUI(ProgressBarsConfig.Instance.progressBarUIPrefab, transform.gameObject, false);
			entry.breathBar = gameObject2.GetComponent<ProgressBar>();
			gameObject2.gameObject.GetComponent<ToolTip>().AddMultiStringTooltip("Breath", this.ToolTipStyle_Property);
			gameObject2.name = "Breath Bar";
			gameObject2.transform.FindChild("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("BreathBar");
			gameObject2.GetComponent<KSelectable>().entityName = UI.METERS.BREATH.TOOLTIP;
		}
		else if (Component is Equipment)
		{
			GameObject gameObject3 = Util.KInstantiateUI(ProgressBarsConfig.Instance.progressBarUIPrefab, transform.gameObject, false);
			entry.suitBar = gameObject3.GetComponent<ProgressBar>();
			gameObject3.name = "Suit Tank Bar";
			gameObject3.transform.FindChild("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("OxygenTankBar");
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
				Vector3 vector = this.entries[i].world_go.transform.position;
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
		entry.healthBar.gameObject.SetActive(bVisible);
		entry.healthBar.SetUpdateFunc(updatePercentFull);
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

	public void SetSuitTankDisplayState(bool state, GameObject minion_go)
	{
		NameDisplayScreen.Entry entry = this.GetEntry(minion_go);
		entry.suitBar.gameObject.SetActive(state);
	}

	private NameDisplayScreen.Entry GetEntry(GameObject worldObject)
	{
		return this.entries.Find((NameDisplayScreen.Entry entry) => entry.world_go == worldObject);
	}

	[SerializeField]
	private float HideDistance;

	public static NameDisplayScreen Instance;

	public GameObject dispayRootPrefab;

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
	}
}
