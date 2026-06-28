using System;

internal struct DecorEntry
{
	public DecorEntry(string name, int decor)
	{
		this.name = name;
		this.decor = decor;
		this.count = 1;
	}

	public override string ToString()
	{
		string text = string.Empty;
		if (this.decor > 0)
		{
			text = "+";
		}
		string text2 = string.Empty;
		if (this.count > 1)
		{
			text2 = "(" + this.count + ")";
		}
		return string.Concat(new string[]
		{
			this.name,
			text2,
			": ",
			text,
			this.decor.ToString()
		});
	}

	public string name;

	public int count;

	public int decor;
}
