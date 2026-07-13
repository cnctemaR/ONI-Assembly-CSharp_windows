using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace UnityEngine.Networking.PlayerConnection
{
	[Serializable]
	internal class PlayerEditorConnectionEvents
	{
		public IReadOnlyList<PlayerEditorConnectionEvents.MessageTypeSubscribers> messageTypeSubscribers
		{
			get
			{
				return this.m_MessageTypeSubscribers;
			}
		}

		private void BuildLookup()
		{
			bool flag = this.m_SubscriberLookup == null;
			if (flag)
			{
				this.m_SubscriberLookup = new Dictionary<Guid, PlayerEditorConnectionEvents.MessageTypeSubscribers>();
				foreach (PlayerEditorConnectionEvents.MessageTypeSubscribers messageTypeSubscribers in this.messageTypeSubscribers)
				{
					this.m_SubscriberLookup.Add(messageTypeSubscribers.MessageTypeId, messageTypeSubscribers);
				}
			}
		}

		public void InvokeMessageIdSubscribers(Guid messageId, byte[] data, int playerId)
		{
			this.BuildLookup();
			PlayerEditorConnectionEvents.MessageTypeSubscribers messageTypeSubscribers;
			bool flag = !this.m_SubscriberLookup.TryGetValue(messageId, out messageTypeSubscribers);
			if (flag)
			{
				string text = "No actions found for messageId: ";
				Guid guid = messageId;
				Debug.LogError(text + guid.ToString());
			}
			else
			{
				MessageEventArgs e = new MessageEventArgs
				{
					playerId = playerId,
					data = data
				};
				messageTypeSubscribers.messageCallback.Invoke(e);
			}
		}

		public UnityEvent<MessageEventArgs> AddAndCreate(Guid messageId)
		{
			this.BuildLookup();
			PlayerEditorConnectionEvents.MessageTypeSubscribers messageTypeSubscribers;
			bool flag = !this.m_SubscriberLookup.TryGetValue(messageId, out messageTypeSubscribers);
			if (flag)
			{
				messageTypeSubscribers = new PlayerEditorConnectionEvents.MessageTypeSubscribers
				{
					MessageTypeId = messageId,
					messageCallback = new PlayerEditorConnectionEvents.MessageEvent()
				};
				this.m_MessageTypeSubscribers.Add(messageTypeSubscribers);
				this.m_SubscriberLookup.Add(messageId, messageTypeSubscribers);
			}
			messageTypeSubscribers.subscriberCount++;
			return messageTypeSubscribers.messageCallback;
		}

		public void UnregisterManagedCallback(Guid messageId, UnityAction<MessageEventArgs> callback)
		{
			this.BuildLookup();
			PlayerEditorConnectionEvents.MessageTypeSubscribers messageTypeSubscribers;
			bool flag = !this.m_SubscriberLookup.TryGetValue(messageId, out messageTypeSubscribers);
			if (!flag)
			{
				messageTypeSubscribers.subscriberCount--;
				messageTypeSubscribers.messageCallback.RemoveListener(callback);
				bool flag2 = messageTypeSubscribers.subscriberCount <= 0;
				if (flag2)
				{
					this.m_MessageTypeSubscribers.Remove(messageTypeSubscribers);
					this.m_SubscriberLookup.Remove(messageId);
				}
			}
		}

		public void Clear()
		{
			bool flag = this.m_SubscriberLookup != null;
			if (flag)
			{
				this.m_SubscriberLookup.Clear();
			}
			this.m_MessageTypeSubscribers.Clear();
		}

		[SerializeField]
		private List<PlayerEditorConnectionEvents.MessageTypeSubscribers> m_MessageTypeSubscribers = new List<PlayerEditorConnectionEvents.MessageTypeSubscribers>();

		private Dictionary<Guid, PlayerEditorConnectionEvents.MessageTypeSubscribers> m_SubscriberLookup;

		[SerializeField]
		public PlayerEditorConnectionEvents.ConnectionChangeEvent connectionEvent = new PlayerEditorConnectionEvents.ConnectionChangeEvent();

		[SerializeField]
		public PlayerEditorConnectionEvents.ConnectionChangeEvent disconnectionEvent = new PlayerEditorConnectionEvents.ConnectionChangeEvent();

		[Serializable]
		public class MessageEvent : UnityEvent<MessageEventArgs>
		{
		}

		[Serializable]
		public class ConnectionChangeEvent : UnityEvent<int>
		{
		}

		[Serializable]
		public class MessageTypeSubscribers
		{
			public Guid MessageTypeId
			{
				get
				{
					return new Guid(this.m_messageTypeId);
				}
				set
				{
					this.m_messageTypeId = value.ToString();
				}
			}

			[SerializeField]
			private string m_messageTypeId;

			public int subscriberCount = 0;

			public PlayerEditorConnectionEvents.MessageEvent messageCallback = new PlayerEditorConnectionEvents.MessageEvent();
		}
	}
}
