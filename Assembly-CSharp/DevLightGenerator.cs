using System;
using STRINGS;

public class DevLightGenerator : Light2D, IMultiSliderControl
{
	public DevLightGenerator()
	{
		this.sliderControls = new ISliderControl[]
		{
			new DevLightGenerator.LuxController(this),
			new DevLightGenerator.RangeController(this),
			new DevLightGenerator.FalloffController(this)
		};
	}

	string IMultiSliderControl.SidescreenTitleKey
	{
		get
		{
			return "STRINGS.BUILDINGS.PREFABS.DEVLIGHTGENERATOR.NAME";
		}
	}

	ISliderControl[] IMultiSliderControl.sliderControls
	{
		get
		{
			return this.sliderControls;
		}
	}

	bool IMultiSliderControl.SidescreenEnabled()
	{
		return true;
	}

	protected ISliderControl[] sliderControls;

	protected class LuxController : ISingleSliderControl, ISliderControl
	{
		public LuxController(Light2D t)
		{
			this.target = t;
		}

		public string SliderTitleKey
		{
			get
			{
				return "STRINGS.BUILDINGS.PREFABS.DEVLIGHTGENERATOR.BRIGHTNESS_LABEL";
			}
		}

		public string SliderUnits
		{
			get
			{
				return UI.UNITSUFFIXES.LIGHT.LUX;
			}
		}

		public float GetSliderMax(int index)
		{
			return 100000f;
		}

		public float GetSliderMin(int index)
		{
			return 0f;
		}

		public string GetSliderTooltip(int index)
		{
			return string.Format(UI.GAMEOBJECTEFFECTS.EMITS_LIGHT_LUX, this.target.Lux);
		}

		public string GetSliderTooltipKey(int index)
		{
			return "<unused>";
		}

		public float GetSliderValue(int index)
		{
			return (float)this.target.Lux;
		}

		public void SetSliderValue(float value, int index)
		{
			this.target.Lux = (int)value;
			this.target.FullRefresh();
		}

		public int SliderDecimalPlaces(int index)
		{
			return 0;
		}

		protected Light2D target;
	}

	protected class RangeController : ISingleSliderControl, ISliderControl
	{
		public RangeController(Light2D t)
		{
			this.target = t;
		}

		public string SliderTitleKey
		{
			get
			{
				return "STRINGS.BUILDINGS.PREFABS.DEVLIGHTGENERATOR.RANGE_LABEL";
			}
		}

		public string SliderUnits
		{
			get
			{
				return UI.UNITSUFFIXES.TILES;
			}
		}

		public float GetSliderMax(int index)
		{
			return 20f;
		}

		public float GetSliderMin(int index)
		{
			return 1f;
		}

		public string GetSliderTooltip(int index)
		{
			return string.Format(UI.GAMEOBJECTEFFECTS.EMITS_LIGHT, this.target.Range);
		}

		public string GetSliderTooltipKey(int index)
		{
			return "";
		}

		public float GetSliderValue(int index)
		{
			return this.target.Range;
		}

		public void SetSliderValue(float value, int index)
		{
			this.target.Range = (float)((int)value);
			this.target.FullRefresh();
		}

		public int SliderDecimalPlaces(int index)
		{
			return 0;
		}

		protected Light2D target;
	}

	protected class FalloffController : ISingleSliderControl, ISliderControl
	{
		public FalloffController(Light2D t)
		{
			this.target = t;
		}

		public string SliderTitleKey
		{
			get
			{
				return "STRINGS.BUILDINGS.PREFABS.DEVLIGHTGENERATOR.FALLOFF_LABEL";
			}
		}

		public string SliderUnits
		{
			get
			{
				return UI.UNITSUFFIXES.PERCENT;
			}
		}

		public float GetSliderMax(int index)
		{
			return 100f;
		}

		public float GetSliderMin(int index)
		{
			return 1f;
		}

		public string GetSliderTooltip(int index)
		{
			return string.Format("{0}", this.target.FalloffRate * 100f);
		}

		public string GetSliderTooltipKey(int index)
		{
			return "";
		}

		public float GetSliderValue(int index)
		{
			return this.target.FalloffRate * 100f;
		}

		public void SetSliderValue(float value, int index)
		{
			this.target.FalloffRate = value / 100f;
			this.target.FullRefresh();
		}

		public int SliderDecimalPlaces(int index)
		{
			return 0;
		}

		protected Light2D target;
	}
}
