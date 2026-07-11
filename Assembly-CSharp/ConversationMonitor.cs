using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class ConversationMonitor : GameStateMachine<ConversationMonitor, ConversationMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.EventHandler(GameHashes.TopicDiscussed, delegate(ConversationMonitor.Instance smi, object obj)
		{
			smi.OnTopicDiscussed(obj);
		}).EventHandler(GameHashes.TopicDiscovered, delegate(ConversationMonitor.Instance smi, object obj)
		{
			smi.OnTopicDiscovered(obj);
		});
	}

	private const int MAX_RECENT_TOPICS = 5;

	private const int MAX_FAVOURITE_TOPICS = 5;

	private const float FAVOURITE_CHANCE = 0.033333335f;

	private const float LEARN_CHANCE = 0.33333334f;

	public class Def : StateMachine.BaseDef
	{
	}

	[SerializationConfig(MemberSerialization.OptIn)]
	public new class Instance : GameStateMachine<ConversationMonitor, ConversationMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master, ConversationMonitor.Def def)
			: base(master)
		{
			this.recentTopics = new Queue<string>();
			this.favouriteTopics = new List<string>();
			this.personalTopics = new List<string> { ConversationMonitor.Instance.randomTopics[global::UnityEngine.Random.Range(0, ConversationMonitor.Instance.randomTopics.Count)] };
		}

		public string GetATopic()
		{
			int num = this.recentTopics.Count + this.favouriteTopics.Count * 2 + this.personalTopics.Count;
			int num2 = global::UnityEngine.Random.Range(0, num);
			if (num2 < this.recentTopics.Count)
			{
				return this.recentTopics.Dequeue();
			}
			num2 -= this.recentTopics.Count;
			if (num2 < this.favouriteTopics.Count)
			{
				return this.favouriteTopics[num2];
			}
			num2 -= this.favouriteTopics.Count;
			if (num2 < this.favouriteTopics.Count)
			{
				return this.favouriteTopics[num2];
			}
			num2 -= this.favouriteTopics.Count;
			return this.personalTopics[num2];
		}

		public void OnTopicDiscovered(object data)
		{
			string text = (string)data;
			if (!this.recentTopics.Contains(text))
			{
				this.recentTopics.Enqueue(text);
				if (this.recentTopics.Count > 5)
				{
					string text2 = this.recentTopics.Dequeue();
					this.TryMakeFavouriteTopic(text2);
				}
			}
		}

		public void OnTopicDiscussed(object data)
		{
			string text = (string)data;
			if (global::UnityEngine.Random.value < 0.33333334f)
			{
				this.OnTopicDiscovered(text);
			}
		}

		private void TryMakeFavouriteTopic(string topic)
		{
			if (global::UnityEngine.Random.value < 0.033333335f)
			{
				if (this.favouriteTopics.Count < 5)
				{
					this.favouriteTopics.Add(topic);
				}
				else
				{
					this.favouriteTopics[global::UnityEngine.Random.Range(0, this.favouriteTopics.Count)] = topic;
				}
			}
		}

		[Serialize]
		private Queue<string> recentTopics;

		[Serialize]
		private List<string> favouriteTopics;

		private List<string> personalTopics;

		private static readonly List<string> randomTopics = new List<string> { "ManualGenerator", "Hatch", "Mushbar", "FriedMushbar" };
	}
}
