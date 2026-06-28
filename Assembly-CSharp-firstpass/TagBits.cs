using System;
using System.Collections.Generic;

public struct TagBits
{
	public TagBits(Tag tag)
	{
		this.bits0 = 0UL;
		this.bits1 = 0UL;
		this.bits2 = 0UL;
		this.bits3 = 0UL;
		this.SetTag(tag);
	}

	public TagBits(Tag[] tags)
	{
		this.bits0 = 0UL;
		this.bits1 = 0UL;
		this.bits2 = 0UL;
		this.bits3 = 0UL;
		if (tags == null)
		{
			return;
		}
		for (int i = 0; i < tags.Length; i++)
		{
			this.SetTag(tags[i]);
		}
	}

	public List<Tag> GetTagsVerySlow()
	{
		List<Tag> list = new List<Tag>();
		this.GetTagsVerySlow(0, this.bits0, list);
		this.GetTagsVerySlow(1, this.bits1, list);
		this.GetTagsVerySlow(2, this.bits2, list);
		this.GetTagsVerySlow(3, this.bits3, list);
		return list;
	}

	public void GetTagsVerySlow(int bits_idx, ulong bits, List<Tag> tags)
	{
		for (int i = 0; i < 64; i++)
		{
			if ((bits & (1UL << i)) != 0UL)
			{
				int num = 64 * bits_idx + i;
				tags.Add(TagBits.inverseTagTable[num]);
			}
		}
	}

	private static TagBits GetTagBits(Tag tag)
	{
		TagBits tagBits;
		if (!TagBits.tagTable.TryGetValue(tag, out tagBits))
		{
			int count = TagBits.tagTable.Count;
			tagBits.SetFlag(count);
			TagBits.tagTable.Add(tag, tagBits);
			TagBits.inverseTagTable[count] = tag;
			if (TagBits.tagTable.Count >= 256)
			{
				string text = "Out of tag bits:";
				foreach (KeyValuePair<Tag, TagBits> keyValuePair in TagBits.tagTable)
				{
					text = text + "\n" + keyValuePair.Key.ToString();
				}
				Debug.LogError(text, null);
			}
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
		else if (flag_idx < 192)
		{
			this.bits2 |= 1UL << flag_idx;
		}
		else if (flag_idx < 256)
		{
			this.bits3 |= 1UL << flag_idx;
		}
		else
		{
			Debug.LogError("Out of bits!", null);
		}
	}

	public void SetTag(Tag tag)
	{
		TagBits tagBits = TagBits.GetTagBits(tag);
		this.bits0 |= tagBits.bits0;
		this.bits1 |= tagBits.bits1;
		this.bits2 |= tagBits.bits2;
		this.bits3 |= tagBits.bits3;
	}

	public void Clear(Tag tag)
	{
		TagBits tagBits = TagBits.GetTagBits(tag);
		this.bits0 &= ~tagBits.bits0;
		this.bits1 &= ~tagBits.bits1;
		this.bits2 &= ~tagBits.bits2;
		this.bits3 &= ~tagBits.bits3;
	}

	public bool HasAll(TagBits tag_bits)
	{
		return (this.bits0 & tag_bits.bits0) == tag_bits.bits0 && (this.bits1 & tag_bits.bits1) == tag_bits.bits1 && (this.bits2 & tag_bits.bits2) == tag_bits.bits2 && (this.bits2 & tag_bits.bits3) == tag_bits.bits3;
	}

	public bool HasAny(TagBits tag_bits)
	{
		return ((this.bits0 & tag_bits.bits0) | (this.bits1 & tag_bits.bits1) | (this.bits2 & tag_bits.bits2) | (this.bits3 & tag_bits.bits3)) != 0UL;
	}

	public bool AreEqual(TagBits tag_bits)
	{
		return tag_bits.bits0 == this.bits0 && tag_bits.bits1 == this.bits1 && tag_bits.bits2 == this.bits2 && tag_bits.bits3 == this.bits3;
	}

	public static implicit operator TagBits(Tag tag)
	{
		return new TagBits(tag);
	}

	public static TagBits operator &(TagBits a, TagBits b)
	{
		return new TagBits
		{
			bits0 = (a.bits0 & b.bits0),
			bits1 = (a.bits1 & b.bits1),
			bits2 = (a.bits2 & b.bits2),
			bits3 = (a.bits3 & b.bits3)
		};
	}

	public static TagBits operator ~(TagBits tag_bits)
	{
		return new TagBits
		{
			bits0 = ~tag_bits.bits0,
			bits1 = ~tag_bits.bits1,
			bits2 = ~tag_bits.bits2,
			bits3 = ~tag_bits.bits3
		};
	}

	private static Dictionary<Tag, TagBits> tagTable = new Dictionary<Tag, TagBits>();

	private static Dictionary<int, Tag> inverseTagTable = new Dictionary<int, Tag>();

	private ulong bits0;

	private ulong bits1;

	private ulong bits2;

	private ulong bits3;
}
