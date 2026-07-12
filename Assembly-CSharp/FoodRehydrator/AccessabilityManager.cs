using System;
using UnityEngine;

namespace FoodRehydrator
{
	public class AccessabilityManager : KMonoBehaviour
	{
		protected override void OnSpawn()
		{
			base.OnSpawn();
			Components.FoodRehydrators.Add(base.gameObject);
			base.Subscribe(824508782, new Action<object>(this.ActiveChangedHandler));
		}

		protected override void OnCleanUp()
		{
			Components.FoodRehydrators.Remove(base.gameObject);
			base.OnCleanUp();
		}

		public void Reserve(GameObject reserver)
		{
			this.reserver = reserver;
			global::Debug.Assert(reserver != null && reserver.GetComponent<MinionResume>() != null);
		}

		public void Unreserve()
		{
			this.activeWorkable = null;
			this.reserver = null;
		}

		public void SetActiveWorkable(Workable work)
		{
			DebugUtil.DevAssert(this.activeWorkable == null || work == null, "FoodRehydrator::AccessabilityManager activating a second workable", null);
			this.activeWorkable = work;
			this.operational.SetActive(this.activeWorkable != null, false);
		}

		public bool CanAccess(GameObject worker)
		{
			return this.operational.IsOperational && (this.reserver == null || this.reserver == worker);
		}

		protected void ActiveChangedHandler(object obj)
		{
			if (!this.operational.IsActive)
			{
				this.CancelActiveWorkable();
			}
		}

		public void CancelActiveWorkable()
		{
			if (this.activeWorkable != null)
			{
				this.activeWorkable.StopWork(this.activeWorkable.worker, true);
			}
		}

		[MyCmpReq]
		private Operational operational;

		private GameObject reserver;

		private Workable activeWorkable;
	}
}
