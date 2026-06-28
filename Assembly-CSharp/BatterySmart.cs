using System;
using System.Diagnostics;
using KSerialization;
using STRINGS;

[SerializationConfig(MemberSerialization.OptIn)]
[DebuggerDisplay("{name}")]
public class BatterySmart : Battery, ISliderControl
{
	private new void SimUpdate(float dt)
	{
		base.SimUpdate(dt);
		float num = base.JoulesAvailable / base.Capacity;
		int num2 = ((num < this.logicOnePercentThreshold) ? 0 : 1);
		this.logicPorts.SendSignal(BatterySmart.PORT_ID, num2);
	}

	public string SliderTitleKey
	{
		get
		{
			return "STRINGS.BUILDINGS.PREFABS.BATTERYSMART.SIDESCREEN_LOGIC_ACTIVE_THRESHOLD";
		}
	}

	public string GetSliderTooltipKey(int index)
	{
		return "STRINGS.BUILDINGS.PREFABS.BATTERYSMART.SIDESCREEN_LOGIC_ACTIVE_THRESHOLD_TOOLTIP";
	}

	public string SliderUnits
	{
		get
		{
			return UI.UNITSUFFIXES.PERCENT;
		}
	}

	public float GetSliderMin(int index)
	{
		return 0f;
	}

	public float GetSliderMax(int index)
	{
		return 100f;
	}

	public float GetSliderValue(int index)
	{
		return this.logicOnePercentThreshold * 100f;
	}

	public void SetSliderValue(float value, int index)
	{
		this.logicOnePercentThreshold = value / 100f;
	}

	public static readonly HashedString PORT_ID = "BatterySmartLogicPort";

	[Serialize]
	private float logicOnePercentThreshold = 1f;

	[MyCmpGet]
	private LogicPorts logicPorts;
}
