using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Mono
{
	internal abstract class DataConverter
	{
		public abstract double GetDouble(byte[] data, int index);

		public abstract float GetFloat(byte[] data, int index);

		public abstract long GetInt64(byte[] data, int index);

		public abstract int GetInt32(byte[] data, int index);

		public abstract short GetInt16(byte[] data, int index);

		[CLSCompliant(false)]
		public abstract uint GetUInt32(byte[] data, int index);

		[CLSCompliant(false)]
		public abstract ushort GetUInt16(byte[] data, int index);

		[CLSCompliant(false)]
		public abstract ulong GetUInt64(byte[] data, int index);

		public abstract void PutBytes(byte[] dest, int destIdx, double value);

		public abstract void PutBytes(byte[] dest, int destIdx, float value);

		public abstract void PutBytes(byte[] dest, int destIdx, int value);

		public abstract void PutBytes(byte[] dest, int destIdx, long value);

		public abstract void PutBytes(byte[] dest, int destIdx, short value);

		[CLSCompliant(false)]
		public abstract void PutBytes(byte[] dest, int destIdx, ushort value);

		[CLSCompliant(false)]
		public abstract void PutBytes(byte[] dest, int destIdx, uint value);

		[CLSCompliant(false)]
		public abstract void PutBytes(byte[] dest, int destIdx, ulong value);

		public byte[] GetBytes(double value)
		{
			byte[] array = new byte[8];
			this.PutBytes(array, 0, value);
			return array;
		}

		public byte[] GetBytes(float value)
		{
			byte[] array = new byte[4];
			this.PutBytes(array, 0, value);
			return array;
		}

		public byte[] GetBytes(int value)
		{
			byte[] array = new byte[4];
			this.PutBytes(array, 0, value);
			return array;
		}

		public byte[] GetBytes(long value)
		{
			byte[] array = new byte[8];
			this.PutBytes(array, 0, value);
			return array;
		}

		public byte[] GetBytes(short value)
		{
			byte[] array = new byte[2];
			this.PutBytes(array, 0, value);
			return array;
		}

		[CLSCompliant(false)]
		public byte[] GetBytes(ushort value)
		{
			byte[] array = new byte[2];
			this.PutBytes(array, 0, value);
			return array;
		}

		[CLSCompliant(false)]
		public byte[] GetBytes(uint value)
		{
			byte[] array = new byte[4];
			this.PutBytes(array, 0, value);
			return array;
		}

		[CLSCompliant(false)]
		public byte[] GetBytes(ulong value)
		{
			byte[] array = new byte[8];
			this.PutBytes(array, 0, value);
			return array;
		}

		public static DataConverter LittleEndian
		{
			get
			{
				if (!BitConverter.IsLittleEndian)
				{
					return DataConverter.SwapConv;
				}
				return DataConverter.CopyConv;
			}
		}

		public static DataConverter BigEndian
		{
			get
			{
				if (!BitConverter.IsLittleEndian)
				{
					return DataConverter.CopyConv;
				}
				return DataConverter.SwapConv;
			}
		}

		public static DataConverter Native
		{
			get
			{
				return DataConverter.CopyConv;
			}
		}

		private static int Align(int current, int align)
		{
			return (current + align - 1) / align * align;
		}

		public static byte[] Pack(string description, params object[] args)
		{
			int num = 0;
			DataConverter.PackContext packContext = new DataConverter.PackContext();
			packContext.conv = DataConverter.CopyConv;
			packContext.description = description;
			packContext.i = 0;
			while (packContext.i < description.Length)
			{
				object obj;
				if (num < args.Length)
				{
					obj = args[num];
				}
				else
				{
					if (packContext.repeat != 0)
					{
						break;
					}
					obj = null;
				}
				int i = packContext.i;
				if (DataConverter.PackOne(packContext, obj))
				{
					num++;
					if (packContext.repeat > 0)
					{
						DataConverter.PackContext packContext2 = packContext;
						int num2 = packContext2.repeat - 1;
						packContext2.repeat = num2;
						if (num2 > 0)
						{
							packContext.i = i;
						}
						else
						{
							packContext.i++;
						}
					}
					else
					{
						packContext.i++;
					}
				}
				else
				{
					packContext.i++;
				}
			}
			return packContext.Get();
		}

		public static byte[] PackEnumerable(string description, IEnumerable args)
		{
			DataConverter.PackContext packContext = new DataConverter.PackContext();
			packContext.conv = DataConverter.CopyConv;
			packContext.description = description;
			IEnumerator enumerator = args.GetEnumerator();
			bool flag = enumerator.MoveNext();
			packContext.i = 0;
			while (packContext.i < description.Length)
			{
				object obj;
				if (flag)
				{
					obj = enumerator.Current;
				}
				else
				{
					if (packContext.repeat != 0)
					{
						break;
					}
					obj = null;
				}
				int i = packContext.i;
				if (DataConverter.PackOne(packContext, obj))
				{
					flag = enumerator.MoveNext();
					if (packContext.repeat > 0)
					{
						DataConverter.PackContext packContext2 = packContext;
						int num = packContext2.repeat - 1;
						packContext2.repeat = num;
						if (num > 0)
						{
							packContext.i = i;
						}
						else
						{
							packContext.i++;
						}
					}
					else
					{
						packContext.i++;
					}
				}
				else
				{
					packContext.i++;
				}
			}
			return packContext.Get();
		}

		private static bool PackOne(DataConverter.PackContext b, object oarg)
		{
			char c = b.description[b.i];
			if (c <= 'S')
			{
				if (c <= 'C')
				{
					switch (c)
					{
					case '!':
						b.align = -1;
						return false;
					case '"':
					case '#':
					case '&':
					case '\'':
					case '(':
					case ')':
					case '+':
					case ',':
					case '-':
					case '.':
					case '/':
					case '0':
						goto IL_0457;
					case '$':
						break;
					case '%':
						b.conv = DataConverter.Native;
						return false;
					case '*':
						b.repeat = int.MaxValue;
						return false;
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						b.repeat = (int)((short)b.description[b.i] - 48);
						return false;
					default:
						if (c != 'C')
						{
							goto IL_0457;
						}
						b.Add(new byte[] { Convert.ToByte(oarg) });
						return true;
					}
				}
				else
				{
					if (c == 'I')
					{
						b.Add(b.conv.GetBytes(Convert.ToUInt32(oarg)));
						return true;
					}
					if (c == 'L')
					{
						b.Add(b.conv.GetBytes(Convert.ToUInt64(oarg)));
						return true;
					}
					if (c != 'S')
					{
						goto IL_0457;
					}
					b.Add(b.conv.GetBytes(Convert.ToUInt16(oarg)));
					return true;
				}
			}
			else if (c <= 'l')
			{
				switch (c)
				{
				case '[':
				{
					int num = -1;
					int num2 = b.i + 1;
					while (num2 < b.description.Length && b.description[num2] != ']')
					{
						int num3 = (int)((short)b.description[num2] - 48);
						if (num3 >= 0 && num3 <= 9)
						{
							if (num == -1)
							{
								num = num3;
							}
							else
							{
								num = num * 10 + num3;
							}
						}
						num2++;
					}
					if (num == -1)
					{
						throw new ArgumentException("invalid size specification");
					}
					b.i = num2;
					b.repeat = num;
					return false;
				}
				case '\\':
				case ']':
				case '`':
				case 'a':
				case 'e':
				case 'g':
				case 'h':
					goto IL_0457;
				case '^':
					b.conv = DataConverter.BigEndian;
					return false;
				case '_':
					b.conv = DataConverter.LittleEndian;
					return false;
				case 'b':
					b.Add(new byte[] { Convert.ToByte(oarg) });
					return true;
				case 'c':
					b.Add(new byte[] { (byte)Convert.ToSByte(oarg) });
					return true;
				case 'd':
					b.Add(b.conv.GetBytes(Convert.ToDouble(oarg)));
					return true;
				case 'f':
					b.Add(b.conv.GetBytes(Convert.ToSingle(oarg)));
					return true;
				case 'i':
					b.Add(b.conv.GetBytes(Convert.ToInt32(oarg)));
					return true;
				default:
					if (c != 'l')
					{
						goto IL_0457;
					}
					b.Add(b.conv.GetBytes(Convert.ToInt64(oarg)));
					return true;
				}
			}
			else
			{
				if (c == 's')
				{
					b.Add(b.conv.GetBytes(Convert.ToInt16(oarg)));
					return true;
				}
				if (c == 'x')
				{
					b.Add(new byte[1]);
					return false;
				}
				if (c != 'z')
				{
					goto IL_0457;
				}
			}
			bool flag = b.description[b.i] == 'z';
			b.i++;
			if (b.i >= b.description.Length)
			{
				throw new ArgumentException("$ description needs a type specified", "description");
			}
			char c2 = b.description[b.i];
			Encoding encoding;
			switch (c2)
			{
			case '3':
			{
				encoding = Encoding.GetEncoding(12000);
				int num3 = 4;
				goto IL_0423;
			}
			case '4':
			{
				encoding = Encoding.GetEncoding(12001);
				int num3 = 4;
				goto IL_0423;
			}
			case '5':
				break;
			case '6':
			{
				encoding = Encoding.Unicode;
				int num3 = 2;
				goto IL_0423;
			}
			case '7':
			{
				encoding = Encoding.UTF7;
				int num3 = 1;
				goto IL_0423;
			}
			case '8':
			{
				encoding = Encoding.UTF8;
				int num3 = 1;
				goto IL_0423;
			}
			default:
				if (c2 == 'b')
				{
					encoding = Encoding.BigEndianUnicode;
					int num3 = 2;
					goto IL_0423;
				}
				break;
			}
			throw new ArgumentException("Invalid format for $ specifier", "description");
			IL_0423:
			if (b.align == -1)
			{
				b.align = 4;
			}
			b.Add(encoding.GetBytes(Convert.ToString(oarg)));
			if (flag)
			{
				int num3;
				b.Add(new byte[num3]);
				return true;
			}
			return true;
			IL_0457:
			throw new ArgumentException(string.Format("invalid format specified `{0}'", b.description[b.i]));
		}

		private static bool Prepare(byte[] buffer, ref int idx, int size, ref bool align)
		{
			if (align)
			{
				idx = DataConverter.Align(idx, size);
				align = false;
			}
			if (idx + size > buffer.Length)
			{
				idx = buffer.Length;
				return false;
			}
			return true;
		}

		public static IList Unpack(string description, byte[] buffer, int startIndex)
		{
			DataConverter dataConverter = DataConverter.CopyConv;
			List<object> list = new List<object>();
			int num = startIndex;
			bool flag = false;
			int num2 = 0;
			int num3 = 0;
			while (num3 < description.Length && num < buffer.Length)
			{
				int num4 = num3;
				char c = description[num3];
				if (c <= 'S')
				{
					if (c <= 'C')
					{
						switch (c)
						{
						case '!':
							flag = true;
							break;
						case '"':
						case '#':
						case '&':
						case '\'':
						case '(':
						case ')':
						case '+':
						case ',':
						case '-':
						case '.':
						case '/':
						case '0':
							goto IL_05C2;
						case '$':
							goto IL_03E0;
						case '%':
							dataConverter = DataConverter.Native;
							break;
						case '*':
							num2 = int.MaxValue;
							break;
						case '1':
						case '2':
						case '3':
						case '4':
						case '5':
						case '6':
						case '7':
						case '8':
						case '9':
							num2 = (int)((short)description[num3] - 48);
							num4 = num3 + 1;
							break;
						default:
							if (c != 'C')
							{
								goto IL_05C2;
							}
							goto IL_0303;
						}
					}
					else if (c != 'I')
					{
						if (c != 'L')
						{
							if (c != 'S')
							{
								goto IL_05C2;
							}
							if (DataConverter.Prepare(buffer, ref num, 2, ref flag))
							{
								list.Add(dataConverter.GetUInt16(buffer, num));
								num += 2;
							}
						}
						else if (DataConverter.Prepare(buffer, ref num, 8, ref flag))
						{
							list.Add(dataConverter.GetUInt64(buffer, num));
							num += 8;
						}
					}
					else if (DataConverter.Prepare(buffer, ref num, 4, ref flag))
					{
						list.Add(dataConverter.GetUInt32(buffer, num));
						num += 4;
					}
				}
				else if (c <= 'l')
				{
					switch (c)
					{
					case '[':
					{
						int num5 = -1;
						int num6 = num3 + 1;
						while (num6 < description.Length && description[num6] != ']')
						{
							int num7 = (int)((short)description[num6] - 48);
							if (num7 >= 0 && num7 <= 9)
							{
								if (num5 == -1)
								{
									num5 = num7;
								}
								else
								{
									num5 = num5 * 10 + num7;
								}
							}
							num6++;
						}
						if (num5 == -1)
						{
							throw new ArgumentException("invalid size specification");
						}
						num3 = num6;
						num4 = num3 + 1;
						num2 = num5;
						break;
					}
					case '\\':
					case ']':
					case '`':
					case 'a':
					case 'e':
					case 'g':
					case 'h':
						goto IL_05C2;
					case '^':
						dataConverter = DataConverter.BigEndian;
						break;
					case '_':
						dataConverter = DataConverter.LittleEndian;
						break;
					case 'b':
						if (DataConverter.Prepare(buffer, ref num, 1, ref flag))
						{
							list.Add(buffer[num]);
							num++;
						}
						break;
					case 'c':
						goto IL_0303;
					case 'd':
						if (DataConverter.Prepare(buffer, ref num, 8, ref flag))
						{
							list.Add(dataConverter.GetDouble(buffer, num));
							num += 8;
						}
						break;
					case 'f':
						if (DataConverter.Prepare(buffer, ref num, 4, ref flag))
						{
							list.Add(dataConverter.GetFloat(buffer, num));
							num += 4;
						}
						break;
					case 'i':
						if (DataConverter.Prepare(buffer, ref num, 4, ref flag))
						{
							list.Add(dataConverter.GetInt32(buffer, num));
							num += 4;
						}
						break;
					default:
						if (c != 'l')
						{
							goto IL_05C2;
						}
						if (DataConverter.Prepare(buffer, ref num, 8, ref flag))
						{
							list.Add(dataConverter.GetInt64(buffer, num));
							num += 8;
						}
						break;
					}
				}
				else if (c != 's')
				{
					if (c != 'x')
					{
						if (c != 'z')
						{
							goto IL_05C2;
						}
						goto IL_03E0;
					}
					else
					{
						num++;
					}
				}
				else if (DataConverter.Prepare(buffer, ref num, 2, ref flag))
				{
					list.Add(dataConverter.GetInt16(buffer, num));
					num += 2;
				}
				IL_05DF:
				if (num2 <= 0)
				{
					num3++;
					continue;
				}
				if (--num2 > 0)
				{
					num3 = num4;
					continue;
				}
				continue;
				IL_0303:
				if (DataConverter.Prepare(buffer, ref num, 1, ref flag))
				{
					char c2;
					if (description[num3] == 'c')
					{
						c2 = (char)((sbyte)buffer[num]);
					}
					else
					{
						c2 = (char)buffer[num];
					}
					list.Add(c2);
					num++;
					goto IL_05DF;
				}
				goto IL_05DF;
				IL_03E0:
				num3++;
				if (num3 >= description.Length)
				{
					throw new ArgumentException("$ description needs a type specified", "description");
				}
				char c3 = description[num3];
				if (flag)
				{
					num = DataConverter.Align(num, 4);
					flag = false;
				}
				if (num < buffer.Length)
				{
					int num7;
					Encoding encoding;
					switch (c3)
					{
					case '3':
						encoding = Encoding.GetEncoding(12000);
						num7 = 4;
						break;
					case '4':
						encoding = Encoding.GetEncoding(12001);
						num7 = 4;
						break;
					case '5':
						goto IL_049C;
					case '6':
						encoding = Encoding.Unicode;
						num7 = 2;
						break;
					case '7':
						encoding = Encoding.UTF7;
						num7 = 1;
						break;
					case '8':
						encoding = Encoding.UTF8;
						num7 = 1;
						break;
					default:
						if (c3 != 'b')
						{
							goto IL_049C;
						}
						encoding = Encoding.BigEndianUnicode;
						num7 = 2;
						break;
					}
					int i = num;
					switch (num7)
					{
					case 1:
						while (i < buffer.Length && buffer[i] != 0)
						{
							i++;
						}
						list.Add(encoding.GetChars(buffer, num, i - num));
						if (i == buffer.Length)
						{
							num = i;
							goto IL_05DF;
						}
						num = i + 1;
						goto IL_05DF;
					case 2:
						while (i < buffer.Length)
						{
							if (i + 1 == buffer.Length)
							{
								i++;
								break;
							}
							if (buffer[i] == 0 && buffer[i + 1] == 0)
							{
								break;
							}
							i++;
						}
						list.Add(encoding.GetChars(buffer, num, i - num));
						if (i == buffer.Length)
						{
							num = i;
							goto IL_05DF;
						}
						num = i + 2;
						goto IL_05DF;
					case 3:
						goto IL_05DF;
					case 4:
						while (i < buffer.Length)
						{
							if (i + 3 >= buffer.Length)
							{
								i = buffer.Length;
								break;
							}
							if (buffer[i] == 0 && buffer[i + 1] == 0 && buffer[i + 2] == 0 && buffer[i + 3] == 0)
							{
								break;
							}
							i++;
						}
						list.Add(encoding.GetChars(buffer, num, i - num));
						if (i == buffer.Length)
						{
							num = i;
							goto IL_05DF;
						}
						num = i + 4;
						goto IL_05DF;
					default:
						goto IL_05DF;
					}
					IL_049C:
					throw new ArgumentException("Invalid format for $ specifier", "description");
				}
				goto IL_05DF;
				IL_05C2:
				throw new ArgumentException(string.Format("invalid format specified `{0}'", description[num3]));
			}
			return list;
		}

		internal void Check(byte[] dest, int destIdx, int size)
		{
			if (dest == null)
			{
				throw new ArgumentNullException("dest");
			}
			if (destIdx < 0 || destIdx > dest.Length - size)
			{
				throw new ArgumentException("destIdx");
			}
		}

		private static readonly DataConverter SwapConv = new DataConverter.SwapConverter();

		private static readonly DataConverter CopyConv = new DataConverter.CopyConverter();

		public static readonly bool IsLittleEndian = BitConverter.IsLittleEndian;

		private class PackContext
		{
			public void Add(byte[] group)
			{
				if (this.buffer == null)
				{
					this.buffer = group;
					this.next = group.Length;
					return;
				}
				if (this.align != 0)
				{
					if (this.align == -1)
					{
						this.next = DataConverter.Align(this.next, group.Length);
					}
					else
					{
						this.next = DataConverter.Align(this.next, this.align);
					}
					this.align = 0;
				}
				if (this.next + group.Length > this.buffer.Length)
				{
					byte[] array = new byte[Math.Max(this.next, 16) * 2 + group.Length];
					Array.Copy(this.buffer, array, this.buffer.Length);
					Array.Copy(group, 0, array, this.next, group.Length);
					this.next += group.Length;
					this.buffer = array;
					return;
				}
				Array.Copy(group, 0, this.buffer, this.next, group.Length);
				this.next += group.Length;
			}

			public byte[] Get()
			{
				if (this.buffer == null)
				{
					return new byte[0];
				}
				if (this.buffer.Length != this.next)
				{
					byte[] array = new byte[this.next];
					Array.Copy(this.buffer, array, this.next);
					return array;
				}
				return this.buffer;
			}

			public byte[] buffer;

			private int next;

			public string description;

			public int i;

			public DataConverter conv;

			public int repeat;

			public int align;
		}

		private class CopyConverter : DataConverter
		{
			public unsafe override double GetDouble(byte[] data, int index)
			{
				if (data == null)
				{
					throw new ArgumentNullException("data");
				}
				if (data.Length - index < 8)
				{
					throw new ArgumentException("index");
				}
				if (index < 0)
				{
					throw new ArgumentException("index");
				}
				double num;
				byte* ptr = (byte*)(&num);
				for (int i = 0; i < 8; i++)
				{
					ptr[i] = data[index + i];
				}
				return num;
			}

			public unsafe override ulong GetUInt64(byte[] data, int index)
			{
				if (data == null)
				{
					throw new ArgumentNullException("data");
				}
				if (data.Length - index < 8)
				{
					throw new ArgumentException("index");
				}
				if (index < 0)
				{
					throw new ArgumentException("index");
				}
				ulong num;
				byte* ptr = (byte*)(&num);
				for (int i = 0; i < 8; i++)
				{
					ptr[i] = data[index + i];
				}
				return num;
			}

			public unsafe override long GetInt64(byte[] data, int index)
			{
				if (data == null)
				{
					throw new ArgumentNullException("data");
				}
				if (data.Length - index < 8)
				{
					throw new ArgumentException("index");
				}
				if (index < 0)
				{
					throw new ArgumentException("index");
				}
				long num;
				byte* ptr = (byte*)(&num);
				for (int i = 0; i < 8; i++)
				{
					ptr[i] = data[index + i];
				}
				return num;
			}

			public unsafe override float GetFloat(byte[] data, int index)
			{
				if (data == null)
				{
					throw new ArgumentNullException("data");
				}
				if (data.Length - index < 4)
				{
					throw new ArgumentException("index");
				}
				if (index < 0)
				{
					throw new ArgumentException("index");
				}
				float num;
				byte* ptr = (byte*)(&num);
				for (int i = 0; i < 4; i++)
				{
					ptr[i] = data[index + i];
				}
				return num;
			}

			public unsafe override int GetInt32(byte[] data, int index)
			{
				if (data == null)
				{
					throw new ArgumentNullException("data");
				}
				if (data.Length - index < 4)
				{
					throw new ArgumentException("index");
				}
				if (index < 0)
				{
					throw new ArgumentException("index");
				}
				int num;
				byte* ptr = (byte*)(&num);
				for (int i = 0; i < 4; i++)
				{
					ptr[i] = data[index + i];
				}
				return num;
			}

			public unsafe override uint GetUInt32(byte[] data, int index)
			{
				if (data == null)
				{
					throw new ArgumentNullException("data");
				}
				if (data.Length - index < 4)
				{
					throw new ArgumentException("index");
				}
				if (index < 0)
				{
					throw new ArgumentException("index");
				}
				uint num;
				byte* ptr = (byte*)(&num);
				for (int i = 0; i < 4; i++)
				{
					ptr[i] = data[index + i];
				}
				return num;
			}

			public unsafe override short GetInt16(byte[] data, int index)
			{
				if (data == null)
				{
					throw new ArgumentNullException("data");
				}
				if (data.Length - index < 2)
				{
					throw new ArgumentException("index");
				}
				if (index < 0)
				{
					throw new ArgumentException("index");
				}
				short num;
				byte* ptr = (byte*)(&num);
				for (int i = 0; i < 2; i++)
				{
					ptr[i] = data[index + i];
				}
				return num;
			}

			public unsafe override ushort GetUInt16(byte[] data, int index)
			{
				if (data == null)
				{
					throw new ArgumentNullException("data");
				}
				if (data.Length - index < 2)
				{
					throw new ArgumentException("index");
				}
				if (index < 0)
				{
					throw new ArgumentException("index");
				}
				ushort num;
				byte* ptr = (byte*)(&num);
				for (int i = 0; i < 2; i++)
				{
					ptr[i] = data[index + i];
				}
				return num;
			}

			public unsafe override void PutBytes(byte[] dest, int destIdx, double value)
			{
				base.Check(dest, destIdx, 8);
				fixed (byte* ptr = &dest[destIdx])
				{
					ref long ptr2 = ref *(long*)ptr;
					long* ptr3 = (long*)(&value);
					ptr2 = *ptr3;
				}
			}

			public unsafe override void PutBytes(byte[] dest, int destIdx, float value)
			{
				base.Check(dest, destIdx, 4);
				fixed (byte* ptr = &dest[destIdx])
				{
					ref int ptr2 = ref *(int*)ptr;
					uint* ptr3 = (uint*)(&value);
					ptr2 = (int)(*ptr3);
				}
			}

			public unsafe override void PutBytes(byte[] dest, int destIdx, int value)
			{
				base.Check(dest, destIdx, 4);
				fixed (byte* ptr = &dest[destIdx])
				{
					ref int ptr2 = ref *(int*)ptr;
					uint* ptr3 = (uint*)(&value);
					ptr2 = (int)(*ptr3);
				}
			}

			public unsafe override void PutBytes(byte[] dest, int destIdx, uint value)
			{
				base.Check(dest, destIdx, 4);
				fixed (byte* ptr = &dest[destIdx])
				{
					ref int ptr2 = ref *(int*)ptr;
					uint* ptr3 = &value;
					ptr2 = (int)(*ptr3);
				}
			}

			public unsafe override void PutBytes(byte[] dest, int destIdx, long value)
			{
				base.Check(dest, destIdx, 8);
				fixed (byte* ptr = &dest[destIdx])
				{
					ref long ptr2 = ref *(long*)ptr;
					long* ptr3 = &value;
					ptr2 = *ptr3;
				}
			}

			public unsafe override void PutBytes(byte[] dest, int destIdx, ulong value)
			{
				base.Check(dest, destIdx, 8);
				fixed (byte* ptr = &dest[destIdx])
				{
					ref long ptr2 = ref *(long*)ptr;
					ulong* ptr3 = &value;
					ptr2 = (long)(*ptr3);
				}
			}

			public unsafe override void PutBytes(byte[] dest, int destIdx, short value)
			{
				base.Check(dest, destIdx, 2);
				fixed (byte* ptr = &dest[destIdx])
				{
					ref short ptr2 = ref *(short*)ptr;
					ushort* ptr3 = (ushort*)(&value);
					ptr2 = (short)(*ptr3);
				}
			}

			public unsafe override void PutBytes(byte[] dest, int destIdx, ushort value)
			{
				base.Check(dest, destIdx, 2);
				fixed (byte* ptr = &dest[destIdx])
				{
					ref short ptr2 = ref *(short*)ptr;
					ushort* ptr3 = &value;
					ptr2 = (short)(*ptr3);
				}
			}
		}

		private class SwapConverter : DataConverter
		{
			public unsafe override double GetDouble(byte[] data, int index)
			{
				if (data == null)
				{
					throw new ArgumentNullException("data");
				}
				if (data.Length - index < 8)
				{
					throw new ArgumentException("index");
				}
				if (index < 0)
				{
					throw new ArgumentException("index");
				}
				double num;
				byte* ptr = (byte*)(&num);
				for (int i = 0; i < 8; i++)
				{
					ptr[7 - i] = data[index + i];
				}
				return num;
			}

			public unsafe override ulong GetUInt64(byte[] data, int index)
			{
				if (data == null)
				{
					throw new ArgumentNullException("data");
				}
				if (data.Length - index < 8)
				{
					throw new ArgumentException("index");
				}
				if (index < 0)
				{
					throw new ArgumentException("index");
				}
				ulong num;
				byte* ptr = (byte*)(&num);
				for (int i = 0; i < 8; i++)
				{
					ptr[7 - i] = data[index + i];
				}
				return num;
			}

			public unsafe override long GetInt64(byte[] data, int index)
			{
				if (data == null)
				{
					throw new ArgumentNullException("data");
				}
				if (data.Length - index < 8)
				{
					throw new ArgumentException("index");
				}
				if (index < 0)
				{
					throw new ArgumentException("index");
				}
				long num;
				byte* ptr = (byte*)(&num);
				for (int i = 0; i < 8; i++)
				{
					ptr[7 - i] = data[index + i];
				}
				return num;
			}

			public unsafe override float GetFloat(byte[] data, int index)
			{
				if (data == null)
				{
					throw new ArgumentNullException("data");
				}
				if (data.Length - index < 4)
				{
					throw new ArgumentException("index");
				}
				if (index < 0)
				{
					throw new ArgumentException("index");
				}
				float num;
				byte* ptr = (byte*)(&num);
				for (int i = 0; i < 4; i++)
				{
					ptr[3 - i] = data[index + i];
				}
				return num;
			}

			public unsafe override int GetInt32(byte[] data, int index)
			{
				if (data == null)
				{
					throw new ArgumentNullException("data");
				}
				if (data.Length - index < 4)
				{
					throw new ArgumentException("index");
				}
				if (index < 0)
				{
					throw new ArgumentException("index");
				}
				int num;
				byte* ptr = (byte*)(&num);
				for (int i = 0; i < 4; i++)
				{
					ptr[3 - i] = data[index + i];
				}
				return num;
			}

			public unsafe override uint GetUInt32(byte[] data, int index)
			{
				if (data == null)
				{
					throw new ArgumentNullException("data");
				}
				if (data.Length - index < 4)
				{
					throw new ArgumentException("index");
				}
				if (index < 0)
				{
					throw new ArgumentException("index");
				}
				uint num;
				byte* ptr = (byte*)(&num);
				for (int i = 0; i < 4; i++)
				{
					ptr[3 - i] = data[index + i];
				}
				return num;
			}

			public unsafe override short GetInt16(byte[] data, int index)
			{
				if (data == null)
				{
					throw new ArgumentNullException("data");
				}
				if (data.Length - index < 2)
				{
					throw new ArgumentException("index");
				}
				if (index < 0)
				{
					throw new ArgumentException("index");
				}
				short num;
				byte* ptr = (byte*)(&num);
				for (int i = 0; i < 2; i++)
				{
					ptr[1 - i] = data[index + i];
				}
				return num;
			}

			public unsafe override ushort GetUInt16(byte[] data, int index)
			{
				if (data == null)
				{
					throw new ArgumentNullException("data");
				}
				if (data.Length - index < 2)
				{
					throw new ArgumentException("index");
				}
				if (index < 0)
				{
					throw new ArgumentException("index");
				}
				ushort num;
				byte* ptr = (byte*)(&num);
				for (int i = 0; i < 2; i++)
				{
					ptr[1 - i] = data[index + i];
				}
				return num;
			}

			public unsafe override void PutBytes(byte[] dest, int destIdx, double value)
			{
				base.Check(dest, destIdx, 8);
				fixed (byte* ptr = &dest[destIdx])
				{
					byte* ptr2 = ptr;
					byte* ptr3 = (byte*)(&value);
					for (int i = 0; i < 8; i++)
					{
						ptr2[i] = ptr3[7 - i];
					}
				}
			}

			public unsafe override void PutBytes(byte[] dest, int destIdx, float value)
			{
				base.Check(dest, destIdx, 4);
				fixed (byte* ptr = &dest[destIdx])
				{
					byte* ptr2 = ptr;
					byte* ptr3 = (byte*)(&value);
					for (int i = 0; i < 4; i++)
					{
						ptr2[i] = ptr3[3 - i];
					}
				}
			}

			public unsafe override void PutBytes(byte[] dest, int destIdx, int value)
			{
				base.Check(dest, destIdx, 4);
				fixed (byte* ptr = &dest[destIdx])
				{
					byte* ptr2 = ptr;
					byte* ptr3 = (byte*)(&value);
					for (int i = 0; i < 4; i++)
					{
						ptr2[i] = ptr3[3 - i];
					}
				}
			}

			public unsafe override void PutBytes(byte[] dest, int destIdx, uint value)
			{
				base.Check(dest, destIdx, 4);
				fixed (byte* ptr = &dest[destIdx])
				{
					byte* ptr2 = ptr;
					byte* ptr3 = (byte*)(&value);
					for (int i = 0; i < 4; i++)
					{
						ptr2[i] = ptr3[3 - i];
					}
				}
			}

			public unsafe override void PutBytes(byte[] dest, int destIdx, long value)
			{
				base.Check(dest, destIdx, 8);
				fixed (byte* ptr = &dest[destIdx])
				{
					byte* ptr2 = ptr;
					byte* ptr3 = (byte*)(&value);
					for (int i = 0; i < 8; i++)
					{
						ptr2[i] = ptr3[7 - i];
					}
				}
			}

			public unsafe override void PutBytes(byte[] dest, int destIdx, ulong value)
			{
				base.Check(dest, destIdx, 8);
				fixed (byte* ptr = &dest[destIdx])
				{
					byte* ptr2 = ptr;
					byte* ptr3 = (byte*)(&value);
					for (int i = 0; i < 8; i++)
					{
						ptr2[i] = ptr3[7 - i];
					}
				}
			}

			public unsafe override void PutBytes(byte[] dest, int destIdx, short value)
			{
				base.Check(dest, destIdx, 2);
				fixed (byte* ptr = &dest[destIdx])
				{
					byte* ptr2 = ptr;
					byte* ptr3 = (byte*)(&value);
					for (int i = 0; i < 2; i++)
					{
						ptr2[i] = ptr3[1 - i];
					}
				}
			}

			public unsafe override void PutBytes(byte[] dest, int destIdx, ushort value)
			{
				base.Check(dest, destIdx, 2);
				fixed (byte* ptr = &dest[destIdx])
				{
					byte* ptr2 = ptr;
					byte* ptr3 = (byte*)(&value);
					for (int i = 0; i < 2; i++)
					{
						ptr2[i] = ptr3[1 - i];
					}
				}
			}
		}
	}
}
