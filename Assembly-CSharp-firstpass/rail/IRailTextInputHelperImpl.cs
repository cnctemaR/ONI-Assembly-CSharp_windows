using System;

namespace rail
{
	public class IRailTextInputHelperImpl : RailObject, IRailTextInputHelper
	{
		internal IRailTextInputHelperImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailTextInputHelperImpl()
		{
		}

		public virtual RailResult ShowTextInputWindow(RailTextInputWindowOption options)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailTextInputWindowOption__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailTextInputHelper_ShowTextInputWindow(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailTextInputWindowOption(intPtr);
			}
			return railResult;
		}

		public virtual void GetTextInputContent(out string content)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				RAIL_API_PINVOKE.IRailTextInputHelper_GetTextInputContent(this.swigCPtr_, intPtr);
			}
			finally
			{
				content = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual RailResult HideTextInputWindow()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailTextInputHelper_HideTextInputWindow(this.swigCPtr_);
		}
	}
}
