using System;
using System.Collections.Generic;

public struct TagBits
{
	public TagBits(ref TagBits other)
	{
		this.bits0 = other.bits0;
		this.bits1 = other.bits1;
		this.bits2 = other.bits2;
		this.bits3 = other.bits3;
		this.bits4 = other.bits4;
		this.bits5 = other.bits5;
	}

	public TagBits(Tag tag)
	{
		this.bits0 = 0UL;
		this.bits1 = 0UL;
		this.bits2 = 0UL;
		this.bits3 = 0UL;
		this.bits4 = 0UL;
		this.bits5 = 0UL;
		this.SetTag(tag);
	}

	public TagBits(Tag[] tags)
	{
		this.bits0 = 0UL;
		this.bits1 = 0UL;
		this.bits2 = 0UL;
		this.bits3 = 0UL;
		this.bits4 = 0UL;
		this.bits5 = 0UL;
		if (tags == null)
		{
			return;
		}
		foreach (Tag tag in tags)
		{
			this.SetTag(tag);
		}
	}

	public List<Tag> GetTagsVerySlow()
	{
		List<Tag> list = new List<Tag>();
		this.GetTagsVerySlow(0, this.bits0, list);
		this.GetTagsVerySlow(1, this.bits1, list);
		this.GetTagsVerySlow(2, this.bits2, list);
		this.GetTagsVerySlow(3, this.bits3, list);
		this.GetTagsVerySlow(4, this.bits4, list);
		this.GetTagsVerySlow(5, this.bits5, list);
		return list;
	}

	private void GetTagsVerySlow(int bits_idx, ulong bits, List<Tag> tags)
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

	private static int ManifestFlagIndex(Tag tag)
	{
		int count;
		if (TagBits.tagTable.TryGetValue(tag, out count))
		{
			return count;
		}
		count = TagBits.tagTable.Count;
		TagBits.tagTable.Add(tag, count);
		TagBits.inverseTagTable.Add(tag);
		DebugUtil.Assert(TagBits.inverseTagTable.Count == count + 1);
		if (TagBits.tagTable.Count >= 384)
		{
			string text = "Out of tag bits:\n";
			int num = 0;
			foreach (KeyValuePair<Tag, int> keyValuePair in TagBits.tagTable)
			{
				text = text + keyValuePair.Key.ToString() + ", ";
				num++;
				if (num % 64 == 0)
				{
					text += "\n";
				}
			}
			Debug.LogError(text);
		}
		return count;
	}

	public void SetTag(Tag tag)
	{
		int num = TagBits.ManifestFlagIndex(tag);
		if (num < 64)
		{
			this.bits0 |= 1UL << num;
		}
		else if (num < 128)
		{
			this.bits1 |= 1UL << num;
		}
		else if (num < 192)
		{
			this.bits2 |= 1UL << num;
		}
		else if (num < 256)
		{
			this.bits3 |= 1UL << num;
		}
		else if (num < 320)
		{
			this.bits4 |= 1UL << num;
		}
		else if (num < 384)
		{
			this.bits5 |= 1UL << num;
		}
		else
		{
			Debug.LogError("Out of bits!");
		}
	}

	public void Clear(Tag tag)
	{
		int num = TagBits.ManifestFlagIndex(tag);
		if (num < 64)
		{
			this.bits0 &= ~(1UL << num);
		}
		else if (num < 128)
		{
			this.bits1 &= ~(1UL << num);
		}
		else if (num < 192)
		{
			this.bits2 &= ~(1UL << num);
		}
		else if (num < 256)
		{
			this.bits3 &= ~(1UL << num);
		}
		else if (num < 320)
		{
			this.bits4 &= ~(1UL << num);
		}
		else if (num < 384)
		{
			this.bits5 &= ~(1UL << num);
		}
		else
		{
			Debug.LogError("Out of bits!");
		}
	}

	public void ClearAll()
	{
		this.bits0 = 0UL;
		this.bits1 = 0UL;
		this.bits2 = 0UL;
		this.bits3 = 0UL;
		this.bits4 = 0UL;
		this.bits5 = 0UL;
	}

	public bool HasAll(ref TagBits tag_bits)
	{
		return (this.bits0 & tag_bits.bits0) == tag_bits.bits0 && (this.bits1 & tag_bits.bits1) == tag_bits.bits1 && (this.bits2 & tag_bits.bits2) == tag_bits.bits2 && (this.bits3 & tag_bits.bits3) == tag_bits.bits3 && (this.bits4 & tag_bits.bits4) == tag_bits.bits4 && (this.bits5 & tag_bits.bits5) == tag_bits.bits5;
	}

	public bool HasAny(ref TagBits tag_bits)
	{
		return ((this.bits0 & tag_bits.bits0) | (this.bits1 & tag_bits.bits1) | (this.bits2 & tag_bits.bits2) | (this.bits3 & tag_bits.bits3) | (this.bits4 & tag_bits.bits4) | (this.bits5 & tag_bits.bits5)) != 0UL;
	}

	public bool AreEqual(ref TagBits tag_bits)
	{
		return tag_bits.bits0 == this.bits0 && tag_bits.bits1 == this.bits1 && tag_bits.bits2 == this.bits2 && tag_bits.bits3 == this.bits3 && tag_bits.bits4 == this.bits4 && tag_bits.bits5 == this.bits5;
	}

	public void And(ref TagBits rhs)
	{
		this.bits0 &= rhs.bits0;
		this.bits1 &= rhs.bits1;
		this.bits2 &= rhs.bits2;
		this.bits3 &= rhs.bits3;
		this.bits4 &= rhs.bits4;
		this.bits5 &= rhs.bits5;
	}

	public void Or(ref TagBits rhs)
	{
		this.bits0 |= rhs.bits0;
		this.bits1 |= rhs.bits1;
		this.bits2 |= rhs.bits2;
		this.bits3 |= rhs.bits3;
		this.bits4 |= rhs.bits4;
		this.bits5 |= rhs.bits5;
	}

	public void Xor(ref TagBits rhs)
	{
		this.bits0 ^= rhs.bits0;
		this.bits1 ^= rhs.bits1;
		this.bits2 ^= rhs.bits2;
		this.bits3 ^= rhs.bits3;
		this.bits4 ^= rhs.bits4;
		this.bits5 ^= rhs.bits5;
	}

	public void Complement()
	{
		this.bits0 = ~this.bits0;
		this.bits1 = ~this.bits1;
		this.bits2 = ~this.bits2;
		this.bits3 = ~this.bits3;
		this.bits4 = ~this.bits4;
		this.bits5 = ~this.bits5;
	}

	public static TagBits MakeComplement(ref TagBits rhs)
	{
		TagBits tagBits = new TagBits(ref rhs);
		tagBits.Complement();
		return tagBits;
	}

	private static Dictionary<Tag, int> tagTable = new Dictionary<Tag, int>();

	private static List<Tag> inverseTagTable = new List<Tag>();

	private const int Capacity = 384;

	private ulong bits0;

	private ulong bits1;

	private ulong bits2;

	private ulong bits3;

	private ulong bits4;

	private ulong bits5;

	public static TagBits None = default(TagBits);
}
