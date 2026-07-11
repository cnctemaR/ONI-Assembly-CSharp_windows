using System;

namespace FMOD
{
	public struct CREATESOUNDEXINFO
	{
		public int cbsize;

		public uint length;

		public uint fileoffset;

		public int numchannels;

		public int defaultfrequency;

		public SOUND_FORMAT format;

		public uint decodebuffersize;

		public int initialsubsound;

		public int numsubsounds;

		public IntPtr inclusionlist;

		public int inclusionlistnum;

		public SOUND_PCMREAD_CALLBACK pcmreadcallback;

		public SOUND_PCMSETPOS_CALLBACK pcmsetposcallback;

		public SOUND_NONBLOCK_CALLBACK nonblockcallback;

		public IntPtr dlsname;

		public IntPtr encryptionkey;

		public int maxpolyphony;

		public IntPtr userdata;

		public SOUND_TYPE suggestedsoundtype;

		public FILE_OPEN_CALLBACK fileuseropen;

		public FILE_CLOSE_CALLBACK fileuserclose;

		public FILE_READ_CALLBACK fileuserread;

		public FILE_SEEK_CALLBACK fileuserseek;

		public FILE_ASYNCREAD_CALLBACK fileuserasyncread;

		public FILE_ASYNCCANCEL_CALLBACK fileuserasynccancel;

		public IntPtr fileuserdata;

		public int filebuffersize;

		public CHANNELORDER channelorder;

		public IntPtr initialsoundgroup;

		public uint initialseekposition;

		public TIMEUNIT initialseekpostype;

		public int ignoresetfilesystem;

		public uint audioqueuepolicy;

		public uint minmidigranularity;

		public int nonblockthreadid;

		public IntPtr fsbguid;
	}
}
