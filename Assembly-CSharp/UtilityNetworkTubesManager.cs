using System;
using STRINGS;
using UnityEngine;

public class UtilityNetworkTubesManager : UtilityNetworkManager<TravelTubeNetwork, TravelTube>
{
	public UtilityNetworkTubesManager(int game_width, int game_height, int tile_layer)
		: base(game_width, game_height, tile_layer)
	{
	}

	public override bool CanAddConnection(UtilityConnections new_connection, int cell, bool is_physical_building, out string fail_reason)
	{
		return this.TestForUTurnLeft(cell, new_connection, is_physical_building, out fail_reason) && this.TestForUTurnRight(cell, new_connection, is_physical_building, out fail_reason) && this.TestForNoAdjacentBridge(cell, new_connection, out fail_reason);
	}

	public override void SetConnections(UtilityConnections connections, int cell, bool is_physical_building)
	{
		base.SetConnections(connections, cell, is_physical_building);
		Pathfinding.Instance.AddDirtyNavGridCell(cell);
	}

	private bool TestForUTurnLeft(int first_cell, UtilityConnections first_connection, bool is_physical_building, out string fail_reason)
	{
		int num = first_cell;
		UtilityConnections utilityConnections = first_connection;
		int num2 = 1;
		for (int i = 0; i < 3; i++)
		{
			int num3 = utilityConnections.CellInDirection(num);
			UtilityConnections utilityConnections2 = utilityConnections.LeftDirection();
			if (this.HasConnection(num3, utilityConnections2, is_physical_building))
			{
				num2++;
			}
			num = num3;
			utilityConnections = utilityConnections2;
		}
		fail_reason = UI.TOOLTIPS.HELP_TUBELOCATION_NO_UTURNS;
		return num2 <= 2;
	}

	private bool TestForUTurnRight(int first_cell, UtilityConnections first_connection, bool is_physical_building, out string fail_reason)
	{
		int num = first_cell;
		UtilityConnections utilityConnections = first_connection;
		int num2 = 1;
		for (int i = 0; i < 3; i++)
		{
			int num3 = utilityConnections.CellInDirection(num);
			UtilityConnections utilityConnections2 = utilityConnections.RightDirection();
			if (this.HasConnection(num3, utilityConnections2, is_physical_building))
			{
				num2++;
			}
			num = num3;
			utilityConnections = utilityConnections2;
		}
		fail_reason = UI.TOOLTIPS.HELP_TUBELOCATION_NO_UTURNS;
		return num2 <= 2;
	}

	private bool TestForNoAdjacentBridge(int cell, UtilityConnections connection, out string fail_reason)
	{
		UtilityConnections utilityConnections = connection.LeftDirection();
		UtilityConnections utilityConnections2 = connection.RightDirection();
		int num = utilityConnections.CellInDirection(cell);
		int num2 = utilityConnections2.CellInDirection(cell);
		GameObject gameObject = Grid.Objects[num, 9];
		GameObject gameObject2 = Grid.Objects[num2, 9];
		fail_reason = UI.TOOLTIPS.HELP_TUBELOCATION_STRAIGHT_BRIDGES;
		return (gameObject == null || gameObject.GetComponent<TravelTubeBridge>() == null) && (gameObject2 == null || gameObject2.GetComponent<TravelTubeBridge>() == null);
	}

	private bool HasConnection(int cell, UtilityConnections connection, bool is_physical_building)
	{
		return (base.GetConnections(cell, is_physical_building) & connection) != (UtilityConnections)0;
	}
}
