using System;

namespace FoodRehydrator
{
	public class ResourceRequirementMonitor : KMonoBehaviour
	{
		protected override void OnSpawn()
		{
			base.OnSpawn();
			Storage[] components = base.GetComponents<Storage>();
			DebugUtil.DevAssert(components.Length == 2, "Incorrect number of storages on foodrehydrator", null);
			this.packages = components[0];
			this.water = components[1];
			base.Subscribe<ResourceRequirementMonitor>(-1697596308, ResourceRequirementMonitor.OnStorageChangedDelegate);
		}

		protected float GetAvailableWater()
		{
			return this.water.GetMassAvailable(GameTags.Water);
		}

		protected bool HasSufficientResources()
		{
			return this.packages.items.Count > 0 && this.GetAvailableWater() > 1f;
		}

		protected void OnStorageChanged(object _)
		{
			this.operational.SetFlag(ResourceRequirementMonitor.flag, this.HasSufficientResources());
		}

		[MyCmpReq]
		private Operational operational;

		private Storage packages;

		private Storage water;

		private static readonly Operational.Flag flag = new Operational.Flag("HasSufficientResources", Operational.Flag.Type.Requirement);

		private static readonly EventSystem.IntraObjectHandler<ResourceRequirementMonitor> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<ResourceRequirementMonitor>(delegate(ResourceRequirementMonitor component, object data)
		{
			component.OnStorageChanged(data);
		});
	}
}
