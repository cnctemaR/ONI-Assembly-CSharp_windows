using System;
using System.Collections.Generic;

public class TagNameComparer : IComparer<Tag>
{
	public TagNameComparer()
	{
	}

	public TagNameComparer(Tag firstTag)
	{
		this.firstTag = firstTag;
	}

	public int Compare(Tag x, Tag y)
	{
		if (x == y)
		{
			return 0;
		}
		if (this.firstTag.IsValid)
		{
			if (x == this.firstTag && y != this.firstTag)
			{
				return 1;
			}
			if (x != this.firstTag && y == this.firstTag)
			{
				return -1;
			}
		}
		return x.ProperNameStripLink().CompareTo(y.ProperNameStripLink());
	}

	private Tag firstTag;
}
