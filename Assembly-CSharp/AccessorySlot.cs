using System;
using System.Collections.Generic;
using UnityEngine;

public class AccessorySlot : Resource
{
	public AccessorySlot(string id, ResourceSet parent, KAnimFile swap_build)
		: base(id, parent, null)
	{
		Debug.AssertFormat(swap_build != null, "AccessorySlot {0} missing swap_build", new object[] { id });
		this.targetSymbolId = new KAnimHashedString("snapTo_" + id.ToLower());
		this.accessories = new List<Accessory>();
		this.file = swap_build;
	}

	public KAnimHashedString targetSymbolId { get; private set; }

	public List<Accessory> accessories { get; private set; }

	public void AddAccessories(KAnimFile default_build, ResourceSet parent)
	{
		KAnim.Build build = this.file.GetData().build;
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
				Accessory accessory2 = new Accessory(text2, parent, this.accessories.Count, this, this.file.batchTag, build.symbols[i]);
				this.accessories.Add(accessory2);
			}
		}
	}

	private KAnimFile file;
}
