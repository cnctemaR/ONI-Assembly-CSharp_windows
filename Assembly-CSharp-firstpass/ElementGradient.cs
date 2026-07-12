using System;
using System.Diagnostics;
using ProcGen;

[DebuggerDisplay("{content} {bandSize} {maxValue}")]
[Serializable]
public class ElementGradient : Gradient<string>
{
	public ElementGradient()
		: base(null, 0f)
	{
	}

	public ElementGradient(ElementGradient refToCopy)
		: this(refToCopy.content, refToCopy.bandSize, null)
	{
	}

	public ElementGradient(string content, float bandSize, SampleDescriber.Override overrides)
		: base(content, bandSize)
	{
		this.overrides = overrides;
	}

	public SampleDescriber.Override overrides { get; set; }

	public void Mod(WorldTrait.ElementBandModifier mod)
	{
		global::Debug.Assert(mod.element == base.content);
		base.bandSize *= mod.bandMultiplier;
		if (this.overrides == null)
		{
			this.overrides = new SampleDescriber.Override();
		}
		this.overrides.ModMultiplyMass(mod.massMultiplier);
	}
}
