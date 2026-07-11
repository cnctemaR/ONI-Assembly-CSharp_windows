using System;
using UnityEngine;

namespace Klei.AI
{
	public class PeriodicEmoteSickness : Sickness.SicknessComponent
	{
		public PeriodicEmoteSickness(HashedString kanim, HashedString[] anims, float cooldown)
		{
			this.kanim = kanim;
			this.anims = anims;
			this.cooldown = cooldown;
		}

		public override object OnInfect(GameObject go, SicknessInstance diseaseInstance)
		{
			PeriodicEmoteSickness.StatesInstance statesInstance = new PeriodicEmoteSickness.StatesInstance(diseaseInstance, this);
			statesInstance.StartSM();
			return statesInstance;
		}

		public override void OnCure(GameObject go, object instance_data)
		{
			PeriodicEmoteSickness.StatesInstance statesInstance = (PeriodicEmoteSickness.StatesInstance)instance_data;
			statesInstance.StopSM("Cured");
		}

		private HashedString kanim;

		private HashedString[] anims;

		private float cooldown;

		public class StatesInstance : GameStateMachine<PeriodicEmoteSickness.States, PeriodicEmoteSickness.StatesInstance, SicknessInstance, object>.GameInstance
		{
			public StatesInstance(SicknessInstance master, PeriodicEmoteSickness periodicEmoteDisease)
				: base(master)
			{
				this.periodicEmoteDisease = periodicEmoteDisease;
			}

			public PeriodicEmoteSickness periodicEmoteDisease;
		}

		public class States : GameStateMachine<PeriodicEmoteSickness.States, PeriodicEmoteSickness.StatesInstance, SicknessInstance>
		{
			public override void InitializeStates(out StateMachine.BaseState default_state)
			{
				default_state = this.emoting;
				this.emoting.ToggleChore((PeriodicEmoteSickness.StatesInstance smi) => new EmoteChore(smi.master, Db.Get().ChoreTypes.Emote, smi.periodicEmoteDisease.kanim, smi.periodicEmoteDisease.anims, KAnim.PlayMode.Once, false), this.cooldown);
				this.cooldown.ScheduleGoTo((PeriodicEmoteSickness.StatesInstance smi) => smi.periodicEmoteDisease.cooldown, this.emoting);
			}

			public GameStateMachine<PeriodicEmoteSickness.States, PeriodicEmoteSickness.StatesInstance, SicknessInstance, object>.State emoting;

			public GameStateMachine<PeriodicEmoteSickness.States, PeriodicEmoteSickness.StatesInstance, SicknessInstance, object>.State cooldown;
		}
	}
}
