using System;
using ProcGen;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ZoneTile")]
public class ZoneTile : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		int[] placementCells = this.building.PlacementCells;
		for (int i = 0; i < placementCells.Length; i++)
		{
			SimMessages.ModifyCellWorldZone(placementCells[i], 0);
		}
		base.Subscribe<ZoneTile>(1606648047, ZoneTile.OnObjectReplacedDelegate);
	}

	protected override void OnCleanUp()
	{
		if (!this.wasReplaced)
		{
			this.ClearZone();
		}
	}

	private void OnObjectReplaced(object data)
	{
		this.ClearZone();
		this.wasReplaced = true;
	}

	private void ClearZone()
	{
		foreach (int num in this.building.PlacementCells)
		{
			SubWorld.ZoneType subWorldZoneType = global::World.Instance.zoneRenderData.GetSubWorldZoneType(num);
			byte b = ((subWorldZoneType == SubWorld.ZoneType.Space) ? byte.MaxValue : ((byte)subWorldZoneType));
			SimMessages.ModifyCellWorldZone(num, b);
		}
	}

	[MyCmpReq]
	public Building building;

	private bool wasReplaced;

	private static readonly EventSystem.IntraObjectHandler<ZoneTile> OnObjectReplacedDelegate = new EventSystem.IntraObjectHandler<ZoneTile>(delegate(ZoneTile component, object data)
	{
		component.OnObjectReplaced(data);
	});
}
