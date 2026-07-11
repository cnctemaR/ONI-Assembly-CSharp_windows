using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal struct EventInterests
	{
		public bool wantsMouseMove { get; set; }

		public bool wantsMouseEnterLeaveWindow { get; set; }

		public bool WantsEvent(EventType type)
		{
			bool flag;
			if (type != EventType.MouseEnterWindow && type != EventType.MouseLeaveWindow)
			{
				flag = type != EventType.MouseMove || this.wantsMouseMove;
			}
			else
			{
				flag = this.wantsMouseEnterLeaveWindow;
			}
			return flag;
		}
	}
}
