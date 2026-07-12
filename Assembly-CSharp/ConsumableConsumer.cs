using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ConsumableConsumer")]
public class ConsumableConsumer : KMonoBehaviour
{
	[OnDeserialized]
	[Obsolete]
	private void OnDeserialized()
	{
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 29))
		{
			this.forbiddenTagSet = new HashSet<Tag>(this.forbiddenTags);
			this.forbiddenTags = null;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (ConsumerManager.instance != null)
		{
			this.forbiddenTagSet = new HashSet<Tag>(ConsumerManager.instance.DefaultForbiddenTagsList);
			return;
		}
		this.forbiddenTagSet = new HashSet<Tag>();
	}

	public bool IsPermitted(string consumable_id)
	{
		Tag tag = new Tag(consumable_id);
		return !this.forbiddenTagSet.Contains(tag);
	}

	public void SetPermitted(string consumable_id, bool is_allowed)
	{
		Tag tag = new Tag(consumable_id);
		if (is_allowed)
		{
			this.forbiddenTagSet.Remove(tag);
		}
		else
		{
			this.forbiddenTagSet.Add(tag);
		}
		this.consumableRulesChanged.Signal();
	}

	[Obsolete("Deprecated, use forbiddenTagSet")]
	[Serialize]
	public Tag[] forbiddenTags;

	[Serialize]
	public HashSet<Tag> forbiddenTagSet;

	public global::System.Action consumableRulesChanged;
}
