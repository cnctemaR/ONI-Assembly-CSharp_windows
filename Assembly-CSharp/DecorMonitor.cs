using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class DecorMonitor : GameStateMachine<DecorMonitor, DecorMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.ToggleAttributeModifier("DecorSmoother", (DecorMonitor.Instance smi) => smi.GetDecorModifier(), (DecorMonitor.Instance smi) => true).Update("DecorSensing", delegate(DecorMonitor.Instance smi, float dt)
		{
			smi.Update(dt);
		}, UpdateRate.SIM_200ms, false).EventHandler(GameHashes.NewDay, (DecorMonitor.Instance smi) => GameClock.Instance, delegate(DecorMonitor.Instance smi)
		{
			smi.OnNewDay();
		});
	}

	public new class Instance : GameStateMachine<DecorMonitor, DecorMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.cycleTotalDecor = 2250f;
			this.amount = Db.Get().Amounts.Decor.Lookup(base.gameObject);
			this.modifier = new AttributeModifier(Db.Get().Amounts.Decor.deltaAttribute.Id, 1f, DUPLICANTS.NEEDS.DECOR.OBSERVED_DECOR, false, false, false);
		}

		public AttributeModifier GetDecorModifier()
		{
			return this.modifier;
		}

		public void Update(float dt)
		{
			int num = Grid.PosToCell(base.gameObject);
			if (!Grid.IsValidCell(num))
			{
				return;
			}
			float decorAtCell = GameUtil.GetDecorAtCell(num);
			this.cycleTotalDecor += decorAtCell * dt;
			float num2 = 0f;
			float num3 = 4.1666665f;
			if (Mathf.Abs(decorAtCell - this.amount.value) > 0.5f)
			{
				if (decorAtCell > this.amount.value)
				{
					num2 = 3f * num3;
				}
				else if (decorAtCell < this.amount.value)
				{
					num2 = -num3;
				}
			}
			else
			{
				this.amount.value = decorAtCell;
			}
			this.modifier.SetValue(num2);
		}

		public void OnNewDay()
		{
			this.yesterdaysTotalDecor = this.cycleTotalDecor;
			this.cycleTotalDecor = 0f;
			Attributes attributes = base.gameObject.GetAttributes();
			AttributeInstance attributeInstance = attributes.Add(Db.Get().Attributes.DecorExpectation);
			float totalValue = attributeInstance.GetTotalValue();
			float num = this.yesterdaysTotalDecor / 600f;
			num += totalValue;
			Effects component = base.gameObject.GetComponent<Effects>();
			string text = null;
			foreach (KeyValuePair<float, string> keyValuePair in this.effectLookup)
			{
				component.Remove(keyValuePair.Value);
				if (text == null && num < keyValuePair.Key)
				{
					text = keyValuePair.Value;
				}
			}
			component.Add(text, true);
		}

		public float GetTodaysAverageDecor()
		{
			return this.cycleTotalDecor / (GameClock.Instance.GetCurrentCycleAsPercentage() * 600f);
		}

		public float GetYesterdaysAverageDecor()
		{
			return this.yesterdaysTotalDecor / 600f;
		}

		[Serialize]
		private float cycleTotalDecor;

		[Serialize]
		private float yesterdaysTotalDecor;

		private AmountInstance amount;

		private AttributeModifier modifier;

		private List<KeyValuePair<float, string>> effectLookup = new List<KeyValuePair<float, string>>
		{
			new KeyValuePair<float, string>(-30f, "DecorMinus1"),
			new KeyValuePair<float, string>(0f, "Decor0"),
			new KeyValuePair<float, string>(30f, "Decor1"),
			new KeyValuePair<float, string>(60f, "Decor2"),
			new KeyValuePair<float, string>(90f, "Decor3"),
			new KeyValuePair<float, string>(120f, "Decor4"),
			new KeyValuePair<float, string>(float.MaxValue, "Decor5")
		};
	}
}
