using System;

namespace UnityEngine.UIElements
{
	public struct ManipulatorActivationFilter : IEquatable<ManipulatorActivationFilter>
	{
		public MouseButton button { get; set; }

		public EventModifiers modifiers { get; set; }

		public int clickCount { get; set; }

		public override bool Equals(object obj)
		{
			return obj is ManipulatorActivationFilter && this.Equals((ManipulatorActivationFilter)obj);
		}

		public bool Equals(ManipulatorActivationFilter other)
		{
			return this.button == other.button && this.modifiers == other.modifiers && this.clickCount == other.clickCount;
		}

		public override int GetHashCode()
		{
			int num = 390957112;
			num = num * -1521134295 + this.button.GetHashCode();
			num = num * -1521134295 + this.modifiers.GetHashCode();
			return num * -1521134295 + this.clickCount.GetHashCode();
		}

		public bool Matches(IMouseEvent e)
		{
			bool flag = e == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = this.clickCount == 0 || e.clickCount >= this.clickCount;
				flag2 = this.button == (MouseButton)e.button && this.HasModifiers(e) && flag3;
			}
			return flag2;
		}

		private bool HasModifiers(IMouseEvent e)
		{
			bool flag = e == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = ((this.modifiers & EventModifiers.Alt) != EventModifiers.None && !e.altKey) || ((this.modifiers & EventModifiers.Alt) == EventModifiers.None && e.altKey);
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					bool flag4 = ((this.modifiers & EventModifiers.Control) != EventModifiers.None && !e.ctrlKey) || ((this.modifiers & EventModifiers.Control) == EventModifiers.None && e.ctrlKey);
					if (flag4)
					{
						flag2 = false;
					}
					else
					{
						bool flag5 = ((this.modifiers & EventModifiers.Shift) != EventModifiers.None && !e.shiftKey) || ((this.modifiers & EventModifiers.Shift) == EventModifiers.None && e.shiftKey);
						flag2 = !flag5 && ((this.modifiers & EventModifiers.Command) == EventModifiers.None || e.commandKey) && ((this.modifiers & EventModifiers.Command) != EventModifiers.None || !e.commandKey);
					}
				}
			}
			return flag2;
		}

		public static bool operator ==(ManipulatorActivationFilter filter1, ManipulatorActivationFilter filter2)
		{
			return filter1.Equals(filter2);
		}

		public static bool operator !=(ManipulatorActivationFilter filter1, ManipulatorActivationFilter filter2)
		{
			return !(filter1 == filter2);
		}
	}
}
