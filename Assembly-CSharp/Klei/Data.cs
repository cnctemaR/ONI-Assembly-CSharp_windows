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
			this.worldLayout = new WorldLayout(0);
			this.terrainCells = new List<TerrainCell>();
			this.overworldCells = new List<TerrainCell>();
			this.rivers = new List<global::ProcGen.River>();
			this.gameSpawnData = new GameSpawnData();
			this.world = new Chunk();
			this.voronoiTree = new Tree(0);
		}

		public int globalWorldSeed = 0;

		public int globalWorldLayoutSeed = 0;

		public int globalTerrainSeed = 0;

		public int globalNoiseSeed = 0;

		public int chunkEdgeSize = 32;

		public Vector2I subWorldSize = new Vector2I(512, 256);

		public WorldLayout worldLayout = null;

		public List<TerrainCell> terrainCells = null;

		public List<TerrainCell> overworldCells = null;

		public List<global::ProcGen.River> rivers = null;

		public GameSpawnData gameSpawnData = null;

		public Chunk world = null;

		public Tree voronoiTree = null;
	}
}
