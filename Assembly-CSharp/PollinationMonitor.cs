using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class PollinationMonitor : GameStateMachine<PollinationMonitor, PollinationMonitor.StatesInstance, IStateMachineTarget, PollinationMonitor.Def>
{
	public static bool IsPollinationEffect(Effect effect)
	{
		return Array.IndexOf<HashedString>(PollinationMonitor.PollinationEffects, effect.IdHash) != -1;
	}

	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.initialize;
		this.initialize.Enter(delegate(PollinationMonitor.StatesInstance smi)
		{
			if (smi.effects == null)
			{
				smi.GoTo(this.not_pollinated);
				return;
			}
			bool flag = false;
			foreach (HashedString hashedString in PollinationMonitor.PollinationEffects)
			{
				if (smi.effects.HasEffect(hashedString))
				{
					flag = true;
					break;
				}
			}
			smi.GoTo(flag ? this.pollinated : this.not_pollinated);
		});
		this.not_pollinated.Enter(delegate(PollinationMonitor.StatesInstance smi)
		{
			smi.Trigger(-200207042, false);
		}).EventHandler(GameHashes.EffectAdded, delegate(PollinationMonitor.StatesInstance smi, object data)
		{
			if (PollinationMonitor.IsPollinationEffect(data as Effect))
			{
				smi.GoTo(this.pollinated);
			}
		});
		this.pollinated.Enter(delegate(PollinationMonitor.StatesInstance smi)
		{
			smi.Trigger(-200207042, true);
		}).EventHandler(GameHashes.EffectRemoved, delegate(PollinationMonitor.StatesInstance smi, object data)
		{
			if (!PollinationMonitor.IsPollinationEffect(data as Effect))
			{
				return;
			}
			if (smi.effects == null)
			{
				smi.GoTo(this.not_pollinated);
				return;
			}
			bool flag2 = false;
			foreach (HashedString hashedString2 in PollinationMonitor.PollinationEffects)
			{
				if (smi.effects.HasEffect(hashedString2))
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				smi.GoTo(this.not_pollinated);
			}
		});
	}

	public static readonly string INITIALLY_POLLINATED_EFFECT = "InitiallyPollinated";

	public static readonly HashedString[] PollinationEffects = new HashedString[]
	{
		PollinationMonitor.INITIALLY_POLLINATED_EFFECT,
		"DivergentCropTended",
		"DivergentCropTendedWorm",
		"ButterflyPollinated"
	};

	public GameStateMachine<PollinationMonitor, PollinationMonitor.StatesInstance, IStateMachineTarget, PollinationMonitor.Def>.State initialize;

	public GameStateMachine<PollinationMonitor, PollinationMonitor.StatesInstance, IStateMachineTarget, PollinationMonitor.Def>.State not_pollinated;

	public GameStateMachine<PollinationMonitor, PollinationMonitor.StatesInstance, IStateMachineTarget, PollinationMonitor.Def>.State pollinated;

	private readonly StateMachine<PollinationMonitor, PollinationMonitor.StatesInstance, IStateMachineTarget, PollinationMonitor.Def>.BoolParameter spawn_pollinated = new StateMachine<PollinationMonitor, PollinationMonitor.StatesInstance, IStateMachineTarget, PollinationMonitor.Def>.BoolParameter(false);

	public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
	{
		public List<Descriptor> GetDescriptors(GameObject go)
		{
			return new List<Descriptor>
			{
				new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_POLLINATION, UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_POLLINATION, Descriptor.DescriptorType.Requirement, false)
			};
		}
	}

	public class StatesInstance : GameStateMachine<PollinationMonitor, PollinationMonitor.StatesInstance, IStateMachineTarget, PollinationMonitor.Def>.GameInstance, IWiltCause
	{
		public StatesInstance(IStateMachineTarget master, PollinationMonitor.Def def)
			: base(master, def)
		{
			this.effects = base.GetComponent<Effects>();
			base.Subscribe(1119167081, delegate(object _)
			{
				base.sm.spawn_pollinated.Set(true, this, false);
			});
		}

		public override void StartSM()
		{
			base.StartSM();
			if (base.sm.spawn_pollinated.Get(this))
			{
				base.sm.spawn_pollinated.Set(false, this, false);
				if (this.effects != null)
				{
					this.effects.Add(PollinationMonitor.INITIALLY_POLLINATED_EFFECT, true).timeRemaining *= global::UnityEngine.Random.Range(0.75f, 1f);
				}
			}
		}

		public WiltCondition.Condition[] Conditions
		{
			get
			{
				return new WiltCondition.Condition[] { WiltCondition.Condition.Pollination };
			}
		}

		public string WiltStateString
		{
			get
			{
				if (!base.IsInsideState(base.sm.not_pollinated))
				{
					return "";
				}
				return Db.Get().CreatureStatusItems.NotPollinated.GetName(this);
			}
		}

		public Effects effects;
	}
}
