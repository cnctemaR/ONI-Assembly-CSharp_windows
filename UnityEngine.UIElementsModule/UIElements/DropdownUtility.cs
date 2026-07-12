using System;

namespace UnityEngine.UIElements
{
	internal static class DropdownUtility
	{
		internal static IGenericMenu CreateDropdown()
		{
			IGenericMenu genericMenu2;
			if (DropdownUtility.MakeDropdownFunc == null)
			{
				IGenericMenu genericMenu = new GenericDropdownMenu();
				genericMenu2 = genericMenu;
			}
			else
			{
				genericMenu2 = DropdownUtility.MakeDropdownFunc();
			}
			return genericMenu2;
		}

		internal static Func<IGenericMenu> MakeDropdownFunc;
	}
}
