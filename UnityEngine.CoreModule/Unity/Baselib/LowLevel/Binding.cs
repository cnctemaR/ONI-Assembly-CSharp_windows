using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace Unity.Baselib.LowLevel
{
	[NativeHeader("External/baselib/builds/CSharp/UnityBinding/Baselib_RegisteredNetwork.gen.binding.h")]
	[NativeHeader("External/baselib/builds/CSharp/UnityBinding/Baselib_SourceLocation.gen.binding.h")]
	[NativeHeader("External/baselib/builds/CSharp/UnityBinding/Baselib_ErrorCode.gen.binding.h")]
	[NativeHeader("External/baselib/builds/CSharp/UnityBinding/Baselib_ErrorState.gen.binding.h")]
	[NativeHeader("External/baselib/builds/CSharp/UnityBinding/Baselib_NetworkAddress.gen.binding.h")]
	[NativeHeader("External/baselib/builds/CSharp/UnityBinding/Baselib_Memory.gen.binding.h")]
	internal static class Binding
	{
		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern uint Baselib_ErrorState_Explain(Binding.Baselib_ErrorState* errorState, byte* buffer, uint bufferLen, Binding.Baselib_ErrorState_ExplainVerbosity verbosity);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void Baselib_Memory_GetPageSizeInfo(Binding.Baselib_Memory_PageSizeInfo* outPagesSizeInfo);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr Baselib_Memory_Allocate(UIntPtr size);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr Baselib_Memory_Reallocate(IntPtr ptr, UIntPtr newSize);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Baselib_Memory_Free(IntPtr ptr);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr Baselib_Memory_AlignedAllocate(UIntPtr size, UIntPtr alignment);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr Baselib_Memory_AlignedReallocate(IntPtr ptr, UIntPtr newSize, UIntPtr alignment);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Baselib_Memory_AlignedFree(IntPtr ptr);

		[FreeFunction(IsThreadSafe = true)]
		public unsafe static Binding.Baselib_Memory_PageAllocation Baselib_Memory_AllocatePages(ulong pageSize, ulong pageCount, ulong alignmentInMultipleOfPageSize, Binding.Baselib_Memory_PageState pageState, Binding.Baselib_ErrorState* errorState)
		{
			Binding.Baselib_Memory_PageAllocation baselib_Memory_PageAllocation;
			Binding.Baselib_Memory_AllocatePages_Injected(pageSize, pageCount, alignmentInMultipleOfPageSize, pageState, errorState, out baselib_Memory_PageAllocation);
			return baselib_Memory_PageAllocation;
		}

		[FreeFunction(IsThreadSafe = true)]
		public unsafe static void Baselib_Memory_ReleasePages(Binding.Baselib_Memory_PageAllocation pageAllocation, Binding.Baselib_ErrorState* errorState)
		{
			Binding.Baselib_Memory_ReleasePages_Injected(ref pageAllocation, errorState);
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void Baselib_Memory_SetPageState(IntPtr addressOfFirstPage, ulong pageSize, ulong pageCount, Binding.Baselib_Memory_PageState pageState, Binding.Baselib_ErrorState* errorState);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void Baselib_NetworkAddress_Encode(Binding.Baselib_NetworkAddress* dstAddress, Binding.Baselib_NetworkAddress_Family family, byte* ip, ushort port, Binding.Baselib_ErrorState* errorState);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void Baselib_NetworkAddress_Decode(Binding.Baselib_NetworkAddress* srcAddress, Binding.Baselib_NetworkAddress_Family* family, byte* ipAddressBuffer, uint ipAddressBufferLen, ushort* port, Binding.Baselib_ErrorState* errorState);

		[FreeFunction(IsThreadSafe = true)]
		public unsafe static Binding.Baselib_RegisteredNetwork_Buffer Baselib_RegisteredNetwork_Buffer_Register(Binding.Baselib_Memory_PageAllocation pageAllocation, Binding.Baselib_ErrorState* errorState)
		{
			Binding.Baselib_RegisteredNetwork_Buffer baselib_RegisteredNetwork_Buffer;
			Binding.Baselib_RegisteredNetwork_Buffer_Register_Injected(ref pageAllocation, errorState, out baselib_RegisteredNetwork_Buffer);
			return baselib_RegisteredNetwork_Buffer;
		}

		[FreeFunction(IsThreadSafe = true)]
		public static void Baselib_RegisteredNetwork_Buffer_Deregister(Binding.Baselib_RegisteredNetwork_Buffer buffer)
		{
			Binding.Baselib_RegisteredNetwork_Buffer_Deregister_Injected(ref buffer);
		}

		[FreeFunction(IsThreadSafe = true)]
		public static Binding.Baselib_RegisteredNetwork_BufferSlice Baselib_RegisteredNetwork_BufferSlice_Create(Binding.Baselib_RegisteredNetwork_Buffer buffer, uint offset, uint size)
		{
			Binding.Baselib_RegisteredNetwork_BufferSlice baselib_RegisteredNetwork_BufferSlice;
			Binding.Baselib_RegisteredNetwork_BufferSlice_Create_Injected(ref buffer, offset, size, out baselib_RegisteredNetwork_BufferSlice);
			return baselib_RegisteredNetwork_BufferSlice;
		}

		[FreeFunction(IsThreadSafe = true)]
		public static Binding.Baselib_RegisteredNetwork_BufferSlice Baselib_RegisteredNetwork_BufferSlice_Empty()
		{
			Binding.Baselib_RegisteredNetwork_BufferSlice baselib_RegisteredNetwork_BufferSlice;
			Binding.Baselib_RegisteredNetwork_BufferSlice_Empty_Injected(out baselib_RegisteredNetwork_BufferSlice);
			return baselib_RegisteredNetwork_BufferSlice;
		}

		[FreeFunction(IsThreadSafe = true)]
		public unsafe static Binding.Baselib_RegisteredNetwork_Endpoint Baselib_RegisteredNetwork_Endpoint_Create(Binding.Baselib_NetworkAddress* srcAddress, Binding.Baselib_RegisteredNetwork_BufferSlice dstSlice, Binding.Baselib_ErrorState* errorState)
		{
			Binding.Baselib_RegisteredNetwork_Endpoint baselib_RegisteredNetwork_Endpoint;
			Binding.Baselib_RegisteredNetwork_Endpoint_Create_Injected(srcAddress, ref dstSlice, errorState, out baselib_RegisteredNetwork_Endpoint);
			return baselib_RegisteredNetwork_Endpoint;
		}

		[FreeFunction(IsThreadSafe = true)]
		public static Binding.Baselib_RegisteredNetwork_Endpoint Baselib_RegisteredNetwork_Endpoint_Empty()
		{
			Binding.Baselib_RegisteredNetwork_Endpoint baselib_RegisteredNetwork_Endpoint;
			Binding.Baselib_RegisteredNetwork_Endpoint_Empty_Injected(out baselib_RegisteredNetwork_Endpoint);
			return baselib_RegisteredNetwork_Endpoint;
		}

		[FreeFunction(IsThreadSafe = true)]
		public unsafe static void Baselib_RegisteredNetwork_Endpoint_GetNetworkAddress(Binding.Baselib_RegisteredNetwork_Endpoint endpoint, Binding.Baselib_NetworkAddress* dstAddress, Binding.Baselib_ErrorState* errorState)
		{
			Binding.Baselib_RegisteredNetwork_Endpoint_GetNetworkAddress_Injected(ref endpoint, dstAddress, errorState);
		}

		[FreeFunction(IsThreadSafe = true)]
		public unsafe static Binding.Baselib_RegisteredNetwork_Socket_UDP Baselib_RegisteredNetwork_Socket_UDP_Create(Binding.Baselib_NetworkAddress* bindAddress, Binding.Baselib_NetworkAddress_AddressReuse endpointReuse, uint sendQueueSize, uint recvQueueSize, Binding.Baselib_ErrorState* errorState)
		{
			Binding.Baselib_RegisteredNetwork_Socket_UDP baselib_RegisteredNetwork_Socket_UDP;
			Binding.Baselib_RegisteredNetwork_Socket_UDP_Create_Injected(bindAddress, endpointReuse, sendQueueSize, recvQueueSize, errorState, out baselib_RegisteredNetwork_Socket_UDP);
			return baselib_RegisteredNetwork_Socket_UDP;
		}

		[FreeFunction(IsThreadSafe = true)]
		public unsafe static uint Baselib_RegisteredNetwork_Socket_UDP_ScheduleRecv(Binding.Baselib_RegisteredNetwork_Socket_UDP socket, Binding.Baselib_RegisteredNetwork_Request* requests, uint requestsCount, Binding.Baselib_ErrorState* errorState)
		{
			return Binding.Baselib_RegisteredNetwork_Socket_UDP_ScheduleRecv_Injected(ref socket, requests, requestsCount, errorState);
		}

		[FreeFunction(IsThreadSafe = true)]
		public unsafe static uint Baselib_RegisteredNetwork_Socket_UDP_ScheduleSend(Binding.Baselib_RegisteredNetwork_Socket_UDP socket, Binding.Baselib_RegisteredNetwork_Request* requests, uint requestsCount, Binding.Baselib_ErrorState* errorState)
		{
			return Binding.Baselib_RegisteredNetwork_Socket_UDP_ScheduleSend_Injected(ref socket, requests, requestsCount, errorState);
		}

		[FreeFunction(IsThreadSafe = true)]
		public unsafe static Binding.Baselib_RegisteredNetwork_ProcessStatus Baselib_RegisteredNetwork_Socket_UDP_ProcessRecv(Binding.Baselib_RegisteredNetwork_Socket_UDP socket, Binding.Baselib_ErrorState* errorState)
		{
			return Binding.Baselib_RegisteredNetwork_Socket_UDP_ProcessRecv_Injected(ref socket, errorState);
		}

		[FreeFunction(IsThreadSafe = true)]
		public unsafe static Binding.Baselib_RegisteredNetwork_ProcessStatus Baselib_RegisteredNetwork_Socket_UDP_ProcessSend(Binding.Baselib_RegisteredNetwork_Socket_UDP socket, Binding.Baselib_ErrorState* errorState)
		{
			return Binding.Baselib_RegisteredNetwork_Socket_UDP_ProcessSend_Injected(ref socket, errorState);
		}

		[FreeFunction(IsThreadSafe = true)]
		public unsafe static Binding.Baselib_RegisteredNetwork_CompletionQueueStatus Baselib_RegisteredNetwork_Socket_UDP_WaitForCompletedRecv(Binding.Baselib_RegisteredNetwork_Socket_UDP socket, uint timeoutInMilliseconds, Binding.Baselib_ErrorState* errorState)
		{
			return Binding.Baselib_RegisteredNetwork_Socket_UDP_WaitForCompletedRecv_Injected(ref socket, timeoutInMilliseconds, errorState);
		}

		[FreeFunction(IsThreadSafe = true)]
		public unsafe static Binding.Baselib_RegisteredNetwork_CompletionQueueStatus Baselib_RegisteredNetwork_Socket_UDP_WaitForCompletedSend(Binding.Baselib_RegisteredNetwork_Socket_UDP socket, uint timeoutInMilliseconds, Binding.Baselib_ErrorState* errorState)
		{
			return Binding.Baselib_RegisteredNetwork_Socket_UDP_WaitForCompletedSend_Injected(ref socket, timeoutInMilliseconds, errorState);
		}

		[FreeFunction(IsThreadSafe = true)]
		public unsafe static uint Baselib_RegisteredNetwork_Socket_UDP_DequeueRecv(Binding.Baselib_RegisteredNetwork_Socket_UDP socket, Binding.Baselib_RegisteredNetwork_CompletionResult* results, uint resultsCount, Binding.Baselib_ErrorState* errorState)
		{
			return Binding.Baselib_RegisteredNetwork_Socket_UDP_DequeueRecv_Injected(ref socket, results, resultsCount, errorState);
		}

		[FreeFunction(IsThreadSafe = true)]
		public unsafe static uint Baselib_RegisteredNetwork_Socket_UDP_DequeueSend(Binding.Baselib_RegisteredNetwork_Socket_UDP socket, Binding.Baselib_RegisteredNetwork_CompletionResult* results, uint resultsCount, Binding.Baselib_ErrorState* errorState)
		{
			return Binding.Baselib_RegisteredNetwork_Socket_UDP_DequeueSend_Injected(ref socket, results, resultsCount, errorState);
		}

		[FreeFunction(IsThreadSafe = true)]
		public unsafe static void Baselib_RegisteredNetwork_Socket_UDP_GetNetworkAddress(Binding.Baselib_RegisteredNetwork_Socket_UDP socket, Binding.Baselib_NetworkAddress* dstAddress, Binding.Baselib_ErrorState* errorState)
		{
			Binding.Baselib_RegisteredNetwork_Socket_UDP_GetNetworkAddress_Injected(ref socket, dstAddress, errorState);
		}

		[FreeFunction(IsThreadSafe = true)]
		public static void Baselib_RegisteredNetwork_Socket_UDP_Close(Binding.Baselib_RegisteredNetwork_Socket_UDP socket)
		{
			Binding.Baselib_RegisteredNetwork_Socket_UDP_Close_Injected(ref socket);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void Baselib_Memory_AllocatePages_Injected(ulong pageSize, ulong pageCount, ulong alignmentInMultipleOfPageSize, Binding.Baselib_Memory_PageState pageState, Binding.Baselib_ErrorState* errorState, out Binding.Baselib_Memory_PageAllocation ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void Baselib_Memory_ReleasePages_Injected(ref Binding.Baselib_Memory_PageAllocation pageAllocation, Binding.Baselib_ErrorState* errorState);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void Baselib_RegisteredNetwork_Buffer_Register_Injected(ref Binding.Baselib_Memory_PageAllocation pageAllocation, Binding.Baselib_ErrorState* errorState, out Binding.Baselib_RegisteredNetwork_Buffer ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Baselib_RegisteredNetwork_Buffer_Deregister_Injected(ref Binding.Baselib_RegisteredNetwork_Buffer buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Baselib_RegisteredNetwork_BufferSlice_Create_Injected(ref Binding.Baselib_RegisteredNetwork_Buffer buffer, uint offset, uint size, out Binding.Baselib_RegisteredNetwork_BufferSlice ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Baselib_RegisteredNetwork_BufferSlice_Empty_Injected(out Binding.Baselib_RegisteredNetwork_BufferSlice ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void Baselib_RegisteredNetwork_Endpoint_Create_Injected(Binding.Baselib_NetworkAddress* srcAddress, ref Binding.Baselib_RegisteredNetwork_BufferSlice dstSlice, Binding.Baselib_ErrorState* errorState, out Binding.Baselib_RegisteredNetwork_Endpoint ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Baselib_RegisteredNetwork_Endpoint_Empty_Injected(out Binding.Baselib_RegisteredNetwork_Endpoint ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void Baselib_RegisteredNetwork_Endpoint_GetNetworkAddress_Injected(ref Binding.Baselib_RegisteredNetwork_Endpoint endpoint, Binding.Baselib_NetworkAddress* dstAddress, Binding.Baselib_ErrorState* errorState);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void Baselib_RegisteredNetwork_Socket_UDP_Create_Injected(Binding.Baselib_NetworkAddress* bindAddress, Binding.Baselib_NetworkAddress_AddressReuse endpointReuse, uint sendQueueSize, uint recvQueueSize, Binding.Baselib_ErrorState* errorState, out Binding.Baselib_RegisteredNetwork_Socket_UDP ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern uint Baselib_RegisteredNetwork_Socket_UDP_ScheduleRecv_Injected(ref Binding.Baselib_RegisteredNetwork_Socket_UDP socket, Binding.Baselib_RegisteredNetwork_Request* requests, uint requestsCount, Binding.Baselib_ErrorState* errorState);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern uint Baselib_RegisteredNetwork_Socket_UDP_ScheduleSend_Injected(ref Binding.Baselib_RegisteredNetwork_Socket_UDP socket, Binding.Baselib_RegisteredNetwork_Request* requests, uint requestsCount, Binding.Baselib_ErrorState* errorState);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern Binding.Baselib_RegisteredNetwork_ProcessStatus Baselib_RegisteredNetwork_Socket_UDP_ProcessRecv_Injected(ref Binding.Baselib_RegisteredNetwork_Socket_UDP socket, Binding.Baselib_ErrorState* errorState);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern Binding.Baselib_RegisteredNetwork_ProcessStatus Baselib_RegisteredNetwork_Socket_UDP_ProcessSend_Injected(ref Binding.Baselib_RegisteredNetwork_Socket_UDP socket, Binding.Baselib_ErrorState* errorState);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern Binding.Baselib_RegisteredNetwork_CompletionQueueStatus Baselib_RegisteredNetwork_Socket_UDP_WaitForCompletedRecv_Injected(ref Binding.Baselib_RegisteredNetwork_Socket_UDP socket, uint timeoutInMilliseconds, Binding.Baselib_ErrorState* errorState);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern Binding.Baselib_RegisteredNetwork_CompletionQueueStatus Baselib_RegisteredNetwork_Socket_UDP_WaitForCompletedSend_Injected(ref Binding.Baselib_RegisteredNetwork_Socket_UDP socket, uint timeoutInMilliseconds, Binding.Baselib_ErrorState* errorState);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern uint Baselib_RegisteredNetwork_Socket_UDP_DequeueRecv_Injected(ref Binding.Baselib_RegisteredNetwork_Socket_UDP socket, Binding.Baselib_RegisteredNetwork_CompletionResult* results, uint resultsCount, Binding.Baselib_ErrorState* errorState);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern uint Baselib_RegisteredNetwork_Socket_UDP_DequeueSend_Injected(ref Binding.Baselib_RegisteredNetwork_Socket_UDP socket, Binding.Baselib_RegisteredNetwork_CompletionResult* results, uint resultsCount, Binding.Baselib_ErrorState* errorState);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void Baselib_RegisteredNetwork_Socket_UDP_GetNetworkAddress_Injected(ref Binding.Baselib_RegisteredNetwork_Socket_UDP socket, Binding.Baselib_NetworkAddress* dstAddress, Binding.Baselib_ErrorState* errorState);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Baselib_RegisteredNetwork_Socket_UDP_Close_Injected(ref Binding.Baselib_RegisteredNetwork_Socket_UDP socket);

		public static readonly UIntPtr Baselib_Memory_MaxAlignment = new UIntPtr(65536U);

		public static readonly Binding.Baselib_Memory_PageAllocation Baselib_Memory_PageAllocation_Invalid = default(Binding.Baselib_Memory_PageAllocation);

		public const uint Baselib_NetworkAddress_IpMaxStringLength = 46U;

		public static readonly IntPtr Baselib_RegisteredNetwork_Buffer_Id_Invalid = IntPtr.Zero;

		public const uint Baselib_RegisteredNetwork_Endpoint_MaxSize = 28U;

		public static readonly Binding.Baselib_RegisteredNetwork_Socket_UDP Baselib_RegisteredNetwork_Socket_UDP_Invalid = default(Binding.Baselib_RegisteredNetwork_Socket_UDP);

		public enum Baselib_ErrorCode
		{
			Success,
			OutOfMemory = 16777216,
			OutOfSystemResources,
			InvalidAddressRange,
			InvalidArgument,
			InvalidBufferSize,
			InvalidState,
			NotSupported,
			Timeout,
			UnsupportedAlignment = 33554432,
			InvalidPageSize,
			InvalidPageCount,
			UnsupportedPageState,
			UninitializedThreadConfig = 50331648,
			ThreadEntryPointFunctionNotSet,
			ThreadCannotJoinSelf,
			NetworkInitializationError = 67108864,
			AddressInUse,
			AddressUnreachable,
			AddressFamilyNotSupported,
			UnexpectedError = -1
		}

		public enum Baselib_ErrorState_NativeErrorCodeType : byte
		{
			None,
			PlatformDefined
		}

		public struct Baselib_ErrorState
		{
			public Binding.Baselib_ErrorCode code;

			public Binding.Baselib_ErrorState_NativeErrorCodeType nativeErrorCodeType;

			public ulong nativeErrorCode;

			public Binding.Baselib_SourceLocation sourceLocation;
		}

		public enum Baselib_ErrorState_ExplainVerbosity
		{
			ErrorType,
			ErrorType_SourceLocation_Explanation
		}

		public struct Baselib_Memory_PageSizeInfo
		{
			public ulong defaultPageSize;

			public ulong pageSizes0;

			public ulong pageSizes1;

			public ulong pageSizes2;

			public ulong pageSizes3;

			public ulong pageSizes4;

			public ulong pageSizes5;

			public ulong pageSizesLen;
		}

		public struct Baselib_Memory_PageAllocation
		{
			public IntPtr ptr;

			public ulong pageSize;

			public ulong pageCount;
		}

		public enum Baselib_Memory_PageState
		{
			Reserved,
			NoAccess,
			ReadOnly,
			ReadWrite = 4,
			ReadOnly_Executable = 18,
			ReadWrite_Executable = 20
		}

		public enum Baselib_NetworkAddress_Family
		{
			Invalid,
			IPv4,
			IPv6
		}

		public struct Baselib_NetworkAddress
		{
			public byte data0;

			public byte data1;

			public byte data2;

			public byte data3;

			public byte data4;

			public byte data5;

			public byte data6;

			public byte data7;

			public byte data8;

			public byte data9;

			public byte data10;

			public byte data11;

			public byte data12;

			public byte data13;

			public byte data14;

			public byte data15;

			public byte port0;

			public byte port1;

			public byte family;

			public byte _padding;
		}

		public enum Baselib_NetworkAddress_AddressReuse
		{
			DoNotAllow,
			Allow
		}

		public struct Baselib_RegisteredNetwork_Buffer
		{
			public IntPtr id;

			public Binding.Baselib_Memory_PageAllocation allocation;
		}

		public struct Baselib_RegisteredNetwork_BufferSlice
		{
			public IntPtr id;

			public IntPtr data;

			public uint size;

			public uint offset;
		}

		public struct Baselib_RegisteredNetwork_Endpoint
		{
			public Binding.Baselib_RegisteredNetwork_BufferSlice slice;
		}

		public struct Baselib_RegisteredNetwork_Request
		{
			public Binding.Baselib_RegisteredNetwork_BufferSlice payload;

			public Binding.Baselib_RegisteredNetwork_Endpoint remoteEndpoint;

			public IntPtr requestUserdata;
		}

		public enum Baselib_RegisteredNetwork_CompletionStatus
		{
			Failed,
			Success
		}

		public struct Baselib_RegisteredNetwork_CompletionResult
		{
			public Binding.Baselib_RegisteredNetwork_CompletionStatus status;

			public uint bytesTransferred;

			public IntPtr requestUserdata;
		}

		public struct Baselib_RegisteredNetwork_Socket_UDP
		{
			public IntPtr handle;
		}

		public enum Baselib_RegisteredNetwork_ProcessStatus
		{
			Done,
			Pending
		}

		public enum Baselib_RegisteredNetwork_CompletionQueueStatus
		{
			NoResultsAvailable,
			ResultsAvailable
		}

		public struct Baselib_SourceLocation
		{
			public unsafe byte* file;

			public unsafe byte* function;

			public uint lineNumber;
		}
	}
}
