using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class QuestManager : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		if (QuestManager.instance != null)
		{
			global::UnityEngine.Object.Destroy(QuestManager.instance);
			return;
		}
		QuestManager.instance = this;
		base.OnPrefabInit();
	}

	public static QuestInstance InitializeQuest(Tag ownerId, Quest quest)
	{
		QuestInstance questInstance;
		if (!QuestManager.TryGetQuest(ownerId.GetHash(), quest, out questInstance))
		{
			questInstance = (QuestManager.instance.ownerToQuests[ownerId.GetHash()][quest.IdHash] = new QuestInstance(quest));
		}
		questInstance.Initialize(quest);
		return questInstance;
	}

	public static QuestInstance InitializeQuest(HashedString ownerId, Quest quest)
	{
		QuestInstance questInstance;
		if (!QuestManager.TryGetQuest(ownerId.HashValue, quest, out questInstance))
		{
			questInstance = (QuestManager.instance.ownerToQuests[ownerId.HashValue][quest.IdHash] = new QuestInstance(quest));
		}
		questInstance.Initialize(quest);
		return questInstance;
	}

	public static QuestInstance GetInstance(Tag ownerId, Quest quest)
	{
		QuestInstance questInstance;
		QuestManager.TryGetQuest(ownerId.GetHash(), quest, out questInstance);
		return questInstance;
	}

	public static QuestInstance GetInstance(HashedString ownerId, Quest quest)
	{
		QuestInstance questInstance;
		QuestManager.TryGetQuest(ownerId.HashValue, quest, out questInstance);
		return questInstance;
	}

	public static bool CheckState(HashedString ownerId, Quest quest, Quest.State state)
	{
		QuestInstance questInstance;
		QuestManager.TryGetQuest(ownerId.HashValue, quest, out questInstance);
		return questInstance != null && questInstance.CurrentState == state;
	}

	public static bool CheckState(Tag ownerId, Quest quest, Quest.State state)
	{
		QuestInstance questInstance;
		QuestManager.TryGetQuest(ownerId.GetHash(), quest, out questInstance);
		return questInstance != null && questInstance.CurrentState == state;
	}

	private static bool TryGetQuest(int ownerId, Quest quest, out QuestInstance qInst)
	{
		qInst = null;
		Dictionary<HashedString, QuestInstance> dictionary;
		if (!QuestManager.instance.ownerToQuests.TryGetValue(ownerId, out dictionary))
		{
			dictionary = (QuestManager.instance.ownerToQuests[ownerId] = new Dictionary<HashedString, QuestInstance>());
		}
		return dictionary.TryGetValue(quest.IdHash, out qInst);
	}

	private static QuestManager instance;

	[Serialize]
	private Dictionary<int, Dictionary<HashedString, QuestInstance>> ownerToQuests = new Dictionary<int, Dictionary<HashedString, QuestInstance>>();
}
