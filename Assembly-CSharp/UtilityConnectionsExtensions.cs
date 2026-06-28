using System;

public static class UtilityConnectionsExtensions
{
	public static UtilityConnections InverseDirection(this UtilityConnections direction)
	{
		switch (direction)
		{
		case UtilityConnections.Left:
			return UtilityConnections.Right;
		case UtilityConnections.Right:
			return UtilityConnections.Left;
		case UtilityConnections.Up:
			return UtilityConnections.Down;
		case UtilityConnections.Down:
			return UtilityConnections.Up;
		}
		throw new ArgumentException("Unexpected enum value: " + direction, "direction");
	}

	public static UtilityConnections LeftDirection(this UtilityConnections direction)
	{
		switch (direction)
		{
		case UtilityConnections.Left:
			return UtilityConnections.Down;
		case UtilityConnections.Right:
			return UtilityConnections.Up;
		case UtilityConnections.Up:
			return UtilityConnections.Left;
		case UtilityConnections.Down:
			return UtilityConnections.Right;
		}
		throw new ArgumentException("Unexpected enum value: " + direction, "direction");
	}

	public static UtilityConnections RightDirection(this UtilityConnections direction)
	{
		switch (direction)
		{
		case UtilityConnections.Left:
			return UtilityConnections.Up;
		case UtilityConnections.Right:
			return UtilityConnections.Down;
		case UtilityConnections.Up:
			return UtilityConnections.Right;
		case UtilityConnections.Down:
			return UtilityConnections.Left;
		}
		throw new ArgumentException("Unexpected enum value: " + direction, "direction");
	}

	public static int CellInDirection(this UtilityConnections direction, int from_cell)
	{
		switch (direction)
		{
		case UtilityConnections.Left:
			return from_cell - 1;
		case UtilityConnections.Right:
			return from_cell + 1;
		case UtilityConnections.Up:
			return from_cell + Grid.WidthInCells;
		case UtilityConnections.Down:
			return from_cell - Grid.WidthInCells;
		}
		throw new ArgumentException("Unexpected enum value: " + direction, "direction");
	}

	public static UtilityConnections DirectionFromToCell(int from_cell, int to_cell)
	{
		if (to_cell == from_cell - 1)
		{
			return UtilityConnections.Left;
		}
		if (to_cell == from_cell + 1)
		{
			return UtilityConnections.Right;
		}
		if (to_cell == from_cell + Grid.WidthInCells)
		{
			return UtilityConnections.Up;
		}
		if (to_cell == from_cell - Grid.WidthInCells)
		{
			return UtilityConnections.Down;
		}
		return (UtilityConnections)0;
	}
}
