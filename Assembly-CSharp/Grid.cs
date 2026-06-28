using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
	public static bool IsInitialized()
	{
		return Grid.CellValues != null;
	}

	public static int GetCellInDirection(int cell, Direction d)
	{
		switch (d)
		{
		case Direction.Up:
			return cell + Grid.WidthInCells;
		case Direction.Right:
			return cell + 1;
		case Direction.Down:
			return cell - Grid.WidthInCells;
		case Direction.Left:
			return cell - 1;
		case Direction.None:
			return cell;
		}
		return -1;
	}

	public static int CellAbove(int cell)
	{
		return cell + Grid.WidthInCells;
	}

	public static int CellBelow(int cell)
	{
		return cell - Grid.WidthInCells;
	}

	public static int CellLeft(int cell)
	{
		return (cell % Grid.WidthInCells <= 0) ? (-1) : (cell - 1);
	}

	public static int CellRight(int cell)
	{
		return (cell % Grid.WidthInCells >= Grid.WidthInCells - 1) ? (-1) : (cell + 1);
	}

	public static CellOffset GetOffset(int cell)
	{
		int num = 0;
		int num2 = 0;
		Grid.CellToXY(cell, out num, out num2);
		return new CellOffset(num, num2);
	}

	public static int CellUpLeft(int cell)
	{
		int num = -1;
		if (cell < (Grid.HeightInCells - 1) * Grid.WidthInCells && cell % Grid.WidthInCells > 0)
		{
			num = cell - 1 + Grid.WidthInCells;
		}
		return num;
	}

	public static int CellUpRight(int cell)
	{
		int num = -1;
		if (cell < (Grid.HeightInCells - 1) * Grid.WidthInCells && cell % Grid.WidthInCells < Grid.WidthInCells - 1)
		{
			num = cell + 1 + Grid.WidthInCells;
		}
		return num;
	}

	public static int CellDownLeft(int cell)
	{
		int num = -1;
		if (cell > Grid.WidthInCells && cell % Grid.WidthInCells > 0)
		{
			num = cell - 1 - Grid.WidthInCells;
		}
		return num;
	}

	public static int CellDownRight(int cell)
	{
		int num = -1;
		if (cell >= Grid.WidthInCells && cell % Grid.WidthInCells < Grid.WidthInCells - 1)
		{
			num = cell + 1 - Grid.WidthInCells;
		}
		return num;
	}

	public static bool IsCellLeftOf(int cell, int other_cell)
	{
		return Grid.CellColumn(cell) < Grid.CellColumn(other_cell);
	}

	public static bool IsCellOffsetOf(int cell, int target_cell, CellOffset[] target_offsets)
	{
		int num = target_offsets.Length;
		for (int i = 0; i < num; i++)
		{
			if (cell == Grid.OffsetCell(target_cell, target_offsets[i]))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsCellOffsetOf(int cell, GameObject target, CellOffset[] target_offsets)
	{
		int num = Grid.PosToCell(target);
		return Grid.IsCellOffsetOf(cell, num, target_offsets);
	}

	public static int GetCellDistance(int cell_a, int cell_b)
	{
		CellOffset offset = Grid.GetOffset(cell_a, cell_b);
		return Math.Abs(offset.x) + Math.Abs(offset.y);
	}

	public static CellOffset GetOffset(int base_cell, int offset_cell)
	{
		int num;
		int num2;
		Grid.CellToXY(base_cell, out num, out num2);
		int num3;
		int num4;
		Grid.CellToXY(offset_cell, out num3, out num4);
		return new CellOffset(num3 - num, num4 - num2);
	}

	public static int OffsetCell(int cell, CellOffset offset)
	{
		return cell + offset.x + offset.y * Grid.WidthInCells;
	}

	public static int OffsetCell(int cell, int x, int y)
	{
		return cell + x + y * Grid.WidthInCells;
	}

	public static int PosToCell(GameObject go)
	{
		return Grid.PosToCell(go.transform.position);
	}

	public static int PosToCell(KMonoBehaviour cmp)
	{
		return Grid.PosToCell(cmp.transform.position);
	}

	public static bool IsValidCell(int cell)
	{
		return cell >= 0 && cell < Grid.CellCount;
	}

	public static int PosToCell(Vector2 pos)
	{
		float x = pos.x;
		float num = pos.y + 0.05f;
		int num2 = (int)(num / Grid.CellSizeInMeters);
		int num3 = (int)(x / Grid.CellSizeInMeters);
		return num2 * Grid.WidthInCells + num3;
	}

	public static int PosToCell(Vector3 pos)
	{
		float x = pos.x;
		float num = pos.y + 0.05f;
		int num2 = (int)(num / Grid.CellSizeInMeters);
		int num3 = (int)(x / Grid.CellSizeInMeters);
		return num2 * Grid.WidthInCells + num3;
	}

	public static void PosToXY(Vector3 pos, out int x, out int y)
	{
		int num = Grid.PosToCell(pos);
		Grid.CellToXY(num, out x, out y);
	}

	public static void PosToXY(Vector3 pos, out Vector2I xy)
	{
		int num = Grid.PosToCell(pos);
		Grid.CellToXY(num, out xy.x, out xy.y);
	}

	public static Vector2I PosToXY(Vector3 pos)
	{
		int num = Grid.PosToCell(pos);
		Vector2I vector2I;
		Grid.CellToXY(num, out vector2I.x, out vector2I.y);
		return vector2I;
	}

	public static int XYToCell(int x, int y)
	{
		return x + y * Grid.WidthInCells;
	}

	public static void CellToXY(int cell, out int x, out int y)
	{
		x = Grid.CellColumn(cell);
		y = Grid.CellRow(cell);
	}

	public static Vector2I CellToXY(int cell)
	{
		return new Vector2I(Grid.CellColumn(cell), Grid.CellRow(cell));
	}

	public static Vector3 CellToPos(int cell, float x_offset, float y_offset, float z_offset)
	{
		int widthInCells = Grid.WidthInCells;
		float num = Grid.CellSizeInMeters * (float)(cell % widthInCells);
		float num2 = Grid.CellSizeInMeters * (float)(cell / widthInCells);
		return new Vector3(num + x_offset, num2 + y_offset, z_offset);
	}

	public static Vector3 CellToPos(int cell)
	{
		int widthInCells = Grid.WidthInCells;
		float num = Grid.CellSizeInMeters * (float)(cell % widthInCells);
		float num2 = Grid.CellSizeInMeters * (float)(cell / widthInCells);
		return new Vector3(num, num2, 0f);
	}

	public static Vector3 CellToPos2D(int cell)
	{
		int widthInCells = Grid.WidthInCells;
		float num = Grid.CellSizeInMeters * (float)(cell % widthInCells);
		float num2 = Grid.CellSizeInMeters * (float)(cell / widthInCells);
		return new Vector2(num, num2);
	}

	public static int CellRow(int cell)
	{
		return cell / Grid.WidthInCells;
	}

	public static int CellColumn(int cell)
	{
		return cell % Grid.WidthInCells;
	}

	public static int ClampX(int x)
	{
		return Math.Min(Math.Max(x, 0), Grid.WidthInCells - 1);
	}

	public static int ClampY(int y)
	{
		return Math.Min(Math.Max(y, 0), Grid.HeightInCells - 1);
	}

	public static Vector2I Constrain(Vector2I val)
	{
		val.x = Mathf.Max(0, Mathf.Min(val.x, Grid.WidthInCells - 1));
		val.y = Mathf.Max(0, Mathf.Min(val.y, Grid.HeightInCells - 1));
		return val;
	}

	public static void Reveal(int cell, byte visibility = 255)
	{
		bool flag = Grid.Spawnable[cell] == 0 && visibility > 0;
		Grid.Spawnable[cell] = Math.Max(visibility, Grid.Visible[cell]);
		if (!Grid.PreventFogOfWarReveal[cell])
		{
			Grid.Visible[cell] = Math.Max(visibility, Grid.Visible[cell]);
		}
		if (flag && Grid.OnReveal != null)
		{
			Grid.OnReveal(cell);
		}
	}

	public static Vector3 CellToPos(int cell, CellAlignment alignment, Grid.SceneLayer layer)
	{
		switch (alignment)
		{
		case CellAlignment.Bottom:
			return Grid.CellToPosCBC(cell, layer);
		case CellAlignment.Top:
			return Grid.CellToPosCTC(cell, layer);
		case CellAlignment.Left:
			return Grid.CellToPosLCC(cell, layer);
		case CellAlignment.Right:
			return Grid.CellToPosRCC(cell, layer);
		case CellAlignment.RandomInternal:
		{
			Vector3 vector = new Vector3(global::UnityEngine.Random.Range(-0.3f, 0.3f), 0f, 0f);
			return Grid.CellToPosCCC(cell, layer) + vector;
		}
		}
		return Grid.CellToPosCCC(cell, layer);
	}

	public static float GetLayerZ(Grid.SceneLayer layer)
	{
		return -Grid.HalfCellSizeInMeters - Grid.CellSizeInMeters * (float)layer * Grid.LayerMultiplier;
	}

	public static Vector3 CellToPosCCC(int cell, Grid.SceneLayer layer)
	{
		return Grid.CellToPos(cell, Grid.HalfCellSizeInMeters, Grid.HalfCellSizeInMeters, Grid.GetLayerZ(layer));
	}

	public static Vector3 CellToPosCBC(int cell, Grid.SceneLayer layer)
	{
		return Grid.CellToPos(cell, Grid.HalfCellSizeInMeters, 0.01f, Grid.GetLayerZ(layer));
	}

	public static Vector3 CellToPosCCF(int cell, Grid.SceneLayer layer)
	{
		return Grid.CellToPos(cell, Grid.HalfCellSizeInMeters, Grid.HalfCellSizeInMeters, -Grid.CellSizeInMeters * (float)layer * Grid.LayerMultiplier);
	}

	public static Vector3 CellToPosLCC(int cell, Grid.SceneLayer layer)
	{
		return Grid.CellToPos(cell, 0.01f, Grid.HalfCellSizeInMeters, Grid.GetLayerZ(layer));
	}

	public static Vector3 CellToPosRCC(int cell, Grid.SceneLayer layer)
	{
		return Grid.CellToPos(cell, Grid.CellSizeInMeters - 0.01f, Grid.HalfCellSizeInMeters, Grid.GetLayerZ(layer));
	}

	public static Vector3 CellToPosCTC(int cell, Grid.SceneLayer layer)
	{
		return Grid.CellToPos(cell, Grid.HalfCellSizeInMeters, Grid.CellSizeInMeters - 0.01f, Grid.GetLayerZ(layer));
	}

	public static void SetSolid(int cell, bool solid, CellSolidEvent ev)
	{
		Grid.BitFields[cell] = (ushort)((byte)(Grid.BitFields[cell] & 65503));
		ushort[] bitFields = Grid.BitFields;
		bitFields[cell] |= (ushort)((!solid) ? 0 : 32);
	}

	public static bool IsSubstantialLiquid(int cell, float threshold = 0.35f)
	{
		if (Grid.IsValidCell(cell))
		{
			byte elementIdx = Grid.Cell[cell].elementIdx;
			if ((int)elementIdx < ElementLoader.elements.Count)
			{
				Element element = ElementLoader.elements[(int)Grid.Cell[cell].elementIdx];
				if (element.IsLiquid && Grid.Cell[cell].mass >= element.defaultValues.mass * threshold)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool IsLiquid(int cell)
	{
		Element element = ElementLoader.elements[(int)Grid.Cell[cell].elementIdx];
		return element.IsLiquid;
	}

	public static bool IsGas(int cell)
	{
		Element element = ElementLoader.elements[(int)Grid.Cell[cell].elementIdx];
		return element.IsGas;
	}

	public static void GetVisibleExtents(out int min_x, out int min_y, out int max_x, out int max_y)
	{
		Vector3 vector = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.main.transform.position.z));
		Vector3 vector2 = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, Camera.main.transform.position.z));
		min_y = (int)vector2.y;
		max_y = (int)(vector.y + 0.5f);
		min_x = (int)vector2.x;
		max_x = (int)(vector.x + 0.5f);
	}

	public static void GetVisibleExtents(out Vector2I min, out Vector2I max)
	{
		Grid.GetVisibleExtents(out min.x, out min.y, out max.x, out max.y);
	}

	public unsafe static void InitializeCells(Sim.Cell* cells)
	{
		int widthInCells = Grid.WidthInCells;
		int heightInCells = Grid.HeightInCells;
		List<Element> elements = ElementLoader.elements;
		for (int i = 0; i < heightInCells; i++)
		{
			for (int j = 0; j < widthInCells; j++)
			{
				int num = i * widthInCells + j;
				byte elementIdx = cells[num].elementIdx;
				Element element = elements[(int)elementIdx];
				Grid.Element[num] = element;
				int num2 = (int)Grid.BitFields[num];
				num2 &= 65303;
				num2 |= ((!element.IsSolid) ? 0 : 96);
				num2 |= ((element.substance == null || !element.substance.renderedByWorld || !(Grid.Objects[num, 9] == null)) ? 0 : 128);
				Grid.BitFields[num] = (ushort)((byte)num2);
			}
		}
	}

	public static bool VisibilityTest(int x, int y, int x2, int y2)
	{
		int num = x;
		int num2 = y;
		int num3 = x2 - x;
		int num4 = y2 - y;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		int num8 = 0;
		if (num3 < 0)
		{
			num5 = -1;
		}
		else if (num3 > 0)
		{
			num5 = 1;
		}
		if (num4 < 0)
		{
			num6 = -1;
		}
		else if (num4 > 0)
		{
			num6 = 1;
		}
		if (num3 < 0)
		{
			num7 = -1;
		}
		else if (num3 > 0)
		{
			num7 = 1;
		}
		int num9 = Math.Abs(num3);
		int num10 = Math.Abs(num4);
		if (num9 <= num10)
		{
			num9 = Math.Abs(num4);
			num10 = Math.Abs(num3);
			if (num4 < 0)
			{
				num8 = -1;
			}
			else if (num4 > 0)
			{
				num8 = 1;
			}
			num7 = 0;
		}
		int num11 = num9 >> 1;
		for (int i = 0; i <= num9; i++)
		{
			if ((x != num || y != num2) && Grid.Element[Grid.XYToCell(x, y)].IsSolid)
			{
				return false;
			}
			num11 += num10;
			if (num11 >= num9)
			{
				num11 -= num9;
				x += num5;
				y += num6;
			}
			else
			{
				x += num7;
				y += num8;
			}
		}
		return true;
	}

	public static bool VisibilityTest(int cell, int target_cell)
	{
		int num = 0;
		int num2 = 0;
		Grid.CellToXY(cell, out num, out num2);
		int num3 = 0;
		int num4 = 0;
		Grid.CellToXY(target_cell, out num3, out num4);
		return Grid.VisibilityTest(num, num2, num3, num4);
	}

	public static readonly CellOffset[] DefaultOffset = new CellOffset[] { default(CellOffset) };

	public static float WidthInMeters;

	public static float HeightInMeters;

	public static int WidthInCells;

	public static int HeightInCells;

	public static float CellSizeInMeters;

	public static float HalfCellSizeInMeters;

	public static int CellCount;

	public static int InvalidCell = -1;

	public unsafe static Sim.Cell* CellValues = null;

	public unsafe static Sim.DiseaseCell* DiseaseCellValues = null;

	public unsafe static float* AccumulatedFlowValues = null;

	public static Grid.CellIndexer Cell;

	public static Grid.DiseaseCellIndexer Disease;

	public static bool[] Revealed;

	public static bool[] Reserved;

	public static byte[] Visible;

	public static byte[] Spawnable;

	public static float[] Damage;

	public static bool[] HasDoor;

	public static bool[] HasAccessDoor;

	public static bool[] HasLadder;

	public static bool[] HasPole;

	public static bool[] IsTileUnderConstruction;

	public static bool[] PreventFogOfWarReveal;

	public static bool[] PreventIdlingOnCell;

	public static float[] Decor;

	public static float[] Loudness;

	public static ushort[] Room;

	public static Grid.PressureIndexer Pressure;

	public static Grid.TemperatureIndexer Temperature;

	public static Grid.AccumulatedFlowIndexer AccumulatedFlow;

	public static Grid.FoundationIndexer Foundation;

	public static Grid.SolidIndexer Solid;

	public static Grid.PreviousSolidIndexer PreviousSolid;

	public static Grid.RenderedByWorldIndexer RenderedByWorld;

	public static Grid.FakeFloorIndexer FakeFloor;

	public static Grid.LiquidPumpFloorIndexer LiquidPumpFloor;

	public static Grid.ForceFieldIndexer ForceField;

	public static Grid.ImpassableIndexer Impassable;

	public static ushort[] BitFields;

	public static Element[] Element;

	public static byte[] LightCount;

	public static Grid.ObjectLayerIndexer Objects;

	public static Dictionary<int, GameObject>[] ObjectLayers;

	public static Action<int> OnReveal;

	public static float LayerMultiplier = 1f;

	[Flags]
	public enum BitField : ushort
	{
		Unused = 1,
		FakeFloor = 2,
		ForceField = 4,
		SuitRequired = 8,
		Foundation = 16,
		Solid = 32,
		PreviousSolid = 64,
		RenderedByWorld = 128,
		Impassable = 256,
		LiquidPumpFloor = 512
	}

	public enum SceneLayer
	{
		NoLayer = -2,
		Background,
		GasConduits = 1,
		GasConduitBridges,
		LiquidConduits,
		LiquidConduitBridges,
		Wires,
		WireBridges,
		Paintings,
		BuildingBack,
		Building,
		BuildingUse,
		BuildingFront,
		Ore,
		Creatures,
		Move,
		Front,
		Liquid,
		Ground,
		TileMain,
		TileFront,
		FXFront,
		FXFront2,
		SceneMAX
	}

	public struct ObjectLayerIndexer
	{
		public GameObject this[int cell, int layer]
		{
			get
			{
				GameObject gameObject = null;
				Grid.ObjectLayers[layer].TryGetValue(cell, out gameObject);
				return gameObject;
			}
			set
			{
				if (value == null)
				{
					Grid.ObjectLayers[layer].Remove(cell);
				}
				else
				{
					Grid.ObjectLayers[layer][cell] = value;
				}
				GameScenePartitioner.Instance.TriggerEvent(cell, GameScenePartitioner.Instance.objectLayers[layer], value);
			}
		}
	}

	public struct CellIndexer
	{
		public unsafe Sim.Cell this[int i]
		{
			get
			{
				return Grid.CellValues[i];
			}
		}
	}

	public struct DiseaseCellIndexer
	{
		public unsafe Sim.DiseaseCell this[int i]
		{
			get
			{
				return Grid.DiseaseCellValues[i];
			}
		}
	}

	public struct PressureIndexer
	{
		public float this[int i]
		{
			get
			{
				return Grid.Cell[i].mass * 101.3f;
			}
		}
	}

	public struct TemperatureIndexer
	{
		public float this[int i]
		{
			get
			{
				return Grid.Cell[i].temperature;
			}
		}
	}

	public struct AccumulatedFlowIndexer
	{
		public unsafe float this[int i]
		{
			get
			{
				return Grid.AccumulatedFlowValues[i];
			}
		}
	}

	public struct FoundationIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.BitFields[i] & 16) != 0;
			}
			set
			{
				Grid.BitFields[i] = (ushort)((byte)(Grid.BitFields[i] & 65519));
				ushort[] bitFields = Grid.BitFields;
				bitFields[i] |= (ushort)((!value) ? 0 : 16);
			}
		}
	}

	public struct SolidIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.BitFields[i] & 32) != 0;
			}
		}
	}

	public struct PreviousSolidIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.BitFields[i] & 64) != 0;
			}
			set
			{
				Grid.BitFields[i] = Grid.BitFields[i] & 65471;
				ushort[] bitFields = Grid.BitFields;
				bitFields[i] |= ((!value) ? 0 : 64);
			}
		}
	}

	public struct RenderedByWorldIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.BitFields[i] & 128) != 0;
			}
			set
			{
				Grid.BitFields[i] = Grid.BitFields[i] & 65407;
				ushort[] bitFields = Grid.BitFields;
				bitFields[i] |= ((!value) ? 0 : 128);
			}
		}
	}

	public struct FakeFloorIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.BitFields[i] & 2) != 0;
			}
			set
			{
				Grid.BitFields[i] = Grid.BitFields[i] & 65533;
				ushort[] bitFields = Grid.BitFields;
				bitFields[i] |= ((!value) ? 0 : 2);
			}
		}
	}

	public struct LiquidPumpFloorIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.BitFields[i] & 2) != 0;
			}
			set
			{
				Grid.BitFields[i] = Grid.BitFields[i] & 65023;
				ushort[] bitFields = Grid.BitFields;
				bitFields[i] |= ((!value) ? 0 : 512);
			}
		}
	}

	public struct ForceFieldIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.BitFields[i] & 4) != 0;
			}
			set
			{
				Grid.BitFields[i] = Grid.BitFields[i] & 65531;
				ushort[] bitFields = Grid.BitFields;
				bitFields[i] |= ((!value) ? 0 : 4);
			}
		}
	}

	public struct ImpassableIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.BitFields[i] & 256) != 0;
			}
			set
			{
				Grid.BitFields[i] = Grid.BitFields[i] & 65279;
				ushort[] bitFields = Grid.BitFields;
				bitFields[i] |= ((!value) ? 0 : 256);
			}
		}
	}
}
