using System;

public class EntityTypeSet : ResourceSet<EntityType>
{
	public virtual void RegisterPrefab(KPrefabID prefab)
	{
	}

	public static EntityTypeSet Instance;
}
