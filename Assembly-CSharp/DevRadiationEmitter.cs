using System;
using STRINGS;

public class DevRadiationEmitter : KMonoBehaviour, ISingleSliderControl, ISliderControl
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.radiationEmitter != null)
		{
			this.radiationEmitter.SetEmitting(true);
		}
	}

	public string SliderTitleKey
	{
		get
		{
			return BUILDINGS.PREFABS.DEVRADIATIONGENERATOR.NAME;
		}
	}

	public string SliderUnits
	{
		get
		{
			return UI.UNITSUFFIXES.RADIATION.RADS;
		}
	}

	public float GetSliderMax(int index)
	{
		return 5000f;
	}

	public float GetSliderMin(int index)
	{
		return 0f;
	}

	public string GetSliderTooltip(int index)
	{
		return "";
	}

	public string GetSliderTooltipKey(int index)
	{
		return "";
	}

	public float GetSliderValue(int index)
	{
		return this.radiationEmitter.emitRads;
	}

	public void SetSliderValue(float value, int index)
	{
		this.radiationEmitter.emitRads = value;
		this.radiationEmitter.Refresh();
	}

	public int SliderDecimalPlaces(int index)
	{
		return 0;
	}

	[MyCmpReq]
	private RadiationEmitter radiationEmitter;
}
