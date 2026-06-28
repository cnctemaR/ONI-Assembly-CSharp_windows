using System;
using UnityEngine;

namespace Klei.AI
{
	public class FierySkin : AnimatedDisease
	{
		public FierySkin()
			: base("FierySkin", 0.1f, 900f, "anim_idle_fiery", "SickFierySkin")
		{
		}

		protected override object OnInfect(GameObject go)
		{
			base.OnInfect(go);
			FierySkin.InstanceData instanceData = default(FierySkin.InstanceData);
			instanceData.schedulerHandle = GameScheduler.Instance.SchedulePeriodic("EmitHeat", 3f, new Action<object>(this.Emit), go, null, 0f);
			this.Emit(go);
			KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("fiery_fx", go.transform, true, Grid.SceneLayer.Front);
			kbatchedAnimController.transform.localPosition = Vector3.zero;
			kbatchedAnimController.Play(new string[] { "working_pre", "working_loop" }, KAnim.PlayMode.Loop);
			instanceData.controller = kbatchedAnimController;
			return instanceData;
		}

		protected override void OnCure(GameObject go, object instance_data)
		{
			FierySkin.InstanceData instanceData = (FierySkin.InstanceData)instance_data;
			instanceData.schedulerHandle.Clear();
			KAnimControllerBase controller = instanceData.controller;
			controller.Play("working_pst", KAnim.PlayMode.Once, 1f, 0f);
			controller.destroyOnAnimComplete = true;
			base.OnCure(go, instance_data);
		}

		private void Emit(object data)
		{
			GameObject gameObject = (GameObject)data;
			int num = Grid.PosToCell(gameObject.transform.position);
			int num2 = Grid.CellAbove(num);
			SimMessages.ModifyEnergy(num, 5f, SimMessages.EnergySourceID.FierySkin);
			SimMessages.ModifyEnergy(num2, 5f, SimMessages.EnergySourceID.FierySkin);
		}

		private const float EmitInterval = 3f;

		private const float EmitEnergy = 10f;

		private struct InstanceData
		{
			public SchedulerHandle schedulerHandle;

			public KAnimControllerBase controller;
		}
	}
}
