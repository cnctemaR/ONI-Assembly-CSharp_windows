using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/PlayerConnection/PlayerConnectionInternal.bindings.h")]
	internal class PlayerConnectionInternal : IPlayerEditorConnectionNative
	{
		void IPlayerEditorConnectionNative.SendMessage(Guid messageId, byte[] data, int playerId)
		{
			bool flag = messageId == Guid.Empty;
			if (flag)
			{
				throw new ArgumentException("messageId must not be empty");
			}
			PlayerConnectionInternal.SendMessage(messageId.ToString("N"), data, playerId);
		}

		bool IPlayerEditorConnectionNative.TrySendMessage(Guid messageId, byte[] data, int playerId)
		{
			bool flag = messageId == Guid.Empty;
			if (flag)
			{
				throw new ArgumentException("messageId must not be empty");
			}
			return PlayerConnectionInternal.TrySendMessage(messageId.ToString("N"), data, playerId);
		}

		void IPlayerEditorConnectionNative.Poll()
		{
			PlayerConnectionInternal.PollInternal();
		}

		void IPlayerEditorConnectionNative.RegisterInternal(Guid messageId)
		{
			PlayerConnectionInternal.RegisterInternal(messageId.ToString("N"));
		}

		void IPlayerEditorConnectionNative.UnregisterInternal(Guid messageId)
		{
			PlayerConnectionInternal.UnregisterInternal(messageId.ToString("N"));
		}

		void IPlayerEditorConnectionNative.Initialize()
		{
			PlayerConnectionInternal.Initialize();
		}

		bool IPlayerEditorConnectionNative.IsConnected()
		{
			return PlayerConnectionInternal.IsConnected();
		}

		void IPlayerEditorConnectionNative.DisconnectAll()
		{
			PlayerConnectionInternal.DisconnectAll();
		}

		[FreeFunction("PlayerConnection_Bindings::IsConnected")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsConnected();

		[FreeFunction("PlayerConnection_Bindings::Initialize")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Initialize();

		[FreeFunction("PlayerConnection_Bindings::RegisterInternal")]
		private unsafe static void RegisterInternal(string messageId)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(messageId, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = messageId.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				PlayerConnectionInternal.RegisterInternal_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("PlayerConnection_Bindings::UnregisterInternal")]
		private unsafe static void UnregisterInternal(string messageId)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(messageId, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = messageId.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				PlayerConnectionInternal.UnregisterInternal_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("PlayerConnection_Bindings::SendMessage")]
		private unsafe static void SendMessage(string messageId, byte[] data, int playerId)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(messageId, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = messageId.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Span<byte> span = new Span<byte>(data);
				fixed (byte* ptr2 = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, span.Length);
					PlayerConnectionInternal.SendMessage_Injected(ref managedSpanWrapper, ref managedSpanWrapper2, playerId);
				}
			}
			finally
			{
				char* ptr = null;
				byte* ptr2 = null;
			}
		}

		[FreeFunction("PlayerConnection_Bindings::TrySendMessage")]
		private unsafe static bool TrySendMessage(string messageId, byte[] data, int playerId)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(messageId, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = messageId.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Span<byte> span = new Span<byte>(data);
				fixed (byte* ptr2 = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, span.Length);
					flag = PlayerConnectionInternal.TrySendMessage_Injected(ref managedSpanWrapper, ref managedSpanWrapper2, playerId);
				}
			}
			finally
			{
				char* ptr = null;
				byte* ptr2 = null;
			}
			return flag;
		}

		[FreeFunction("PlayerConnection_Bindings::PollInternal")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PollInternal();

		[FreeFunction("PlayerConnection_Bindings::DisconnectAll")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisconnectAll();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RegisterInternal_Injected(ref ManagedSpanWrapper messageId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UnregisterInternal_Injected(ref ManagedSpanWrapper messageId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SendMessage_Injected(ref ManagedSpanWrapper messageId, ref ManagedSpanWrapper data, int playerId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TrySendMessage_Injected(ref ManagedSpanWrapper messageId, ref ManagedSpanWrapper data, int playerId);

		[Flags]
		public enum MulticastFlags
		{
			kRequestImmediateConnect = 1,
			kSupportsProfile = 2,
			kCustomMessage = 4,
			kUseAlternateIP = 8
		}
	}
}
