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
		if ((bool)data)
		{
			base.CurrentFlow = base.MaxFlow;
			return;
		}
		base.CurrentFlow = 0f;
	}

	public override void UpdateAnim()
	{
		float averageRate = Game.Instance.accumulators.GetAverageRate(this.flowAccumulator);
		if (this.operational.IsOperational)
		{
			if (averageRate > 0f)
			{
				this.controller.Queue("on_flow", KAnim.PlayMode.Loop, 1f, 0f);
				return;
			}
			this.controller.Queue("on", KAnim.PlayMode.Once, 1f, 0f);
			return;
		}
		else
		{
			if (averageRate > 0f)
			{
				this.controller.Queue("off_flow", KAnim.PlayMode.Loop, 1f, 0f);
				return;
			}
			this.controller.Queue("off", KAnim.PlayMode.Once, 1f, 0f);
			return;
		}
	}

	[MyCmpReq]
	private Operational operational;

	private static readonly EventSystem.IntraObjectHandler<OperationalValve> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<OperationalValve>(delegate(OperationalValve component, object data)
	{
		component.OnOperationalChanged(data);
	});
}
