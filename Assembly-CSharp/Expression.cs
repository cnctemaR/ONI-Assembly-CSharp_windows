using System;

public class Expression : Resource
{
	public Expression(string id, ResourceSet parent, Face face)
		: base(id, parent, null)
	{
		this.face = face;
	}

	public Face face;

	public int priority;
}
