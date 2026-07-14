using System;
using System.Collections.Generic;
using ProcGen;
using ProcGenGame;
using UnityEngine;

namespace Klei
{
	public class BiomeSizeData
	{
		public BiomeSizeData(SubWorld.ZoneType id, Vector4 size, List<TerrainCell> terrainCells)
		{
			this.biome = id;
			this.size = size;
			this.terrainCells = terrainCells;
		}

		private SubWorld.ZoneType biome;

		public Vector4 size;

		public List<TerrainCell> terrainCells;
	}
}
