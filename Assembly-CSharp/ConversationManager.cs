using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ConversationManager")]
public class ConversationManager : KMonoBehaviour, ISim200ms
{
	protected override void OnPrefabInit()
	{
		this.conversations = new List<Conversation>();
		this.lastConvoTimeByMinion = new Dictionary<MinionIdentity, float>();
		this.simRenderLoadBalance = true;
	}

	public void Sim200ms(float dt)
	{
		for (int i = this.conversations.Count - 1; i >= 0; i--)
		{
			Conversation conversation = this.conversations[i];
			for (int j = conversation.minions.Count - 1; j >= 0; j--)
			{
				MinionIdentity minionIdentity = conversation.minions[j];
				if (!this.ValidMinionTags(minionIdentity) || !this.MinionCloseEnoughToConvo(minionIdentity, conversation))
				{
					conversation.minions.RemoveAt(j);
					if (conversation.lastTalked == minionIdentity)
					{
						conversation.lastTalked = null;
					}
				}
				else
				{
					this.minionConversations[minionIdentity] = conversation;
				}
			}
			if (conversation.minions.Count <= 1)
			{
				this.conversations.RemoveAt(i);
			}
			else if (!(conversation.lastTalked != null) || !conversation.lastTalked.GetComponent<KPrefabID>().HasTag(GameTags.DoNotInterruptMe))
			{
				bool flag = conversation.minions.Find((MinionIdentity match) => match.HasTag(GameTags.CommunalDining)) != null;
				bool flag2 = true;
				if (!flag && conversation.numUtterances >= TuningData<ConversationManager.Tuning>.Get().maxUtterances)
				{
					flag2 = false;
				}
				else
				{
					bool flag3 = conversation.numUtterances == 0;
					bool flag4 = conversation.minions.Find((MinionIdentity match) => !match.HasTag(GameTags.Partying)) == null;
					float num = (flag3 ? TuningData<ConversationManager.Tuning>.Get().delayBeforeStart : TuningData<ConversationManager.Tuning>.Get().delayBetweenUtterances);
					if (flag4)
					{
						num = 0f;
					}
					float num2 = (flag3 ? 0f : TuningData<ConversationManager.Tuning>.Get().speakTime);
					if (flag4)
					{
						num2 /= 4f;
					}
					num2 += num;
					if (GameClock.Instance.GetTime() > conversation.lastTalkedTime + num2)
					{
						flag2 = this.TryContinueConversation(conversation, flag3);
					}
				}
				if (!flag2)
				{
					this.conversations.RemoveAt(i);
				}
			}
		}
		foreach (MinionIdentity minionIdentity2 in Components.LiveMinionIdentities.Items)
		{
			if (this.ValidMinionTags(minionIdentity2) && !this.minionConversations.ContainsKey(minionIdentity2) && !this.MinionOnCooldown(minionIdentity2))
			{
				foreach (MinionIdentity minionIdentity3 in Components.LiveMinionIdentities.Items)
				{
					if (!(minionIdentity3 == minionIdentity2) && this.ValidMinionTags(minionIdentity3))
					{
						Conversation conversation2;
						if (this.minionConversations.TryGetValue(minionIdentity3, out conversation2))
						{
							if (conversation2.minions.Count < TuningData<ConversationManager.Tuning>.Get().maxDupesPerConvo && (this.GetCentroid(conversation2) - minionIdentity2.transform.GetPosition()).magnitude < TuningData<ConversationManager.Tuning>.Get().maxDistance * 0.5f)
							{
								conversation2.minions.Add(minionIdentity2);
								this.minionConversations[minionIdentity2] = conversation2;
								break;
							}
						}
						else if (!this.MinionOnCooldown(minionIdentity3) && (minionIdentity3.transform.GetPosition() - minionIdentity2.transform.GetPosition()).magnitude < TuningData<ConversationManager.Tuning>.Get().maxDistance)
						{
							conversation2 = new Conversation();
							conversation2.minions.Add(minionIdentity2);
							conversation2.minions.Add(minionIdentity3);
							Type type = this.convoTypes[global::UnityEngine.Random.Range(0, this.convoTypes.Count)];
							conversation2.conversationType = (ConversationType)Activator.CreateInstance(type);
							conversation2.lastTalkedTime = GameClock.Instance.GetTime();
							this.conversations.Add(conversation2);
							this.minionConversations[minionIdentity2] = conversation2;
							this.minionConversations[minionIdentity3] = conversation2;
							break;
						}
					}
				}
			}
		}
		this.minionConversations.Clear();
	}

	private bool TryContinueConversation(Conversation conversation, bool isOpeningLine)
	{
		ListPool<int, ConversationManager>.PooledList pooledList = ListPool<int, ConversationManager>.Allocate();
		int num = -1;
		pooledList.Capacity = Math.Max(pooledList.Capacity, conversation.minions.Count);
		for (int num2 = 0; num2 != conversation.minions.Count; num2++)
		{
			if (conversation.minions[num2] == conversation.lastTalked)
			{
				num = num2;
			}
			else
			{
				pooledList.Add(num2);
			}
		}
		pooledList.Shuffle<int>();
		if (num != -1)
		{
			pooledList.Add(num);
		}
		if (isOpeningLine)
		{
			MinionIdentity minionIdentity = conversation.minions[pooledList[0]];
			conversation.conversationType.NewTarget(minionIdentity);
		}
		bool flag = false;
		foreach (int num3 in pooledList)
		{
			MinionIdentity minionIdentity2 = conversation.minions[num3];
			if (this.DoTalking(conversation, minionIdentity2))
			{
				flag = true;
				break;
			}
		}
		pooledList.Recycle();
		return flag;
	}

	private bool DoTalking(Conversation conversation, MinionIdentity new_speaker)
	{
		DebugUtil.Assert(conversation != null, "conversation was null");
		DebugUtil.Assert(new_speaker != null, "new_speaker was null");
		DebugUtil.Assert(conversation.conversationType != null, "conversation.conversationType was null");
		Conversation.Topic nextTopic = conversation.conversationType.GetNextTopic(new_speaker, conversation.lastTopic);
		if (nextTopic == null || nextTopic.mode == Conversation.ModeType.End)
		{
			return false;
		}
		Thought thoughtForTopic = this.GetThoughtForTopic(conversation, nextTopic);
		if (thoughtForTopic == null)
		{
			return false;
		}
		ThoughtGraph.Instance smi = new_speaker.GetSMI<ThoughtGraph.Instance>();
		if (smi == null)
		{
			return false;
		}
		if (conversation.lastTalked != null)
		{
			conversation.lastTalked.Trigger(25860745, conversation.lastTalked.gameObject);
		}
		smi.AddThought(thoughtForTopic);
		conversation.lastTopic = nextTopic;
		conversation.lastTalked = new_speaker;
		conversation.lastTalkedTime = GameClock.Instance.GetTime();
		DebugUtil.Assert(this.lastConvoTimeByMinion != null, "lastConvoTimeByMinion was null");
		this.lastConvoTimeByMinion[conversation.lastTalked] = GameClock.Instance.GetTime();
		Effects component = conversation.lastTalked.GetComponent<Effects>();
		DebugUtil.Assert(component != null, "effects was null");
		component.Add("GoodConversation", true);
		Conversation.Mode mode = Conversation.Topic.Modes[(int)nextTopic.mode];
		DebugUtil.Assert(mode != null, "mode was null");
		ConversationManager.StartedTalkingEvent startedTalkingEvent = new ConversationManager.StartedTalkingEvent
		{
			talker = new_speaker.gameObject,
			anim = mode.anim
		};
		foreach (MinionIdentity minionIdentity in conversation.minions)
		{
			if (!minionIdentity)
			{
				DebugUtil.DevAssert(false, "minion in conversation.minions was null", null);
			}
			else
			{
				minionIdentity.Trigger(-594200555, startedTalkingEvent);
			}
		}
		conversation.numUtterances++;
		return true;
	}

	public bool TryGetConversation(MinionIdentity minion, out Conversation conversation)
	{
		return this.minionConversations.TryGetValue(minion, out conversation);
	}

	private Vector3 GetCentroid(Conversation conversation)
	{
		Vector3 vector = Vector3.zero;
		foreach (MinionIdentity minionIdentity in conversation.minions)
		{
			if (!(minionIdentity == null))
			{
				vector += minionIdentity.transform.GetPosition();
			}
		}
		return vector / (float)conversation.minions.Count;
	}

	private Thought GetThoughtForTopic(Conversation conversation, Conversation.Topic topic)
	{
		if (string.IsNullOrEmpty(topic.topic))
		{
			DebugUtil.DevAssert(false, "topic.topic was null", null);
			return null;
		}
		Sprite sprite = conversation.conversationType.GetSprite(topic.topic);
		if (sprite != null)
		{
			Conversation.Mode mode = Conversation.Topic.Modes[(int)topic.mode];
			return new Thought("Topic_" + topic.topic, null, sprite, mode.icon, mode.voice, "bubble_chatter", mode.mouth, DUPLICANTS.THOUGHTS.CONVERSATION.TOOLTIP, true, TuningData<ConversationManager.Tuning>.Get().speakTime);
		}
		return null;
	}

	private bool ValidMinionTags(MinionIdentity minion)
	{
		return !(minion == null) && !minion.GetComponent<KPrefabID>().HasAnyTags(ConversationManager.invalidConvoTags);
	}

	private bool MinionCloseEnoughToConvo(MinionIdentity minion, Conversation conversation)
	{
		return (this.GetCentroid(conversation) - minion.transform.GetPosition()).magnitude < TuningData<ConversationManager.Tuning>.Get().maxDistance * 0.5f;
	}

	private bool MinionOnCooldown(MinionIdentity minion)
	{
		if (minion.GetComponent<KPrefabID>().HasTag(GameTags.AlwaysConverse))
		{
			return false;
		}
		float num;
		if (!this.lastConvoTimeByMinion.TryGetValue(minion, out num))
		{
			return false;
		}
		float num2 = GameClock.Instance.GetTime() - TuningData<ConversationManager.Tuning>.Get().minionCooldownTime;
		return num > num2;
	}

	private List<Conversation> conversations;

	private Dictionary<MinionIdentity, float> lastConvoTimeByMinion;

	private readonly Dictionary<MinionIdentity, Conversation> minionConversations = new Dictionary<MinionIdentity, Conversation>();

	private List<Type> convoTypes = new List<Type>
	{
		typeof(RecentThingConversation),
		typeof(AmountStateConversation),
		typeof(CurrentJobConversation)
	};

	private static readonly Tag[] invalidConvoTags = new Tag[]
	{
		GameTags.Asleep,
		GameTags.BionicBedTime,
		GameTags.HoldingBreath,
		GameTags.Dead,
		GameTags.SuppressConversation
	};

	public class Tuning : TuningData<ConversationManager.Tuning>
	{
		public float cyclesBeforeFirstConversation;

		public float maxDistance;

		public int maxDupesPerConvo;

		public float minionCooldownTime;

		public float speakTime;

		public float delayBetweenUtterances;

		public float delayBeforeStart;

		public int maxUtterances;
	}

	public class StartedTalkingEvent
	{
		public GameObject talker;

		public string anim;
	}
}
