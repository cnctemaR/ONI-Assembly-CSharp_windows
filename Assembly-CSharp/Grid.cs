using System;
using System.Collections.Generic;
using System.Diagnostics;
using ProcGen;
using UnityEngine;

public class Grid
{
	private static void UpdateBuildMask(int i, Grid.BuildFlags flag, bool state)
	{
		if (state)
		{
			Grid.BuildMasks[i] |= flag;
			return;
		}
		Grid.BuildMasks[i] &= ~flag;
	}

	public static void SetSolid(int cell, bool solid, CellSolidEvent ev)
	{
		Grid.UpdateBuildMask(cell, Grid.BuildFlags.Solid, solid);
	}

	private static void UpdateVisMask(int i, Grid.VisFlags flag, bool state)
	{
		if (state)
		{
			Grid.VisMasks[i] |= flag;
			return;
		}
		Grid.VisMasks[i] &= ~flag;
	}

	private static void UpdateNavValidatorMask(int i, Grid.NavValidatorFlags flag, bool state)
	{
		if (state)
		{
			Grid.NavValidatorMasks[i] |= flag;
			return;
		}
		Grid.NavValidatorMasks[i] &= ~flag;
	}

	private static void UpdateNavMask(int i, Grid.NavFlags flag, bool state)
	{
		if (state)
		{
			Grid.NavMasks[i] |= flag;
			return;
		}
		Grid.NavMasks[i] &= ~flag;
	}

	public static void ResetNavMasksAndDetails()
	{
		Grid.NavMasks = null;
		Grid.tubeEntrances.Clear();
		Grid.restrictions.Clear();
		Grid.suitMarkers.Clear();
	}

	public static bool DEBUG_GetRestrictions(int cell, out Grid.Restriction restriction)
	{
		return Grid.restrictions.TryGetValue(cell, out restriction);
	}

	public static void RegisterRestriction(int cell, Grid.Restriction.Orientation orientation)
	{
		Grid.HasAccessDoor[cell] = true;
		Grid.restrictions[cell] = new Grid.Restriction
		{
			DirectionMasksForMinionInstanceID = new Dictionary<int, Grid.Restriction.Directions>(),
			orientation = orientation
		};
	}

	public static void UnregisterRestriction(int cell)
	{
		Grid.restrictions.Remove(cell);
		Grid.HasAccessDoor[cell] = false;
	}

	public static void SetRestriction(int cell, int minionInstanceID, Grid.Restriction.Directions directions)
	{
		Grid.restrictions[cell].DirectionMasksForMinionInstanceID[minionInstanceID] = directions;
	}

	public static void ClearRestriction(int cell, int minionInstanceID)
	{
		Grid.restrictions[cell].DirectionMasksForMinionInstanceID.Remove(minionInstanceID);
	}

	public static bool HasPermission(int cell, int minionInstanceID, int fromCell, NavType fromNavType)
	{
		if (!Grid.HasAccessDoor[cell])
		{
			return true;
		}
		Grid.Restriction restriction = Grid.restrictions[cell];
		Vector2I vector2I = Grid.CellToXY(cell);
		Vector2I vector2I2 = Grid.CellToXY(fromCell);
		Grid.Restriction.Directions directions = (Grid.Restriction.Directions)0;
		int num = vector2I.x - vector2I2.x;
		int num2 = vector2I.y - vector2I2.y;
		switch (restriction.orientation)
		{
		case Grid.Restriction.Orientation.Vertical:
			if (num < 0)
			{
				directions |= Grid.Restriction.Directions.Left;
			}
			if (num > 0)
			{
				directions |= Grid.Restriction.Directions.Right;
			}
			break;
		case Grid.Restriction.Orientation.Horizontal:
			if (num2 > 0)
			{
				directions |= Grid.Restriction.Directions.Left;
			}
			if (num2 < 0)
			{
				directions |= Grid.Restriction.Directions.Right;
			}
			break;
		case Grid.Restriction.Orientation.SingleCell:
			if (Math.Abs(num) != 1 && Math.Abs(num2) != 1 && fromNavType != NavType.Teleport)
			{
				directions |= Grid.Restriction.Directions.Teleport;
			}
			break;
		}
		Grid.Restriction.Directions directions2 = (Grid.Restriction.Directions)0;
		return (!restriction.DirectionMasksForMinionInstanceID.TryGetValue(minionInstanceID, out directions2) && !restriction.DirectionMasksForMinionInstanceID.TryGetValue(-1, out directions2)) || (directions2 & directions) == (Grid.Restriction.Directions)0;
	}

	public static void RegisterTubeEntrance(int cell, int reservationCapacity)
	{
		DebugUtil.Assert(!Grid.tubeEntrances.ContainsKey(cell));
		Grid.HasTubeEntrance[cell] = true;
		Grid.tubeEntrances[cell] = new Grid.TubeEntrance
		{
			reservationCapacity = reservationCapacity,
			reservedInstanceIDs = new HashSet<int>()
		};
	}

	public static void UnregisterTubeEntrance(int cell)
	{
		DebugUtil.Assert(Grid.tubeEntrances.ContainsKey(cell));
		Grid.HasTubeEntrance[cell] = false;
		Grid.tubeEntrances.Remove(cell);
	}

	public static bool ReserveTubeEntrance(int cell, int minionInstanceID, bool reserve)
	{
		Grid.TubeEntrance tubeEntrance = Grid.tubeEntrances[cell];
		HashSet<int> reservedInstanceIDs = tubeEntrance.reservedInstanceIDs;
		if (!reserve)
		{
			return reservedInstanceIDs.Remove(minionInstanceID);
		}
		DebugUtil.Assert(Grid.HasTubeEntrance[cell]);
		if (reservedInstanceIDs.Count == tubeEntrance.reservationCapacity)
		{
			return false;
		}
		DebugUtil.Assert(reservedInstanceIDs.Add(minionInstanceID));
		return true;
	}

	public static void SetTubeEntranceReservationCapacity(int cell, int newReservationCapacity)
	{
		DebugUtil.Assert(Grid.HasTubeEntrance[cell]);
		Grid.TubeEntrance tubeEntrance = Grid.tubeEntrances[cell];
		tubeEntrance.reservationCapacity = newReservationCapacity;
		Grid.tubeEntrances[cell] = tubeEntrance;
	}

	public static bool HasUsableTubeEntrance(int cell, int minionInstanceID)
	{
		if (!Grid.HasTubeEntrance[cell])
		{
			return false;
		}
		Grid.TubeEntrance tubeEntrance = Grid.tubeEntrances[cell];
		if (!tubeEntrance.operational)
		{
			return false;
		}
		HashSet<int> reservedInstanceIDs = tubeEntrance.reservedInstanceIDs;
		return reservedInstanceIDs.Count < tubeEntrance.reservationCapacity || reservedInstanceIDs.Contains(minionInstanceID);
	}

	public static bool HasReservedTubeEntrance(int cell, int minionInstanceID)
	{
		DebugUtil.Assert(Grid.HasTubeEntrance[cell]);
		return Grid.tubeEntrances[cell].reservedInstanceIDs.Contains(minionInstanceID);
	}

	public static void SetTubeEntranceOperational(int cell, bool operational)
	{
		DebugUtil.Assert(Grid.HasTubeEntrance[cell]);
		Grid.TubeEntrance tubeEntrance = Grid.tubeEntrances[cell];
		tubeEntrance.operational = operational;
		Grid.tubeEntrances[cell] = tubeEntrance;
	}

	public static void RegisterSuitMarker(int cell)
	{
		DebugUtil.Assert(!Grid.HasSuitMarker[cell]);
		Grid.HasSuitMarker[cell] = true;
		Grid.suitMarkers[cell] = new Grid.SuitMarker
		{
			suitCount = 0,
			lockerCount = 0,
			flags = Grid.SuitMarker.Flags.Operational,
			minionIDsWithSuitReservations = new HashSet<int>(),
			minionIDsWithEmptyLockerReservations = new HashSet<int>()
		};
	}

	public static void UnregisterSuitMarker(int cell)
	{
		DebugUtil.Assert(Grid.HasSuitMarker[cell]);
		Grid.HasSuitMarker[cell] = false;
		Grid.suitMarkers.Remove(cell);
	}

	public static bool ReserveSuit(int cell, int minionInstanceID, bool reserve)
	{
		DebugUtil.Assert(Grid.HasSuitMarker[cell]);
		Grid.SuitMarker suitMarker = Grid.suitMarkers[cell];
		HashSet<int> minionIDsWithSuitReservations = suitMarker.minionIDsWithSuitReservations;
		if (!reserve)
		{
			bool flag = minionIDsWithSuitReservations.Remove(minionInstanceID);
			DebugUtil.Assert(flag);
			return flag;
		}
		if (minionIDsWithSuitReservations.Count >= suitMarker.suitCount)
		{
			return false;
		}
		DebugUtil.Assert(minionIDsWithSuitReservations.Add(minionInstanceID));
		return true;
	}

	public static bool ReserveEmptyLocker(int cell, int minionInstanceID, bool reserve)
	{
		DebugUtil.Assert(Grid.HasSuitMarker[cell]);
		Grid.SuitMarker suitMarker = Grid.suitMarkers[cell];
		HashSet<int> minionIDsWithEmptyLockerReservations = suitMarker.minionIDsWithEmptyLockerReservations;
		if (!reserve)
		{
			bool flag = minionIDsWithEmptyLockerReservations.Remove(minionInstanceID);
			DebugUtil.Assert(flag);
			return flag;
		}
		if (minionIDsWithEmptyLockerReservations.Count >= suitMarker.emptyLockerCount)
		{
			return false;
		}
		DebugUtil.Assert(minionIDsWithEmptyLockerReservations.Add(minionInstanceID));
		return true;
	}

	public static void UpdateSuitMarker(int cell, int fullLockerCount, int emptyLockerCount, Grid.SuitMarker.Flags flags, PathFinder.PotentialPath.Flags pathFlags)
	{
		DebugUtil.Assert(Grid.HasSuitMarker[cell]);
		Grid.SuitMarker suitMarker = Grid.suitMarkers[cell];
		suitMarker.suitCount = fullLockerCount;
		suitMarker.lockerCount = fullLockerCount + emptyLockerCount;
		suitMarker.flags = flags;
		suitMarker.pathFlags = pathFlags;
		Grid.suitMarkers[cell] = suitMarker;
	}

	public static bool TryGetSuitMarkerFlags(int cell, out Grid.SuitMarker.Flags flags, out PathFinder.PotentialPath.Flags pathFlags)
	{
		if (Grid.HasSuitMarker[cell])
		{
			flags = Grid.suitMarkers[cell].flags;
			pathFlags = Grid.suitMarkers[cell].pathFlags;
			return true;
		}
		flags = (Grid.SuitMarker.Flags)0;
		pathFlags = PathFinder.PotentialPath.Flags.None;
		return false;
	}

	public static bool HasSuit(int cell, int minionInstanceID)
	{
		if (!Grid.HasSuitMarker[cell])
		{
			return false;
		}
		Grid.SuitMarker suitMarker = Grid.suitMarkers[cell];
		HashSet<int> minionIDsWithSuitReservations = suitMarker.minionIDsWithSuitReservations;
		return minionIDsWithSuitReservations.Count < suitMarker.suitCount || minionIDsWithSuitReservations.Contains(minionInstanceID);
	}

	public static bool HasEmptyLocker(int cell, int minionInstanceID)
	{
		if (!Grid.HasSuitMarker[cell])
		{
			return false;
		}
		Grid.SuitMarker suitMarker = Grid.suitMarkers[cell];
		HashSet<int> minionIDsWithEmptyLockerReservations = suitMarker.minionIDsWithEmptyLockerReservations;
		return minionIDsWithEmptyLockerReservations.Count < suitMarker.emptyLockerCount || minionIDsWithEmptyLockerReservations.Contains(minionInstanceID);
	}

	public unsafe static void InitializeCells()
	{
		for (int num = 0; num != Grid.WidthInCells * Grid.HeightInCells; num++)
		{
			ushort num2 = Grid.elementIdx[num];
			Element element = ElementLoader.elements[(int)num2];
			Grid.Element[num] = element;
			if (element.IsSolid)
			{
				Grid.BuildMasks[num] |= Grid.BuildFlags.Solid;
			}
			else
			{
				Grid.BuildMasks[num] &= ~Grid.BuildFlags.Solid;
			}
			Grid.RenderedByWorld[num] = element.substance != null && element.substance.renderedByWorld && Grid.Objects[num, 9] == null;
		}
	}

	public static bool IsInitialized()
	{
		return Grid.mass != null;
	}

	public static int GetCellInDirection(int cell, Direction d)
	{
		switch (d)
		{
		case Direction.Up:
			return Grid.CellAbove(cell);
		case Direction.Right:
			return Grid.CellRight(cell);
		case Direction.Down:
			return Grid.CellBelow(cell);
		case Direction.Left:
			return Grid.CellLeft(cell);
		case Direction.None:
			return cell;
		}
		return -1;
	}

	public static bool Raycast(int cell, Vector2I direction, out int hitDistance, int maxDistance = 100, Grid.BuildFlags layerMask = Grid.BuildFlags.Any)
	{
		bool flag = false;
		Vector2I vector2I = Grid.CellToXY(cell);
		Vector2I vector2I2 = vector2I + direction * maxDistance;
		int num = cell;
		int num2 = Grid.XYToCell(vector2I2.x, vector2I2.y);
		int num3 = 0;
		int num4 = 0;
		float num5 = (float)maxDistance * 0.5f;
		while ((float)num3 < num5)
		{
			if (!Grid.IsValidCell(num) || (Grid.BuildMasks[num] & layerMask) != ~(Grid.BuildFlags.Solid | Grid.BuildFlags.Foundation | Grid.BuildFlags.Door | Grid.BuildFlags.DupePassable | Grid.BuildFlags.DupeImpassable | Grid.BuildFlags.CritterImpassable | Grid.BuildFlags.FakeFloor))
			{
				flag = true;
				break;
			}
			if (!Grid.IsValidCell(num2) || (Grid.BuildMasks[num2] & layerMask) != ~(Grid.BuildFlags.Solid | Grid.BuildFlags.Foundation | Grid.BuildFlags.Door | Grid.BuildFlags.DupePassable | Grid.BuildFlags.DupeImpassable | Grid.BuildFlags.CritterImpassable | Grid.BuildFlags.FakeFloor))
			{
				num4 = maxDistance - num3;
			}
			vector2I += direction;
			vector2I2 -= direction;
			num = Grid.XYToCell(vector2I.x, vector2I.y);
			num2 = Grid.XYToCell(vector2I2.x, vector2I2.y);
			num3++;
		}
		if (!flag && maxDistance % 2 == 0)
		{
			flag = !Grid.IsValidCell(num2) || (Grid.BuildMasks[num2] & layerMask) > ~(Grid.BuildFlags.Solid | Grid.BuildFlags.Foundation | Grid.BuildFlags.Door | Grid.BuildFlags.DupePassable | Grid.BuildFlags.DupeImpassable | Grid.BuildFlags.CritterImpassable | Grid.BuildFlags.FakeFloor);
		}
		hitDistance = (flag ? num3 : ((num4 > 0) ? num4 : maxDistance));
		return flag | (hitDistance == num4);
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
		if (cell % Grid.WidthInCells <= 0)
		{
			return -1;
		}
		return cell - 1;
	}

	public static int CellRight(int cell)
	{
		if (cell % Grid.WidthInCells >= Grid.WidthInCells - 1)
		{
			return -1;
		}
		return cell + 1;
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

	public static int GetCellRange(int cell_a, int cell_b)
	{
		CellOffset offset = Grid.GetOffset(cell_a, cell_b);
		return Math.Max(Math.Abs(offset.x), Math.Abs(offset.y));
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

	public static CellOffset GetCellOffsetDirection(int base_cell, int offset_cell)
	{
		CellOffset offset = Grid.GetOffset(base_cell, offset_cell);
		offset.x = Mathf.Clamp(offset.x, -1, 1);
		offset.y = Mathf.Clamp(offset.y, -1, 1);
		return offset;
	}

	public static int OffsetCell(int cell, CellOffset offset)
	{
		return cell + offset.x + offset.y * Grid.WidthInCells;
	}

	public static int OffsetCell(int cell, int x, int y)
	{
		return cell + x + y * Grid.WidthInCells;
	}

	public static bool IsCellOffsetValid(int cell, int x, int y)
	{
		int num;
		int num2;
		Grid.CellToXY(cell, out num, out num2);
		return num + x >= 0 && num + x < Grid.WidthInCells && num2 + y >= 0 && num2 + y < Grid.HeightInCells;
	}

	public static bool IsCellOffsetValid(int cell, CellOffset offset)
	{
		return Grid.IsCellOffsetValid(cell, offset.x, offset.y);
	}

	public static int PosToCell(StateMachine.Instance smi)
	{
		return Grid.PosToCell(smi.transform.GetPosition());
	}

	public static int PosToCell(GameObject go)
	{
		return Grid.PosToCell(go.transform.GetPosition());
	}

	public static int PosToCell(KMonoBehaviour cmp)
	{
		return Grid.PosToCell(cmp.transform.GetPosition());
	}

	public static bool IsValidBuildingCell(int cell)
	{
		if (!Grid.IsWorldValidCell(cell))
		{
			return false;
		}
		WorldContainer world = ClusterManager.Instance.GetWorld((int)Grid.WorldIdx[cell]);
		if (world == null)
		{
			return false;
		}
		Vector2I vector2I = Grid.CellToXY(cell);
		return (float)vector2I.x >= world.minimumBounds.x && (float)vector2I.x <= world.maximumBounds.x && (float)vector2I.y >= world.minimumBounds.y && (float)vector2I.y <= world.maximumBounds.y - (float)Grid.TopBorderHeight;
	}

	public static bool IsWorldValidCell(int cell)
	{
		return Grid.IsValidCell(cell) && Grid.WorldIdx[cell] != byte.MaxValue;
	}

	public static bool IsValidCell(int cell)
	{
		return cell >= 0 && cell < Grid.CellCount;
	}

	public static bool IsValidCellInWorld(int cell, int world)
	{
		return cell >= 0 && cell < Grid.CellCount && (int)Grid.WorldIdx[cell] == world;
	}

	public static bool IsActiveWorld(int cell)
	{
		return ClusterManager.Instance != null && ClusterManager.Instance.activeWorldId == (int)Grid.WorldIdx[cell];
	}

	public static bool AreCellsInSameWorld(int cell, int world_cell)
	{
		return Grid.IsValidCell(cell) && Grid.IsValidCell(world_cell) && Grid.WorldIdx[cell] == Grid.WorldIdx[world_cell];
	}

	public static bool IsCellOpenToSpace(int cell)
	{
		return !Grid.IsSolidCell(cell) && !(Grid.Objects[cell, 2] != null) && global::World.Instance.zoneRenderData.GetSubWorldZoneType(cell) == SubWorld.ZoneType.Space;
	}

	public static int PosToCell(Vector2 pos)
	{
		float x = pos.x;
		int num = (int)(pos.y + 0.05f);
		int num2 = (int)x;
		return num * Grid.WidthInCells + num2;
	}

	public static int PosToCell(Vector3 pos)
	{
		float x = pos.x;
		int num = (int)(pos.y + 0.05f);
		int num2 = (int)x;
		return num * Grid.WidthInCells + num2;
	}

	public static void PosToXY(Vector3 pos, out int x, out int y)
	{
		Grid.CellToXY(Grid.PosToCell(pos), out x, out y);
	}

	public static void PosToXY(Vector3 pos, out Vector2I xy)
	{
		Grid.CellToXY(Grid.PosToCell(pos), out xy.x, out xy.y);
	}

	public static Vector2I PosToXY(Vector3 pos)
	{
		Vector2I vector2I;
		Grid.CellToXY(Grid.PosToCell(pos), out vector2I.x, out vector2I.y);
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

	public static void Reveal(int cell, byte visibility = 255, bool forceReveal = false)
	{
		bool flag = Grid.Spawnable[cell] == 0 && visibility > 0;
		Grid.Spawnable[cell] = Math.Max(visibility, Grid.Visible[cell]);
		if (forceReveal || !Grid.PreventFogOfWarReveal[cell])
		{
			Grid.Visible[cell] = Math.Max(visibility, Grid.Visible[cell]);
		}
		if (flag && Grid.OnReveal != null)
		{
			Grid.OnReveal(cell);
		}
	}

	public static ObjectLayer GetObjectLayerForConduitType(ConduitType conduit_type)
	{
		switch (conduit_type)
		{
		case ConduitType.Gas:
			return ObjectLayer.GasConduitConnection;
		case ConduitType.Liquid:
			return ObjectLayer.LiquidConduitConnection;
		case ConduitType.Solid:
			return ObjectLayer.SolidConduitConnection;
		default:
			throw new ArgumentException("Invalid value.", "conduit_type");
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

	public static bool IsSolidCell(int cell)
	{
		return Grid.IsValidCell(cell) && Grid.Solid[cell];
	}

	public unsafe static bool IsSubstantialLiquid(int cell, float threshold = 0.35f)
	{
		if (Grid.IsValidCell(cell))
		{
			ushort num = Grid.elementIdx[cell];
			if ((int)num < ElementLoader.elements.Count)
			{
				Element element = ElementLoader.elements[(int)num];
				if (element.IsLiquid && Grid.mass[cell] >= element.defaultValues.mass * threshold)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool IsVisiblyInLiquid(Vector2 pos)
	{
		int num = Grid.PosToCell(pos);
		if (Grid.IsValidCell(num) && Grid.IsLiquid(num))
		{
			int num2 = Grid.CellAbove(num);
			if (Grid.IsValidCell(num2) && Grid.IsLiquid(num2))
			{
				return true;
			}
			float num3 = Grid.Mass[num];
			float num4 = (float)((int)pos.y) - pos.y;
			if (num3 / 1000f <= num4)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsLiquid(int cell)
	{
		return ElementLoader.elements[(int)Grid.ElementIdx[cell]].IsLiquid;
	}

	public static bool IsGas(int cell)
	{
		return ElementLoader.elements[(int)Grid.ElementIdx[cell]].IsGas;
	}

	public static void GetVisibleExtents(out int min_x, out int min_y, out int max_x, out int max_y)
	{
		Vector3 vector = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.main.transform.GetPosition().z));
		Vector3 vector2 = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, Camera.main.transform.GetPosition().z));
		min_y = (int)vector2.y;
		max_y = (int)(vector.y + 0.5f);
		min_x = (int)vector2.x;
		max_x = (int)(vector.x + 0.5f);
	}

	public static void GetVisibleExtents(out Vector2I min, out Vector2I max)
	{
		Grid.GetVisibleExtents(out min.x, out min.y, out max.x, out max.y);
	}

	public static bool IsVisible(int cell)
	{
		return Grid.Visible[cell] > 0 || !PropertyTextures.IsFogOfWarEnabled;
	}

	public static bool VisibleBlockingCB(int cell)
	{
		return !Grid.Transparent[cell] && Grid.IsSolidCell(cell);
	}

	public static bool VisibilityTest(int x, int y, int x2, int y2, bool blocking_tile_visible = false)
	{
		return Grid.TestLineOfSight(x, y, x2, y2, Grid.VisibleBlockingDelegate, blocking_tile_visible, false);
	}

	public static bool VisibilityTest(int cell, int target_cell, bool blocking_tile_visible = false)
	{
		int num = 0;
		int num2 = 0;
		Grid.CellToXY(cell, out num, out num2);
		int num3 = 0;
		int num4 = 0;
		Grid.CellToXY(target_cell, out num3, out num4);
		return Grid.VisibilityTest(num, num2, num3, num4, blocking_tile_visible);
	}

	public static bool PhysicalBlockingCB(int cell)
	{
		return Grid.Solid[cell];
	}

	public static bool IsPhysicallyAccessible(int x, int y, int x2, int y2, bool blocking_tile_visible = false)
	{
		return Grid.TestLineOfSight(x, y, x2, y2, Grid.PhysicalBlockingDelegate, blocking_tile_visible, false);
	}

	public static void CollectCellsInLine(int startCell, int endCell, HashSet<int> outputCells)
	{
		int num = 2;
		int cellDistance = Grid.GetCellDistance(startCell, endCell);
		Vector2 vector = (Grid.CellToPos(endCell) - Grid.CellToPos(startCell)).normalized;
		for (float num2 = 0f; num2 < (float)cellDistance; num2 = Mathf.Min(num2 + 1f / (float)num, (float)cellDistance))
		{
			int num3 = Grid.PosToCell(Grid.CellToPos(startCell) + vector * num2);
			if (Grid.GetCellDistance(startCell, num3) <= cellDistance)
			{
				outputCells.Add(num3);
			}
		}
	}

	public static bool IsRangeExposedToSunlight(int cell, int scanRadius, CellOffset scanShape, out int cellsClear, int clearThreshold = 1)
	{
		cellsClear = 0;
		if (Grid.IsValidCell(cell) && (int)Grid.ExposedToSunlight[cell] >= clearThreshold)
		{
			cellsClear++;
		}
		bool flag = true;
		bool flag2 = true;
		int num = 1;
		while (num <= scanRadius && (flag || flag2))
		{
			int num2 = Grid.OffsetCell(cell, scanShape.x * num, scanShape.y * num);
			int num3 = Grid.OffsetCell(cell, -scanShape.x * num, scanShape.y * num);
			if (Grid.IsValidCell(num2) && (int)Grid.ExposedToSunlight[num2] >= clearThreshold)
			{
				cellsClear++;
			}
			if (Grid.IsValidCell(num3) && (int)Grid.ExposedToSunlight[num3] >= clearThreshold)
			{
				cellsClear++;
			}
			num++;
		}
		return cellsClear > 0;
	}

	public static bool TestLineOfSight(int x, int y, int x2, int y2, Func<int, bool> blocking_cb, Func<int, bool> blocking_tile_visible_cb, bool allow_invalid_cells = false)
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
			int num12 = Grid.XYToCell(x, y);
			if (!allow_invalid_cells && !Grid.IsValidCell(num12))
			{
				return false;
			}
			bool flag = blocking_cb(num12);
			if ((x != num || y != num2) && flag)
			{
				return blocking_tile_visible_cb(num12) && x == x2 && y == y2;
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

	public static bool TestLineOfSight(int x, int y, int x2, int y2, Func<int, bool> blocking_cb, bool blocking_tile_visible = false, bool allow_invalid_cells = false)
	{
		return Grid.TestLineOfSight(x, y, x2, y2, blocking_cb, (int c) => blocking_tile_visible, allow_invalid_cells);
	}

	public static bool GetFreeGridSpace(Vector2I size, out Vector2I offset)
	{
		Vector2I gridOffset = BestFit.GetGridOffset(ClusterManager.Instance.WorldContainers, size, out offset);
		if (gridOffset.X <= Grid.WidthInCells && gridOffset.Y <= Grid.HeightInCells)
		{
			SimMessages.SimDataResizeGridAndInitializeVacuumCells(gridOffset, size.x, size.y, offset.x, offset.y);
			Game.Instance.roomProber.Refresh();
			return true;
		}
		return false;
	}

	public static void FreeGridSpace(Vector2I size, Vector2I offset)
	{
		SimMessages.SimDataFreeCells(size.x, size.y, offset.x, offset.y);
		for (int i = offset.y; i < size.y + offset.y + 1; i++)
		{
			for (int j = offset.x - 1; j < size.x + offset.x + 1; j++)
			{
				int num = Grid.XYToCell(j, i);
				if (Grid.IsValidCell(num))
				{
					Grid.Element[num] = ElementLoader.FindElementByHash(SimHashes.Vacuum);
				}
			}
		}
		Game.Instance.roomProber.Refresh();
	}

	[Conditional("UNITY_EDITOR")]
	public static void DrawBoxOnCell(int cell, Color color, float offset = 0f)
	{
		Grid.CellToPos(cell) + new Vector3(0.5f, 0.5f, 0f);
	}

	public static readonly CellOffset[] DefaultOffset = new CellOffset[1];

	public static float WidthInMeters;

	public static float HeightInMeters;

	public static int WidthInCells;

	public static int HeightInCells;

	public static float CellSizeInMeters;

	public static float InverseCellSizeInMeters;

	public static float HalfCellSizeInMeters;

	public static int CellCount;

	public static int InvalidCell = -1;

	public static int TopBorderHeight = 2;

	public static Dictionary<int, GameObject>[] ObjectLayers;

	public static Action<int> OnReveal;

	public static Grid.BuildFlags[] BuildMasks;

	public static Grid.BuildFlagsFoundationIndexer Foundation;

	public static Grid.BuildFlagsSolidIndexer Solid;

	public static Grid.BuildFlagsDupeImpassableIndexer DupeImpassable;

	public static Grid.BuildFlagsFakeFloorIndexer FakeFloor;

	public static Grid.BuildFlagsDupePassableIndexer DupePassable;

	public static Grid.BuildFlagsImpassableIndexer CritterImpassable;

	public static Grid.BuildFlagsDoorIndexer HasDoor;

	public static Grid.VisFlags[] VisMasks;

	public static Grid.VisFlagsRevealedIndexer Revealed;

	public static Grid.VisFlagsPreventFogOfWarRevealIndexer PreventFogOfWarReveal;

	public static Grid.VisFlagsRenderedByWorldIndexer RenderedByWorld;

	public static Grid.VisFlagsAllowPathfindingIndexer AllowPathfinding;

	public static Grid.NavValidatorFlags[] NavValidatorMasks;

	public static Grid.NavValidatorFlagsLadderIndexer HasLadder;

	public static Grid.NavValidatorFlagsPoleIndexer HasPole;

	public static Grid.NavValidatorFlagsTubeIndexer HasTube;

	public static Grid.NavValidatorFlagsNavTeleporterIndexer HasNavTeleporter;

	public static Grid.NavValidatorFlagsUnderConstructionIndexer IsTileUnderConstruction;

	public static Grid.NavFlags[] NavMasks;

	private static Grid.NavFlagsAccessDoorIndexer HasAccessDoor;

	public static Grid.NavFlagsTubeEntranceIndexer HasTubeEntrance;

	public static Grid.NavFlagsPreventIdleTraversalIndexer PreventIdleTraversal;

	public static Grid.NavFlagsReservedIndexer Reserved;

	public static Grid.NavFlagsSuitMarkerIndexer HasSuitMarker;

	private static Dictionary<int, Grid.Restriction> restrictions = new Dictionary<int, Grid.Restriction>();

	private static Dictionary<int, Grid.TubeEntrance> tubeEntrances = new Dictionary<int, Grid.TubeEntrance>();

	private static Dictionary<int, Grid.SuitMarker> suitMarkers = new Dictionary<int, Grid.SuitMarker>();

	public unsafe static ushort* elementIdx;

	public unsafe static float* temperature;

	public unsafe static float* radiation;

	public unsafe static float* mass;

	public unsafe static byte* properties;

	public unsafe static byte* strengthInfo;

	public unsafe static byte* insulation;

	public unsafe static byte* diseaseIdx;

	public unsafe static int* diseaseCount;

	public unsafe static byte* exposedToSunlight;

	public unsafe static float* AccumulatedFlowValues = null;

	public static byte[] Visible;

	public static byte[] Spawnable;

	public static float[] Damage;

	public static float[] Decor;

	public static bool[] GravitasFacility;

	public static byte[] WorldIdx;

	public static float[] Loudness;

	public static Element[] Element;

	public static int[] LightCount;

	public static Grid.PressureIndexer Pressure;

	public static Grid.TransparentIndexer Transparent;

	public static Grid.ElementIdxIndexer ElementIdx;

	public static Grid.TemperatureIndexer Temperature;

	public static Grid.RadiationIndexer Radiation;

	public static Grid.MassIndexer Mass;

	public static Grid.PropertiesIndexer Properties;

	public static Grid.ExposedToSunlightIndexer ExposedToSunlight;

	public static Grid.StrengthInfoIndexer StrengthInfo;

	public static Grid.Insulationndexer Insulation;

	public static Grid.DiseaseIdxIndexer DiseaseIdx;

	public static Grid.DiseaseCountIndexer DiseaseCount;

	public static Grid.LightIntensityIndexer LightIntensity;

	public static Grid.AccumulatedFlowIndexer AccumulatedFlow;

	public static Grid.ObjectLayerIndexer Objects;

	public static float LayerMultiplier = 1f;

	private static readonly Func<int, bool> VisibleBlockingDelegate = (int cell) => Grid.VisibleBlockingCB(cell);

	private static readonly Func<int, bool> PhysicalBlockingDelegate = (int cell) => Grid.PhysicalBlockingCB(cell);

	[Flags]
	public enum BuildFlags : byte
	{
		Solid = 1,
		Foundation = 2,
		Door = 4,
		DupePassable = 8,
		DupeImpassable = 16,
		CritterImpassable = 32,
		FakeFloor = 192,
		Any = 255
	}

	public struct BuildFlagsFoundationIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.BuildMasks[i] & Grid.BuildFlags.Foundation) > ~(Grid.BuildFlags.Solid | Grid.BuildFlags.Foundation | Grid.BuildFlags.Door | Grid.BuildFlags.DupePassable | Grid.BuildFlags.DupeImpassable | Grid.BuildFlags.CritterImpassable | Grid.BuildFlags.FakeFloor);
			}
			set
			{
				Grid.UpdateBuildMask(i, Grid.BuildFlags.Foundation, value);
			}
		}
	}

	public struct BuildFlagsSolidIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.BuildMasks[i] & Grid.BuildFlags.Solid) > ~(Grid.BuildFlags.Solid | Grid.BuildFlags.Foundation | Grid.BuildFlags.Door | Grid.BuildFlags.DupePassable | Grid.BuildFlags.DupeImpassable | Grid.BuildFlags.CritterImpassable | Grid.BuildFlags.FakeFloor);
			}
		}
	}

	public struct BuildFlagsDupeImpassableIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.BuildMasks[i] & Grid.BuildFlags.DupeImpassable) > ~(Grid.BuildFlags.Solid | Grid.BuildFlags.Foundation | Grid.BuildFlags.Door | Grid.BuildFlags.DupePassable | Grid.BuildFlags.DupeImpassable | Grid.BuildFlags.CritterImpassable | Grid.BuildFlags.FakeFloor);
			}
			set
			{
				Grid.UpdateBuildMask(i, Grid.BuildFlags.DupeImpassable, value);
			}
		}
	}

	public struct BuildFlagsFakeFloorIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.BuildMasks[i] & Grid.BuildFlags.FakeFloor) > ~(Grid.BuildFlags.Solid | Grid.BuildFlags.Foundation | Grid.BuildFlags.Door | Grid.BuildFlags.DupePassable | Grid.BuildFlags.DupeImpassable | Grid.BuildFlags.CritterImpassable | Grid.BuildFlags.FakeFloor);
			}
		}

		public void Add(int i)
		{
			Grid.BuildFlags buildFlags = Grid.BuildMasks[i];
			int num = (int)(((buildFlags & Grid.BuildFlags.FakeFloor) >> 6) + 1);
			num = Math.Min(num, 3);
			Grid.BuildMasks[i] = (buildFlags & ~Grid.BuildFlags.FakeFloor) | ((Grid.BuildFlags)(num << 6) & Grid.BuildFlags.FakeFloor);
		}

		public void Remove(int i)
		{
			Grid.BuildFlags buildFlags = Grid.BuildMasks[i];
			int num = (int)(((buildFlags & Grid.BuildFlags.FakeFloor) >> 6) - Grid.BuildFlags.Solid);
			num = Math.Max(num, 0);
			Grid.BuildMasks[i] = (buildFlags & ~Grid.BuildFlags.FakeFloor) | ((Grid.BuildFlags)(num << 6) & Grid.BuildFlags.FakeFloor);
		}
	}

	public struct BuildFlagsDupePassableIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.BuildMasks[i] & Grid.BuildFlags.DupePassable) > ~(Grid.BuildFlags.Solid | Grid.BuildFlags.Foundation | Grid.BuildFlags.Door | Grid.BuildFlags.DupePassable | Grid.BuildFlags.DupeImpassable | Grid.BuildFlags.CritterImpassable | Grid.BuildFlags.FakeFloor);
			}
			set
			{
				Grid.UpdateBuildMask(i, Grid.BuildFlags.DupePassable, value);
			}
		}
	}

	public struct BuildFlagsImpassableIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.BuildMasks[i] & Grid.BuildFlags.CritterImpassable) > ~(Grid.BuildFlags.Solid | Grid.BuildFlags.Foundation | Grid.BuildFlags.Door | Grid.BuildFlags.DupePassable | Grid.BuildFlags.DupeImpassable | Grid.BuildFlags.CritterImpassable | Grid.BuildFlags.FakeFloor);
			}
			set
			{
				Grid.UpdateBuildMask(i, Grid.BuildFlags.CritterImpassable, value);
			}
		}
	}

	public struct BuildFlagsDoorIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.BuildMasks[i] & Grid.BuildFlags.Door) > ~(Grid.BuildFlags.Solid | Grid.BuildFlags.Foundation | Grid.BuildFlags.Door | Grid.BuildFlags.DupePassable | Grid.BuildFlags.DupeImpassable | Grid.BuildFlags.CritterImpassable | Grid.BuildFlags.FakeFloor);
			}
			set
			{
				Grid.UpdateBuildMask(i, Grid.BuildFlags.Door, value);
			}
		}
	}

	[Flags]
	public enum VisFlags : byte
	{
		Revealed = 1,
		PreventFogOfWarReveal = 2,
		RenderedByWorld = 4,
		AllowPathfinding = 8
	}

	public struct VisFlagsRevealedIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.VisMasks[i] & Grid.VisFlags.Revealed) > (Grid.VisFlags)0;
			}
			set
			{
				Grid.UpdateVisMask(i, Grid.VisFlags.Revealed, value);
			}
		}
	}

	public struct VisFlagsPreventFogOfWarRevealIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.VisMasks[i] & Grid.VisFlags.PreventFogOfWarReveal) > (Grid.VisFlags)0;
			}
			set
			{
				Grid.UpdateVisMask(i, Grid.VisFlags.PreventFogOfWarReveal, value);
			}
		}
	}

	public struct VisFlagsRenderedByWorldIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.VisMasks[i] & Grid.VisFlags.RenderedByWorld) > (Grid.VisFlags)0;
			}
			set
			{
				Grid.UpdateVisMask(i, Grid.VisFlags.RenderedByWorld, value);
			}
		}
	}

	public struct VisFlagsAllowPathfindingIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.VisMasks[i] & Grid.VisFlags.AllowPathfinding) > (Grid.VisFlags)0;
			}
			set
			{
				Grid.UpdateVisMask(i, Grid.VisFlags.AllowPathfinding, value);
			}
		}
	}

	[Flags]
	public enum NavValidatorFlags : byte
	{
		Ladder = 1,
		Pole = 2,
		Tube = 4,
		NavTeleporter = 8,
		UnderConstruction = 16
	}

	public struct NavValidatorFlagsLadderIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.NavValidatorMasks[i] & Grid.NavValidatorFlags.Ladder) > (Grid.NavValidatorFlags)0;
			}
			set
			{
				Grid.UpdateNavValidatorMask(i, Grid.NavValidatorFlags.Ladder, value);
			}
		}
	}

	public struct NavValidatorFlagsPoleIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.NavValidatorMasks[i] & Grid.NavValidatorFlags.Pole) > (Grid.NavValidatorFlags)0;
			}
			set
			{
				Grid.UpdateNavValidatorMask(i, Grid.NavValidatorFlags.Pole, value);
			}
		}
	}

	public struct NavValidatorFlagsTubeIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.NavValidatorMasks[i] & Grid.NavValidatorFlags.Tube) > (Grid.NavValidatorFlags)0;
			}
			set
			{
				Grid.UpdateNavValidatorMask(i, Grid.NavValidatorFlags.Tube, value);
			}
		}
	}

	public struct NavValidatorFlagsNavTeleporterIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.NavValidatorMasks[i] & Grid.NavValidatorFlags.NavTeleporter) > (Grid.NavValidatorFlags)0;
			}
			set
			{
				Grid.UpdateNavValidatorMask(i, Grid.NavValidatorFlags.NavTeleporter, value);
			}
		}
	}

	public struct NavValidatorFlagsUnderConstructionIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.NavValidatorMasks[i] & Grid.NavValidatorFlags.UnderConstruction) > (Grid.NavValidatorFlags)0;
			}
			set
			{
				Grid.UpdateNavValidatorMask(i, Grid.NavValidatorFlags.UnderConstruction, value);
			}
		}
	}

	[Flags]
	public enum NavFlags : byte
	{
		AccessDoor = 1,
		TubeEntrance = 2,
		PreventIdleTraversal = 4,
		Reserved = 8,
		SuitMarker = 16
	}

	public struct NavFlagsAccessDoorIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.NavMasks[i] & Grid.NavFlags.AccessDoor) > (Grid.NavFlags)0;
			}
			set
			{
				Grid.UpdateNavMask(i, Grid.NavFlags.AccessDoor, value);
			}
		}
	}

	public struct NavFlagsTubeEntranceIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.NavMasks[i] & Grid.NavFlags.TubeEntrance) > (Grid.NavFlags)0;
			}
			set
			{
				Grid.UpdateNavMask(i, Grid.NavFlags.TubeEntrance, value);
			}
		}
	}

	public struct NavFlagsPreventIdleTraversalIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.NavMasks[i] & Grid.NavFlags.PreventIdleTraversal) > (Grid.NavFlags)0;
			}
			set
			{
				Grid.UpdateNavMask(i, Grid.NavFlags.PreventIdleTraversal, value);
			}
		}
	}

	public struct NavFlagsReservedIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.NavMasks[i] & Grid.NavFlags.Reserved) > (Grid.NavFlags)0;
			}
			set
			{
				Grid.UpdateNavMask(i, Grid.NavFlags.Reserved, value);
			}
		}
	}

	public struct NavFlagsSuitMarkerIndexer
	{
		public bool this[int i]
		{
			get
			{
				return (Grid.NavMasks[i] & Grid.NavFlags.SuitMarker) > (Grid.NavFlags)0;
			}
			set
			{
				Grid.UpdateNavMask(i, Grid.NavFlags.SuitMarker, value);
			}
		}
	}

	public struct Restriction
	{
		public const int DefaultID = -1;

		public Dictionary<int, Grid.Restriction.Directions> DirectionMasksForMinionInstanceID;

		public Grid.Restriction.Orientation orientation;

		[Flags]
		public enum Directions : byte
		{
			Left = 1,
			Right = 2,
			Teleport = 4
		}

		public enum Orientation : byte
		{
			Vertical,
			Horizontal,
			SingleCell
		}
	}

	private struct TubeEntrance
	{
		public bool operational;

		public int reservationCapacity;

		public HashSet<int> reservedInstanceIDs;
	}

	public struct SuitMarker
	{
		public int emptyLockerCount
		{
			get
			{
				return this.lockerCount - this.suitCount;
			}
		}

		public int suitCount;

		public int lockerCount;

		public Grid.SuitMarker.Flags flags;

		public PathFinder.PotentialPath.Flags pathFlags;

		public HashSet<int> minionIDsWithSuitReservations;

		public HashSet<int> minionIDsWithEmptyLockerReservations;

		[Flags]
		public enum Flags : byte
		{
			OnlyTraverseIfUnequipAvailable = 1,
			Operational = 2,
			Rotated = 4
		}
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

	public struct PressureIndexer
	{
		public unsafe float this[int i]
		{
			get
			{
				return Grid.mass[i] * 101.3f;
			}
		}
	}

	public struct TransparentIndexer
	{
		public unsafe bool this[int i]
		{
			get
			{
				return (Grid.properties[i] & 16) > 0;
			}
		}
	}

	public struct ElementIdxIndexer
	{
		public unsafe ushort this[int i]
		{
			get
			{
				return Grid.elementIdx[i];
			}
		}
	}

	public struct TemperatureIndexer
	{
		public unsafe float this[int i]
		{
			get
			{
				return Grid.temperature[i];
			}
		}
	}

	public struct RadiationIndexer
	{
		public unsafe float this[int i]
		{
			get
			{
				return Grid.radiation[i];
			}
		}
	}

	public struct MassIndexer
	{
		public unsafe float this[int i]
		{
			get
			{
				return Grid.mass[i];
			}
		}
	}

	public struct PropertiesIndexer
	{
		public unsafe byte this[int i]
		{
			get
			{
				return Grid.properties[i];
			}
		}
	}

	public struct ExposedToSunlightIndexer
	{
		public unsafe byte this[int i]
		{
			get
			{
				return Grid.exposedToSunlight[i];
			}
		}
	}

	public struct StrengthInfoIndexer
	{
		public unsafe byte this[int i]
		{
			get
			{
				return Grid.strengthInfo[i];
			}
		}
	}

	public struct Insulationndexer
	{
		public unsafe byte this[int i]
		{
			get
			{
				return Grid.insulation[i];
			}
		}
	}

	public struct DiseaseIdxIndexer
	{
		public unsafe byte this[int i]
		{
			get
			{
				return Grid.diseaseIdx[i];
			}
		}
	}

	public struct DiseaseCountIndexer
	{
		public unsafe int this[int i]
		{
			get
			{
				return Grid.diseaseCount[i];
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

	public struct LightIntensityIndexer
	{
		public unsafe int this[int i]
		{
			get
			{
				float num = Game.Instance.currentFallbackSunlightIntensity;
				WorldContainer world = ClusterManager.Instance.GetWorld((int)Grid.WorldIdx[i]);
				if (world != null)
				{
					num = world.currentSunlightIntensity;
				}
				int num2 = (int)((float)Grid.exposedToSunlight[i] / 255f * num);
				int num3 = Grid.LightCount[i];
				return num2 + num3;
			}
		}
	}

	public enum SceneLayer
	{
		WorldSelection = -3,
		NoLayer,
		Background,
		Backwall = 1,
		Gas,
		GasConduits,
		GasConduitBridges,
		LiquidConduits,
		LiquidConduitBridges,
		SolidConduits,
		SolidConduitContents,
		SolidConduitBridges,
		Wires,
		WireBridges,
		WireBridgesFront,
		LogicWires,
		LogicGates,
		LogicGatesFront,
		InteriorWall,
		GasFront,
		BuildingBack,
		Building,
		BuildingUse,
		BuildingFront,
		TransferArm,
		Ore,
		Creatures,
		Move,
		Front,
		GlassTile,
		Liquid,
		Ground,
		TileMain,
		TileFront,
		FXFront,
		FXFront2,
		SceneMAX
	}
}
