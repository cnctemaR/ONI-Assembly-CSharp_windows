using System;
using UnityEngine;

public class MucusSecretionStates : GameStateMachine<MucusSecretionStates, MucusSecretionStates.Instance, IStateMachineTarget, MucusSecretionStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.secretePre;
		this.secretePre.QueueAnim("poop", false, null).Exit(new StateMachine<MucusSecretionStates, MucusSecretionStates.Instance, IStateMachineTarget, MucusSecretionStates.Def>.State.Callback(MucusSecretionStates.Secrete)).OnAnimQueueComplete(this.behaviourComplete);
		this.behaviourComplete.BehaviourComplete(GameTags.Creatures.Behaviours.SecretingMucusBehavior, false);
	}

	private static void Secrete(MucusSecretionStates.Instance smi)
	{
		smi.position = smi.transform.GetPosition();
		MoistureMonitor.Instance smi2 = smi.GetSMI<MoistureMonitor.Instance>();
		if (smi2 != null)
		{
			smi2.ProduceLubricant();
		}
	}

	public GameStateMachine<MucusSecretionStates, MucusSecretionStates.Instance, IStateMachineTarget, MucusSecretionStates.Def>.State secretePre;

	public GameStateMachine<MucusSecretionStates, MucusSecretionStates.Instance, IStateMachineTarget, MucusSecretionStates.Def>.State behaviourComplete;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<MucusSecretionStates, MucusSecretionStates.Instance, IStateMachineTarget, MucusSecretionStates.Def>.GameInstance
	{
		public Instance(Chore<MucusSecretionStates.Instance> chore, MucusSecretionStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Behaviours.SecretingMucusBehavior);
		}

		public Vector3 position;
	}
}
