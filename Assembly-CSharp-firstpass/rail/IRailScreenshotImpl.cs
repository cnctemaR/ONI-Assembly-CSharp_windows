using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailScreenshotImpl : RailObject, IRailScreenshot, IRailComponent
	{
		internal IRailScreenshotImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailScreenshotImpl()
		{
		}

		public virtual bool SetLocation(string location)
		{
			return RAIL_API_PINVOKE.IRailScreenshot_SetLocation(this.swigCPtr_, location);
		}

		public virtual bool SetUsers(List<RailID> users)
		{
			IntPtr intPtr = ((users == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailID__SWIG_0());
			if (users != null)
			{
				RailConverter.Csharp2Cpp(users, intPtr);
			}
			bool flag;
			try
			{
				flag = RAIL_API_PINVOKE.IRailScreenshot_SetUsers(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailID(intPtr);
			}
			return flag;
		}

		public virtual bool AssociatePublishedFiles(List<SpaceWorkID> work_files)
		{
			IntPtr intPtr = ((work_files == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArraySpaceWorkID__SWIG_0());
			if (work_files != null)
			{
				RailConverter.Csharp2Cpp(work_files, intPtr);
			}
			bool flag;
			try
			{
				flag = RAIL_API_PINVOKE.IRailScreenshot_AssociatePublishedFiles(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArraySpaceWorkID(intPtr);
			}
			return flag;
		}

		public virtual RailResult AsyncPublishScreenshot(string work_name, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailScreenshot_AsyncPublishScreenshot(this.swigCPtr_, work_name, user_data);
		}

		public virtual ulong GetComponentVersion()
		{
			return RAIL_API_PINVOKE.IRailComponent_GetComponentVersion(this.swigCPtr_);
		}

		public virtual void Release()
		{
			RAIL_API_PINVOKE.IRailComponent_Release(this.swigCPtr_);
		}
	}
}
