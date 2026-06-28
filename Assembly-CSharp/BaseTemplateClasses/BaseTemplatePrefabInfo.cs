using System;
using System.Collections.Generic;

namespace BaseTemplateClasses
{
	[Serializable]
	public class BaseTemplatePrefabInfo
	{
		public BaseTemplatePrefabInfo(string _id, int loc_x, int loc_y)
		{
			this.id = _id;
			this.location_x = loc_x;
			this.location_y = loc_y;
		}

		public BaseTemplatePrefabInfo(string _id, int loc_x, int loc_y, SimHashes _element, float _temperature, float _mass = 1f, Orientation _rotation = Orientation.Up)
			: this(_id, loc_x, loc_y)
		{
			this.element = _element;
			this.temperature = _temperature;
			this.mass = _mass;
			this.rotationOrientation = _rotation;
		}

		public void AssignStorage(BaseTemplateStorageItem _storage)
		{
			if (this.storage == null)
			{
				this.storage = new List<BaseTemplateStorageItem>();
			}
			this.storage.Add(_storage);
		}

		public string id;

		public int location_x;

		public int location_y;

		public SimHashes element;

		public float temperature;

		public float mass;

		public BaseTemplateCellInfo[] placementCells;

		public Orientation rotationOrientation;

		public List<BaseTemplateStorageItem> storage;

		public BaseTemplatePrefabInfo.Ration rations;

		public BaseTemplatePrefabInfo.Rottable rottable;

		[Serializable]
		public struct Ration
		{
			public float rations;
		}

		[Serializable]
		public struct Rottable
		{
			public float rotAmount;
		}
	}
}
