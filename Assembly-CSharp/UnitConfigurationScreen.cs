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
		switch (PlayerPrefs.GetInt("TemperatureUnit", 0))
		{
		case 0:
			this.celsiusToggle.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
			this.kelvinToggle.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
			this.fahrenheitToggle.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
			goto IL_02A4;
		case 2:
			this.celsiusToggle.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
			this.kelvinToggle.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
			this.fahrenheitToggle.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
			goto IL_02A4;
		}
		this.celsiusToggle.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
		this.kelvinToggle.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
		this.fahrenheitToggle.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
		IL_02A4:
		this.closeButton.onClick += this.Deactivate;
		this.doneButton.onClick += this.Deactivate;
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
		PlayerPrefs.SetInt("TemperatureUnit", GameUtil.temperatureUnit.GetHashCode());
		this.celsiusToggle.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
		this.kelvinToggle.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
		this.fahrenheitToggle.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
	}

	private void OnKelvinClicked()
	{
		GameUtil.temperatureUnit = GameUtil.TemperatureUnit.Kelvin;
		PlayerPrefs.SetInt("TemperatureUnit", GameUtil.temperatureUnit.GetHashCode());
		this.celsiusToggle.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
		this.kelvinToggle.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
		this.fahrenheitToggle.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
	}

	private void OnFahrenheitClicked()
	{
		GameUtil.temperatureUnit = GameUtil.TemperatureUnit.Fahrenheit;
		PlayerPrefs.SetInt("TemperatureUnit", GameUtil.temperatureUnit.GetHashCode());
		this.celsiusToggle.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
		this.kelvinToggle.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
		this.fahrenheitToggle.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
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
}
