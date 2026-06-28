using System;
using System.Collections.Generic;

public static class Hash
{
	public static int SDBMLower(string s)
	{
		int num;
		if (s == null)
		{
			num = 0;
		}
		else
		{
			num = Hash.SDBM(s.ToLower());
		}
		return num;
	}

	public static int SDBM(string s)
	{
		int num;
		if (s == null)
		{
			num = 0;
		}
		else
		{
			uint num2 = 0U;
			foreach (char c in s)
			{
				num2 = (uint)c + (num2 << 6) + (num2 << 16) - num2;
			}
			num = (int)num2;
		}
		return num;
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
