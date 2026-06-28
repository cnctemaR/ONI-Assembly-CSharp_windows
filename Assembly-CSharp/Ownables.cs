using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class Ownables : Assignables
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public void UnassignAll()
	{
		foreach (AssignableSlotInstance assignableSlotInstance in this.slots)
		{
			if (assignableSlotInstance.assignable != null)
			{
				assignableSlotInstance.assignable.Unassign();
			}
		}
	}
}
