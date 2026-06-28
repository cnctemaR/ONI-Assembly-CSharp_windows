using System;
using System.Collections.Generic;

namespace I18N.Common
{
	public sealed class Strings
	{
		public static string GetString(string tag)
		{
			if (tag != null)
			{
				if (Strings.<>f__switch$map0 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(6);
					dictionary.Add("ArgRange_Array", 0);
					dictionary.Add("Arg_InsufficientSpace", 1);
					dictionary.Add("ArgRange_NonNegative", 2);
					dictionary.Add("NotSupp_MissingCodeTable", 3);
					dictionary.Add("ArgRange_StringIndex", 4);
					dictionary.Add("ArgRange_StringRange", 5);
					Strings.<>f__switch$map0 = dictionary;
				}
				int num;
				if (Strings.<>f__switch$map0.TryGetValue(tag, ref num))
				{
					switch (num)
					{
					case 0:
						return "Argument index is out of array range.";
					case 1:
						return "Insufficient space in the argument array.";
					case 2:
						return "Non-negative value is expected.";
					case 3:
						return "This encoding is not supported. Code table is missing.";
					case 4:
						return "String index is out of range.";
					case 5:
						return "String length is out of range.";
					}
				}
			}
			throw new ArgumentException(string.Format("Unexpected error tag name:  {0}", tag));
		}
	}
}
