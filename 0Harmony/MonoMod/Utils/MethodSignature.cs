using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using MonoMod.Logs;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class MethodSignature : IEquatable<MethodSignature>, IDebugFormattable
	{
		public Type ReturnType { get; }

		public int ParameterCount
		{
			get
			{
				return this.parameters.Length;
			}
		}

		public IEnumerable<Type> Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		[Nullable(2)]
		public Type FirstParameter
		{
			[NullableContext(2)]
			get
			{
				if (this.parameters.Length < 1)
				{
					return null;
				}
				return this.parameters[0];
			}
		}

		public MethodSignature(Type returnType, Type[] parameters)
		{
			this.ReturnType = returnType;
			this.parameters = parameters;
		}

		public MethodSignature(Type returnType, IEnumerable<Type> parameters)
		{
			this.ReturnType = returnType;
			this.parameters = parameters.ToArray<Type>();
		}

		public MethodSignature(MethodBase method)
			: this(method, false)
		{
		}

		public MethodSignature(MethodBase method, bool ignoreThis)
		{
			MethodInfo methodInfo = method as MethodInfo;
			this.ReturnType = ((methodInfo != null) ? methodInfo.ReturnType : null) ?? typeof(void);
			int num = ((ignoreThis || method.IsStatic) ? 0 : 1);
			ParameterInfo[] array = method.GetParameters();
			this.parameters = new Type[array.Length + num];
			for (int i = num; i < this.parameters.Length; i++)
			{
				this.parameters[i] = array[i - num].ParameterType;
			}
			if (!ignoreThis && !method.IsStatic)
			{
				this.parameters[0] = method.GetThisParamType();
			}
		}

		public static MethodSignature ForMethod(MethodBase method)
		{
			return MethodSignature.ForMethod(method, false);
		}

		public static MethodSignature ForMethod(MethodBase method, bool ignoreThis)
		{
			return (ignoreThis ? MethodSignature.noThisSigMap : MethodSignature.thisSigMap).GetValue(method, (MethodBase m) => new MethodSignature(m, ignoreThis));
		}

		public bool IsCompatibleWith(MethodSignature other)
		{
			Helpers.ThrowIfArgumentNull<MethodSignature>(other, "other");
			return this == other || (this.ReturnType.IsCompatible(other.ReturnType) && this.parameters.SequenceEqual<Type>(other.Parameters, MethodSignature.CompatableComparer.Instance));
		}

		public DynamicMethodDefinition CreateDmd(string name)
		{
			return new DynamicMethodDefinition(name, this.ReturnType, this.parameters);
		}

		public override string ToString()
		{
			int num = 2 + this.parameters.Length - 1;
			int num2 = 1 + this.parameters.Length;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(num, num2);
			defaultInterpolatedStringHandler.AppendFormatted<Type>(this.ReturnType);
			defaultInterpolatedStringHandler.AppendLiteral(" (");
			for (int i = 0; i < this.parameters.Length; i++)
			{
				if (i != 0)
				{
					defaultInterpolatedStringHandler.AppendLiteral(", ");
				}
				defaultInterpolatedStringHandler.AppendFormatted<Type>(this.parameters[i]);
			}
			defaultInterpolatedStringHandler.AppendLiteral(")");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}

		[NullableContext(0)]
		unsafe bool IDebugFormattable.TryFormatInto(global::System.Span<char> span, out int wrote)
		{
			wrote = 0;
			global::System.Span<char> span2 = span;
			global::System.Span<char> span3 = span2;
			bool flag;
			FormatIntoInterpolatedStringHandler formatIntoInterpolatedStringHandler = new FormatIntoInterpolatedStringHandler(2, 1, span2, out flag);
			if (flag && formatIntoInterpolatedStringHandler.AppendFormatted<Type>(this.ReturnType))
			{
				formatIntoInterpolatedStringHandler.AppendLiteral(" (");
			}
			int num;
			if (!DebugFormatter.Into(span3, out num, ref formatIntoInterpolatedStringHandler))
			{
				return false;
			}
			wrote += num;
			for (int i = 0; i < this.parameters.Length; i++)
			{
				if (i != 0)
				{
					if (!", ".AsSpan().TryCopyTo(span.Slice(wrote)))
					{
						return false;
					}
					wrote += 2;
				}
				span2 = span.Slice(wrote);
				global::System.Span<char> span4 = span2;
				bool flag2;
				FormatIntoInterpolatedStringHandler formatIntoInterpolatedStringHandler2 = new FormatIntoInterpolatedStringHandler(0, 1, span2, out flag2);
				if (flag2)
				{
					formatIntoInterpolatedStringHandler2.AppendFormatted<Type>(this.parameters[i]);
				}
				if (!DebugFormatter.Into(span4, out num, ref formatIntoInterpolatedStringHandler2))
				{
					return false;
				}
				wrote += num;
			}
			if (span.Slice(wrote).Length < 1)
			{
				return false;
			}
			int num2 = wrote;
			wrote = num2 + 1;
			*span[num2] = ')';
			return true;
		}

		[NullableContext(2)]
		public bool Equals(MethodSignature other)
		{
			return other != null && (this == other || (this.ReturnType.Equals(other.ReturnType) && this.Parameters.SequenceEqual<Type>(other.Parameters)));
		}

		[NullableContext(2)]
		public override bool Equals(object obj)
		{
			MethodSignature methodSignature = obj as MethodSignature;
			return methodSignature != null && this.Equals(methodSignature);
		}

		public override int GetHashCode()
		{
			global::System.HashCode hashCode = default(global::System.HashCode);
			hashCode.Add<Type>(this.ReturnType);
			hashCode.Add<int>(this.parameters.Length);
			foreach (Type type in this.parameters)
			{
				hashCode.Add<Type>(type);
			}
			return hashCode.ToHashCode();
		}

		private readonly Type[] parameters;

		private static readonly ConditionalWeakTable<MethodBase, MethodSignature> thisSigMap = new ConditionalWeakTable<MethodBase, MethodSignature>();

		private static readonly ConditionalWeakTable<MethodBase, MethodSignature> noThisSigMap = new ConditionalWeakTable<MethodBase, MethodSignature>();

		[Nullable(0)]
		private sealed class CompatableComparer : IEqualityComparer<Type>
		{
			[NullableContext(2)]
			public bool Equals(Type x, Type y)
			{
				return x == y || (x != null && y != null && x.IsCompatible(y));
			}

			public int GetHashCode([global::System.Diagnostics.CodeAnalysis.DisallowNull] Type obj)
			{
				throw new NotSupportedException();
			}

			public static readonly MethodSignature.CompatableComparer Instance = new MethodSignature.CompatableComparer();
		}
	}
}
