using System;

namespace FMOD
{
	public struct StringWrapper
	{
		public static implicit operator string(StringWrapper fstring)
		{
			string text;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				text = freeHelper.stringFromNative(fstring.nativeUtf8Ptr);
			}
			return text;
		}

		private IntPtr nativeUtf8Ptr;
	}
}
