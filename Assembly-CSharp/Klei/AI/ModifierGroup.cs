using System;
using System.Collections.Generic;

namespace Klei.AI
{
	public class ModifierGroup<T> : Resource
	{
		public IEnumerator<T> GetEnumerator()
		{
			return this.modifiers.GetEnumerator();
		}

		public T this[int idx]
		{
			get
			{
				return this.modifiers[idx];
			}
		}

		public int Count
		{
			get
			{
				return this.modifiers.Count;
			}
		}

		public ModifierGroup(string id, string name)
			: base(id, name)
		{
		}

		public void Add(T modifier)
		{
			this.modifiers.Add(modifier);
		}

		public List<T> modifiers = new List<T>();
	}
}
