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
			((PeriodicEmoteSickness.StatesInstance)instance_data).StopSM("Cured");
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
				SelfEmoteReactable selfEmoteReactable = new SelfEmoteReactable(base.master.gameObject, "PeriodicEmoteSickness", Db.Get().ChoreTypes.Emote, "anim_sneeze_kanim", 0f, this.periodicEmoteSickness.cooldown, float.PositiveInfinity);
				foreach (HashedString hashedString in this.periodicEmoteSickness.anims)
				{
					selfEmoteReactable.AddStep(new EmoteReactable.EmoteStep
					{
						anim = hashedString
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
