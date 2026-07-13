using System;
using System.Diagnostics;
using Unity.Burst.LowLevel;
using Unity.Jobs.LowLevel.Unsafe;

namespace Unity.Burst
{
	public static class BurstRuntime
	{
		public static int GetHashCode32<T>()
		{
			return BurstRuntime.HashCode32<T>.Value;
		}

		public static int GetHashCode32(Type type)
		{
			return BurstRuntime.HashStringWithFNV1A32(type.AssemblyQualifiedName);
		}

		public static long GetHashCode64<T>()
		{
			return BurstRuntime.HashCode64<T>.Value;
		}

		public static long GetHashCode64(Type type)
		{
			return BurstRuntime.HashStringWithFNV1A64(type.AssemblyQualifiedName);
		}

		internal static int HashStringWithFNV1A32(string text)
		{
			uint num = 2166136261U;
			foreach (char c in text)
			{
				num = 16777619U * (num ^ (uint)((byte)(c & 'ÿ')));
				num = 16777619U * (num ^ (uint)((byte)(c >> 8)));
			}
			return (int)num;
		}

		internal static long HashStringWithFNV1A64(string text)
		{
			ulong num = 14695981039346656037UL;
			foreach (char c in text)
			{
				num = 1099511628211UL * (num ^ (ulong)((byte)(c & 'ÿ')));
				num = 1099511628211UL * (num ^ (ulong)((byte)(c >> 8)));
			}
			return (long)num;
		}

		public static bool LoadAdditionalLibrary(string pathToLibBurstGenerated)
		{
			return BurstCompiler.IsLoadAdditionalLibrarySupported() && BurstRuntime.LoadAdditionalLibraryInternal(pathToLibBurstGenerated);
		}

		internal static bool LoadAdditionalLibraryInternal(string pathToLibBurstGenerated)
		{
			return (bool)typeof(BurstCompilerService).GetMethod("LoadBurstLibrary").Invoke(null, new object[] { pathToLibBurstGenerated });
		}

		[BurstRuntime.PreserveAttribute]
		internal unsafe static void RuntimeLog(byte* message, int logType, byte* fileName, int lineNumber)
		{
			BurstCompilerService.RuntimeLog(null, (BurstCompilerService.BurstLogType)logType, message, fileName, lineNumber);
		}

		internal static void Initialize()
		{
		}

		[BurstRuntime.PreserveAttribute]
		internal static void PreventRequiredAttributeStrip()
		{
			new BurstDiscardAttribute();
			new ConditionalAttribute("HEJSA");
			new JobProducerTypeAttribute(typeof(BurstRuntime));
		}

		[BurstRuntime.PreserveAttribute]
		internal unsafe static void Log(byte* message, int logType, byte* fileName, int lineNumber)
		{
			BurstCompilerService.Log(null, (BurstCompilerService.BurstLogType)logType, message, null, lineNumber);
		}

		public unsafe static byte* GetUTF8LiteralPointer(string str, out int byteCount)
		{
			throw new NotImplementedException("This function only works from Burst");
		}

		private struct HashCode32<T>
		{
			public static readonly int Value = BurstRuntime.HashStringWithFNV1A32(typeof(T).AssemblyQualifiedName);
		}

		private struct HashCode64<T>
		{
			public static readonly long Value = BurstRuntime.HashStringWithFNV1A64(typeof(T).AssemblyQualifiedName);
		}

		internal class PreserveAttribute : Attribute
		{
		}
	}
}
