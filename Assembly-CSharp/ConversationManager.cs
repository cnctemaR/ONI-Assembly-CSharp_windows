using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class ConversationManager : KMonoBehaviour, ISim200ms
{
	protected override void OnPrefabInit()
	{
		this.activeSetups = new List<Conversation>();
		this.lastConvoTimeByMinion = new Dictionary<MinionIdentity, float>();
	}

	public void Sim200ms(float dt)
	{
		for (int i = this.activeSetups.Count - 1; i >= 0; i--)
		{
			Conversation conversation = this.activeSetups[i];
			for (int j = conversation.minions.Count - 1; j >= 0; j--)
			{
				if (!this.ValidMinionTags(conversation.minions[j]) || !this.MinionCloseEnoughToConvo(conversation.minions[j], conversation))
				{
					conversation.minions.RemoveAt(j);
				}
				else
				{
					this.setupsByMinion[conversation.minions[j]] = conversation;
				}
			}
			if (conversation.minions.Count <= 1)
			{
				this.activeSetups.RemoveAt(i);
			}
			else
			{
				bool flag = true;
				if (conversation.numUtterances == 0 && GameClock.Instance.GetTime() > conversation.lastTalkedTime + TuningData<ConversationManager.Tuning>.Get().delayBeforeStart)
				{
					MinionIdentity minionIdentity = conversation.minions[global::UnityEngine.Random.Range(0, conversation.minions.Count)];
					conversation.conversationType.NewTarget(minionIdentity);
					flag = this.DoTalking(conversation, minionIdentity);
				}
				else if (conversation.numUtterances > 0 && conversation.numUtterances < TuningData<ConversationManager.Tuning>.Get().maxUtterances && GameClock.Instance.GetTime() > conversation.lastTalkedTime + TuningData<ConversationManager.Tuning>.Get().speakTime + TuningData<ConversationManager.Tuning>.Get().delayBetweenUtterances)
				{
					int num = conversation.minions.IndexOf(conversation.lastTalked);
					int num2 = (num + global::UnityEngine.Random.Range(1, conversation.minions.Count)) % conversation.minions.Count;
					MinionIdentity minionIdentity2 = conversation.minions[num2];
					flag = this.DoTalking(conversation, minionIdentity2);
				}
				else if (conversation.numUtterances >= TuningData<ConversationManager.Tuning>.Get().maxUtterances)
				{
					flag = false;
				}
				if (!flag)
				{
					this.activeSetups.RemoveAt(i);
				}
			}
		}
		foreach (MinionIdentity minionIdentity3 in Components.LiveMinionIdentities.Items)
		{
			if (this.ValidMinionTags(minionIdentity3) && !this.setupsByMinion.ContainsKey(minionIdentity3) && !this.MinionOnCooldown(minionIdentity3))
			{
				foreach (MinionIdentity minionIdentity4 in Components.LiveMinionIdentities.Items)
				{
					if (!(minionIdentity4 == minionIdentity3) && this.ValidMinionTags(minionIdentity4))
					{
						if (this.setupsByMinion.ContainsKey(minionIdentity4))
						{
							Conversation conversation2 = this.setupsByMinion[minionIdentity4];
							if (conversation2.minions.Count < TuningData<ConversationManager.Tuning>.Get().maxDupesPerConvo)
							{
								Vector3 centroid = this.GetCentroid(conversation2);
								float magnitude = (centroid - minionIdentity3.transform.GetPosition()).magnitude;
								if (magnitude < TuningData<ConversationManager.Tuning>.Get().maxDistance * 0.5f)
								{
									conversation2.minions.Add(minionIdentity3);
									this.setupsByMinion[minionIdentity3] = conversation2;
									break;
								}
							}
						}
						else if (!this.MinionOnCooldown(minionIdentity4))
						{
							float magnitude2 = (minionIdentity4.transform.GetPosition() - minionIdentity3.transform.GetPosition()).magnitude;
							if (magnitude2 < TuningData<ConversationManager.Tuning>.Get().maxDistance)
							{
								Conversation conversation3 = new Conversation();
								conversation3.minions.Add(minionIdentity3);
								conversation3.minions.Add(minionIdentity4);
								Type type = this.convoTypes[global::UnityEngine.Random.Range(0, this.convoTypes.Count)];
								conversation3.conversationType = (ConversationType)Activator.CreateInstance(type);
								conversation3.lastTalkedTime = GameClock.Instance.GetTime();
								this.activeSetups.Add(conversation3);
								this.setupsByMinion[minionIdentity3] = conversation3;
								this.setupsByMinion[minionIdentity4] = conversation3;
								break;
							}
						}
					}
				}
			}
		}
		this.setupsByMinion.Clear();
	}

	private bool DoTalking(Conversation setup, MinionIdentity new_speaker)
	{
		if (setup.lastTalked != null)
		{
			setup.lastTalked.Trigger(25860745, setup.lastTalked.gameObject);
		}
		Conversation.Topic nextTopic = setup.conversationType.GetNextTopic(new_speaker, setup.lastTopic);
		if (nextTopic == null || nextTopic.mode == Conversation.ModeType.End || nextTopic.mode == Conversation.ModeType.Segue)
		{
			return false;
		}
		Thought thoughtForTopic = this.GetThoughtForTopic(setup, nextTopic);
		if (thoughtForTopic == null)
		{
			return false;
		}
		setup.lastTopic = nextTopic;
		setup.lastTalked = new_speaker;
		setup.lastTalkedTime = GameClock.Instance.GetTime();
		this.lastConvoTimeByMinion[setup.lastTalked] = GameClock.Instance.GetTime();
		ThoughtGraph.Instance smi = setup.lastTalked.GetSMI<ThoughtGraph.Instance>();
		smi.AddThought(thoughtForTopic);
		Effects component = setup.lastTalked.GetComponent<Effects>();
		component.Add("GoodConversation", true);
		Conversation.Mode mode = Conversation.Topic.Modes[(int)nextTopic.mode];
		ConversationManager.StartedTalkingEvent startedTalkingEvent = new ConversationManager.StartedTalkingEvent
		{
			talker = new_speaker.gameObject,
			anim = mode.anim
		};
		foreach (MinionIdentity minionIdentity in setup.minions)
		{
			DebugUtil.DevAssert(minionIdentity, new object[] { "minion in setup.minions was null" });
			if (minionIdentity)
			{
				minionIdentity.Trigger(-594200555, startedTalkingEvent);
			}
		}
		setup.numUtterances++;
		return true;
	}

	private Vector3 GetCentroid(Conversation setup)
	{
		Vector3 vector = Vector3.zero;
		foreach (MinionIdentity minionIdentity in setup.minions)
		{
			if (!(minionIdentity == null))
			{
				vector += minionIdentity.transform.GetPosition();
			}
		}
		return vector / (float)setup.minions.Count;
	}

	private Thought GetThoughtForTopic(Conversation setup, Conversation.Topic topic)
	{
		DebugUtil.DevAssert(!string.IsNullOrEmpty(topic.topic), new object[0]);
		if (string.IsNullOrEmpty(topic.topic))
		{
			return null;
		}
		Sprite sprite = setup.conversationType.GetSprite(topic.topic);
		if (sprite != null)
		{
			Conversation.Mode mode = Conversation.Topic.Modes[(int)topic.mode];
			return new Thought("Topic_" + topic.topic, null, sprite, mode.icon, mode.voice, "bubble_chatter", mode.mouth, DUPLICANTS.THOUGHTS.CONVERSATION.TOOLTIP, true, TuningData<ConversationManager.Tuning>.Get().speakTime);
		}
		DebugUtil.DevAssert(sprite != null, new object[] { "Couldn't find a sprite for conversation topic:", topic.topic });
		return null;
	}

	private bool ValidMinionTags(MinionIdentity minion)
	{
		if (minion == null)
		{
			return false;
		}
		KPrefabID component = minion.GetComponent<KPrefabID>();
		return !component.HasAnyTags(ConversationManager.invalidConvoTags);
	}

	private bool MinionCloseEnoughToConvo(MinionIdentity minion, Conversation setup)
	{
		Vector3 centroid = this.GetCentroid(setup);
		float magnitude = (centroid - minion.transform.GetPosition()).magnitude;
		return magnitude < TuningData<ConversationManager.Tuning>.Get().maxDistance * 0.5f;
	}

	private bool MinionOnCooldown(MinionIdentity minion)
	{
		KPrefabID component = minion.GetComponent<KPrefabID>();
		return !component.HasTag(GameTags.AlwaysConverse) && ((this.lastConvoTimeByMinion.ContainsKey(minion) && GameClock.Instance.GetTime() < this.lastConvoTimeByMinion[minion] + TuningData<ConversationManager.Tuning>.Get().minionCooldownTime) || GameClock.Instance.GetTime() / 600f < TuningData<ConversationManager.Tuning>.Get().cyclesBeforeFirstConversation);
	}

	private List<Conversation> activeSetups;

	private Dictionary<MinionIdentity, float> lastConvoTimeByMinion;

	private Dictionary<MinionIdentity, Conversation> setupsByMinion = new Dictionary<MinionIdentity, Conversation>();

	private List<Type> convoTypes = new List<Type>
	{
		typeof(RecentThingConversation),
		typeof(AmountStateConversation),
		typeof(CurrentJobConversation)
	};

	private static readonly List<Tag> invalidConvoTags = new List<Tag>
	{
		GameTags.Asleep,
		GameTags.HoldingBreath,
		GameTags.Dead
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
