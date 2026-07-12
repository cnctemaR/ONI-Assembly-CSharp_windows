using System;

public struct TagChangedEventData
{
	public TagChangedEventData(Tag tag, bool added)
	{
		this.tag = tag;
		this.added = added;
	}

	public Tag tag;

	public bool added;
}
