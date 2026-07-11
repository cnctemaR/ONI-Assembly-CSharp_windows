using System;
using UnityEngine;

namespace Klei.AI
{
	public class PeriodicEmoteDisease : Disease.DiseaseComponent
	{
		public PeriodicEmoteDisease(HashedString kanim, HashedString[] anims, float cooldown)
		{
			this.kanim = kanim;
			this.anims = anims;
			this.cooldown = cooldown;
		}

		public override object OnInfect(GameObject go, DiseaseInstance diseaseInstance)
		{
			PeriodicEmoteDisease.StatesInstance statesInstance = new PeriodicEmoteDisease.StatesInstance(diseaseInstance, this);
			statesInstance.StartSM();
			return statesInstance;
		}

		public override void OnCure(GameObject go, object instance_data)
		{
			PeriodicEmoteDisease.StatesInstance statesInstance = (PeriodicEmoteDisease.StatesInstance)instance_data;
			statesInstance.StopSM("Cured");
		}

		private HashedString kanim;

		private HashedString[] anims;

		private float cooldown;

		public class StatesInstance : GameStateMachine<PeriodicEmoteDisease.States, PeriodicEmoteDisease.StatesInstance, DiseaseInstance, object>.GameInstance
		{
			public StatesInstance(DiseaseInstance master, PeriodicEmoteDisease periodicEmoteDisease)
				: base(master)
			{
				this.periodicEmoteDisease = periodicEmoteDisease;
			}

			public PeriodicEmoteDisease periodicEmoteDisease;
		}

		public class States : GameStateMachine<PeriodicEmoteDisease.States, PeriodicEmoteDisease.StatesInstance, DiseaseInstance>
		{
			public override void InitializeStates(out StateMachine.BaseState default_state)
			{
				default_state = this.emoting;
				this.emoting.ToggleChore((PeriodicEmoteDisease.StatesInstance smi) => new EmoteChore(smi.master, Db.Get().ChoreTypes.Emote, smi.periodicEmoteDisease.kanim, smi.periodicEmoteDisease.anims, KAnim.PlayMode.Once, false), this.cooldown);
				this.cooldown.ScheduleGoTo((PeriodicEmoteDisease.StatesInstance smi) => smi.periodicEmoteDisease.cooldown, this.emoting);
			}

			public GameStateMachine<PeriodicEmoteDisease.States, PeriodicEmoteDisease.StatesInstance, DiseaseInstance, object>.State emoting;

			public GameStateMachine<PeriodicEmoteDisease.States, PeriodicEmoteDisease.StatesInstance, DiseaseInstance, object>.State cooldown;
		}
	}
}
