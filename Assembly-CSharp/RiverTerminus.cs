using System;
using Generated;
using Klei;
using UnityEngine;

public class RiverTerminus : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		River riverForCell = WorldGen.GetRiverForCell(Grid.PosToCell(this.transform.position));
		if (riverForCell != null)
		{
			this.flowRate = riverForCell.flowOut;
		}
	}

	private void SimUpdate(float dt)
	{
		int num = Grid.PosToCell(this.transform.position);
		Element element = Grid.Element[num];
		if (element.IsLiquid)
		{
			float mass = Grid.Cell[num].mass;
			float num2 = Mathf.Min(this.flowRate * dt, mass);
			SimMessages.ModifyMass(num, -num2, CellEventLogger.Instance.RiverTerminusSimUpdate, -1f, SimHashes.Water);
		}
	}

	[SerializeField]
	private float flowRate;
}
