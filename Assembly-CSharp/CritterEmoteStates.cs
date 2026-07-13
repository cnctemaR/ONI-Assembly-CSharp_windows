using System;
using Database;
using UnityEngine;

public class CritterEmoteStates : GameStateMachine<CritterEmoteStates, CritterEmoteStates.Instance, IStateMachineTarget, CritterEmoteStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.Enter(delegate(CritterEmoteStates.Instance smi)
		{
			smi.emotion = smi.GetSMI<CritterEmoteMonitor.Instance>().GetCritterEmotion();
			if (smi.emotion != null)
			{
				smi.GoTo(this.playing);
				return;
			}
			smi.GoTo(this.behaviourcomplete);
		});
		this.playing.ToggleAnims((CritterEmoteStates.Instance smi) => smi.emoteBuildFile).PlayAnims(delegate(CritterEmoteStates.Instance smi)
		{
			if (!smi.emotion.isPositiveEmotion)
			{
				return new HashedString[] { "react_neg" };
			}
			return new HashedString[] { "react_pos" };
		}, KAnim.PlayMode.Once).ScheduleGoTo(10f, this.behaviourcomplete)
			.OnAnimQueueComplete(this.behaviourcomplete)
			.Enter(delegate(CritterEmoteStates.Instance smi)
			{
				CritterEmoteMonitor.Instance smi2 = smi.GetSMI<CritterEmoteMonitor.Instance>();
				smi.emotion = smi2.GetCritterEmotion();
				if (!smi2.cooldowns.ContainsKey(smi.emotion))
				{
					smi2.cooldowns.Add(smi.emotion, Time.timeSinceLevelLoad);
				}
				else
				{
					smi2.cooldowns[smi.emotion] = Time.timeSinceLevelLoad;
				}
				if (smi.emotion.sprite != null)
				{
					NameDisplayScreen.Instance.SetThoughtBubbleDisplay(smi.gameObject, true, "", Assets.GetSprite("bubble_alert"), smi.emotion.sprite);
					smi.hasSetThoughtBubble = true;
				}
			})
			.Exit(delegate(CritterEmoteStates.Instance smi)
			{
				if (smi.hasSetThoughtBubble)
				{
					NameDisplayScreen.Instance.SetThoughtBubbleDisplay(smi.gameObject, false, null, null, null);
					smi.hasSetThoughtBubble = false;
				}
			});
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.Behaviours.CritterEmoteBehaviour, false);
	}

	public GameStateMachine<CritterEmoteStates, CritterEmoteStates.Instance, IStateMachineTarget, CritterEmoteStates.Def>.State playing;

	public GameStateMachine<CritterEmoteStates, CritterEmoteStates.Instance, IStateMachineTarget, CritterEmoteStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
		public Def(KAnimFile emoteBuildFile)
		{
			this.emoteBuildFile = emoteBuildFile;
		}

		public KAnimFile emoteBuildFile;
	}

	public new class Instance : GameStateMachine<CritterEmoteStates, CritterEmoteStates.Instance, IStateMachineTarget, CritterEmoteStates.Def>.GameInstance
	{
		public Instance(Chore<CritterEmoteStates.Instance> chore, CritterEmoteStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Behaviours.CritterEmoteBehaviour);
			this.emoteBuildFile = def.emoteBuildFile;
		}

		public KAnimFile emoteBuildFile;

		public CritterEmotion emotion;

		public bool hasSetThoughtBubble;
	}
}
