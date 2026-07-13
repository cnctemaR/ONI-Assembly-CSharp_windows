using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/EntityConfigManager")]
public class EntityConfigManager : KMonoBehaviour
{
	public static void DestroyInstance()
	{
		EntityConfigManager.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		EntityConfigManager.Instance = this;
	}

	private static int GetSortOrder(Type type)
	{
		foreach (Attribute attribute in type.GetCustomAttributes(true))
		{
			if (attribute.GetType() == typeof(EntityConfigOrder))
			{
				return (attribute as EntityConfigOrder).sortOrder;
			}
		}
		return 0;
	}

	public void LoadGeneratedEntities(List<Type> types)
	{
		Type typeFromHandle = typeof(IEntityConfig);
		Type typeFromHandle2 = typeof(IMultiEntityConfig);
		List<EntityConfigManager.ConfigEntry> list = new List<EntityConfigManager.ConfigEntry>();
		foreach (Type type in types)
		{
			if ((typeFromHandle.IsAssignableFrom(type) || typeFromHandle2.IsAssignableFrom(type)) && !type.IsAbstract && !type.IsInterface)
			{
				int sortOrder = EntityConfigManager.GetSortOrder(type);
				EntityConfigManager.ConfigEntry configEntry = new EntityConfigManager.ConfigEntry
				{
					type = type,
					sortOrder = sortOrder
				};
				list.Add(configEntry);
			}
		}
		list.Sort((EntityConfigManager.ConfigEntry x, EntityConfigManager.ConfigEntry y) => x.sortOrder.CompareTo(y.sortOrder));
		foreach (EntityConfigManager.ConfigEntry configEntry2 in list)
		{
			object obj = Activator.CreateInstance(configEntry2.type);
			if (obj is IEntityConfig)
			{
				IEntityConfig entityConfig = obj as IEntityConfig;
				string[] array = null;
				string[] array2 = null;
				if (entityConfig.GetDlcIds() != null)
				{
					DlcManager.ConvertAvailableToRequireAndForbidden(entityConfig.GetDlcIds(), out array, out array2);
					DebugUtil.DevLogError(string.Format("{0} implements GetDlcIds, which is obsolete.", configEntry2.type));
				}
				else
				{
					IHasDlcRestrictions hasDlcRestrictions = obj as IHasDlcRestrictions;
					if (hasDlcRestrictions != null)
					{
						array = hasDlcRestrictions.GetRequiredDlcIds();
						array2 = hasDlcRestrictions.GetForbiddenDlcIds();
					}
				}
				if (DlcManager.IsCorrectDlcSubscribed(array, array2))
				{
					this.RegisterEntity(entityConfig, array, array2);
				}
			}
			IMultiEntityConfig multiEntityConfig = obj as IMultiEntityConfig;
			if (multiEntityConfig != null)
			{
				DebugUtil.Assert(!(obj is IHasDlcRestrictions), "IMultiEntityConfig cannot implement IHasDlcRestrictions, wrap the individual config instead.");
				this.RegisterEntities(multiEntityConfig);
			}
		}
	}

	[Conditional("UNITY_EDITOR")]
	private void ValidateEntityConfig(IEntityConfig entityConfig)
	{
		if (entityConfig == null)
		{
			throw new ArgumentNullException("entityConfig");
		}
		Type type = entityConfig.GetType();
		Type typeFromHandle = typeof(IHasDlcRestrictions);
		bool flag = type.GetMethod("GetRequiredDlcIds", Type.EmptyTypes) != null;
		bool flag2 = type.GetMethod("GetForbiddenDlcIds", Type.EmptyTypes) != null;
		bool flag3 = typeFromHandle.IsAssignableFrom(type);
		if ((flag || flag2) && !flag3)
		{
			DebugUtil.LogErrorArgs(new object[] { type.Name + " is an IEntityConfig and has GetRequiredDlcIds or GetForbiddenDlcIds but does not implement IHasDlcRestrictions." });
		}
	}

	[Conditional("UNITY_EDITOR")]
	private void ValidateMultiEntityConfig(IMultiEntityConfig entityConfig)
	{
		if (entityConfig == null)
		{
			throw new ArgumentNullException("entityConfig");
		}
		Type type = entityConfig.GetType();
		bool flag = type.GetMethod("GetRequiredDlcIds", Type.EmptyTypes) != null;
		bool flag2 = type.GetMethod("GetForbiddenDlcIds", Type.EmptyTypes) != null;
		if (flag || flag2)
		{
			DebugUtil.LogErrorArgs(new object[] { type.Name + " is an IMultiEntityConfig and you shouldn't be specifying GetRequiredDlcIds or GetForbiddenDlcIds. Wrap each config in a DLC check instead." });
		}
	}

	public void RegisterEntity(IEntityConfig config, string[] requiredDlcIds = null, string[] forbiddenDlcIds = null)
	{
		GameObject gameObject = config.CreatePrefab();
		if (gameObject == null)
		{
			return;
		}
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.requiredDlcIds = requiredDlcIds;
		component.forbiddenDlcIds = forbiddenDlcIds;
		component.prefabInitFn += config.OnPrefabInit;
		component.prefabSpawnFn += config.OnSpawn;
		Assets.AddPrefab(component);
	}

	public void RegisterEntities(IMultiEntityConfig config)
	{
		foreach (GameObject gameObject in config.CreatePrefabs())
		{
			KPrefabID component = gameObject.GetComponent<KPrefabID>();
			component.prefabInitFn += config.OnPrefabInit;
			component.prefabSpawnFn += config.OnSpawn;
			Assets.AddPrefab(component);
		}
	}

	public static EntityConfigManager Instance;

	private struct ConfigEntry
	{
		public Type type;

		public int sortOrder;
	}
}
