using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public abstract class StateMachineComponent : KMonoBehaviour, ISaveLoadable, IStateMachineTarget
{
	public abstract StateMachine.Instance GetSMI();

	[MyCmpAdd]
	protected StateMachineController stateMachineController;
}
