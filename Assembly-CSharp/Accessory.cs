using System;

public class Accessory : Resource
{
	public Accessory(string id, ResourceSet parent, int subtype, AccessorySlot slot, HashedString batchSource, KAnim.Build.Symbol symbol)
		: base(id, parent, null)
	{
		this.subtype = subtype;
		this.slot = slot;
		this.symbol = symbol;
		this.batchSource = batchSource;
	}

	public KAnim.Build.Symbol symbol { get; private set; }

	public HashedString batchSource { get; private set; }

	public AccessorySlot slot { get; private set; }

	public int subtype { get; private set; }
}
