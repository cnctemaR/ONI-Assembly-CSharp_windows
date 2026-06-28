using System;
using System.Collections.Generic;

public class KCompBuild
{
	public KCompBuild(KAnimFile anim_file)
	{
		this.animFile = anim_file;
		this.buildFile = anim_file;
		KAnimFileData data = anim_file.GetData();
		for (int i = 0; i < data.animCount; i++)
		{
			KAnim.Anim anim = data.GetAnim(i);
			if (this.Get(anim.rootSymbol) == null)
			{
				KCompSymbol kcompSymbol = new KCompSymbol(anim.rootSymbol, anim_file);
				if (kcompSymbol.defaultAnim != null)
				{
					this.symbols.Add(kcompSymbol);
				}
			}
		}
	}

	public IEnumerator<KCompSymbol> GetEnumerator()
	{
		return this.symbols.GetEnumerator();
	}

	private KCompSymbol Get(KAnimHashedString name)
	{
		for (int i = 0; i < this.symbols.Count; i++)
		{
			if (this.symbols[i].name == name)
			{
				return this.symbols[i];
			}
		}
		return null;
	}

	public KAnimFile GetAnimFile()
	{
		return this.animFile;
	}

	public KAnimFile GetBuildFile()
	{
		return this.buildFile;
	}

	private List<KCompSymbol> symbols = new List<KCompSymbol>();

	private KAnimFile animFile = null;

	private KAnimFile buildFile = null;
}
