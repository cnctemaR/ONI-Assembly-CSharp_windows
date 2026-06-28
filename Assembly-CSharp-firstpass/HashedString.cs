using System;
using UnityEngine;

[Serializable]
public struct HashedString : ISerializationCallbackReceiver, IComparable<HashedString>, IEquatable<HashedString>
{
	public HashedString(string name)
	{
		this.hash = global::Hash.SDBMLower(name);
	}

	public bool isValid
	{
		get
		{
			return this.HashValue != 0;
		}
	}

	public int HashValue
	{
		get
		{
			return this.hash;
		}
		set
		{
			this.hash = value;
		}
	}

	public static int Hash(string name)
	{
		return global::Hash.SDBMLower(name);
	}

	public int CompareTo(HashedString obj)
	{
		if (this.hash < obj.hash)
		{
			return -1;
		}
		if (this.hash > obj.hash)
		{
			return 1;
		}
		return 0;
	}

	public override bool Equals(object obj)
	{
		HashedString hashedString = (HashedString)obj;
		return this.hash == hashedString.hash;
	}

	public bool Equals(HashedString other)
	{
		return this.hash == other.hash;
	}

	public override int GetHashCode()
	{
		return this.hash;
	}

	public override string ToString()
	{
		return base.ToString();
	}

	public void OnAfterDeserialize()
	{
	}

	public void OnBeforeSerialize()
	{
	}

	public bool IsValid()
	{
		return this.hash != 0;
	}

	public static bool operator ==(HashedString x, HashedString y)
	{
		return x.hash == y.hash;
	}

	public static bool operator !=(HashedString x, HashedString y)
	{
		return x.hash != y.hash;
	}

	[SerializeField]
	private int hash;

	public static HashedString Invalid = default(HashedString);
}
