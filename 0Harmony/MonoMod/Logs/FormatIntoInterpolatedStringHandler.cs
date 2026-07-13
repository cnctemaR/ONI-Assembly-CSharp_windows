using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Logs
{
	[NullableContext(2)]
	[Nullable(0)]
	[InterpolatedStringHandler]
	internal ref struct FormatIntoInterpolatedStringHandler
	{
		[NullableContext(0)]
		public FormatIntoInterpolatedStringHandler(int literalLen, int numHoles, global::System.Span<char> into, out bool enabled)
		{
			this._chars = into;
			this.pos = 0;
			if (into.Length < literalLen)
			{
				this.incomplete = true;
				enabled = false;
				return;
			}
			this.incomplete = false;
			enabled = true;
		}

		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool AppendLiteral(string value)
		{
			if (value.Length == 1)
			{
				global::System.Span<char> chars = this._chars;
				int num = this.pos;
				if (num < chars.Length)
				{
					*chars[num] = value[0];
					this.pos = num + 1;
					return true;
				}
				this.incomplete = true;
				return false;
			}
			else
			{
				if (value.Length != 2)
				{
					return this.AppendStringDirect(value);
				}
				global::System.Span<char> chars2 = this._chars;
				int num2 = this.pos;
				if ((ulong)num2 < (ulong)((long)(chars2.Length - 1)))
				{
					value.AsSpan().CopyTo(chars2.Slice(num2));
					this.pos = num2 + 2;
					return true;
				}
				this.incomplete = true;
				return false;
			}
		}

		[NullableContext(1)]
		private bool AppendStringDirect(string value)
		{
			if (value.AsSpan().TryCopyTo(this._chars.Slice(this.pos)))
			{
				this.pos += value.Length;
				return true;
			}
			this.incomplete = true;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool AppendFormatted(string value)
		{
			if (value == null)
			{
				return true;
			}
			if (value.AsSpan().TryCopyTo(this._chars.Slice(this.pos)))
			{
				this.pos += value.Length;
				return true;
			}
			this.incomplete = true;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool AppendFormatted(string value, int alignment = 0, string format = null)
		{
			return this.AppendFormatted<string>(value, alignment, format);
		}

		[NullableContext(0)]
		public bool AppendFormatted(global::System.ReadOnlySpan<char> value)
		{
			if (value.TryCopyTo(this._chars.Slice(this.pos)))
			{
				this.pos += value.Length;
				return true;
			}
			this.incomplete = true;
			return false;
		}

		[NullableContext(0)]
		public bool AppendFormatted(global::System.ReadOnlySpan<char> value, int alignment = 0, [Nullable(2)] string format = null)
		{
			bool flag = false;
			if (alignment < 0)
			{
				flag = true;
				alignment = -alignment;
			}
			int num = alignment - value.Length;
			if (num <= 0)
			{
				return this.AppendFormatted(value);
			}
			if (this._chars.Slice(this.pos).Length < value.Length + num)
			{
				this.incomplete = true;
				return false;
			}
			if (flag)
			{
				value.CopyTo(this._chars.Slice(this.pos));
				this.pos += value.Length;
				this._chars.Slice(this.pos, num).Fill(' ');
				this.pos += num;
			}
			else
			{
				this._chars.Slice(this.pos, num).Fill(' ');
				this.pos += num;
				value.CopyTo(this._chars.Slice(this.pos));
				this.pos += value.Length;
			}
			return true;
		}

		[NullableContext(1)]
		public unsafe bool AppendFormatted<[Nullable(2)] T>(T value)
		{
			if (typeof(T) == typeof(IntPtr))
			{
				return this.AppendFormatted(*Unsafe.As<T, IntPtr>(ref value));
			}
			if (typeof(T) == typeof(UIntPtr))
			{
				return this.AppendFormatted(*Unsafe.As<T, UIntPtr>(ref value));
			}
			object obj;
			if (!DebugFormatter.CanDebugFormat<T>(in value, out obj))
			{
				string text;
				if (value is IFormattable)
				{
					text = ((IFormattable)((object)value)).ToString(null, null);
				}
				else
				{
					ref T ptr = ref value;
					T t = default(T);
					string text2;
					if (t == null)
					{
						t = value;
						ptr = ref t;
						if (t == null)
						{
							text2 = null;
							goto IL_00E8;
						}
					}
					text2 = ptr.ToString();
					IL_00E8:
					text = text2;
				}
				return text == null || this.AppendStringDirect(text);
			}
			int num;
			if (!DebugFormatter.TryFormatInto<T>(in value, obj, this._chars.Slice(this.pos), out num))
			{
				this.incomplete = true;
				return false;
			}
			this.pos += num;
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool AppendFormatted(IntPtr value)
		{
			if (IntPtr.Size == 4)
			{
				return this.AppendFormatted<int>((int)value);
			}
			return this.AppendFormatted<long>((long)value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool AppendFormatted(IntPtr value, string format)
		{
			if (IntPtr.Size == 4)
			{
				return this.AppendFormatted<int>((int)value, format);
			}
			return this.AppendFormatted<long>((long)value, format);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool AppendFormatted(UIntPtr value)
		{
			if (UIntPtr.Size == 4)
			{
				return this.AppendFormatted<uint>((uint)value);
			}
			return this.AppendFormatted<ulong>((ulong)value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool AppendFormatted(UIntPtr value, string format)
		{
			if (UIntPtr.Size == 4)
			{
				return this.AppendFormatted<uint>((uint)value, format);
			}
			return this.AppendFormatted<ulong>((ulong)value, format);
		}

		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool AppendFormatted<[Nullable(2)] T>(T value, int alignment)
		{
			int num = this.pos;
			return this.AppendFormatted<T>(value) && (alignment == 0 || this.AppendOrInsertAlignmentIfNeeded(num, alignment));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool AppendFormatted<T>([Nullable(1)] T value, string format)
		{
			if (typeof(T) == typeof(IntPtr))
			{
				return this.AppendFormatted(*Unsafe.As<T, IntPtr>(ref value), format);
			}
			if (typeof(T) == typeof(UIntPtr))
			{
				return this.AppendFormatted(*Unsafe.As<T, UIntPtr>(ref value), format);
			}
			object obj;
			if (!DebugFormatter.CanDebugFormat<T>(in value, out obj))
			{
				string text;
				if (value is IFormattable)
				{
					text = ((IFormattable)((object)value)).ToString(format, null);
				}
				else
				{
					ref T ptr = ref value;
					T t = default(T);
					string text2;
					if (t == null)
					{
						t = value;
						ptr = ref t;
						if (t == null)
						{
							text2 = null;
							goto IL_00EA;
						}
					}
					text2 = ptr.ToString();
					IL_00EA:
					text = text2;
				}
				return text == null || this.AppendStringDirect(text);
			}
			int num;
			if (!DebugFormatter.TryFormatInto<T>(in value, obj, this._chars.Slice(this.pos), out num))
			{
				this.incomplete = true;
				return false;
			}
			this.pos += num;
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool AppendFormatted<T>([Nullable(1)] T value, int alignment, string format)
		{
			int num = this.pos;
			return this.AppendFormatted<T>(value, format) && (alignment == 0 || this.AppendOrInsertAlignmentIfNeeded(num, alignment));
		}

		private bool AppendOrInsertAlignmentIfNeeded(int startingPos, int alignment)
		{
			int num = this.pos - startingPos;
			bool flag = false;
			if (alignment < 0)
			{
				flag = true;
				alignment = -alignment;
			}
			int num2 = alignment - num;
			if (num2 > 0)
			{
				if (this._chars.Slice(this.pos).Length < num2)
				{
					this.incomplete = true;
					return false;
				}
				if (flag)
				{
					this._chars.Slice(this.pos, num2).Fill(' ');
				}
				else
				{
					this._chars.Slice(startingPos, num).CopyTo(this._chars.Slice(startingPos + num2));
					this._chars.Slice(startingPos, num2).Fill(' ');
				}
				this.pos += num2;
			}
			return true;
		}

		[Nullable(0)]
		private readonly global::System.Span<char> _chars;

		internal int pos;

		internal bool incomplete;
	}
}
