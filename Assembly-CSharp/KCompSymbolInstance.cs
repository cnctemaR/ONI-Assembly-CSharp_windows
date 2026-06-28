using System;

public class KCompSymbolInstance
{
	public KCompSymbolInstance(KCompSymbol symbol, KCompBuildInstance build)
	{
		this.symbol = symbol;
		this.build = build;
	}

	private KAnim.Anim GetAnim(KAnimHashedString parent_anim)
	{
		KAnimHashedString kanimHashedString = this.build.GetAnimOverride();
		if (kanimHashedString.IsValid())
		{
			KAnim.Anim anim = this.symbol.GetAnim(kanimHashedString);
			if (anim != null)
			{
				return anim;
			}
		}
		if (parent_anim.IsValid())
		{
			KAnim.Anim anim2 = this.symbol.GetAnim(parent_anim);
			if (anim2 != null)
			{
				return anim2;
			}
		}
		return this.symbol.defaultAnim;
	}

	public KAnim.Anim.Frame GetFrame(KAnimHashedString parent_anim, int frame_idx)
	{
		KAnim.Anim anim = this.GetAnim(parent_anim);
		if (anim == null)
		{
			Debug.LogError("Head comp missing anim:" + HashCache.Get().Get(parent_anim), null);
		}
		return anim.GetFrame(this.build.GetData().batchTag, frame_idx);
	}

	private KCompSymbol symbol;

	private KCompBuildInstance build;
}
