using System;
using UnityEngine;

public class RangedAttackable : AttackableBase
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.preferUnreservedCell = true;
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
	}

	public new int GetCell()
	{
		return Grid.PosToCell(this);
	}

	public new bool ShouldPreferUnreservedCell()
	{
		return true;
	}

	public new bool ShouldPreferPrimaryCell()
	{
		return false;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0f, 0.5f, 0.5f, 0.15f);
		foreach (CellOffset cellOffset in base.GetOffsets())
		{
			Gizmos.DrawCube(new Vector3(0.5f, 0.5f, 0f) + Grid.CellToPos(Grid.OffsetCell(Grid.PosToCell(base.gameObject), cellOffset)), Vector3.one);
		}
	}
}
