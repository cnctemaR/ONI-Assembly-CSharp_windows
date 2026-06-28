using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct CREATESOUNDEXINFO_INTERNAL
	{
		public static CREATESOUNDEXINFO_INTERNAL CreateFromExternal(ref CREATESOUNDEXINFO exinfoExt)
		{
			return new CREATESOUNDEXINFO_INTERNAL
			{
				cbsize = Marshal.SizeOf(typeof(CREATESOUNDEXINFO_INTERNAL)),
				fileoffset = exinfoExt.fileoffset,
				numchannels = exinfoExt.numchannels,
				defaultfrequency = exinfoExt.defaultfrequency,
				format = exinfoExt.format,
				decodebuffersize = exinfoExt.decodebuffersize,
				initialsubsound = exinfoExt.initialsubsound,
				numsubsounds = exinfoExt.numsubsounds,
				inclusionlist = exinfoExt.inclusionlist,
				inclusionlistnum = exinfoExt.inclusionlistnum,
				pcmreadcallback = ((exinfoExt.pcmreadcallback == null) ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(exinfoExt.pcmreadcallback)),
				pcmsetposcallback = ((exinfoExt.pcmsetposcallback == null) ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(exinfoExt.pcmsetposcallback)),
				nonblockcallback = ((exinfoExt.nonblockcallback == null) ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(exinfoExt.nonblockcallback)),
				dlsname = exinfoExt.dlsname,
				encryptionkey = exinfoExt.encryptionkey,
				maxpolyphony = exinfoExt.maxpolyphony,
				userdata = exinfoExt.userdata,
				suggestedsoundtype = exinfoExt.suggestedsoundtype,
				fileuseropen = ((exinfoExt.fileuseropen == null) ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(exinfoExt.fileuseropen)),
				fileuserclose = ((exinfoExt.fileuserclose == null) ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(exinfoExt.fileuserclose)),
				fileuserread = ((exinfoExt.fileuserread == null) ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(exinfoExt.fileuserread)),
				fileuserseek = ((exinfoExt.fileuserseek == null) ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(exinfoExt.fileuserseek)),
				fileuserasyncread = ((exinfoExt.fileuserasyncread == null) ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(exinfoExt.fileuserasyncread)),
				fileuserasynccancel = ((exinfoExt.fileuserasynccancel == null) ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(exinfoExt.fileuserasynccancel)),
				fileuserdata = exinfoExt.fileuserdata,
				filebuffersize = exinfoExt.filebuffersize,
				channelorder = exinfoExt.channelorder,
				channelmask = exinfoExt.channelmask,
				initialsoundgroup = exinfoExt.initialsoundgroup,
				initialseekposition = exinfoExt.initialseekposition,
				initialseekpostype = exinfoExt.initialseekpostype,
				ignoresetfilesystem = exinfoExt.ignoresetfilesystem,
				audioqueuepolicy = exinfoExt.audioqueuepolicy,
				minmidigranularity = exinfoExt.minmidigranularity,
				nonblockthreadid = exinfoExt.nonblockthreadid,
				fsbguid = exinfoExt.fsbguid
			};
		}

		public static CREATESOUNDEXINFO CreateFromInternal(ref CREATESOUNDEXINFO_INTERNAL exinfoInt)
		{
			return new CREATESOUNDEXINFO
			{
				cbsize = Marshal.SizeOf(typeof(CREATESOUNDEXINFO_INTERNAL)),
				fileoffset = exinfoInt.fileoffset,
				numchannels = exinfoInt.numchannels,
				defaultfrequency = exinfoInt.defaultfrequency,
				format = exinfoInt.format,
				decodebuffersize = exinfoInt.decodebuffersize,
				initialsubsound = exinfoInt.initialsubsound,
				numsubsounds = exinfoInt.numsubsounds,
				inclusionlist = exinfoInt.inclusionlist,
				inclusionlistnum = exinfoInt.inclusionlistnum,
				pcmreadcallback = ((!(exinfoInt.pcmreadcallback != IntPtr.Zero)) ? null : (Marshal.GetDelegateForFunctionPointer(exinfoInt.pcmreadcallback, typeof(SOUND_PCMREADCALLBACK)) as SOUND_PCMREADCALLBACK)),
				pcmsetposcallback = ((!(exinfoInt.pcmsetposcallback != IntPtr.Zero)) ? null : (Marshal.GetDelegateForFunctionPointer(exinfoInt.pcmsetposcallback, typeof(SOUND_PCMSETPOSCALLBACK)) as SOUND_PCMSETPOSCALLBACK)),
				nonblockcallback = ((!(exinfoInt.nonblockcallback != IntPtr.Zero)) ? null : (Marshal.GetDelegateForFunctionPointer(exinfoInt.nonblockcallback, typeof(SOUND_NONBLOCKCALLBACK)) as SOUND_NONBLOCKCALLBACK)),
				dlsname = exinfoInt.dlsname,
				encryptionkey = exinfoInt.encryptionkey,
				maxpolyphony = exinfoInt.maxpolyphony,
				userdata = exinfoInt.userdata,
				suggestedsoundtype = exinfoInt.suggestedsoundtype,
				fileuseropen = ((!(exinfoInt.fileuseropen != IntPtr.Zero)) ? null : (Marshal.GetDelegateForFunctionPointer(exinfoInt.fileuseropen, typeof(FILE_OPENCALLBACK)) as FILE_OPENCALLBACK)),
				fileuserclose = ((!(exinfoInt.fileuserclose != IntPtr.Zero)) ? null : (Marshal.GetDelegateForFunctionPointer(exinfoInt.fileuserclose, typeof(FILE_CLOSECALLBACK)) as FILE_CLOSECALLBACK)),
				fileuserread = ((!(exinfoInt.fileuserread != IntPtr.Zero)) ? null : (Marshal.GetDelegateForFunctionPointer(exinfoInt.fileuserread, typeof(FILE_READCALLBACK)) as FILE_READCALLBACK)),
				fileuserseek = ((!(exinfoInt.fileuserseek != IntPtr.Zero)) ? null : (Marshal.GetDelegateForFunctionPointer(exinfoInt.fileuserseek, typeof(FILE_SEEKCALLBACK)) as FILE_SEEKCALLBACK)),
				fileuserasyncread = ((!(exinfoInt.fileuserasyncread != IntPtr.Zero)) ? null : (Marshal.GetDelegateForFunctionPointer(exinfoInt.fileuserasyncread, typeof(FILE_ASYNCREADCALLBACK)) as FILE_ASYNCREADCALLBACK)),
				fileuserasynccancel = ((!(exinfoInt.fileuserasynccancel != IntPtr.Zero)) ? null : (Marshal.GetDelegateForFunctionPointer(exinfoInt.fileuserasynccancel, typeof(FILE_ASYNCCANCELCALLBACK)) as FILE_ASYNCCANCELCALLBACK)),
				fileuserdata = exinfoInt.fileuserdata,
				filebuffersize = exinfoInt.filebuffersize,
				channelorder = exinfoInt.channelorder,
				channelmask = exinfoInt.channelmask,
				initialsoundgroup = exinfoInt.initialsoundgroup,
				initialseekposition = exinfoInt.initialseekposition,
				initialseekpostype = exinfoInt.initialseekpostype,
				ignoresetfilesystem = exinfoInt.ignoresetfilesystem,
				audioqueuepolicy = exinfoInt.audioqueuepolicy,
				minmidigranularity = exinfoInt.minmidigranularity,
				nonblockthreadid = exinfoInt.nonblockthreadid,
				fsbguid = exinfoInt.fsbguid
			};
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

		public IntPtr pcmreadcallback;

		public IntPtr pcmsetposcallback;

		public IntPtr nonblockcallback;

		public IntPtr dlsname;

		public IntPtr encryptionkey;

		public int maxpolyphony;

		public IntPtr userdata;

		public SOUND_TYPE suggestedsoundtype;

		public IntPtr fileuseropen;

		public IntPtr fileuserclose;

		public IntPtr fileuserread;

		public IntPtr fileuserseek;

		public IntPtr fileuserasyncread;

		public IntPtr fileuserasynccancel;

		public IntPtr fileuserdata;

		public int filebuffersize;

		public CHANNELORDER channelorder;

		public CHANNELMASK channelmask;

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
