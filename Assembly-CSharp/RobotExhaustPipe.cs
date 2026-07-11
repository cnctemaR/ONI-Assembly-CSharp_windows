using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/RobotExhaustPipe")]
public class RobotExhaustPipe : KMonoBehaviour, ISim4000ms
{
	public void Sim4000ms(float dt)
	{
		Facing component = base.GetComponent<Facing>();
		bool flag = false;
		if (component)
		{
			flag = component.GetFacing();
		}
		CO2Manager.instance.SpawnBreath(Grid.CellToPos(Grid.PosToCell(base.gameObject)), dt * this.CO2_RATE, 303.15f, flag);
	}

	private float CO2_RATE = 0.001f;
}
