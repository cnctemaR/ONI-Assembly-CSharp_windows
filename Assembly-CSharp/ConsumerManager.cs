using System;
using System.Collections.Generic;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class ConsumerManager : KMonoBehaviour, ISaveLoadable
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
		ConsumerManager.instance = this;
	}

	public static ConsumerManager instance;

	[Serialize]
	private List<Tag> defaultForbiddenTagsList = new List<Tag>();
}
