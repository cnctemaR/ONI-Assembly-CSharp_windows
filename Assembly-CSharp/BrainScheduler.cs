using System;
using System.Collections.Generic;

public class BrainScheduler : KMonoBehaviour
{
	public static BrainScheduler Get()
	{
		return BrainScheduler.Instance;
	}

	protected override void OnPrefabInit()
	{
		BrainScheduler.Instance = this;
		Components.Brains.Register(new Action<Brain>(this.OnAddBrain), new Action<Brain>(this.OnRemoveBrain));
	}

	private void OnAddBrain(Brain brain)
	{
		this.brains.Add(brain);
	}

	private void OnRemoveBrain(Brain brain)
	{
		this.brains.Remove(brain);
	}

	private void Update()
	{
		if (!Game.IsQuitting() && !KMonoBehaviour.isLoadingScene)
		{
			this.updated_brains.Clear();
			int num = this.BrainUpdatesPerFrame;
			int num2 = 0;
			while (num2 < this.brains.Count && num > 0)
			{
				Brain brain = this.brains[num2];
				if (brain.IsRunning())
				{
					brain.UpdateBrain();
					this.updated_brains.Add(brain);
					this.brains.RemoveAt(num2);
					num--;
				}
				else
				{
					num2++;
				}
			}
			this.brains.AddRange(this.updated_brains);
		}
	}

	private static BrainScheduler Instance;

	public int BrainUpdatesPerFrame;

	private List<Brain> brains = new List<Brain>();

	private List<Brain> updated_brains = new List<Brain>();
}
