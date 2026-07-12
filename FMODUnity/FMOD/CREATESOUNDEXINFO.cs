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
					return (SOUND_PCMREAD_CALLBACK)Marshal.GetDelegateForFunctionPointer(this.pcmreadcallback_internal, typeof(SOUND_PCMREAD_CALLBACK));
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
					return (SOUND_PCMSETPOS_CALLBACK)Marshal.GetDelegateForFunctionPointer(this.pcmsetposcallback_internal, typeof(SOUND_PCMSETPOS_CALLBACK));
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
					return (SOUND_NONBLOCK_CALLBACK)Marshal.GetDelegateForFunctionPointer(this.nonblockcallback_internal, typeof(SOUND_NONBLOCK_CALLBACK));
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
					return (FILE_OPEN_CALLBACK)Marshal.GetDelegateForFunctionPointer(this.fileuseropen_internal, typeof(FILE_OPEN_CALLBACK));
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
					return (FILE_CLOSE_CALLBACK)Marshal.GetDelegateForFunctionPointer(this.fileuserclose_internal, typeof(FILE_CLOSE_CALLBACK));
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
					return (FILE_READ_CALLBACK)Marshal.GetDelegateForFunctionPointer(this.fileuserread_internal, typeof(FILE_READ_CALLBACK));
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
					return (FILE_SEEK_CALLBACK)Marshal.GetDelegateForFunctionPointer(this.fileuserseek_internal, typeof(FILE_SEEK_CALLBACK));
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
					return (FILE_ASYNCREAD_CALLBACK)Marshal.GetDelegateForFunctionPointer(this.fileuserasyncread_internal, typeof(FILE_ASYNCREAD_CALLBACK));
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
					return (FILE_ASYNCCANCEL_CALLBACK)Marshal.GetDelegateForFunctionPointer(this.fileuserasynccancel_internal, typeof(FILE_ASYNCCANCEL_CALLBACK));
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
