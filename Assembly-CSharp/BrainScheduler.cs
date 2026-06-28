using System;
using System.Collections.Generic;

public class BrainScheduler : KMonoBehaviour, IRenderEveryTick
{
	protected override void OnPrefabInit()
	{
		var anon = new <>__AnonType0<Tag>[]
		{
			new
			{
				tag = GameTags.DupeBrain
			},
			new
			{
				tag = GameTags.CreatureBrain
			}
		};
		var anon2 = anon;
		for (int i = 0; i < anon2.Length; i++)
		{
			var anon3 = anon2[i];
			BrainScheduler.BrainGroup brainGroup = new BrainScheduler.BrainGroup(anon3.tag);
			this.brainGroups.Add(brainGroup);
		}
		Components.Brains.Register(new Action<Brain>(this.OnAddBrain), new Action<Brain>(this.OnRemoveBrain));
	}

	private void OnAddBrain(Brain brain)
	{
		bool flag = false;
		foreach (BrainScheduler.BrainGroup brainGroup in this.brainGroups)
		{
			if (brain.HasTag(brainGroup.tag))
			{
				brainGroup.AddBrain(brain);
				flag = true;
			}
		}
		DebugUtil.Assert(flag, "Assert!");
	}

	private void OnRemoveBrain(Brain brain)
	{
		bool flag = false;
		foreach (BrainScheduler.BrainGroup brainGroup in this.brainGroups)
		{
			if (brain.HasTag(brainGroup.tag))
			{
				flag = true;
				brainGroup.RemoveBrain(brain);
			}
		}
		DebugUtil.Assert(flag, "Assert!");
	}

	public void RenderEveryTick(float dt)
	{
		if (Game.IsQuitting() || KMonoBehaviour.isLoadingScene)
		{
			return;
		}
		foreach (BrainScheduler.BrainGroup brainGroup in this.brainGroups)
		{
			brainGroup.RenderEveryTick(dt);
		}
	}

	private List<BrainScheduler.BrainGroup> brainGroups = new List<BrainScheduler.BrainGroup>();

	private class BrainGroup
	{
		public BrainGroup(Tag tag)
		{
			this.tag = tag;
		}

		public Tag tag { get; private set; }

		public void AddBrain(Brain brain)
		{
			this.brains.Add(brain);
		}

		public void RemoveBrain(Brain brain)
		{
			this.brains.Remove(brain);
		}

		public void RenderEveryTick(float dt)
		{
			this.upodatedBrains.Clear();
			int num = 1;
			int num2 = 0;
			while (num2 < this.brains.Count && num > 0)
			{
				Brain brain = this.brains[num2];
				if (brain.IsRunning())
				{
					brain.UpdateBrain();
					this.upodatedBrains.Add(brain);
					this.brains.RemoveAt(num2);
					num--;
				}
				else
				{
					num2++;
				}
			}
			this.brains.AddRange(this.upodatedBrains);
		}

		private const int BRAIN_UPDATES_PER_FRAME = 1;

		private List<Brain> brains = new List<Brain>();

		private List<Brain> upodatedBrains = new List<Brain>();
	}
}
