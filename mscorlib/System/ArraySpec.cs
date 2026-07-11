using System;
using System.Text;

namespace System
{
	internal class ArraySpec : ModifierSpec
	{
		internal ArraySpec(int dimensions, bool bound)
		{
			this.dimensions = dimensions;
			this.bound = bound;
		}

		public Type Resolve(Type type)
		{
			if (this.bound)
			{
				return type.MakeArrayType(1);
			}
			if (this.dimensions == 1)
			{
				return type.MakeArrayType();
			}
			return type.MakeArrayType(this.dimensions);
		}

		public StringBuilder Append(StringBuilder sb)
		{
			if (this.bound)
			{
				return sb.Append("[*]");
			}
			return sb.Append('[').Append(',', this.dimensions - 1).Append(']');
		}

		public override string ToString()
		{
			return this.Append(new StringBuilder()).ToString();
		}

		public int Rank
		{
			get
			{
				return this.dimensions;
			}
		}

		public bool IsBound
		{
			get
			{
				return this.bound;
			}
		}

		private int dimensions;

		private bool bound;
	}
}
