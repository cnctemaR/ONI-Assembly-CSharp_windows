using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ProcGen
{
	[DebuggerDisplay("{world} ({x}, {y})")]
	[Serializable]
	public class WorldPlacement
	{
		public static int GetSortOrder(WorldPlacement.LocationType type)
		{
			if (type == WorldPlacement.LocationType.Startworld)
			{
				return 1;
			}
			if (type == WorldPlacement.LocationType.InnerCluster)
			{
				return 2;
			}
			return 3;
		}

		public string world { get; set; }

		public WorldPlacement.WorldMixing worldMixing { get; set; }

		public MinMaxI allowedRings { get; set; }

		public int buffer { get; set; }

		public WorldPlacement.LocationType locationType { get; set; }

		public int x { get; private set; }

		public int y { get; private set; }

		public int width { get; private set; }

		public int height { get; private set; }

		public bool startWorld { get; set; }

		public int hiddenY { get; private set; }

		public WorldPlacement()
		{
			this.allowedRings = new MinMaxI(0, 9999);
			this.buffer = 2;
			this.locationType = WorldPlacement.LocationType.Cluster;
			this.worldMixing = new WorldPlacement.WorldMixing();
		}

		public void SetPosition(Vector2I pos)
		{
			this.x = pos.X;
			this.y = pos.Y;
		}

		public void SetSize(Vector2I size)
		{
			this.width = size.X;
			this.height = size.Y;
		}

		public bool IsMixingPlacement()
		{
			return this.worldMixing.requiredTags.Count != 0 || this.worldMixing.forbiddenTags.Count != 0;
		}

		public void UndoWorldMixing()
		{
			if (this.worldMixing.mixingWasApplied)
			{
				this.world = this.worldMixing.previousWorld;
				this.worldMixing.mixingWasApplied = false;
			}
		}

		public enum LocationType
		{
			Cluster,
			Startworld,
			InnerCluster
		}

		[Serializable]
		public class WorldMixing
		{
			public List<string> requiredTags { get; set; }

			public List<string> forbiddenTags { get; set; }

			public List<World.TemplateSpawnRules> additionalWorldTemplateRules { get; set; }

			public List<World.AllowedCellsFilter> additionalUnknownCellFilters { get; private set; }

			public List<WeightedSubworldName> additionalSubworldFiles { get; private set; }

			public List<string> additionalSeasons { get; private set; }

			public WorldMixing()
			{
				this.requiredTags = new List<string>();
				this.forbiddenTags = new List<string>();
				this.additionalWorldTemplateRules = new List<World.TemplateSpawnRules>();
				this.additionalUnknownCellFilters = new List<World.AllowedCellsFilter>();
				this.additionalSubworldFiles = new List<WeightedSubworldName>();
				this.additionalSeasons = new List<string>();
			}

			[NonSerialized]
			public bool mixingWasApplied;

			[NonSerialized]
			public string previousWorld;
		}
	}
}
