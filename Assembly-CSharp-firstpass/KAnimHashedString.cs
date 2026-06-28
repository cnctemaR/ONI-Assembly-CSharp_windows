using System;
using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("Name = {DebuggerDisplay}")]
[Serializable]
public struct KAnimHashedString : IComparable<KAnimHashedString>, IEquatable<KAnimHashedString>
{
	public KAnimHashedString(string name)
	{
		this.hash = Hash.SDBMLower(name);
	}

	public KAnimHashedString(int hash)
	{
		this.hash = hash;
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

	public bool IsValid()
	{
		return this.hash != 0;
	}

	public string DebuggerDisplay
	{
		get
		{
			return HashCache.Get().Get(this.hash);
		}
	}

	public static implicit operator KAnimHashedString(HashedString hash)
	{
		return new KAnimHashedString(hash.HashValue);
	}

	public static implicit operator KAnimHashedString(string str)
	{
		return new KAnimHashedString(str);
	}

	public int CompareTo(KAnimHashedString obj)
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
		KAnimHashedString kanimHashedString = (KAnimHashedString)obj;
		return this.hash == kanimHashedString.hash;
	}

	public bool Equals(KAnimHashedString other)
	{
		return this.hash == other.hash;
	}

	public override int GetHashCode()
	{
		return this.hash;
	}

	public static bool operator ==(KAnimHashedString x, HashedString y)
	{
		return x.HashValue == y.HashValue;
	}

	public static bool operator !=(KAnimHashedString x, HashedString y)
	{
		return x.HashValue != y.HashValue;
	}

	public static bool operator ==(KAnimHashedString x, KAnimHashedString y)
	{
		return x.hash == y.hash;
	}

	public static bool operator !=(KAnimHashedString x, KAnimHashedString y)
	{
		return x.hash != y.hash;
	}

	public override string ToString()
	{
		return string.IsNullOrEmpty(this.DebuggerDisplay) ? ("0x" + this.hash.ToString("X")) : this.DebuggerDisplay;
	}

	[SerializeField]
	private int hash;
}
