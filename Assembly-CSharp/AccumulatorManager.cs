using System;
using System.Collections.Generic;

public class AccumulatorManager : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		AccumulatorManager.Instance = this;
	}

	protected override void OnCleanUp()
	{
		AccumulatorManager.Instance = null;
	}

	public void Add(Accumulator accumulator)
	{
		this.accumulators.Add(accumulator);
	}

	public void Remove(Accumulator accumulator)
	{
		this.accumulators.Remove(accumulator);
	}

	public List<Accumulator> accumulators = new List<Accumulator>();

	public static AccumulatorManager Instance;
}
