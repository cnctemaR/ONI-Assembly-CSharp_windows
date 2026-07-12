using System;
using UnityEngine;

namespace FoodRehydrator
{
	public class AccessabilityManager : KMonoBehaviour
	{
		protected override void OnSpawn()
		{
			base.OnSpawn();
			base.Subscribe(824508782, new Action<object>(this.ActiveChangedHandler));
		}

		public void Reserve(GameObject reserver)
		{
			this.reserver = reserver;
			global::Debug.Assert(reserver != null && reserver.GetComponent<MinionResume>() != null);
		}

		public void Unreserve()
		{
			global::Debug.Assert(this.reserver != null);
			this.reserver = null;
		}

		public void SetActiveWorkable(Workable work)
		{
			DebugUtil.DevAssert(this.activeWorkable == null || work == null, "FoodRehydrator::AccessabilityManager activating a second workable", null);
			this.activeWorkable = work;
			this.operational.SetActive(this.activeWorkable != null, false);
			if (this.activeWorkable == null)
			{
				this.Unreserve();
			}
		}

		public bool CanAccess(GameObject worker)
		{
			return this.operational.IsOperational && (this.reserver == null || this.reserver == worker);
		}

		protected void ActiveChangedHandler(object obj)
		{
			if (!this.operational.IsActive && this.activeWorkable != null && this.activeWorkable.worker != null)
			{
				this.activeWorkable.worker.StopWork();
			}
		}

		[MyCmpReq]
		private Operational operational;

		private GameObject reserver;

		private Workable activeWorkable;
	}
}
