using System;

public interface IUtilityItem
{
	UtilityConnections Connections { get; set; }

	void UpdateConnections(UtilityConnections Connections);

	int GetNetworkID();

	UtilityNetwork GetNetworkForOrientation(Orientation o);
}
