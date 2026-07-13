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
		this.nolure.PreBrainUpdate(delegate(LureableMonitor.Instance smi)
		{
			smi.FindLure();
		}).ParamTransition<GameObject>(this.targetLure, this.haslure, (LureableMonitor.Instance smi, GameObject p) => p != null);
		this.haslure.ParamTransition<GameObject>(this.targetLure, this.nolure, (LureableMonitor.Instance smi, GameObject p) => p == null).PreBrainUpdate(delegate(LureableMonitor.Instance smi)
		{
			smi.FindLure();
		}).ToggleBehaviour(GameTags.Creatures.MoveToLure, (LureableMonitor.Instance smi) => smi.HasLure(), delegate(LureableMonitor.Instance smi)
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
			List<Descriptor> list = new List<Descriptor>();
			foreach (Tag tag in this.lures)
			{
				if (tag == GameTags.Creatures.FlyersLure)
				{
					list.Add(new Descriptor(UI.BUILDINGEFFECTS.CAPTURE_METHOD_FLYING_TRAP, UI.BUILDINGEFFECTS.TOOLTIPS.CAPTURE_METHOD_FLYING_TRAP, Descriptor.DescriptorType.Effect, false));
				}
				else if (tag == GameTags.Creatures.FishTrapLure)
				{
					list.Add(new Descriptor(UI.BUILDINGEFFECTS.CAPTURE_METHOD_FISH_TRAP, UI.BUILDINGEFFECTS.TOOLTIPS.CAPTURE_METHOD_FISH_TRAP, Descriptor.DescriptorType.Effect, false));
				}
			}
			return list;
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

		private static bool FindLureCounter(object obj, LureableMonitor.Instance.FindLureCounterContext context)
		{
			Lure.Instance instance = obj as Lure.Instance;
			if (instance == null || !instance.IsActive() || !instance.HasAnyLure(context.inst.def.lures))
			{
				return true;
			}
			int navigationCost = context.inst.navigator.GetNavigationCost(Grid.PosToCell(instance.transform.GetPosition()), instance.LurePoints);
			if (navigationCost != -1 && (context.cost == -1 || navigationCost < context.cost))
			{
				context.cost = navigationCost;
				context.result = instance.gameObject;
			}
			return true;
		}

		public void FindLure()
		{
			LureableMonitor.Instance.context.inst = this;
			LureableMonitor.Instance.context.cost = -1;
			LureableMonitor.Instance.context.result = null;
			GameScenePartitioner.Instance.AsyncSafeVisit<LureableMonitor.Instance.FindLureCounterContext>(Grid.PosToCell(base.smi.transform.GetPosition()), 1, GameScenePartitioner.Instance.lure, new Func<object, LureableMonitor.Instance.FindLureCounterContext, bool>(LureableMonitor.Instance.FindLureCounter), LureableMonitor.Instance.context);
			base.sm.targetLure.Set(LureableMonitor.Instance.context.result, this, false);
		}

		public bool HasLure()
		{
			return base.sm.targetLure.Get(this) != null;
		}

		public GameObject GetTargetLure()
		{
			return base.sm.targetLure.Get(this);
		}

		[MyCmpReq]
		private Navigator navigator;

		private static LureableMonitor.Instance.FindLureCounterContext context = new LureableMonitor.Instance.FindLureCounterContext();

		private class FindLureCounterContext
		{
			public LureableMonitor.Instance inst;

			public int cost;

			public GameObject result;
		}
	}
}
