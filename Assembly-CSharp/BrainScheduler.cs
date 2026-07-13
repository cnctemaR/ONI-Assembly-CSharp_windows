using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/BrainScheduler")]
public class BrainScheduler : KMonoBehaviour, IRenderEveryTick
{
	public List<BrainScheduler.BrainGroup> debugGetBrainGroups()
	{
		return this.brainGroups;
	}

	protected override void OnPrefabInit()
	{
		this.brainGroups.Add(new BrainScheduler.DupeBrainGroup());
		this.brainGroups.Add(new BrainScheduler.CreatureBrainGroup());
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
		DebugUtil.Assert(flag);
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
			Navigator component = brain.GetComponent<Navigator>();
			if (component != null)
			{
				component.executePathProbeTaskAsync = false;
			}
		}
		DebugUtil.Assert(flag);
	}

	public void PrioritizeBrain(Brain brain)
	{
		foreach (BrainScheduler.BrainGroup brainGroup in this.brainGroups)
		{
			if (brain.HasTag(brainGroup.tag))
			{
				brainGroup.PrioritizeBrain(brain);
			}
		}
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

	protected override void OnForcedCleanUp()
	{
		base.OnForcedCleanUp();
	}

	public const float millisecondsPerFrame = 33.33333f;

	public const float secondsPerFrame = 0.033333328f;

	public const float framesPerSecond = 30.000006f;

	private List<BrainScheduler.BrainGroup> brainGroups = new List<BrainScheduler.BrainGroup>();

	public abstract class BrainGroup
	{
		public Tag tag { get; private set; }

		protected BrainGroup(Tag tag)
		{
			this.tag = tag;
			string text = tag.ToString();
			this.increaseLoadLabel = "IncLoad" + text;
			this.decreaseLoadLabel = "DecLoad" + text;
		}

		public void AddBrain(Brain brain)
		{
			this.brains.Add(brain);
		}

		public void RemoveBrain(Brain brain)
		{
			int num = this.brains.IndexOf(brain);
			if (num != -1)
			{
				this.brains.RemoveAt(num);
				this.OnRemoveBrain(num, ref this.nextUpdateBrain);
			}
			if (this.priorityBrains.Contains(brain))
			{
				List<Brain> list = new List<Brain>(this.priorityBrains);
				list.Remove(brain);
				this.priorityBrains = new Queue<Brain>(list);
			}
		}

		public int BrainCount
		{
			get
			{
				return this.brains.Count;
			}
		}

		public void PrioritizeBrain(Brain brain)
		{
			if (!this.priorityBrains.Contains(brain))
			{
				this.priorityBrains.Enqueue(brain);
			}
		}

		private void IncrementBrainIndex(ref int brainIndex)
		{
			brainIndex++;
			if (brainIndex == this.brains.Count)
			{
				brainIndex = 0;
			}
		}

		private void ClampBrainIndex(ref int brainIndex)
		{
			brainIndex = MathUtil.Clamp(0, this.brains.Count - 1, brainIndex);
		}

		private void OnRemoveBrain(int removedIndex, ref int brainIndex)
		{
			if (removedIndex < brainIndex)
			{
				brainIndex--;
				return;
			}
			if (brainIndex == this.brains.Count)
			{
				brainIndex = 0;
			}
		}

		public void RenderEveryTick(float dt)
		{
			this.BeginBrainGroupUpdate();
			int num = this.InitialProbeCount();
			int num2 = 0;
			while (num2 != this.brains.Count && num != 0)
			{
				this.ClampBrainIndex(ref this.nextUpdateBrain);
				this.debugMaxPriorityBrainCountSeen = Mathf.Max(this.debugMaxPriorityBrainCountSeen, this.priorityBrains.Count);
				Brain brain;
				if (this.AllowPriorityBrains() && this.priorityBrains.Count > 0)
				{
					brain = this.priorityBrains.Dequeue();
				}
				else
				{
					brain = this.brains[this.nextUpdateBrain];
					this.IncrementBrainIndex(ref this.nextUpdateBrain);
				}
				if (brain.IsRunning())
				{
					brain.UpdateBrain();
					num--;
				}
				num2++;
			}
			this.EndBrainGroupUpdate();
		}

		protected abstract int InitialProbeCount();

		protected abstract int InitialProbeSize();

		protected abstract int MinProbeSize();

		protected abstract int IdealProbeSize();

		protected abstract int ProbeSizeStep();

		public abstract float GetEstimatedFrameTime();

		public abstract float LoadBalanceThreshold();

		public abstract bool AllowPriorityBrains();

		public virtual void BeginBrainGroupUpdate()
		{
		}

		public virtual void EndBrainGroupUpdate()
		{
		}

		protected List<Brain> brains = new List<Brain>();

		protected Queue<Brain> priorityBrains = new Queue<Brain>();

		private string increaseLoadLabel;

		private string decreaseLoadLabel;

		public bool debugFreezeLoadAdustment;

		public int debugMaxPriorityBrainCountSeen;

		private int nextUpdateBrain;
	}

	private class DupeBrainGroup : BrainScheduler.BrainGroup
	{
		public DupeBrainGroup()
			: base(GameTags.DupeBrain)
		{
		}

		protected override int InitialProbeCount()
		{
			return TuningData<BrainScheduler.DupeBrainGroup.Tuning>.Get().initialProbeCount;
		}

		protected override int InitialProbeSize()
		{
			return TuningData<BrainScheduler.DupeBrainGroup.Tuning>.Get().initialProbeSize;
		}

		protected override int MinProbeSize()
		{
			return TuningData<BrainScheduler.DupeBrainGroup.Tuning>.Get().minProbeSize;
		}

		protected override int IdealProbeSize()
		{
			return TuningData<BrainScheduler.DupeBrainGroup.Tuning>.Get().idealProbeSize;
		}

		protected override int ProbeSizeStep()
		{
			return TuningData<BrainScheduler.DupeBrainGroup.Tuning>.Get().probeSizeStep;
		}

		public override float GetEstimatedFrameTime()
		{
			return TuningData<BrainScheduler.DupeBrainGroup.Tuning>.Get().estimatedFrameTime;
		}

		public override float LoadBalanceThreshold()
		{
			return TuningData<BrainScheduler.DupeBrainGroup.Tuning>.Get().loadBalanceThreshold;
		}

		public override bool AllowPriorityBrains()
		{
			return this.usePriorityBrain;
		}

		public override void BeginBrainGroupUpdate()
		{
			base.BeginBrainGroupUpdate();
			this.usePriorityBrain = !this.usePriorityBrain;
		}

		private bool usePriorityBrain = true;

		public class Tuning : TuningData<BrainScheduler.DupeBrainGroup.Tuning>
		{
			public int initialProbeCount = 1;

			public int initialProbeSize = 1000;

			public int minProbeSize = 100;

			public int idealProbeSize = 1000;

			public int probeSizeStep = 100;

			public float estimatedFrameTime = 2f;

			public float loadBalanceThreshold = 0.1f;
		}
	}

	private class CreatureBrainGroup : BrainScheduler.BrainGroup
	{
		public CreatureBrainGroup()
			: base(GameTags.CreatureBrain)
		{
		}

		protected override int InitialProbeCount()
		{
			return TuningData<BrainScheduler.CreatureBrainGroup.Tuning>.Get().initialProbeCount;
		}

		protected override int InitialProbeSize()
		{
			return TuningData<BrainScheduler.CreatureBrainGroup.Tuning>.Get().initialProbeSize;
		}

		protected override int MinProbeSize()
		{
			return TuningData<BrainScheduler.CreatureBrainGroup.Tuning>.Get().minProbeSize;
		}

		protected override int IdealProbeSize()
		{
			return TuningData<BrainScheduler.CreatureBrainGroup.Tuning>.Get().idealProbeSize;
		}

		protected override int ProbeSizeStep()
		{
			return TuningData<BrainScheduler.CreatureBrainGroup.Tuning>.Get().probeSizeStep;
		}

		public override float GetEstimatedFrameTime()
		{
			return TuningData<BrainScheduler.CreatureBrainGroup.Tuning>.Get().estimatedFrameTime;
		}

		public override float LoadBalanceThreshold()
		{
			return TuningData<BrainScheduler.CreatureBrainGroup.Tuning>.Get().loadBalanceThreshold;
		}

		public override bool AllowPriorityBrains()
		{
			return true;
		}

		public class Tuning : TuningData<BrainScheduler.CreatureBrainGroup.Tuning>
		{
			public int initialProbeCount = 5;

			public int initialProbeSize = 1000;

			public int minProbeSize = 100;

			public int idealProbeSize = 300;

			public int probeSizeStep = 100;

			public float estimatedFrameTime = 1f;

			public float loadBalanceThreshold = 0.1f;
		}
	}
}
