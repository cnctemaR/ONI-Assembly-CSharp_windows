using System;

public class ElementUsage
{
	public ElementUsage(Tag tag, float amount, bool continuous)
	{
		this.tag = tag;
		this.amount = amount;
		this.continuous = continuous;
	}

	public Tag tag;

	public float amount;

	public bool continuous;
}
