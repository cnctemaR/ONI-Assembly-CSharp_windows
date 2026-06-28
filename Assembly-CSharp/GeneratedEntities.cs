using System;
using System.Reflection;

public class GeneratedEntities
{
	public static void LoadGeneratedEntities()
	{
		Type typeFromHandle = typeof(IEntityConfig);
		Assembly assembly = Assembly.GetAssembly(typeof(PropaneSpoutConfig));
		Type[] types = assembly.GetTypes();
		foreach (Type type in types)
		{
			if (typeFromHandle.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
			{
				object obj = Activator.CreateInstance(type);
				EntityConfigManager.Instance.RegisterEntity(obj as IEntityConfig);
			}
		}
	}
}
