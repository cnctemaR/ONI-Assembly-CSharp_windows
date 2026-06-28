using System;
using ProcGenGame;
using UnityEngine;

public class RiverSource : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		River riverForCell = Rivers.GetRiverForCell(WorldGen.Rivers, Grid.PosToCell(this.transform.position));
		if (riverForCell != null)
		{
			this.elementID = riverForCell.element;
			this.maxMass = riverForCell.maxMass;
			this.flowRate = riverForCell.flowIn;
			this.temperature = riverForCell.temperature;
		}
		this.elementIdx = SimMessages.GetElementIndex(this.elementID);
	}

	private void SimUpdate(float dt)
	{
		int num = Grid.PosToCell(this.transform.position);
		Element element = Grid.Element[num];
		if (element.id == this.elementID || !element.IsSolid)
		{
			float mass = Grid.Cell[num].mass;
			if (mass < this.maxMass)
			{
				float num2 = Mathf.Min(this.flowRate * dt, this.maxMass - mass);
				SimMessages.ModifyMass(num, num2, byte.MaxValue, 0, CellEventLogger.Instance.RiverSourceSimUpdate, this.temperature, this.elementID);
			}
			if ((double)(this.maxMass - mass) < 5.0 && Grid.Temperature[num] < this.temperature)
			{
				SimMessages.ModifyCell(num, this.elementIdx, this.temperature, this.maxMass, byte.MaxValue, 0, SimMessages.ReplaceType.ReplaceAndDisplace, -1);
			}
		}
		else
		{
			SimMessages.ModifyCell(num, this.elementIdx, this.temperature, this.maxMass, byte.MaxValue, 0, SimMessages.ReplaceType.ReplaceAndDisplace, -1);
		}
	}

	[SerializeField]
	[HashedEnum]
	public SimHashes elementID;

	[SerializeField]
	public float maxMass;

	[SerializeField]
	public float flowRate;

	[SerializeField]
	public float temperature;

	private int elementIdx;
}
