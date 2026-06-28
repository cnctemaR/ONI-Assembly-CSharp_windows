using System;
using System.Collections.Generic;

public struct TagBits
{
	public TagBits(Tag[] tags)
	{
		this.bits0 = 0UL;
		this.bits1 = 0UL;
		this.bits2 = 0UL;
		for (int i = 0; i < tags.Length; i++)
		{
			this.SetTag(tags[i]);
		}
	}

	private static TagBits GetTagBits(Tag tag)
	{
		TagBits tagBits;
		if (!TagBits.tagTable.TryGetValue(tag, out tagBits))
		{
			tagBits.SetFlag(TagBits.tagTable.Count);
			TagBits.tagTable.Add(tag, tagBits);
			DebugUtil.Assert(TagBits.tagTable.Count <= 192, "Assert!");
		}
		return tagBits;
	}

	private void SetFlag(int flag_idx)
	{
		if (flag_idx < 64)
		{
			this.bits0 |= 1UL << flag_idx;
		}
		else if (flag_idx < 128)
		{
			this.bits1 |= 1UL << flag_idx;
		}
		else
		{
			this.bits2 |= 1UL << flag_idx;
		}
	}

	public void SetTag(Tag tag)
	{
		TagBits tagBits = TagBits.GetTagBits(tag);
		this.bits0 |= tagBits.bits0;
		this.bits1 |= tagBits.bits1;
		this.bits2 |= tagBits.bits2;
	}

	public void Clear(Tag tag)
	{
		TagBits tagBits = TagBits.GetTagBits(tag);
		this.bits0 &= ~tagBits.bits0;
		this.bits1 &= ~tagBits.bits1;
		this.bits2 &= ~tagBits.bits2;
	}

	public bool HasAny(TagBits tag_bits)
	{
		return (this.bits0 & tag_bits.bits0) != 0UL || (this.bits1 & tag_bits.bits1) != 0UL || (this.bits2 & tag_bits.bits2) != 0UL;
	}

	public bool AreEqual(TagBits tag_bits)
	{
		return this.bits0 == tag_bits.bits0 && this.bits1 == tag_bits.bits1 && this.bits2 == tag_bits.bits2;
	}

	private static Dictionary<Tag, TagBits> tagTable = new Dictionary<Tag, TagBits>();

	private ulong bits0;

	private ulong bits1;

	private ulong bits2;
}
