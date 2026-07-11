using System;

namespace FMOD.Studio
{
	public struct SOUND_INFO
	{
		public string name
		{
			get
			{
				string text;
				using (StringHelper.ThreadSafeEncoding freeHelper = StringHelper.GetFreeHelper())
				{
					text = (((this.mode & (MODE.OPENMEMORY | MODE.OPENMEMORY_POINT)) != MODE.DEFAULT) ? string.Empty : freeHelper.stringFromNative(this.name_or_data));
				}
				return text;
			}
		}

		public IntPtr name_or_data;

		public MODE mode;

		public CREATESOUNDEXINFO exinfo;

		public int subsoundindex;
	}
}
