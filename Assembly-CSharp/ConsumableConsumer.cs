using System;
using System.Collections.Generic;
using KSerialization;

public class ConsumableConsumer : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (ConsumerManager.instance != null)
		{
			this.forbiddenTags = ConsumerManager.instance.DefaultForbiddenTagsList.ToArray();
			return;
		}
		this.forbiddenTags = new Tag[0];
	}

	public bool IsPermitted(string consumable_id)
	{
		Tag tag = new Tag(consumable_id);
		for (int i = 0; i < this.forbiddenTags.Length; i++)
		{
			if (this.forbiddenTags[i] == tag)
			{
				return false;
			}
		}
		return true;
	}

	public void SetPermitted(string consumable_id, bool is_allowed)
	{
		Tag tag = new Tag(consumable_id);
		List<Tag> list = new List<Tag>(this.forbiddenTags);
		if (is_allowed)
		{
			list.Remove(tag);
		}
		else if (!list.Contains(tag))
		{
			list.Add(tag);
		}
		this.forbiddenTags = list.ToArray();
		this.consumableRulesChanged.Signal();
	}

	[Serialize]
	public Tag[] forbiddenTags;

	public global::System.Action consumableRulesChanged;
}
