using System;

public class StringEntry
{
	public StringEntry(string str)
	{
		this.String = str;
	}

	public override string ToString()
	{
		return this.String;
	}

	public static implicit operator string(StringEntry entry)
	{
		return entry.String;
	}

	public string String;
}
