using System;
using System.Collections.Generic;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class ChoreGroupManager : KMonoBehaviour, ISaveLoadable
{
	public List<Tag> DefaultForbiddenTagsList
	{
		get
		{
			return this.defaultForbiddenTagsList;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ChoreGroupManager.instance = this;
	}

	public static ChoreGroupManager instance;

	[Serialize]
	private List<Tag> defaultForbiddenTagsList = new List<Tag>();
}
