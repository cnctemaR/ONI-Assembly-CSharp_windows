using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[Serializable]
public struct HashedString : IComparable<HashedString>, IEquatable<HashedString>, ISerializationCallbackReceiver
{
	public HashedString(string name)
	{
		this.hash = global::Hash.SDBMLower(name);
	}

	public HashedString(int initial_hash)
	{
		this.hash = initial_hash;
	}

	public static implicit operator HashedString(string s)
	{
		return new HashedString(s);
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
		int num;
		if (this.hash < obj.hash)
		{
			num = -1;
		}
		else if (this.hash > obj.hash)
		{
			num = 1;
		}
		else
		{
			num = 0;
		}
		return num;
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

	public static bool operator ==(HashedString x, HashedString y)
	{
		return x.hash == y.hash;
	}

	public static bool operator !=(HashedString x, HashedString y)
	{
		return x.hash != y.hash;
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

	public static HashedString Invalid = default(HashedString);

	[SerializeField]
	[Serialize]
	private int hash;
}
