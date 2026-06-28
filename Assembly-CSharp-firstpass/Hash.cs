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
		return Hash.SDBM(s.ToLower());
	}

	public static int SDBM(string s)
	{
		if (s == null)
		{
			return 0;
		}
		uint num = 0U;
		foreach (char c in s)
		{
			num = (uint)c + (num << 6) + (num << 16) - num;
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
