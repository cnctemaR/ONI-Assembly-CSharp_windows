using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	internal static class StringMarshaller
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static bool TryMarshalEmptyOrNullString(string s, ref ManagedSpanWrapper managedSpanWrapper)
		{
			bool flag = s == null;
			bool flag2;
			if (flag)
			{
				managedSpanWrapper = default(ManagedSpanWrapper);
				flag2 = true;
			}
			else
			{
				bool flag3 = s.Length == 0;
				if (flag3)
				{
					managedSpanWrapper = new ManagedSpanWrapper((void*)((UIntPtr)1UL), 0);
					flag2 = true;
				}
				else
				{
					flag2 = false;
				}
			}
			return flag2;
		}
	}
}
