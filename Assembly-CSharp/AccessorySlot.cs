using System;
using System.Collections.Generic;

public class AccessorySlot : Resource
{
	public AccessorySlot(string id, ResourceSet parent)
		: base(id, parent, null)
	{
		this.targetSymbolId = new KAnimHashedString("snapTo_" + id.ToLower());
		this.accessories = new List<Accessory>();
	}

	public KAnimHashedString targetSymbolId { get; private set; }

	public List<Accessory> accessories { get; private set; }

	public void AddAccessories(KAnimFile default_build, KAnimFile file, ResourceSet parent)
	{
		KAnim.Build build = file.GetData().build;
		KAnim.Build.Symbol symbol = default_build.GetData().build.GetSymbol(this.targetSymbolId);
		if (symbol != null)
		{
			Accessory accessory = new Accessory(this.Id.ToLower() + "_000", parent, this.accessories.Count, this, default_build.batchTag, symbol);
			this.accessories.Add(accessory);
		}
		string text = this.Id.ToLower();
		for (int i = 0; i < build.symbols.Length; i++)
		{
			string text2 = HashCache.Get().Get(build.symbols[i].hash);
			if (text2.StartsWith(text))
			{
				Accessory accessory2 = new Accessory(text2, parent, this.accessories.Count, this, file.batchTag, build.symbols[i]);
				this.accessories.Add(accessory2);
			}
		}
	}
}
