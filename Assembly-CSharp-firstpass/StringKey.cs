using System;

[Serializable]
public struct StringKey
{
	public StringKey(string str)
	{
		this.String = str;
		this.Hash = str.GetHashCode();
	}

	public override string ToString()
	{
		return string.Concat(new string[]
		{
			"S: [",
			this.String,
			"] H: [",
			this.Hash.ToString(),
			"] Value: [",
			Strings.Get(this),
			"]"
		});
	}

	public bool IsValid()
	{
		return this.Hash != 0;
	}

	public string String;

	public int Hash;
}
