using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct CREATESOUNDEXINFO
	{
		public SOUND_PCMREAD_CALLBACK pcmreadcallback
		{
			get
			{
				if (!(this.pcmreadcallback_internal == IntPtr.Zero))
				{
					return Marshal.GetDelegateForFunctionPointer<SOUND_PCMREAD_CALLBACK>(this.pcmreadcallback_internal);
				}
				return null;
			}
			set
			{
				this.pcmreadcallback_internal = ((value == null) ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate<SOUND_PCMREAD_CALLBACK>(value));
			}
		}

		public SOUND_PCMSETPOS_CALLBACK pcmsetposcallback
		{
			get
			{
				if (!(this.pcmsetposcallback_internal == IntPtr.Zero))
				{
					return Marshal.GetDelegateForFunctionPointer<SOUND_PCMSETPOS_CALLBACK>(this.pcmsetposcallback_internal);
				}
				return null;
			}
			set
			{
				this.pcmsetposcallback_internal = ((value == null) ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate<SOUND_PCMSETPOS_CALLBACK>(value));
			}
		}

		public SOUND_NONBLOCK_CALLBACK nonblockcallback
		{
			get
			{
				if (!(this.nonblockcallback_internal == IntPtr.Zero))
				{
					return Marshal.GetDelegateForFunctionPointer<SOUND_NONBLOCK_CALLBACK>(this.nonblockcallback_internal);
				}
				return null;
			}
			set
			{
				this.nonblockcallback_internal = ((value == null) ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate<SOUND_NONBLOCK_CALLBACK>(value));
			}
		}

		public FILE_OPEN_CALLBACK fileuseropen
		{
			get
			{
				if (!(this.fileuseropen_internal == IntPtr.Zero))
				{
					return Marshal.GetDelegateForFunctionPointer<FILE_OPEN_CALLBACK>(this.fileuseropen_internal);
				}
				return null;
			}
			set
			{
				this.fileuseropen_internal = ((value == null) ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate<FILE_OPEN_CALLBACK>(value));
			}
		}

		public FILE_CLOSE_CALLBACK fileuserclose
		{
			get
			{
				if (!(this.fileuserclose_internal == IntPtr.Zero))
				{
					return Marshal.GetDelegateForFunctionPointer<FILE_CLOSE_CALLBACK>(this.fileuserclose_internal);
				}
				return null;
			}
			set
			{
				this.fileuserclose_internal = ((value == null) ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate<FILE_CLOSE_CALLBACK>(value));
			}
		}

		public FILE_READ_CALLBACK fileuserread
		{
			get
			{
				if (!(this.fileuserread_internal == IntPtr.Zero))
				{
					return Marshal.GetDelegateForFunctionPointer<FILE_READ_CALLBACK>(this.fileuserread_internal);
				}
				return null;
			}
			set
			{
				this.fileuserread_internal = ((value == null) ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate<FILE_READ_CALLBACK>(value));
			}
		}

		public FILE_SEEK_CALLBACK fileuserseek
		{
			get
			{
				if (!(this.fileuserseek_internal == IntPtr.Zero))
				{
					return Marshal.GetDelegateForFunctionPointer<FILE_SEEK_CALLBACK>(this.fileuserseek_internal);
				}
				return null;
			}
			set
			{
				this.fileuserseek_internal = ((value == null) ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate<FILE_SEEK_CALLBACK>(value));
			}
		}

		public FILE_ASYNCREAD_CALLBACK fileuserasyncread
		{
			get
			{
				if (!(this.fileuserasyncread_internal == IntPtr.Zero))
				{
					return Marshal.GetDelegateForFunctionPointer<FILE_ASYNCREAD_CALLBACK>(this.fileuserasyncread_internal);
				}
				return null;
			}
			set
			{
				this.fileuserasyncread_internal = ((value == null) ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate<FILE_ASYNCREAD_CALLBACK>(value));
			}
		}

		public FILE_ASYNCCANCEL_CALLBACK fileuserasynccancel
		{
			get
			{
				if (!(this.fileuserasynccancel_internal == IntPtr.Zero))
				{
					return Marshal.GetDelegateForFunctionPointer<FILE_ASYNCCANCEL_CALLBACK>(this.fileuserasynccancel_internal);
				}
				return null;
			}
			set
			{
				this.fileuserasynccancel_internal = ((value == null) ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate<FILE_ASYNCCANCEL_CALLBACK>(value));
			}
		}

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

		public IntPtr pcmreadcallback_internal;

		public IntPtr pcmsetposcallback_internal;

		public IntPtr nonblockcallback_internal;

		public IntPtr dlsname;

		public IntPtr encryptionkey;

		public int maxpolyphony;

		public IntPtr userdata;

		public SOUND_TYPE suggestedsoundtype;

		public IntPtr fileuseropen_internal;

		public IntPtr fileuserclose_internal;

		public IntPtr fileuserread_internal;

		public IntPtr fileuserseek_internal;

		public IntPtr fileuserasyncread_internal;

		public IntPtr fileuserasynccancel_internal;

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
