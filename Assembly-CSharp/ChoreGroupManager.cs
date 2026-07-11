using System;
using System.Collections.Generic;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class ChoreGroupManager : KMonoBehaviour, ISaveLoadable
{
	public static void DestroyInstance()
	{
		ChoreGroupManager.instance = null;
	}

	public List<Tag> DefaultForbiddenTagsList
	{
		get
		{
			return this.defaultForbiddenTagsList;
		}
	}

	public Dictionary<Tag, int> DefaultChorePermission
	{
		get
		{
			return this.defaultChorePermissions;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ChoreGroupManager.instance = this;
		this.ConvertOldVersion();
		foreach (ChoreGroup choreGroup in Db.Get().ChoreGroups.resources)
		{
			if (!this.defaultChorePermissions.ContainsKey(choreGroup.Id.ToTag()))
			{
				this.defaultChorePermissions.Add(choreGroup.Id.ToTag(), 2);
			}
		}
	}

	private void ConvertOldVersion()
	{
		foreach (Tag tag in this.defaultForbiddenTagsList)
		{
			if (!this.defaultChorePermissions.ContainsKey(tag))
			{
				this.defaultChorePermissions.Add(tag, -1);
			}
			this.defaultChorePermissions[tag] = 0;
		}
		this.defaultForbiddenTagsList.Clear();
	}

	public static ChoreGroupManager instance;

	[Serialize]
	private List<Tag> defaultForbiddenTagsList = new List<Tag>();

	[Serialize]
	private Dictionary<Tag, int> defaultChorePermissions = new Dictionary<Tag, int>();
}
