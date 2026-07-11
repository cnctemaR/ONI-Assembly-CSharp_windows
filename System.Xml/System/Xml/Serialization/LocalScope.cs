using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace System.Xml.Serialization
{
	internal class LocalScope
	{
		public LocalScope()
		{
			this.locals = new Dictionary<string, LocalBuilder>();
		}

		public LocalScope(LocalScope parent)
			: this()
		{
			this.parent = parent;
		}

		public void Add(string key, LocalBuilder value)
		{
			this.locals.Add(key, value);
		}

		public bool ContainsKey(string key)
		{
			return this.locals.ContainsKey(key) || (this.parent != null && this.parent.ContainsKey(key));
		}

		public bool TryGetValue(string key, out LocalBuilder value)
		{
			if (this.locals.TryGetValue(key, out value))
			{
				return true;
			}
			if (this.parent != null)
			{
				return this.parent.TryGetValue(key, out value);
			}
			value = null;
			return false;
		}

		public LocalBuilder this[string key]
		{
			get
			{
				LocalBuilder localBuilder;
				this.TryGetValue(key, out localBuilder);
				return localBuilder;
			}
			set
			{
				this.locals[key] = value;
			}
		}

		public void AddToFreeLocals(Dictionary<Tuple<Type, string>, Queue<LocalBuilder>> freeLocals)
		{
			foreach (KeyValuePair<string, LocalBuilder> keyValuePair in this.locals)
			{
				Tuple<Type, string> tuple = new Tuple<Type, string>(keyValuePair.Value.LocalType, keyValuePair.Key);
				Queue<LocalBuilder> queue;
				if (freeLocals.TryGetValue(tuple, out queue))
				{
					queue.Enqueue(keyValuePair.Value);
				}
				else
				{
					queue = new Queue<LocalBuilder>();
					queue.Enqueue(keyValuePair.Value);
					freeLocals.Add(tuple, queue);
				}
			}
		}

		public readonly LocalScope parent;

		private readonly Dictionary<string, LocalBuilder> locals;
	}
}
