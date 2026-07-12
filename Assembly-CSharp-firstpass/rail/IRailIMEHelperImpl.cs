using System;

namespace rail
{
	public class IRailIMEHelperImpl : RailObject, IRailIMEHelper
	{
		internal IRailIMEHelperImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailIMEHelperImpl()
		{
		}

		public virtual RailResult EnableIMEHelperTextInputWindow(bool enable, RailTextInputImeWindowOption option)
		{
			IntPtr intPtr = ((option == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailTextInputImeWindowOption__SWIG_0());
			if (option != null)
			{
				RailConverter.Csharp2Cpp(option, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailIMEHelper_EnableIMEHelperTextInputWindow(this.swigCPtr_, enable, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailTextInputImeWindowOption(intPtr);
			}
			return railResult;
		}

		public virtual RailResult UpdateIMEHelperTextInputWindowPosition(RailWindowPosition position)
		{
			IntPtr intPtr = ((position == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailWindowPosition__SWIG_0());
			if (position != null)
			{
				RailConverter.Csharp2Cpp(position, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailIMEHelper_UpdateIMEHelperTextInputWindowPosition(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailWindowPosition(intPtr);
			}
			return railResult;
		}
	}
}
