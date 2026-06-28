using System;
using UnityEngine;

namespace Klei.AI
{
	public class Spores : Disease
	{
		public Spores()
			: base("Spores", 0.00083333335f, 900f, null)
		{
		}

		protected override object OnInfect(GameObject go)
		{
			Spores.InstanceData instanceData = default(Spores.InstanceData);
			instanceData.schedulerHandle = GameScheduler.Instance.SchedulePeriodic("EmitSpores", 3f, new Action<object>(this.Emit), go, null, 0f, null);
			KAnimFile anim = Assets.GetAnim("anim_idle_spores_kanim");
			go.GetComponent<KAnimControllerBase>().AddAnimOverrides(anim, 10f);
			go.GetComponent<FaceGraph>().AddExpression(Db.Get().Expressions.SickSpores);
			this.Emit(go);
			return instanceData;
		}

		protected override void OnCure(GameObject go, object instace_data)
		{
			go.GetComponent<FaceGraph>().RemoveExpression(Db.Get().Expressions.SickSpores);
			KAnimFile anim = Assets.GetAnim("anim_idle_spores_kanim");
			go.GetComponent<KAnimControllerBase>().RemoveAnimOverrides(anim);
			((Spores.InstanceData)instace_data).schedulerHandle.Clear();
		}

		private void Emit(object data)
		{
			GameObject gameObject = (GameObject)data;
			int num = Grid.PosToCell(gameObject.transform.position);
			float value = Db.Get().Amounts.Temperature.Lookup(gameObject).value;
			SimMessages.AddRemoveSubstance(num, SimHashes.ContaminatedOxygen, CellEventLogger.Instance.ElementConsumerSimUpdate, 0.05f, value, -1);
			KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("spore_fx_kanim", gameObject.transform.position, gameObject.transform, true, Grid.SceneLayer.Front);
			kbatchedAnimController.Play(Spores.WorkLoopAnims, KAnim.PlayMode.Once);
			kbatchedAnimController.destroyOnAnimComplete = true;
		}

		private const float EmitInterval = 3f;

		private const float EmitMass = 0.05f;

		private const SimHashes EmitElement = SimHashes.ContaminatedOxygen;

		private const string KAnimFilename = "anim_idle_spores_kanim";

		private static readonly HashedString[] WorkLoopAnims = new HashedString[] { "working_pre", "working_loop", "working_pst" };

		private struct InstanceData
		{
			public SchedulerHandle schedulerHandle;
		}
	}
}
