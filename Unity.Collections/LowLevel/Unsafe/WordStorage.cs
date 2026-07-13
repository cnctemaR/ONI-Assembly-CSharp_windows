using System;
using System.Diagnostics;

namespace Unity.Collections.LowLevel.Unsafe
{
	[Obsolete("This storage will no longer be used. (RemovedAfter 2021-06-01)")]
	[DebuggerTypeProxy(typeof(WordStorageDebugView))]
	public struct WordStorage
	{
		[NotBurstCompatible]
		public static ref WordStorage Instance
		{
			get
			{
				WordStorage.Initialize();
				return ref WordStorageStatic.Ref.Data;
			}
		}

		public int Entries
		{
			get
			{
				return this.entries;
			}
		}

		[NotBurstCompatible]
		public static void Initialize()
		{
			if (WordStorageStatic.Ref.Data.buffer.IsCreated)
			{
				return;
			}
			WordStorageStatic.Ref.Data.buffer = new NativeArray<byte>(2097152, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			WordStorageStatic.Ref.Data.entry = new NativeArray<WordStorage.Entry>(16384, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			WordStorageStatic.Ref.Data.hash = new NativeMultiHashMap<int, int>(16384, Allocator.Persistent);
			WordStorage.Clear();
			AppDomain.CurrentDomain.DomainUnload += delegate(object _, EventArgs __)
			{
				WordStorage.Shutdown();
			};
			AppDomain.CurrentDomain.ProcessExit += delegate(object _, EventArgs __)
			{
				WordStorage.Shutdown();
			};
		}

		[NotBurstCompatible]
		public static void Shutdown()
		{
			if (!WordStorageStatic.Ref.Data.buffer.IsCreated)
			{
				return;
			}
			WordStorageStatic.Ref.Data.buffer.Dispose();
			WordStorageStatic.Ref.Data.entry.Dispose();
			WordStorageStatic.Ref.Data.hash.Dispose();
			WordStorageStatic.Ref.Data = default(WordStorage);
		}

		[NotBurstCompatible]
		public static void Clear()
		{
			WordStorage.Initialize();
			WordStorageStatic.Ref.Data.chars = 0;
			WordStorageStatic.Ref.Data.entries = 0;
			WordStorageStatic.Ref.Data.hash.Clear();
			FixedString32Bytes fixedString32Bytes = default(FixedString32Bytes);
			WordStorageStatic.Ref.Data.GetOrCreateIndex<FixedString32Bytes>(ref fixedString32Bytes);
		}

		[NotBurstCompatible]
		public static void Setup()
		{
			WordStorage.Clear();
		}

		public unsafe void GetFixedString<T>(int index, ref T temp) where T : IUTF8Bytes, INativeList<byte>
		{
			WordStorage.Entry entry = this.entry[index];
			temp.Length = entry.length;
			UnsafeUtility.MemCpy((void*)temp.GetUnsafePtr(), (void*)((byte*)this.buffer.GetUnsafePtr<byte>() + entry.offset), (long)temp.Length);
		}

		public int GetIndexFromHashAndFixedString<T>(int h, ref T temp) where T : IUTF8Bytes, INativeList<byte>
		{
			int num;
			NativeMultiHashMapIterator<int> nativeMultiHashMapIterator;
			if (this.hash.TryGetFirstValue(h, out num, out nativeMultiHashMapIterator))
			{
				for (;;)
				{
					WordStorage.Entry entry = this.entry[num];
					if (entry.length == temp.Length)
					{
						int num2 = 0;
						while (num2 < entry.length && temp[num2] == this.buffer[entry.offset + num2])
						{
							num2++;
						}
						if (num2 == temp.Length)
						{
							break;
						}
					}
					if (!this.hash.TryGetNextValue(out num, ref nativeMultiHashMapIterator))
					{
						return -1;
					}
				}
				return num;
			}
			return -1;
		}

		public bool Contains<T>(ref T value) where T : IUTF8Bytes, INativeList<byte>
		{
			int hashCode = value.GetHashCode();
			return this.GetIndexFromHashAndFixedString<T>(hashCode, ref value) != -1;
		}

		[NotBurstCompatible]
		public bool Contains(string value)
		{
			FixedString512Bytes fixedString512Bytes = value;
			return this.Contains<FixedString512Bytes>(ref fixedString512Bytes);
		}

		public int GetOrCreateIndex<T>(ref T value) where T : IUTF8Bytes, INativeList<byte>
		{
			int hashCode = value.GetHashCode();
			int indexFromHashAndFixedString = this.GetIndexFromHashAndFixedString<T>(hashCode, ref value);
			if (indexFromHashAndFixedString != -1)
			{
				return indexFromHashAndFixedString;
			}
			int num = this.chars;
			ushort num2 = (ushort)value.Length;
			int num3;
			for (int i = 0; i < (int)num2; i++)
			{
				num3 = this.chars;
				this.chars = num3 + 1;
				this.buffer[num3] = value[i];
			}
			this.entry[this.entries] = new WordStorage.Entry
			{
				offset = num,
				length = (int)num2
			};
			this.hash.Add(hashCode, this.entries);
			num3 = this.entries;
			this.entries = num3 + 1;
			return num3;
		}

		private NativeArray<byte> buffer;

		private NativeArray<WordStorage.Entry> entry;

		private NativeMultiHashMap<int, int> hash;

		private int chars;

		private int entries;

		private const int kMaxEntries = 16384;

		private const int kMaxChars = 2097152;

		public const int kMaxCharsPerEntry = 4096;

		private struct Entry
		{
			public int offset;

			public int length;
		}
	}
}
