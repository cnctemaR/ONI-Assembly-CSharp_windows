using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace HarmonyLib
{
	internal class VariableState
	{
		public void Add(InjectionType type, LocalBuilder local)
		{
			this.injected[type] = local;
		}

		public void Add(string name, LocalBuilder local)
		{
			this.other[name] = local;
		}

		public bool TryGetValue(InjectionType type, out LocalBuilder local)
		{
			return this.injected.TryGetValue(type, out local);
		}

		public bool TryGetValue(string name, out LocalBuilder local)
		{
			return this.other.TryGetValue(name, out local);
		}

		public LocalBuilder this[InjectionType type]
		{
			get
			{
				LocalBuilder localBuilder;
				if (this.injected.TryGetValue(type, out localBuilder))
				{
					return localBuilder;
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(42, 1);
				defaultInterpolatedStringHandler.AppendLiteral("VariableState: variable of type ");
				defaultInterpolatedStringHandler.AppendFormatted<InjectionType>(type);
				defaultInterpolatedStringHandler.AppendLiteral(" not found");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			set
			{
				this.injected[type] = value;
			}
		}

		public LocalBuilder this[string name]
		{
			get
			{
				LocalBuilder localBuilder;
				if (this.other.TryGetValue(name, out localBuilder))
				{
					return localBuilder;
				}
				throw new ArgumentException("VariableState: variable named '" + name + "' not found");
			}
			set
			{
				this.other[name] = value;
			}
		}

		private readonly Dictionary<InjectionType, LocalBuilder> injected = new Dictionary<InjectionType, LocalBuilder>();

		private readonly Dictionary<string, LocalBuilder> other = new Dictionary<string, LocalBuilder>();
	}
}
