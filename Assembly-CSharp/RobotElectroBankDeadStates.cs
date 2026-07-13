using System;
using UnityEngine;

public class RobotElectroBankDeadStates : GameStateMachine<RobotElectroBankDeadStates, RobotElectroBankDeadStates.Instance, IStateMachineTarget, RobotElectroBankDeadStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.powerdown;
		this.powerdown.DefaultState(this.powerdown.pre).ToggleStatusItem(Db.Get().RobotStatusItems.DeadBatteryFlydo, (RobotElectroBankDeadStates.Instance smi) => smi.gameObject, Db.Get().StatusItemCategories.Main).EventTransition(GameHashes.OnStorageChange, this.powerup.grounded, (RobotElectroBankDeadStates.Instance smi) => RobotElectroBankDeadStates.ElectrobankDelivered(smi))
			.Exit(delegate(RobotElectroBankDeadStates.Instance smi)
			{
				if (GameComps.Fallers.Has(smi.gameObject))
				{
					GameComps.Fallers.Remove(smi.gameObject);
				}
			});
		this.powerdown.pre.PlayAnim("power_down_pre").OnAnimQueueComplete(this.powerdown.fall);
		this.powerdown.fall.PlayAnim("power_down_loop", KAnim.PlayMode.Loop).Enter(delegate(RobotElectroBankDeadStates.Instance smi)
		{
			if (!GameComps.Fallers.Has(smi.gameObject))
			{
				GameComps.Fallers.Add(smi.gameObject, Vector2.zero);
			}
		}).Update(delegate(RobotElectroBankDeadStates.Instance smi, float dt)
		{
			if (!GameComps.Gravities.Has(smi.gameObject))
			{
				smi.GoTo(this.powerdown.landed);
			}
		}, UpdateRate.SIM_200ms, false)
			.EventTransition(GameHashes.Landed, this.powerdown.landed, null);
		this.powerdown.landed.PlayAnim("power_down_pst").Enter(delegate(RobotElectroBankDeadStates.Instance smi)
		{
			smi.GetComponent<LoopingSounds>().PauseSound(GlobalAssets.GetSound("Flydo_flying_LP", false), true);
		}).OnAnimQueueComplete(this.powerdown.dead);
		this.powerdown.dead.PlayAnim("dead_battery").EventTransition(GameHashes.OnStorageChange, this.powerup.grounded, (RobotElectroBankDeadStates.Instance smi) => RobotElectroBankDeadStates.ElectrobankDelivered(smi));
		this.powerup.Exit(delegate(RobotElectroBankDeadStates.Instance smi)
		{
			smi.GetComponent<LoopingSounds>().PauseSound(GlobalAssets.GetSound("Flydo_flying_LP", false), false);
			smi.Get<Brain>().Resume("power up");
		});
		this.powerup.grounded.PlayAnim("battery_change_dead").OnAnimQueueComplete(this.powerup.takeoff);
		this.powerup.takeoff.PlayAnim("power_up").OnAnimQueueComplete(this.behaviourcomplete);
		this.behaviourcomplete.BehaviourComplete(GameTags.Robots.Behaviours.NoElectroBank, false);
	}

	private static bool ElectrobankDelivered(RobotElectroBankDeadStates.Instance smi)
	{
		foreach (Storage storage in smi.gameObject.GetComponents<Storage>())
		{
			if (storage.storageID == GameTags.ChargedPortableBattery)
			{
				return storage.Has(GameTags.ChargedPortableBattery);
			}
		}
		return false;
	}

	public RobotElectroBankDeadStates.PowerDown powerdown;

	public RobotElectroBankDeadStates.PowerUp powerup;

	public GameStateMachine<RobotElectroBankDeadStates, RobotElectroBankDeadStates.Instance, IStateMachineTarget, RobotElectroBankDeadStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
	}

	public class PowerDown : GameStateMachine<RobotElectroBankDeadStates, RobotElectroBankDeadStates.Instance, IStateMachineTarget, RobotElectroBankDeadStates.Def>.State
	{
		public GameStateMachine<RobotElectroBankDeadStates, RobotElectroBankDeadStates.Instance, IStateMachineTarget, RobotElectroBankDeadStates.Def>.State pre;

		public GameStateMachine<RobotElectroBankDeadStates, RobotElectroBankDeadStates.Instance, IStateMachineTarget, RobotElectroBankDeadStates.Def>.State fall;

		public GameStateMachine<RobotElectroBankDeadStates, RobotElectroBankDeadStates.Instance, IStateMachineTarget, RobotElectroBankDeadStates.Def>.State landed;

		public GameStateMachine<RobotElectroBankDeadStates, RobotElectroBankDeadStates.Instance, IStateMachineTarget, RobotElectroBankDeadStates.Def>.State dead;
	}

	public class PowerUp : GameStateMachine<RobotElectroBankDeadStates, RobotElectroBankDeadStates.Instance, IStateMachineTarget, RobotElectroBankDeadStates.Def>.State
	{
		public GameStateMachine<RobotElectroBankDeadStates, RobotElectroBankDeadStates.Instance, IStateMachineTarget, RobotElectroBankDeadStates.Def>.State grounded;

		public GameStateMachine<RobotElectroBankDeadStates, RobotElectroBankDeadStates.Instance, IStateMachineTarget, RobotElectroBankDeadStates.Def>.State takeoff;
	}

	public new class Instance : GameStateMachine<RobotElectroBankDeadStates, RobotElectroBankDeadStates.Instance, IStateMachineTarget, RobotElectroBankDeadStates.Def>.GameInstance
	{
		public Instance(Chore<RobotElectroBankDeadStates.Instance> chore, RobotElectroBankDeadStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Robots.Behaviours.NoElectroBank);
		}
	}
}
