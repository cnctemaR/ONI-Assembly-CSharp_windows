using System;

namespace UnityEngine
{
	public class EnumButtonsAttribute : PropertyAttribute
	{
		public EnumButtonsAttribute(bool includeObsolete = false)
		{
			this.includeObsolete = includeObsolete;
		}

		public bool includeObsolete;
	}
}
