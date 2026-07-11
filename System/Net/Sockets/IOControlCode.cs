using System;

namespace System.Net.Sockets
{
	public enum IOControlCode : long
	{
		AsyncIO = 2147772029L,
		NonBlockingIO,
		DataToRead = 1074030207L,
		OobDataRead = 1074033415L,
		AssociateHandle = 2281701377L,
		EnableCircularQueuing = 671088642L,
		Flush = 671088644L,
		GetBroadcastAddress = 1207959557L,
		GetExtensionFunctionPointer = 3355443206L,
		GetQos,
		GetGroupQos,
		MultipointLoopback = 2281701385L,
		MulticastScope,
		SetQos,
		SetGroupQos,
		TranslateHandle = 3355443213L,
		RoutingInterfaceQuery = 3355443220L,
		RoutingInterfaceChange = 2281701397L,
		AddressListQuery = 1207959574L,
		AddressListChange = 671088663L,
		QueryTargetPnpHandle = 1207959576L,
		NamespaceChange = 2281701401L,
		AddressListSort = 3355443225L,
		ReceiveAll = 2550136833L,
		ReceiveAllMulticast,
		ReceiveAllIgmpMulticast,
		KeepAliveValues,
		AbsorbRouterAlert,
		UnicastInterface,
		LimitBroadcasts,
		BindToInterface,
		MulticastInterface,
		AddMulticastGroupOnInterface,
		DeleteMulticastGroupFromInterface
	}
}
