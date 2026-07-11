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
		smi.GetSMI<SpeechMonitor.Instance>().PlaySpeech(smi.currentThought.speechPrefix, smi.currentThought.sound);
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

	public new class Instance : GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.GameInstance, IRenderEveryTick
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.animController = base.GetComponent<KBatchedAnimController>();
			this.bubble = Util.KInstantiate(EffectPrefabs.Instance.ThoughtBubble, base.gameObject, null);
			this.bubbleConvo = Util.KInstantiate(EffectPrefabs.Instance.ThoughtBubbleConvo, base.gameObject, null);
			this.bubble.SetActive(false);
			this.bubbleConvo.SetActive(false);
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
			GameObject gameObject = this.bubble;
			if (thought.modeSprite != null)
			{
				gameObject = this.bubbleConvo;
			}
			this.ApplySprite(gameObject, thought.sprite, "icon_sprite");
			this.ApplySprite(gameObject, thought.bubbleSprite, "bubble_sprite");
			if (thought.modeSprite != null)
			{
				this.ApplySprite(gameObject, thought.modeSprite, "icon_sprite_mode");
			}
			gameObject.SetActive(true);
			base.sm.thoughtDisplayTime.Set(thought.showTime, this);
			gameObject.GetComponent<KSelectable>().entityName = thought.hoverText;
			KCollider2D component = gameObject.GetComponent<KCollider2D>();
			if (component != null)
			{
				component.MarkDirty(false);
			}
			SimAndRenderScheduler.instance.Add(this, false);
			this.currentThought = thought;
			if (thought.showImmediately)
			{
				this.thoughts.RemoveAt(0);
			}
		}

		private void ApplySprite(GameObject active_bubble, Sprite sprite, string target)
		{
			HierarchyReferences component = active_bubble.GetComponent<HierarchyReferences>();
			SpriteRenderer reference = component.GetReference<SpriteRenderer>(target);
			reference.sprite = sprite;
		}

		public void RenderEveryTick(float dt)
		{
			if (this.animController == null)
			{
				return;
			}
			bool flag;
			Matrix2x3 symbolLocalTransform = this.animController.GetSymbolLocalTransform(ThoughtGraph.Instance.symbol, out flag);
			Matrix4x4 matrix4x = this.animController.GetTransformMatrix() * symbolLocalTransform;
			Vector3 vector = new Vector3(matrix4x.m03, matrix4x.m13, 0f);
			this.bubble.transform.SetPosition(vector + EffectPrefabs.Instance.ThoughtBubble.transform.GetLocalPosition());
			this.bubbleConvo.transform.SetPosition(vector + EffectPrefabs.Instance.ThoughtBubble.transform.GetLocalPosition());
		}

		public void DestroyBubble()
		{
			SimAndRenderScheduler.instance.Remove(this);
			this.bubble.SetActive(false);
			this.bubbleConvo.SetActive(false);
		}

		private List<Thought> thoughts = new List<Thought>();

		private GameObject bubble;

		private GameObject bubbleConvo;

		private KBatchedAnimController animController;

		public Thought currentThought;

		public static HashedString symbol = new HashedString("snapTo_pivot");
	}
}
