using System;

public struct StringKey
{
	public StringKey(string str)
	{
		this.String = str;
		this.Hash = str.GetHashCode();
	}

	public override string ToString()
	{
		return Strings.Get(this);
	}

	public bool IsValid()
	{
		return this.Hash != 0;
	}

	public static implicit operator string(StringKey key)
	{
		return Strings.Get(key);
	}

	public string String;

	public int Hash;
}
