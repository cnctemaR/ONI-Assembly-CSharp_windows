using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailSmallObjectServiceHelperImpl : RailObject, IRailSmallObjectServiceHelper
	{
		internal IRailSmallObjectServiceHelperImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailSmallObjectServiceHelperImpl()
		{
		}

		public virtual RailResult AsyncDownloadObjects(List<uint> indexes, string user_data)
		{
			IntPtr intPtr = ((indexes == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayuint32_t__SWIG_0());
			if (indexes != null)
			{
				RailConverter.Csharp2Cpp(indexes, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSmallObjectServiceHelper_AsyncDownloadObjects(this.swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayuint32_t(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetObjectContent(uint index, out string content)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailSmallObjectServiceHelper_GetObjectContent(this.swigCPtr_, index, intPtr);
			}
			finally
			{
				content = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult AsyncQueryObjectState(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSmallObjectServiceHelper_AsyncQueryObjectState(this.swigCPtr_, user_data);
		}
	}
}
