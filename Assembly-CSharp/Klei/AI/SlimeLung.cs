using System;
using System.Collections.Generic;
using Klei.AI.DiseaseGrowthRules;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class SlimeLung : Disease
	{
		public SlimeLung()
			: base("SlimeLung", Disease.DiseaseType.Pathogen, Disease.Severity.Critical, 0.00025f, new List<Disease.InfectionVector> { Disease.InfectionVector.Inhalation }, 2400f, 1, new Disease.RangeInfo(283.15f, 293.15f, 363.15f, 373.15f), new Disease.RangeInfo(10f, 1200f, 1200f, 10f), new Disease.RangeInfo(0f, 0f, 1000f, 1000f), Disease.RangeInfo.Idempotent())
		{
			this.doctorRequired = true;
			this.fatalityDuration = 6000f;
			base.AddDiseaseComponent(new CommonSickEffectDisease());
			base.AddDiseaseComponent(new AttributeModifierDisease(new AttributeModifier[]
			{
				new AttributeModifier("BreathDelta", -1.1363636f, DUPLICANTS.DISEASES.SLIMELUNG.NAME, false, false, true),
				new AttributeModifier("Athletics", -3f, DUPLICANTS.DISEASES.SLIMELUNG.NAME, false, false, true)
			}));
			base.AddDiseaseComponent(new SlimeLung.SlimeLungComponent());
		}

		protected override void PopulateElemGrowthInfo()
		{
			base.InitializeElemGrowthArray(ref this.elemGrowthInfo, Disease.DEFAULT_GROWTH_INFO);
			base.AddGrowthRule(new GrowthRule
			{
				underPopulationDeathRate = new float?(2.6666667f),
				minCountPerKG = new float?(0.4f),
				populationHalfLife = new float?(12000f),
				maxCountPerKG = new float?(500f),
				overPopulationHalfLife = new float?(1200f),
				minDiffusionCount = new int?(1000),
				diffusionScale = new float?(0.001f),
				minDiffusionInfestationTickCount = new byte?(1)
			});
			base.AddGrowthRule(new StateGrowthRule(Element.State.Solid)
			{
				minCountPerKG = new float?(0.4f),
				populationHalfLife = new float?(3000f),
				overPopulationHalfLife = new float?(1200f),
				diffusionScale = new float?(1E-06f),
				minDiffusionCount = new int?(1000000)
			});
			base.AddGrowthRule(new ElementGrowthRule(SimHashes.SlimeMold)
			{
				underPopulationDeathRate = new float?(0f),
				populationHalfLife = new float?(-3000f),
				overPopulationHalfLife = new float?(3000f),
				maxCountPerKG = new float?(4500f),
				diffusionScale = new float?(0.05f)
			});
			base.AddGrowthRule(new ElementGrowthRule(SimHashes.BleachStone)
			{
				populationHalfLife = new float?(10f),
				overPopulationHalfLife = new float?(10f),
				minDiffusionCount = new int?(100000),
				diffusionScale = new float?(0.001f)
			});
			base.AddGrowthRule(new StateGrowthRule(Element.State.Gas)
			{
				minCountPerKG = new float?(250f),
				populationHalfLife = new float?(12000f),
				overPopulationHalfLife = new float?(1200f),
				maxCountPerKG = new float?(10000f),
				minDiffusionCount = new int?(5100),
				diffusionScale = new float?(0.005f)
			});
			base.AddGrowthRule(new ElementGrowthRule(SimHashes.ContaminatedOxygen)
			{
				underPopulationDeathRate = new float?(0f),
				populationHalfLife = new float?(-300f),
				overPopulationHalfLife = new float?(1200f)
			});
			base.AddGrowthRule(new ElementGrowthRule(SimHashes.Oxygen)
			{
				populationHalfLife = new float?(1200f),
				overPopulationHalfLife = new float?(10f)
			});
			base.AddGrowthRule(new ElementGrowthRule(SimHashes.ChlorineGas)
			{
				populationHalfLife = new float?(10f),
				overPopulationHalfLife = new float?(10f),
				minDiffusionCount = new int?(100000),
				diffusionScale = new float?(0.001f)
			});
			base.AddGrowthRule(new StateGrowthRule(Element.State.Liquid)
			{
				minCountPerKG = new float?(0.4f),
				populationHalfLife = new float?(1200f),
				overPopulationHalfLife = new float?(300f),
				maxCountPerKG = new float?(100f),
				diffusionScale = new float?(0.01f)
			});
			base.InitializeElemExposureArray(ref this.elemExposureInfo, Disease.DEFAULT_EXPOSURE_INFO);
			base.AddExposureRule(new ExposureRule
			{
				populationHalfLife = new float?(float.PositiveInfinity)
			});
			base.AddExposureRule(new ElementExposureRule(SimHashes.DirtyWater)
			{
				populationHalfLife = new float?(-12000f)
			});
			base.AddExposureRule(new ElementExposureRule(SimHashes.ContaminatedOxygen)
			{
				populationHalfLife = new float?(-12000f)
			});
			base.AddExposureRule(new ElementExposureRule(SimHashes.Oxygen)
			{
				populationHalfLife = new float?(3000f)
			});
			base.AddExposureRule(new ElementExposureRule(SimHashes.ChlorineGas)
			{
				populationHalfLife = new float?(10f)
			});
		}

		public override List<Descriptor> GetDiseaseSourceDescriptors()
		{
			return new List<Descriptor>
			{
				new Descriptor(string.Format(DUPLICANTS.DISEASES.SLIMELUNG.DISEASE_SOURCE_DESCRIPTOR, GameUtil.GetFormattedTime(20f), GameUtil.GetFormattedDiseaseAmount(1000), base.Name), string.Format(DUPLICANTS.DISEASES.SLIMELUNG.DISEASE_SOURCE_DESCRIPTOR_TOOLTIP, GameUtil.GetFormattedTime(20f), GameUtil.GetFormattedDiseaseAmount(1000), base.Name), Descriptor.DescriptorType.DiseaseSource, false)
			};
		}

		private const float COUGH_FREQUENCY = 20f;

		private const float COUGH_MASS = 0.1f;

		private const int DISEASE_AMOUNT = 1000;

		private const float DEATH_TIMER = 6000f;

		public const string ID = "SlimeLung";

		public class SlimeLungComponent : Disease.DiseaseComponent
		{
			public override object OnInfect(GameObject go, DiseaseInstance diseaseInstance)
			{
				SlimeLung.SlimeLungComponent.StatesInstance statesInstance = new SlimeLung.SlimeLungComponent.StatesInstance(diseaseInstance);
				statesInstance.StartSM();
				return statesInstance;
			}

			public override void OnCure(GameObject go, object instance_data)
			{
				SlimeLung.SlimeLungComponent.StatesInstance statesInstance = (SlimeLung.SlimeLungComponent.StatesInstance)instance_data;
				statesInstance.StopSM("Cured");
			}

			public override List<Descriptor> GetSymptoms()
			{
				return new List<Descriptor>
				{
					new Descriptor(DUPLICANTS.DISEASES.SLIMELUNG.COUGH_SYMPTOM, DUPLICANTS.DISEASES.SLIMELUNG.COUGH_SYMPTOM_TOOLTIP, Descriptor.DescriptorType.SymptomAidable, false)
				};
			}

			public class StatesInstance : GameStateMachine<SlimeLung.SlimeLungComponent.States, SlimeLung.SlimeLungComponent.StatesInstance, DiseaseInstance, object>.GameInstance
			{
				public StatesInstance(DiseaseInstance master)
					: base(master)
				{
				}

				public Reactable GetReactable()
				{
					return new SelfEmoteReactable(base.master.gameObject, Db.Get().ChoreTypes.Cough, "anim_sneeze_kanim").AddStep(new EmoteReactable.EmoteStep
					{
						anim = "sneeze",
						finishcb = new Action<GameObject>(this.ProduceSlime)
					}).AddStep(new EmoteReactable.EmoteStep
					{
						anim = "sneeze_pst"
					}).AddStep(new EmoteReactable.EmoteStep
					{
						startcb = new Action<GameObject>(this.FinishedCoughing)
					});
				}

				private void ProduceSlime(GameObject cougher)
				{
					AmountInstance amountInstance = Db.Get().Amounts.Temperature.Lookup(cougher);
					int num = Grid.PosToCell(cougher);
					SimMessages.AddRemoveSubstance(num, SimHashes.ContaminatedOxygen, CellEventLogger.Instance.Cough, 0.1f, amountInstance.value, Db.Get().Diseases.GetIndex("SlimeLung"), 1000, -1);
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, string.Format(DUPLICANTS.DISEASES.ADDED_POPFX, base.master.modifier.Name, 1000), cougher.transform, 1.5f, false);
				}

				private void FinishedCoughing(GameObject cougher)
				{
					base.sm.coughFinished.Trigger(this);
				}

				public float lastCoughtTime;
			}

			public class States : GameStateMachine<SlimeLung.SlimeLungComponent.States, SlimeLung.SlimeLungComponent.StatesInstance, DiseaseInstance>
			{
				public override void InitializeStates(out StateMachine.BaseState default_state)
				{
					default_state = this.breathing;
					this.breathing.DefaultState(this.breathing.normal).TagTransition(GameTags.NoOxygen, this.notbreathing, false).Enter("SetCoughTime", delegate(SlimeLung.SlimeLungComponent.StatesInstance smi)
					{
						smi.lastCoughtTime = Time.time;
					})
						.Update("Cough", delegate(SlimeLung.SlimeLungComponent.StatesInstance smi, float dt)
						{
							if (!smi.master.IsDoctored && Time.time - smi.lastCoughtTime > 20f)
							{
								smi.GoTo(this.breathing.cough);
							}
						}, UpdateRate.SIM_4000ms, false);
					this.breathing.cough.ToggleReactable((SlimeLung.SlimeLungComponent.StatesInstance smi) => smi.GetReactable()).OnSignal(this.coughFinished, this.breathing.normal);
					this.notbreathing.TagTransition(new Tag[] { GameTags.NoOxygen }, this.breathing, true);
				}

				public StateMachine<SlimeLung.SlimeLungComponent.States, SlimeLung.SlimeLungComponent.StatesInstance, DiseaseInstance, object>.Signal coughFinished;

				public SlimeLung.SlimeLungComponent.States.BreathingStates breathing;

				public GameStateMachine<SlimeLung.SlimeLungComponent.States, SlimeLung.SlimeLungComponent.StatesInstance, DiseaseInstance, object>.State notbreathing;

				public class BreathingStates : GameStateMachine<SlimeLung.SlimeLungComponent.States, SlimeLung.SlimeLungComponent.StatesInstance, DiseaseInstance, object>.State
				{
					public GameStateMachine<SlimeLung.SlimeLungComponent.States, SlimeLung.SlimeLungComponent.StatesInstance, DiseaseInstance, object>.State normal;

					public GameStateMachine<SlimeLung.SlimeLungComponent.States, SlimeLung.SlimeLungComponent.StatesInstance, DiseaseInstance, object>.State cough;
				}
			}
		}
	}
}
