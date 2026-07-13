using System;
using System.Collections.Generic;
using UnityEngine;

public class RecentThingConversation : ConversationType
{
	public RecentThingConversation()
	{
		this.id = "RecentThingConversation";
	}

	public override void NewTarget(MinionIdentity speaker)
	{
		ConversationMonitor.Instance smi = speaker.GetSMI<ConversationMonitor.Instance>();
		this.target = smi.GetATopic();
	}

	public override Conversation.Topic GetNextTopic(MinionIdentity speaker, Conversation.Topic lastTopic)
	{
		if (string.IsNullOrEmpty(this.target))
		{
			return null;
		}
		List<Conversation.ModeType> list = ((lastTopic == null) ? RecentThingConversation.INITIAL_MODES : RecentThingConversation.transitions[lastTopic.mode]);
		Conversation.ModeType modeType = list[global::UnityEngine.Random.Range(0, list.Count)];
		if (modeType == Conversation.ModeType.Segue)
		{
			this.NewTarget(speaker);
			modeType = RecentThingConversation.INITIAL_MODES[global::UnityEngine.Random.Range(0, RecentThingConversation.INITIAL_MODES.Count)];
		}
		return new Conversation.Topic(this.target, modeType);
	}

	public override Sprite GetSprite(string topic)
	{
		global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(topic, "ui", true);
		if (uisprite != null)
		{
			return uisprite.first;
		}
		return null;
	}

	public static Dictionary<Conversation.ModeType, List<Conversation.ModeType>> transitions = new Dictionary<Conversation.ModeType, List<Conversation.ModeType>>
	{
		{
			Conversation.ModeType.Query,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Agreement,
				Conversation.ModeType.Disagreement,
				Conversation.ModeType.Musing
			}
		},
		{
			Conversation.ModeType.Statement,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Agreement,
				Conversation.ModeType.Disagreement,
				Conversation.ModeType.Query,
				Conversation.ModeType.Segue
			}
		},
		{
			Conversation.ModeType.Agreement,
			new List<Conversation.ModeType> { Conversation.ModeType.Satisfaction }
		},
		{
			Conversation.ModeType.Disagreement,
			new List<Conversation.ModeType> { Conversation.ModeType.Dissatisfaction }
		},
		{
			Conversation.ModeType.Musing,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Query,
				Conversation.ModeType.Statement,
				Conversation.ModeType.Segue
			}
		},
		{
			Conversation.ModeType.Satisfaction,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Segue,
				Conversation.ModeType.End
			}
		},
		{
			Conversation.ModeType.Nominal,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Segue,
				Conversation.ModeType.End
			}
		},
		{
			Conversation.ModeType.Dissatisfaction,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Segue,
				Conversation.ModeType.End
			}
		}
	};

	private static readonly List<Conversation.ModeType> INITIAL_MODES = new List<Conversation.ModeType>
	{
		Conversation.ModeType.Query,
		Conversation.ModeType.Statement,
		Conversation.ModeType.Musing
	};
}
