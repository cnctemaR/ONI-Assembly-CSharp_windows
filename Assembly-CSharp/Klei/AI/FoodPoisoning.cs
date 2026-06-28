using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class FoodPoisoning : Disease
	{
		public FoodPoisoning()
			: base("FoodPoisoning", 0.1f, 900f, new Disease.EffectProbabilityDelta[]
			{
				new Disease.EffectProbabilityDelta
				{
					effectID = "DirtyHands",
					probabilityDelta = 5E-08f
				}
			})
		{
		}

		protected override object OnInfect(GameObject go)
		{
			KAnimControllerBase kanimControllerBase = base.StartCommonSickEffect(go);
			FoodPoisoning.InstanceData instanceData = new FoodPoisoning.InstanceData(go, kanimControllerBase);
			instanceData.StartChore();
			return instanceData;
		}

		protected override void OnCure(GameObject go, object instance_data)
		{
			FoodPoisoning.InstanceData instanceData = (FoodPoisoning.InstanceData)instance_data;
			instanceData.Shutdown();
		}

		private class InstanceData
		{
			public InstanceData(GameObject go, KAnimControllerBase fx_controller)
			{
				this.go = go;
				this.fxController = fx_controller;
			}

			public void StartChore()
			{
				ChoreProvider chore_provider = this.go.GetComponent<ChoreProvider>();
				this.vomitHandle = GameScheduler.Instance.Schedule("Vomit", 30f, delegate(object data)
				{
					this.chore = new VomitChore(Db.Get().ChoreTypes.Vomit, chore_provider, Db.Get().DuplicantStatusItems.Vomiting, this.vomiting, delegate(Chore unused)
					{
						this.StartChore();
					});
				}, null, null);
			}

			private void StopChore()
			{
				if (this.chore != null)
				{
					this.vomitHandle.Clear();
					this.chore.Cancel("FoodPoisoning.StopChore");
				}
			}

			public void Shutdown()
			{
				this.fxController.gameObject.DeleteObject();
				this.StopChore();
			}

			private GameObject go;

			private Chore chore;

			private KAnimControllerBase fxController;

			private SchedulerHandle vomitHandle;

			public Notification vomiting = new Notification(DUPLICANTS.STATUSITEMS.VOMITING.NOTIFICATION_NAME, NotificationType.Bad, HashedString.Invalid, (List<Notification> notificationList, object data) => DUPLICANTS.STATUSITEMS.VOMITING.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false), null, true, 0f, null, null, null);
		}
	}
}
