using System;
using System.Collections.Generic;
using System.Reflection;

public static class DevToolCommandPaletteUtil
{
	public static List<DevToolCommandPalette.Command> GenerateDefaultCommandPalette()
	{
		List<DevToolCommandPalette.Command> list = new List<DevToolCommandPalette.Command>();
		using (List<Type>.Enumerator enumerator = ReflectionUtil.CollectTypesThatInheritOrImplement<DevTool>(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Type devToolType = enumerator.Current;
				if (!devToolType.IsAbstract && ReflectionUtil.HasDefaultConstructor(devToolType))
				{
					list.Add(new DevToolCommandPalette.Command("Open DevTool: \"" + DevToolUtil.GenerateDevToolName(devToolType) + "\"", delegate
					{
						DevToolUtil.Open((DevTool)Activator.CreateInstance(devToolType));
					}));
				}
			}
		}
		return list;
	}
}
