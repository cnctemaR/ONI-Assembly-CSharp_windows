using System;
using System.Collections.Generic;
using Database;
using Klei.AI;
using UnityEngine;

public class CritterEmoteMonitor : GameStateMachine<CritterEmoteMonitor, CritterEmoteMonitor.Instance, IStateMachineTarget, CritterEmoteMonitor.Def>
{
	public static bool ShouldEmote(CritterEmoteMonitor.Instance smi)
	{
		return true;
	}

	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.cooldown;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		List<CritterEmotion> cooldownsToRemove = new List<CritterEmotion>();
		this.cooldown.ScheduleGoTo((CritterEmoteMonitor.Instance smi) => global::UnityEngine.Random.Range(37.5f, 75f), this.express).Enter(delegate(CritterEmoteMonitor.Instance smi)
		{
			NameDisplayScreen.Instance.SetThoughtBubbleDisplay(smi.gameObject, false, null, null, null);
		}).Update(delegate(CritterEmoteMonitor.Instance smi, float dt)
		{
			foreach (KeyValuePair<CritterEmotion, float> keyValuePair in smi.cooldowns)
			{
				if (Time.timeSinceLevelLoad > smi.cooldowns[keyValuePair.Key] + 30f)
				{
					cooldownsToRemove.Add(keyValuePair.Key);
				}
			}
			foreach (CritterEmotion critterEmotion in cooldownsToRemove)
			{
				smi.cooldowns.Remove(critterEmotion);
			}
			cooldownsToRemove.Clear();
		}, UpdateRate.SIM_200ms, false);
		this.express.ToggleBehaviour(GameTags.Creatures.Behaviours.CritterEmoteBehaviour, new StateMachine<CritterEmoteMonitor, CritterEmoteMonitor.Instance, IStateMachineTarget, CritterEmoteMonitor.Def>.Transition.ConditionCallback(CritterEmoteMonitor.ShouldEmote), delegate(CritterEmoteMonitor.Instance smi)
		{
			smi.GoTo(this.cooldown);
		});
	}

	public GameStateMachine<CritterEmoteMonitor, CritterEmoteMonitor.Instance, IStateMachineTarget, CritterEmoteMonitor.Def>.State cooldown;

	public GameStateMachine<CritterEmoteMonitor, CritterEmoteMonitor.Instance, IStateMachineTarget, CritterEmoteMonitor.Def>.State express;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<CritterEmoteMonitor, CritterEmoteMonitor.Instance, IStateMachineTarget, CritterEmoteMonitor.Def>.GameInstance, IDevQuickAction
	{
		public Instance(IStateMachineTarget master, CritterEmoteMonitor.Def def)
			: base(master, def)
		{
			this.emotePositive = Db.Get().Emotes.Critter.Positive;
			this.emoteNegative = Db.Get().Emotes.Critter.Negative;
		}

		public CritterEmotion GetCritterEmotion()
		{
			if (this.currentNegativeEmotions.Count > 0)
			{
				float num = float.PositiveInfinity;
				CritterEmotion critterEmotion = null;
				foreach (CritterEmotion critterEmotion2 in this.currentNegativeEmotions)
				{
					if (!this.cooldowns.ContainsKey(critterEmotion2))
					{
						return critterEmotion2;
					}
					float num2 = this.cooldowns[critterEmotion2];
					if (num2 < num)
					{
						num = num2;
						critterEmotion = critterEmotion2;
					}
				}
				return critterEmotion;
			}
			if (this.currentPositiveEmotions.Count > 0)
			{
				float num3 = 0f;
				CritterEmotion critterEmotion3 = null;
				foreach (CritterEmotion critterEmotion4 in this.currentPositiveEmotions)
				{
					if (!this.cooldowns.ContainsKey(critterEmotion4))
					{
						return critterEmotion4;
					}
					float num4 = this.cooldowns[critterEmotion4];
					if (num4 < num3)
					{
						num3 = num4;
						critterEmotion3 = critterEmotion4;
					}
				}
				return critterEmotion3;
			}
			return null;
		}

		public void AddCritterEmotion(CritterEmotion emotion)
		{
			if (base.smi.GetSMI<BabyMonitor.Instance>() != null)
			{
				return;
			}
			if (!emotion.isPositiveEmotion)
			{
				if (this.currentNegativeEmotions.Contains(emotion))
				{
					return;
				}
				this.currentNegativeEmotions.Add(emotion);
			}
			else
			{
				if (this.currentPositiveEmotions.Contains(emotion))
				{
					return;
				}
				this.currentPositiveEmotions.Add(emotion);
			}
			if (base.smi.IsInsideState(base.sm.cooldown) && !this.cooldowns.ContainsKey(emotion))
			{
				base.smi.GoTo(base.sm.express);
			}
		}

		public void RemoveCritterEmotion(CritterEmotion emotion)
		{
			this.currentNegativeEmotions.RemoveAll((CritterEmotion e) => e.id == emotion.id);
			this.currentPositiveEmotions.RemoveAll((CritterEmotion e) => e.id == emotion.id);
		}

		public List<DevQuickActionInstruction> GetDevInstructions()
		{
			return new List<DevQuickActionInstruction>
			{
				new DevQuickActionInstruction("Emote/Play", delegate
				{
					base.smi.GoTo(base.smi.sm.express);
				})
			};
		}

		public Emote emotePositive;

		public Emote emoteNegative;

		public List<CritterEmotion> currentNegativeEmotions = new List<CritterEmotion>();

		public List<CritterEmotion> currentPositiveEmotions = new List<CritterEmotion>();

		public const float SPECIFIC_EMOTE_COOLDOWN = 30f;

		public Dictionary<CritterEmotion, float> cooldowns = new Dictionary<CritterEmotion, float>();
	}
}
