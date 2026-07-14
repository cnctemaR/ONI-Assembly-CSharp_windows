using System;

public class DissolvingAlgae : DissolvingElementDiseaseEmitter
{
	public DissolvingAlgae()
		: base(SimHashes.Oxygen)
	{
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void EvaluateEmissionCondition()
	{
		int num = Grid.PosToCell(this);
		bool flag = Grid.IsValidCell(num) && Grid.IsLiquid(num) && Grid.Element[num].HasTag(GameTags.AnyWater) && Grid.LightIntensity[num] > 500;
		if (this.enableEmitter == flag)
		{
			return;
		}
		base.SetEnable(flag);
		base.UpdateStatusItem();
		if (flag)
		{
			base.SpawnVisualFX();
		}
	}

	private const int LIGHT_THRESHOLD = 500;
}
