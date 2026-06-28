using System;
using STRINGS;
using UnityEngine;

namespace TUNING
{
	public class REGIONS
	{
		public const int BEDROOM_MIN_WIDTH = 3;

		public const int BEDROOM_MIN_HEIGHT = 3;

		public const int MESSHALL_MIN_WIDTH = 2;

		public const int MESSHALL_MIN_HEIGHT = 2;

		public const string RoomRegionID = "RoomRegion";

		public const string MedicalRegionID = "MedicalRegion";

		public const string MessHallRegionID = "MessHallRegion";

		public const string DisposalRegionID = "DisposalRegion";

		public const string ToiletRegionID = "ToiletRegion";

		public const string RecreationRegionID = "RecreationRegion";

		public const string ExosuitRegionID = "ExosuitRegion";

		public static readonly Tag RoomRegionTag = TagManager.Create("RoomRegion", null);

		public static readonly Tag MedicalRegionTag = TagManager.Create("MedicalRegion", null);

		public static readonly Tag MessHallRegionTag = TagManager.Create("MessHallRegion", null);

		public static readonly Tag DisposalRegionTag = TagManager.Create("DisposalRegion", null);

		public static readonly Tag ToiletRegionTag = TagManager.Create("ToiletRegion", null);

		public static readonly Tag RecreationRegionTag = TagManager.Create("RecreationRegion", null);

		public static readonly Tag ExosuitRegionTag = TagManager.Create("ExosuitRegion", null);

		public static RegionManager.RegionInfo[] REGIONS_TYPES = new RegionManager.RegionInfo[]
		{
			new RegionManager.RegionInfo(REGIONS.EXOSUITREGION.NAME, "ExosuitRegion", false, new Vector2I(0, 0), true, false, new RegionManager.BuildingRequirement[0], Color.red, UI.TOOLS.EXOSUITREGION.NAME, UI.TOOLS.EXOSUITREGION.TOOLNAME, "overlay_suit", false, new REGIONS.RequiredComponent[]
			{
				new REGIONS.RequiredComponent("ExosuitRegion", null)
			}, global::Action.CreateExosuitRegion, new string[] { "Decor" }, "wallpaper_exosuit")
		};

		public class RequiredComponent
		{
			public RequiredComponent(string typeStr, Action<GameObject> callback)
			{
				this.typeStr = typeStr;
				this.callback = callback;
			}

			public void AddToObject(GameObject gameObject)
			{
				gameObject.AddComponent(Type.GetType(this.typeStr));
				if (this.callback != null)
				{
					this.callback(gameObject);
				}
			}

			public string typeStr;

			public Action<GameObject> callback;
		}
	}
}
