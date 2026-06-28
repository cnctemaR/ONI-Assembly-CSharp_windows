using System;
using STRINGS;
using UnityEngine;

public class UnitConfigurationScreen : KModalScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
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
		this.closeButton.onClick += this.Deactivate;
		this.doneButton.onClick += this.Deactivate;
	}

	private void DisplayCurrentUnit()
	{
		int @int = KPlayerPrefs.GetInt(UnitConfigurationScreen.TemperatureUnitKey, 0);
		GameUtil.TemperatureUnit temperatureUnit = (GameUtil.TemperatureUnit)@int;
		if (temperatureUnit != GameUtil.TemperatureUnit.Celsius)
		{
			if (temperatureUnit != GameUtil.TemperatureUnit.Kelvin)
			{
				this.celsiusToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(false);
				this.kelvinToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(false);
				this.fahrenheitToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(true);
			}
			else
			{
				this.celsiusToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(false);
				this.kelvinToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(true);
				this.fahrenheitToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(false);
			}
		}
		else
		{
			this.celsiusToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(true);
			this.kelvinToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(false);
			this.fahrenheitToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(false);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape) || e.TryConsume(global::Action.MouseRight))
		{
			this.Deactivate();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	private void OnCelsiusClicked()
	{
		GameUtil.temperatureUnit = GameUtil.TemperatureUnit.Celsius;
		KPlayerPrefs.SetInt(UnitConfigurationScreen.TemperatureUnitKey, GameUtil.temperatureUnit.GetHashCode());
		this.DisplayCurrentUnit();
	}

	private void OnKelvinClicked()
	{
		GameUtil.temperatureUnit = GameUtil.TemperatureUnit.Kelvin;
		KPlayerPrefs.SetInt(UnitConfigurationScreen.TemperatureUnitKey, GameUtil.temperatureUnit.GetHashCode());
		this.DisplayCurrentUnit();
	}

	private void OnFahrenheitClicked()
	{
		GameUtil.temperatureUnit = GameUtil.TemperatureUnit.Fahrenheit;
		KPlayerPrefs.SetInt(UnitConfigurationScreen.TemperatureUnitKey, GameUtil.temperatureUnit.GetHashCode());
		this.DisplayCurrentUnit();
	}

	[SerializeField]
	private GameObject toggleUnitPrefab;

	[SerializeField]
	private GameObject toggleGroup;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton doneButton;

	private GameObject celsiusToggle;

	private GameObject kelvinToggle;

	private GameObject fahrenheitToggle;

	public static readonly string TemperatureUnitKey = "TemperatureUnit";

	public static readonly string MassUnitKey = "MassUnit";
}
