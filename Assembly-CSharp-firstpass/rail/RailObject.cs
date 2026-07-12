using System;

namespace rail
{
	public class RailObject
	{
		internal RailObject()
		{
		}

		internal static IntPtr getCPtr(RailObject obj)
		{
			if (obj != null)
			{
				return obj.swigCPtr_;
			}
			return IntPtr.Zero;
		}

		~RailObject()
		{
		}

		protected IntPtr swigCPtr_ = IntPtr.Zero;
	}
}
