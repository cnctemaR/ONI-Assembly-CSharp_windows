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
					text = (((this.mode & (MODE.OPENMEMORY | MODE.OPENMEMORY_POINT)) == MODE.DEFAULT) ? freeHelper.stringFromNative(this.name_or_data) : string.Empty);
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
