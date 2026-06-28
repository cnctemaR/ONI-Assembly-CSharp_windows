using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class Insulator : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		int num = Grid.PosToCell(base.transform.position);
		num = Grid.OffsetCell(num, this.offset);
		SimMessages.SetInsulation(num, this.building.Def.Insulation);
	}

	protected override void OnCleanUp()
	{
		int num = Grid.PosToCell(base.transform.position);
		num = Grid.OffsetCell(num, this.offset);
		SimMessages.SetInsulation(num, 1f);
	}

	[MyCmpReq]
	private Building building;

	[SerializeField]
	public CellOffset offset = CellOffset.none;
}
