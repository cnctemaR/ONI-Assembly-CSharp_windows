using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
	public class SimpleEvent : GameplayEvent<SimpleEvent.StatesInstance>
	{
		public SimpleEvent(string id, string title, string description, string buttonText = null, string buttonTooltip = null)
			: base(id, 0, 0)
		{
			this.popupTitle = title;
			this.popupDescription = description;
			this.popupButtonText = buttonText;
			this.popupButtonTooltip = buttonTooltip;
		}

		public override StateMachine.Instance GetSMI(GameplayEventManager manager, GameplayEventInstance eventInstance)
		{
			return new SimpleEvent.StatesInstance(manager, eventInstance, this);
		}

		private string popupButtonText;

		private string popupButtonTooltip;

		public class States : GameplayEventStateMachine<SimpleEvent.States, SimpleEvent.StatesInstance, GameplayEventManager, SimpleEvent>
		{
			public override void InitializeStates(out StateMachine.BaseState default_state)
			{
				default_state = this.root;
				this.ending.ReturnSuccess();
			}

			public override GameplayEventPopupData GenerateEventPopupData(SimpleEvent.StatesInstance smi)
			{
				GameplayEventPopupData gameplayEventPopupData = new GameplayEventPopupData(smi.gameplayEvent);
				gameplayEventPopupData.minions = smi.minions;
				gameplayEventPopupData.artifact = smi.artifact;
				GameplayEventPopupData.PopupOption popupOption = gameplayEventPopupData.AddOption(smi.gameplayEvent.popupButtonText, null);
				popupOption.callback = delegate
				{
					if (smi.callback != null)
					{
						smi.callback();
					}
					smi.StopSM("SimpleEvent Finished");
				};
				popupOption.tooltip = smi.gameplayEvent.popupButtonTooltip;
				if (smi.textParameters != null)
				{
					foreach (global::Tuple<string, string> tuple in smi.textParameters)
					{
						gameplayEventPopupData.SetTextParameter(tuple.first, tuple.second);
					}
				}
				return gameplayEventPopupData;
			}

			public GameStateMachine<SimpleEvent.States, SimpleEvent.StatesInstance, GameplayEventManager, object>.State ending;
		}

		public class StatesInstance : GameplayEventStateMachine<SimpleEvent.States, SimpleEvent.StatesInstance, GameplayEventManager, SimpleEvent>.GameplayEventStateMachineInstance
		{
			public StatesInstance(GameplayEventManager master, GameplayEventInstance eventInstance, SimpleEvent simpleEvent)
				: base(master, eventInstance, simpleEvent)
			{
			}

			public void SetTextParameter(string key, string value)
			{
				if (this.textParameters == null)
				{
					this.textParameters = new List<global::Tuple<string, string>>();
				}
				this.textParameters.Add(new global::Tuple<string, string>(key, value));
			}

			public void ShowEventPopup(object buttonCallBackData = null)
			{
				GameplayEventInstance.ShowEventPopup(base.smi.sm.GenerateEventPopupData(base.smi));
			}

			public GameObject[] minions;

			public GameObject artifact;

			public List<global::Tuple<string, string>> textParameters;

			public global::System.Action callback;
		}
	}
}
