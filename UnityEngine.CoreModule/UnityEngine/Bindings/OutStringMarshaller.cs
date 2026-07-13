using System;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	internal ref struct OutStringMarshaller
	{
		public unsafe static string GetStringAndDispose(ManagedSpanWrapper managedSpan)
		{
			bool flag = managedSpan.length == 0;
			string text;
			if (flag)
			{
				text = ((managedSpan.begin == null) ? null : string.Empty);
			}
			else
			{
				string text2 = new string((char*)managedSpan.begin, 0, managedSpan.length);
				BindingsAllocator.Free(managedSpan.begin);
				text = text2;
			}
			return text;
		}

		public static void UpdateStringAndDispose(ManagedSpanWrapper inSpanWrapper, ManagedSpanWrapper outSpanWrapper, ref string outString)
		{
			bool flag = inSpanWrapper.begin != outSpanWrapper.begin;
			if (flag)
			{
				outString = OutStringMarshaller.GetStringAndDispose(outSpanWrapper);
			}
		}
	}
}
