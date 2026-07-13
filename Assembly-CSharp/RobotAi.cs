using System;
using UnityEngine;

public class RobotAi : GameStateMachine<RobotAi, RobotAi.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.ToggleStateMachine((RobotAi.Instance smi) => new DeathMonitor.Instance(smi.master, new DeathMonitor.Def())).Enter(delegate(RobotAi.Instance smi)
		{
			if (smi.HasTag(GameTags.Dead))
			{
				smi.GoTo(this.dead);
				return;
			}
			smi.GoTo(this.alive);
		});
		this.alive.DefaultState(this.alive.normal).TagTransition(GameTags.Dead, this.dead, false).Toggle("Toggle Component Registration", delegate(RobotAi.Instance smi)
		{
			RobotAi.ToggleRegistration(smi, true);
		}, delegate(RobotAi.Instance smi)
		{
			RobotAi.ToggleRegistration(smi, false);
		});
		this.alive.normal.TagTransition(GameTags.Stored, this.alive.stored, false).Enter(delegate(RobotAi.Instance smi)
		{
			if (!smi.HasTag(GameTags.Robots.Models.FetchDrone))
			{
				smi.fallMonitor = new FallMonitor.Instance(smi.master, false, null);
				smi.fallMonitor.StartSM();
			}
		}).Exit(delegate(RobotAi.Instance smi)
		{
			if (smi.fallMonitor != null)
			{
				smi.fallMonitor.StopSM("StoredRobotAI");
			}
		});
		this.alive.stored.PlayAnim("in_storage").TagTransition(GameTags.Stored, this.alive.normal, true).ToggleBrain("stored")
			.Enter(delegate(RobotAi.Instance smi)
			{
				smi.GetComponent<Navigator>().Pause("stored");
			})
			.Exit(delegate(RobotAi.Instance smi)
			{
				smi.GetComponent<Navigator>().Unpause("unstored");
			});
		this.dead.ToggleBrain("dead").ToggleComponentIfFound<Deconstructable>(false).ToggleStateMachine((RobotAi.Instance smi) => new FallWhenDeadMonitor.Instance(smi.master))
			.Enter("RefreshUserMenu", delegate(RobotAi.Instance smi)
			{
				smi.RefreshUserMenu();
			})
			.Enter("DropStorage", delegate(RobotAi.Instance smi)
			{
				smi.GetComponent<Storage>().DropAll(false, false, default(Vector3), true, null);
			})
			.Enter("Delete", new StateMachine<RobotAi, RobotAi.Instance, IStateMachineTarget, object>.State.Callback(RobotAi.DeleteOnDeath));
	}

	public static void DeleteOnDeath(RobotAi.Instance smi)
	{
		if (((RobotAi.Def)smi.def).DeleteOnDead)
		{
			smi.gameObject.DeleteObject();
		}
	}

	private static void ToggleRegistration(RobotAi.Instance smi, bool register)
	{
		if (register)
		{
			Components.LiveRobotsIdentities.Add(smi);
			return;
		}
		Components.LiveRobotsIdentities.Remove(smi);
	}

	public RobotAi.AliveStates alive;

	public GameStateMachine<RobotAi, RobotAi.Instance, IStateMachineTarget, object>.State dead;

	public class Def : StateMachine.BaseDef
	{
		public bool DeleteOnDead;
	}

	public class AliveStates : GameStateMachine<RobotAi, RobotAi.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<RobotAi, RobotAi.Instance, IStateMachineTarget, object>.State normal;

		public GameStateMachine<RobotAi, RobotAi.Instance, IStateMachineTarget, object>.State stored;
	}

	public new class Instance : GameStateMachine<RobotAi, RobotAi.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master, RobotAi.Def def)
			: base(master, def)
		{
			ChoreConsumer component = base.GetComponent<ChoreConsumer>();
			component.AddUrge(Db.Get().Urges.EmoteHighPriority);
			component.AddUrge(Db.Get().Urges.EmoteIdle);
			this.onBeginChoreHandlerID = base.Subscribe(-1988963660, new Action<object>(this.OnBeginChore));
		}

		private void OnBeginChore(object data)
		{
			Storage component = base.GetComponent<Storage>();
			if (component != null)
			{
				component.DropAll(false, false, default(Vector3), true, null);
			}
		}

		protected override void OnCleanUp()
		{
			base.Unsubscribe(ref this.onBeginChoreHandlerID);
			base.OnCleanUp();
		}

		public void RefreshUserMenu()
		{
			Game.Instance.userMenu.Refresh(base.master.gameObject);
		}

		public FallMonitor.Instance fallMonitor;

		private int onBeginChoreHandlerID;
	}
}
