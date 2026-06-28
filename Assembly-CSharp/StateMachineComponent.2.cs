using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class StateMachineComponent<StateMachineInstanceType> : StateMachineComponent, ISaveLoadable where StateMachineInstanceType : StateMachine.Instance
{
	public StateMachineInstanceType smi
	{
		get
		{
			if (this._smi == null)
			{
				this._smi = (StateMachineInstanceType)((object)Activator.CreateInstance(typeof(StateMachineInstanceType), new object[] { this }));
			}
			return this._smi;
		}
	}

	public override StateMachine.Instance GetSMI()
	{
		return this._smi;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this._smi != null)
		{
			this._smi.StopSM("StateMachineComponent.OnCleanUp");
			this._smi = (StateMachineInstanceType)((object)null);
		}
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		if (base.isSpawned)
		{
			StateMachineInstanceType smi = this.smi;
			smi.StartSM();
		}
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		StateMachineInstanceType smi = this.smi;
		smi.StopSM("StateMachineComponent.OnDisable");
	}

	private StateMachineInstanceType _smi;
}
