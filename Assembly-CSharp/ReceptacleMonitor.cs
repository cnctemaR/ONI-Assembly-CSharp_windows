using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class ReceptacleMonitor : StateMachineComponent<ReceptacleMonitor.StatesInstance>, IGameObjectEffectDescriptor, IWiltCause
{
	public bool Replanted
	{
		get
		{
			return this.replanted;
		}
	}

	private static bool HasReceptacleOperationalComponent(ReceptacleMonitor.StatesInstance smi)
	{
		return smi.ReceptacleObject != null && smi.ReceptacleObject.GetComponent<Operational>() != null;
	}

	private static bool IsReceptacleOperational(ReceptacleMonitor.StatesInstance smi)
	{
		return ReceptacleMonitor.HasReceptacleOperationalComponent(smi) && smi.ReceptacleObject.GetComponent<Operational>().IsOperational;
	}

	private static bool IsReceptacleOperational(ReceptacleMonitor.StatesInstance smi, object obj)
	{
		return ReceptacleMonitor.IsReceptacleOperational(smi);
	}

	private static bool IsReceptacle_NOT_Operational(ReceptacleMonitor.StatesInstance smi, object obj)
	{
		return !ReceptacleMonitor.IsReceptacleOperational(smi);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public PlantablePlot GetReceptacle()
	{
		return (PlantablePlot)base.smi.sm.receptacle.Get(base.smi);
	}

	public void SetReceptacle(PlantablePlot plot = null)
	{
		if (plot == null)
		{
			base.smi.sm.receptacle.Set(null, base.smi, false);
			this.replanted = false;
		}
		else
		{
			base.smi.sm.receptacle.Set(plot, base.smi, false);
			this.replanted = true;
		}
		base.Trigger(-1636776682, null);
	}

	WiltCondition.Condition[] IWiltCause.Conditions
	{
		get
		{
			return new WiltCondition.Condition[] { WiltCondition.Condition.Receptacle };
		}
	}

	public string WiltStateString
	{
		get
		{
			string text = "";
			if (base.smi.IsInsideState(base.smi.sm.domestic.operationalExist.inoperational))
			{
				text += CREATURES.STATUSITEMS.RECEPTACLEINOPERATIONAL.NAME;
			}
			return text;
		}
	}

	public bool HasReceptacle()
	{
		return !base.smi.IsInsideState(base.smi.sm.wild);
	}

	public bool HasOperationalReceptacle()
	{
		return base.smi.IsInsideState(base.smi.sm.domestic.operationalExist.operational);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_RECEPTACLE, UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_RECEPTACLE, Descriptor.DescriptorType.Requirement, false)
		};
	}

	private bool replanted;

	public class StatesInstance : GameStateMachine<ReceptacleMonitor.States, ReceptacleMonitor.StatesInstance, ReceptacleMonitor, object>.GameInstance
	{
		public SingleEntityReceptacle ReceptacleObject
		{
			get
			{
				return base.sm.receptacle.Get(this);
			}
		}

		public StatesInstance(ReceptacleMonitor master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<ReceptacleMonitor.States, ReceptacleMonitor.StatesInstance, ReceptacleMonitor>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.wild;
			base.serializable = StateMachine.SerializeType.Never;
			this.wild.ParamTransition<SingleEntityReceptacle>(this.receptacle, this.domestic, (ReceptacleMonitor.StatesInstance smi, SingleEntityReceptacle p) => p != null);
			this.domestic.ParamTransition<SingleEntityReceptacle>(this.receptacle, this.wild, (ReceptacleMonitor.StatesInstance smi, SingleEntityReceptacle p) => p == null).EnterTransition(this.domestic.operationalExist, new StateMachine<ReceptacleMonitor.States, ReceptacleMonitor.StatesInstance, ReceptacleMonitor, object>.Transition.ConditionCallback(ReceptacleMonitor.HasReceptacleOperationalComponent)).EnterTransition(this.domestic.simple, GameStateMachine<ReceptacleMonitor.States, ReceptacleMonitor.StatesInstance, ReceptacleMonitor, object>.Not(new StateMachine<ReceptacleMonitor.States, ReceptacleMonitor.StatesInstance, ReceptacleMonitor, object>.Transition.ConditionCallback(ReceptacleMonitor.HasReceptacleOperationalComponent)));
			this.domestic.simple.DoNothing();
			this.domestic.operationalExist.EnterTransition(this.domestic.operationalExist.operational, new StateMachine<ReceptacleMonitor.States, ReceptacleMonitor.StatesInstance, ReceptacleMonitor, object>.Transition.ConditionCallback(ReceptacleMonitor.IsReceptacleOperational)).EnterGoTo(this.domestic.operationalExist.inoperational);
			this.domestic.operationalExist.inoperational.EventHandlerTransition(GameHashes.ReceptacleOperational, this.domestic.operationalExist.operational, new Func<ReceptacleMonitor.StatesInstance, object, bool>(ReceptacleMonitor.IsReceptacleOperational));
			this.domestic.operationalExist.operational.EventHandlerTransition(GameHashes.ReceptacleInoperational, this.domestic.operationalExist.inoperational, new Func<ReceptacleMonitor.StatesInstance, object, bool>(ReceptacleMonitor.IsReceptacle_NOT_Operational));
		}

		public StateMachine<ReceptacleMonitor.States, ReceptacleMonitor.StatesInstance, ReceptacleMonitor, object>.ObjectParameter<SingleEntityReceptacle> receptacle;

		public GameStateMachine<ReceptacleMonitor.States, ReceptacleMonitor.StatesInstance, ReceptacleMonitor, object>.State wild;

		public ReceptacleMonitor.States.DomesticState domestic;

		public class DomesticState : GameStateMachine<ReceptacleMonitor.States, ReceptacleMonitor.StatesInstance, ReceptacleMonitor, object>.State
		{
			public GameStateMachine<ReceptacleMonitor.States, ReceptacleMonitor.StatesInstance, ReceptacleMonitor, object>.State simple;

			public ReceptacleMonitor.States.OperationalState operationalExist;
		}

		public class OperationalState : GameStateMachine<ReceptacleMonitor.States, ReceptacleMonitor.StatesInstance, ReceptacleMonitor, object>.State
		{
			public GameStateMachine<ReceptacleMonitor.States, ReceptacleMonitor.StatesInstance, ReceptacleMonitor, object>.State inoperational;

			public GameStateMachine<ReceptacleMonitor.States, ReceptacleMonitor.StatesInstance, ReceptacleMonitor, object>.State operational;
		}
	}
}
