using System;
using Klei.AI;

public class HandSanitizer : StateMachineComponent<HandSanitizer.SMInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.gameObject.FindOrAddComponent<HandSanitizer.Workable>();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	[NonSerialized]
	public float massConsumedPerUse = 1f;

	public class SMInstance : GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer>.GameInstance
	{
		public SMInstance(HandSanitizer master)
			: base(master)
		{
		}

		public bool HasSufficientMass()
		{
			bool flag = false;
			PrimaryElement primaryElement = base.GetComponent<Storage>().FindPrimaryElement(SimHashes.BleachStone);
			if (primaryElement != null)
			{
				flag = primaryElement.Mass > 0f;
			}
			return flag;
		}

		private void OnUseComplete(Chore chore)
		{
			PrimaryElement primaryElement = base.smi.master.GetComponent<Storage>().FindPrimaryElement(SimHashes.BleachStone);
			primaryElement.Mass -= base.smi.master.massConsumedPerUse;
		}

		private const SimHashes consumedElement = SimHashes.BleachStone;
	}

	public class States : GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.notready;
			base.serializable = true;
			this.notready.PlayAnim("off", KAnim.PlayMode.Once, null).EventTransition(GameHashes.OnStorageChange, this.ready, (HandSanitizer.SMInstance smi) => smi.HasSufficientMass());
			this.ready.DefaultState(this.ready.free).ToggleChore((HandSanitizer.SMInstance smi) => new WorkChore<HandSanitizer.Workable>(Db.Get().ChoreTypes.WashHands, smi.master, null, true, null, null, null, false, null, true, default(Tag), null, false, true), this.ready, true);
			this.ready.free.EventTransition(GameHashes.OnStorageChange, this.notready, (HandSanitizer.SMInstance smi) => !smi.HasSufficientMass()).PlayAnim("on", KAnim.PlayMode.Once, null).EventTransition(GameHashes.WorkStarted, this.ready.occupied, null);
			this.ready.occupied.Enter(delegate(HandSanitizer.SMInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(true, false);
				KAnimControllerBase component = smi.GetComponent<KAnimControllerBase>();
				component.Play("working_pre", KAnim.PlayMode.Once, 1f, 0f);
				component.Queue("working_loop", KAnim.PlayMode.Loop, 1f, 0f);
			}).EventTransition(GameHashes.WorkCompleted, this.ready.free, null).EventTransition(GameHashes.WorkAborted, this.ready.free, null)
				.Exit(delegate(HandSanitizer.SMInstance smi)
				{
					smi.GetComponent<Operational>().SetActive(false, false);
					KAnimControllerBase component2 = smi.GetComponent<KAnimControllerBase>();
					component2.Play("working_pst", KAnim.PlayMode.Once, 1f, 0f);
				});
		}

		public GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer>.State notready;

		public HandSanitizer.States.ReadyStates ready;

		public class ReadyStates : GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer>.State
		{
			public GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer>.State free;

			public GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer>.State occupied;
		}
	}

	public class Workable : BuildingWorkable
	{
		protected override void OnCompleteWork(Worker worker)
		{
			worker.GetComponent<Effects>().Remove("DirtyHands");
			base.OnCompleteWork(worker);
		}
	}
}
