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
			base.AddDiseaseComponent(new AnimatedDisease(new HashedString[] { "anim_idle_spores_kanim" }, "SickSpores"));
			base.AddDiseaseComponent(new Spores.SporesComponent());
		}

		private const float EmitInterval = 3f;

		private const float EmitMass = 0.05f;

		private const SimHashes EmitElement = SimHashes.ContaminatedOxygen;

		public const string ID = "Spores";

		private class SporesComponent : Disease.DiseaseComponent
		{
			public override object OnInfect(GameObject go, DiseaseInstance diseaseInstance)
			{
				Spores.SporesComponent.InstanceData instanceData = default(Spores.SporesComponent.InstanceData);
				instanceData.schedulerHandle = GameScheduler.Instance.SchedulePeriodic("EmitSpores", 3f, new Action<object>(this.Emit), go, null, 0f, null);
				this.Emit(go);
				return instanceData;
			}

			public override void OnCure(GameObject go, object instace_data)
			{
				((Spores.SporesComponent.InstanceData)instace_data).schedulerHandle.ClearScheduler();
			}

			private void Emit(object data)
			{
				GameObject gameObject = (GameObject)data;
				int num = Grid.PosToCell(gameObject.transform.position);
				float value = Db.Get().Amounts.Temperature.Lookup(gameObject).value;
				SimMessages.AddRemoveSubstance(num, SimHashes.ContaminatedOxygen, CellEventLogger.Instance.ElementConsumerSimUpdate, 0.05f, value, byte.MaxValue, 0, -1);
				KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("spore_fx_kanim", gameObject.transform.position, gameObject.transform, true, Grid.SceneLayer.Front, false);
				kbatchedAnimController.Play(Spores.SporesComponent.WorkLoopAnims, KAnim.PlayMode.Once);
				kbatchedAnimController.destroyOnAnimComplete = true;
			}

			private static readonly HashedString[] WorkLoopAnims = new HashedString[] { "working_pre", "working_loop", "working_pst" };

			private struct InstanceData
			{
				public SchedulerHandle schedulerHandle;
			}
		}
	}
}
