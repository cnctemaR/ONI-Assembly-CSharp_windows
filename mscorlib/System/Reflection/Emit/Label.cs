using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[Serializable]
	public struct Label
	{
		internal Label(int val)
		{
			this.label = val;
		}

		public override bool Equals(object obj)
		{
			bool flag = obj is Label;
			if (flag)
			{
				Label label = (Label)obj;
				flag = this.label == label.label;
			}
			return flag;
		}

		public bool Equals(Label obj)
		{
			return this.label == obj.label;
		}

		public override int GetHashCode()
		{
			return this.label.GetHashCode();
		}

		public static bool operator ==(Label a, Label b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Label a, Label b)
		{
			return !(a == b);
		}

		internal int label;
	}
}
