using System;
using System.Collections.Generic;
using Klei.AI.DiseaseGrowthRules;
using UnityEngine;

namespace Klei.AI
{
	public class PutridOdour : Disease
	{
		public PutridOdour()
			: base("PutridOdour", Disease.DiseaseType.Ailment, Disease.Severity.Minor, 0.005f, new List<Disease.InfectionVector> { Disease.InfectionVector.Inhalation }, 900f, 1, new Disease.RangeInfo(283.15f, 293.15f, 363.15f, 373.15f), new Disease.RangeInfo(10f, 1200f, 1200f, 10f), new Disease.RangeInfo(0f, 0f, 1000f, 1000f), Disease.RangeInfo.Idempotent())
		{
			base.AddDiseaseComponent(new CommonSickEffectDisease());
			base.AddDiseaseComponent(new PutridOdour.PutridOdourComponent());
		}

		protected override void PopulateElemGrowthInfo()
		{
			base.InitializeElemGrowthArray(ref this.elemGrowthInfo, Disease.DEFAULT_GROWTH_INFO);
			base.AddGrowthRule(new GrowthRule
			{
				underPopulationDeathRate = new float?(2.6666667f),
				minCountPerKG = new float?(100f),
				populationHalfLife = new float?(12000f),
				maxCountPerKG = new float?(1000f),
				overPopulationHalfLife = new float?(3000f),
				minDiffusionCount = new int?(1000),
				diffusionScale = new float?(0.001f),
				minDiffusionInfestationTickCount = new byte?(1)
			});
			base.AddGrowthRule(new StateGrowthRule(Element.State.Solid)
			{
				populationHalfLife = new float?(12000f),
				overPopulationHalfLife = new float?(6000f),
				minDiffusionCount = new int?(1000000)
			});
			base.AddGrowthRule(new ElementGrowthRule(SimHashes.SlimeMold)
			{
				populationHalfLife = new float?(float.PositiveInfinity),
				overPopulationHalfLife = new float?(12000f),
				maxCountPerKG = new float?(10000f),
				diffusionScale = new float?(0.05f)
			});
			base.AddGrowthRule(new StateGrowthRule(Element.State.Gas)
			{
				populationHalfLife = new float?(12000f),
				overPopulationHalfLife = new float?(6000f),
				diffusionScale = new float?(0.2f)
			});
			base.AddGrowthRule(new ElementGrowthRule(SimHashes.ContaminatedOxygen)
			{
				populationHalfLife = new float?(float.PositiveInfinity),
				overPopulationHalfLife = new float?(12000f),
				maxCountPerKG = new float?(10000000f)
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
			base.AddExposureRule(new ElementExposureRule(SimHashes.ChlorineGas)
			{
				populationHalfLife = new float?(10f)
			});
		}

		private const float EmitInterval = 5f;

		private const float EmissionRadius = 1.5f;

		private const float MaxDistanceSq = 2.25f;

		public const string ID = "PutridOdour";

		public class PutridOdourComponent : Disease.DiseaseComponent
		{
			public override object OnInfect(GameObject go, DiseaseInstance diseaseInstance)
			{
				PutridOdour.PutridOdourComponent.InstanceData instanceData = default(PutridOdour.PutridOdourComponent.InstanceData);
				instanceData.schedulerHandle = GameScheduler.Instance.SchedulePeriodic("PutridOdourEmit", 5f, new Action<object>(this.Emit), go, null, 0f, null);
				KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("odor_fx_kanim", go.transform.position, go.transform, true, Grid.SceneLayer.Front, false);
				kbatchedAnimController.Play(PutridOdour.PutridOdourComponent.WorkLoopAnims, KAnim.PlayMode.Loop);
				instanceData.controller = kbatchedAnimController;
				this.Emit(go);
				return instanceData;
			}

			public override void OnCure(GameObject go, object instance_data)
			{
				PutridOdour.PutridOdourComponent.InstanceData instanceData = (PutridOdour.PutridOdourComponent.InstanceData)instance_data;
				KAnimControllerBase controller = instanceData.controller;
				controller.Play("working_pst", KAnim.PlayMode.Once, 1f, 0f);
				controller.destroyOnAnimComplete = true;
				instanceData.schedulerHandle.ClearScheduler();
			}

			private void Emit(object data)
			{
				GameObject gameObject = (GameObject)data;
				if (gameObject == null)
				{
					return;
				}
				Components.Cmps<MinionIdentity> liveMinionIdentities = Components.LiveMinionIdentities;
				Vector2 vector = gameObject.transform.position;
				for (int i = 0; i < liveMinionIdentities.Count; i++)
				{
					MinionIdentity minionIdentity = liveMinionIdentities[i];
					if (minionIdentity.gameObject != gameObject.gameObject)
					{
						Vector2 vector2 = minionIdentity.transform.position;
						float num = Vector2.SqrMagnitude(vector - vector2);
						if (num <= 2.25f)
						{
							minionIdentity.Trigger(508119890, Strings.Get("STRINGS.DUPLICANTS.DISEASES.PUTRIDODOUR.CRINGE_EFFECT").String);
							minionIdentity.GetComponent<Effects>().Add("SmelledPutridOdour", true);
							minionIdentity.gameObject.GetSMI<ThoughtGraph.Instance>().AddThought(Db.Get().Thoughts.PutridOdour);
						}
					}
				}
			}

			private static readonly HashedString[] WorkLoopAnims = new HashedString[] { "working_pre", "working_loop" };

			private struct InstanceData
			{
				public SchedulerHandle schedulerHandle;

				public KAnimControllerBase controller;
			}
		}
	}
}
