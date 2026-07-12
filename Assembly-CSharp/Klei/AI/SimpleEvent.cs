using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
	public class SimpleEvent : GameplayEvent<SimpleEvent.StatesInstance>
	{
		public SimpleEvent(string id, string title, string description, string animFileName, string buttonText = null, string buttonTooltip = null)
			: base(id, 0, 0)
		{
			this.title = title;
			this.description = description;
			this.buttonText = buttonText;
			this.buttonTooltip = buttonTooltip;
			this.animFileName = animFileName;
		}

		public override StateMachine.Instance GetSMI(GameplayEventManager manager, GameplayEventInstance eventInstance)
		{
			return new SimpleEvent.StatesInstance(manager, eventInstance, this);
		}

		private string buttonText;

		private string buttonTooltip;

		public class States : GameplayEventStateMachine<SimpleEvent.States, SimpleEvent.StatesInstance, GameplayEventManager, SimpleEvent>
		{
			public override void InitializeStates(out StateMachine.BaseState default_state)
			{
				default_state = this.root;
				this.ending.ReturnSuccess();
			}

			public override EventInfoData GenerateEventPopupData(SimpleEvent.StatesInstance smi)
			{
				EventInfoData eventInfoData = new EventInfoData(smi.gameplayEvent.title, smi.gameplayEvent.description, smi.gameplayEvent.animFileName);
				eventInfoData.minions = smi.minions;
				eventInfoData.artifact = smi.artifact;
				EventInfoData.Option option = eventInfoData.AddOption(smi.gameplayEvent.buttonText, null);
				option.callback = delegate
				{
					if (smi.callback != null)
					{
						smi.callback();
					}
					smi.StopSM("SimpleEvent Finished");
				};
				option.tooltip = smi.gameplayEvent.buttonTooltip;
				if (smi.textParameters != null)
				{
					foreach (global::Tuple<string, string> tuple in smi.textParameters)
					{
						eventInfoData.SetTextParameter(tuple.first, tuple.second);
					}
				}
				return eventInfoData;
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

			public void ShowEventPopup()
			{
				EventInfoScreen.ShowPopup(base.smi.sm.GenerateEventPopupData(base.smi));
			}

			public GameObject[] minions;

			public GameObject artifact;

			public List<global::Tuple<string, string>> textParameters;

			public global::System.Action callback;
		}
	}
}
