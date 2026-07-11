using System;
using UnityEngine;

namespace Klei.AI
{
	public class PeriodicEmoteSickness : Sickness.SicknessComponent
	{
		public PeriodicEmoteSickness(HashedString kanim, HashedString[] anims, float cooldown)
		{
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

		private HashedString[] anims;

		private float cooldown;

		public class StatesInstance : GameStateMachine<PeriodicEmoteSickness.States, PeriodicEmoteSickness.StatesInstance, SicknessInstance, object>.GameInstance
		{
			public StatesInstance(SicknessInstance master, PeriodicEmoteSickness periodicEmoteSickness)
				: base(master)
			{
				this.periodicEmoteSickness = periodicEmoteSickness;
			}

			public Reactable GetReactable()
			{
				GameObject gameObject = base.master.gameObject;
				HashedString hashedString = "PeriodicEmoteSickness";
				ChoreType emote = Db.Get().ChoreTypes.Emote;
				HashedString hashedString2 = "anim_sneeze_kanim";
				float cooldown = this.periodicEmoteSickness.cooldown;
				SelfEmoteReactable selfEmoteReactable = new SelfEmoteReactable(gameObject, hashedString, emote, hashedString2, 0f, cooldown, float.PositiveInfinity);
				foreach (HashedString hashedString3 in this.periodicEmoteSickness.anims)
				{
					selfEmoteReactable.AddStep(new EmoteReactable.EmoteStep
					{
						anim = hashedString3
					});
				}
				return selfEmoteReactable;
			}

			public PeriodicEmoteSickness periodicEmoteSickness;
		}

		public class States : GameStateMachine<PeriodicEmoteSickness.States, PeriodicEmoteSickness.StatesInstance, SicknessInstance>
		{
			public override void InitializeStates(out StateMachine.BaseState default_state)
			{
				default_state = this.root;
				this.root.ToggleReactable((PeriodicEmoteSickness.StatesInstance smi) => smi.GetReactable());
			}
		}
	}
}
