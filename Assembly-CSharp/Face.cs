using System;

public class Face : Resource
{
	public Face(string id)
		: base(id, null, null)
	{
		this.hash = new HashedString(id);
	}

	public HashedString hash;
}
