using System;

public interface IUtilityNetworkMgr
{
	void AddConnection(UtilityConnections new_connection, int cell, bool is_physical_building);

	void StashVisualGrids();

	void UnstashVisualGrids();

	string GetVisualizerString(int cell);

	string GetVisualizerString(UtilityConnections connections);

	UtilityConnections GetConnections(int cell, bool is_physical_building);

	UtilityConnections GetDisplayConnections(int cell);

	void SetConnections(UtilityConnections connections, int cell, bool is_physical_building);

	void ClearCell(int cell, bool is_physical_building);

	void ForceRebuildNetworks();

	void AddToNetworks(int cell, object item, bool is_endpoint);

	void RemoveFromNetworks(int cell, object vent, bool is_endpoint);

	object GetEndpoint(int cell);

	UtilityNetwork GetNetworkForDirection(int cell, Direction direction);

	UtilityNetwork GetNetworkForCell(int cell);

	ConduitFlow ConduitFlowManager { get; set; }
}
