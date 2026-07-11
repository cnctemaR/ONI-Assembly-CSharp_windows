using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class LureableMonitor : GameStateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.cooldown;
		this.cooldown.ScheduleGoTo((LureableMonitor.Instance smi) => smi.def.cooldown, this.nolure);
		this.nolure.Update("FindLure", delegate(LureableMonitor.Instance smi, float dt)
		{
			smi.FindLure();
		}, UpdateRate.SIM_1000ms, false).ParamTransition<GameObject>(this.targetLure, this.haslure, (LureableMonitor.Instance smi, GameObject p) => p != null);
		this.haslure.ParamTransition<GameObject>(this.targetLure, this.haslure, (LureableMonitor.Instance smi, GameObject p) => p == null).Update("FindLure", delegate(LureableMonitor.Instance smi, float dt)
		{
			smi.FindLure();
		}, UpdateRate.SIM_1000ms, false).ToggleBehaviour(GameTags.Creatures.MoveToLure, (LureableMonitor.Instance smi) => smi.HasLure(), delegate(LureableMonitor.Instance smi)
		{
			smi.GoTo(this.cooldown);
		});
	}

	public StateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>.TargetParameter targetLure;

	public GameStateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>.State nolure;

	public GameStateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>.State haslure;

	public GameStateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>.State cooldown;

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public List<Descriptor> GetDescriptors(GameObject go)
		{
			return new List<Descriptor>
			{
				new Descriptor(UI.BUILDINGEFFECTS.CAPTURE_METHOD_LURE, UI.BUILDINGEFFECTS.TOOLTIPS.CAPTURE_METHOD_LURE, Descriptor.DescriptorType.Effect, false)
			};
		}

		public float cooldown = 20f;

		public Tag[] lures;
	}

	public new class Instance : GameStateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, LureableMonitor.Def def)
			: base(master, def)
		{
		}

		public void FindLure()
		{
			LureableMonitor.Instance.LureIterator lureIterator = new LureableMonitor.Instance.LureIterator(base.GetComponent<Navigator>(), base.def.lures);
			GameScenePartitioner.Instance.Iterate<LureableMonitor.Instance.LureIterator>(Grid.PosToCell(base.smi.transform.GetPosition()), 1, GameScenePartitioner.Instance.lure, ref lureIterator);
			lureIterator.Cleanup();
			base.sm.targetLure.Set(lureIterator.result, this);
		}

		public bool HasLure()
		{
			return base.sm.targetLure.Get(this) != null;
		}

		public GameObject GetTargetLure()
		{
			return base.sm.targetLure.Get(this);
		}

		private struct LureIterator : GameScenePartitioner.Iterator
		{
			public LureIterator(Navigator navigator, Tag[] lures)
			{
				this.navigator = navigator;
				this.lures = lures;
				this.cost = PathProber.InvalidCost;
				this.result = null;
			}

			public int cost { get; private set; }

			public GameObject result { get; private set; }

			public void Iterate(object target_obj)
			{
				Lure.Instance instance = target_obj as Lure.Instance;
				if (instance == null || !instance.IsActive() || !instance.HasAnyLure(this.lures))
				{
					return;
				}
				int navigationCost = this.navigator.GetNavigationCost(Grid.PosToCell(instance.transform.GetPosition()), instance.def.lurePoints);
				if (navigationCost != PathProber.InvalidCost && (this.cost == PathProber.InvalidCost || navigationCost < this.cost))
				{
					this.cost = navigationCost;
					this.result = instance.gameObject;
				}
			}

			public void Cleanup()
			{
			}

			private Navigator navigator;

			private Tag[] lures;
		}
	}
}
