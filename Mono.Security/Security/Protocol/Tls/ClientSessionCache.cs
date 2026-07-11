using System;
using System.Collections;

namespace Mono.Security.Protocol.Tls
{
	internal class ClientSessionCache
	{
		public static void Add(string host, byte[] id)
		{
			object obj = ClientSessionCache.locker;
			lock (obj)
			{
				string text = BitConverter.ToString(id);
				ClientSessionInfo clientSessionInfo = (ClientSessionInfo)ClientSessionCache.cache[text];
				if (clientSessionInfo == null)
				{
					ClientSessionCache.cache.Add(text, new ClientSessionInfo(host, id));
				}
				else if (clientSessionInfo.HostName == host)
				{
					clientSessionInfo.KeepAlive();
				}
				else
				{
					clientSessionInfo.Dispose();
					ClientSessionCache.cache.Remove(text);
					ClientSessionCache.cache.Add(text, new ClientSessionInfo(host, id));
				}
			}
		}

		public static byte[] FromHost(string host)
		{
			object obj = ClientSessionCache.locker;
			byte[] array;
			lock (obj)
			{
				foreach (object obj2 in ClientSessionCache.cache.Values)
				{
					ClientSessionInfo clientSessionInfo = (ClientSessionInfo)obj2;
					if (clientSessionInfo.HostName == host && clientSessionInfo.Valid)
					{
						clientSessionInfo.KeepAlive();
						return clientSessionInfo.Id;
					}
				}
				array = null;
			}
			return array;
		}

		private static ClientSessionInfo FromContext(Context context, bool checkValidity)
		{
			if (context == null)
			{
				return null;
			}
			byte[] sessionId = context.SessionId;
			if (sessionId == null || sessionId.Length == 0)
			{
				return null;
			}
			string text = BitConverter.ToString(sessionId);
			ClientSessionInfo clientSessionInfo = (ClientSessionInfo)ClientSessionCache.cache[text];
			if (clientSessionInfo == null)
			{
				return null;
			}
			if (context.ClientSettings.TargetHost != clientSessionInfo.HostName)
			{
				return null;
			}
			if (checkValidity && !clientSessionInfo.Valid)
			{
				clientSessionInfo.Dispose();
				ClientSessionCache.cache.Remove(text);
				return null;
			}
			return clientSessionInfo;
		}

		public static bool SetContextInCache(Context context)
		{
			object obj = ClientSessionCache.locker;
			bool flag2;
			lock (obj)
			{
				ClientSessionInfo clientSessionInfo = ClientSessionCache.FromContext(context, false);
				if (clientSessionInfo == null)
				{
					flag2 = false;
				}
				else
				{
					clientSessionInfo.GetContext(context);
					clientSessionInfo.KeepAlive();
					flag2 = true;
				}
			}
			return flag2;
		}

		public static bool SetContextFromCache(Context context)
		{
			object obj = ClientSessionCache.locker;
			bool flag2;
			lock (obj)
			{
				ClientSessionInfo clientSessionInfo = ClientSessionCache.FromContext(context, true);
				if (clientSessionInfo == null)
				{
					flag2 = false;
				}
				else
				{
					clientSessionInfo.SetContext(context);
					clientSessionInfo.KeepAlive();
					flag2 = true;
				}
			}
			return flag2;
		}

		private static Hashtable cache = new Hashtable();

		private static object locker = new object();
	}
}
