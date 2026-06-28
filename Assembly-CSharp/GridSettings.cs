using System;
using System.Collections.Generic;
using UnityEngine;

public class GridSettings : KMonoBehaviour
{
	public static void Reset(int width, int height)
	{
		Grid.WidthInCells = width;
		Grid.HeightInCells = height;
		Grid.CellCount = width * height;
		Grid.WidthInMeters = 1f * (float)width;
		Grid.HeightInMeters = 1f * (float)height;
		Grid.CellSizeInMeters = 1f;
		Grid.HalfCellSizeInMeters = 0.5f;
		Grid.Element = new Element[Grid.CellCount];
		Grid.Revealed = new bool[Grid.CellCount];
		Grid.Reserved = new bool[Grid.CellCount];
		Grid.Visible = new byte[Grid.CellCount];
		Grid.BitFields = new ushort[Grid.CellCount];
		Grid.LightCount = new byte[Grid.CellCount];
		Grid.Damage = new float[Grid.CellCount];
		Grid.HasDoor = new bool[Grid.CellCount];
		Grid.HasLadder = new bool[Grid.CellCount];
		Grid.Decor = new int[Grid.CellCount];
		Grid.ObjectLayers = new Dictionary<int, GameObject>[25];
		for (int i = 0; i < Grid.ObjectLayers.Length; i++)
		{
			Grid.ObjectLayers[i] = new Dictionary<int, GameObject>();
		}
		Grid.Room = new ushort[Grid.CellCount];
		for (int j = 0; j < Grid.CellCount; j++)
		{
			Grid.Room[j] = ushort.MaxValue;
			Grid.SuitRequired[j] = false;
		}
		if (Game.Instance != null)
		{
			Game.Instance.gasConduitSystem.Initialize(Grid.WidthInCells, Grid.HeightInCells);
			Game.Instance.liquidConduitSystem.Initialize(Grid.WidthInCells, Grid.HeightInCells);
			Game.Instance.electricalConduitSystem.Initialize(Grid.WidthInCells, Grid.HeightInCells);
			Game.Instance.gasConduitFlow.Initialize(Grid.CellCount);
			Game.Instance.liquidConduitFlow.Initialize(Grid.CellCount);
		}
		if (Application.isPlaying)
		{
			KBatchedAnimUpdater.instance.InitializeGrid();
		}
	}

	public const float CellSizeInMeters = 1f;

	public int GridWidthInCells;

	public int GridHeightInCells;

	public int SimChunkEdgeSize = 32;
}
