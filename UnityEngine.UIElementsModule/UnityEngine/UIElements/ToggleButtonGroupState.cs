using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct ToggleButtonGroupState : IEquatable<ToggleButtonGroupState>, IComparable<ToggleButtonGroupState>
	{
		public ToggleButtonGroupState(ulong optionsBitMask, int length)
		{
			bool flag = length < 0 || length > 64;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("length", string.Format("length of {0} should be greater than or equal to 0 and less than or equal to {1}.", length, 64));
			}
			this.m_Data = optionsBitMask;
			this.m_Length = length;
			this.ResetOptions(this.m_Length);
		}

		public int length
		{
			get
			{
				return this.m_Length;
			}
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			internal set
			{
				this.m_Length = value;
			}
		}

		internal ulong data
		{
			get
			{
				return this.m_Data;
			}
		}

		public bool this[int index]
		{
			get
			{
				bool flag = index < 0 || index >= this.m_Length;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("index", string.Format("index of {0} should be in the range of 0 and {1} inclusively.", index, this.m_Length - 1));
				}
				ulong num = 1UL << index;
				return (this.m_Data & num) == num;
			}
			set
			{
				bool flag = index < 0 || index >= this.m_Length;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("index", string.Format("index of {0} should be in the range of 0 and {1} inclusively.", index, this.m_Length - 1));
				}
				ulong num = 1UL << index;
				if (value)
				{
					this.m_Data |= num;
				}
				else
				{
					this.m_Data &= ~num;
				}
			}
		}

		public unsafe Span<int> GetActiveOptions(Span<int> activeOptionsIndices)
		{
			bool flag = activeOptionsIndices.Length < this.m_Length;
			if (flag)
			{
				throw new ArgumentException(string.Format("indices' length ({0}) should be equal to or greater than the ToggleButtonGroupState's length ({1}).", activeOptionsIndices.Length, this.m_Length));
			}
			int num = 0;
			for (int i = 0; i < this.m_Length; i++)
			{
				bool flag2 = !this[i];
				if (!flag2)
				{
					*activeOptionsIndices[num] = i;
					num++;
				}
			}
			return activeOptionsIndices.Slice(0, num);
		}

		public unsafe Span<int> GetInactiveOptions(Span<int> inactiveOptionsIndices)
		{
			bool flag = inactiveOptionsIndices.Length < this.m_Length;
			if (flag)
			{
				throw new ArgumentException(string.Format("indices' length ({0}) should be equal to or greater than the ToggleButtonGroupState's length ({1}).", inactiveOptionsIndices.Length, this.m_Length));
			}
			int num = 0;
			for (int i = 0; i < this.m_Length; i++)
			{
				bool flag2 = this[i];
				if (!flag2)
				{
					*inactiveOptionsIndices[num] = i;
					num++;
				}
			}
			return inactiveOptionsIndices.Slice(0, num);
		}

		public void SetAllOptions()
		{
			this.m_Data = ulong.MaxValue;
			this.ResetOptions(this.m_Length);
		}

		public void ResetAllOptions()
		{
			this.m_Data = 0UL;
		}

		public void ToggleAllOptions()
		{
			this.m_Data = ~this.m_Data;
			this.ResetOptions(this.m_Length);
		}

		public static ToggleButtonGroupState CreateFromOptions(IList<bool> options)
		{
			int count = options.Count;
			ToggleButtonGroupState toggleButtonGroupState = new ToggleButtonGroupState(0UL, count);
			for (int i = 0; i < count; i++)
			{
				toggleButtonGroupState[i] = options[i];
			}
			return toggleButtonGroupState;
		}

		public unsafe static ToggleButtonGroupState FromEnumFlags<T>(T options, int length = -1) where T : Enum
		{
			bool flag = !TypeTraits<T>.IsEnumFlags;
			if (flag)
			{
				throw new ArgumentException("Enum type " + typeof(T).Name + " is not a flag enum type.");
			}
			Type underlyingType = Enum.GetUnderlyingType(typeof(T));
			bool flag2 = length == -1;
			if (flag2)
			{
				TypeCode typeCode = Type.GetTypeCode(underlyingType);
				if (!true)
				{
				}
				int num;
				switch (typeCode)
				{
				case TypeCode.SByte:
					num = 8;
					break;
				case TypeCode.Byte:
					num = 8;
					break;
				case TypeCode.Int16:
					num = 16;
					break;
				case TypeCode.UInt16:
					num = 16;
					break;
				case TypeCode.Int32:
					num = 32;
					break;
				case TypeCode.UInt32:
					num = 32;
					break;
				case TypeCode.Int64:
					num = 64;
					break;
				case TypeCode.UInt64:
					num = 64;
					break;
				default:
					num = 0;
					break;
				}
				if (!true)
				{
				}
				length = num;
			}
			return new ToggleButtonGroupState((ulong)((long)(*UnsafeUtility.As<T, int>(ref options))), length);
		}

		public static T ToEnumFlags<T>(ToggleButtonGroupState options, bool acceptsLengthMismatch = true) where T : Enum
		{
			bool flag = !TypeTraits<T>.IsEnumFlags;
			if (flag)
			{
				throw new ArgumentException("Enum type " + typeof(T).Name + " is not a flag enum type.");
			}
			Type underlyingType = Enum.GetUnderlyingType(typeof(T));
			TypeCode typeCode = Type.GetTypeCode(underlyingType);
			if (!true)
			{
			}
			int num;
			switch (typeCode)
			{
			case TypeCode.SByte:
				num = 8;
				break;
			case TypeCode.Byte:
				num = 8;
				break;
			case TypeCode.Int16:
				num = 16;
				break;
			case TypeCode.UInt16:
				num = 16;
				break;
			case TypeCode.Int32:
				num = 32;
				break;
			case TypeCode.UInt32:
				num = 32;
				break;
			case TypeCode.Int64:
				num = 64;
				break;
			case TypeCode.UInt64:
				num = 64;
				break;
			default:
				num = -1;
				break;
			}
			if (!true)
			{
			}
			int num2 = num;
			bool flag2 = !acceptsLengthMismatch && options.m_Length != num2;
			if (flag2)
			{
				throw new ArgumentException("Cannot sync to enum flag since the ToggleButtonGroupState has a different amount of options.");
			}
			return (T)((object)Enum.Parse(typeof(T), options.m_Data.ToString()));
		}

		public int CompareTo(ToggleButtonGroupState other)
		{
			return (other == this) ? 1 : (-1);
		}

		public unsafe static bool Compare<T>(ToggleButtonGroupState options, T value) where T : Enum
		{
			bool flag = !TypeTraits<T>.IsEnumFlags;
			if (flag)
			{
				throw new ArgumentException("Enum type " + typeof(T).Name + " is not a flag enum type.");
			}
			ulong num = (ulong)((long)(*UnsafeUtility.As<T, int>(ref value)));
			return options.m_Data == num;
		}

		private void ResetOptions(int startingIndex)
		{
			for (int i = startingIndex; i < 64; i++)
			{
				ulong num = 1UL << i;
				this.m_Data &= ~num;
			}
		}

		public static bool operator ==(ToggleButtonGroupState lhs, ToggleButtonGroupState rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(ToggleButtonGroupState lhs, ToggleButtonGroupState rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(ToggleButtonGroupState other)
		{
			return this.m_Data == other.m_Data && this.m_Length == other.m_Length;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is ToggleButtonGroupState)
			{
				ToggleButtonGroupState toggleButtonGroupState = (ToggleButtonGroupState)obj;
				flag = this.Equals(toggleButtonGroupState);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<ulong, int>(this.m_Data, this.m_Length);
		}

		public override string ToString()
		{
			return Convert.ToString((long)this.m_Data, 2).PadLeft(this.length, '0');
		}

		internal const int maxLength = 64;

		[SerializeField]
		private ulong m_Data;

		[SerializeField]
		private int m_Length;
	}
}
