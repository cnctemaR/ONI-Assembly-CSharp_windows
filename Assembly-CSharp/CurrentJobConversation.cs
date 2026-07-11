using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

public class CurrentJobConversation : ConversationType
{
	public CurrentJobConversation()
	{
		this.id = "CurrentJobConversation";
	}

	public override void NewTarget(MinionIdentity speaker)
	{
		this.target = "hows_role";
	}

	public override Conversation.Topic GetNextTopic(MinionIdentity speaker, Conversation.Topic lastTopic)
	{
		if (lastTopic == null)
		{
			return new Conversation.Topic(this.target, Conversation.ModeType.Query);
		}
		List<Conversation.ModeType> list = CurrentJobConversation.transitions[lastTopic.mode];
		Conversation.ModeType modeType = list[global::UnityEngine.Random.Range(0, list.Count)];
		if (modeType == Conversation.ModeType.Statement)
		{
			this.target = this.GetRoleForSpeaker(speaker);
			Conversation.ModeType modeForRole = this.GetModeForRole(speaker, this.target);
			return new Conversation.Topic(this.target, modeForRole);
		}
		return new Conversation.Topic(this.target, modeType);
	}

	public override Sprite GetSprite(string topic)
	{
		if (topic == "hows_role")
		{
			return Assets.GetSprite("crew_state_role");
		}
		if (RoleManager.roleHatIndex.ContainsKey(topic))
		{
			return Assets.GetSprite(RoleManager.roleHatIndex[topic]);
		}
		return null;
	}

	private Conversation.ModeType GetModeForRole(MinionIdentity speaker, string roleId)
	{
		MinionResume component = speaker.GetComponent<MinionResume>();
		RoleConfig role = Game.Instance.roleManager.GetRole(roleId);
		AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(speaker);
		AttributeInstance attributeInstance2 = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(speaker);
		if (component.AptitudeByRoleGroup[role.roleGroup] > 0f)
		{
			return Conversation.ModeType.Satisfaction;
		}
		if (attributeInstance.GetTotalValue() < attributeInstance2.GetTotalValue())
		{
			return Conversation.ModeType.Stressing;
		}
		if (roleId == "NoRole")
		{
			return Conversation.ModeType.Dissatisfaction;
		}
		return Conversation.ModeType.Nominal;
	}

	private string GetRoleForSpeaker(MinionIdentity speaker)
	{
		MinionResume component = speaker.GetComponent<MinionResume>();
		return component.CurrentRole;
	}

	public static Dictionary<Conversation.ModeType, List<Conversation.ModeType>> transitions = new Dictionary<Conversation.ModeType, List<Conversation.ModeType>>
	{
		{
			Conversation.ModeType.Query,
			new List<Conversation.ModeType> { Conversation.ModeType.Statement }
		},
		{
			Conversation.ModeType.Satisfaction,
			new List<Conversation.ModeType> { Conversation.ModeType.Agreement }
		},
		{
			Conversation.ModeType.Nominal,
			new List<Conversation.ModeType> { Conversation.ModeType.Musing }
		},
		{
			Conversation.ModeType.Dissatisfaction,
			new List<Conversation.ModeType> { Conversation.ModeType.Disagreement }
		},
		{
			Conversation.ModeType.Stressing,
			new List<Conversation.ModeType> { Conversation.ModeType.Disagreement }
		},
		{
			Conversation.ModeType.Agreement,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Query,
				Conversation.ModeType.End
			}
		},
		{
			Conversation.ModeType.Disagreement,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Query,
				Conversation.ModeType.End
			}
		},
		{
			Conversation.ModeType.Musing,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Query,
				Conversation.ModeType.End
			}
		}
	};
}
