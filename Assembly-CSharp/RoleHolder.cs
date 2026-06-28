using System;
using Klei.AI;
using KSerialization;

public class RoleHolder : GameStateMachine<RoleHolder, RoleHolder.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.unassigned;
		base.serializable = false;
		this.unassigned.ParamTransition<bool>(this.isAssigned, this.assigned, (RoleHolder.Instance smi, bool p) => p);
		this.assigned.ParamTransition<bool>(this.isAssigned, this.assigned, (RoleHolder.Instance smi, bool p) => !p);
	}

	public StateMachine<RoleHolder, RoleHolder.Instance, IStateMachineTarget, object>.BoolParameter isAssigned;

	public GameStateMachine<RoleHolder, RoleHolder.Instance, IStateMachineTarget, object>.State unassigned;

	public GameStateMachine<RoleHolder, RoleHolder.Instance, IStateMachineTarget, object>.State assigned;

	public new class Instance : GameStateMachine<RoleHolder, RoleHolder.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public void SetRole(string roleID)
		{
			if (this.assignedRole != null)
			{
				this.UnapplyEntitlements(this.assignedRole);
			}
			this.assignedRole = roleID;
			if (this.assignedRole != null)
			{
				this.ApplyEntitlements(this.assignedRole);
			}
			base.sm.isAssigned.Set(this.assignedRole != null, base.smi);
		}

		public void ApplyEntitlements(string roleID)
		{
			Role role = Db.Get().Roles.Get(roleID);
			role.ApplyEntitlements(base.master.gameObject);
		}

		public void UnapplyEntitlements(string roleID)
		{
			Role role = Db.Get().Roles.Get(roleID);
			role.UnapplyEntitlements(base.master.gameObject);
		}

		[Serialize]
		public string assignedRole;
	}
}
