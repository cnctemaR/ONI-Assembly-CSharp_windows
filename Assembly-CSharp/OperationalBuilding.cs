using System;

public class OperationalBuilding : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.Subscribe(824508782, new Action<object>(this.InternalOnActiveChanged));
	}

	protected void SimUpdate(float dt)
	{
		if (this.Operational.IsOperational)
		{
			this.OperationalUpdate(dt);
		}
	}

	private void InternalOnActiveChanged(object data)
	{
		this.OnActiveChanged((bool)data);
	}

	protected virtual void OperationalUpdate(float dt)
	{
	}

	protected virtual void OnActiveChanged(bool is_active)
	{
	}

	[MyCmpReq]
	protected Operational Operational;
}
