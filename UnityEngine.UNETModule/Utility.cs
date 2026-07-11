using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.Networking.Types;

namespace UnityEngine.Networking
{
	public class Utility
	{
		private Utility()
		{
		}

		[Obsolete("This property is unused and should not be referenced in code.", true)]
		public static bool useRandomSourceID
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public static SourceID GetSourceID()
		{
			return (SourceID)((long)SystemInfo.deviceUniqueIdentifier.GetHashCode());
		}

		[Obsolete("This function is unused and should not be referenced in code. Please sign in and setup your project in the editor instead.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetAppID(AppID newAppID)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This function is unused and should not be referenced in code. Please sign in and setup your project in the editor instead.", true)]
		public static AppID GetAppID()
		{
			return AppID.Invalid;
		}

		public static void SetAccessTokenForNetwork(NetworkID netId, NetworkAccessToken accessToken)
		{
			if (Utility.s_dictTokens.ContainsKey(netId))
			{
				Utility.s_dictTokens.Remove(netId);
			}
			Utility.s_dictTokens.Add(netId, accessToken);
		}

		public static NetworkAccessToken GetAccessTokenForNetwork(NetworkID netId)
		{
			NetworkAccessToken networkAccessToken;
			if (!Utility.s_dictTokens.TryGetValue(netId, out networkAccessToken))
			{
				networkAccessToken = new NetworkAccessToken();
			}
			return networkAccessToken;
		}

		private static Dictionary<NetworkID, NetworkAccessToken> s_dictTokens = new Dictionary<NetworkID, NetworkAccessToken>();
	}
}
