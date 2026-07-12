using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class CargoDropperMinion : GameStateMachine<CargoDropperMinion, CargoDropperMinion.StatesInstance, IStateMachineTarget, CargoDropperMinion.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.notLanded;
		this.root.ParamTransition<bool>(this.hasLanded, this.complete, GameStateMachine<CargoDropperMinion, CargoDropperMinion.StatesInstance, IStateMachineTarget, CargoDropperMinion.Def>.IsTrue);
		this.notLanded.EventHandlerTransition(GameHashes.JettisonCargo, this.landed, (CargoDropperMinion.StatesInstance smi, object obj) => true);
		this.landed.Enter(delegate(CargoDropperMinion.StatesInstance smi)
		{
			smi.JettisonCargo(null);
			smi.GoTo(this.exiting);
		});
		this.exiting.Update(delegate(CargoDropperMinion.StatesInstance smi, float dt)
		{
			if (!smi.SyncMinionExitAnimation())
			{
				smi.GoTo(this.complete);
			}
		}, UpdateRate.SIM_200ms, false);
		this.complete.Enter(delegate(CargoDropperMinion.StatesInstance smi)
		{
			this.hasLanded.Set(true, smi);
		});
	}

	private GameStateMachine<CargoDropperMinion, CargoDropperMinion.StatesInstance, IStateMachineTarget, CargoDropperMinion.Def>.State notLanded;

	private GameStateMachine<CargoDropperMinion, CargoDropperMinion.StatesInstance, IStateMachineTarget, CargoDropperMinion.Def>.State landed;

	private GameStateMachine<CargoDropperMinion, CargoDropperMinion.StatesInstance, IStateMachineTarget, CargoDropperMinion.Def>.State exiting;

	private GameStateMachine<CargoDropperMinion, CargoDropperMinion.StatesInstance, IStateMachineTarget, CargoDropperMinion.Def>.State complete;

	public StateMachine<CargoDropperMinion, CargoDropperMinion.StatesInstance, IStateMachineTarget, CargoDropperMinion.Def>.BoolParameter hasLanded = new StateMachine<CargoDropperMinion, CargoDropperMinion.StatesInstance, IStateMachineTarget, CargoDropperMinion.Def>.BoolParameter(false);

	public class Def : StateMachine.BaseDef
	{
		public Vector3 dropOffset;

		public string kAnimName;

		public string animName;

		public Grid.SceneLayer animLayer = Grid.SceneLayer.Move;

		public bool notifyOnJettison;
	}

	public class StatesInstance : GameStateMachine<CargoDropperMinion, CargoDropperMinion.StatesInstance, IStateMachineTarget, CargoDropperMinion.Def>.GameInstance
	{
		public StatesInstance(IStateMachineTarget master, CargoDropperMinion.Def def)
			: base(master, def)
		{
		}

		public void JettisonCargo(object data = null)
		{
			Vector3 vector = base.master.transform.GetPosition() + base.def.dropOffset;
			MinionStorage component = base.GetComponent<MinionStorage>();
			if (component != null)
			{
				List<MinionStorage.Info> storedMinionInfo = component.GetStoredMinionInfo();
				for (int i = storedMinionInfo.Count - 1; i >= 0; i--)
				{
					MinionStorage.Info info = storedMinionInfo[i];
					GameObject gameObject = component.DeserializeMinion(info.id, vector);
					this.escapingMinion = gameObject.GetComponent<MinionIdentity>();
					gameObject.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
					ChoreProvider component2 = gameObject.GetComponent<ChoreProvider>();
					if (component2 != null)
					{
						this.exitAnimChore = new EmoteChore(component2, Db.Get().ChoreTypes.EmoteHighPriority, base.def.kAnimName, new HashedString[] { base.def.animName }, KAnim.PlayMode.Once, false);
						Vector3 position = gameObject.transform.GetPosition();
						position.z = Grid.GetLayerZ(base.def.animLayer);
						gameObject.transform.SetPosition(position);
					}
					if (base.def.notifyOnJettison)
					{
						gameObject.GetComponent<Notifier>().Add(this.CreateCrashLandedNotification(), "");
					}
				}
			}
		}

		public bool SyncMinionExitAnimation()
		{
			if (this.escapingMinion != null && this.exitAnimChore != null && !this.exitAnimChore.isComplete)
			{
				KBatchedAnimController component = this.escapingMinion.GetComponent<KBatchedAnimController>();
				KBatchedAnimController component2 = base.master.GetComponent<KBatchedAnimController>();
				if (component2.CurrentAnim.name == base.def.animName)
				{
					component.SetElapsedTime(component2.GetElapsedTime());
					return true;
				}
			}
			return false;
		}

		public Notification CreateCrashLandedNotification()
		{
			return new Notification(MISC.NOTIFICATIONS.DUPLICANT_CRASH_LANDED.NAME, NotificationType.Bad, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.DUPLICANT_CRASH_LANDED.TOOLTIP + notificationList.ReduceMessages(false), null, true, 0f, null, null, null, true);
		}

		public MinionIdentity escapingMinion;

		public Chore exitAnimChore;
	}
}
