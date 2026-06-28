using System;

internal struct EffectorEntry
{
	public EffectorEntry(string name, int value)
	{
		this.name = name;
		this.value = value;
		this.count = 1;
	}

	public override string ToString()
	{
		string text = string.Empty;
		if (this.value > 0)
		{
			text = "+";
		}
		string text2 = string.Empty;
		if (this.count > 1)
		{
			text2 = " (" + this.count + ")";
		}
		return string.Concat(new string[]
		{
			this.name,
			text2,
			": ",
			text,
			this.value.ToString()
		});
	}

	public string ToStringDecibel()
	{
		string text = string.Empty;
		if (this.count > 1)
		{
			text = " (" + this.count + ")";
		}
		return this.name + text + ": " + GameUtil.GetFormattedDecibels((float)this.value);
	}

	public string name;

	public int count;

	public int value;
}
