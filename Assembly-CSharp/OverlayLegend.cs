using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class OverlayLegend : KScreen
{
	[ContextMenu("Set all fonts color")]
	public void SetAllFontsColor()
	{
		foreach (OverlayLegend.OverlayInfo overlayInfo in this.overlayInfoList)
		{
			for (int i = 0; i < overlayInfo.infoUnits.Count; i++)
			{
				if (overlayInfo.infoUnits[i].fontColor == Color.clear)
				{
					overlayInfo.infoUnits[i].fontColor = Color.white;
				}
			}
		}
	}

	[ContextMenu("Set all tooltips")]
	public void SetAllTooltips()
	{
		foreach (OverlayLegend.OverlayInfo overlayInfo in this.overlayInfoList)
		{
			string text = overlayInfo.name;
			text = text.Replace("NAME", string.Empty);
			for (int i = 0; i < overlayInfo.infoUnits.Count; i++)
			{
				string text2 = overlayInfo.infoUnits[i].description;
				text2 = text2.Replace(text, string.Empty);
				text2 = text + "TOOLTIPS." + text2;
				overlayInfo.infoUnits[i].tooltip = text2;
			}
		}
	}

	[ContextMenu("Set Sliced for empty icons")]
	public void SetSlicedForEmptyIcons()
	{
		foreach (OverlayLegend.OverlayInfo overlayInfo in this.overlayInfoList)
		{
			for (int i = 0; i < overlayInfo.infoUnits.Count; i++)
			{
				if (overlayInfo.infoUnits[i].icon == this.emptySprite)
				{
					overlayInfo.infoUnits[i].sliceIcon = true;
				}
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (OverlayLegend.Instance == null)
		{
			OverlayLegend.Instance = this;
			this.activeUnitObjs = new List<GameObject>();
			this.inactiveUnitObjs = new List<GameObject>();
			RegionManager regionManager = Game.Instance.RegionManager;
			regionManager.OnRegionChanged = (global::System.Action)Delegate.Combine(regionManager.OnRegionChanged, new global::System.Action(this.OnRegionChanged));
			foreach (OverlayLegend.OverlayInfo overlayInfo in this.overlayInfoList)
			{
				overlayInfo.name = Strings.Get(overlayInfo.name);
				for (int i = 0; i < overlayInfo.infoUnits.Count; i++)
				{
					overlayInfo.infoUnits[i].description = Strings.Get(overlayInfo.infoUnits[i].description);
					if (!string.IsNullOrEmpty(overlayInfo.infoUnits[i].tooltip))
					{
						overlayInfo.infoUnits[i].tooltip = Strings.Get(overlayInfo.infoUnits[i].tooltip);
					}
				}
			}
			this.ClearLegend();
			return;
		}
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	protected override void OnLoadLevel()
	{
		OverlayLegend.Instance = null;
		this.activeDiagrams.Clear();
		global::UnityEngine.Object.Destroy(base.gameObject);
		base.OnLoadLevel();
	}

	private void OnRegionChanged()
	{
		if (this.currentMode != SimViewMode.Regions)
		{
			return;
		}
		this.SetLegend(SimViewMode.Regions, true);
	}

	private void OnChamberChanged()
	{
		if (this.currentMode != SimViewMode.Rooms)
		{
			return;
		}
		this.SetLegend(SimViewMode.Rooms, true);
	}

	private void SetLegend(OverlayLegend.OverlayInfo overlayInfo)
	{
		if (overlayInfo == null)
		{
			this.ClearLegend();
			return;
		}
		if (!overlayInfo.isProgrammaticallyPopulated && (overlayInfo.infoUnits == null || overlayInfo.infoUnits.Count == 0))
		{
			this.ClearLegend();
			return;
		}
		base.Show(true);
		this.title.text = overlayInfo.name;
		if (overlayInfo.isProgrammaticallyPopulated)
		{
			SimViewMode mode = overlayInfo.mode;
			if (mode != SimViewMode.Disease)
			{
				if (mode != SimViewMode.NoisePollution)
				{
					if (mode == SimViewMode.Rooms)
					{
						this.PopulateRoomsLegend(overlayInfo);
					}
				}
				else
				{
					this.PopulateNoiseLegend(overlayInfo);
				}
			}
			else
			{
				this.PopulateDiseaseLegend(overlayInfo);
			}
		}
		else
		{
			this.PopulateOverlayInfoUnits(overlayInfo);
		}
	}

	public void SetLegend(SimViewMode mode, bool refreshing = false)
	{
		if (this.currentMode == mode && !refreshing)
		{
			return;
		}
		this.ClearLegend();
		OverlayLegend.OverlayInfo overlayInfo = this.overlayInfoList.Find((OverlayLegend.OverlayInfo ol) => ol.mode == mode);
		if (mode == SimViewMode.Regions)
		{
			overlayInfo.infoUnits.Clear();
			List<Region> regionPrefabs = Game.Instance.RegionManager.regionPrefabs;
			for (int i = 0; i < regionPrefabs.Count; i++)
			{
				overlayInfo.infoUnits.Add(new OverlayLegend.OverlayInfoUnit(this.emptySprite, regionPrefabs[i].RegionName, regionPrefabs[i].OverlayColor, Color.white, null, true));
			}
		}
		else if (mode == SimViewMode.TemperatureMap)
		{
			int num = SimDebugView.Instance.temperatureThresholds.Length - 1;
			for (int j = 0; j < overlayInfo.infoUnits.Count; j++)
			{
				overlayInfo.infoUnits[j].color = SimDebugView.Instance.temperatureThresholds[num - j].color;
				overlayInfo.infoUnits[j].tooltip = UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE;
				overlayInfo.infoUnits[j].tooltipFormatData = GameUtil.GetFormattedTemperature(SimDebugView.Instance.temperatureThresholds[num - j].value, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true);
			}
		}
		else if (mode == SimViewMode.HeatFlow)
		{
			overlayInfo.infoUnits[0].tooltip = UI.OVERLAYS.HEATFLOW.TOOLTIPS.HEATING;
			overlayInfo.infoUnits[1].tooltip = UI.OVERLAYS.HEATFLOW.TOOLTIPS.NEUTRAL;
			overlayInfo.infoUnits[2].tooltip = UI.OVERLAYS.HEATFLOW.TOOLTIPS.COOLING;
		}
		this.SetLegend(overlayInfo);
		this.currentMode = mode;
	}

	public GameObject GetFreeUnitObject()
	{
		if (this.inactiveUnitObjs.Count == 0)
		{
			this.inactiveUnitObjs.Add(Util.KInstantiateUI(this.unitPrefab, this.inactiveUnitsParent, false));
		}
		GameObject gameObject = this.inactiveUnitObjs[0];
		this.inactiveUnitObjs.RemoveAt(0);
		this.activeUnitObjs.Add(gameObject);
		return gameObject;
	}

	public void ClearLegend()
	{
		while (this.activeUnitObjs.Count > 0)
		{
			this.activeUnitObjs[0].transform.Find("Icon").GetComponent<Image>().enabled = false;
			this.activeUnitObjs[0].GetComponentInChildren<LocText>().enabled = false;
			this.activeUnitObjs[0].transform.SetParent(this.inactiveUnitsParent.transform);
			this.activeUnitObjs[0].SetActive(false);
			this.inactiveUnitObjs.Add(this.activeUnitObjs[0]);
			this.activeUnitObjs.RemoveAt(0);
		}
		for (int i = 0; i < this.activeDiagrams.Count; i++)
		{
			if (this.activeDiagrams[i] != null)
			{
				global::UnityEngine.Object.Destroy(this.activeDiagrams[i]);
			}
		}
		this.activeDiagrams.Clear();
		Vector2 sizeDelta = this.diagramsParent.GetComponent<RectTransform>().sizeDelta;
		sizeDelta.y = 0f;
		this.diagramsParent.GetComponent<RectTransform>().sizeDelta = sizeDelta;
		base.Show(false);
	}

	public OverlayLegend.OverlayInfo GetOverlayInfo(SimViewMode mode)
	{
		for (int i = 0; i < this.overlayInfoList.Count; i++)
		{
			if (this.overlayInfoList[i].mode == mode)
			{
				return this.overlayInfoList[i];
			}
		}
		return null;
	}

	private void PopulateOverlayInfoUnits(OverlayLegend.OverlayInfo overlayInfo)
	{
		foreach (OverlayLegend.OverlayInfoUnit overlayInfoUnit in overlayInfo.infoUnits)
		{
			GameObject freeUnitObject = this.GetFreeUnitObject();
			if (overlayInfoUnit.icon != null)
			{
				Image component = freeUnitObject.transform.Find("Icon").GetComponent<Image>();
				component.gameObject.SetActive(true);
				component.sprite = overlayInfoUnit.icon;
				component.color = overlayInfoUnit.color;
				component.enabled = true;
				component.type = ((!overlayInfoUnit.sliceIcon) ? Image.Type.Simple : Image.Type.Sliced);
			}
			else
			{
				freeUnitObject.transform.Find("Icon").gameObject.SetActive(false);
			}
			if (!string.IsNullOrEmpty(overlayInfoUnit.description))
			{
				LocText componentInChildren = freeUnitObject.GetComponentInChildren<LocText>();
				componentInChildren.text = string.Format(overlayInfoUnit.description, overlayInfoUnit.formatData);
				componentInChildren.color = overlayInfoUnit.fontColor;
				componentInChildren.enabled = true;
			}
			ToolTip component2 = freeUnitObject.GetComponent<ToolTip>();
			if (!string.IsNullOrEmpty(overlayInfoUnit.tooltip))
			{
				component2.toolTip = string.Format(overlayInfoUnit.tooltip, overlayInfoUnit.tooltipFormatData);
				component2.enabled = true;
			}
			else
			{
				component2.enabled = false;
			}
			freeUnitObject.SetActive(true);
			freeUnitObject.transform.SetParent(this.activeUnitsParent.transform);
		}
		if (overlayInfo.diagrams != null && overlayInfo.diagrams.Count > 0)
		{
			this.diagramsParent.SetActive(true);
			foreach (GameObject gameObject in overlayInfo.diagrams)
			{
				GameObject gameObject2 = Util.KInstantiateUI(gameObject, this.diagramsParent, false);
				this.activeDiagrams.Add(gameObject2);
			}
		}
		else
		{
			this.diagramsParent.SetActive(false);
		}
	}

	private static float CalculateHUE(Color32 colour)
	{
		byte b = Math.Max(colour.r, Math.Max(colour.g, colour.b));
		byte b2 = Math.Min(colour.r, Math.Min(colour.g, colour.b));
		float num = 0f;
		int num2 = (int)(b - b2);
		if (num2 == 0)
		{
			num = 0f;
		}
		else if (b == colour.r)
		{
			num = (float)(colour.g - colour.b) / (float)num2 % 6f;
		}
		else if (b == colour.g)
		{
			num = (float)(colour.b - colour.r) / (float)num2 + 2f;
		}
		else if (b == colour.b)
		{
			num = (float)(colour.r - colour.g) / (float)num2 + 4f;
		}
		return num;
	}

	private void PopulateDiseaseLegend(OverlayLegend.OverlayInfo info)
	{
		if (info.infoUnits != null && info.infoUnits.Count > 0)
		{
			this.PopulateOverlayInfoUnits(info);
		}
		List<OverlayLegend.DiseaseSortInfo> list = new List<OverlayLegend.DiseaseSortInfo>();
		foreach (Disease disease in Db.Get().Diseases)
		{
			list.Add(new OverlayLegend.DiseaseSortInfo(disease));
		}
		list.Sort((OverlayLegend.DiseaseSortInfo a, OverlayLegend.DiseaseSortInfo b) => a.sortkey.CompareTo(b.sortkey));
		foreach (OverlayLegend.DiseaseSortInfo diseaseSortInfo in list)
		{
			if (diseaseSortInfo.disease.diseaseType == Disease.DiseaseType.Pathogen)
			{
				GameObject freeUnitObject = this.GetFreeUnitObject();
				Image component = freeUnitObject.transform.Find("Icon").GetComponent<Image>();
				component.gameObject.SetActive(true);
				component.sprite = Assets.instance.LegendColourBox;
				component.color = diseaseSortInfo.disease.overlayColour;
				component.enabled = true;
				component.type = Image.Type.Simple;
				LocText componentInChildren = freeUnitObject.GetComponentInChildren<LocText>();
				componentInChildren.text = diseaseSortInfo.disease.Name;
				componentInChildren.color = Color.white;
				componentInChildren.enabled = true;
				ToolTip component2 = freeUnitObject.GetComponent<ToolTip>();
				component2.enabled = true;
				component2.toolTip = diseaseSortInfo.disease.overlayLegendHovertext.ToString();
				freeUnitObject.SetActive(true);
				freeUnitObject.transform.SetParent(this.activeUnitsParent.transform);
			}
		}
		GameObject gameObject = Util.KInstantiateUI(this.toolParameterMenuPrefab, this.diagramsParent, false);
		this.activeDiagrams.Add(gameObject);
		this.diagramsParent.SetActive(true);
		this.toolParameterMenu = gameObject.GetComponent<ToolParameterMenu>();
		this.toolParameterMenu.PopulateMenu(this.diseaseOverlayFilters);
		this.toolParameterMenu.onParametersChanged += this.OnDiseaseFiltersChanged;
		this.OnDiseaseFiltersChanged();
	}

	public void DisableDiseaseOverlay()
	{
		this.toolParameterMenu.onParametersChanged -= this.OnDiseaseFiltersChanged;
		this.toolParameterMenu.ClearMenu();
		this.toolParameterMenu.gameObject.SetActive(false);
		this.toolParameterMenu = null;
	}

	private bool InFilter(string layer, Dictionary<string, ToolParameterMenu.ToggleState> filter)
	{
		return (filter.ContainsKey(ToolParameterMenu.FILTERLAYERS.ALL) && filter[ToolParameterMenu.FILTERLAYERS.ALL] == ToolParameterMenu.ToggleState.On) || (filter.ContainsKey(layer) && filter[layer] == ToolParameterMenu.ToggleState.On);
	}

	private static Dictionary<string, ToolParameterMenu.ToggleState> CreateDefaultFilters()
	{
		return new Dictionary<string, ToolParameterMenu.ToggleState>
		{
			{
				ToolParameterMenu.FILTERLAYERS.ALL,
				ToolParameterMenu.ToggleState.On
			},
			{
				ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT,
				ToolParameterMenu.ToggleState.Off
			},
			{
				ToolParameterMenu.FILTERLAYERS.GASCONDUIT,
				ToolParameterMenu.ToggleState.Off
			}
		};
	}

	private void OnDiseaseFiltersChanged()
	{
		Game.Instance.showGasConduitDisease = this.InFilter(ToolParameterMenu.FILTERLAYERS.GASCONDUIT, this.diseaseOverlayFilters);
		Game.Instance.showLiquidConduitDisease = this.InFilter(ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT, this.diseaseOverlayFilters);
		Game.Instance.ForceOverlayUpdate();
	}

	private void PopulateNoiseLegend(OverlayLegend.OverlayInfo info)
	{
		if (info.infoUnits != null && info.infoUnits.Count > 0)
		{
			this.PopulateOverlayInfoUnits(info);
		}
		string[] names = Enum.GetNames(typeof(AudioEventManager.NoiseEffect));
		Array values = Enum.GetValues(typeof(AudioEventManager.NoiseEffect));
		Color[] dbColours = SimDebugView.Instance.dbColours;
		for (int i = 0; i < names.Length; i++)
		{
			GameObject freeUnitObject = this.GetFreeUnitObject();
			Image component = freeUnitObject.transform.Find("Icon").GetComponent<Image>();
			component.gameObject.SetActive(true);
			component.sprite = Assets.instance.LegendColourBox;
			component.color = ((i != 0) ? Color.Lerp(dbColours[i * 2], dbColours[Mathf.Min(dbColours.Length - 1, i * 2 + 1)], 0.5f) : new Color(1f, 1f, 1f, 0.7f));
			component.enabled = true;
			component.type = Image.Type.Simple;
			string text = names[i].ToUpper();
			int num = (int)values.GetValue(i);
			int num2 = (int)values.GetValue(i);
			LocText componentInChildren = freeUnitObject.GetComponentInChildren<LocText>();
			componentInChildren.text = Strings.Get("STRINGS.UI.OVERLAYS.NOISE_POLLUTION.NAMES." + text) + " " + string.Format(UI.OVERLAYS.NOISE_POLLUTION.RANGE, num);
			componentInChildren.color = Color.white;
			componentInChildren.enabled = true;
			ToolTip component2 = freeUnitObject.GetComponent<ToolTip>();
			component2.enabled = true;
			component2.toolTip = string.Format(Strings.Get("STRINGS.UI.OVERLAYS.NOISE_POLLUTION.TOOLTIPS." + text), num, num2);
			freeUnitObject.SetActive(true);
			freeUnitObject.transform.SetParent(this.activeUnitsParent.transform);
		}
	}

	private void PopulateRoomsLegend(OverlayLegend.OverlayInfo info)
	{
		for (int i = 0; i < RoomTypes.types.Length; i++)
		{
			GameObject freeUnitObject = this.GetFreeUnitObject();
			LocText componentInChildren = freeUnitObject.GetComponentInChildren<LocText>();
			componentInChildren.enabled = true;
			componentInChildren.text = RoomTypes.types[i].name + "\n" + RoomTypes.types[i].effect;
			Image component = freeUnitObject.transform.Find("Icon").GetComponent<Image>();
			component.gameObject.SetActive(true);
			component.sprite = Assets.instance.LegendColourBox;
			component.color = RoomTypes.types[i].category.color;
			component.enabled = true;
			component.type = Image.Type.Simple;
			ToolTip component2 = freeUnitObject.GetComponent<ToolTip>();
			component2.enabled = true;
			component2.toolTip = RoomTypes.types[i].GetCriteriaString();
			freeUnitObject.SetActive(true);
			freeUnitObject.transform.SetParent(this.activeUnitsParent.transform);
		}
	}

	public static OverlayLegend Instance;

	[SerializeField]
	private LocText title;

	[SerializeField]
	private Sprite emptySprite;

	[SerializeField]
	private List<OverlayLegend.OverlayInfo> overlayInfoList;

	[SerializeField]
	private GameObject unitPrefab;

	[SerializeField]
	private GameObject activeUnitsParent;

	[SerializeField]
	private GameObject diagramsParent;

	[SerializeField]
	private GameObject inactiveUnitsParent;

	[SerializeField]
	private GameObject toolParameterMenuPrefab;

	private ToolParameterMenu toolParameterMenu;

	private SimViewMode currentMode;

	private List<GameObject> inactiveUnitObjs;

	private List<GameObject> activeUnitObjs;

	private List<GameObject> activeDiagrams = new List<GameObject>();

	private Dictionary<string, ToolParameterMenu.ToggleState> diseaseOverlayFilters = OverlayLegend.CreateDefaultFilters();

	[Serializable]
	public class OverlayInfoUnit
	{
		public OverlayInfoUnit(Sprite icon, string description, Color color, Color fontColor, object formatData = null, bool sliceIcon = false)
		{
			this.icon = icon;
			this.description = description;
			this.color = color;
			this.fontColor = fontColor;
			this.formatData = formatData;
			this.sliceIcon = sliceIcon;
		}

		public Sprite icon;

		public string description;

		public string tooltip;

		public Color color;

		public Color fontColor;

		public object formatData;

		public object tooltipFormatData;

		public bool sliceIcon;
	}

	[Serializable]
	public class OverlayInfo
	{
		public string name;

		public SimViewMode mode;

		public List<OverlayLegend.OverlayInfoUnit> infoUnits;

		public List<GameObject> diagrams;

		public bool isProgrammaticallyPopulated;
	}

	private struct DiseaseSortInfo
	{
		public DiseaseSortInfo(Disease d)
		{
			this.disease = d;
			this.sortkey = OverlayLegend.CalculateHUE(d.overlayColour);
		}

		public float sortkey;

		public Disease disease;
	}
}
