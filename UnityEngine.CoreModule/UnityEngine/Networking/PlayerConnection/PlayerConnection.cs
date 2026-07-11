using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine.Events;
using UnityEngine.Scripting;

namespace UnityEngine.Networking.PlayerConnection
{
	[Serializable]
	public class PlayerConnection : ScriptableObject, IEditorPlayerConnection
	{
		public static PlayerConnection instance
		{
			get
			{
				PlayerConnection playerConnection;
				if (PlayerConnection.s_Instance == null)
				{
					playerConnection = PlayerConnection.CreateInstance();
				}
				else
				{
					playerConnection = PlayerConnection.s_Instance;
				}
				return playerConnection;
			}
		}

		public bool isConnected
		{
			get
			{
				return this.GetConnectionNativeApi().IsConnected();
			}
		}

		private static PlayerConnection CreateInstance()
		{
			PlayerConnection.s_Instance = ScriptableObject.CreateInstance<PlayerConnection>();
			PlayerConnection.s_Instance.hideFlags = HideFlags.HideAndDontSave;
			return PlayerConnection.s_Instance;
		}

		public void OnEnable()
		{
			if (!this.m_IsInitilized)
			{
				this.m_IsInitilized = true;
				this.GetConnectionNativeApi().Initialize();
			}
		}

		private IPlayerEditorConnectionNative GetConnectionNativeApi()
		{
			return PlayerConnection.connectionNative ?? new PlayerConnectionInternal();
		}

		public void Register(Guid messageId, UnityAction<MessageEventArgs> callback)
		{
			if (messageId == Guid.Empty)
			{
				throw new ArgumentException("Cant be Guid.Empty", "messageId");
			}
			if (!this.m_PlayerEditorConnectionEvents.messageTypeSubscribers.Any<PlayerEditorConnectionEvents.MessageTypeSubscribers>((PlayerEditorConnectionEvents.MessageTypeSubscribers x) => x.MessageTypeId == messageId))
			{
				this.GetConnectionNativeApi().RegisterInternal(messageId);
			}
			this.m_PlayerEditorConnectionEvents.AddAndCreate(messageId).AddListener(callback);
		}

		public void Unregister(Guid messageId, UnityAction<MessageEventArgs> callback)
		{
			this.m_PlayerEditorConnectionEvents.UnregisterManagedCallback(messageId, callback);
			if (!this.m_PlayerEditorConnectionEvents.messageTypeSubscribers.Any<PlayerEditorConnectionEvents.MessageTypeSubscribers>((PlayerEditorConnectionEvents.MessageTypeSubscribers x) => x.MessageTypeId == messageId))
			{
				this.GetConnectionNativeApi().UnregisterInternal(messageId);
			}
		}

		public void RegisterConnection(UnityAction<int> callback)
		{
			foreach (int num in this.m_connectedPlayers)
			{
				callback(num);
			}
			this.m_PlayerEditorConnectionEvents.connectionEvent.AddListener(callback);
		}

		public void RegisterDisconnection(UnityAction<int> callback)
		{
			this.m_PlayerEditorConnectionEvents.disconnectionEvent.AddListener(callback);
		}

		public void Send(Guid messageId, byte[] data)
		{
			if (messageId == Guid.Empty)
			{
				throw new ArgumentException("Cant be Guid.Empty", "messageId");
			}
			this.GetConnectionNativeApi().SendMessage(messageId, data, 0);
		}

		public bool TrySend(Guid messageId, byte[] data)
		{
			if (messageId == Guid.Empty)
			{
				throw new ArgumentException("Cant be Guid.Empty", "messageId");
			}
			return this.GetConnectionNativeApi().TrySendMessage(messageId, data, 0);
		}

		public bool BlockUntilRecvMsg(Guid messageId, int timeout)
		{
			bool msgReceived = false;
			UnityAction<MessageEventArgs> unityAction = delegate(MessageEventArgs args)
			{
				msgReceived = true;
			};
			DateTime now = DateTime.Now;
			this.Register(messageId, unityAction);
			while ((DateTime.Now - now).TotalMilliseconds < (double)timeout && !msgReceived)
			{
				this.GetConnectionNativeApi().Poll();
			}
			this.Unregister(messageId, unityAction);
			return msgReceived;
		}

		public void DisconnectAll()
		{
			this.GetConnectionNativeApi().DisconnectAll();
		}

		[RequiredByNativeCode]
		private static void MessageCallbackInternal(IntPtr data, ulong size, ulong guid, string messageId)
		{
			byte[] array = null;
			if (size > 0UL)
			{
				array = new byte[size];
				Marshal.Copy(data, array, 0, (int)size);
			}
			PlayerConnection.instance.m_PlayerEditorConnectionEvents.InvokeMessageIdSubscribers(new Guid(messageId), array, (int)guid);
		}

		[RequiredByNativeCode]
		private static void ConnectedCallbackInternal(int playerId)
		{
			PlayerConnection.instance.m_connectedPlayers.Add(playerId);
			PlayerConnection.instance.m_PlayerEditorConnectionEvents.connectionEvent.Invoke(playerId);
		}

		[RequiredByNativeCode]
		private static void DisconnectedCallback(int playerId)
		{
			PlayerConnection.instance.m_connectedPlayers.Remove(playerId);
			PlayerConnection.instance.m_PlayerEditorConnectionEvents.disconnectionEvent.Invoke(playerId);
		}

		internal static IPlayerEditorConnectionNative connectionNative;

		[SerializeField]
		private PlayerEditorConnectionEvents m_PlayerEditorConnectionEvents = new PlayerEditorConnectionEvents();

		[SerializeField]
		private List<int> m_connectedPlayers = new List<int>();

		private bool m_IsInitilized;

		private static PlayerConnection s_Instance;
	}
}
