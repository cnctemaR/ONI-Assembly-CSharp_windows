using System;
using Klei;

namespace BaseTemplateClasses
{
	[Serializable]
	public class BaseTemplateCellInfo
	{
		public BaseTemplateCellInfo(int loc_x, int loc_y)
		{
			this.location_x = loc_x;
			this.location_y = loc_y;
			this.element = SimHashes.Oxygen;
			this.temperature = WorldGen.Settings.defaults.GetFloat("StartAreaTemperatureOffset");
			this.mass = WorldGen.Settings.defaults.GetFloat("StartAreaPressureMultiplier");
		}

		public BaseTemplateCellInfo(int loc_x, int loc_y, SimHashes _element, float _temperature, float _mass)
			: this(loc_x, loc_y)
		{
			this.element = _element;
			this.temperature = _temperature;
			this.mass = _mass;
		}

		public SimHashes element;

		public float mass;

		public float temperature;

		public int location_x;

		public int location_y;
	}
}
