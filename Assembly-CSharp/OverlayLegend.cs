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
			this.PopulateGeneratedLegend(overlayInfo, false);
		}
		else
		{
			this.PopulateOverlayInfoUnits(overlayInfo, false);
		}
	}

	public void SetLegend(OverlayModes.Mode mode, bool refreshing = false)
	{
		if (this.currentMode != null && this.currentMode.ViewMode() == mode.ViewMode() && !refreshing)
		{
			return;
		}
		this.ClearLegend();
		OverlayLegend.OverlayInfo overlayInfo = this.overlayInfoList.Find((OverlayLegend.OverlayInfo ol) => ol.mode == mode.ViewMode());
		this.currentMode = mode;
		this.SetLegend(overlayInfo);
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

	private void RemoveActiveObjects()
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
	}

	public void ClearLegend()
	{
		this.RemoveActiveObjects();
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

	public OverlayLegend.OverlayInfo GetOverlayInfo(OverlayModes.Mode mode)
	{
		for (int i = 0; i < this.overlayInfoList.Count; i++)
		{
			if (this.overlayInfoList[i].mode == mode.ViewMode())
			{
				return this.overlayInfoList[i];
			}
		}
		return null;
	}

	private void PopulateOverlayInfoUnits(OverlayLegend.OverlayInfo overlayInfo, bool isRefresh = false)
	{
		if (overlayInfo.infoUnits != null && overlayInfo.infoUnits.Count > 0)
		{
			this.activeUnitsParent.SetActive(true);
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
		}
		else
		{
			this.activeUnitsParent.SetActive(false);
		}
		if (!isRefresh)
		{
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
	}

	private void PopulateGeneratedLegend(OverlayLegend.OverlayInfo info, bool isRefresh = false)
	{
		if (isRefresh)
		{
			this.RemoveActiveObjects();
		}
		if (info.infoUnits != null && info.infoUnits.Count > 0)
		{
			this.PopulateOverlayInfoUnits(info, isRefresh);
		}
		List<LegendEntry> customLegendData = this.currentMode.GetCustomLegendData();
		if (customLegendData != null)
		{
			this.activeUnitsParent.SetActive(true);
			foreach (LegendEntry legendEntry in customLegendData)
			{
				GameObject freeUnitObject = this.GetFreeUnitObject();
				Image component = freeUnitObject.transform.Find("Icon").GetComponent<Image>();
				component.gameObject.SetActive(true);
				component.sprite = Assets.instance.LegendColourBox;
				component.color = legendEntry.colour;
				component.enabled = true;
				component.type = Image.Type.Simple;
				LocText componentInChildren = freeUnitObject.GetComponentInChildren<LocText>();
				componentInChildren.text = legendEntry.name;
				componentInChildren.color = Color.white;
				componentInChildren.enabled = true;
				ToolTip component2 = freeUnitObject.GetComponent<ToolTip>();
				component2.enabled = true;
				component2.toolTip = legendEntry.desc;
				freeUnitObject.SetActive(true);
				freeUnitObject.transform.SetParent(this.activeUnitsParent.transform);
			}
		}
		else
		{
			this.activeUnitsParent.SetActive(false);
		}
		if (!isRefresh && this.currentMode.legendFilters != null)
		{
			GameObject gameObject = Util.KInstantiateUI(this.toolParameterMenuPrefab, this.diagramsParent, false);
			this.activeDiagrams.Add(gameObject);
			this.diagramsParent.SetActive(true);
			this.filterMenu = gameObject.GetComponent<ToolParameterMenu>();
			this.filterMenu.PopulateMenu(this.currentMode.legendFilters);
			this.filterMenu.onParametersChanged += this.OnFiltersChanged;
			this.OnFiltersChanged();
		}
	}

	private void OnFiltersChanged()
	{
		this.currentMode.OnFiltersChanged();
		this.PopulateGeneratedLegend(this.GetOverlayInfo(this.currentMode), true);
		Game.Instance.ForceOverlayUpdate();
	}

	private void DisableOverlay()
	{
		this.filterMenu.onParametersChanged -= this.OnFiltersChanged;
		this.filterMenu.ClearMenu();
		this.filterMenu.gameObject.SetActive(false);
		this.filterMenu = null;
	}

	private void PopulateNoiseLegend(OverlayLegend.OverlayInfo info)
	{
		if (info.infoUnits != null && info.infoUnits.Count > 0)
		{
			this.PopulateOverlayInfoUnits(info, false);
		}
		string[] names = Enum.GetNames(typeof(AudioEventManager.NoiseEffect));
		Array values = Enum.GetValues(typeof(AudioEventManager.NoiseEffect));
		Color[] dbColours = SimDebugView.dbColours;
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

	private ToolParameterMenu filterMenu;

	private OverlayModes.Mode currentMode;

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

		public HashedString mode;

		public List<OverlayLegend.OverlayInfoUnit> infoUnits;

		public List<GameObject> diagrams;

		public bool isProgrammaticallyPopulated;
	}
}
