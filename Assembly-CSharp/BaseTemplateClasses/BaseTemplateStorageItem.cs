using System;

namespace BaseTemplateClasses
{
	[Serializable]
	public class BaseTemplateStorageItem
	{
		public BaseTemplateStorageItem(string _id, float _units, float _temp, SimHashes _element, bool _isOre)
		{
			this.id = _id;
			this.element = _element;
			this.units = _units;
			this.isOre = _isOre;
			this.temperature = _temp;
		}

		public string id;

		public SimHashes element;

		public float units;

		public bool isOre;

		public float temperature;

		public BaseTemplateStorageItem.Rottable rottable;

		[Serializable]
		public struct Rottable
		{
			public float rotAmount;
		}
	}
}
