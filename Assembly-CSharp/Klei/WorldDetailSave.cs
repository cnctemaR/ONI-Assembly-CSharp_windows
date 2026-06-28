using System;
using System.Collections.Generic;
using Delaunay.Geo;
using KSerialization;
using ProcGen;
using ProcGenGame;

namespace Klei
{
	public class WorldDetailSave
	{
		public WorldDetailSave()
		{
			this.overworldCells = new List<WorldDetailSave.OverworldCell>();
		}

		public List<WorldDetailSave.OverworldCell> overworldCells = null;

		public int globalWorldSeed = 0;

		public int globalWorldLayoutSeed = 0;

		public int globalTerrainSeed = 0;

		public int globalNoiseSeed = 0;

		[SerializationConfig(MemberSerialization.OptOut)]
		public class OverworldCell
		{
			public OverworldCell()
			{
			}

			public OverworldCell(TerrainCell tc)
			{
				this.poly = tc.poly;
				SubWorld subWorld = WorldGen.Settings.GetSubWorld(tc.node.type);
				this.zoneType = subWorld.zoneType;
			}

			public Polygon poly;

			public SubWorld.ZoneType zoneType;
		}
	}
}
