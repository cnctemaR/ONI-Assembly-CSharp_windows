using System;
using System.Collections.Generic;
using ProcGen;
using ProcGenGame;
using VoronoiTree;

namespace Klei
{
	public class Data
	{
		public Data()
		{
			this.worldLayout = new WorldLayout(null, 0);
			this.terrainCells = new List<TerrainCell>();
			this.overworldCells = new List<TerrainCell>();
			this.rivers = new List<global::ProcGen.River>();
			this.gameSpawnData = new GameSpawnData();
			this.world = new Chunk();
			this.voronoiTree = new Tree(0);
		}

		public int globalWorldSeed;

		public int globalWorldLayoutSeed;

		public int globalTerrainSeed;

		public int globalNoiseSeed;

		public int chunkEdgeSize = 32;

		public WorldLayout worldLayout;

		public List<TerrainCell> terrainCells;

		public List<TerrainCell> overworldCells;

		public List<global::ProcGen.River> rivers;

		public GameSpawnData gameSpawnData;

		public Chunk world;

		public Tree voronoiTree;

		public AxialI clusterLocation;
	}
}
