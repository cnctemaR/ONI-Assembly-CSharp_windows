using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
	public struct Util
	{
		public static RESULT parseID(string idString, out GUID id)
		{
			RESULT result;
			using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
			{
				result = Util.FMOD_Studio_ParseID(freeHelper.byteFromStringUTF8(idString), out id);
			}
			return result;
		}

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_ParseID(byte[] idString, out GUID id);
	}
}
