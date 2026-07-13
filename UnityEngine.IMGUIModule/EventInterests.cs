using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal struct EventInterests
	{
		public bool wantsMouseMove { readonly get; set; }

		public bool wantsMouseEnterLeaveWindow { readonly get; set; }

		public bool wantsLessLayoutEvents { readonly get; set; }

		public bool WantsEvent(EventType type)
		{
			bool flag;
			if (type != EventType.MouseMove)
			{
				flag = type - EventType.MouseEnterWindow > 1 || this.wantsMouseEnterLeaveWindow;
			}
			else
			{
				flag = this.wantsMouseMove;
			}
			return flag;
		}

		public bool WantsLayoutPass(EventType type)
		{
			bool flag = !this.wantsLessLayoutEvents;
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				switch (type)
				{
				case EventType.MouseDown:
				case EventType.MouseUp:
					return this.wantsMouseMove;
				case EventType.MouseMove:
				case EventType.MouseDrag:
				case EventType.ScrollWheel:
					goto IL_006C;
				case EventType.KeyDown:
				case EventType.KeyUp:
					return GUIUtility.textFieldInput;
				case EventType.Repaint:
					break;
				default:
					if (type != EventType.ExecuteCommand)
					{
						if (type - EventType.MouseEnterWindow > 1)
						{
							goto IL_006C;
						}
						return this.wantsMouseEnterLeaveWindow;
					}
					break;
				}
				return true;
				IL_006C:
				flag2 = false;
			}
			return flag2;
		}
	}
}
