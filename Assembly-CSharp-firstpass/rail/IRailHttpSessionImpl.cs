using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailHttpSessionImpl : RailObject, IRailHttpSession, IRailComponent
	{
		internal IRailHttpSessionImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailHttpSessionImpl()
		{
		}

		public virtual RailResult SetRequestMethod(RailHttpSessionMethod method)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailHttpSession_SetRequestMethod(this.swigCPtr_, (int)method);
		}

		public virtual RailResult SetParameters(List<RailKeyValue> parameters)
		{
			IntPtr intPtr = ((parameters == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailKeyValue__SWIG_0());
			if (parameters != null)
			{
				RailConverter.Csharp2Cpp(parameters, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailHttpSession_SetParameters(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailKeyValue(intPtr);
			}
			return railResult;
		}

		public virtual RailResult SetPostBodyContent(string body_content)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailHttpSession_SetPostBodyContent(this.swigCPtr_, body_content);
		}

		public virtual RailResult SetRequestTimeOut(uint timeout_secs)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailHttpSession_SetRequestTimeOut(this.swigCPtr_, timeout_secs);
		}

		public virtual RailResult SetRequestHeaders(List<string> headers)
		{
			IntPtr intPtr = ((headers == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			if (headers != null)
			{
				RailConverter.Csharp2Cpp(headers, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailHttpSession_SetRequestHeaders(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult AsyncSendRequest(string url, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailHttpSession_AsyncSendRequest(this.swigCPtr_, url, user_data);
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
