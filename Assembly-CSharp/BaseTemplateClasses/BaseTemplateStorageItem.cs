using System;

namespace BaseTemplateClasses
{
	[Serializable]
	public class BaseTemplateStorageItem
	{
		public BaseTemplateStorageItem(string _id, float _mass, float _temp, SimHashes _element, bool _isOre)
		{
			this.id = _id;
			this.element = _element;
			this.mass = _mass;
			this.isOre = _isOre;
			this.temperature = _temp;
		}

		public string id;

		public SimHashes element;

		public float mass;

		public bool isOre;

		public float temperature;

		public BaseTemplateStorageItem.Ration rations;

		public BaseTemplateStorageItem.Rottable rottable;

		[Serializable]
		public struct Ration
		{
			public int rations;
		}

		[Serializable]
		public struct Rottable
		{
			public float rotAmount;
		}
	}
}
