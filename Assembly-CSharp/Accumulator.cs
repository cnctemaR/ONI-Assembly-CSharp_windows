using System;

public class Accumulator
{
	public Accumulator(string name, KMonoBehaviour owner, float _timeWindow = 3f)
	{
		this.name = name;
		this.owner = owner;
		owner.Subscribe(1969584890, new Action<object>(this.OnCleanUp));
		this.timeWindow = _timeWindow;
		this.handle = GameScheduler.Instance.SchedulePeriodic(this.GetDebugName() + ".Accumulator", this.timeWindow, delegate(object obj)
		{
			((Accumulator)obj).CalculateAverage();
		}, this, null, 0f, null);
		if (AccumulatorManager.Instance != null)
		{
			AccumulatorManager.Instance.Add(this);
		}
	}

	public float DebugCurrentAccumulate
	{
		get
		{
			return this.accumulated;
		}
	}

	public float AvgRate
	{
		get
		{
			return this.avgRate;
		}
	}

	public void Accumulate(float amount)
	{
		this.accumulated += amount;
	}

	private void CalculateAverage()
	{
		if (this.timeWindow > 0f)
		{
			this.avgRate = this.accumulated / this.timeWindow;
			this.accumulated = 0f;
		}
	}

	public void RestartTimeWindow()
	{
		float num = this.timeWindow - this.handle.TimeRemaining;
		if (num > 0f)
		{
			this.avgRate = this.accumulated / num;
			this.accumulated = 0f;
		}
		this.handle.Clear();
		this.handle = GameScheduler.Instance.SchedulePeriodic(this.GetDebugName() + ".Accumulator", this.timeWindow, delegate(object obj)
		{
			((Accumulator)obj).CalculateAverage();
		}, this, null, 0f, null);
	}

	private void OnCleanUp(object data)
	{
		this.handle.Clear();
		if (AccumulatorManager.Instance != null)
		{
			AccumulatorManager.Instance.Remove(this);
		}
	}

	public string GetDebugName()
	{
		return string.Concat(new string[]
		{
			this.owner.name,
			".",
			this.owner.GetType().Name,
			".",
			this.name
		});
	}

	private float timeWindow;

	private float avgRate;

	private float accumulated;

	private SchedulerHandle handle;

	private KMonoBehaviour owner;

	private string name;
}
