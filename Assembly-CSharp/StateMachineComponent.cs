using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public abstract class StateMachineComponent : KMonoBehaviour, IStateMachineTarget, ISaveLoadableJson
{
	public abstract StateMachine.Instance GetSMI();

	[MyCmpAdd]
	protected StateMachineController stateMachineController;
}
