using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode(Optional = true)]
	public struct NetworkPlayer
	{
		public NetworkPlayer(string ip, int port)
		{
			Debug.LogError("Not yet implemented");
			this.index = 0;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string Internal_GetIPAddress(int index);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetPort(int index);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string Internal_GetExternalIP();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetExternalPort();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string Internal_GetLocalIP();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetLocalPort();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetPlayerIndex();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string Internal_GetGUID(int index);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string Internal_GetLocalGUID();

		public static bool operator ==(NetworkPlayer lhs, NetworkPlayer rhs)
		{
			return lhs.index == rhs.index;
		}

		public static bool operator !=(NetworkPlayer lhs, NetworkPlayer rhs)
		{
			return lhs.index != rhs.index;
		}

		public override int GetHashCode()
		{
			return this.index.GetHashCode();
		}

		public override bool Equals(object other)
		{
			return other is NetworkPlayer && ((NetworkPlayer)other).index == this.index;
		}

		public string ipAddress
		{
			get
			{
				string text;
				if (this.index == NetworkPlayer.Internal_GetPlayerIndex())
				{
					text = NetworkPlayer.Internal_GetLocalIP();
				}
				else
				{
					text = NetworkPlayer.Internal_GetIPAddress(this.index);
				}
				return text;
			}
		}

		public int port
		{
			get
			{
				int num;
				if (this.index == NetworkPlayer.Internal_GetPlayerIndex())
				{
					num = NetworkPlayer.Internal_GetLocalPort();
				}
				else
				{
					num = NetworkPlayer.Internal_GetPort(this.index);
				}
				return num;
			}
		}

		public string guid
		{
			get
			{
				string text;
				if (this.index == NetworkPlayer.Internal_GetPlayerIndex())
				{
					text = NetworkPlayer.Internal_GetLocalGUID();
				}
				else
				{
					text = NetworkPlayer.Internal_GetGUID(this.index);
				}
				return text;
			}
		}

		public override string ToString()
		{
			return this.index.ToString();
		}

		public string externalIP
		{
			get
			{
				return NetworkPlayer.Internal_GetExternalIP();
			}
		}

		public int externalPort
		{
			get
			{
				return NetworkPlayer.Internal_GetExternalPort();
			}
		}

		internal static NetworkPlayer unassigned
		{
			get
			{
				NetworkPlayer networkPlayer;
				networkPlayer.index = -1;
				return networkPlayer;
			}
		}

		internal int index;
	}
}
