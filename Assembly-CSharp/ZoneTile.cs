using System;
using ProcGen;

internal class ZoneTile : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		foreach (int num in this.building.PlacementCells)
		{
			SimMessages.ModifyCellWorldZone(num, 0);
		}
	}

	protected override void OnCleanUp()
	{
		foreach (int num in this.building.PlacementCells)
		{
			SubWorld.ZoneType subWorldZoneType = global::World.Instance.zoneRenderData.GetSubWorldZoneType(num);
			byte b = ((subWorldZoneType != SubWorld.ZoneType.Space) ? ((byte)subWorldZoneType) : byte.MaxValue);
			SimMessages.ModifyCellWorldZone(num, b);
		}
	}

	[MyCmpReq]
	public Building building;
}
