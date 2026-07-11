using System;
using System.Text;

namespace System
{
	internal class PointerSpec : ModifierSpec
	{
		internal PointerSpec(int pointer_level)
		{
			this.pointer_level = pointer_level;
		}

		public Type Resolve(Type type)
		{
			for (int i = 0; i < this.pointer_level; i++)
			{
				type = type.MakePointerType();
			}
			return type;
		}

		public StringBuilder Append(StringBuilder sb)
		{
			return sb.Append('*', this.pointer_level);
		}

		public override string ToString()
		{
			return this.Append(new StringBuilder()).ToString();
		}

		private int pointer_level;
	}
}
