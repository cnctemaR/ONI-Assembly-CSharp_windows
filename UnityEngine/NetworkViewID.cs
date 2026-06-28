using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode(Optional = true)]
	public struct NetworkViewID
	{
		public static NetworkViewID unassigned
		{
			get
			{
				NetworkViewID networkViewID;
				NetworkViewID.INTERNAL_get_unassigned(out networkViewID);
				return networkViewID;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_unassigned(out NetworkViewID value);

		internal static bool Internal_IsMine(NetworkViewID value)
		{
			return NetworkViewID.INTERNAL_CALL_Internal_IsMine(ref value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Internal_IsMine(ref NetworkViewID value);

		internal static void Internal_GetOwner(NetworkViewID value, out NetworkPlayer player)
		{
			NetworkViewID.INTERNAL_CALL_Internal_GetOwner(ref value, out player);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_GetOwner(ref NetworkViewID value, out NetworkPlayer player);

		internal static string Internal_GetString(NetworkViewID value)
		{
			return NetworkViewID.INTERNAL_CALL_Internal_GetString(ref value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string INTERNAL_CALL_Internal_GetString(ref NetworkViewID value);

		internal static bool Internal_Compare(NetworkViewID lhs, NetworkViewID rhs)
		{
			return NetworkViewID.INTERNAL_CALL_Internal_Compare(ref lhs, ref rhs);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Internal_Compare(ref NetworkViewID lhs, ref NetworkViewID rhs);

		public static bool operator ==(NetworkViewID lhs, NetworkViewID rhs)
		{
			return NetworkViewID.Internal_Compare(lhs, rhs);
		}

		public static bool operator !=(NetworkViewID lhs, NetworkViewID rhs)
		{
			return !NetworkViewID.Internal_Compare(lhs, rhs);
		}

		public override int GetHashCode()
		{
			return this.a ^ this.b ^ this.c;
		}

		public override bool Equals(object other)
		{
			bool flag;
			if (!(other is NetworkViewID))
			{
				flag = false;
			}
			else
			{
				NetworkViewID networkViewID = (NetworkViewID)other;
				flag = NetworkViewID.Internal_Compare(this, networkViewID);
			}
			return flag;
		}

		public bool isMine
		{
			get
			{
				return NetworkViewID.Internal_IsMine(this);
			}
		}

		public NetworkPlayer owner
		{
			get
			{
				NetworkPlayer networkPlayer;
				NetworkViewID.Internal_GetOwner(this, out networkPlayer);
				return networkPlayer;
			}
		}

		public override string ToString()
		{
			return NetworkViewID.Internal_GetString(this);
		}

		private int a;

		private int b;

		private int c;
	}
}
