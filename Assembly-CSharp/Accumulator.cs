using System;

public class Accumulator
{
	public Accumulator(string name, KMonoBehaviour owner, float _timeWindow = 3f)
	{
		this.name = name;
		this.owner = owner;
		owner.Subscribe(1969584890, new EventSystem.EventHandler(this.OnCleanUp));
		this.timeWindow = _timeWindow;
		this.handle = GameScheduler.Instance.SchedulePeriodic(this.GetDebugName() + ".Accumulator", this.timeWindow, delegate(object obj)
		{
			((Accumulator)obj).CalculateAverage();
		}, this, null, 0f);
		if (AccumulatorManager.Instance != null)
		{
			AccumulatorManager.Instance.Add(this);
		}
	}

	public float DebugCurrentAccumulate
	{
		get
		{
			return this.accumulatedMass;
		}
	}

	public float AvgFlowRate
	{
		get
		{
			return this.avgFlowRate;
		}
	}

	public void Accumulate(float mass)
	{
		this.accumulatedMass += mass;
	}

	private void CalculateAverage()
	{
		if (this.timeWindow > 0f)
		{
			this.avgFlowRate = this.accumulatedMass / this.timeWindow;
			this.accumulatedMass = 0f;
		}
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

	private float avgFlowRate;

	private float accumulatedMass;

	private SchedulerHandle handle;

	private KMonoBehaviour owner;

	private string name;
}
