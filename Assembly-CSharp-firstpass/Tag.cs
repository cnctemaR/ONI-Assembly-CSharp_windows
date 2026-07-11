using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[Serializable]
public struct Tag : ISerializationCallbackReceiver, IEquatable<Tag>, IComparable<Tag>
{
	public Tag(int hash)
	{
		this.hash = hash;
		this.name = "";
	}

	public Tag(Tag orig)
	{
		this.name = orig.name;
		this.hash = orig.hash;
	}

	public Tag(string name)
	{
		this.name = name;
		this.hash = Hash.SDBMLower(name);
	}

	public string Name
	{
		get
		{
			return this.name;
		}
		set
		{
			this.name = string.Intern(value);
			this.hash = Hash.SDBMLower(this.name);
		}
	}

	public bool IsValid
	{
		get
		{
			return this.hash != 0;
		}
	}

	public void Clear()
	{
		this.name = null;
		this.hash = 0;
	}

	public override int GetHashCode()
	{
		return this.hash;
	}

	public int GetHash()
	{
		return this.hash;
	}

	public override bool Equals(object obj)
	{
		Tag tag = (Tag)obj;
		return this.hash == tag.hash;
	}

	public bool Equals(Tag other)
	{
		return this.hash == other.hash;
	}

	public static bool operator ==(Tag a, Tag b)
	{
		return a.hash == b.hash;
	}

	public static bool operator !=(Tag a, Tag b)
	{
		return a.hash != b.hash;
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		if (this.name != null)
		{
			this.Name = this.name;
			return;
		}
		this.name = "";
	}

	public int CompareTo(Tag other)
	{
		return this.hash - other.hash;
	}

	public override string ToString()
	{
		if (this.name == null)
		{
			return this.hash.ToString("X");
		}
		return this.name;
	}

	public static implicit operator Tag(string s)
	{
		return new Tag(s);
	}

	public static readonly Tag Invalid;

	[Serialize]
	[SerializeField]
	private string name;

	[Serialize]
	private int hash;
}
