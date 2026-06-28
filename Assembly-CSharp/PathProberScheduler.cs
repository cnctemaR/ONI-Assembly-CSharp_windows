using System;
using System.Collections.Generic;

public class PathProberScheduler : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		PathProberScheduler.Instance = this;
	}

	public void Add(Navigator navigator)
	{
		this.navigators.Add(navigator);
	}

	public void Remove(Navigator navigator)
	{
		this.navigators.Remove(navigator);
	}

	private void Update()
	{
		int num = Math.Min(5, this.navigators.Count);
		for (int i = 0; i < num; i++)
		{
			this.lastUpdateIdx++;
			int num2 = this.lastUpdateIdx % this.navigators.Count;
			Navigator navigator = this.navigators[num2];
			if (navigator != null)
			{
				navigator.UpdateProbe();
			}
		}
	}

	private int lastUpdateIdx;

	private List<Navigator> navigators = new List<Navigator>();

	public static PathProberScheduler Instance;
}
