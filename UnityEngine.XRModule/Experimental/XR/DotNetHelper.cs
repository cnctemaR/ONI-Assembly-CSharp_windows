using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.XR
{
	internal static class DotNetHelper
	{
		public static bool TryCopyFixedArrayToList<T>(T[] fixedArrayIn, List<T> listOut)
		{
			bool flag;
			if (fixedArrayIn == null)
			{
				flag = false;
			}
			else
			{
				int num = fixedArrayIn.Length;
				listOut.Clear();
				if (listOut.Capacity < num)
				{
					listOut.Capacity = num;
				}
				listOut.AddRange(fixedArrayIn);
				flag = true;
			}
			return flag;
		}
	}
}
