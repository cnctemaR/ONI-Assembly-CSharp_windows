using System;
using System.Collections.Generic;
using Generated;

namespace Klei
{
	public class Data
	{
		public Data()
		{
			this.worldLayout = new WorldLayout();
			this.terrainCells = new List<TerrainCell>();
			this.overworldCells = new List<TerrainCell>();
			this.rivers = new List<River>();
			this.clouds = new List<Cloud>();
			this.gameSpawnData = new WorldGen.GameSpawnData();
			this.world = new Chunk();
			this.voronoiTree = new VoronoiTree();
		}

		public int globalWorldSeed;

		public int chunkEdgeSize = 32;

		public Vector2I subWorldSize = new Vector2I(512, 256);

		public WorldLayout worldLayout;

		public List<TerrainCell> terrainCells;

		public List<TerrainCell> overworldCells;

		public List<River> rivers;

		public List<Cloud> clouds;

		public WorldGen.GameSpawnData gameSpawnData;

		public Chunk world;

		public VoronoiTree voronoiTree;
	}
}
