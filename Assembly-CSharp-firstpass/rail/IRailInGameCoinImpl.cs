using System;

namespace rail
{
	public class IRailInGameCoinImpl : RailObject, IRailInGameCoin
	{
		internal IRailInGameCoinImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailInGameCoinImpl()
		{
		}

		public virtual RailResult AsyncRequestCoinInfo(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailInGameCoin_AsyncRequestCoinInfo(this.swigCPtr_, user_data);
		}

		public virtual RailResult AsyncPurchaseCoins(RailCoins purchase_info, string user_data)
		{
			IntPtr intPtr = ((purchase_info == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailCoins__SWIG_0());
			if (purchase_info != null)
			{
				RailConverter.Csharp2Cpp(purchase_info, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailInGameCoin_AsyncPurchaseCoins(this.swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailCoins(intPtr);
			}
			return railResult;
		}
	}
}
