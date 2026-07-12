using System;

public class ElementUsage
{
	public ElementUsage(Tag tag, float amount, bool continuous)
		: this(tag, amount, continuous, null)
	{
	}

	public ElementUsage(Tag tag, float amount, bool continuous, Func<Tag, float, bool, string> customFormating)
	{
		this.tag = tag;
		this.amount = amount;
		this.continuous = continuous;
		this.customFormating = customFormating;
	}

	public Tag tag;

	public float amount;

	public bool continuous;

	public Func<Tag, float, bool, string> customFormating;
}
