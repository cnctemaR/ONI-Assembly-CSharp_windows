using System;
using System.Collections.Generic;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class GridRestrictionSerializer : KMonoBehaviour, ISaveLoadable
{
	public static void DestroyInstance()
	{
		GridRestrictionSerializer.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GridRestrictionSerializer.Instance = this;
	}

	public int GetTagId(Tag gameTag)
	{
		foreach (KeyValuePair<Tag, int> keyValuePair in this.tagToId)
		{
			if (keyValuePair.Key == gameTag)
			{
				return keyValuePair.Value;
			}
		}
		DebugUtil.DevAssert(false, "Gametag " + gameTag.Name + " has not been added to the valid list of GridRestrictionTagId's before requesting the ID", null);
		return 0;
	}

	public Tag[] ValidRobotTypes
	{
		get
		{
			return this.robotTypeTags;
		}
	}

	public static GridRestrictionSerializer Instance;

	private List<KeyValuePair<Tag, int>> tagToId = new List<KeyValuePair<Tag, int>>
	{
		new KeyValuePair<Tag, int>(GameTags.Minions.Models.Standard, -1),
		new KeyValuePair<Tag, int>(GameTags.Minions.Models.Bionic, -2),
		new KeyValuePair<Tag, int>(GameTags.Robot, -3),
		new KeyValuePair<Tag, int>(GameTags.Robots.Models.FetchDrone, -4),
		new KeyValuePair<Tag, int>(GameTags.Robots.Models.ScoutRover, -5),
		new KeyValuePair<Tag, int>(GameTags.Robots.Models.MorbRover, -6)
	};

	private Tag[] robotTypeTags = new Tag[]
	{
		GameTags.Robots.Models.FetchDrone,
		GameTags.Robots.Models.ScoutRover,
		GameTags.Robots.Models.MorbRover
	};
}
