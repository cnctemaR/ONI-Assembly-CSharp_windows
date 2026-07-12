using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailStorageHelperImpl : RailObject, IRailStorageHelper
	{
		internal IRailStorageHelperImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailStorageHelperImpl()
		{
		}

		public virtual IRailFile OpenFile(string filename, out RailResult result)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailStorageHelper_OpenFile__SWIG_0(this.swigCPtr_, filename, out result);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailFileImpl(intPtr);
			}
			return null;
		}

		public virtual IRailFile OpenFile(string filename)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailStorageHelper_OpenFile__SWIG_1(this.swigCPtr_, filename);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailFileImpl(intPtr);
			}
			return null;
		}

		public virtual IRailFile CreateFile(string filename, out RailResult result)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailStorageHelper_CreateFile__SWIG_0(this.swigCPtr_, filename, out result);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailFileImpl(intPtr);
			}
			return null;
		}

		public virtual IRailFile CreateFile(string filename)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailStorageHelper_CreateFile__SWIG_1(this.swigCPtr_, filename);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailFileImpl(intPtr);
			}
			return null;
		}

		public virtual bool IsFileExist(string filename)
		{
			return RAIL_API_PINVOKE.IRailStorageHelper_IsFileExist(this.swigCPtr_, filename);
		}

		public virtual bool ListFiles(List<string> filelist)
		{
			IntPtr intPtr = ((filelist == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			bool flag;
			try
			{
				flag = RAIL_API_PINVOKE.IRailStorageHelper_ListFiles(this.swigCPtr_, intPtr);
			}
			finally
			{
				if (filelist != null)
				{
					RailConverter.Cpp2Csharp(intPtr, filelist);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
			return flag;
		}

		public virtual RailResult RemoveFile(string filename)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailStorageHelper_RemoveFile(this.swigCPtr_, filename);
		}

		public virtual bool IsFileSyncedToCloud(string filename)
		{
			return RAIL_API_PINVOKE.IRailStorageHelper_IsFileSyncedToCloud(this.swigCPtr_, filename);
		}

		public virtual RailResult GetFileTimestamp(string filename, out ulong time_stamp)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailStorageHelper_GetFileTimestamp(this.swigCPtr_, filename, out time_stamp);
		}

		public virtual uint GetFileCount()
		{
			return RAIL_API_PINVOKE.IRailStorageHelper_GetFileCount(this.swigCPtr_);
		}

		public virtual RailResult GetFileNameAndSize(uint file_index, out string filename, out ulong file_size)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailStorageHelper_GetFileNameAndSize(this.swigCPtr_, file_index, intPtr, out file_size);
			}
			finally
			{
				filename = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult AsyncQueryQuota()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailStorageHelper_AsyncQueryQuota(this.swigCPtr_);
		}

		public virtual RailResult SetSyncFileOption(string filename, RailSyncFileOption option)
		{
			IntPtr intPtr = ((option == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailSyncFileOption__SWIG_0());
			if (option != null)
			{
				RailConverter.Csharp2Cpp(option, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailStorageHelper_SetSyncFileOption(this.swigCPtr_, filename, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailSyncFileOption(intPtr);
			}
			return railResult;
		}

		public virtual bool IsCloudStorageEnabledForApp()
		{
			return RAIL_API_PINVOKE.IRailStorageHelper_IsCloudStorageEnabledForApp(this.swigCPtr_);
		}

		public virtual bool IsCloudStorageEnabledForPlayer()
		{
			return RAIL_API_PINVOKE.IRailStorageHelper_IsCloudStorageEnabledForPlayer(this.swigCPtr_);
		}

		public virtual RailResult AsyncPublishFileToUserSpace(RailPublishFileToUserSpaceOption option, string user_data)
		{
			IntPtr intPtr = ((option == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailPublishFileToUserSpaceOption__SWIG_0());
			if (option != null)
			{
				RailConverter.Csharp2Cpp(option, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailStorageHelper_AsyncPublishFileToUserSpace(this.swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailPublishFileToUserSpaceOption(intPtr);
			}
			return railResult;
		}

		public virtual IRailStreamFile OpenStreamFile(string filename, RailStreamFileOption option, out RailResult result)
		{
			IntPtr intPtr = ((option == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailStreamFileOption__SWIG_0());
			if (option != null)
			{
				RailConverter.Csharp2Cpp(option, intPtr);
			}
			IRailStreamFile railStreamFile;
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailStorageHelper_OpenStreamFile__SWIG_0(this.swigCPtr_, filename, intPtr, out result);
				railStreamFile = ((intPtr2 == IntPtr.Zero) ? null : new IRailStreamFileImpl(intPtr2));
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailStreamFileOption(intPtr);
			}
			return railStreamFile;
		}

		public virtual IRailStreamFile OpenStreamFile(string filename, RailStreamFileOption option)
		{
			IntPtr intPtr = ((option == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailStreamFileOption__SWIG_0());
			if (option != null)
			{
				RailConverter.Csharp2Cpp(option, intPtr);
			}
			IRailStreamFile railStreamFile;
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailStorageHelper_OpenStreamFile__SWIG_1(this.swigCPtr_, filename, intPtr);
				railStreamFile = ((intPtr2 == IntPtr.Zero) ? null : new IRailStreamFileImpl(intPtr2));
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailStreamFileOption(intPtr);
			}
			return railStreamFile;
		}

		public virtual RailResult AsyncListStreamFiles(string contents, RailListStreamFileOption option, string user_data)
		{
			IntPtr intPtr = ((option == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailListStreamFileOption__SWIG_0());
			if (option != null)
			{
				RailConverter.Csharp2Cpp(option, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailStorageHelper_AsyncListStreamFiles(this.swigCPtr_, contents, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailListStreamFileOption(intPtr);
			}
			return railResult;
		}

		public virtual RailResult AsyncRenameStreamFile(string old_filename, string new_filename, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailStorageHelper_AsyncRenameStreamFile(this.swigCPtr_, old_filename, new_filename, user_data);
		}

		public virtual RailResult AsyncDeleteStreamFile(string filename, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailStorageHelper_AsyncDeleteStreamFile(this.swigCPtr_, filename, user_data);
		}

		public virtual uint GetRailFileEnabledOS(string filename)
		{
			return RAIL_API_PINVOKE.IRailStorageHelper_GetRailFileEnabledOS(this.swigCPtr_, filename);
		}

		public virtual RailResult SetRailFileEnabledOS(string filename, EnumRailStorageFileEnabledOS sync_os)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailStorageHelper_SetRailFileEnabledOS(this.swigCPtr_, filename, (int)sync_os);
		}
	}
}
