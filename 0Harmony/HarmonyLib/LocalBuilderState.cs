using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace HarmonyLib
{
	internal class LocalBuilderState
	{
		public void Add(string key, LocalBuilder local)
		{
			this.locals[key] = local;
		}

		public bool TryGetValue(string key, out LocalBuilder local)
		{
			return this.locals.TryGetValue(key, out local);
		}

		public LocalBuilder this[string key]
		{
			get
			{
				return this.locals[key];
			}
			set
			{
				this.locals[key] = value;
			}
		}

		private readonly Dictionary<string, LocalBuilder> locals = new Dictionary<string, LocalBuilder>();
	}
}
