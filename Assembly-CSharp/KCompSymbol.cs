using System;
using System.Collections.Generic;

public class KCompSymbol
{
	public KCompSymbol(KAnimHashedString name, KAnimFile file)
	{
		this.name = name;
		foreach (KAnim.Anim anim in file.GetData().anims)
		{
			if (anim.rootSymbol == name)
			{
				this.anims.Add(anim);
				if (anim.name == "neutral")
				{
					this.defaultAnim = anim;
				}
			}
		}
	}

	public KAnim.Anim GetAnim(KAnimHashedString name)
	{
		foreach (KAnim.Anim anim in this.anims)
		{
			if (anim.hash == name)
			{
				return anim;
			}
		}
		return null;
	}

	public KAnimHashedString name;

	private List<KAnim.Anim> anims = new List<KAnim.Anim>();

	public KAnim.Anim defaultAnim;
}
