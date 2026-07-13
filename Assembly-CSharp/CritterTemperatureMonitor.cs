using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class CritterTemperatureMonitor : GameStateMachine<CritterTemperatureMonitor, CritterTemperatureMonitor.Instance, IStateMachineTarget, CritterTemperatureMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.comfortable;
		this.uncomfortableEffect = new Effect("EffectCritterTemperatureUncomfortable", CREATURES.MODIFIERS.CRITTER_TEMPERATURE_UNCOMFORTABLE.NAME, CREATURES.MODIFIERS.CRITTER_TEMPERATURE_UNCOMFORTABLE.TOOLTIP, 0f, false, false, true, null, -1f, 0f, null, "");
		this.uncomfortableEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -1f, CREATURES.MODIFIERS.CRITTER_TEMPERATURE_UNCOMFORTABLE.NAME, false, false, true));
		this.deadlyEffect = new Effect("EffectCritterTemperatureDeadly", CREATURES.MODIFIERS.CRITTER_TEMPERATURE_DEADLY.NAME, CREATURES.MODIFIERS.CRITTER_TEMPERATURE_DEADLY.TOOLTIP, 0f, false, false, true, null, -1f, 0f, null, "");
		this.deadlyEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -2f, CREATURES.MODIFIERS.CRITTER_TEMPERATURE_DEADLY.NAME, false, false, true));
		this.root.Enter(new StateMachine<CritterTemperatureMonitor, CritterTemperatureMonitor.Instance, IStateMachineTarget, CritterTemperatureMonitor.Def>.State.Callback(CritterTemperatureMonitor.RefreshInternalTemperature)).Update(delegate(CritterTemperatureMonitor.Instance smi, float dt)
		{
			StateMachine.BaseState targetState = smi.GetTargetState();
			if (smi.GetCurrentState() != targetState)
			{
				smi.GoTo(targetState);
			}
		}, UpdateRate.SIM_200ms, false).Update(new Action<CritterTemperatureMonitor.Instance, float>(CritterTemperatureMonitor.UpdateInternalTemperature), UpdateRate.SIM_1000ms, false);
		this.hot.TagTransition(GameTags.Dead, this.dead, false).ToggleCritterEmotion(Db.Get().CritterEmotions.Hot, null);
		this.cold.TagTransition(GameTags.Dead, this.dead, false).ToggleCritterEmotion(Db.Get().CritterEmotions.Cold, null);
		this.hot.uncomfortable.ToggleStatusItem(Db.Get().CreatureStatusItems.TemperatureHotUncomfortable, null).ToggleEffect((CritterTemperatureMonitor.Instance smi) => this.uncomfortableEffect);
		this.hot.deadly.ToggleStatusItem(Db.Get().CreatureStatusItems.TemperatureHotDeadly, null).ToggleEffect((CritterTemperatureMonitor.Instance smi) => this.deadlyEffect).Enter(delegate(CritterTemperatureMonitor.Instance smi)
		{
			smi.ResetDamageCooldown();
		})
			.Update(delegate(CritterTemperatureMonitor.Instance smi, float dt)
			{
				smi.TryDamage(dt);
			}, UpdateRate.SIM_200ms, false);
		this.cold.uncomfortable.ToggleStatusItem(Db.Get().CreatureStatusItems.TemperatureColdUncomfortable, null).ToggleEffect((CritterTemperatureMonitor.Instance smi) => this.uncomfortableEffect);
		this.cold.deadly.ToggleStatusItem(Db.Get().CreatureStatusItems.TemperatureColdDeadly, null).ToggleEffect((CritterTemperatureMonitor.Instance smi) => this.deadlyEffect).Enter(delegate(CritterTemperatureMonitor.Instance smi)
		{
			smi.ResetDamageCooldown();
		})
			.Update(delegate(CritterTemperatureMonitor.Instance smi, float dt)
			{
				smi.TryDamage(dt);
			}, UpdateRate.SIM_200ms, false);
		this.dead.DoNothing();
	}

	public static void UpdateInternalTemperature(CritterTemperatureMonitor.Instance smi, float dt)
	{
		CritterTemperatureMonitor.RefreshInternalTemperature(smi);
		if (smi.OnUpdate_GetTemperatureInternal != null)
		{
			smi.OnUpdate_GetTemperatureInternal(dt, smi.GetTemperatureInternal());
		}
	}

	public static void RefreshInternalTemperature(CritterTemperatureMonitor.Instance smi)
	{
		if (smi.temperature != null)
		{
			smi.temperature.SetValue(smi.GetTemperatureInternal());
		}
	}

	public GameStateMachine<CritterTemperatureMonitor, CritterTemperatureMonitor.Instance, IStateMachineTarget, CritterTemperatureMonitor.Def>.State comfortable;

	public GameStateMachine<CritterTemperatureMonitor, CritterTemperatureMonitor.Instance, IStateMachineTarget, CritterTemperatureMonitor.Def>.State dead;

	public CritterTemperatureMonitor.TemperatureStates hot;

	public CritterTemperatureMonitor.TemperatureStates cold;

	public Effect uncomfortableEffect;

	public Effect deadlyEffect;

	public class Def : StateMachine.BaseDef
	{
		public float GetIdealTemperature()
		{
			return (this.temperatureHotUncomfortable + this.temperatureColdUncomfortable) / 2f;
		}

		public float temperatureHotDeadly = float.MaxValue;

		public float temperatureHotUncomfortable = float.MaxValue;

		public float temperatureColdDeadly = float.MinValue;

		public float temperatureColdUncomfortable = float.MinValue;

		public float secondsUntilDamageStarts = 1f;

		public float damagePerSecond = 0.25f;
	}

	public class TemperatureStates : GameStateMachine<CritterTemperatureMonitor, CritterTemperatureMonitor.Instance, IStateMachineTarget, CritterTemperatureMonitor.Def>.State
	{
		public GameStateMachine<CritterTemperatureMonitor, CritterTemperatureMonitor.Instance, IStateMachineTarget, CritterTemperatureMonitor.Def>.State uncomfortable;

		public GameStateMachine<CritterTemperatureMonitor, CritterTemperatureMonitor.Instance, IStateMachineTarget, CritterTemperatureMonitor.Def>.State deadly;
	}

	public new class Instance : GameStateMachine<CritterTemperatureMonitor, CritterTemperatureMonitor.Instance, IStateMachineTarget, CritterTemperatureMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, CritterTemperatureMonitor.Def def)
			: base(master, def)
		{
			this.health = master.GetComponent<Health>();
			this.occupyArea = master.GetComponent<OccupyArea>();
			this.primaryElement = master.GetComponent<PrimaryElement>();
			this.temperature = Db.Get().Amounts.CritterTemperature.Lookup(base.gameObject);
			this.pickupable = master.GetComponent<Pickupable>();
		}

		public void ResetDamageCooldown()
		{
			this.secondsUntilDamage = base.def.secondsUntilDamageStarts;
		}

		public void TryDamage(float deltaSeconds)
		{
			if (this.secondsUntilDamage <= 0f)
			{
				this.health.Damage(base.def.damagePerSecond);
				this.secondsUntilDamage = 1f;
				return;
			}
			this.secondsUntilDamage -= deltaSeconds;
		}

		public StateMachine.BaseState GetTargetState()
		{
			bool flag = this.IsEntirelyInVaccum();
			float temperatureExternal = this.GetTemperatureExternal();
			float temperatureInternal = this.GetTemperatureInternal();
			StateMachine.BaseState baseState;
			if (this.pickupable.KPrefabID.HasTag(GameTags.Dead))
			{
				baseState = base.sm.dead;
			}
			else if (!flag && temperatureExternal > base.def.temperatureHotDeadly)
			{
				baseState = base.sm.hot.deadly;
			}
			else if (!flag && temperatureExternal < base.def.temperatureColdDeadly)
			{
				baseState = base.sm.cold.deadly;
			}
			else if (temperatureInternal > base.def.temperatureHotUncomfortable)
			{
				baseState = base.sm.hot.uncomfortable;
			}
			else if (temperatureInternal < base.def.temperatureColdUncomfortable)
			{
				baseState = base.sm.cold.uncomfortable;
			}
			else
			{
				baseState = base.sm.comfortable;
			}
			return baseState;
		}

		public bool IsEntirelyInVaccum()
		{
			int cachedCell = this.pickupable.cachedCell;
			bool flag;
			if (this.occupyArea != null)
			{
				flag = true;
				for (int i = 0; i < this.occupyArea.OccupiedCellsOffsets.Length; i++)
				{
					int num = Grid.OffsetCell(cachedCell, this.occupyArea.OccupiedCellsOffsets[i]);
					if (!Grid.IsValidCell(num) || !Grid.Element[num].IsVacuum)
					{
						flag = false;
						break;
					}
				}
			}
			else
			{
				flag = !Grid.IsValidCell(cachedCell) || Grid.Element[cachedCell].IsVacuum;
			}
			return flag;
		}

		public float GetTemperatureInternal()
		{
			return this.primaryElement.Temperature;
		}

		public float GetTemperatureExternal()
		{
			int cachedCell = this.pickupable.cachedCell;
			if (this.occupyArea != null)
			{
				float num = 0f;
				int num2 = 0;
				for (int i = 0; i < this.occupyArea.OccupiedCellsOffsets.Length; i++)
				{
					int num3 = Grid.OffsetCell(cachedCell, this.occupyArea.OccupiedCellsOffsets[i]);
					if (Grid.IsValidCell(num3))
					{
						bool flag = Grid.Element[num3].id == SimHashes.Vacuum || Grid.Element[num3].id == SimHashes.Void;
						num2++;
						num += (flag ? this.GetTemperatureInternal() : Grid.Temperature[num3]);
					}
				}
				return num / (float)Mathf.Max(1, num2);
			}
			if (Grid.Element[cachedCell].id != SimHashes.Vacuum && Grid.Element[cachedCell].id != SimHashes.Void)
			{
				return Grid.Temperature[cachedCell];
			}
			return this.GetTemperatureInternal();
		}

		public AmountInstance temperature;

		public Health health;

		public OccupyArea occupyArea;

		public PrimaryElement primaryElement;

		public Pickupable pickupable;

		public float secondsUntilDamage;

		public Action<float, float> OnUpdate_GetTemperatureInternal;
	}
}
