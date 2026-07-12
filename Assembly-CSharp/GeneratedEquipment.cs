using System;
using System.Collections.Generic;

public class GeneratedEquipment
{
	public static void LoadGeneratedEquipment(List<Type> types)
	{
		Type typeFromHandle = typeof(IEquipmentConfig);
		List<Type> list = new List<Type>();
		foreach (Type type in types)
		{
			if (typeFromHandle.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
			{
				list.Add(type);
			}
		}
		foreach (Type type2 in list)
		{
			object obj = Activator.CreateInstance(type2);
			try
			{
				EquipmentConfigManager.Instance.RegisterEquipment(obj as IEquipmentConfig);
			}
			catch (Exception ex)
			{
				DebugUtil.LogException(null, "Exception in RegisterEquipment for type " + type2.FullName + " from " + type2.Assembly.GetName().Name, ex);
			}
		}
	}
}
