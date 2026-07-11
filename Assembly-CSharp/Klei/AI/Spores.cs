using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
	public class Spores : Disease
	{
		public Spores()
			: base("Spores", Disease.DiseaseType.Ailment, Disease.Severity.Major, 0.005f, new List<Disease.InfectionVector> { Disease.InfectionVector.Contact }, 900f, 0, new Disease.RangeInfo(0f, 0f, 1000f, 1000f), Disease.RangeInfo.Idempotent(), new Disease.RangeInfo(0f, 0f, 1000f, 1000f), Disease.RangeInfo.Idempotent())
		{
			base.AddDiseaseComponent(new AnimatedDisease(new HashedString[] { "anim_idle_spores_kanim" }, Db.Get().Expressions.SickSpores));
			base.AddDiseaseComponent(new Spores.SporesComponent());
		}

		private const float EmitMass = 0.05f;

		private const SimHashes EmitElement = SimHashes.ContaminatedOxygen;

		public const string ID = "Spores";

		private class SporesComponent : Disease.DiseaseComponent
		{
			public override object OnInfect(GameObject go, DiseaseInstance diseaseInstance)
			{
				Spores.SporesComponent.InstanceData instanceData = new Spores.SporesComponent.InstanceData();
				instanceData.go = go;
				SimAndRenderScheduler.instance.Add(instanceData, false);
				instanceData.Sim4000ms(0f);
				return instanceData;
			}

			public override void OnCure(GameObject go, object instace_data)
			{
				Spores.SporesComponent.InstanceData instanceData = (Spores.SporesComponent.InstanceData)instace_data;
				SimAndRenderScheduler.instance.Remove(instanceData);
			}

			private void Emit(GameObject go)
			{
			}

			private static readonly HashedString[] WorkLoopAnims = new HashedString[] { "working_pre", "working_loop", "working_pst" };

			private class InstanceData : ISim4000ms
			{
				public void Sim4000ms(float dt)
				{
					if (this.go == null)
					{
						return;
					}
					int num = Grid.PosToCell(this.go.transform.GetPosition());
					float value = Db.Get().Amounts.Temperature.Lookup(this.go).value;
					SimMessages.AddRemoveSubstance(num, SimHashes.ContaminatedOxygen, CellEventLogger.Instance.ElementConsumerSimUpdate, 0.05f, value, byte.MaxValue, 0, true, -1);
					KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("spore_fx_kanim", this.go.transform.GetPosition(), this.go.transform, true, Grid.SceneLayer.Front, false);
					kbatchedAnimController.Play(Spores.SporesComponent.WorkLoopAnims, KAnim.PlayMode.Once);
					kbatchedAnimController.destroyOnAnimComplete = true;
				}

				public GameObject go;
			}
		}
	}
}
