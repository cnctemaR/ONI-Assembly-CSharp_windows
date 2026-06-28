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

	void AddToNetworks(int cell, FlowUtilityNetwork.IItem item);

	void RemoveFromNetworks(int cell, FlowUtilityNetwork.IItem vent);

	FlowUtilityNetwork.IItem GetEndpoint(int cell);

	UtilityNetwork GetNetworkForOrientation(int cell, Orientation orientation);

	UtilityNetwork GetNetworkForCell(int cell);

	ConduitFlow ConduitFlowManager { get; set; }
}
