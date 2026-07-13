using System;
using System.Collections.Generic;

public interface IDevQuickAction
{
	List<DevQuickActionInstruction> GetDevInstructions();

	public enum CommonMenusNames
	{
		Storage
	}
}
