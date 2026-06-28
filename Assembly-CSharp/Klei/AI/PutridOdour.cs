using System;
using UnityEngine;

namespace Klei.AI
{
	public class PutridOdour : Disease
	{
		public PutridOdour()
			: base("PutridOdour", 0.00083333335f, 900f, null)
		{
		}

		protected override object OnInfect(GameObject go)
		{
			PutridOdour.InstanceData instanceData = default(PutridOdour.InstanceData);
			instanceData.schedulerHandle = GameScheduler.Instance.SchedulePeriodic("PutridOdourEmit", 5f, new Action<object>(this.Emit), go, null, 0f);
			KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("odor_fx", go.transform, true, Grid.SceneLayer.Front);
			kbatchedAnimController.transform.localPosition = Vector3.zero;
			kbatchedAnimController.Play(new string[] { "working_pre", "working_loop" }, KAnim.PlayMode.Loop);
			instanceData.controller = kbatchedAnimController;
			this.Emit(go);
			return instanceData;
		}

		protected override void OnCure(GameObject go, object instance_data)
		{
			PutridOdour.InstanceData instanceData = (PutridOdour.InstanceData)instance_data;
			KAnimControllerBase controller = instanceData.controller;
			controller.Play("working_pst", KAnim.PlayMode.Once, 1f, 0f);
			controller.destroyOnAnimComplete = true;
			instanceData.schedulerHandle.Clear();
		}

		private void Emit(object data)
		{
			GameObject gameObject = (GameObject)data;
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

		private const float EmitInterval = 5f;

		private const float EmissionRadius = 1.5f;

		private const float MaxDistanceSq = 2.25f;

		private struct InstanceData
		{
			public SchedulerHandle schedulerHandle;

			public KAnimControllerBase controller;
		}
	}
}
