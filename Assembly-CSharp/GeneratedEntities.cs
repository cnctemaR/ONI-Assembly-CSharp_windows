using System;
using System.Reflection;
using TUNING;

public class GeneratedEntities
{
	public static void LoadGeneratedEntities()
	{
		Type typeFromHandle = typeof(IEntityConfig);
		Assembly assembly = Assembly.GetAssembly(typeof(GeneratedEntities));
		Type[] types = assembly.GetTypes();
		foreach (Type type in types)
		{
			if (typeFromHandle.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
			{
				object obj = Activator.CreateInstance(type);
				EntityConfigManager.Instance.RegisterEntity(obj as IEntityConfig);
			}
		}
		Type typeFromHandle2 = typeof(IEffectDescriptor);
		Type typeFromHandle3 = typeof(IGameObjectEffectDescriptor);
		foreach (Type type2 in types)
		{
			if ((typeFromHandle2.IsAssignableFrom(type2) || typeFromHandle3.IsAssignableFrom(type2)) && !type2.IsAbstract && !type2.IsInterface)
			{
				string name = type2.Name;
				if (!BUILDINGS.COMPONENT_DESCRIPTION_ORDER.Contains(name))
				{
					Debug.LogWarning(string.Format("Component [{0}] is an effect descriptor but is missing from TUNING.BUILDINGS.COMPONENT_DESCRIPTION_ORDER!", name), null);
				}
			}
		}
	}
}
