using System;

namespace rail
{
	public enum PurchaseProductOrderState
	{
		kPurchaseProductOrderStateInvalid,
		kPurchaseProductOrderStateCreateOrderOk = 100,
		kPurchaseProductOrderStatePayOk = 200,
		kPurchaseProductOrderStateDeliverOk = 300
	}
}
