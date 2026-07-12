using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class Telephone : StateMachineComponent<Telephone.StatesInstance>, IGameObjectEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		Components.Telephones.Add(this);
		GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, delegate(object obj)
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule, true);
		}, null, null);
	}

	protected override void OnCleanUp()
	{
		Components.Telephones.Remove(this);
		base.OnCleanUp();
	}

	public void AddModifierDescriptions(List<Descriptor> descs, string effect_id)
	{
		Effect effect = Db.Get().effects.Get(effect_id);
		string text;
		string text2;
		if (effect.Id == this.babbleEffect)
		{
			text = BUILDINGS.PREFABS.TELEPHONE.EFFECT_BABBLE;
			text2 = BUILDINGS.PREFABS.TELEPHONE.EFFECT_BABBLE_TOOLTIP;
		}
		else if (effect.Id == this.chatEffect)
		{
			text = BUILDINGS.PREFABS.TELEPHONE.EFFECT_CHAT;
			text2 = BUILDINGS.PREFABS.TELEPHONE.EFFECT_CHAT_TOOLTIP;
		}
		else
		{
			text = BUILDINGS.PREFABS.TELEPHONE.EFFECT_LONG_DISTANCE;
			text2 = BUILDINGS.PREFABS.TELEPHONE.EFFECT_LONG_DISTANCE_TOOLTIP;
		}
		foreach (AttributeModifier attributeModifier in effect.SelfModifiers)
		{
			Descriptor descriptor = new Descriptor(text.Replace("{attrib}", Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + attributeModifier.AttributeId.ToUpper() + ".NAME")).Replace("{amount}", attributeModifier.GetFormattedString()), text2.Replace("{attrib}", Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + attributeModifier.AttributeId.ToUpper() + ".NAME")).Replace("{amount}", attributeModifier.GetFormattedString()), Descriptor.DescriptorType.Effect, false);
			descriptor.IncreaseIndent();
			descs.Add(descriptor);
		}
	}

	List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(UI.BUILDINGEFFECTS.RECREATION, UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION, Descriptor.DescriptorType.Effect);
		list.Add(descriptor);
		this.AddModifierDescriptions(list, this.babbleEffect);
		this.AddModifierDescriptions(list, this.chatEffect);
		this.AddModifierDescriptions(list, this.longDistanceEffect);
		return list;
	}

	public void HangUp()
	{
		this.isInUse = false;
		this.wasAnswered = false;
		this.RemoveTag(GameTags.LongDistanceCall);
	}

	public string babbleEffect;

	public string chatEffect;

	public string longDistanceEffect;

	public string trackingEffect;

	public bool isInUse;

	public bool wasAnswered;

	public class States : GameStateMachine<Telephone.States, Telephone.StatesInstance, Telephone>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.unoperational;
			this.unoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.ready, false);
			this.ready.TagTransition(GameTags.Operational, this.unoperational, true).DefaultState(this.ready.idle).ToggleRecurringChore(new Func<Telephone.StatesInstance, Chore>(this.CreateChore), null);
			this.ready.idle.WorkableStartTransition((Telephone.StatesInstance smi) => smi.master.GetComponent<TelephoneCallerWorkable>(), this.ready.calling).TagTransition(GameTags.TelephoneRinging, this.ready.ringing, false).PlayAnim("off");
			this.ready.calling.PlayAnim("on_pre").QueueAnim("on", true, null).Enter(delegate(Telephone.StatesInstance smi)
			{
				foreach (Telephone telephone in Components.Telephones.Items)
				{
					if (telephone.GetComponent<Operational>().IsOperational)
					{
						telephone.AddTag(GameTags.TelephoneRinging);
					}
				}
			})
				.Transition(this.ready.talking, (Telephone.StatesInstance smi) => smi.CallAnswered(), UpdateRate.SIM_4000ms)
				.ScheduleGoTo(15f, this.ready.talking);
			this.ready.ringing.PlayAnim("on_receiving", KAnim.PlayMode.Loop).Transition(this.ready.answer, (Telephone.StatesInstance smi) => smi.GetComponent<Telephone>().isInUse, UpdateRate.SIM_33ms).ScheduleGoTo(15f, this.ready.speaker)
				.Exit(delegate(Telephone.StatesInstance smi)
				{
					smi.GetComponent<Telephone>().RemoveTag(GameTags.TelephoneRinging);
				});
			this.ready.answer.PlayAnim("on_pre_loop_receiving").OnAnimQueueComplete(this.ready.talking.chatting);
			this.ready.talking.DefaultState(this.ready.talking.babbling).ScheduleGoTo(25f, this.ready.hangup).PlayAnim("on_loop", KAnim.PlayMode.Loop)
				.Enter(delegate(Telephone.StatesInstance smi)
				{
					smi.GetComponent<Telephone>().RemoveTag(GameTags.TelephoneRinging);
				})
				.EventHandler(GameHashes.PartyLineJoined, delegate(Telephone.StatesInstance smi)
				{
					this.UpdatePartyLine(smi);
				})
				.TriggerOnEnter(GameHashes.PartyLineJoined, null);
			this.ready.talking.babbling.Transition(this.ready.talking.chatting, (Telephone.StatesInstance smi) => smi.CallAnswered(), UpdateRate.SIM_4000ms);
			this.ready.talking.chatting.PlayAnim("on_loop", KAnim.PlayMode.Loop).Transition(this.ready.talking.babbling, (Telephone.StatesInstance smi) => !smi.CallAnswered(), UpdateRate.SIM_4000ms);
			this.ready.speaker.PlayAnim("on_loop_nobody", KAnim.PlayMode.Loop).Transition(this.ready, (Telephone.StatesInstance smi) => !smi.CallAnswered(), UpdateRate.SIM_4000ms);
			this.ready.hangup.OnAnimQueueComplete(this.ready);
		}

		private Chore CreateChore(Telephone.StatesInstance smi)
		{
			Workable component = smi.master.GetComponent<TelephoneCallerWorkable>();
			WorkChore<TelephoneCallerWorkable> workChore = new WorkChore<TelephoneCallerWorkable>(Db.Get().ChoreTypes.Relax, component, null, true, null, null, null, false, Db.Get().ScheduleBlockTypes.Recreation, false, true, null, false, true, false, PriorityScreen.PriorityClass.high, 5, false, true);
			workChore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, component);
			return workChore;
		}

		public void UpdatePartyLine(Telephone.StatesInstance smi)
		{
			int myWorldId = smi.GetMyWorldId();
			foreach (Telephone telephone in Components.Telephones.Items)
			{
				if (telephone.isInUse && myWorldId != telephone.GetMyWorldId())
				{
					smi.GetComponent<Telephone>().AddTag(GameTags.LongDistanceCall);
					telephone.AddTag(GameTags.LongDistanceCall);
				}
			}
		}

		private GameStateMachine<Telephone.States, Telephone.StatesInstance, Telephone, object>.State unoperational;

		private Telephone.States.ReadyStates ready;

		public class ReadyStates : GameStateMachine<Telephone.States, Telephone.StatesInstance, Telephone, object>.State
		{
			public GameStateMachine<Telephone.States, Telephone.StatesInstance, Telephone, object>.State idle;

			public GameStateMachine<Telephone.States, Telephone.StatesInstance, Telephone, object>.State calling;

			public GameStateMachine<Telephone.States, Telephone.StatesInstance, Telephone, object>.State ringing;

			public GameStateMachine<Telephone.States, Telephone.StatesInstance, Telephone, object>.State answer;

			public GameStateMachine<Telephone.States, Telephone.StatesInstance, Telephone, object>.State speaker;

			public GameStateMachine<Telephone.States, Telephone.StatesInstance, Telephone, object>.State hangup;

			public Telephone.States.ReadyStates.TalkingStates talking;

			public class TalkingStates : GameStateMachine<Telephone.States, Telephone.StatesInstance, Telephone, object>.State
			{
				public GameStateMachine<Telephone.States, Telephone.StatesInstance, Telephone, object>.State babbling;

				public GameStateMachine<Telephone.States, Telephone.StatesInstance, Telephone, object>.State chatting;
			}
		}
	}

	public class StatesInstance : GameStateMachine<Telephone.States, Telephone.StatesInstance, Telephone, object>.GameInstance
	{
		public StatesInstance(Telephone smi)
			: base(smi)
		{
		}

		public bool CallAnswered()
		{
			foreach (Telephone telephone in Components.Telephones.Items)
			{
				if (telephone.isInUse && telephone != base.smi.GetComponent<Telephone>())
				{
					telephone.wasAnswered = true;
					return true;
				}
			}
			return false;
		}

		public bool CallEnded()
		{
			using (List<Telephone>.Enumerator enumerator = Components.Telephones.Items.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.isInUse)
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
