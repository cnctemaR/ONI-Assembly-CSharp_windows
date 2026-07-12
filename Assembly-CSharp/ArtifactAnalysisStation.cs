using System;
using System.Collections.Generic;
using KSerialization;

public class ArtifactAnalysisStation : GameStateMachine<ArtifactAnalysisStation, ArtifactAnalysisStation.StatesInstance, IStateMachineTarget, ArtifactAnalysisStation.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.inoperational;
		this.inoperational.EventTransition(GameHashes.OperationalChanged, this.ready, new StateMachine<ArtifactAnalysisStation, ArtifactAnalysisStation.StatesInstance, IStateMachineTarget, ArtifactAnalysisStation.Def>.Transition.ConditionCallback(this.IsOperational));
		this.operational.EventTransition(GameHashes.OperationalChanged, this.inoperational, GameStateMachine<ArtifactAnalysisStation, ArtifactAnalysisStation.StatesInstance, IStateMachineTarget, ArtifactAnalysisStation.Def>.Not(new StateMachine<ArtifactAnalysisStation, ArtifactAnalysisStation.StatesInstance, IStateMachineTarget, ArtifactAnalysisStation.Def>.Transition.ConditionCallback(this.IsOperational))).EventTransition(GameHashes.OnStorageChange, this.ready, new StateMachine<ArtifactAnalysisStation, ArtifactAnalysisStation.StatesInstance, IStateMachineTarget, ArtifactAnalysisStation.Def>.Transition.ConditionCallback(this.HasArtifactToStudy));
		this.ready.EventTransition(GameHashes.OperationalChanged, this.inoperational, GameStateMachine<ArtifactAnalysisStation, ArtifactAnalysisStation.StatesInstance, IStateMachineTarget, ArtifactAnalysisStation.Def>.Not(new StateMachine<ArtifactAnalysisStation, ArtifactAnalysisStation.StatesInstance, IStateMachineTarget, ArtifactAnalysisStation.Def>.Transition.ConditionCallback(this.IsOperational))).EventTransition(GameHashes.OnStorageChange, this.operational, GameStateMachine<ArtifactAnalysisStation, ArtifactAnalysisStation.StatesInstance, IStateMachineTarget, ArtifactAnalysisStation.Def>.Not(new StateMachine<ArtifactAnalysisStation, ArtifactAnalysisStation.StatesInstance, IStateMachineTarget, ArtifactAnalysisStation.Def>.Transition.ConditionCallback(this.HasArtifactToStudy))).ToggleChore(new Func<ArtifactAnalysisStation.StatesInstance, Chore>(this.CreateChore), this.operational);
	}

	private bool HasArtifactToStudy(ArtifactAnalysisStation.StatesInstance smi)
	{
		return smi.storage.GetMassAvailable(GameTags.CharmedArtifact) >= 1f;
	}

	private bool IsOperational(ArtifactAnalysisStation.StatesInstance smi)
	{
		return smi.GetComponent<Operational>().IsOperational;
	}

	private Chore CreateChore(ArtifactAnalysisStation.StatesInstance smi)
	{
		return new WorkChore<ArtifactAnalysisStationWorkable>(Db.Get().ChoreTypes.AnalyzeArtifact, smi.workable, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
	}

	public GameStateMachine<ArtifactAnalysisStation, ArtifactAnalysisStation.StatesInstance, IStateMachineTarget, ArtifactAnalysisStation.Def>.State inoperational;

	public GameStateMachine<ArtifactAnalysisStation, ArtifactAnalysisStation.StatesInstance, IStateMachineTarget, ArtifactAnalysisStation.Def>.State operational;

	public GameStateMachine<ArtifactAnalysisStation, ArtifactAnalysisStation.StatesInstance, IStateMachineTarget, ArtifactAnalysisStation.Def>.State ready;

	public class Def : StateMachine.BaseDef
	{
	}

	public class StatesInstance : GameStateMachine<ArtifactAnalysisStation, ArtifactAnalysisStation.StatesInstance, IStateMachineTarget, ArtifactAnalysisStation.Def>.GameInstance
	{
		public StatesInstance(IStateMachineTarget master, ArtifactAnalysisStation.Def def)
			: base(master, def)
		{
			this.workable.statesInstance = this;
		}

		public override void StartSM()
		{
			base.StartSM();
		}

		[MyCmpReq]
		public Storage storage;

		[MyCmpReq]
		public ManualDeliveryKG manualDelivery;

		[MyCmpReq]
		public ArtifactAnalysisStationWorkable workable;

		[Serialize]
		private HashSet<Tag> forbiddenSeeds;
	}
}
