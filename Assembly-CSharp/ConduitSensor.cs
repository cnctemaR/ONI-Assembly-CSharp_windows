using System;

public abstract class ConduitSensor : Switch
{
	protected abstract void ConduitUpdate(float dt);

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.animController = base.GetComponent<KBatchedAnimController>();
		base.OnToggle += this.OnSwitchToggled;
		this.UpdateLogicCircuit();
		this.UpdateVisualState(true);
		this.wasOn = this.switchedOn;
		if (this.conduitType == ConduitType.Liquid || this.conduitType == ConduitType.Gas)
		{
			Conduit.GetFlowManager(this.conduitType).AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Default);
			return;
		}
		SolidConduit.GetFlowManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Default);
	}

	protected override void OnCleanUp()
	{
		if (this.conduitType == ConduitType.Liquid || this.conduitType == ConduitType.Gas)
		{
			Conduit.GetFlowManager(this.conduitType).RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
		}
		else
		{
			SolidConduit.GetFlowManager().RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
		}
		base.OnCleanUp();
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		this.UpdateLogicCircuit();
		this.UpdateVisualState(false);
	}

	private void UpdateLogicCircuit()
	{
		base.GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, this.switchedOn ? 1 : 0);
	}

	protected virtual void UpdateVisualState(bool force = false)
	{
		if (this.wasOn != this.switchedOn || force)
		{
			this.wasOn = this.switchedOn;
			if (this.switchedOn)
			{
				this.animController.Play(ConduitSensor.ON_ANIMS, KAnim.PlayMode.Loop);
				return;
			}
			this.animController.Play(ConduitSensor.OFF_ANIMS, KAnim.PlayMode.Once);
		}
	}

	public ConduitType conduitType;

	protected bool wasOn;

	protected KBatchedAnimController animController;

	protected static readonly HashedString[] ON_ANIMS = new HashedString[] { "on_pre", "on" };

	protected static readonly HashedString[] OFF_ANIMS = new HashedString[] { "on_pst", "off" };
}
