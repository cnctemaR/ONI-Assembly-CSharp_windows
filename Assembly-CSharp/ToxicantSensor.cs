using System;

public class ToxicantSensor : Sensor
{
	public ToxicantSensor(Sensors sensors)
		: base(sensors)
	{
	}

	public override void Update()
	{
		bool flag = this.IsInToxicArea();
		this.toxicity = 0f;
		int num = Grid.PosToCell(base.transform.position);
		foreach (CellOffset cellOffset in ToxicantSensor.offsets)
		{
			int num2 = Grid.OffsetCell(num, cellOffset);
			this.toxicity += Grid.Element[num2].toxicity;
		}
		this.toxicity /= (float)ToxicantSensor.offsets.Length;
		bool flag2 = this.IsInToxicArea();
		if (flag2 != flag)
		{
			if (flag2)
			{
				base.Trigger(-1198182067, null);
			}
			else
			{
				base.Trigger(369532135, null);
			}
		}
	}

	public bool IsInToxicArea()
	{
		return this.toxicity > 0f;
	}

	public float GetToxicity()
	{
		return this.toxicity;
	}

	private float toxicity;

	private static CellOffset[] offsets = new CellOffset[]
	{
		new CellOffset(0, 0),
		new CellOffset(0, 1)
	};
}
