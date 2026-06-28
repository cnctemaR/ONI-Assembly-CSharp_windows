using System;
using UnityEngine;

public class AlgaeHabitat : StateMachineComponent<AlgaeHabitat.SMInstance>
{
	protected override void OnPrefabInit()
	{
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		component.randomiseLoopedOffset = true;
		base.OnPrefabInit();
		base.GetComponent<Storage>().choreType = Db.Get().ChoreTypes.FetchCritical;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		GameScheduler.Instance.Schedule("WaterFetchingTutorial", 2f, delegate(object obj)
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_FetchingWater);
		}, null, null);
	}

	[MyCmpGet]
	private Operational operational;

	[SerializeField]
	public float lightBonusMultiplier = 1.1f;

	public class SMInstance : GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat>.GameInstance
	{
		public SMInstance(AlgaeHabitat master)
			: base(master)
		{
			this.converter = master.GetComponent<ElementConverter>();
		}

		public bool HasEnoughMass(SimHashes element)
		{
			return this.converter.HasEnoughMass(element);
		}

		public ElementConverter converter;
	}

	public class States : GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.noAlgae;
			this.root.EventTransition(GameHashes.OperationalChanged, this.notoperational, (AlgaeHabitat.SMInstance smi) => !smi.master.operational.IsOperational).EventTransition(GameHashes.OperationalChanged, this.noAlgae, (AlgaeHabitat.SMInstance smi) => smi.master.operational.IsOperational);
			this.noAlgae.QueueAnim("off", false, null).EventTransition(GameHashes.OnStorageChange, this.gotAlgae, (AlgaeHabitat.SMInstance smi) => smi.HasEnoughMass(SimHashes.Algae)).Enter(delegate(AlgaeHabitat.SMInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			});
			this.notoperational.QueueAnim("off", false, null);
			this.gotAlgae.PlayAnim("on_pre", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.noWater);
			this.lostAlgae.PlayAnim("on_pst", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.noAlgae);
			this.noWater.QueueAnim("on", false, null).EventTransition(GameHashes.OnStorageChange, this.lostAlgae, (AlgaeHabitat.SMInstance smi) => !smi.HasEnoughMass(SimHashes.Algae)).EventTransition(GameHashes.OnStorageChange, this.gotWater, (AlgaeHabitat.SMInstance smi) => smi.HasEnoughMass(SimHashes.Algae) && smi.HasEnoughMass(SimHashes.Water));
			this.gotWater.PlayAnim("working_pre", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.generatingOxygen);
			this.generatingOxygen.Enter(delegate(AlgaeHabitat.SMInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).Exit(delegate(AlgaeHabitat.SMInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			}).Update(delegate(AlgaeHabitat.SMInstance smi)
			{
				int num = Grid.PosToCell(smi.master.transform.position);
				smi.converter.OutputMultiplier = ((Grid.LightCount[num] <= 0) ? 1f : smi.master.lightBonusMultiplier);
			})
				.QueueAnim("working_loop", true, null)
				.EventTransition(GameHashes.OnStorageChange, this.stoppedGeneratingOxygen, (AlgaeHabitat.SMInstance smi) => !smi.HasEnoughMass(SimHashes.Water) || !smi.HasEnoughMass(SimHashes.Algae));
			this.stoppedGeneratingOxygen.PlayAnim("working_pst", KAnim.PlayMode.Once, null).OnAnimQueueComplete(this.stoppedGeneratingOxygenTransition);
			this.stoppedGeneratingOxygenTransition.EventTransition(GameHashes.OnStorageChange, this.noWater, (AlgaeHabitat.SMInstance smi) => !smi.HasEnoughMass(SimHashes.Water)).EventTransition(GameHashes.OnStorageChange, this.lostAlgae, (AlgaeHabitat.SMInstance smi) => !smi.HasEnoughMass(SimHashes.Algae));
		}

		public GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat>.State generatingOxygen;

		public GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat>.State stoppedGeneratingOxygen;

		public GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat>.State stoppedGeneratingOxygenTransition;

		public GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat>.State noWater;

		public GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat>.State noAlgae;

		public GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat>.State gotAlgae;

		public GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat>.State gotWater;

		public GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat>.State lostWater;

		public GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat>.State lostAlgae;

		public GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat>.State notoperational;
	}
}
