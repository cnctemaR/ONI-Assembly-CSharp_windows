using System;
using System.Collections.Generic;

public class CreatureThoughtGraph : GameStateMachine<CreatureThoughtGraph, CreatureThoughtGraph.Instance, IStateMachineTarget, CreatureThoughtGraph.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.initialdelay;
		this.initialdelay.ScheduleGoTo(1f, this.nothoughts);
		this.nothoughts.OnSignal(this.thoughtsChanged, this.displayingthought, (CreatureThoughtGraph.Instance smi, StateMachine<CreatureThoughtGraph, CreatureThoughtGraph.Instance, IStateMachineTarget, CreatureThoughtGraph.Def>.SignalParameter param) => smi.HasThoughts()).OnSignal(this.thoughtsChangedImmediate, this.displayingthought, (CreatureThoughtGraph.Instance smi, StateMachine<CreatureThoughtGraph, CreatureThoughtGraph.Instance, IStateMachineTarget, CreatureThoughtGraph.Def>.SignalParameter param) => smi.HasThoughts());
		this.displayingthought.Enter("CreateBubble", delegate(CreatureThoughtGraph.Instance smi)
		{
			smi.CreateBubble();
		}).Exit("DestroyBubble", delegate(CreatureThoughtGraph.Instance smi)
		{
			smi.DestroyBubble();
		}).ScheduleGoTo((CreatureThoughtGraph.Instance smi) => this.thoughtDisplayTime.Get(smi), this.cooldown);
		this.cooldown.OnSignal(this.thoughtsChangedImmediate, this.displayingthought, (CreatureThoughtGraph.Instance smi, StateMachine<CreatureThoughtGraph, CreatureThoughtGraph.Instance, IStateMachineTarget, CreatureThoughtGraph.Def>.SignalParameter param) => smi.HasImmediateThought()).ScheduleGoTo(20f, this.nothoughts);
	}

	public StateMachine<CreatureThoughtGraph, CreatureThoughtGraph.Instance, IStateMachineTarget, CreatureThoughtGraph.Def>.Signal thoughtsChanged;

	public StateMachine<CreatureThoughtGraph, CreatureThoughtGraph.Instance, IStateMachineTarget, CreatureThoughtGraph.Def>.Signal thoughtsChangedImmediate;

	public StateMachine<CreatureThoughtGraph, CreatureThoughtGraph.Instance, IStateMachineTarget, CreatureThoughtGraph.Def>.FloatParameter thoughtDisplayTime;

	public GameStateMachine<CreatureThoughtGraph, CreatureThoughtGraph.Instance, IStateMachineTarget, CreatureThoughtGraph.Def>.State initialdelay;

	public GameStateMachine<CreatureThoughtGraph, CreatureThoughtGraph.Instance, IStateMachineTarget, CreatureThoughtGraph.Def>.State nothoughts;

	public GameStateMachine<CreatureThoughtGraph, CreatureThoughtGraph.Instance, IStateMachineTarget, CreatureThoughtGraph.Def>.State displayingthought;

	public GameStateMachine<CreatureThoughtGraph, CreatureThoughtGraph.Instance, IStateMachineTarget, CreatureThoughtGraph.Def>.State cooldown;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<CreatureThoughtGraph, CreatureThoughtGraph.Instance, IStateMachineTarget, CreatureThoughtGraph.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, CreatureThoughtGraph.Def def)
			: base(master, def)
		{
			NameDisplayScreen.Instance.RegisterComponent(base.gameObject, this, false);
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
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
			base.sm.thoughtDisplayTime.Set(thought.showTime, this, false);
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
