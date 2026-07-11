using System;

public class RolePerkMissingComplainer : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.requiredRolePerk.IsValid)
		{
			this.roleUpdateHandle = Game.Instance.Subscribe(-1523247426, new Action<object>(this.UpdateStatusItem));
		}
		this.UpdateStatusItem(null);
	}

	protected override void OnCleanUp()
	{
		if (this.roleUpdateHandle != -1)
		{
			Game.Instance.Unsubscribe(this.roleUpdateHandle);
		}
		base.OnCleanUp();
	}

	protected virtual void UpdateStatusItem(object data = null)
	{
		KSelectable component = base.GetComponent<KSelectable>();
		if (component == null)
		{
			return;
		}
		if (!this.requiredRolePerk.IsValid)
		{
			return;
		}
		bool flag = Game.Instance.roleManager.GetRoleAssigneesWithPerk(this.requiredRolePerk).Count > 0;
		if (!flag && this.workStatusItemHandle == Guid.Empty)
		{
			this.workStatusItemHandle = component.AddStatusItem(Db.Get().BuildingStatusItems.ColonyLacksRequiredRolePerk, this.requiredRolePerk);
		}
		else if (flag && this.workStatusItemHandle != Guid.Empty)
		{
			component.RemoveStatusItem(this.workStatusItemHandle, false);
			this.workStatusItemHandle = Guid.Empty;
		}
	}

	public HashedString requiredRolePerk;

	private int roleUpdateHandle = -1;

	private Guid workStatusItemHandle;
}
