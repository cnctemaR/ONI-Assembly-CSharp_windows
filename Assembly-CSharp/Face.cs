using System;

public class Face : Resource
{
	public Face(string id, string headFXSymbol = null)
		: base(id, null, null)
	{
		this.hash = new HashedString(id);
		this.headFXHash = headFXSymbol;
	}

	public HashedString hash;

	public HashedString headFXHash;

	private const string SYMBOL_PREFIX = "headfx_";
}
