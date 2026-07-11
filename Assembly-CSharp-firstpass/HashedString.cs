using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[Serializable]
public struct HashedString : IComparable<HashedString>, IEquatable<HashedString>, ISerializationCallbackReceiver
{
	public static implicit operator HashedString(string s)
	{
		return new HashedString(s);
	}

	public bool IsValid
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

	public HashedString(string name)
	{
		this.hash = global::Hash.SDBMLower(name);
	}

	public static int Hash(string name)
	{
		return global::Hash.SDBMLower(name);
	}

	public HashedString(int initial_hash)
	{
		this.hash = initial_hash;
	}

	public int CompareTo(HashedString obj)
	{
		return this.hash - obj.hash;
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

	public static implicit operator HashedString(KAnimHashedString hash)
	{
		return new HashedString(hash.HashValue);
	}

	public override string ToString()
	{
		return "0x" + this.hash.ToString("X");
	}

	public void OnAfterDeserialize()
	{
	}

	public void OnBeforeSerialize()
	{
	}

	public static HashedString Invalid;

	[SerializeField]
	[Serialize]
	private int hash;
}
