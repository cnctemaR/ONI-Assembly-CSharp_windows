using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class OperationalValve : ValveBase
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<OperationalValve>(-592767678, OperationalValve.OnOperationalChangedDelegate);
	}

	protected override void OnSpawn()
	{
		this.OnOperationalChanged(this.operational.IsOperational);
		base.OnSpawn();
	}

	protected override void OnCleanUp()
	{
		base.Unsubscribe<OperationalValve>(-592767678, OperationalValve.OnOperationalChangedDelegate, false);
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
		float averageRate = Game.Instance.accumulators.GetAverageRate(this.flowAccumulator);
		if (this.operational.IsOperational)
		{
			if (averageRate > 0f)
			{
				this.controller.Play("on_flow", KAnim.PlayMode.Loop, 1f, 0f);
			}
			else
			{
				this.controller.Play("on", KAnim.PlayMode.Once, 1f, 0f);
			}
		}
		else if (averageRate > 0f)
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

	private static readonly EventSystem.IntraObjectHandler<OperationalValve> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<OperationalValve>(delegate(OperationalValve component, object data)
	{
		component.OnOperationalChanged(data);
	});
}
