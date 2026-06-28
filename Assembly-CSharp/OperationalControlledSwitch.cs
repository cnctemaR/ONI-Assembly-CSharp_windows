using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class OperationalControlledSwitch : CircuitSwitch
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.manuallyControlled = false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
	}

	private void OnOperationalChanged(object data)
	{
		bool flag = (bool)data;
		this.SetState(flag);
	}
}
