using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/RobotExhaustPipe")]
public class RobotExhaustPipe : KMonoBehaviour, ISim4000ms
{
	public void Sim4000ms(float dt)
	{
		CO2Manager.instance.SpawnBreath(Grid.CellToPos(Grid.PosToCell(base.gameObject)), dt * this.CO2_RATE, 303.15f);
	}

	private float CO2_RATE = 0.001f;
}
