using System;
using System.Collections.Generic;
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
		if (overlayInfo == null || overlayInfo.infoUnits == null || overlayInfo.infoUnits.Count == 0)
		{
			this.ClearLegend();
			return;
		}
		base.Show(true);
		this.title.text = overlayInfo.name;
		foreach (OverlayLegend.OverlayInfoUnit overlayInfoUnit in overlayInfo.infoUnits)
		{
			GameObject freeUnitObject = this.GetFreeUnitObject();
			if (overlayInfoUnit.icon != null)
			{
				Image component = freeUnitObject.transform.FindChild("Icon").GetComponent<Image>();
				component.gameObject.SetActive(true);
				component.sprite = overlayInfoUnit.icon;
				component.color = overlayInfoUnit.color;
				component.enabled = true;
				component.type = ((!overlayInfoUnit.sliceIcon) ? Image.Type.Simple : Image.Type.Sliced);
			}
			else
			{
				freeUnitObject.transform.FindChild("Icon").gameObject.SetActive(false);
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
				overlayInfo.infoUnits[j].tooltipFormatData = GameUtil.GetFormattedTemperature(SimDebugView.Instance.temperatureThresholds[num - j].value, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute);
			}
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
			this.activeUnitObjs[0].transform.FindChild("Icon").GetComponent<Image>().enabled = false;
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

	private SimViewMode currentMode;

	private List<GameObject> inactiveUnitObjs;

	private List<GameObject> activeUnitObjs;

	private List<GameObject> activeDiagrams = new List<GameObject>();

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
	}
}
