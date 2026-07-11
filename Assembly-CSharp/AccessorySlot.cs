using System;
using System.Collections.Generic;

public class AccessorySlot : Resource
{
	public AccessorySlot(string id, ResourceSet parent, KAnimFile swap_build, string build_symbol_override = null)
		: base(id, parent, null)
	{
		if (swap_build == null)
		{
			Debug.LogErrorFormat("AccessorySlot {0} missing swap_build", new object[] { id });
		}
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
		string text = this.Id.ToLower();
		if (symbol != null)
		{
			string text2 = text + "_DEFAULT";
			Accessory accessory = new Accessory(text2, parent, this, default_build.batchTag, symbol);
			this.accessories.Add(accessory);
			HashCache.Get().Add(accessory.IdHash.HashValue, accessory.Id);
		}
		for (int i = 0; i < build.symbols.Length; i++)
		{
			string text3 = HashCache.Get().Get(build.symbols[i].hash);
			if (text3.StartsWith(text))
			{
				Accessory accessory2 = new Accessory(text3, parent, this, this.file.batchTag, build.symbols[i]);
				this.accessories.Add(accessory2);
				HashCache.Get().Add(accessory2.IdHash.HashValue, accessory2.Id);
			}
		}
	}

	public Accessory Lookup(string id)
	{
		return this.Lookup(new HashedString(id));
	}

	public Accessory Lookup(HashedString full_id)
	{
		return this.accessories.Find((Accessory a) => a.IdHash == full_id);
	}

	private KAnimFile file;
}
