using System;
using UnityEngine;

public class DevLifeSupport : KMonoBehaviour, ISim200ms
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.elementConsumer != null)
		{
			this.elementConsumer.EnableConsumption(true);
		}
	}

	public void Sim200ms(float dt)
	{
		Vector2I vector2I = new Vector2I(-this.effectRadius, -this.effectRadius);
		Vector2I vector2I2 = new Vector2I(this.effectRadius, this.effectRadius);
		int num;
		int num2;
		Grid.PosToXY(base.transform.GetPosition(), out num, out num2);
		int num3 = Grid.XYToCell(num, num2);
		if (Grid.IsValidCell(num3))
		{
			int num4 = (int)Grid.WorldIdx[num3];
			for (int i = vector2I.y; i <= vector2I2.y; i++)
			{
				for (int j = vector2I.x; j <= vector2I2.x; j++)
				{
					int num5 = Grid.XYToCell(num + j, num2 + i);
					if (Grid.IsValidCellInWorld(num5, num4))
					{
						float num6 = (this.targetTemperature - Grid.Temperature[num5]) * Grid.Element[num5].specificHeatCapacity * Grid.Mass[num5];
						if (!Mathf.Approximately(0f, num6))
						{
							SimMessages.ModifyEnergy(num5, num6 * 0.2f, 5000f, (num6 > 0f) ? SimMessages.EnergySourceID.DebugHeat : SimMessages.EnergySourceID.DebugCool);
						}
					}
				}
			}
		}
	}

	[MyCmpReq]
	private ElementConsumer elementConsumer;

	public float targetTemperature = 303.15f;

	public int effectRadius = 7;

	private const float temperatureControlK = 0.2f;
}
