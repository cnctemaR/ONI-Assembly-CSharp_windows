using System;

namespace rail
{
	public class IRailInGameStorePurchaseHelperImpl : RailObject, IRailInGameStorePurchaseHelper
	{
		internal IRailInGameStorePurchaseHelperImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailInGameStorePurchaseHelperImpl()
		{
		}

		public virtual RailResult AsyncShowPaymentWindow(string order_id, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailInGameStorePurchaseHelper_AsyncShowPaymentWindow(this.swigCPtr_, order_id, user_data);
		}
	}
}
