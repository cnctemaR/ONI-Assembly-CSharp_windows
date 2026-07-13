using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections
{
	[GenerateTestsForBurstCompatibility]
	public static class FixedString
	{
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, int arg1, int arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, int arg1, int arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, int arg1, int arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, int arg1, int arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, float arg1, int arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, float arg1, int arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, float arg1, int arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, float arg1, int arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, string arg1, int arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, string arg1, int arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, string arg1, int arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, string arg1, int arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, T1 arg1, int arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, T1 arg1, int arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, T1 arg1, int arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, T2 arg1, int arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, T2, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in arg1, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, int arg1, float arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, int arg1, float arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, int arg1, float arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, int arg1, float arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, float arg1, float arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, float arg1, float arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, float arg1, float arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, float arg1, float arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, string arg1, float arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, string arg1, float arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, string arg1, float arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, string arg1, float arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, T1 arg1, float arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, T1 arg1, float arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, T1 arg1, float arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, T2 arg1, float arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, T2, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in arg1, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, int arg1, string arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, int arg1, string arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, int arg1, string arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, int arg1, string arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, float arg1, string arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, float arg1, string arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, float arg1, string arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, float arg1, string arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, string arg1, string arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, string arg1, string arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, string arg1, string arg2, int arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, string arg1, string arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, T1 arg1, string arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, T1 arg1, string arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, T1 arg1, string arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, T2 arg1, string arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, T2, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in arg1, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, int arg1, T1 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, int arg1, T1 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, int arg1, T1 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, int arg1, T2 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, T2, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in arg2, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, float arg1, T1 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, float arg1, T1 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, float arg1, T1 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, float arg1, T2 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, T2, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in arg2, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, string arg1, T1 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, string arg1, T1 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, string arg1, T1 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, string arg1, T2 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, T2, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in arg2, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, int arg0, T1 arg1, T2 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, T2, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in arg2, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, float arg0, T1 arg1, T2 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, T2, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in arg2, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, string arg0, T1 arg1, T2 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, T2, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in arg2, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2, [global::System.Runtime.CompilerServices.IsUnmanaged] T3>(FixedString512Bytes formatString, T1 arg0, T2 arg1, T3 arg2, int arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, T2, T3, FixedString32Bytes>(in formatString, in arg0, in arg1, in arg2, in fixedString32Bytes);
			return fixedString512Bytes;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, int arg1, int arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, int arg1, int arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, int arg1, int arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, int arg1, int arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, float arg1, int arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, float arg1, int arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, float arg1, int arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, float arg1, int arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, string arg1, int arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, string arg1, int arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, string arg1, int arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, string arg1, int arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, T1 arg1, int arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, T1 arg1, int arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, T1 arg1, int arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, T2 arg1, int arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, T2, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in arg1, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, int arg1, float arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, int arg1, float arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, int arg1, float arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, int arg1, float arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, float arg1, float arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, float arg1, float arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, float arg1, float arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, float arg1, float arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, string arg1, float arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, string arg1, float arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, string arg1, float arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, string arg1, float arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, T1 arg1, float arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, T1 arg1, float arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, T1 arg1, float arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, T2 arg1, float arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, T2, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in arg1, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, int arg1, string arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, int arg1, string arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, int arg1, string arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, int arg1, string arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, float arg1, string arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, float arg1, string arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, float arg1, string arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, float arg1, string arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, string arg1, string arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, string arg1, string arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, string arg1, string arg2, float arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, string arg1, string arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, T1 arg1, string arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, T1 arg1, string arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, T1 arg1, string arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, T2 arg1, string arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, T2, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in arg1, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, int arg1, T1 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, int arg1, T1 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, int arg1, T1 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, int arg1, T2 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, T2, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in arg2, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, float arg1, T1 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, float arg1, T1 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, float arg1, T1 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, float arg1, T2 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, T2, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in arg2, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, string arg1, T1 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, string arg1, T1 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, string arg1, T1 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, string arg1, T2 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, T2, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in arg2, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, int arg0, T1 arg1, T2 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, T2, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in arg2, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, float arg0, T1 arg1, T2 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, T2, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in arg2, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, string arg0, T1 arg1, T2 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, T2, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in arg2, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2, [global::System.Runtime.CompilerServices.IsUnmanaged] T3>(FixedString512Bytes formatString, T1 arg0, T2 arg1, T3 arg2, float arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg3, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, T2, T3, FixedString32Bytes>(in formatString, in arg0, in arg1, in arg2, in fixedString32Bytes);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, int arg1, int arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, int arg1, int arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, int arg1, int arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, int arg1, int arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, float arg1, int arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, float arg1, int arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, float arg1, int arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, float arg1, int arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, string arg1, int arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, string arg1, int arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, string arg1, int arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, string arg1, int arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, T1 arg1, int arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, T1 arg1, int arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, T1 arg1, int arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, T2 arg1, int arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, T2, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in arg1, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, int arg1, float arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, int arg1, float arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, int arg1, float arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, int arg1, float arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, float arg1, float arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, float arg1, float arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, float arg1, float arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, float arg1, float arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, string arg1, float arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, string arg1, float arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, string arg1, float arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, string arg1, float arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, T1 arg1, float arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, T1 arg1, float arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, T1 arg1, float arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, T2 arg1, float arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg2, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, T2, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in arg1, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, int arg1, string arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, int arg1, string arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, int arg1, string arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, int arg1, string arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, float arg1, string arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, float arg1, string arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, float arg1, string arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, float arg1, string arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, int arg0, string arg1, string arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, float arg0, string arg1, string arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format(FixedString512Bytes formatString, string arg0, string arg1, string arg2, string arg3)
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes4 = default(FixedString32Bytes);
			(ref fixedString32Bytes4).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in fixedString32Bytes4);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, T1 arg0, string arg1, string arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, T1 arg1, string arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, T1 arg1, string arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, T1 arg1, string arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, T2 arg1, string arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg2);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, T2, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in arg1, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, int arg1, T1 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, int arg1, T1 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, int arg1, T1 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, int arg1, T2 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, T2, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in arg2, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, float arg1, T1 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, float arg1, T1 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, float arg1, T1 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, float arg1, T2 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, T2, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in arg2, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, string arg1, T1 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, string arg1, T1 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, string arg1, T1 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in fixedString32Bytes3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, string arg1, T2 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, T2, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in arg2, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, int arg0, T1 arg1, T2 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, T2, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in arg2, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, float arg0, T1 arg1, T2 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, T2, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in arg2, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, string arg0, T1 arg1, T2 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, T2, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in arg2, in fixedString32Bytes2);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2, [global::System.Runtime.CompilerServices.IsUnmanaged] T3>(FixedString512Bytes formatString, T1 arg0, T2 arg1, T3 arg2, string arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg3);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, T2, T3, FixedString32Bytes>(in formatString, in arg0, in arg1, in arg2, in fixedString32Bytes);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, int arg1, int arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, int arg1, int arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, int arg1, int arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, int arg1, int arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, T2>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, float arg1, int arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, float arg1, int arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, float arg1, int arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, float arg1, int arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, T2>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, string arg1, int arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, string arg1, int arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, string arg1, int arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, string arg1, int arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, T2>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, int arg0, T1 arg1, int arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, T2>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, float arg0, T1 arg1, int arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, T2>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, string arg0, T1 arg1, int arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, T2>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2, [global::System.Runtime.CompilerServices.IsUnmanaged] T3>(FixedString512Bytes formatString, T1 arg0, T2 arg1, int arg2, T3 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, T2, FixedString32Bytes, T3>(in formatString, in arg0, in arg1, in fixedString32Bytes, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, int arg1, float arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, int arg1, float arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, int arg1, float arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, int arg1, float arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, T2>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, float arg1, float arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, float arg1, float arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, float arg1, float arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, float arg1, float arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, T2>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, string arg1, float arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, string arg1, float arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, string arg1, float arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, string arg1, float arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, T2>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, int arg0, T1 arg1, float arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, T2>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, float arg0, T1 arg1, float arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, T2>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, string arg0, T1 arg1, float arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, T2>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2, [global::System.Runtime.CompilerServices.IsUnmanaged] T3>(FixedString512Bytes formatString, T1 arg0, T2 arg1, float arg2, T3 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, T2, FixedString32Bytes, T3>(in formatString, in arg0, in arg1, in fixedString32Bytes, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, int arg1, string arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, int arg1, string arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, int arg1, string arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, int arg1, string arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, T2>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, float arg1, string arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, float arg1, string arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, float arg1, string arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, float arg1, string arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, T2>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, int arg0, string arg1, string arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, float arg0, string arg1, string arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString512Bytes formatString, string arg0, string arg1, string arg2, T1 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, T1 arg0, string arg1, string arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, FixedString32Bytes, T2>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, int arg0, T1 arg1, string arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, T2>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, float arg0, T1 arg1, string arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, T2>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, string arg0, T1 arg1, string arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, FixedString32Bytes, T2>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2, [global::System.Runtime.CompilerServices.IsUnmanaged] T3>(FixedString512Bytes formatString, T1 arg0, T2 arg1, string arg2, T3 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg2);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, T2, FixedString32Bytes, T3>(in formatString, in arg0, in arg1, in fixedString32Bytes, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, int arg0, int arg1, T1 arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, T2>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, float arg0, int arg1, T1 arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, T2>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, string arg0, int arg1, T1 arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, T2>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2, [global::System.Runtime.CompilerServices.IsUnmanaged] T3>(FixedString512Bytes formatString, T1 arg0, int arg1, T2 arg2, T3 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, T2, T3>(in formatString, in arg0, in fixedString32Bytes, in arg2, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, int arg0, float arg1, T1 arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, T2>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, float arg0, float arg1, T1 arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, T2>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, string arg0, float arg1, T1 arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, T2>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2, [global::System.Runtime.CompilerServices.IsUnmanaged] T3>(FixedString512Bytes formatString, T1 arg0, float arg1, T2 arg2, T3 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, T2, T3>(in formatString, in arg0, in fixedString32Bytes, in arg2, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, int arg0, string arg1, T1 arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, T2>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, float arg0, string arg1, T1 arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, T2>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString512Bytes formatString, string arg0, string arg1, T1 arg2, T2 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, FixedString32Bytes, T1, T2>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2, [global::System.Runtime.CompilerServices.IsUnmanaged] T3>(FixedString512Bytes formatString, T1 arg0, string arg1, T2 arg2, T3 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, FixedString32Bytes, T2, T3>(in formatString, in arg0, in fixedString32Bytes, in arg2, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2, [global::System.Runtime.CompilerServices.IsUnmanaged] T3>(FixedString512Bytes formatString, int arg0, T1 arg1, T2 arg2, T3 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, T2, T3>(in formatString, in fixedString32Bytes, in arg1, in arg2, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2, [global::System.Runtime.CompilerServices.IsUnmanaged] T3>(FixedString512Bytes formatString, float arg0, T1 arg1, T2 arg2, T3 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, T2, T3>(in formatString, in fixedString32Bytes, in arg1, in arg2, in arg3);
			return fixedString512Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2, [global::System.Runtime.CompilerServices.IsUnmanaged] T3>(FixedString512Bytes formatString, string arg0, T1 arg1, T2 arg2, T3 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, FixedString32Bytes, T1, T2, T3>(in formatString, in fixedString32Bytes, in arg1, in arg2, in arg3);
			return fixedString512Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString512Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2, [global::System.Runtime.CompilerServices.IsUnmanaged] T3, [global::System.Runtime.CompilerServices.IsUnmanaged] T4>(FixedString512Bytes formatString, T1 arg0, T2 arg1, T3 arg2, T4 arg3) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T4 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString512Bytes fixedString512Bytes = default(FixedString512Bytes);
			(ref fixedString512Bytes).AppendFormat<FixedString512Bytes, FixedString512Bytes, T1, T2, T3, T4>(in formatString, in arg0, in arg1, in arg2, in arg3);
			return fixedString512Bytes;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, int arg1, int arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, int arg1, int arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, int arg1, int arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, int arg1, int arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, float arg1, int arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, float arg1, int arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, float arg1, int arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, float arg1, int arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, string arg1, int arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, string arg1, int arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, string arg1, int arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, string arg1, int arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, int arg0, T1 arg1, int arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, float arg0, T1 arg1, int arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, string arg0, T1 arg1, int arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString128Bytes formatString, T1 arg0, T2 arg1, int arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, T1, T2, FixedString32Bytes>(in formatString, in arg0, in arg1, in fixedString32Bytes);
			return fixedString128Bytes;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, int arg1, float arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, int arg1, float arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, int arg1, float arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, int arg1, float arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, float arg1, float arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, float arg1, float arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, float arg1, float arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, float arg1, float arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, string arg1, float arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, string arg1, float arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, string arg1, float arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, string arg1, float arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, int arg0, T1 arg1, float arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, float arg0, T1 arg1, float arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, string arg0, T1 arg1, float arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString128Bytes formatString, T1 arg0, T2 arg1, float arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg2, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, T1, T2, FixedString32Bytes>(in formatString, in arg0, in arg1, in fixedString32Bytes);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, int arg1, string arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, int arg1, string arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, int arg1, string arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, int arg1, string arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, float arg1, string arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, float arg1, string arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, float arg1, string arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, float arg1, string arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, string arg1, string arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, string arg1, string arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, string arg1, string arg2)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes3 = default(FixedString32Bytes);
			(ref fixedString32Bytes3).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in fixedString32Bytes3);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, string arg1, string arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, T1, FixedString32Bytes, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, int arg0, T1 arg1, string arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, float arg0, T1 arg1, string arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, string arg0, T1 arg1, string arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, T1, FixedString32Bytes>(in formatString, in fixedString32Bytes, in arg1, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString128Bytes formatString, T1 arg0, T2 arg1, string arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg2);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, T1, T2, FixedString32Bytes>(in formatString, in arg0, in arg1, in fixedString32Bytes);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, int arg0, int arg1, T1 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, float arg0, int arg1, T1 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, string arg0, int arg1, T1 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString128Bytes formatString, T1 arg0, int arg1, T2 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, T1, FixedString32Bytes, T2>(in formatString, in arg0, in fixedString32Bytes, in arg2);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, int arg0, float arg1, T1 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, float arg0, float arg1, T1 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, string arg0, float arg1, T1 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString128Bytes formatString, T1 arg0, float arg1, T2 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, T1, FixedString32Bytes, T2>(in formatString, in arg0, in fixedString32Bytes, in arg2);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, int arg0, string arg1, T1 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, float arg0, string arg1, T1 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, string arg0, string arg1, T1 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in fixedString32Bytes2, in arg2);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString128Bytes formatString, T1 arg0, string arg1, T2 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, T1, FixedString32Bytes, T2>(in formatString, in arg0, in fixedString32Bytes, in arg2);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString128Bytes formatString, int arg0, T1 arg1, T2 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, T1, T2>(in formatString, in fixedString32Bytes, in arg1, in arg2);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString128Bytes formatString, float arg0, T1 arg1, T2 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, T1, T2>(in formatString, in fixedString32Bytes, in arg1, in arg2);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString128Bytes formatString, string arg0, T1 arg1, T2 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, T1, T2>(in formatString, in fixedString32Bytes, in arg1, in arg2);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2, [global::System.Runtime.CompilerServices.IsUnmanaged] T3>(FixedString128Bytes formatString, T1 arg0, T2 arg1, T3 arg2) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T3 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, T1, T2, T3>(in formatString, in arg0, in arg1, in arg2);
			return fixedString128Bytes;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, int arg1)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, int arg1)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, int arg1)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, int arg1) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, T1, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes);
			return fixedString128Bytes;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, float arg1)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, float arg1)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, float arg1)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, float arg1) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, T1, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0, string arg1)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0, string arg1)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0, string arg1)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			FixedString32Bytes fixedString32Bytes2 = default(FixedString32Bytes);
			(ref fixedString32Bytes2).Append<FixedString32Bytes>(arg1);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes, in fixedString32Bytes2);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0, string arg1) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg1);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, T1, FixedString32Bytes>(in formatString, in arg0, in fixedString32Bytes);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, int arg0, T1 arg1) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in arg1);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, float arg0, T1 arg1) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in arg1);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, string arg0, T1 arg1) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes, T1>(in formatString, in fixedString32Bytes, in arg1);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(FixedString32Bytes),
			typeof(FixedString32Bytes)
		})]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(FixedString128Bytes formatString, T1 arg0, T2 arg1) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes where T2 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, T1, T2>(in formatString, in arg0, in arg1);
			return fixedString128Bytes;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, int arg0)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes);
			return fixedString128Bytes;
		}

		public static FixedString128Bytes Format(FixedString128Bytes formatString, float arg0)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0, '.');
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes);
			return fixedString128Bytes;
		}

		[ExcludeFromBurstCompatTesting("Takes managed string")]
		public static FixedString128Bytes Format(FixedString128Bytes formatString, string arg0)
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			(ref fixedString32Bytes).Append<FixedString32Bytes>(arg0);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, FixedString32Bytes>(in formatString, in fixedString32Bytes);
			return fixedString128Bytes;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[] { typeof(FixedString32Bytes) })]
		public static FixedString128Bytes Format<[global::System.Runtime.CompilerServices.IsUnmanaged] T1>(FixedString128Bytes formatString, T1 arg0) where T1 : struct, ValueType, INativeList<byte>, IUTF8Bytes
		{
			FixedString128Bytes fixedString128Bytes = default(FixedString128Bytes);
			(ref fixedString128Bytes).AppendFormat<FixedString128Bytes, FixedString128Bytes, T1>(in formatString, in arg0);
			return fixedString128Bytes;
		}
	}
}
