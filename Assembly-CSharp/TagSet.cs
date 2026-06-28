using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[Serializable]
public class TagSet : IEnumerable, ICollection<Tag>, IEnumerable<Tag>
{
	public TagSet()
	{
		this.tags = new List<Tag>();
	}

	public TagSet(TagSet other)
	{
		this.tags = new List<Tag>(other.tags);
	}

	public TagSet(Tag[] other)
	{
		this.tags = new List<Tag>(other);
	}

	public TagSet(IEnumerable<string> others)
	{
		this.tags = new List<Tag>();
		foreach (string text in others)
		{
			this.tags.Add(new Tag(text));
		}
	}

	public TagSet(params TagSet[] others)
	{
		this.tags = new List<Tag>();
		for (int i = 0; i < others.Length; i++)
		{
			this.tags.AddRange(others[i]);
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	public int Count
	{
		get
		{
			return this.tags.Count;
		}
	}

	public bool IsReadOnly
	{
		get
		{
			return false;
		}
	}

	public void Add(Tag item)
	{
		if (!this.tags.Contains(item))
		{
			this.tags.Add(item);
		}
	}

	public void Union(TagSet others)
	{
		for (int i = 0; i < others.tags.Count; i++)
		{
			if (!this.tags.Contains(others.tags[i]))
			{
				this.tags.Add(others.tags[i]);
			}
		}
	}

	public void Clear()
	{
		this.tags.Clear();
	}

	public bool Contains(Tag item)
	{
		return this.tags.Contains(item);
	}

	public bool ContainsAll(TagSet others)
	{
		for (int i = 0; i < others.tags.Count; i++)
		{
			if (!this.tags.Contains(others.tags[i]))
			{
				return false;
			}
		}
		return true;
	}

	public bool ContainsOne(TagSet others)
	{
		for (int i = 0; i < others.tags.Count; i++)
		{
			if (this.tags.Contains(others.tags[i]))
			{
				return true;
			}
		}
		return false;
	}

	public void CopyTo(Tag[] array, int arrayIndex)
	{
		this.tags.CopyTo(array, arrayIndex);
	}

	public bool Remove(Tag item)
	{
		return this.tags.Remove(item);
	}

	public void Remove(TagSet other)
	{
		for (int i = 0; i < other.tags.Count; i++)
		{
			if (this.tags.Contains(other.tags[i]))
			{
				this.tags.Remove(other.tags[i]);
			}
		}
	}

	public IEnumerator<Tag> GetEnumerator()
	{
		return this.tags.GetEnumerator();
	}

	public Tag this[int i]
	{
		get
		{
			return this.tags[i];
		}
	}

	public override string ToString()
	{
		if (this.tags.Count > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.tags[0].Name);
			for (int i = 1; i < this.tags.Count; i++)
			{
				stringBuilder.Append(", ");
				stringBuilder.Append(this.tags[i].Name);
			}
			return stringBuilder.ToString();
		}
		return string.Empty;
	}

	public string GetTagDescription()
	{
		if (this.tags.Count > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(TagDescriptions.GetDescription(this.tags[0].ToString()));
			for (int i = 1; i < this.tags.Count; i++)
			{
				stringBuilder.Append(", ");
				stringBuilder.Append(TagDescriptions.GetDescription(this.tags[i].ToString()));
			}
			return stringBuilder.ToString();
		}
		return string.Empty;
	}

	[SerializeField]
	[Serialize]
	private List<Tag> tags = new List<Tag>();
}
