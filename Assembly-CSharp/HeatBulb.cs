using System;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/HeatBulb")]
public class HeatBulb : KMonoBehaviour, ISim200ms
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.kanim.Play("off", KAnim.PlayMode.Once, 1f, 0f);
	}

	public void Sim200ms(float dt)
	{
		float num = this.kjConsumptionRate * dt;
		Vector2I vector2I = this.maxCheckOffset - this.minCheckOffset + 1;
		int num2 = vector2I.x * vector2I.y;
		float num3 = num / (float)num2;
		int num4;
		int num5;
		Grid.PosToXY(base.transform.GetPosition(), out num4, out num5);
		for (int i = this.minCheckOffset.y; i <= this.maxCheckOffset.y; i++)
		{
			for (int j = this.minCheckOffset.x; j <= this.maxCheckOffset.x; j++)
			{
				int num6 = Grid.XYToCell(num4 + j, num5 + i);
				if (Grid.IsValidCell(num6) && Grid.Temperature[num6] > this.minTemperature)
				{
					this.kjConsumed += num3;
					SimMessages.ModifyEnergy(num6, -num3, 5000f, SimMessages.EnergySourceID.HeatBulb);
				}
			}
		}
		float num7 = this.lightKJConsumptionRate * dt;
		if (this.kjConsumed > num7)
		{
			if (!this.lightSource.enabled)
			{
				this.kanim.Play("open", KAnim.PlayMode.Once, 1f, 0f);
				this.kanim.Queue("on", KAnim.PlayMode.Once, 1f, 0f);
				this.lightSource.enabled = true;
			}
			this.kjConsumed -= num7;
			return;
		}
		if (this.lightSource.enabled)
		{
			this.kanim.Play("close", KAnim.PlayMode.Once, 1f, 0f);
			this.kanim.Queue("off", KAnim.PlayMode.Once, 1f, 0f);
		}
		this.lightSource.enabled = false;
	}

	[SerializeField]
	private float minTemperature;

	[SerializeField]
	private float kjConsumptionRate;

	[SerializeField]
	private float lightKJConsumptionRate;

	[SerializeField]
	private Vector2I minCheckOffset;

	[SerializeField]
	private Vector2I maxCheckOffset;

	[MyCmpGet]
	private Light2D lightSource;

	[MyCmpGet]
	private KBatchedAnimController kanim;

	[Serialize]
	private float kjConsumed;
}
