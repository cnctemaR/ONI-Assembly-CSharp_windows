using System;
using System.Collections.Generic;
using ProcGen;
using ProcGenGame;

namespace Klei
{
	public class Data
	{
		public Data()
		{
			this.worldLayout = new WorldLayout(0);
			this.terrainCells = new List<TerrainCell>();
			this.overworldCells = new List<TerrainCell>();
			this.rivers = new List<River>();
			this.gameSpawnData = new GameSpawnData();
			this.world = new Chunk();
			this.voronoiTree = new VoronoiTree(0);
		}

		public int globalWorldSeed;

		public int globalWorldLayoutSeed;

		public int globalTerrainSeed;

		public int globalNoiseSeed;

		public int chunkEdgeSize = 32;

		public Vector2I subWorldSize = new Vector2I(512, 256);

		public WorldLayout worldLayout;

		public List<TerrainCell> terrainCells;

		public List<TerrainCell> overworldCells;

		public List<River> rivers;

		public GameSpawnData gameSpawnData;

		public Chunk world;

		public VoronoiTree voronoiTree;
	}
}
