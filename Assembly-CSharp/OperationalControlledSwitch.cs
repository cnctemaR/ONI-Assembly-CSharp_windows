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
		base.Subscribe<OperationalControlledSwitch>(-592767678, OperationalControlledSwitch.OnOperationalChangedDelegate);
	}

	private void OnOperationalChanged(object data)
	{
		bool value = ((Boxed<bool>)data).value;
		this.SetState(value);
	}

	private static readonly EventSystem.IntraObjectHandler<OperationalControlledSwitch> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<OperationalControlledSwitch>(delegate(OperationalControlledSwitch component, object data)
	{
		component.OnOperationalChanged(data);
	});
}
