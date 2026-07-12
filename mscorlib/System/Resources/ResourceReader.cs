using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Text;

namespace System.Resources
{
	[ComVisible(true)]
	public sealed class ResourceReader : IResourceReader, IEnumerable, IDisposable
	{
		[SecuritySafeCritical]
		public ResourceReader(string fileName)
		{
			this._resCache = new Dictionary<string, ResourceLocator>(FastResourceComparer.Default);
			this._store = new BinaryReader(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.RandomAccess, Path.GetFileName(fileName), false, false, false), Encoding.UTF8);
			try
			{
				this.ReadResources();
			}
			catch
			{
				this._store.Close();
				throw;
			}
		}

		[SecurityCritical]
		public ResourceReader(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				throw new ArgumentException(Environment.GetResourceString("Stream was not readable."));
			}
			this._resCache = new Dictionary<string, ResourceLocator>(FastResourceComparer.Default);
			this._store = new BinaryReader(stream, Encoding.UTF8);
			this._ums = stream as UnmanagedMemoryStream;
			this.ReadResources();
		}

		[SecurityCritical]
		internal ResourceReader(Stream stream, Dictionary<string, ResourceLocator> resCache)
		{
			this._resCache = resCache;
			this._store = new BinaryReader(stream, Encoding.UTF8);
			this._ums = stream as UnmanagedMemoryStream;
			this.ReadResources();
		}

		public void Close()
		{
			this.Dispose(true);
		}

		public void Dispose()
		{
			this.Close();
		}

		[SecuritySafeCritical]
		private void Dispose(bool disposing)
		{
			if (this._store != null)
			{
				this._resCache = null;
				if (disposing)
				{
					BinaryReader store = this._store;
					this._store = null;
					if (store != null)
					{
						store.Close();
					}
				}
				this._store = null;
				this._namePositions = null;
				this._nameHashes = null;
				this._ums = null;
				this._namePositionsPtr = null;
				this._nameHashesPtr = null;
			}
		}

		[SecurityCritical]
		internal unsafe static int ReadUnalignedI4(int* p)
		{
			return (int)(*(byte*)p) | ((int)((byte*)p)[1] << 8) | ((int)((byte*)p)[2] << 16) | ((int)((byte*)p)[3] << 24);
		}

		private void SkipInt32()
		{
			this._store.BaseStream.Seek(4L, SeekOrigin.Current);
		}

		private void SkipString()
		{
			int num = this._store.Read7BitEncodedInt();
			if (num < 0)
			{
				throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file. String length must be non-negative."));
			}
			this._store.BaseStream.Seek((long)num, SeekOrigin.Current);
		}

		[SecuritySafeCritical]
		private int GetNameHash(int index)
		{
			if (this._ums == null)
			{
				return this._nameHashes[index];
			}
			return ResourceReader.ReadUnalignedI4(this._nameHashesPtr + index);
		}

		[SecuritySafeCritical]
		private int GetNamePosition(int index)
		{
			int num;
			if (this._ums == null)
			{
				num = this._namePositions[index];
			}
			else
			{
				num = ResourceReader.ReadUnalignedI4(this._namePositionsPtr + index);
			}
			if (num < 0 || (long)num > this._dataSectionOffset - this._nameSectionOffset)
			{
				throw new FormatException(Environment.GetResourceString("Corrupt .resources file. Invalid offset '{0}' into name section.", new object[] { num }));
			}
			return num;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			if (this._resCache == null)
			{
				throw new InvalidOperationException(Environment.GetResourceString("ResourceReader is closed."));
			}
			return new ResourceReader.ResourceEnumerator(this);
		}

		internal ResourceReader.ResourceEnumerator GetEnumeratorInternal()
		{
			return new ResourceReader.ResourceEnumerator(this);
		}

		internal int FindPosForResource(string name)
		{
			int num = FastResourceComparer.HashFunction(name);
			int i = 0;
			int num2 = this._numResources - 1;
			int num3 = -1;
			bool flag = false;
			while (i <= num2)
			{
				num3 = i + num2 >> 1;
				int nameHash = this.GetNameHash(num3);
				int num4;
				if (nameHash == num)
				{
					num4 = 0;
				}
				else if (nameHash < num)
				{
					num4 = -1;
				}
				else
				{
					num4 = 1;
				}
				if (num4 == 0)
				{
					flag = true;
					break;
				}
				if (num4 < 0)
				{
					i = num3 + 1;
				}
				else
				{
					num2 = num3 - 1;
				}
			}
			if (!flag)
			{
				return -1;
			}
			if (i != num3)
			{
				i = num3;
				while (i > 0 && this.GetNameHash(i - 1) == num)
				{
					i--;
				}
			}
			if (num2 != num3)
			{
				num2 = num3;
				while (num2 < this._numResources - 1 && this.GetNameHash(num2 + 1) == num)
				{
					num2++;
				}
			}
			lock (this)
			{
				int j = i;
				while (j <= num2)
				{
					this._store.BaseStream.Seek(this._nameSectionOffset + (long)this.GetNamePosition(j), SeekOrigin.Begin);
					if (this.CompareStringEqualsName(name))
					{
						int num5 = this._store.ReadInt32();
						if (num5 < 0 || (long)num5 >= this._store.BaseStream.Length - this._dataSectionOffset)
						{
							throw new FormatException(Environment.GetResourceString("Corrupt .resources file. Invalid offset '{0}' into data section.", new object[] { num5 }));
						}
						return num5;
					}
					else
					{
						j++;
					}
				}
			}
			return -1;
		}

		[SecuritySafeCritical]
		private unsafe bool CompareStringEqualsName(string name)
		{
			int num = this._store.Read7BitEncodedInt();
			if (num < 0)
			{
				throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file. String length must be non-negative."));
			}
			if (this._ums == null)
			{
				byte[] array = new byte[num];
				int num2;
				for (int i = num; i > 0; i -= num2)
				{
					num2 = this._store.Read(array, num - i, i);
					if (num2 == 0)
					{
						throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file. A resource name extends past the end of the stream."));
					}
				}
				return FastResourceComparer.CompareOrdinal(array, num / 2, name) == 0;
			}
			byte* positionPointer = this._ums.PositionPointer;
			this._ums.Seek((long)num, SeekOrigin.Current);
			if (this._ums.Position > this._ums.Length)
			{
				throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file. Resource name extends past the end of the file."));
			}
			return FastResourceComparer.CompareOrdinal(positionPointer, num, name) == 0;
		}

		[SecurityCritical]
		private unsafe string AllocateStringForNameIndex(int index, out int dataOffset)
		{
			long num = (long)this.GetNamePosition(index);
			int num2;
			byte[] array3;
			lock (this)
			{
				this._store.BaseStream.Seek(num + this._nameSectionOffset, SeekOrigin.Begin);
				num2 = this._store.Read7BitEncodedInt();
				if (num2 < 0)
				{
					throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file. String length must be non-negative."));
				}
				if (this._ums != null)
				{
					if (this._ums.Position > this._ums.Length - (long)num2)
					{
						throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file. String for name index '{0}' extends past the end of the file.", new object[] { index }));
					}
					char* positionPointer = (char*)this._ums.PositionPointer;
					string text;
					if (!BitConverter.IsLittleEndian)
					{
						byte* ptr = (byte*)positionPointer;
						byte[] array = new byte[num2];
						for (int i = 0; i < num2; i += 2)
						{
							array[i] = (ptr + i)[1];
							array[i + 1] = ptr[i];
						}
						byte[] array2;
						byte* ptr2;
						if ((array2 = array) == null || array2.Length == 0)
						{
							ptr2 = null;
						}
						else
						{
							ptr2 = &array2[0];
						}
						text = new string((char*)ptr2, 0, num2 / 2);
						array2 = null;
					}
					else
					{
						text = new string(positionPointer, 0, num2 / 2);
					}
					this._ums.Position += (long)num2;
					dataOffset = this._store.ReadInt32();
					if (dataOffset < 0 || (long)dataOffset >= this._store.BaseStream.Length - this._dataSectionOffset)
					{
						throw new FormatException(Environment.GetResourceString("Corrupt .resources file. Invalid offset '{0}' into data section.", new object[] { dataOffset }));
					}
					return text;
				}
				else
				{
					array3 = new byte[num2];
					int num3;
					for (int j = num2; j > 0; j -= num3)
					{
						num3 = this._store.Read(array3, num2 - j, j);
						if (num3 == 0)
						{
							throw new EndOfStreamException(Environment.GetResourceString("Corrupt .resources file. The resource name for name index {0} extends past the end of the stream.", new object[] { index }));
						}
					}
					dataOffset = this._store.ReadInt32();
					if (dataOffset < 0 || (long)dataOffset >= this._store.BaseStream.Length - this._dataSectionOffset)
					{
						throw new FormatException(Environment.GetResourceString("Corrupt .resources file. Invalid offset '{0}' into data section.", new object[] { dataOffset }));
					}
				}
			}
			return Encoding.Unicode.GetString(array3, 0, num2);
		}

		private object GetValueForNameIndex(int index)
		{
			long num = (long)this.GetNamePosition(index);
			object obj;
			lock (this)
			{
				this._store.BaseStream.Seek(num + this._nameSectionOffset, SeekOrigin.Begin);
				this.SkipString();
				int num2 = this._store.ReadInt32();
				if (num2 < 0 || (long)num2 >= this._store.BaseStream.Length - this._dataSectionOffset)
				{
					throw new FormatException(Environment.GetResourceString("Corrupt .resources file. Invalid offset '{0}' into data section.", new object[] { num2 }));
				}
				if (this._version == 1)
				{
					obj = this.LoadObjectV1(num2);
				}
				else
				{
					ResourceTypeCode resourceTypeCode;
					obj = this.LoadObjectV2(num2, out resourceTypeCode);
				}
			}
			return obj;
		}

		internal string LoadString(int pos)
		{
			this._store.BaseStream.Seek(this._dataSectionOffset + (long)pos, SeekOrigin.Begin);
			string text = null;
			int num = this._store.Read7BitEncodedInt();
			if (this._version == 1)
			{
				if (num == -1)
				{
					return null;
				}
				if (this.FindType(num) != typeof(string))
				{
					throw new InvalidOperationException(Environment.GetResourceString("Resource was of type '{0}' instead of String - call GetObject instead.", new object[] { this.FindType(num).FullName }));
				}
				text = this._store.ReadString();
			}
			else
			{
				ResourceTypeCode resourceTypeCode = (ResourceTypeCode)num;
				if (resourceTypeCode != ResourceTypeCode.String && resourceTypeCode != ResourceTypeCode.Null)
				{
					string text2;
					if (resourceTypeCode < ResourceTypeCode.StartOfUserTypes)
					{
						text2 = resourceTypeCode.ToString();
					}
					else
					{
						text2 = this.FindType(resourceTypeCode - ResourceTypeCode.StartOfUserTypes).FullName;
					}
					throw new InvalidOperationException(Environment.GetResourceString("Resource was of type '{0}' instead of String - call GetObject instead.", new object[] { text2 }));
				}
				if (resourceTypeCode == ResourceTypeCode.String)
				{
					text = this._store.ReadString();
				}
			}
			return text;
		}

		internal object LoadObject(int pos)
		{
			if (this._version == 1)
			{
				return this.LoadObjectV1(pos);
			}
			ResourceTypeCode resourceTypeCode;
			return this.LoadObjectV2(pos, out resourceTypeCode);
		}

		internal object LoadObject(int pos, out ResourceTypeCode typeCode)
		{
			if (this._version == 1)
			{
				object obj = this.LoadObjectV1(pos);
				typeCode = ((obj is string) ? ResourceTypeCode.String : ResourceTypeCode.StartOfUserTypes);
				return obj;
			}
			return this.LoadObjectV2(pos, out typeCode);
		}

		internal object LoadObjectV1(int pos)
		{
			object obj;
			try
			{
				obj = this._LoadObjectV1(pos);
			}
			catch (EndOfStreamException ex)
			{
				throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file.  The specified type doesn't match the available data in the stream."), ex);
			}
			catch (ArgumentOutOfRangeException ex2)
			{
				throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file.  The specified type doesn't match the available data in the stream."), ex2);
			}
			return obj;
		}

		[SecuritySafeCritical]
		private object _LoadObjectV1(int pos)
		{
			this._store.BaseStream.Seek(this._dataSectionOffset + (long)pos, SeekOrigin.Begin);
			int num = this._store.Read7BitEncodedInt();
			if (num == -1)
			{
				return null;
			}
			RuntimeType runtimeType = this.FindType(num);
			if (runtimeType == typeof(string))
			{
				return this._store.ReadString();
			}
			if (runtimeType == typeof(int))
			{
				return this._store.ReadInt32();
			}
			if (runtimeType == typeof(byte))
			{
				return this._store.ReadByte();
			}
			if (runtimeType == typeof(sbyte))
			{
				return this._store.ReadSByte();
			}
			if (runtimeType == typeof(short))
			{
				return this._store.ReadInt16();
			}
			if (runtimeType == typeof(long))
			{
				return this._store.ReadInt64();
			}
			if (runtimeType == typeof(ushort))
			{
				return this._store.ReadUInt16();
			}
			if (runtimeType == typeof(uint))
			{
				return this._store.ReadUInt32();
			}
			if (runtimeType == typeof(ulong))
			{
				return this._store.ReadUInt64();
			}
			if (runtimeType == typeof(float))
			{
				return this._store.ReadSingle();
			}
			if (runtimeType == typeof(double))
			{
				return this._store.ReadDouble();
			}
			if (runtimeType == typeof(DateTime))
			{
				return new DateTime(this._store.ReadInt64());
			}
			if (runtimeType == typeof(TimeSpan))
			{
				return new TimeSpan(this._store.ReadInt64());
			}
			if (runtimeType == typeof(decimal))
			{
				int[] array = new int[4];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this._store.ReadInt32();
				}
				return new decimal(array);
			}
			return this.DeserializeObject(num);
		}

		internal object LoadObjectV2(int pos, out ResourceTypeCode typeCode)
		{
			object obj;
			try
			{
				obj = this._LoadObjectV2(pos, out typeCode);
			}
			catch (EndOfStreamException ex)
			{
				throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file.  The specified type doesn't match the available data in the stream."), ex);
			}
			catch (ArgumentOutOfRangeException ex2)
			{
				throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file.  The specified type doesn't match the available data in the stream."), ex2);
			}
			return obj;
		}

		[SecuritySafeCritical]
		private object _LoadObjectV2(int pos, out ResourceTypeCode typeCode)
		{
			this._store.BaseStream.Seek(this._dataSectionOffset + (long)pos, SeekOrigin.Begin);
			typeCode = (ResourceTypeCode)this._store.Read7BitEncodedInt();
			switch (typeCode)
			{
			case ResourceTypeCode.Null:
				return null;
			case ResourceTypeCode.String:
				return this._store.ReadString();
			case ResourceTypeCode.Boolean:
				return this._store.ReadBoolean();
			case ResourceTypeCode.Char:
				return (char)this._store.ReadUInt16();
			case ResourceTypeCode.Byte:
				return this._store.ReadByte();
			case ResourceTypeCode.SByte:
				return this._store.ReadSByte();
			case ResourceTypeCode.Int16:
				return this._store.ReadInt16();
			case ResourceTypeCode.UInt16:
				return this._store.ReadUInt16();
			case ResourceTypeCode.Int32:
				return this._store.ReadInt32();
			case ResourceTypeCode.UInt32:
				return this._store.ReadUInt32();
			case ResourceTypeCode.Int64:
				return this._store.ReadInt64();
			case ResourceTypeCode.UInt64:
				return this._store.ReadUInt64();
			case ResourceTypeCode.Single:
				return this._store.ReadSingle();
			case ResourceTypeCode.Double:
				return this._store.ReadDouble();
			case ResourceTypeCode.Decimal:
				return this._store.ReadDecimal();
			case ResourceTypeCode.DateTime:
				return DateTime.FromBinary(this._store.ReadInt64());
			case ResourceTypeCode.TimeSpan:
				return new TimeSpan(this._store.ReadInt64());
			case ResourceTypeCode.ByteArray:
			{
				int num = this._store.ReadInt32();
				if (num < 0)
				{
					throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file.  The specified data length '{0}' is not a valid position in the stream.", new object[] { num }));
				}
				if (this._ums == null)
				{
					if ((long)num > this._store.BaseStream.Length)
					{
						throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file.  The specified data length '{0}' is not a valid position in the stream.", new object[] { num }));
					}
					return this._store.ReadBytes(num);
				}
				else
				{
					if ((long)num > this._ums.Length - this._ums.Position)
					{
						throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file.  The specified data length '{0}' is not a valid position in the stream.", new object[] { num }));
					}
					byte[] array = new byte[num];
					this._ums.Read(array, 0, num);
					return array;
				}
				break;
			}
			case ResourceTypeCode.Stream:
			{
				int num2 = this._store.ReadInt32();
				if (num2 < 0)
				{
					throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file.  The specified data length '{0}' is not a valid position in the stream.", new object[] { num2 }));
				}
				if (this._ums == null)
				{
					return new PinnedBufferMemoryStream(this._store.ReadBytes(num2));
				}
				if ((long)num2 > this._ums.Length - this._ums.Position)
				{
					throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file.  The specified data length '{0}' is not a valid position in the stream.", new object[] { num2 }));
				}
				return new UnmanagedMemoryStream(this._ums.PositionPointer, (long)num2, (long)num2, FileAccess.Read);
			}
			}
			if (typeCode < ResourceTypeCode.StartOfUserTypes)
			{
				throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file.  The specified type doesn't match the available data in the stream."));
			}
			int num3 = typeCode - ResourceTypeCode.StartOfUserTypes;
			return this.DeserializeObject(num3);
		}

		[SecurityCritical]
		private object DeserializeObject(int typeIndex)
		{
			RuntimeType runtimeType = this.FindType(typeIndex);
			object obj = this._objFormatter.Deserialize(this._store.BaseStream);
			if (obj.GetType() != runtimeType)
			{
				throw new BadImageFormatException(Environment.GetResourceString("The type serialized in the .resources file was not the same type that the .resources file said it contained. Expected '{0}' but read '{1}'.", new object[]
				{
					runtimeType.FullName,
					obj.GetType().FullName
				}));
			}
			return obj;
		}

		[SecurityCritical]
		private void ReadResources()
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.File | StreamingContextStates.Persistence));
			this._objFormatter = binaryFormatter;
			try
			{
				this._ReadResources();
			}
			catch (EndOfStreamException ex)
			{
				throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file. Unable to read resources from this file because of invalid header information. Try regenerating the .resources file."), ex);
			}
			catch (IndexOutOfRangeException ex2)
			{
				throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file. Unable to read resources from this file because of invalid header information. Try regenerating the .resources file."), ex2);
			}
		}

		[SecurityCritical]
		private unsafe void _ReadResources()
		{
			if (this._store.ReadInt32() != ResourceManager.MagicNumber)
			{
				throw new ArgumentException(Environment.GetResourceString("Stream is not a valid resource file."));
			}
			int num = this._store.ReadInt32();
			int num2 = this._store.ReadInt32();
			if (num2 < 0 || num < 0)
			{
				throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file. Unable to read resources from this file because of invalid header information. Try regenerating the .resources file."));
			}
			if (num > 1)
			{
				this._store.BaseStream.Seek((long)num2, SeekOrigin.Current);
			}
			else
			{
				string text = this._store.ReadString();
				AssemblyName assemblyName = new AssemblyName(ResourceManager.MscorlibName);
				if (!ResourceManager.CompareNames(text, ResourceManager.ResReaderTypeName, assemblyName))
				{
					throw new NotSupportedException(Environment.GetResourceString("This .resources file should not be read with this reader. The resource reader type is \"{0}\".", new object[] { text }));
				}
				this.SkipString();
			}
			int num3 = this._store.ReadInt32();
			if (num3 != 2 && num3 != 1)
			{
				throw new ArgumentException(Environment.GetResourceString("The ResourceReader class does not know how to read this version of .resources files. Expected version: {0}  This file: {1}", new object[] { 2, num3 }));
			}
			this._version = num3;
			this._numResources = this._store.ReadInt32();
			if (this._numResources < 0)
			{
				throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file. Unable to read resources from this file because of invalid header information. Try regenerating the .resources file."));
			}
			int num4 = this._store.ReadInt32();
			if (num4 < 0)
			{
				throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file. Unable to read resources from this file because of invalid header information. Try regenerating the .resources file."));
			}
			this._typeTable = new RuntimeType[num4];
			this._typeNamePositions = new int[num4];
			for (int i = 0; i < num4; i++)
			{
				this._typeNamePositions[i] = (int)this._store.BaseStream.Position;
				this.SkipString();
			}
			int num5 = (int)this._store.BaseStream.Position & 7;
			if (num5 != 0)
			{
				for (int j = 0; j < 8 - num5; j++)
				{
					this._store.ReadByte();
				}
			}
			if (this._ums == null)
			{
				this._nameHashes = new int[this._numResources];
				for (int k = 0; k < this._numResources; k++)
				{
					this._nameHashes[k] = this._store.ReadInt32();
				}
			}
			else
			{
				if (((long)this._numResources & (long)((ulong)(-536870912))) != 0L)
				{
					throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file. Unable to read resources from this file because of invalid header information. Try regenerating the .resources file."));
				}
				int num6 = 4 * this._numResources;
				this._nameHashesPtr = (int*)this._ums.PositionPointer;
				this._ums.Seek((long)num6, SeekOrigin.Current);
				byte* positionPointer = this._ums.PositionPointer;
			}
			if (this._ums == null)
			{
				this._namePositions = new int[this._numResources];
				for (int l = 0; l < this._numResources; l++)
				{
					int num7 = this._store.ReadInt32();
					if (num7 < 0)
					{
						throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file. Unable to read resources from this file because of invalid header information. Try regenerating the .resources file."));
					}
					this._namePositions[l] = num7;
				}
			}
			else
			{
				if (((long)this._numResources & (long)((ulong)(-536870912))) != 0L)
				{
					throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file. Unable to read resources from this file because of invalid header information. Try regenerating the .resources file."));
				}
				int num8 = 4 * this._numResources;
				this._namePositionsPtr = (int*)this._ums.PositionPointer;
				this._ums.Seek((long)num8, SeekOrigin.Current);
				byte* positionPointer2 = this._ums.PositionPointer;
			}
			this._dataSectionOffset = (long)this._store.ReadInt32();
			if (this._dataSectionOffset < 0L)
			{
				throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file. Unable to read resources from this file because of invalid header information. Try regenerating the .resources file."));
			}
			this._nameSectionOffset = this._store.BaseStream.Position;
			if (this._dataSectionOffset < this._nameSectionOffset)
			{
				throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file. Unable to read resources from this file because of invalid header information. Try regenerating the .resources file."));
			}
		}

		private RuntimeType FindType(int typeIndex)
		{
			if (typeIndex < 0 || typeIndex >= this._typeTable.Length)
			{
				throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file.  The specified type doesn't exist."));
			}
			if (this._typeTable[typeIndex] == null)
			{
				long position = this._store.BaseStream.Position;
				try
				{
					this._store.BaseStream.Position = (long)this._typeNamePositions[typeIndex];
					string text = this._store.ReadString();
					this._typeTable[typeIndex] = (RuntimeType)Type.GetType(text, true);
				}
				finally
				{
					this._store.BaseStream.Position = position;
				}
			}
			return this._typeTable[typeIndex];
		}

		public void GetResourceData(string resourceName, out string resourceType, out byte[] resourceData)
		{
			if (resourceName == null)
			{
				throw new ArgumentNullException("resourceName");
			}
			if (this._resCache == null)
			{
				throw new InvalidOperationException(Environment.GetResourceString("ResourceReader is closed."));
			}
			int[] array = new int[this._numResources];
			int num = this.FindPosForResource(resourceName);
			if (num == -1)
			{
				throw new ArgumentException(Environment.GetResourceString("The specified resource name \"{0}\" does not exist in the resource file.", new object[] { resourceName }));
			}
			lock (this)
			{
				for (int i = 0; i < this._numResources; i++)
				{
					this._store.BaseStream.Position = this._nameSectionOffset + (long)this.GetNamePosition(i);
					int num2 = this._store.Read7BitEncodedInt();
					if (num2 < 0)
					{
						throw new FormatException(Environment.GetResourceString("Corrupt .resources file. Invalid offset '{0}' into name section.", new object[] { num2 }));
					}
					this._store.BaseStream.Position += (long)num2;
					int num3 = this._store.ReadInt32();
					if (num3 < 0 || (long)num3 >= this._store.BaseStream.Length - this._dataSectionOffset)
					{
						throw new FormatException(Environment.GetResourceString("Corrupt .resources file. Invalid offset '{0}' into data section.", new object[] { num3 }));
					}
					array[i] = num3;
				}
				Array.Sort<int>(array);
				int num4 = Array.BinarySearch<int>(array, num);
				int num5 = (int)(((num4 < this._numResources - 1) ? ((long)array[num4 + 1] + this._dataSectionOffset) : this._store.BaseStream.Length) - ((long)num + this._dataSectionOffset));
				this._store.BaseStream.Position = this._dataSectionOffset + (long)num;
				ResourceTypeCode resourceTypeCode = (ResourceTypeCode)this._store.Read7BitEncodedInt();
				if (resourceTypeCode < ResourceTypeCode.Null || resourceTypeCode >= ResourceTypeCode.StartOfUserTypes + this._typeTable.Length)
				{
					throw new BadImageFormatException(Environment.GetResourceString("Corrupt .resources file.  The specified type doesn't exist."));
				}
				resourceType = this.TypeNameFromTypeCode(resourceTypeCode);
				num5 -= (int)(this._store.BaseStream.Position - (this._dataSectionOffset + (long)num));
				byte[] array2 = this._store.ReadBytes(num5);
				if (array2.Length != num5)
				{
					throw new FormatException(Environment.GetResourceString("Corrupt .resources file. A resource name extends past the end of the stream."));
				}
				resourceData = array2;
			}
		}

		private string TypeNameFromTypeCode(ResourceTypeCode typeCode)
		{
			if (typeCode < ResourceTypeCode.StartOfUserTypes)
			{
				return "ResourceTypeCode." + typeCode.ToString();
			}
			int num = typeCode - ResourceTypeCode.StartOfUserTypes;
			long position = this._store.BaseStream.Position;
			string text;
			try
			{
				this._store.BaseStream.Position = (long)this._typeNamePositions[num];
				text = this._store.ReadString();
			}
			finally
			{
				this._store.BaseStream.Position = position;
			}
			return text;
		}

		private const int DefaultFileStreamBufferSize = 4096;

		private BinaryReader _store;

		internal Dictionary<string, ResourceLocator> _resCache;

		private long _nameSectionOffset;

		private long _dataSectionOffset;

		private int[] _nameHashes;

		[SecurityCritical]
		private unsafe int* _nameHashesPtr;

		private int[] _namePositions;

		[SecurityCritical]
		private unsafe int* _namePositionsPtr;

		private RuntimeType[] _typeTable;

		private int[] _typeNamePositions;

		private BinaryFormatter _objFormatter;

		private int _numResources;

		private UnmanagedMemoryStream _ums;

		private int _version;

		internal sealed class ResourceEnumerator : IDictionaryEnumerator, IEnumerator
		{
			internal ResourceEnumerator(ResourceReader reader)
			{
				this._currentName = -1;
				this._reader = reader;
				this._dataPosition = -2;
			}

			public bool MoveNext()
			{
				if (this._currentName == this._reader._numResources - 1 || this._currentName == -2147483648)
				{
					this._currentIsValid = false;
					this._currentName = int.MinValue;
					return false;
				}
				this._currentIsValid = true;
				this._currentName++;
				return true;
			}

			public object Key
			{
				[SecuritySafeCritical]
				get
				{
					if (this._currentName == -2147483648)
					{
						throw new InvalidOperationException(Environment.GetResourceString("Enumeration already finished."));
					}
					if (!this._currentIsValid)
					{
						throw new InvalidOperationException(Environment.GetResourceString("Enumeration has not started. Call MoveNext."));
					}
					if (this._reader._resCache == null)
					{
						throw new InvalidOperationException(Environment.GetResourceString("ResourceReader is closed."));
					}
					return this._reader.AllocateStringForNameIndex(this._currentName, out this._dataPosition);
				}
			}

			public object Current
			{
				get
				{
					return this.Entry;
				}
			}

			internal int DataPosition
			{
				get
				{
					return this._dataPosition;
				}
			}

			public DictionaryEntry Entry
			{
				[SecuritySafeCritical]
				get
				{
					if (this._currentName == -2147483648)
					{
						throw new InvalidOperationException(Environment.GetResourceString("Enumeration already finished."));
					}
					if (!this._currentIsValid)
					{
						throw new InvalidOperationException(Environment.GetResourceString("Enumeration has not started. Call MoveNext."));
					}
					if (this._reader._resCache == null)
					{
						throw new InvalidOperationException(Environment.GetResourceString("ResourceReader is closed."));
					}
					object obj = null;
					ResourceReader reader = this._reader;
					string text;
					lock (reader)
					{
						Dictionary<string, ResourceLocator> resCache = this._reader._resCache;
						lock (resCache)
						{
							text = this._reader.AllocateStringForNameIndex(this._currentName, out this._dataPosition);
							ResourceLocator resourceLocator;
							if (this._reader._resCache.TryGetValue(text, out resourceLocator))
							{
								obj = resourceLocator.Value;
							}
							if (obj == null)
							{
								if (this._dataPosition == -1)
								{
									obj = this._reader.GetValueForNameIndex(this._currentName);
								}
								else
								{
									obj = this._reader.LoadObject(this._dataPosition);
								}
							}
						}
					}
					return new DictionaryEntry(text, obj);
				}
			}

			public object Value
			{
				get
				{
					if (this._currentName == -2147483648)
					{
						throw new InvalidOperationException(Environment.GetResourceString("Enumeration already finished."));
					}
					if (!this._currentIsValid)
					{
						throw new InvalidOperationException(Environment.GetResourceString("Enumeration has not started. Call MoveNext."));
					}
					if (this._reader._resCache == null)
					{
						throw new InvalidOperationException(Environment.GetResourceString("ResourceReader is closed."));
					}
					return this._reader.GetValueForNameIndex(this._currentName);
				}
			}

			public void Reset()
			{
				if (this._reader._resCache == null)
				{
					throw new InvalidOperationException(Environment.GetResourceString("ResourceReader is closed."));
				}
				this._currentIsValid = false;
				this._currentName = -1;
			}

			private const int ENUM_DONE = -2147483648;

			private const int ENUM_NOT_STARTED = -1;

			private ResourceReader _reader;

			private bool _currentIsValid;

			private int _currentName;

			private int _dataPosition;
		}
	}
}
