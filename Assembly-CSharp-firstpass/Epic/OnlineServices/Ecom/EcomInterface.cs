using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Ecom
{
	public sealed class EcomInterface : Handle
	{
		public EcomInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public void QueryOwnership(QueryOwnershipOptions options, object clientData, OnQueryOwnershipCallback completionDelegate)
		{
			QueryOwnershipOptionsInternal queryOwnershipOptionsInternal = Helper.CopyProperties<QueryOwnershipOptionsInternal>(options);
			OnQueryOwnershipCallbackInternal onQueryOwnershipCallbackInternal = new OnQueryOwnershipCallbackInternal(EcomInterface.OnQueryOwnership);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onQueryOwnershipCallbackInternal, Array.Empty<Delegate>());
			EcomInterface.EOS_Ecom_QueryOwnership(base.InnerHandle, ref queryOwnershipOptionsInternal, zero, onQueryOwnershipCallbackInternal);
			Helper.TryMarshalDispose<QueryOwnershipOptionsInternal>(ref queryOwnershipOptionsInternal);
		}

		public void QueryOwnershipToken(QueryOwnershipTokenOptions options, object clientData, OnQueryOwnershipTokenCallback completionDelegate)
		{
			QueryOwnershipTokenOptionsInternal queryOwnershipTokenOptionsInternal = Helper.CopyProperties<QueryOwnershipTokenOptionsInternal>(options);
			OnQueryOwnershipTokenCallbackInternal onQueryOwnershipTokenCallbackInternal = new OnQueryOwnershipTokenCallbackInternal(EcomInterface.OnQueryOwnershipToken);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onQueryOwnershipTokenCallbackInternal, Array.Empty<Delegate>());
			EcomInterface.EOS_Ecom_QueryOwnershipToken(base.InnerHandle, ref queryOwnershipTokenOptionsInternal, zero, onQueryOwnershipTokenCallbackInternal);
			Helper.TryMarshalDispose<QueryOwnershipTokenOptionsInternal>(ref queryOwnershipTokenOptionsInternal);
		}

		public void QueryEntitlements(QueryEntitlementsOptions options, object clientData, OnQueryEntitlementsCallback completionDelegate)
		{
			QueryEntitlementsOptionsInternal queryEntitlementsOptionsInternal = Helper.CopyProperties<QueryEntitlementsOptionsInternal>(options);
			OnQueryEntitlementsCallbackInternal onQueryEntitlementsCallbackInternal = new OnQueryEntitlementsCallbackInternal(EcomInterface.OnQueryEntitlements);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onQueryEntitlementsCallbackInternal, Array.Empty<Delegate>());
			EcomInterface.EOS_Ecom_QueryEntitlements(base.InnerHandle, ref queryEntitlementsOptionsInternal, zero, onQueryEntitlementsCallbackInternal);
			Helper.TryMarshalDispose<QueryEntitlementsOptionsInternal>(ref queryEntitlementsOptionsInternal);
		}

		public void QueryOffers(QueryOffersOptions options, object clientData, OnQueryOffersCallback completionDelegate)
		{
			QueryOffersOptionsInternal queryOffersOptionsInternal = Helper.CopyProperties<QueryOffersOptionsInternal>(options);
			OnQueryOffersCallbackInternal onQueryOffersCallbackInternal = new OnQueryOffersCallbackInternal(EcomInterface.OnQueryOffers);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onQueryOffersCallbackInternal, Array.Empty<Delegate>());
			EcomInterface.EOS_Ecom_QueryOffers(base.InnerHandle, ref queryOffersOptionsInternal, zero, onQueryOffersCallbackInternal);
			Helper.TryMarshalDispose<QueryOffersOptionsInternal>(ref queryOffersOptionsInternal);
		}

		public void Checkout(CheckoutOptions options, object clientData, OnCheckoutCallback completionDelegate)
		{
			CheckoutOptionsInternal checkoutOptionsInternal = Helper.CopyProperties<CheckoutOptionsInternal>(options);
			OnCheckoutCallbackInternal onCheckoutCallbackInternal = new OnCheckoutCallbackInternal(EcomInterface.OnCheckout);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onCheckoutCallbackInternal, Array.Empty<Delegate>());
			EcomInterface.EOS_Ecom_Checkout(base.InnerHandle, ref checkoutOptionsInternal, zero, onCheckoutCallbackInternal);
			Helper.TryMarshalDispose<CheckoutOptionsInternal>(ref checkoutOptionsInternal);
		}

		public void RedeemEntitlements(RedeemEntitlementsOptions options, object clientData, OnRedeemEntitlementsCallback completionDelegate)
		{
			RedeemEntitlementsOptionsInternal redeemEntitlementsOptionsInternal = Helper.CopyProperties<RedeemEntitlementsOptionsInternal>(options);
			OnRedeemEntitlementsCallbackInternal onRedeemEntitlementsCallbackInternal = new OnRedeemEntitlementsCallbackInternal(EcomInterface.OnRedeemEntitlements);
			IntPtr zero = IntPtr.Zero;
			Helper.AddCallback(ref zero, clientData, completionDelegate, onRedeemEntitlementsCallbackInternal, Array.Empty<Delegate>());
			EcomInterface.EOS_Ecom_RedeemEntitlements(base.InnerHandle, ref redeemEntitlementsOptionsInternal, zero, onRedeemEntitlementsCallbackInternal);
			Helper.TryMarshalDispose<RedeemEntitlementsOptionsInternal>(ref redeemEntitlementsOptionsInternal);
		}

		public uint GetEntitlementsCount(GetEntitlementsCountOptions options)
		{
			GetEntitlementsCountOptionsInternal getEntitlementsCountOptionsInternal = Helper.CopyProperties<GetEntitlementsCountOptionsInternal>(options);
			uint num = EcomInterface.EOS_Ecom_GetEntitlementsCount(base.InnerHandle, ref getEntitlementsCountOptionsInternal);
			Helper.TryMarshalDispose<GetEntitlementsCountOptionsInternal>(ref getEntitlementsCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public uint GetEntitlementsByNameCount(GetEntitlementsByNameCountOptions options)
		{
			GetEntitlementsByNameCountOptionsInternal getEntitlementsByNameCountOptionsInternal = Helper.CopyProperties<GetEntitlementsByNameCountOptionsInternal>(options);
			uint num = EcomInterface.EOS_Ecom_GetEntitlementsByNameCount(base.InnerHandle, ref getEntitlementsByNameCountOptionsInternal);
			Helper.TryMarshalDispose<GetEntitlementsByNameCountOptionsInternal>(ref getEntitlementsByNameCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopyEntitlementByIndex(CopyEntitlementByIndexOptions options, out Entitlement outEntitlement)
		{
			CopyEntitlementByIndexOptionsInternal copyEntitlementByIndexOptionsInternal = Helper.CopyProperties<CopyEntitlementByIndexOptionsInternal>(options);
			outEntitlement = Helper.GetDefault<Entitlement>();
			IntPtr zero = IntPtr.Zero;
			Result result = EcomInterface.EOS_Ecom_CopyEntitlementByIndex(base.InnerHandle, ref copyEntitlementByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyEntitlementByIndexOptionsInternal>(ref copyEntitlementByIndexOptionsInternal);
			if (Helper.TryMarshalGet<EntitlementInternal, Entitlement>(zero, out outEntitlement))
			{
				EcomInterface.EOS_Ecom_Entitlement_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyEntitlementByNameAndIndex(CopyEntitlementByNameAndIndexOptions options, out Entitlement outEntitlement)
		{
			CopyEntitlementByNameAndIndexOptionsInternal copyEntitlementByNameAndIndexOptionsInternal = Helper.CopyProperties<CopyEntitlementByNameAndIndexOptionsInternal>(options);
			outEntitlement = Helper.GetDefault<Entitlement>();
			IntPtr zero = IntPtr.Zero;
			Result result = EcomInterface.EOS_Ecom_CopyEntitlementByNameAndIndex(base.InnerHandle, ref copyEntitlementByNameAndIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyEntitlementByNameAndIndexOptionsInternal>(ref copyEntitlementByNameAndIndexOptionsInternal);
			if (Helper.TryMarshalGet<EntitlementInternal, Entitlement>(zero, out outEntitlement))
			{
				EcomInterface.EOS_Ecom_Entitlement_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyEntitlementById(CopyEntitlementByIdOptions options, out Entitlement outEntitlement)
		{
			CopyEntitlementByIdOptionsInternal copyEntitlementByIdOptionsInternal = Helper.CopyProperties<CopyEntitlementByIdOptionsInternal>(options);
			outEntitlement = Helper.GetDefault<Entitlement>();
			IntPtr zero = IntPtr.Zero;
			Result result = EcomInterface.EOS_Ecom_CopyEntitlementById(base.InnerHandle, ref copyEntitlementByIdOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyEntitlementByIdOptionsInternal>(ref copyEntitlementByIdOptionsInternal);
			if (Helper.TryMarshalGet<EntitlementInternal, Entitlement>(zero, out outEntitlement))
			{
				EcomInterface.EOS_Ecom_Entitlement_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public uint GetOfferCount(GetOfferCountOptions options)
		{
			GetOfferCountOptionsInternal getOfferCountOptionsInternal = Helper.CopyProperties<GetOfferCountOptionsInternal>(options);
			uint num = EcomInterface.EOS_Ecom_GetOfferCount(base.InnerHandle, ref getOfferCountOptionsInternal);
			Helper.TryMarshalDispose<GetOfferCountOptionsInternal>(ref getOfferCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopyOfferByIndex(CopyOfferByIndexOptions options, out CatalogOffer outOffer)
		{
			CopyOfferByIndexOptionsInternal copyOfferByIndexOptionsInternal = Helper.CopyProperties<CopyOfferByIndexOptionsInternal>(options);
			outOffer = Helper.GetDefault<CatalogOffer>();
			IntPtr zero = IntPtr.Zero;
			Result result = EcomInterface.EOS_Ecom_CopyOfferByIndex(base.InnerHandle, ref copyOfferByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyOfferByIndexOptionsInternal>(ref copyOfferByIndexOptionsInternal);
			if (Helper.TryMarshalGet<CatalogOfferInternal, CatalogOffer>(zero, out outOffer))
			{
				EcomInterface.EOS_Ecom_CatalogOffer_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyOfferById(CopyOfferByIdOptions options, out CatalogOffer outOffer)
		{
			CopyOfferByIdOptionsInternal copyOfferByIdOptionsInternal = Helper.CopyProperties<CopyOfferByIdOptionsInternal>(options);
			outOffer = Helper.GetDefault<CatalogOffer>();
			IntPtr zero = IntPtr.Zero;
			Result result = EcomInterface.EOS_Ecom_CopyOfferById(base.InnerHandle, ref copyOfferByIdOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyOfferByIdOptionsInternal>(ref copyOfferByIdOptionsInternal);
			if (Helper.TryMarshalGet<CatalogOfferInternal, CatalogOffer>(zero, out outOffer))
			{
				EcomInterface.EOS_Ecom_CatalogOffer_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public uint GetOfferItemCount(GetOfferItemCountOptions options)
		{
			GetOfferItemCountOptionsInternal getOfferItemCountOptionsInternal = Helper.CopyProperties<GetOfferItemCountOptionsInternal>(options);
			uint num = EcomInterface.EOS_Ecom_GetOfferItemCount(base.InnerHandle, ref getOfferItemCountOptionsInternal);
			Helper.TryMarshalDispose<GetOfferItemCountOptionsInternal>(ref getOfferItemCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopyOfferItemByIndex(CopyOfferItemByIndexOptions options, out CatalogItem outItem)
		{
			CopyOfferItemByIndexOptionsInternal copyOfferItemByIndexOptionsInternal = Helper.CopyProperties<CopyOfferItemByIndexOptionsInternal>(options);
			outItem = Helper.GetDefault<CatalogItem>();
			IntPtr zero = IntPtr.Zero;
			Result result = EcomInterface.EOS_Ecom_CopyOfferItemByIndex(base.InnerHandle, ref copyOfferItemByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyOfferItemByIndexOptionsInternal>(ref copyOfferItemByIndexOptionsInternal);
			if (Helper.TryMarshalGet<CatalogItemInternal, CatalogItem>(zero, out outItem))
			{
				EcomInterface.EOS_Ecom_CatalogItem_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyItemById(CopyItemByIdOptions options, out CatalogItem outItem)
		{
			CopyItemByIdOptionsInternal copyItemByIdOptionsInternal = Helper.CopyProperties<CopyItemByIdOptionsInternal>(options);
			outItem = Helper.GetDefault<CatalogItem>();
			IntPtr zero = IntPtr.Zero;
			Result result = EcomInterface.EOS_Ecom_CopyItemById(base.InnerHandle, ref copyItemByIdOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyItemByIdOptionsInternal>(ref copyItemByIdOptionsInternal);
			if (Helper.TryMarshalGet<CatalogItemInternal, CatalogItem>(zero, out outItem))
			{
				EcomInterface.EOS_Ecom_CatalogItem_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public uint GetOfferImageInfoCount(GetOfferImageInfoCountOptions options)
		{
			GetOfferImageInfoCountOptionsInternal getOfferImageInfoCountOptionsInternal = Helper.CopyProperties<GetOfferImageInfoCountOptionsInternal>(options);
			uint num = EcomInterface.EOS_Ecom_GetOfferImageInfoCount(base.InnerHandle, ref getOfferImageInfoCountOptionsInternal);
			Helper.TryMarshalDispose<GetOfferImageInfoCountOptionsInternal>(ref getOfferImageInfoCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopyOfferImageInfoByIndex(CopyOfferImageInfoByIndexOptions options, out KeyImageInfo outImageInfo)
		{
			CopyOfferImageInfoByIndexOptionsInternal copyOfferImageInfoByIndexOptionsInternal = Helper.CopyProperties<CopyOfferImageInfoByIndexOptionsInternal>(options);
			outImageInfo = Helper.GetDefault<KeyImageInfo>();
			IntPtr zero = IntPtr.Zero;
			Result result = EcomInterface.EOS_Ecom_CopyOfferImageInfoByIndex(base.InnerHandle, ref copyOfferImageInfoByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyOfferImageInfoByIndexOptionsInternal>(ref copyOfferImageInfoByIndexOptionsInternal);
			if (Helper.TryMarshalGet<KeyImageInfoInternal, KeyImageInfo>(zero, out outImageInfo))
			{
				EcomInterface.EOS_Ecom_KeyImageInfo_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public uint GetItemImageInfoCount(GetItemImageInfoCountOptions options)
		{
			GetItemImageInfoCountOptionsInternal getItemImageInfoCountOptionsInternal = Helper.CopyProperties<GetItemImageInfoCountOptionsInternal>(options);
			uint num = EcomInterface.EOS_Ecom_GetItemImageInfoCount(base.InnerHandle, ref getItemImageInfoCountOptionsInternal);
			Helper.TryMarshalDispose<GetItemImageInfoCountOptionsInternal>(ref getItemImageInfoCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopyItemImageInfoByIndex(CopyItemImageInfoByIndexOptions options, out KeyImageInfo outImageInfo)
		{
			CopyItemImageInfoByIndexOptionsInternal copyItemImageInfoByIndexOptionsInternal = Helper.CopyProperties<CopyItemImageInfoByIndexOptionsInternal>(options);
			outImageInfo = Helper.GetDefault<KeyImageInfo>();
			IntPtr zero = IntPtr.Zero;
			Result result = EcomInterface.EOS_Ecom_CopyItemImageInfoByIndex(base.InnerHandle, ref copyItemImageInfoByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyItemImageInfoByIndexOptionsInternal>(ref copyItemImageInfoByIndexOptionsInternal);
			if (Helper.TryMarshalGet<KeyImageInfoInternal, KeyImageInfo>(zero, out outImageInfo))
			{
				EcomInterface.EOS_Ecom_KeyImageInfo_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public uint GetItemReleaseCount(GetItemReleaseCountOptions options)
		{
			GetItemReleaseCountOptionsInternal getItemReleaseCountOptionsInternal = Helper.CopyProperties<GetItemReleaseCountOptionsInternal>(options);
			uint num = EcomInterface.EOS_Ecom_GetItemReleaseCount(base.InnerHandle, ref getItemReleaseCountOptionsInternal);
			Helper.TryMarshalDispose<GetItemReleaseCountOptionsInternal>(ref getItemReleaseCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopyItemReleaseByIndex(CopyItemReleaseByIndexOptions options, out CatalogRelease outRelease)
		{
			CopyItemReleaseByIndexOptionsInternal copyItemReleaseByIndexOptionsInternal = Helper.CopyProperties<CopyItemReleaseByIndexOptionsInternal>(options);
			outRelease = Helper.GetDefault<CatalogRelease>();
			IntPtr zero = IntPtr.Zero;
			Result result = EcomInterface.EOS_Ecom_CopyItemReleaseByIndex(base.InnerHandle, ref copyItemReleaseByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyItemReleaseByIndexOptionsInternal>(ref copyItemReleaseByIndexOptionsInternal);
			if (Helper.TryMarshalGet<CatalogReleaseInternal, CatalogRelease>(zero, out outRelease))
			{
				EcomInterface.EOS_Ecom_CatalogRelease_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public uint GetTransactionCount(GetTransactionCountOptions options)
		{
			GetTransactionCountOptionsInternal getTransactionCountOptionsInternal = Helper.CopyProperties<GetTransactionCountOptionsInternal>(options);
			uint num = EcomInterface.EOS_Ecom_GetTransactionCount(base.InnerHandle, ref getTransactionCountOptionsInternal);
			Helper.TryMarshalDispose<GetTransactionCountOptionsInternal>(ref getTransactionCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopyTransactionByIndex(CopyTransactionByIndexOptions options, out Transaction outTransaction)
		{
			CopyTransactionByIndexOptionsInternal copyTransactionByIndexOptionsInternal = Helper.CopyProperties<CopyTransactionByIndexOptionsInternal>(options);
			outTransaction = Helper.GetDefault<Transaction>();
			IntPtr zero = IntPtr.Zero;
			Result result = EcomInterface.EOS_Ecom_CopyTransactionByIndex(base.InnerHandle, ref copyTransactionByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyTransactionByIndexOptionsInternal>(ref copyTransactionByIndexOptionsInternal);
			Helper.TryMarshalGet<Transaction>(zero, out outTransaction);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyTransactionById(CopyTransactionByIdOptions options, out Transaction outTransaction)
		{
			CopyTransactionByIdOptionsInternal copyTransactionByIdOptionsInternal = Helper.CopyProperties<CopyTransactionByIdOptionsInternal>(options);
			outTransaction = Helper.GetDefault<Transaction>();
			IntPtr zero = IntPtr.Zero;
			Result result = EcomInterface.EOS_Ecom_CopyTransactionById(base.InnerHandle, ref copyTransactionByIdOptionsInternal, ref zero);
			Helper.TryMarshalDispose<CopyTransactionByIdOptionsInternal>(ref copyTransactionByIdOptionsInternal);
			Helper.TryMarshalGet<Transaction>(zero, out outTransaction);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		[MonoPInvokeCallback]
		internal static void OnRedeemEntitlements(IntPtr address)
		{
			OnRedeemEntitlementsCallback onRedeemEntitlementsCallback = null;
			RedeemEntitlementsCallbackInfo redeemEntitlementsCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnRedeemEntitlementsCallback, RedeemEntitlementsCallbackInfoInternal, RedeemEntitlementsCallbackInfo>(address, out onRedeemEntitlementsCallback, out redeemEntitlementsCallbackInfo))
			{
				onRedeemEntitlementsCallback(redeemEntitlementsCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnCheckout(IntPtr address)
		{
			OnCheckoutCallback onCheckoutCallback = null;
			CheckoutCallbackInfo checkoutCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnCheckoutCallback, CheckoutCallbackInfoInternal, CheckoutCallbackInfo>(address, out onCheckoutCallback, out checkoutCallbackInfo))
			{
				onCheckoutCallback(checkoutCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryOffers(IntPtr address)
		{
			OnQueryOffersCallback onQueryOffersCallback = null;
			QueryOffersCallbackInfo queryOffersCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryOffersCallback, QueryOffersCallbackInfoInternal, QueryOffersCallbackInfo>(address, out onQueryOffersCallback, out queryOffersCallbackInfo))
			{
				onQueryOffersCallback(queryOffersCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryEntitlements(IntPtr address)
		{
			OnQueryEntitlementsCallback onQueryEntitlementsCallback = null;
			QueryEntitlementsCallbackInfo queryEntitlementsCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryEntitlementsCallback, QueryEntitlementsCallbackInfoInternal, QueryEntitlementsCallbackInfo>(address, out onQueryEntitlementsCallback, out queryEntitlementsCallbackInfo))
			{
				onQueryEntitlementsCallback(queryEntitlementsCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryOwnershipToken(IntPtr address)
		{
			OnQueryOwnershipTokenCallback onQueryOwnershipTokenCallback = null;
			QueryOwnershipTokenCallbackInfo queryOwnershipTokenCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryOwnershipTokenCallback, QueryOwnershipTokenCallbackInfoInternal, QueryOwnershipTokenCallbackInfo>(address, out onQueryOwnershipTokenCallback, out queryOwnershipTokenCallbackInfo))
			{
				onQueryOwnershipTokenCallback(queryOwnershipTokenCallbackInfo);
			}
		}

		[MonoPInvokeCallback]
		internal static void OnQueryOwnership(IntPtr address)
		{
			OnQueryOwnershipCallback onQueryOwnershipCallback = null;
			QueryOwnershipCallbackInfo queryOwnershipCallbackInfo = null;
			if (Helper.TryGetAndRemoveCallback<OnQueryOwnershipCallback, QueryOwnershipCallbackInfoInternal, QueryOwnershipCallbackInfo>(address, out onQueryOwnershipCallback, out queryOwnershipCallbackInfo))
			{
				onQueryOwnershipCallback(queryOwnershipCallbackInfo);
			}
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Ecom_CatalogRelease_Release(IntPtr catalogRelease);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Ecom_KeyImageInfo_Release(IntPtr keyImageInfo);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Ecom_CatalogOffer_Release(IntPtr catalogOffer);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Ecom_CatalogItem_Release(IntPtr catalogItem);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Ecom_Entitlement_Release(IntPtr entitlement);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Ecom_CopyTransactionById(IntPtr handle, ref CopyTransactionByIdOptionsInternal options, ref IntPtr outTransaction);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Ecom_CopyTransactionByIndex(IntPtr handle, ref CopyTransactionByIndexOptionsInternal options, ref IntPtr outTransaction);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_Ecom_GetTransactionCount(IntPtr handle, ref GetTransactionCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Ecom_CopyItemReleaseByIndex(IntPtr handle, ref CopyItemReleaseByIndexOptionsInternal options, ref IntPtr outRelease);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_Ecom_GetItemReleaseCount(IntPtr handle, ref GetItemReleaseCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Ecom_CopyItemImageInfoByIndex(IntPtr handle, ref CopyItemImageInfoByIndexOptionsInternal options, ref IntPtr outImageInfo);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_Ecom_GetItemImageInfoCount(IntPtr handle, ref GetItemImageInfoCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Ecom_CopyOfferImageInfoByIndex(IntPtr handle, ref CopyOfferImageInfoByIndexOptionsInternal options, ref IntPtr outImageInfo);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_Ecom_GetOfferImageInfoCount(IntPtr handle, ref GetOfferImageInfoCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Ecom_CopyItemById(IntPtr handle, ref CopyItemByIdOptionsInternal options, ref IntPtr outItem);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Ecom_CopyOfferItemByIndex(IntPtr handle, ref CopyOfferItemByIndexOptionsInternal options, ref IntPtr outItem);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_Ecom_GetOfferItemCount(IntPtr handle, ref GetOfferItemCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Ecom_CopyOfferById(IntPtr handle, ref CopyOfferByIdOptionsInternal options, ref IntPtr outOffer);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Ecom_CopyOfferByIndex(IntPtr handle, ref CopyOfferByIndexOptionsInternal options, ref IntPtr outOffer);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_Ecom_GetOfferCount(IntPtr handle, ref GetOfferCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Ecom_CopyEntitlementById(IntPtr handle, ref CopyEntitlementByIdOptionsInternal options, ref IntPtr outEntitlement);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Ecom_CopyEntitlementByNameAndIndex(IntPtr handle, ref CopyEntitlementByNameAndIndexOptionsInternal options, ref IntPtr outEntitlement);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Ecom_CopyEntitlementByIndex(IntPtr handle, ref CopyEntitlementByIndexOptionsInternal options, ref IntPtr outEntitlement);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_Ecom_GetEntitlementsByNameCount(IntPtr handle, ref GetEntitlementsByNameCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_Ecom_GetEntitlementsCount(IntPtr handle, ref GetEntitlementsCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Ecom_RedeemEntitlements(IntPtr handle, ref RedeemEntitlementsOptionsInternal options, IntPtr clientData, OnRedeemEntitlementsCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Ecom_Checkout(IntPtr handle, ref CheckoutOptionsInternal options, IntPtr clientData, OnCheckoutCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Ecom_QueryOffers(IntPtr handle, ref QueryOffersOptionsInternal options, IntPtr clientData, OnQueryOffersCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Ecom_QueryEntitlements(IntPtr handle, ref QueryEntitlementsOptionsInternal options, IntPtr clientData, OnQueryEntitlementsCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Ecom_QueryOwnershipToken(IntPtr handle, ref QueryOwnershipTokenOptionsInternal options, IntPtr clientData, OnQueryOwnershipTokenCallbackInternal completionDelegate);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Ecom_QueryOwnership(IntPtr handle, ref QueryOwnershipOptionsInternal options, IntPtr clientData, OnQueryOwnershipCallbackInternal completionDelegate);

		public const int TransactionCopyentitlementbyindexApiLatest = 1;

		public const int TransactionGetentitlementscountApiLatest = 1;

		public const int CopytransactionbyidApiLatest = 1;

		public const int CopytransactionbyindexApiLatest = 1;

		public const int GettransactioncountApiLatest = 1;

		public const int CopyitemreleasebyindexApiLatest = 1;

		public const int GetitemreleasecountApiLatest = 1;

		public const int CopyitemimageinfobyindexApiLatest = 1;

		public const int GetitemimageinfocountApiLatest = 1;

		public const int CopyofferimageinfobyindexApiLatest = 1;

		public const int GetofferimageinfocountApiLatest = 1;

		public const int CopyitembyidApiLatest = 1;

		public const int CopyofferitembyindexApiLatest = 1;

		public const int GetofferitemcountApiLatest = 1;

		public const int CopyofferbyidApiLatest = 1;

		public const int CopyofferbyindexApiLatest = 1;

		public const int GetoffercountApiLatest = 1;

		public const int CopyentitlementbyidApiLatest = 2;

		public const int CopyentitlementbynameandindexApiLatest = 1;

		public const int CopyentitlementbyindexApiLatest = 1;

		public const int GetentitlementsbynamecountApiLatest = 1;

		public const int GetentitlementscountApiLatest = 1;

		public const int RedeementitlementsMaxIds = 32;

		public const int RedeementitlementsApiLatest = 1;

		public const int TransactionidMaximumLength = 64;

		public const int CheckoutMaxEntries = 10;

		public const int CheckoutApiLatest = 1;

		public const int QueryoffersApiLatest = 1;

		public const int QueryentitlementsMaxEntitlementIds = 32;

		public const int QueryentitlementsApiLatest = 2;

		public const int QueryownershiptokenMaxCatalogitemIds = 32;

		public const int QueryownershiptokenApiLatest = 2;

		public const int QueryownershipMaxCatalogIds = 32;

		public const int QueryownershipApiLatest = 2;

		public const int CheckoutentryApiLatest = 1;

		public const int CatalogreleaseApiLatest = 1;

		public const int KeyimageinfoApiLatest = 1;

		public const int CatalogofferExpirationtimestampUndefined = -1;

		public const int CatalogofferApiLatest = 2;

		public const int CatalogitemEntitlementendtimestampUndefined = -1;

		public const int CatalogitemApiLatest = 1;

		public const int ItemownershipApiLatest = 1;

		public const int EntitlementEndtimestampUndefined = -1;

		public const int EntitlementApiLatest = 2;
	}
}
