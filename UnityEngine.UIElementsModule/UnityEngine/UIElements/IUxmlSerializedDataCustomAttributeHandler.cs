using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal interface IUxmlSerializedDataCustomAttributeHandler
	{
		void SerializeCustomAttributes(IUxmlAttributes bag, HashSet<string> handledAttributes);
	}
}
