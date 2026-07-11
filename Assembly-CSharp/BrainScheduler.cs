using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/BrainScheduler")]
public class BrainScheduler : KMonoBehaviour, IRenderEveryTick, ICPULoad
{
	private bool isAsyncPathProbeEnabled
	{
		get
		{
			return !TuningData<BrainScheduler.Tuning>.Get().disableAsyncPathProbes;
		}
	}

	protected override void OnPrefabInit()
	{
		this.brainGroups.Add(new BrainScheduler.DupeBrainGroup());
		this.brainGroups.Add(new BrainScheduler.CreatureBrainGroup());
		Components.Brains.Register(new Action<Brain>(this.OnAddBrain), new Action<Brain>(this.OnRemoveBrain));
		CPUBudget.AddRoot(this);
		foreach (BrainScheduler.BrainGroup brainGroup in this.brainGroups)
		{
			CPUBudget.AddChild(this, brainGroup, brainGroup.LoadBalanceThreshold());
		}
		CPUBudget.FinalizeChildren(this);
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
			Navigator component = brain.GetComponent<Navigator>();
			if (component != null)
			{
				component.executePathProbeTaskAsync = this.isAsyncPathProbeEnabled;
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

	public float GetEstimatedFrameTime()
	{
		return TuningData<BrainScheduler.Tuning>.Get().frameTime;
	}

	public bool AdjustLoad(float currentFrameTime, float frameTimeDelta)
	{
		return false;
	}

	public void RenderEveryTick(float dt)
	{
		if (Game.IsQuitting() || KMonoBehaviour.isLoadingScene)
		{
			return;
		}
		foreach (BrainScheduler.BrainGroup brainGroup in this.brainGroups)
		{
			brainGroup.RenderEveryTick(dt, this.isAsyncPathProbeEnabled);
		}
	}

	public const float millisecondsPerFrame = 33.33333f;

	public const float secondsPerFrame = 0.033333328f;

	public const float framesPerSecond = 30.000006f;

	private List<BrainScheduler.BrainGroup> brainGroups = new List<BrainScheduler.BrainGroup>();

	private class Tuning : TuningData<BrainScheduler.Tuning>
	{
		public bool disableAsyncPathProbes;

		public float frameTime = 5f;
	}

	private abstract class BrainGroup : ICPULoad
	{
		public Tag tag { get; private set; }

		protected BrainGroup(Tag tag)
		{
			this.tag = tag;
			this.probeSize = this.InitialProbeSize();
			this.probeCount = this.InitialProbeCount();
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
				this.OnRemoveBrain(num, ref this.nextPathProbeBrain);
			}
		}

		public int probeSize { get; private set; }

		public int probeCount { get; private set; }

		public bool AdjustLoad(float currentFrameTime, float frameTimeDelta)
		{
			bool flag = frameTimeDelta > 0f;
			int num = 0;
			int num2 = Math.Max(this.probeCount, Math.Min(this.brains.Count, CPUBudget.coreCount));
			num += num2 - this.probeCount;
			this.probeCount = num2;
			float num3 = Math.Min(1f, (float)this.probeCount / (float)CPUBudget.coreCount);
			float num4 = num3 * (float)this.probeSize;
			float num5 = num3 * (float)this.probeSize;
			float num6 = currentFrameTime / num5;
			float num7 = frameTimeDelta / num6;
			if (num == 0)
			{
				float num8 = num4 + num7 / (float)CPUBudget.coreCount;
				int num9 = MathUtil.Clamp(this.MinProbeSize(), this.IdealProbeSize(), (int)(num8 / num3));
				num += num9 - this.probeSize;
				this.probeSize = num9;
			}
			if (num == 0)
			{
				int num10 = Math.Max(1, (int)num3 + (flag ? 1 : (-1)));
				int num11 = MathUtil.Clamp(this.MinProbeSize(), this.IdealProbeSize(), (int)((num5 + num7) / (float)num10));
				int num12 = Math.Min(this.brains.Count, num10 * CPUBudget.coreCount);
				num += num12 - this.probeCount;
				this.probeCount = num12;
				this.probeSize = num11;
			}
			if (num == 0 && flag)
			{
				int num13 = this.probeSize + this.ProbeSizeStep();
				num += num13 - this.probeSize;
				this.probeSize = num13;
			}
			if (num >= 0 && num <= 0)
			{
				global::Debug.LogWarning("AdjustLoad() failed");
			}
			return num != 0;
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

		private void AsyncPathProbe()
		{
			int probeSize = this.probeSize;
			this.pathProbeJob.Reset(null);
			for (int num = 0; num != this.brains.Count; num++)
			{
				this.ClampBrainIndex(ref this.nextPathProbeBrain);
				Navigator component = this.brains[this.nextPathProbeBrain].GetComponent<Navigator>();
				this.IncrementBrainIndex(ref this.nextPathProbeBrain);
				if (component != null)
				{
					component.executePathProbeTaskAsync = true;
					component.PathProber.potentialCellsPerUpdate = this.probeSize;
					component.pathProbeTask.Update();
					this.pathProbeJob.Add(component.pathProbeTask);
					if (this.pathProbeJob.Count == this.probeCount)
					{
						break;
					}
				}
			}
			CPUBudget.Start(this);
			GlobalJobManager.Run(this.pathProbeJob);
			CPUBudget.End(this);
		}

		public void RenderEveryTick(float dt, bool isAsyncPathProbeEnabled)
		{
			if (isAsyncPathProbeEnabled)
			{
				this.AsyncPathProbe();
			}
			int num = this.InitialProbeCount();
			int num2 = 0;
			while (num2 != this.brains.Count && num != 0)
			{
				this.ClampBrainIndex(ref this.nextPathProbeBrain);
				Brain brain = this.brains[this.nextUpdateBrain];
				this.IncrementBrainIndex(ref this.nextUpdateBrain);
				if (brain.IsRunning())
				{
					brain.UpdateBrain();
					num--;
				}
				num2++;
			}
		}

		public void AccumulatePathProbeIterations(Dictionary<string, int> pathProbeIterations)
		{
			foreach (Brain brain in this.brains)
			{
				Navigator component = brain.GetComponent<Navigator>();
				if (!(component == null) && !pathProbeIterations.ContainsKey(brain.name))
				{
					pathProbeIterations.Add(brain.name, component.PathProber.updateCount);
				}
			}
		}

		protected abstract int InitialProbeCount();

		protected abstract int InitialProbeSize();

		protected abstract int MinProbeSize();

		protected abstract int IdealProbeSize();

		protected abstract int ProbeSizeStep();

		public abstract float GetEstimatedFrameTime();

		public abstract float LoadBalanceThreshold();

		private List<Brain> brains = new List<Brain>();

		private string increaseLoadLabel;

		private string decreaseLoadLabel;

		private WorkItemCollection<Navigator.PathProbeTask, object> pathProbeJob = new WorkItemCollection<Navigator.PathProbeTask, object>();

		private int nextUpdateBrain;

		private int nextPathProbeBrain;
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

		public class Tuning : TuningData<BrainScheduler.CreatureBrainGroup.Tuning>
		{
			public int initialProbeCount = 1;

			public int initialProbeSize = 1000;

			public int minProbeSize = 100;

			public int idealProbeSize = 300;

			public int probeSizeStep = 100;

			public float estimatedFrameTime = 1f;

			public float loadBalanceThreshold = 0.1f;
		}
	}
}
