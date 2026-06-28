using System;
using System.Collections.Generic;
using Delaunay.Geo;
using KSerialization;

namespace Klei
{
	public class WorldDetailSave
	{
		public WorldDetailSave()
		{
			this.overworldCells = new List<WorldDetailSave.OverworldCell>();
		}

		public List<WorldDetailSave.OverworldCell> overworldCells;

		public int globalWorldSeed;

		[SerializationConfig(MemberSerialization.OptOut)]
		public class OverworldCell
		{
			public OverworldCell()
			{
			}

			public OverworldCell(TerrainCell tc)
			{
				this.poly = tc.poly;
				SubWorld subWorld = WorldGen.Settings.subworlds.zones[tc.node.type];
				this.zoneType = subWorld.zoneType;
			}

			public Polygon poly;

			public SubWorld.ZoneType zoneType;
		}
	}
}
