using System;
using System.Collections.Generic;

public class TagCollection : IReadonlyTags
{
	public TagCollection()
	{
	}

	public TagCollection(int[] initialTags)
	{
		for (int i = 0; i < initialTags.Length; i++)
		{
			this.tags.Add(initialTags[i]);
		}
	}

	public TagCollection(string[] initialTags)
	{
		for (int i = 0; i < initialTags.Length; i++)
		{
			this.tags.Add(Hash.SDBMLower(initialTags[i]));
		}
	}

	public TagCollection(TagCollection initialTags)
	{
		if (initialTags != null && initialTags.tags != null)
		{
			this.tags.UnionWith(initialTags.tags);
		}
	}

	public TagCollection Append(TagCollection others)
	{
		foreach (int num in others.tags)
		{
			this.tags.Add(num);
		}
		return this;
	}

	public void AddTag(string tag)
	{
		this.tags.Add(Hash.SDBMLower(tag));
	}

	public void AddTag(int tag)
	{
		this.tags.Add(tag);
	}

	public void RemoveTag(string tag)
	{
		this.tags.Remove(Hash.SDBMLower(tag));
	}

	public void RemoveTag(int tag)
	{
		this.tags.Remove(tag);
	}

	public bool HasTag(string tag)
	{
		return this.tags.Contains(Hash.SDBMLower(tag));
	}

	public bool HasTag(int tag)
	{
		return this.tags.Contains(tag);
	}

	public bool HasTags(int[] searchTags)
	{
		for (int i = 0; i < searchTags.Length; i++)
		{
			if (!this.tags.Contains(searchTags[i]))
			{
				return false;
			}
		}
		return true;
	}

	private HashSet<int> tags = new HashSet<int>();
}
