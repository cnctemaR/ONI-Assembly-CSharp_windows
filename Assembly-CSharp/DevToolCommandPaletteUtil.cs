using System;
using System.Collections.Generic;

public static class DevToolCommandPaletteUtil
{
	public static List<DevToolCommandPalette.Command> GenerateDefaultCommandPalette()
	{
		List<DevToolCommandPalette.Command> list = new List<DevToolCommandPalette.Command>();
		using (IEnumerator<DevTool> enumerator = DevToolManager.Instance.GetDevTools().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				DevTool dev_tool = enumerator.Current;
				list.Add(new DevToolCommandPalette.Command("[DevTool] Open \"" + dev_tool.Name + "\"", new string[]
				{
					dev_tool.FullPath,
					dev_tool.GetType().Name
				}, delegate
				{
					dev_tool.Enabled = true;
				}));
			}
		}
		return list;
	}
}
