using System;
using System.Collections.Generic;
using Database;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class ImmuneSystemMonitor : GameStateMachine<ImmuneSystemMonitor, ImmuneSystemMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.healthy;
		base.serializable = false;
		this.root.EventHandler(GameHashes.EatCompleteEater, delegate(ImmuneSystemMonitor.Instance smi, object obj)
		{
			smi.OnEatComplete(obj);
		}).EventHandler(GameHashes.AirConsumed, delegate(ImmuneSystemMonitor.Instance smi, object obj)
		{
			smi.OnAirConsumed(obj);
		}).EventTransition(GameHashes.DiseaseAdded, this.infected, (ImmuneSystemMonitor.Instance smi) => smi.IsSick())
			.Transition(this.recovering, (ImmuneSystemMonitor.Instance smi) => smi.effects.HasEffect("PostDiseaseRecovery"));
		this.healthy.ParamTransition<bool>(this.isLosingImmunity, this.infecting, (ImmuneSystemMonitor.Instance smi, bool p) => p).Update(delegate(ImmuneSystemMonitor.Instance smi)
		{
			smi.UpdateImmuneSystem();
		});
		this.infecting.DefaultState(this.infecting.high).ParamTransition<bool>(this.isLosingImmunity, this.healthy, (ImmuneSystemMonitor.Instance smi, bool p) => !p).Update(delegate(ImmuneSystemMonitor.Instance smi)
		{
			smi.UpdateImmuneSystem();
		});
		this.infecting.high.Transition(this.infecting.low, (ImmuneSystemMonitor.Instance smi) => smi.IsLowImmuneLevel());
		this.infecting.low.Transition(this.infecting.high, (ImmuneSystemMonitor.Instance smi) => !smi.IsLowImmuneLevel()).Enter(delegate(ImmuneSystemMonitor.Instance smi)
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_BeingInfected);
		}).ToggleStatusItem(Db.Get().DuplicantStatusItems.LowImmunity, null);
		this.infected.Update(delegate(ImmuneSystemMonitor.Instance smi)
		{
			smi.ClearInternalDisease();
		}).ToggleAttributeModifier("suppressed by sickness", (ImmuneSystemMonitor.Instance smi) => smi.immuneSuppress, null).EventTransition(GameHashes.DiseaseCured, this.beginrecovering, (ImmuneSystemMonitor.Instance smi) => !smi.IsSick());
		this.beginrecovering.AddEffect("PostDiseaseRecovery").GoTo(this.recovering);
		this.recovering.Update(delegate(ImmuneSystemMonitor.Instance smi)
		{
			smi.ClearInternalDisease();
		}).Transition(this.healthy, (ImmuneSystemMonitor.Instance smi) => !smi.effects.HasEffect("PostDiseaseRecovery"));
	}

	public StateMachine<ImmuneSystemMonitor, ImmuneSystemMonitor.Instance, IStateMachineTarget, object>.BoolParameter isLosingImmunity;

	public GameStateMachine<ImmuneSystemMonitor, ImmuneSystemMonitor.Instance, IStateMachineTarget, object>.State healthy;

	public ImmuneSystemMonitor.BelowToleranceState infecting;

	public GameStateMachine<ImmuneSystemMonitor, ImmuneSystemMonitor.Instance, IStateMachineTarget, object>.State infected;

	public GameStateMachine<ImmuneSystemMonitor, ImmuneSystemMonitor.Instance, IStateMachineTarget, object>.State beginrecovering;

	public GameStateMachine<ImmuneSystemMonitor, ImmuneSystemMonitor.Instance, IStateMachineTarget, object>.State recovering;

	public class BelowToleranceState : GameStateMachine<ImmuneSystemMonitor, ImmuneSystemMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<ImmuneSystemMonitor, ImmuneSystemMonitor.Instance, IStateMachineTarget, object>.State high;

		public GameStateMachine<ImmuneSystemMonitor, ImmuneSystemMonitor.Instance, IStateMachineTarget, object>.State low;
	}

	public new class Instance : GameStateMachine<ImmuneSystemMonitor, ImmuneSystemMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.immuneLevel = Db.Get().Amounts.ImmuneLevel.Lookup(base.gameObject);
			AmountInstance amountInstance = this.immuneLevel;
			amountInstance.OnDelta = (Action<float>)Delegate.Combine(amountInstance.OnDelta, new Action<float>(this.OnImmuneDelta));
			AttributeConverterInstance attributeConverterInstance = master.GetComponent<Klei.AI.AttributeConverters>().Get(Db.Get().AttributeConverters.ImmuneLevelBoost);
			this.immuneLevel.deltaAttribute.Add("immunity stat", new AttributeModifier(this.immuneLevel.deltaAttribute.Id, attributeConverterInstance.Evaluate(), DUPLICANTS.ATTRIBUTES.IMMUNITY.BOOST_STAT, false, false, true));
			this.immuneSuppress = new AttributeModifier(this.immuneLevel.deltaAttribute.Id, -0.025f, DUPLICANTS.DISEASES.INFECTED_MODIFIER, false, false, true);
			this.activeDiseases = master.GetComponent<MinionModifiers>().diseases;
			this.primaryElement = master.GetComponent<PrimaryElement>();
			this.effects = master.GetComponent<Effects>();
			this.lastDiseaseSources = new Dictionary<HashedString, ImmuneSystemMonitor.Instance.DiseaseSourceInfo>();
			this.activeImmuneModifiers = new Dictionary<HashedString, AttributeModifier>();
			this.diseaseCountMultModifiers = new Dictionary<HashedString, AttributeModifier>();
			Klei.AI.Attributes attributes = base.gameObject.GetAttributes();
			foreach (Disease disease in Db.Get().Diseases)
			{
				attributes.Add("linear disease loss", new AttributeModifier(disease.amountDeltaAttribute.Id, -0.8333333f, disease.Name, false, false, false));
				AttributeModifier attributeModifier = new AttributeModifier(disease.amountDeltaAttribute.Id, -0.00066666666f, disease.Name, false, false, false);
				this.diseaseCountMultModifiers[disease.id] = attributeModifier;
				attributes.Add("geometric disease loss", attributeModifier);
			}
			GameClock.Instance.Subscribe(-722330267, new Action<object>(this.OnNightTime));
			this.modifiers = base.gameObject.GetComponent<Modifiers>();
			this.immuneDelta = Db.Get().Amounts.ImmuneLevel.deltaAttribute.Lookup(base.gameObject);
		}

		public override void StopSM(string reason)
		{
			GameClock.Instance.Unsubscribe(-722330267, new Action<object>(this.OnNightTime));
			base.StopSM(reason);
		}

		public void OnEatComplete(object obj)
		{
			Edible edible = (Edible)obj;
			HandleVector<int>.Handle handle = GameComps.DiseaseContainers.GetHandle(edible.gameObject);
			if (handle != HandleVector<int>.InvalidHandle)
			{
				DiseaseContainer data = GameComps.DiseaseContainers.GetData(handle);
				if (data.diseaseIdx != 255)
				{
					Disease disease = Db.Get().Diseases[(int)data.diseaseIdx];
					if (disease.infectionVectors.Contains(Disease.InfectionVector.Digestion))
					{
						float num = edible.unitsConsumed / (edible.unitsConsumed + edible.Units);
						int num2 = Mathf.CeilToInt((float)data.diseaseCount * num);
						GameComps.DiseaseContainers.ModifyDiseaseCount(handle, -num2);
						KPrefabID component = edible.GetComponent<KPrefabID>();
						this.InjectDisease(disease, num2, component.PrefabID(), Disease.InfectionVector.Digestion);
					}
				}
			}
		}

		public void OnAirConsumed(object obj)
		{
			Sim.MassConsumptionCallback massConsumptionCallback = (Sim.MassConsumptionCallback)obj;
			if (massConsumptionCallback.diseaseIdx != 255)
			{
				Disease disease = Db.Get().Diseases[(int)massConsumptionCallback.diseaseIdx];
				if (disease.infectionVectors.Contains(Disease.InfectionVector.Inhalation))
				{
					this.InjectDisease(disease, massConsumptionCallback.diseaseCount, ElementLoader.elements[(int)massConsumptionCallback.removedElemIdx].tag, Disease.InfectionVector.Inhalation);
				}
			}
		}

		public void InjectDisease(Disease disease, int count, Tag source, Disease.InfectionVector vector)
		{
			Modifiers component = base.gameObject.GetComponent<Modifiers>();
			Klei.AI.Amounts amounts = component.GetAmounts();
			AmountInstance amountInstance = amounts.Get(disease.amount);
			amountInstance.ApplyDelta((float)count);
			this.lastDiseaseSources[disease.id] = new ImmuneSystemMonitor.Instance.DiseaseSourceInfo(source, vector);
		}

		public void UpdateImmuneSystem()
		{
			Klei.AI.Amounts amounts = this.modifiers.GetAmounts();
			Disease disease3 = null;
			float num = -1f;
			global::Database.Diseases diseases = Db.Get().Diseases;
			for (int i = 0; i < diseases.Count; i++)
			{
				Disease disease = diseases[i];
				float value = amounts.Get(disease.amount).value;
				if (value > 0f)
				{
					if (value > num)
					{
						disease3 = disease;
						num = value;
					}
					float num2 = -0.8333333f;
					float num3 = value * -0.00066666666f;
					num2 += num3;
					this.diseaseCountMultModifiers[disease.id].SetValue(num3);
					float num4 = num2 * disease.immuneAttackStrength;
					if (num4 <= -0.00016666666f && value > 1f)
					{
						AttributeModifier attributeModifier;
						if (!this.activeImmuneModifiers.TryGetValue(disease.id, out attributeModifier))
						{
							attributeModifier = new AttributeModifier(this.immuneDelta.Id, 0f, delegate
							{
								Disease disease2 = disease;
								float value2 = amounts.Get(disease2.amount).value;
								return string.Format(DUPLICANTS.DISEASES.INFECTION_MODIFIER, disease2.Name, GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(value2)));
							}, false, false);
							base.gameObject.GetAttributes().Add("immune damage from " + disease.id, attributeModifier);
							this.activeImmuneModifiers[disease.id] = attributeModifier;
						}
						attributeModifier.SetValue(num4);
					}
					else if (this.activeImmuneModifiers.ContainsKey(disease.id))
					{
						base.gameObject.GetAttributes().Remove(this.activeImmuneModifiers[disease.id]);
						this.activeImmuneModifiers.Remove(disease.id);
					}
				}
			}
			this.lastHighestDisease = disease3;
			base.sm.isLosingImmunity.Set(this.immuneDelta.GetTotalValue() < 0f, base.smi);
		}

		public void ClearInternalDisease()
		{
			Klei.AI.Amounts amounts = this.modifiers.GetAmounts();
			global::Database.Diseases diseases = Db.Get().Diseases;
			for (int i = 0; i < diseases.Count; i++)
			{
				Disease disease = diseases[i];
				AmountInstance amountInstance = amounts.Get(disease.amount);
				amountInstance.SetValue(0f);
				if (this.activeImmuneModifiers.ContainsKey(disease.id))
				{
					base.gameObject.GetAttributes().Remove(this.activeImmuneModifiers[disease.id]);
					this.activeImmuneModifiers.Remove(disease.id);
				}
			}
		}

		private void OnImmuneDelta(float delta)
		{
			if (Game.Instance.customSettings.GetCurrentQualitySetting("ImmuneSystem").id == "Invincible")
			{
				return;
			}
			if (this.immuneLevel.value <= 0f && this.lastHighestDisease != null)
			{
				ImmuneSystemMonitor.Instance.DiseaseSourceInfo diseaseSourceInfo;
				string text;
				if (this.lastDiseaseSources.TryGetValue(this.lastHighestDisease.Id, out diseaseSourceInfo))
				{
					switch (diseaseSourceInfo.vector)
					{
					case Disease.InfectionVector.Contact:
						text = DUPLICANTS.DISEASES.INFECTIONSOURCES.SKIN;
						break;
					case Disease.InfectionVector.Digestion:
						text = string.Format(DUPLICANTS.DISEASES.INFECTIONSOURCES.FOOD, diseaseSourceInfo.sourceObject.ProperName());
						break;
					case Disease.InfectionVector.Inhalation:
						text = string.Format(DUPLICANTS.DISEASES.INFECTIONSOURCES.AIR, diseaseSourceInfo.sourceObject.ProperName());
						break;
					default:
						text = DUPLICANTS.DISEASES.INFECTIONSOURCES.UNKNOWN;
						break;
					}
				}
				else
				{
					text = DUPLICANTS.DISEASES.INFECTIONSOURCES.UNKNOWN;
				}
				this.activeDiseases.Infect(new DiseaseExposureInfo(this.lastHighestDisease.Id, text));
			}
		}

		public AttributeModifier GetCurrentImmuneModifier(Disease disease)
		{
			AttributeModifier attributeModifier = null;
			this.activeImmuneModifiers.TryGetValue(disease.id, out attributeModifier);
			return attributeModifier;
		}

		public bool IsLowImmuneLevel()
		{
			return this.immuneLevel.value < 40f;
		}

		public bool IsSick()
		{
			return this.activeDiseases.Count > 0;
		}

		private void OnNightTime(object data)
		{
			this.UpdateReports();
		}

		private void UpdateReports()
		{
			ReportManager.Instance.ReportValue(ReportManager.ReportType.DiseaseStatus, (float)this.primaryElement.DiseaseCount, string.Format(UI.ENDOFDAYREPORT.NOTES.GERMS, base.master.name), base.master.gameObject.GetProperName());
		}

		private const float LOW_IMMUNE_LEVEL = 40f;

		[Serialize]
		public Dictionary<HashedString, ImmuneSystemMonitor.Instance.DiseaseSourceInfo> lastDiseaseSources;

		public Dictionary<HashedString, AttributeModifier> activeImmuneModifiers;

		public Dictionary<HashedString, AttributeModifier> diseaseCountMultModifiers;

		private Klei.AI.Diseases activeDiseases;

		private PrimaryElement primaryElement;

		private Modifiers modifiers;

		public Effects effects;

		public AmountInstance immuneLevel;

		public AttributeModifier immuneSuppress;

		public AttributeConverterInstance immuneBoostConverter;

		private AttributeInstance immuneDelta;

		public Disease lastHighestDisease;

		public class DiseaseSourceInfo
		{
			public DiseaseSourceInfo(Tag sourceObject, Disease.InfectionVector vector)
			{
				this.sourceObject = sourceObject;
				this.vector = vector;
			}

			public Tag sourceObject;

			public Disease.InfectionVector vector;
		}
	}
}
