using System;

public class Accessory : Resource
{
	public KAnim.Build.Symbol symbol { get; private set; }

	public HashedString batchSource { get; private set; }

	public AccessorySlot slot { get; private set; }

	public KAnimFile animFile { get; private set; }

	public Accessory(string id, ResourceSet parent, AccessorySlot slot, HashedString batchSource, KAnim.Build.Symbol symbol, KAnimFile animFile = null, KAnimFile defaultAnimFile = null)
		: base(id, parent, null)
	{
		this.slot = slot;
		this.symbol = symbol;
		this.batchSource = batchSource;
		this.animFile = animFile;
	}

	public bool IsDefault()
	{
		return this.animFile == this.slot.defaultAnimFile;
	}
}
