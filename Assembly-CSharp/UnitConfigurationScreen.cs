using System;
using STRINGS;
using UnityEngine;

[Serializable]
public class UnitConfigurationScreen
{
	public void Init()
	{
		this.celsiusToggle = Util.KInstantiateUI(this.toggleUnitPrefab, this.toggleGroup, true);
		this.celsiusToggle.GetComponentInChildren<ToolTip>().toolTip = UI.FRONTEND.UNIT_OPTIONS_SCREEN.CELSIUS_TOOLTIP;
		this.celsiusToggle.GetComponentInChildren<KButton>().onClick += this.OnCelsiusClicked;
		this.celsiusToggle.GetComponentInChildren<LocText>().text = UI.FRONTEND.UNIT_OPTIONS_SCREEN.CELSIUS;
		this.kelvinToggle = Util.KInstantiateUI(this.toggleUnitPrefab, this.toggleGroup, true);
		this.kelvinToggle.GetComponentInChildren<ToolTip>().toolTip = UI.FRONTEND.UNIT_OPTIONS_SCREEN.KELVIN_TOOLTIP;
		this.kelvinToggle.GetComponentInChildren<KButton>().onClick += this.OnKelvinClicked;
		this.kelvinToggle.GetComponentInChildren<LocText>().text = UI.FRONTEND.UNIT_OPTIONS_SCREEN.KELVIN;
		this.fahrenheitToggle = Util.KInstantiateUI(this.toggleUnitPrefab, this.toggleGroup, true);
		this.fahrenheitToggle.GetComponentInChildren<ToolTip>().toolTip = UI.FRONTEND.UNIT_OPTIONS_SCREEN.FAHRENHEIT_TOOLTIP;
		this.fahrenheitToggle.GetComponentInChildren<KButton>().onClick += this.OnFahrenheitClicked;
		this.fahrenheitToggle.GetComponentInChildren<LocText>().text = UI.FRONTEND.UNIT_OPTIONS_SCREEN.FAHRENHEIT;
		this.DisplayCurrentUnit();
	}

	private void DisplayCurrentUnit()
	{
		GameUtil.TemperatureUnit @int = (GameUtil.TemperatureUnit)KPlayerPrefs.GetInt(UnitConfigurationScreen.TemperatureUnitKey, 0);
		if (@int == GameUtil.TemperatureUnit.Celsius)
		{
			this.celsiusToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(true);
			this.kelvinToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(false);
			this.fahrenheitToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(false);
			return;
		}
		if (@int != GameUtil.TemperatureUnit.Kelvin)
		{
			this.celsiusToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(false);
			this.kelvinToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(false);
			this.fahrenheitToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(true);
			return;
		}
		this.celsiusToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(false);
		this.kelvinToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(true);
		this.fahrenheitToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(false);
	}

	private void OnCelsiusClicked()
	{
		GameUtil.temperatureUnit = GameUtil.TemperatureUnit.Celsius;
		KPlayerPrefs.SetInt(UnitConfigurationScreen.TemperatureUnitKey, GameUtil.temperatureUnit.GetHashCode());
		this.DisplayCurrentUnit();
		if (Game.Instance != null)
		{
			Game.Instance.Trigger(999382396, GameUtil.TemperatureUnit.Celsius);
		}
	}

	private void OnKelvinClicked()
	{
		GameUtil.temperatureUnit = GameUtil.TemperatureUnit.Kelvin;
		KPlayerPrefs.SetInt(UnitConfigurationScreen.TemperatureUnitKey, GameUtil.temperatureUnit.GetHashCode());
		this.DisplayCurrentUnit();
		if (Game.Instance != null)
		{
			Game.Instance.Trigger(999382396, GameUtil.TemperatureUnit.Kelvin);
		}
	}

	private void OnFahrenheitClicked()
	{
		GameUtil.temperatureUnit = GameUtil.TemperatureUnit.Fahrenheit;
		KPlayerPrefs.SetInt(UnitConfigurationScreen.TemperatureUnitKey, GameUtil.temperatureUnit.GetHashCode());
		this.DisplayCurrentUnit();
		if (Game.Instance != null)
		{
			Game.Instance.Trigger(999382396, GameUtil.TemperatureUnit.Fahrenheit);
		}
	}

	[SerializeField]
	private GameObject toggleUnitPrefab;

	[SerializeField]
	private GameObject toggleGroup;

	private GameObject celsiusToggle;

	private GameObject kelvinToggle;

	private GameObject fahrenheitToggle;

	public static readonly string TemperatureUnitKey = "TemperatureUnit";

	public static readonly string MassUnitKey = "MassUnit";
}
