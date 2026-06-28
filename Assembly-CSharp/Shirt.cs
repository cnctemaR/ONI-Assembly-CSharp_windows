using System;

public class Shirt : Resource
{
	public Shirt(string id)
		: base(id, null, null)
	{
		this.hash = new HashedString(id);
	}

	public HashedString hash;
}
