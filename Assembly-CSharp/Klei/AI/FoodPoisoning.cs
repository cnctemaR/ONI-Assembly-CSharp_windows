using System;
using System.Collections.Generic;
using Klei.AI.DiseaseGrowthRules;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class FoodPoisoning : Disease
	{
		public FoodPoisoning()
			: base("FoodPoisoning", Disease.DiseaseType.Pathogen, Disease.Severity.Major, 0.005f, new List<Disease.InfectionVector> { Disease.InfectionVector.Digestion }, 900f, 1, new Disease.RangeInfo(248.15f, 278.15f, 313.15f, 348.15f), new Disease.RangeInfo(10f, 1200f, 1200f, 10f), new Disease.RangeInfo(0f, 0f, 1000f, 1000f), Disease.RangeInfo.Idempotent())
		{
			base.AddDiseaseComponent(new CommonSickEffectDisease());
			base.AddDiseaseComponent(new AttributeModifierDisease(new AttributeModifier[]
			{
				new AttributeModifier("BladderDelta", 1.25f, DUPLICANTS.DISEASES.FOODPOISONING.NAME, false, false),
				new AttributeModifier("ToiletEfficiency", -0.4f, DUPLICANTS.DISEASES.FOODPOISONING.NAME, false, false),
				new AttributeModifier("StaminaDelta", -2.5f, DUPLICANTS.DISEASES.FOODPOISONING.NAME, false, false)
			}));
			base.AddDiseaseComponent(new FoodPoisoning.FoodPoisoningComponent());
		}

		protected override void PopulateElemGrowthInfo()
		{
			base.InitializeElemGrowthArray(ref this.elemGrowthInfo, Disease.DEFAULT_GROWTH_INFO);
			base.AddGrowthRule(new GrowthRule
			{
				underPopulationDeathRate = new float?(2.6666667f),
				minCountPerKG = new float?(0.4f),
				populationHalfLife = new float?(12000f),
				maxCountPerKG = new float?(1000f),
				overPopulationHalfLife = new float?(3000f),
				minDiffusionCount = new int?(1000),
				diffusionScale = new float?(0.001f),
				minDiffusionInfestationTickCount = 1
			});
			base.AddGrowthRule(new StateGrowthRule(Element.State.Solid)
			{
				minCountPerKG = new float?(0.4f),
				populationHalfLife = new float?(300f),
				overPopulationHalfLife = new float?(10f),
				minDiffusionCount = new int?(1000000)
			});
			base.AddGrowthRule(new ElementGrowthRule(SimHashes.ToxicSand)
			{
				populationHalfLife = new float?(float.PositiveInfinity),
				overPopulationHalfLife = new float?(12000f)
			});
			base.AddGrowthRule(new ElementGrowthRule(SimHashes.Creature)
			{
				populationHalfLife = new float?(float.PositiveInfinity),
				maxCountPerKG = new float?(4000f),
				overPopulationHalfLife = new float?(3000f)
			});
			base.AddGrowthRule(new ElementGrowthRule(SimHashes.BleachStone)
			{
				populationHalfLife = new float?(10f),
				overPopulationHalfLife = new float?(10f),
				diffusionScale = new float?(0.001f)
			});
			base.AddGrowthRule(new StateGrowthRule(Element.State.Gas)
			{
				minCountPerKG = new float?(250f),
				populationHalfLife = new float?(1200f),
				overPopulationHalfLife = new float?(300f),
				diffusionScale = new float?(0.01f)
			});
			base.AddGrowthRule(new ElementGrowthRule(SimHashes.ContaminatedOxygen)
			{
				populationHalfLife = new float?(12000f),
				maxCountPerKG = new float?(10000f),
				overPopulationHalfLife = new float?(3000f),
				diffusionScale = new float?(0.05f)
			});
			base.AddGrowthRule(new ElementGrowthRule(SimHashes.ChlorineGas)
			{
				populationHalfLife = new float?(10f),
				overPopulationHalfLife = new float?(10f),
				minDiffusionCount = new int?(1000000)
			});
			base.AddGrowthRule(new StateGrowthRule(Element.State.Liquid)
			{
				minCountPerKG = new float?(0.4f),
				populationHalfLife = new float?(12000f),
				maxCountPerKG = new float?(5000f),
				diffusionScale = new float?(0.2f)
			});
			base.AddGrowthRule(new ElementGrowthRule(SimHashes.DirtyWater)
			{
				populationHalfLife = new float?(-12000f),
				overPopulationHalfLife = new float?(12000f)
			});
			base.AddGrowthRule(new TagGrowthRule(GameTags.Edible)
			{
				populationHalfLife = new float?(-12000f),
				overPopulationHalfLife = new float?(float.PositiveInfinity)
			});
			base.AddGrowthRule(new TagGrowthRule(GameTags.Pickled)
			{
				populationHalfLife = new float?(10f),
				overPopulationHalfLife = new float?(10f)
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
			base.AddExposureRule(new ElementExposureRule(SimHashes.ChlorineGas)
			{
				populationHalfLife = new float?(10f)
			});
		}

		public override List<Descriptor> GetDiseaseSourceDescriptors()
		{
			return new List<Descriptor>
			{
				new Descriptor(string.Format(DUPLICANTS.DISEASES.FOODPOISONING.DISEASE_SOURCE_DESCRIPTOR, GameUtil.GetFormattedTime(200f), GameUtil.GetFormattedDiseaseAmount(100000), base.Name), string.Format(DUPLICANTS.DISEASES.FOODPOISONING.DISEASE_SOURCE_DESCRIPTOR_TOOLTIP, GameUtil.GetFormattedTime(200f), GameUtil.GetFormattedDiseaseAmount(100000), base.Name), Descriptor.DescriptorType.DiseaseSource, false)
			};
		}

		public const string ID = "FoodPoisoning";

		private const float VOMIT_FREQUENCY = 200f;

		private class FoodPoisoningComponent : Disease.DiseaseComponent
		{
			public override object OnInfect(GameObject go, DiseaseInstance diseaseInstance)
			{
				FoodPoisoning.FoodPoisoningComponent.InstanceData instanceData = new FoodPoisoning.FoodPoisoningComponent.InstanceData(go, diseaseInstance);
				instanceData.StartChore();
				return instanceData;
			}

			public override void OnCure(GameObject go, object instance_data)
			{
				FoodPoisoning.FoodPoisoningComponent.InstanceData instanceData = (FoodPoisoning.FoodPoisoningComponent.InstanceData)instance_data;
				instanceData.StopChore();
			}

			public override List<Descriptor> GetSymptoms()
			{
				return new List<Descriptor>
				{
					new Descriptor(DUPLICANTS.DISEASES.FOODPOISONING.VOMIT_SYMPTOM, DUPLICANTS.DISEASES.FOODPOISONING.VOMIT_SYMPTOM_TOOLTIP, Descriptor.DescriptorType.SymptomAidable, false)
				};
			}

			private class InstanceData
			{
				public InstanceData(GameObject go, DiseaseInstance diseaseInstance)
				{
					this.go = go;
					this.diseaseInstance = diseaseInstance;
				}

				public void StartChore()
				{
					ChoreProvider chore_provider = this.go.GetComponent<ChoreProvider>();
					this.vomitHandle = GameScheduler.Instance.Schedule("Vomit", 200f, delegate(object data)
					{
						if (chore_provider == null)
						{
							return;
						}
						if (!this.diseaseInstance.IsDoctored)
						{
							this.chore = new VomitChore(Db.Get().ChoreTypes.Vomit, chore_provider, Db.Get().DuplicantStatusItems.Vomiting, this.vomiting, delegate(Chore unused)
							{
								this.StartChore();
							});
						}
						else
						{
							this.StartChore();
						}
					}, null, null);
				}

				public void StopChore()
				{
					if (this.vomitHandle.IsValid)
					{
						this.vomitHandle.ClearScheduler();
					}
					if (this.chore != null)
					{
						this.chore.Cancel("FoodPoisoning.StopChore");
					}
				}

				private GameObject go;

				private DiseaseInstance diseaseInstance;

				private Chore chore;

				private SchedulerHandle vomitHandle;

				public Notification vomiting = new Notification(DUPLICANTS.STATUSITEMS.VOMITING.NOTIFICATION_NAME, NotificationType.Bad, HashedString.Invalid, (List<Notification> notificationList, object data) => DUPLICANTS.STATUSITEMS.VOMITING.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false), null, true, 0f, null, null, null);
			}
		}
	}
}
