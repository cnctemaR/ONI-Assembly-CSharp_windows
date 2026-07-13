using System;

public struct DevQuickActionInstruction
{
	public DevQuickActionInstruction(IDevQuickAction.CommonMenusNames category, string name, global::System.Action action)
	{
		this = new DevQuickActionInstruction(category.ToString() + "/" + name, action);
	}

	public DevQuickActionInstruction(string address, global::System.Action action)
	{
		this.Address = address;
		this.Action = action;
	}

	public string Address;

	public global::System.Action Action;
}
