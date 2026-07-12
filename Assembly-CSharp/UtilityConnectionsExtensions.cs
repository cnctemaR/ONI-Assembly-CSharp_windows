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
		case UtilityConnections.Left | UtilityConnections.Right:
			break;
		case UtilityConnections.Up:
			return UtilityConnections.Down;
		default:
			if (direction == UtilityConnections.Down)
			{
				return UtilityConnections.Up;
			}
			break;
		}
		throw new ArgumentException("Unexpected enum value: " + direction.ToString(), "direction");
	}

	public static UtilityConnections LeftDirection(this UtilityConnections direction)
	{
		switch (direction)
		{
		case UtilityConnections.Left:
			return UtilityConnections.Down;
		case UtilityConnections.Right:
			return UtilityConnections.Up;
		case UtilityConnections.Left | UtilityConnections.Right:
			break;
		case UtilityConnections.Up:
			return UtilityConnections.Left;
		default:
			if (direction == UtilityConnections.Down)
			{
				return UtilityConnections.Right;
			}
			break;
		}
		throw new ArgumentException("Unexpected enum value: " + direction.ToString(), "direction");
	}

	public static UtilityConnections RightDirection(this UtilityConnections direction)
	{
		switch (direction)
		{
		case UtilityConnections.Left:
			return UtilityConnections.Up;
		case UtilityConnections.Right:
			return UtilityConnections.Down;
		case UtilityConnections.Left | UtilityConnections.Right:
			break;
		case UtilityConnections.Up:
			return UtilityConnections.Right;
		default:
			if (direction == UtilityConnections.Down)
			{
				return UtilityConnections.Left;
			}
			break;
		}
		throw new ArgumentException("Unexpected enum value: " + direction.ToString(), "direction");
	}

	public static int CellInDirection(this UtilityConnections direction, int from_cell)
	{
		switch (direction)
		{
		case UtilityConnections.Left:
			return from_cell - 1;
		case UtilityConnections.Right:
			return from_cell + 1;
		case UtilityConnections.Left | UtilityConnections.Right:
			break;
		case UtilityConnections.Up:
			return from_cell + Grid.WidthInCells;
		default:
			if (direction == UtilityConnections.Down)
			{
				return from_cell - Grid.WidthInCells;
			}
			break;
		}
		throw new ArgumentException("Unexpected enum value: " + direction.ToString(), "direction");
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
