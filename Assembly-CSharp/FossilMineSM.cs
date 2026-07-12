using System;

public class FossilMineSM : ComplexFabricatorSM
{
	protected override void OnSpawn()
	{
	}

	public void Activate()
	{
		base.smi.StartSM();
	}

	public void Deactivate()
	{
		base.smi.StopSM("FossilMine.Deactivated");
	}
}
