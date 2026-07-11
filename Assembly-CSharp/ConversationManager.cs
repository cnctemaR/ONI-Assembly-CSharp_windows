using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class ConversationManager : KMonoBehaviour, ISim200ms
{
	protected override void OnPrefabInit()
	{
		this.activeSetups = new List<ConversationManager.ConversationSetup>();
		this.lastConvoTimeByMinion = new Dictionary<MinionIdentity, float>();
	}

	public void Sim200ms(float dt)
	{
		Dictionary<MinionIdentity, ConversationManager.ConversationSetup> dictionary = new Dictionary<MinionIdentity, ConversationManager.ConversationSetup>();
		for (int i = this.activeSetups.Count - 1; i >= 0; i--)
		{
			ConversationManager.ConversationSetup conversationSetup = this.activeSetups[i];
			for (int j = conversationSetup.minions.Count - 1; j >= 0; j--)
			{
				if (!this.ValidMinionTags(conversationSetup.minions[j]) || !this.MinionCloseEnoughToConvo(conversationSetup.minions[j], conversationSetup))
				{
					conversationSetup.minions.RemoveAt(j);
				}
				else
				{
					dictionary[conversationSetup.minions[j]] = conversationSetup;
				}
			}
			if (conversationSetup.minions.Count <= 1)
			{
				this.activeSetups.RemoveAt(i);
			}
			else if (conversationSetup.numUtterances == 0 && GameClock.Instance.GetTime() > conversationSetup.lastTalkedTime + TuningData<ConversationManager.Tuning>.Get().delayBeforeStart)
			{
				MinionIdentity minionIdentity = conversationSetup.minions[global::UnityEngine.Random.Range(0, conversationSetup.minions.Count)];
				this.DoTalking(conversationSetup, minionIdentity, this.ChooseNewTopic(minionIdentity));
			}
			else if (conversationSetup.numUtterances > 0 && conversationSetup.numUtterances < TuningData<ConversationManager.Tuning>.Get().maxUtterances && GameClock.Instance.GetTime() > conversationSetup.lastTalkedTime + TuningData<ConversationManager.Tuning>.Get().speakTime + TuningData<ConversationManager.Tuning>.Get().delayBetweenUtterances)
			{
				int num = conversationSetup.minions.IndexOf(conversationSetup.lastTalked);
				int num2 = (num + global::UnityEngine.Random.Range(1, conversationSetup.minions.Count)) % conversationSetup.minions.Count;
				MinionIdentity minionIdentity2 = conversationSetup.minions[num2];
				ConversationManager.Topic topic = this.ChooseRelatedTopic(minionIdentity2, conversationSetup.lastTopic);
				if (topic == null)
				{
					this.activeSetups.RemoveAt(i);
				}
				else
				{
					this.DoTalking(conversationSetup, minionIdentity2, topic);
				}
			}
			else if (conversationSetup.numUtterances >= TuningData<ConversationManager.Tuning>.Get().maxUtterances)
			{
				this.activeSetups.RemoveAt(i);
			}
		}
		foreach (MinionIdentity minionIdentity3 in Components.LiveMinionIdentities.Items)
		{
			if (this.ValidMinionTags(minionIdentity3) && !dictionary.ContainsKey(minionIdentity3) && !this.MinionOnCooldown(minionIdentity3))
			{
				foreach (MinionIdentity minionIdentity4 in Components.LiveMinionIdentities.Items)
				{
					if (!(minionIdentity4 == minionIdentity3) && this.ValidMinionTags(minionIdentity4))
					{
						if (dictionary.ContainsKey(minionIdentity4))
						{
							ConversationManager.ConversationSetup conversationSetup2 = dictionary[minionIdentity4];
							if (conversationSetup2.minions.Count < TuningData<ConversationManager.Tuning>.Get().maxDupesPerConvo)
							{
								Vector3 centroid = this.GetCentroid(conversationSetup2);
								float magnitude = (centroid - minionIdentity3.transform.GetPosition()).magnitude;
								if (magnitude < TuningData<ConversationManager.Tuning>.Get().maxDistance * 0.5f)
								{
									conversationSetup2.minions.Add(minionIdentity3);
									dictionary[minionIdentity3] = conversationSetup2;
									break;
								}
							}
						}
						else if (!this.MinionOnCooldown(minionIdentity4))
						{
							float magnitude2 = (minionIdentity4.transform.GetPosition() - minionIdentity3.transform.GetPosition()).magnitude;
							if (magnitude2 < TuningData<ConversationManager.Tuning>.Get().maxDistance)
							{
								ConversationManager.ConversationSetup conversationSetup3 = new ConversationManager.ConversationSetup();
								conversationSetup3.minions.Add(minionIdentity3);
								conversationSetup3.minions.Add(minionIdentity4);
								conversationSetup3.lastTalkedTime = GameClock.Instance.GetTime();
								this.activeSetups.Add(conversationSetup3);
								dictionary[minionIdentity3] = conversationSetup3;
								dictionary[minionIdentity4] = conversationSetup3;
								break;
							}
						}
					}
				}
			}
		}
	}

	private void DoTalking(ConversationManager.ConversationSetup setup, MinionIdentity new_speaker, ConversationManager.Topic new_topic)
	{
		if (setup.lastTalked != null)
		{
			setup.lastTalked.Trigger(25860745, setup.lastTalked.gameObject);
		}
		Thought thoughtForTopic = this.GetThoughtForTopic(new_topic);
		if (thoughtForTopic == null)
		{
			return;
		}
		setup.lastTopic = new_topic;
		setup.lastTalked = new_speaker;
		setup.lastTalkedTime = GameClock.Instance.GetTime();
		this.lastConvoTimeByMinion[setup.lastTalked] = GameClock.Instance.GetTime();
		ThoughtGraph.Instance smi = setup.lastTalked.GetSMI<ThoughtGraph.Instance>();
		smi.AddThought(thoughtForTopic);
		Effects component = setup.lastTalked.GetComponent<Effects>();
		component.Add("GoodConversation", true);
		ConversationManager.StartedTalkingEvent startedTalkingEvent = new ConversationManager.StartedTalkingEvent
		{
			talker = new_speaker.gameObject,
			anim = ConversationManager.Topic.Modes[new_topic.mode].anim
		};
		foreach (MinionIdentity minionIdentity in setup.minions)
		{
			minionIdentity.Trigger(1102989392, setup.lastTopic.topic);
			minionIdentity.Trigger(-594200555, startedTalkingEvent);
		}
		setup.numUtterances++;
	}

	private Vector3 GetCentroid(ConversationManager.ConversationSetup setup)
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

	private ConversationManager.Topic ChooseNewTopic(MinionIdentity minion)
	{
		ConversationMonitor.Instance smi = minion.GetSMI<ConversationMonitor.Instance>();
		string atopic = smi.GetATopic();
		ConversationManager.ModeType[] array = new ConversationManager.ModeType[]
		{
			ConversationManager.ModeType.Query,
			ConversationManager.ModeType.Statement,
			ConversationManager.ModeType.Musing
		};
		ConversationManager.ModeType modeType = array[global::UnityEngine.Random.Range(0, array.Length)];
		return new ConversationManager.Topic(atopic, modeType);
	}

	private ConversationManager.Topic ChooseRelatedTopic(MinionIdentity minion, ConversationManager.Topic previousTopic)
	{
		ConversationManager.Mode mode = ConversationManager.Topic.Modes[previousTopic.mode];
		ConversationManager.ModeType modeType = mode.transitions[global::UnityEngine.Random.Range(0, mode.transitions.Count)];
		if (modeType == ConversationManager.ModeType.End)
		{
			return null;
		}
		ConversationManager.Mode mode2 = ConversationManager.Topic.Modes[modeType];
		ConversationManager.Topic topic;
		if (mode2.newTopic)
		{
			topic = this.ChooseNewTopic(minion);
			topic.mode = modeType;
		}
		else
		{
			topic = new ConversationManager.Topic(previousTopic.topic, modeType);
		}
		return topic;
	}

	private Thought GetThoughtForTopic(ConversationManager.Topic topic)
	{
		DebugUtil.DevAssert(!string.IsNullOrEmpty(topic.topic), "Assert!");
		if (string.IsNullOrEmpty(topic.topic))
		{
			return null;
		}
		global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(topic.topic, "ui", true);
		if (uisprite != null)
		{
			ConversationManager.Mode mode = ConversationManager.Topic.Modes[topic.mode];
			return new Thought("Topic_" + topic.topic, null, uisprite.first, mode.icon, mode.voice, "bubble_chatter", mode.mouth, DUPLICANTS.THOUGHTS.CONVERSATION.TOOLTIP, true, TuningData<ConversationManager.Tuning>.Get().speakTime);
		}
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

	private bool MinionCloseEnoughToConvo(MinionIdentity minion, ConversationManager.ConversationSetup setup)
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

	private List<ConversationManager.ConversationSetup> activeSetups;

	private Dictionary<MinionIdentity, float> lastConvoTimeByMinion;

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

	public enum ModeType
	{
		Query,
		Statement,
		Agreement,
		Disagreement,
		Musing,
		Satisfaction,
		Dissatisfaction,
		Segue,
		End
	}

	private class Mode
	{
		public Mode(ConversationManager.ModeType type, string voice, string icon, string mouth, string anim, List<ConversationManager.ModeType> transitions, bool newTopic = false)
		{
			this.type = type;
			this.voice = voice;
			this.mouth = mouth;
			this.anim = anim;
			this.icon = icon;
			this.transitions = transitions;
			this.newTopic = newTopic;
		}

		public ConversationManager.ModeType type;

		public string voice;

		public string mouth;

		public string anim;

		public string icon;

		public List<ConversationManager.ModeType> transitions;

		public bool newTopic;
	}

	private class Topic
	{
		public Topic(string topic, ConversationManager.ModeType mode)
		{
			this.topic = topic;
			this.mode = mode;
		}

		public static Dictionary<ConversationManager.ModeType, ConversationManager.Mode> Modes
		{
			get
			{
				if (ConversationManager.Topic._modes == null)
				{
					ConversationManager.Topic._modes = new Dictionary<ConversationManager.ModeType, ConversationManager.Mode>();
					foreach (ConversationManager.Mode mode in ConversationManager.Topic.modeList)
					{
						ConversationManager.Topic._modes[mode.type] = mode;
					}
				}
				return ConversationManager.Topic._modes;
			}
		}

		public static List<ConversationManager.Mode> modeList = new List<ConversationManager.Mode>
		{
			new ConversationManager.Mode(ConversationManager.ModeType.Query, "conversation_question", "mode_query", SpeechMonitor.PREFIX_HAPPY, "happy", new List<ConversationManager.ModeType>
			{
				ConversationManager.ModeType.Agreement,
				ConversationManager.ModeType.Disagreement,
				ConversationManager.ModeType.Musing
			}, false),
			new ConversationManager.Mode(ConversationManager.ModeType.Statement, "conversation_answer", "mode_statement", SpeechMonitor.PREFIX_HAPPY, "happy", new List<ConversationManager.ModeType>
			{
				ConversationManager.ModeType.Agreement,
				ConversationManager.ModeType.Disagreement,
				ConversationManager.ModeType.Query,
				ConversationManager.ModeType.Segue
			}, false),
			new ConversationManager.Mode(ConversationManager.ModeType.Agreement, "conversation_answer", "mode_agreement", SpeechMonitor.PREFIX_HAPPY, "happy", new List<ConversationManager.ModeType> { ConversationManager.ModeType.Satisfaction }, false),
			new ConversationManager.Mode(ConversationManager.ModeType.Disagreement, "conversation_answer", "mode_disagreement", SpeechMonitor.PREFIX_SAD, "unhappy", new List<ConversationManager.ModeType> { ConversationManager.ModeType.Dissatisfaction }, false),
			new ConversationManager.Mode(ConversationManager.ModeType.Musing, "conversation_short", "mode_musing", SpeechMonitor.PREFIX_HAPPY, "happy", new List<ConversationManager.ModeType>
			{
				ConversationManager.ModeType.Query,
				ConversationManager.ModeType.Statement,
				ConversationManager.ModeType.Segue
			}, false),
			new ConversationManager.Mode(ConversationManager.ModeType.Satisfaction, "conversation_short", "mode_satisfaction", SpeechMonitor.PREFIX_HAPPY, "happy", new List<ConversationManager.ModeType>
			{
				ConversationManager.ModeType.Segue,
				ConversationManager.ModeType.End
			}, false),
			new ConversationManager.Mode(ConversationManager.ModeType.Dissatisfaction, "conversation_short", "mode_dissatisfaction", SpeechMonitor.PREFIX_SAD, "unhappy", new List<ConversationManager.ModeType>
			{
				ConversationManager.ModeType.Segue,
				ConversationManager.ModeType.End
			}, false),
			new ConversationManager.Mode(ConversationManager.ModeType.Segue, "conversation_question", "mode_segue", SpeechMonitor.PREFIX_HAPPY, "happy", new List<ConversationManager.ModeType>
			{
				ConversationManager.ModeType.Agreement,
				ConversationManager.ModeType.Disagreement,
				ConversationManager.ModeType.Musing
			}, true)
		};

		private static Dictionary<ConversationManager.ModeType, ConversationManager.Mode> _modes;

		public string topic;

		public ConversationManager.ModeType mode;
	}

	private class ConversationSetup
	{
		public List<MinionIdentity> minions = new List<MinionIdentity>();

		public MinionIdentity lastTalked;

		public float lastTalkedTime;

		public ConversationManager.Topic lastTopic;

		public int numUtterances;
	}
}
