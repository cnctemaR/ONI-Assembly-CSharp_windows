using System;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;

namespace UnityEngine.Networking
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Network/NetworkLobbyPlayer")]
	[Obsolete("The high level API classes are deprecated and will be removed in the future.")]
	public class NetworkLobbyPlayer : NetworkBehaviour
	{
		public byte slot
		{
			get
			{
				return this.m_Slot;
			}
			set
			{
				this.m_Slot = value;
			}
		}

		public bool readyToBegin
		{
			get
			{
				return this.m_ReadyToBegin;
			}
			set
			{
				this.m_ReadyToBegin = value;
			}
		}

		private void Start()
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}

		private void OnEnable()
		{
			SceneManager.sceneLoaded += this.OnSceneLoaded;
		}

		private void OnDisable()
		{
			SceneManager.sceneLoaded -= this.OnSceneLoaded;
		}

		public override void OnStartClient()
		{
			NetworkLobbyManager lobbyManager = this.GetLobbyManager();
			if (lobbyManager)
			{
				lobbyManager.lobbySlots[(int)this.m_Slot] = this;
				this.m_ReadyToBegin = false;
				this.OnClientEnterLobby();
			}
			else
			{
				Debug.LogError("LobbyPlayer could not find a NetworkLobbyManager. The LobbyPlayer requires a NetworkLobbyManager object to function. Make sure that there is one in the scene.");
			}
		}

		public void SendReadyToBeginMessage()
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("NetworkLobbyPlayer SendReadyToBeginMessage");
			}
			NetworkLobbyManager lobbyManager = this.GetLobbyManager();
			if (lobbyManager)
			{
				LobbyReadyToBeginMessage lobbyReadyToBeginMessage = new LobbyReadyToBeginMessage();
				lobbyReadyToBeginMessage.slotId = (byte)base.playerControllerId;
				lobbyReadyToBeginMessage.readyState = true;
				lobbyManager.client.Send(43, lobbyReadyToBeginMessage);
			}
		}

		public void SendNotReadyToBeginMessage()
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("NetworkLobbyPlayer SendReadyToBeginMessage");
			}
			NetworkLobbyManager lobbyManager = this.GetLobbyManager();
			if (lobbyManager)
			{
				LobbyReadyToBeginMessage lobbyReadyToBeginMessage = new LobbyReadyToBeginMessage();
				lobbyReadyToBeginMessage.slotId = (byte)base.playerControllerId;
				lobbyReadyToBeginMessage.readyState = false;
				lobbyManager.client.Send(43, lobbyReadyToBeginMessage);
			}
		}

		public void SendSceneLoadedMessage()
		{
			if (LogFilter.logDebug)
			{
				Debug.Log("NetworkLobbyPlayer SendSceneLoadedMessage");
			}
			NetworkLobbyManager lobbyManager = this.GetLobbyManager();
			if (lobbyManager)
			{
				IntegerMessage integerMessage = new IntegerMessage((int)base.playerControllerId);
				lobbyManager.client.Send(44, integerMessage);
			}
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			NetworkLobbyManager lobbyManager = this.GetLobbyManager();
			if (lobbyManager)
			{
				string name = scene.name;
				if (name == lobbyManager.lobbyScene)
				{
					return;
				}
			}
			if (base.isLocalPlayer)
			{
				this.SendSceneLoadedMessage();
			}
		}

		private NetworkLobbyManager GetLobbyManager()
		{
			return NetworkManager.singleton as NetworkLobbyManager;
		}

		public void RemovePlayer()
		{
			if (base.isLocalPlayer && !this.m_ReadyToBegin)
			{
				if (LogFilter.logDebug)
				{
					Debug.Log("NetworkLobbyPlayer RemovePlayer");
				}
				ClientScene.RemovePlayer(base.GetComponent<NetworkIdentity>().playerControllerId);
			}
		}

		public virtual void OnClientEnterLobby()
		{
		}

		public virtual void OnClientExitLobby()
		{
		}

		public virtual void OnClientReady(bool readyState)
		{
		}

		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			writer.WritePackedUInt32(1U);
			writer.Write(this.m_Slot);
			writer.Write(this.m_ReadyToBegin);
			return true;
		}

		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (reader.ReadPackedUInt32() != 0U)
			{
				this.m_Slot = reader.ReadByte();
				this.m_ReadyToBegin = reader.ReadBoolean();
			}
		}

		private void OnGUI()
		{
			if (this.ShowLobbyGUI)
			{
				NetworkLobbyManager lobbyManager = this.GetLobbyManager();
				if (lobbyManager)
				{
					if (!lobbyManager.showLobbyGUI)
					{
						return;
					}
					string name = SceneManager.GetSceneAt(0).name;
					if (name != lobbyManager.lobbyScene)
					{
						return;
					}
				}
				Rect rect = new Rect((float)(100 + this.m_Slot * 100), 200f, 90f, 20f);
				if (base.isLocalPlayer)
				{
					string text;
					if (this.m_ReadyToBegin)
					{
						text = "(Ready)";
					}
					else
					{
						text = "(Not Ready)";
					}
					GUI.Label(rect, text);
					if (this.m_ReadyToBegin)
					{
						rect.y += 25f;
						if (GUI.Button(rect, "STOP"))
						{
							this.SendNotReadyToBeginMessage();
						}
					}
					else
					{
						rect.y += 25f;
						if (GUI.Button(rect, "START"))
						{
							this.SendReadyToBeginMessage();
						}
						rect.y += 25f;
						if (GUI.Button(rect, "Remove"))
						{
							ClientScene.RemovePlayer(base.GetComponent<NetworkIdentity>().playerControllerId);
						}
					}
				}
				else
				{
					GUI.Label(rect, "Player [" + base.netId + "]");
					rect.y += 25f;
					GUI.Label(rect, "Ready [" + this.m_ReadyToBegin + "]");
				}
			}
		}

		[Tooltip("Enable to show the default lobby GUI for this player.")]
		[SerializeField]
		public bool ShowLobbyGUI = true;

		private byte m_Slot;

		private bool m_ReadyToBegin;
	}
}
