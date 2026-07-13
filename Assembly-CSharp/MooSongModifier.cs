using System;

public class MooSongModifier : Resource
{
	public MooSongModifier(string id, Tag targetTag, string name, string description, Func<string, string> tooltipCB, MooSongModifier.MooSongModFn applyFunction)
		: base(id, name)
	{
		this.Description = description;
		this.TargetTag = targetTag;
		this.TooltipCB = tooltipCB;
		this.ApplyFunction = applyFunction;
	}

	public string GetTooltip()
	{
		if (this.TooltipCB != null)
		{
			return this.TooltipCB(this.Description);
		}
		return this.Description;
	}

	public string Description;

	public Tag TargetTag;

	public Func<string, string> TooltipCB;

	public MooSongModifier.MooSongModFn ApplyFunction;

	public delegate void MooSongModFn(BeckoningMonitor.Instance inst, Tag meteorTag);
}
