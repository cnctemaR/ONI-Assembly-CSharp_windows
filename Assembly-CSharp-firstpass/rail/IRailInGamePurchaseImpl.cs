using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailInGamePurchaseImpl : RailObject, IRailInGamePurchase
	{
		internal IRailInGamePurchaseImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailInGamePurchaseImpl()
		{
		}

		public virtual RailResult AsyncRequestAllPurchasableProducts(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailInGamePurchase_AsyncRequestAllPurchasableProducts(this.swigCPtr_, user_data);
		}

		public virtual RailResult AsyncRequestAllProducts(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailInGamePurchase_AsyncRequestAllProducts(this.swigCPtr_, user_data);
		}

		public virtual RailResult GetProductInfo(uint product_id, RailPurchaseProductInfo product)
		{
			IntPtr intPtr = ((product == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailPurchaseProductInfo__SWIG_0());
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailInGamePurchase_GetProductInfo(this.swigCPtr_, product_id, intPtr);
			}
			finally
			{
				if (product != null)
				{
					RailConverter.Cpp2Csharp(intPtr, product);
				}
				RAIL_API_PINVOKE.delete_RailPurchaseProductInfo(intPtr);
			}
			return railResult;
		}

		public virtual RailResult AsyncPurchaseProducts(List<RailProductItem> cart_items, string user_data)
		{
			IntPtr intPtr = ((cart_items == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailProductItem__SWIG_0());
			if (cart_items != null)
			{
				RailConverter.Csharp2Cpp(cart_items, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailInGamePurchase_AsyncPurchaseProducts(this.swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailProductItem(intPtr);
			}
			return railResult;
		}

		public virtual RailResult AsyncFinishOrder(string order_id, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailInGamePurchase_AsyncFinishOrder(this.swigCPtr_, order_id, user_data);
		}

		public virtual RailResult AsyncPurchaseProductsToAssets(List<RailProductItem> cart_items, string user_data)
		{
			IntPtr intPtr = ((cart_items == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailProductItem__SWIG_0());
			if (cart_items != null)
			{
				RailConverter.Csharp2Cpp(cart_items, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailInGamePurchase_AsyncPurchaseProductsToAssets(this.swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailProductItem(intPtr);
			}
			return railResult;
		}
	}
}
