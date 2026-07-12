using System;

namespace HarmonyLib
{
	public class Traverse<T>
	{
		private Traverse()
		{
		}

		public Traverse(Traverse traverse)
		{
			this.traverse = traverse;
		}

		public T Value
		{
			get
			{
				return this.traverse.GetValue<T>();
			}
			set
			{
				this.traverse.SetValue(value);
			}
		}

		private readonly Traverse traverse;
	}
}
