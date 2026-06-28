using System;
using System.Collections.Generic;
using UnityEngine;

public class ThoughtGraph : GameStateMachine<ThoughtGraph, ThoughtGraph.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.initialdelay;
		this.initialdelay.ScheduleGoTo(1f, this.nothoughts);
		this.nothoughts.OnSignal(this.thoughtsChanged, this.displayingthought, (ThoughtGraph.Instance smi) => smi.HasThoughts()).OnSignal(this.thoughtsChangedImmediate, this.displayingthought, (ThoughtGraph.Instance smi) => smi.HasThoughts());
		this.displayingthought.Enter("CreateBubble", delegate(ThoughtGraph.Instance smi)
		{
			smi.CreateBubble();
		}).Exit("DestroyBubble", delegate(ThoughtGraph.Instance smi)
		{
			smi.DestroyBubble();
		}).ScheduleGoTo(4f, this.cooldown);
		this.cooldown.OnSignal(this.thoughtsChangedImmediate, this.displayingthought, (ThoughtGraph.Instance smi) => smi.HasImmediateThought()).ScheduleGoTo(20f, this.nothoughts);
	}

	public StateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget>.Signal thoughtsChanged;

	public StateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget>.Signal thoughtsChangedImmediate;

	public GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget>.State initialdelay;

	public GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget>.State nothoughts;

	public GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget>.State displayingthought;

	public GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget>.State cooldown;

	public new class Instance : GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.bubble = Util.KInstantiate(EffectPrefabs.Instance.ThoughtBubble, base.gameObject, null);
			this.bubble.SetActive(false);
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
			}
			else
			{
				base.sm.thoughtsChanged.Trigger(base.smi);
			}
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

		public void CreateBubble()
		{
			if (this.thoughts.Count == 0)
			{
				return;
			}
			this.thoughts.Sort((Thought a, Thought b) => b.priority.CompareTo(a.priority));
			Thought thought = this.thoughts[0];
			this.bubble.SetActive(true);
			RenderUtil.SetColor(this.bubble.transform, Color.red);
			Material material = RenderUtil.GetMaterial(this.bubble.transform, "happy");
			material.mainTexture = thought.texture;
			material.color = Color.white;
			this.bubble.transform.localPosition = EffectPrefabs.Instance.ThoughtBubble.transform.localPosition;
			this.bubble.transform.localScale = EffectPrefabs.Instance.ThoughtBubble.transform.localScale;
			this.bubble.transform.localRotation = EffectPrefabs.Instance.ThoughtBubble.transform.localRotation;
			this.bubble.GetComponent<KSelectable>().entityName = thought.hoverText;
			VoiceSoundEvent voiceSoundEvent = new VoiceSoundEvent("ThoughtGraph", thought.texture.name, 0, false);
			voiceSoundEvent.Play(base.transform);
			if (thought.showImmediately)
			{
				this.thoughts.RemoveAt(0);
			}
		}

		public void DestroyBubble()
		{
			this.bubble.SetActive(false);
		}

		private List<Thought> thoughts = new List<Thought>();

		private GameObject bubble;
	}
}
