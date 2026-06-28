using System;

public class AssignableSlot : Resource
{
	public AssignableSlot(string id, string name, bool showInUI = true)
		: base(id, name)
	{
		this.showInUI = showInUI;
	}

	public bool showInUI = true;
}
