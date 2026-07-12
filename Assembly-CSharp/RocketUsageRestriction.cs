using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class RocketUsageRestriction : GameStateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.root.Enter(delegate(RocketUsageRestriction.StatesInstance smi)
		{
			if (DlcManager.FeatureClusterSpaceEnabled() && smi.master.gameObject.GetMyWorld().IsModuleInterior)
			{
				smi.Subscribe(493375141, new Action<object>(smi.OnRefreshUserMenu));
				smi.GoToRestrictionState();
				return;
			}
			smi.StopSM("Not inside rocket or no cluster space");
		});
		this.restriction.Enter(new StateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.State.Callback(this.AquireRocketControlStation)).Enter(delegate(RocketUsageRestriction.StatesInstance smi)
		{
			Components.RocketControlStations.OnAdd += new Action<RocketControlStation>(smi.ControlStationBuilt);
		}).Exit(delegate(RocketUsageRestriction.StatesInstance smi)
		{
			Components.RocketControlStations.OnAdd -= new Action<RocketControlStation>(smi.ControlStationBuilt);
		});
		this.restriction.uncontrolled.ToggleStatusItem(Db.Get().BuildingStatusItems.NoRocketRestriction, null).Enter(delegate(RocketUsageRestriction.StatesInstance smi)
		{
			this.RestrictUsage(smi, false);
		});
		this.restriction.controlled.DefaultState(this.restriction.controlled.nostation);
		this.restriction.controlled.nostation.Enter(new StateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.State.Callback(this.OnRocketRestrictionChanged)).ParamTransition<GameObject>(this.rocketControlStation, this.restriction.controlled.controlled, GameStateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.IsNotNull);
		this.restriction.controlled.controlled.OnTargetLost(this.rocketControlStation, this.restriction.controlled.nostation).Enter(new StateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.State.Callback(this.OnRocketRestrictionChanged)).Target(this.rocketControlStation)
			.EventHandler(GameHashes.RocketRestrictionChanged, new StateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.State.Callback(this.OnRocketRestrictionChanged))
			.Target(this.masterTarget);
	}

	private void OnRocketRestrictionChanged(RocketUsageRestriction.StatesInstance smi)
	{
		this.RestrictUsage(smi, smi.BuildingRestrictionsActive());
	}

	private void RestrictUsage(RocketUsageRestriction.StatesInstance smi, bool restrict)
	{
		smi.master.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.RocketRestrictionInactive, !restrict && smi.isControlled, null);
		if (smi.isRestrictionApplied == restrict)
		{
			return;
		}
		smi.isRestrictionApplied = restrict;
		smi.operational.SetFlag(RocketUsageRestriction.rocketUsageAllowed, !smi.def.restrictOperational || !restrict);
		smi.master.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.RocketRestrictionActive, restrict, null);
		Storage[] components = smi.master.gameObject.GetComponents<Storage>();
		if (components != null && components.Length != 0)
		{
			for (int i = 0; i < components.Length; i++)
			{
				if (restrict)
				{
					smi.previousStorageAllowItemRemovalStates = new bool[components.Length];
					smi.previousStorageAllowItemRemovalStates[i] = components[i].allowItemRemoval;
					components[i].allowItemRemoval = false;
				}
				else if (smi.previousStorageAllowItemRemovalStates != null && i < smi.previousStorageAllowItemRemovalStates.Length)
				{
					components[i].allowItemRemoval = smi.previousStorageAllowItemRemovalStates[i];
				}
				foreach (GameObject gameObject in components[i].items)
				{
					gameObject.Trigger(-778359855, components[i]);
				}
			}
		}
		Ownable component = smi.master.GetComponent<Ownable>();
		if (restrict && component != null && component.IsAssigned())
		{
			component.Unassign();
		}
	}

	private void AquireRocketControlStation(RocketUsageRestriction.StatesInstance smi)
	{
		if (!this.rocketControlStation.IsNull(smi))
		{
			return;
		}
		foreach (object obj in Components.RocketControlStations)
		{
			RocketControlStation rocketControlStation = (RocketControlStation)obj;
			if (rocketControlStation.GetMyWorldId() == smi.GetMyWorldId())
			{
				this.rocketControlStation.Set(rocketControlStation, smi);
			}
		}
	}

	public static readonly Operational.Flag rocketUsageAllowed = new Operational.Flag("rocketUsageAllowed", Operational.Flag.Type.Requirement);

	private StateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.TargetParameter rocketControlStation;

	public RocketUsageRestriction.RestrictionStates restriction;

	public class Def : StateMachine.BaseDef
	{
		public override void Configure(GameObject prefab)
		{
			RocketControlStation.CONTROLLED_BUILDINGS.Add(prefab.PrefabID());
		}

		public bool initialControlledStateWhenBuilt = true;

		public bool restrictOperational = true;
	}

	public class ControlledStates : GameStateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.State
	{
		public GameStateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.State nostation;

		public GameStateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.State controlled;
	}

	public class RestrictionStates : GameStateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.State
	{
		public GameStateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.State uncontrolled;

		public RocketUsageRestriction.ControlledStates controlled;
	}

	public class StatesInstance : GameStateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.GameInstance
	{
		public StatesInstance(IStateMachineTarget master, RocketUsageRestriction.Def def)
			: base(master, def)
		{
			this.isControlled = def.initialControlledStateWhenBuilt;
		}

		public void OnRefreshUserMenu(object data)
		{
			KIconButtonMenu.ButtonInfo buttonInfo;
			if (this.isControlled)
			{
				buttonInfo = new KIconButtonMenu.ButtonInfo("action_rocket_restriction_uncontrolled", UI.USERMENUACTIONS.ROCKETUSAGERESTRICTION.NAME_UNCONTROLLED, new global::System.Action(this.OnChange), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.ROCKETUSAGERESTRICTION.TOOLTIP_UNCONTROLLED, true);
			}
			else
			{
				buttonInfo = new KIconButtonMenu.ButtonInfo("action_rocket_restriction_controlled", UI.USERMENUACTIONS.ROCKETUSAGERESTRICTION.NAME_CONTROLLED, new global::System.Action(this.OnChange), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.ROCKETUSAGERESTRICTION.TOOLTIP_CONTROLLED, true);
			}
			Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo, 11f);
		}

		public void ControlStationBuilt(object o)
		{
			base.sm.AquireRocketControlStation(base.smi);
		}

		private void OnChange()
		{
			this.isControlled = !this.isControlled;
			this.GoToRestrictionState();
		}

		public void GoToRestrictionState()
		{
			if (base.smi.isControlled)
			{
				base.smi.GoTo(base.sm.restriction.controlled);
				return;
			}
			base.smi.GoTo(base.sm.restriction.uncontrolled);
		}

		public bool BuildingRestrictionsActive()
		{
			return this.isControlled && !base.sm.rocketControlStation.IsNull(base.smi) && base.sm.rocketControlStation.Get<RocketControlStation>(base.smi).BuildingRestrictionsActive;
		}

		[MyCmpGet]
		public Operational operational;

		public bool[] previousStorageAllowItemRemovalStates;

		[Serialize]
		public bool isControlled = true;

		public bool isRestrictionApplied;
	}
}
