using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class OperationalValve : ValveBase
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
	}

	protected override void OnSpawn()
	{
		this.OnOperationalChanged(this.operational.IsOperational);
		base.OnSpawn();
	}

	protected override void OnCleanUp()
	{
		base.Unsubscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		base.OnCleanUp();
	}

	private void OnOperationalChanged(object data)
	{
		bool flag = (bool)data;
		if (flag)
		{
			base.CurrentFlow = base.MaxFlow;
		}
		else
		{
			base.CurrentFlow = 0f;
		}
	}

	public override void UpdateAnim()
	{
		if (this.operational.IsOperational)
		{
			if (this.flowAccumulator.AvgRate > 0f)
			{
				this.controller.Play("on_flow", KAnim.PlayMode.Loop, 1f, 0f);
			}
			else
			{
				this.controller.Play("on", KAnim.PlayMode.Once, 1f, 0f);
			}
		}
		else if (this.flowAccumulator.AvgRate > 0f)
		{
			this.controller.Play("off_flow", KAnim.PlayMode.Loop, 1f, 0f);
		}
		else
		{
			this.controller.Play("off", KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	[MyCmpReq]
	private Operational operational;
}
