using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Insulator")]
public class Insulator : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		SimMessages.SetInsulation(Grid.OffsetCell(Grid.PosToCell(base.transform.GetPosition()), this.offset), this.building.Def.ThermalConductivity);
	}

	protected override void OnCleanUp()
	{
		SimMessages.SetInsulation(Grid.OffsetCell(Grid.PosToCell(base.transform.GetPosition()), this.offset), 1f);
	}

	[MyCmpReq]
	private Building building;

	[SerializeField]
	public CellOffset offset = CellOffset.none;
}
