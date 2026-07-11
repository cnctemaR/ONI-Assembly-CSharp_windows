using System;
using System.Collections.Generic;
using KSerialization;

public class Messenger : KMonoBehaviour
{
	public int Count
	{
		get
		{
			return this.messages.Count;
		}
	}

	public IEnumerator<Message> GetEnumerator()
	{
		return this.messages.GetEnumerator();
	}

	public static void DestroyInstance()
	{
		Messenger.Instance = null;
	}

	public SerializedList<Message> Messages
	{
		get
		{
			return this.messages;
		}
	}

	protected override void OnPrefabInit()
	{
		Messenger.Instance = this;
	}

	protected override void OnSpawn()
	{
		int i = 0;
		while (i < this.messages.Count)
		{
			if (this.messages[i].IsValid())
			{
				i++;
			}
			else
			{
				this.messages.RemoveAt(i);
			}
		}
		base.Trigger(-599791736, null);
	}

	public void QueueMessage(Message message)
	{
		this.messages.Add(message);
		base.Trigger(1558809273, message);
	}

	public Message DequeueMessage()
	{
		Message message = null;
		if (this.messages.Count > 0)
		{
			message = this.messages[0];
			this.messages.RemoveAt(0);
		}
		return message;
	}

	public void ClearAllMessages()
	{
		for (int i = this.messages.Count - 1; i >= 0; i--)
		{
			this.messages.RemoveAt(i);
		}
	}

	public void RemoveMessage(Message m)
	{
		this.messages.Remove(m);
		base.Trigger(-599791736, null);
	}

	[Serialize]
	private SerializedList<Message> messages = new SerializedList<Message>();

	public static Messenger Instance;
}
