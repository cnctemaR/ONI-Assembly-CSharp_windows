using System;
using System.Collections.Generic;

public class ThoughtGraph : GameStateMachine<ThoughtGraph, ThoughtGraph.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.initialdelay;
		this.initialdelay.ScheduleGoTo(1f, this.nothoughts);
		this.nothoughts.OnSignal(this.thoughtsChanged, this.displayingthought, (ThoughtGraph.Instance smi) => smi.HasThoughts()).OnSignal(this.thoughtsChangedImmediate, this.displayingthought, (ThoughtGraph.Instance smi) => smi.HasThoughts());
		this.displayingthought.DefaultState(this.displayingthought.pre).Enter("CreateBubble", delegate(ThoughtGraph.Instance smi)
		{
			smi.CreateBubble();
		}).Exit("DestroyBubble", delegate(ThoughtGraph.Instance smi)
		{
			smi.DestroyBubble();
		})
			.ScheduleGoTo((ThoughtGraph.Instance smi) => this.thoughtDisplayTime.Get(smi), this.cooldown);
		this.displayingthought.pre.ScheduleGoTo((ThoughtGraph.Instance smi) => TuningData<ThoughtGraph.Tuning>.Get().preLengthInSeconds, this.displayingthought.talking);
		this.displayingthought.talking.Enter(new StateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.State.Callback(ThoughtGraph.BeginTalking));
		this.cooldown.OnSignal(this.thoughtsChangedImmediate, this.displayingthought, (ThoughtGraph.Instance smi) => smi.HasImmediateThought()).ScheduleGoTo(20f, this.nothoughts);
	}

	private static void BeginTalking(ThoughtGraph.Instance smi)
	{
		if (smi.currentThought == null)
		{
			return;
		}
		if (SpeechMonitor.IsAllowedToPlaySpeech(smi.gameObject))
		{
			smi.GetSMI<SpeechMonitor.Instance>().PlaySpeech(smi.currentThought.speechPrefix, smi.currentThought.sound);
		}
	}

	public StateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.Signal thoughtsChanged;

	public StateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.Signal thoughtsChangedImmediate;

	public StateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.FloatParameter thoughtDisplayTime;

	public GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.State initialdelay;

	public GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.State nothoughts;

	public ThoughtGraph.DisplayingThoughtState displayingthought;

	public GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.State cooldown;

	public class Tuning : TuningData<ThoughtGraph.Tuning>
	{
		public float preLengthInSeconds;
	}

	public class DisplayingThoughtState : GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.State pre;

		public GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.State talking;
	}

	public new class Instance : GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			NameDisplayScreen.Instance.RegisterComponent(base.gameObject, this, false);
		}

		public bool HasThoughts()
		{
			return this.thoughts.Count > 0;
		}

		public bool HasImmediateThought()
		{
			bool flag = false;
			for (int i = 0; i < this.thoughts.Count; i++)
			{
				if (this.thoughts[i].showImmediately)
				{
					flag = true;
					break;
				}
			}
			return flag;
		}

		public void AddThought(Thought thought)
		{
			if (this.thoughts.Contains(thought))
			{
				return;
			}
			this.thoughts.Add(thought);
			if (thought.showImmediately)
			{
				base.sm.thoughtsChangedImmediate.Trigger(base.smi);
				return;
			}
			base.sm.thoughtsChanged.Trigger(base.smi);
		}

		public void RemoveThought(Thought thought)
		{
			if (!this.thoughts.Contains(thought))
			{
				return;
			}
			this.thoughts.Remove(thought);
			base.sm.thoughtsChanged.Trigger(base.smi);
		}

		private int SortThoughts(Thought a, Thought b)
		{
			if (a.showImmediately == b.showImmediately)
			{
				return b.priority.CompareTo(a.priority);
			}
			if (!a.showImmediately)
			{
				return 1;
			}
			return -1;
		}

		public void CreateBubble()
		{
			if (this.thoughts.Count == 0)
			{
				return;
			}
			this.thoughts.Sort(new Comparison<Thought>(this.SortThoughts));
			Thought thought = this.thoughts[0];
			if (thought.modeSprite != null)
			{
				NameDisplayScreen.Instance.SetThoughtBubbleConvoDisplay(base.gameObject, true, thought.hoverText, thought.bubbleSprite, thought.sprite, thought.modeSprite);
			}
			else
			{
				NameDisplayScreen.Instance.SetThoughtBubbleDisplay(base.gameObject, true, thought.hoverText, thought.bubbleSprite, thought.sprite);
			}
			base.sm.thoughtDisplayTime.Set(thought.showTime, this);
			this.currentThought = thought;
			if (thought.showImmediately)
			{
				this.thoughts.RemoveAt(0);
			}
		}

		public void DestroyBubble()
		{
			NameDisplayScreen.Instance.SetThoughtBubbleDisplay(base.gameObject, false, null, null, null);
			NameDisplayScreen.Instance.SetThoughtBubbleConvoDisplay(base.gameObject, false, null, null, null, null);
		}

		private List<Thought> thoughts = new List<Thought>();

		public Thought currentThought;
	}
}
