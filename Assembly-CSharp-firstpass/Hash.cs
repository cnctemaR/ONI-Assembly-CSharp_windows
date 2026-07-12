using System;
using System.Collections.Generic;

public static class Hash
{
	public static int SDBMLower(string s)
	{
		if (s == null)
		{
			return 0;
		}
		uint num = 0U;
		for (int i = 0; i < s.Length; i++)
		{
			num = (uint)char.ToLowerInvariant(s[i]) + (num << 6) + (num << 16) - num;
		}
		return (int)num;
	}

	public static int[] SDBMLower(IList<string> strings)
	{
		int[] array = new int[strings.Count];
		for (int i = 0; i < strings.Count; i++)
		{
			array[i] = Hash.SDBMLower(strings[i]);
		}
		return array;
	}
}
