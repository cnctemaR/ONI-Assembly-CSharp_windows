using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class RecreationTimeMonitor : GameStateMachine<RecreationTimeMonitor, RecreationTimeMonitor.Instance, IStateMachineTarget, RecreationTimeMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.idle;
		this.idle.EventHandler(GameHashes.ScheduleBlocksTick, delegate(RecreationTimeMonitor.Instance smi)
		{
			smi.OnScheduleBlocksTick();
		}).Update(delegate(RecreationTimeMonitor.Instance smi, float dt)
		{
			smi.RefreshTimes();
		}, UpdateRate.SIM_200ms, false);
		this.bonusActive.ToggleEffect((RecreationTimeMonitor.Instance smi) => smi.moraleEffect).EventHandler(GameHashes.ScheduleBlocksTick, delegate(RecreationTimeMonitor.Instance smi)
		{
			smi.OnScheduleBlocksTick();
		}).Update(delegate(RecreationTimeMonitor.Instance smi, float dt)
		{
			smi.RefreshTimes();
		}, UpdateRate.SIM_200ms, false);
	}

	public const int MAX_BONUS = 5;

	public const float BONUS_DURATION_STANDARD = 600f;

	public const float BONUS_DURATION_BIONICS = 1800f;

	public GameStateMachine<RecreationTimeMonitor, RecreationTimeMonitor.Instance, IStateMachineTarget, RecreationTimeMonitor.Def>.State idle;

	public GameStateMachine<RecreationTimeMonitor, RecreationTimeMonitor.Instance, IStateMachineTarget, RecreationTimeMonitor.Def>.State bonusActive;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<RecreationTimeMonitor, RecreationTimeMonitor.Instance, IStateMachineTarget, RecreationTimeMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, RecreationTimeMonitor.Def def)
			: base(master, def)
		{
			this.bonus_duration = ((base.gameObject.PrefabID() == BionicMinionConfig.ID) ? 1800f : 600f);
			this.schedulable = master.GetComponent<Schedulable>();
			this.moraleModifier = new AttributeModifier(Db.Get().Attributes.QualityOfLife.Id, 0f, delegate
			{
				if (Mathf.Clamp(this.moraleAddedTimes.Count - 1, 0, 5) == 5)
				{
					return DUPLICANTS.MODIFIERS.BREAK_BONUS.MAX_NAME;
				}
				return DUPLICANTS.MODIFIERS.BREAK_BONUS.NAME;
			}, false, false);
			this.moraleEffect.Add(this.moraleModifier);
			if ((SaveLoader.Instance.GameInfo.saveMajorVersion != 0 || SaveLoader.Instance.GameInfo.saveMinorVersion != 0) && SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 35))
			{
				this.RestoreFromSchedule();
			}
		}

		public override void StartSM()
		{
			base.StartSM();
			this.RefreshTimes();
		}

		public void RefreshTimes()
		{
			for (int i = this.moraleAddedTimes.Count - 1; i >= 0; i--)
			{
				if (GameClock.Instance.GetTime() - this.moraleAddedTimes[i] > this.bonus_duration)
				{
					this.moraleAddedTimes.RemoveAt(i);
				}
			}
			int num = Math.Clamp(this.moraleAddedTimes.Count - 1, 0, 5);
			this.moraleModifier.SetValue((float)num);
			if (num > 0)
			{
				if (base.smi.GetCurrentState() != base.smi.sm.bonusActive)
				{
					base.smi.GoTo(base.smi.sm.bonusActive);
					return;
				}
			}
			else if (base.smi.GetCurrentState() != base.smi.sm.idle)
			{
				base.smi.GoTo(base.smi.sm.idle);
			}
		}

		public void OnScheduleBlocksTick()
		{
			if (ScheduleManager.Instance.GetSchedule(this.schedulable).GetPreviousScheduleBlock().GroupId == Db.Get().ScheduleGroups.Recreation.Id)
			{
				this.moraleAddedTimes.Add(GameClock.Instance.GetTime());
			}
		}

		private void RestoreFromSchedule()
		{
			Effects component = base.GetComponent<Effects>();
			foreach (string text in new string[] { "Break1", "Break2", "Break3", "Break4", "Break5" })
			{
				if (component.HasEffect(text))
				{
					component.Remove(text);
				}
			}
			Schedule schedule = ScheduleManager.Instance.GetSchedule(this.schedulable);
			List<ScheduleBlock> blocks = schedule.GetBlocks();
			int currentBlockIdx = schedule.GetCurrentBlockIdx();
			int num = 24;
			if (GameClock.Instance.GetTime() <= this.bonus_duration)
			{
				num = Math.Min(currentBlockIdx, Mathf.FloorToInt(GameClock.Instance.GetTime() / 25f));
			}
			for (int j = currentBlockIdx - num; j < currentBlockIdx; j++)
			{
				int k = j;
				global::Debug.Assert(blocks.Count > 0);
				while (k < 0)
				{
					k += blocks.Count;
				}
				if (blocks[k].GroupId == Db.Get().ScheduleGroups.Recreation.Id)
				{
					int num2;
					if (k > currentBlockIdx)
					{
						num2 = blocks.Count - k + currentBlockIdx - 1;
					}
					else
					{
						num2 = currentBlockIdx - k - 1;
					}
					float num3 = (float)num2 * 25f;
					float num4 = GameClock.Instance.GetTime() - num3;
					global::Debug.Assert(num4 > 0f);
					this.moraleAddedTimes.Add(num4);
				}
			}
		}

		[Serialize]
		public List<float> moraleAddedTimes = new List<float>();

		public Effect moraleEffect = new Effect("RecTimeEffect", "Rec Time Effect", "Rec Time Effect Description", 0f, false, false, false, null, -1f, 0f, null, "");

		private Schedulable schedulable;

		private AttributeModifier moraleModifier;

		private int shiftValue;

		private float bonus_duration;
	}
}
