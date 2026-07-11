using System;
using System.Collections;

namespace System.Xml.Serialization
{
	internal class TempAssemblyCache
	{
		internal TempAssembly this[string ns, object o]
		{
			get
			{
				return (TempAssembly)this.cache[new TempAssemblyCacheKey(ns, o)];
			}
		}

		internal void Add(string ns, object o, TempAssembly assembly)
		{
			TempAssemblyCacheKey tempAssemblyCacheKey = new TempAssemblyCacheKey(ns, o);
			lock (this)
			{
				if (this.cache[tempAssemblyCacheKey] != assembly)
				{
					Hashtable hashtable = new Hashtable();
					foreach (object obj in this.cache.Keys)
					{
						hashtable.Add(obj, this.cache[obj]);
					}
					this.cache = hashtable;
					this.cache[tempAssemblyCacheKey] = assembly;
				}
			}
		}

		private Hashtable cache = new Hashtable();
	}
}
