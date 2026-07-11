using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[StructLayout(LayoutKind.Auto)]
	public struct ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> : IEquatable<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>, IStructuralEquatable, IStructuralComparable, IComparable, IComparable<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>, IValueTupleInternal, ITuple where TRest : struct
	{
		public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TRest rest)
		{
			if (!(rest is IValueTupleInternal))
			{
				throw new ArgumentException("The last element of an eight element ValueTuple must be a ValueTuple.");
			}
			this.Item1 = item1;
			this.Item2 = item2;
			this.Item3 = item3;
			this.Item4 = item4;
			this.Item5 = item5;
			this.Item6 = item6;
			this.Item7 = item7;
			this.Rest = rest;
		}

		public override bool Equals(object obj)
		{
			return obj is ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> && this.Equals((ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>)obj);
		}

		public bool Equals(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> other)
		{
			return EqualityComparer<T1>.Default.Equals(this.Item1, other.Item1) && EqualityComparer<T2>.Default.Equals(this.Item2, other.Item2) && EqualityComparer<T3>.Default.Equals(this.Item3, other.Item3) && EqualityComparer<T4>.Default.Equals(this.Item4, other.Item4) && EqualityComparer<T5>.Default.Equals(this.Item5, other.Item5) && EqualityComparer<T6>.Default.Equals(this.Item6, other.Item6) && EqualityComparer<T7>.Default.Equals(this.Item7, other.Item7) && EqualityComparer<TRest>.Default.Equals(this.Rest, other.Rest);
		}

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			if (other == null || !(other is ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>))
			{
				return false;
			}
			ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> valueTuple = (ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>)other;
			return comparer.Equals(this.Item1, valueTuple.Item1) && comparer.Equals(this.Item2, valueTuple.Item2) && comparer.Equals(this.Item3, valueTuple.Item3) && comparer.Equals(this.Item4, valueTuple.Item4) && comparer.Equals(this.Item5, valueTuple.Item5) && comparer.Equals(this.Item6, valueTuple.Item6) && comparer.Equals(this.Item7, valueTuple.Item7) && comparer.Equals(this.Rest, valueTuple.Rest);
		}

		int IComparable.CompareTo(object other)
		{
			if (other == null)
			{
				return 1;
			}
			if (!(other is ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>))
			{
				throw new ArgumentException(SR.Format("Argument must be of type {0}.", base.GetType().ToString()), "other");
			}
			return this.CompareTo((ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>)other);
		}

		public int CompareTo(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> other)
		{
			int num = Comparer<T1>.Default.Compare(this.Item1, other.Item1);
			if (num != 0)
			{
				return num;
			}
			num = Comparer<T2>.Default.Compare(this.Item2, other.Item2);
			if (num != 0)
			{
				return num;
			}
			num = Comparer<T3>.Default.Compare(this.Item3, other.Item3);
			if (num != 0)
			{
				return num;
			}
			num = Comparer<T4>.Default.Compare(this.Item4, other.Item4);
			if (num != 0)
			{
				return num;
			}
			num = Comparer<T5>.Default.Compare(this.Item5, other.Item5);
			if (num != 0)
			{
				return num;
			}
			num = Comparer<T6>.Default.Compare(this.Item6, other.Item6);
			if (num != 0)
			{
				return num;
			}
			num = Comparer<T7>.Default.Compare(this.Item7, other.Item7);
			if (num != 0)
			{
				return num;
			}
			return Comparer<TRest>.Default.Compare(this.Rest, other.Rest);
		}

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			if (other == null)
			{
				return 1;
			}
			if (!(other is ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>))
			{
				throw new ArgumentException(SR.Format("Argument must be of type {0}.", base.GetType().ToString()), "other");
			}
			ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> valueTuple = (ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>)other;
			int num = comparer.Compare(this.Item1, valueTuple.Item1);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(this.Item2, valueTuple.Item2);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(this.Item3, valueTuple.Item3);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(this.Item4, valueTuple.Item4);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(this.Item5, valueTuple.Item5);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(this.Item6, valueTuple.Item6);
			if (num != 0)
			{
				return num;
			}
			num = comparer.Compare(this.Item7, valueTuple.Item7);
			if (num != 0)
			{
				return num;
			}
			return comparer.Compare(this.Rest, valueTuple.Rest);
		}

		public override int GetHashCode()
		{
			IValueTupleInternal valueTupleInternal = this.Rest as IValueTupleInternal;
			if (valueTupleInternal == null)
			{
				ref T1 ptr = ref this.Item1;
				T1 t = default(T1);
				int num;
				if (t == null)
				{
					t = this.Item1;
					ptr = ref t;
					if (t == null)
					{
						num = 0;
						goto IL_004C;
					}
				}
				num = ptr.GetHashCode();
				IL_004C:
				ref T2 ptr2 = ref this.Item2;
				T2 t2 = default(T2);
				int num2;
				if (t2 == null)
				{
					t2 = this.Item2;
					ptr2 = ref t2;
					if (t2 == null)
					{
						num2 = 0;
						goto IL_0084;
					}
				}
				num2 = ptr2.GetHashCode();
				IL_0084:
				ref T3 ptr3 = ref this.Item3;
				T3 t3 = default(T3);
				int num3;
				if (t3 == null)
				{
					t3 = this.Item3;
					ptr3 = ref t3;
					if (t3 == null)
					{
						num3 = 0;
						goto IL_00BC;
					}
				}
				num3 = ptr3.GetHashCode();
				IL_00BC:
				ref T4 ptr4 = ref this.Item4;
				T4 t4 = default(T4);
				int num4;
				if (t4 == null)
				{
					t4 = this.Item4;
					ptr4 = ref t4;
					if (t4 == null)
					{
						num4 = 0;
						goto IL_00F4;
					}
				}
				num4 = ptr4.GetHashCode();
				IL_00F4:
				ref T5 ptr5 = ref this.Item5;
				T5 t5 = default(T5);
				int num5;
				if (t5 == null)
				{
					t5 = this.Item5;
					ptr5 = ref t5;
					if (t5 == null)
					{
						num5 = 0;
						goto IL_012C;
					}
				}
				num5 = ptr5.GetHashCode();
				IL_012C:
				ref T6 ptr6 = ref this.Item6;
				T6 t6 = default(T6);
				int num6;
				if (t6 == null)
				{
					t6 = this.Item6;
					ptr6 = ref t6;
					if (t6 == null)
					{
						num6 = 0;
						goto IL_0164;
					}
				}
				num6 = ptr6.GetHashCode();
				IL_0164:
				ref T7 ptr7 = ref this.Item7;
				T7 t7 = default(T7);
				int num7;
				if (t7 == null)
				{
					t7 = this.Item7;
					ptr7 = ref t7;
					if (t7 == null)
					{
						num7 = 0;
						goto IL_019C;
					}
				}
				num7 = ptr7.GetHashCode();
				IL_019C:
				return ValueTuple.CombineHashCodes(num, num2, num3, num4, num5, num6, num7);
			}
			int length = valueTupleInternal.Length;
			if (length >= 8)
			{
				return valueTupleInternal.GetHashCode();
			}
			switch (8 - length)
			{
			case 1:
			{
				ref T7 ptr8 = ref this.Item7;
				T7 t7 = default(T7);
				int num8;
				if (t7 == null)
				{
					t7 = this.Item7;
					ptr8 = ref t7;
					if (t7 == null)
					{
						num8 = 0;
						goto IL_021D;
					}
				}
				num8 = ptr8.GetHashCode();
				IL_021D:
				return ValueTuple.CombineHashCodes(num8, valueTupleInternal.GetHashCode());
			}
			case 2:
			{
				ref T6 ptr9 = ref this.Item6;
				T6 t6 = default(T6);
				int num9;
				if (t6 == null)
				{
					t6 = this.Item6;
					ptr9 = ref t6;
					if (t6 == null)
					{
						num9 = 0;
						goto IL_0261;
					}
				}
				num9 = ptr9.GetHashCode();
				IL_0261:
				ref T7 ptr10 = ref this.Item7;
				T7 t7 = default(T7);
				int num10;
				if (t7 == null)
				{
					t7 = this.Item7;
					ptr10 = ref t7;
					if (t7 == null)
					{
						num10 = 0;
						goto IL_0299;
					}
				}
				num10 = ptr10.GetHashCode();
				IL_0299:
				return ValueTuple.CombineHashCodes(num9, num10, valueTupleInternal.GetHashCode());
			}
			case 3:
			{
				ref T5 ptr11 = ref this.Item5;
				T5 t5 = default(T5);
				int num11;
				if (t5 == null)
				{
					t5 = this.Item5;
					ptr11 = ref t5;
					if (t5 == null)
					{
						num11 = 0;
						goto IL_02DD;
					}
				}
				num11 = ptr11.GetHashCode();
				IL_02DD:
				ref T6 ptr12 = ref this.Item6;
				T6 t6 = default(T6);
				int num12;
				if (t6 == null)
				{
					t6 = this.Item6;
					ptr12 = ref t6;
					if (t6 == null)
					{
						num12 = 0;
						goto IL_0315;
					}
				}
				num12 = ptr12.GetHashCode();
				IL_0315:
				ref T7 ptr13 = ref this.Item7;
				T7 t7 = default(T7);
				int num13;
				if (t7 == null)
				{
					t7 = this.Item7;
					ptr13 = ref t7;
					if (t7 == null)
					{
						num13 = 0;
						goto IL_034D;
					}
				}
				num13 = ptr13.GetHashCode();
				IL_034D:
				return ValueTuple.CombineHashCodes(num11, num12, num13, valueTupleInternal.GetHashCode());
			}
			case 4:
			{
				ref T4 ptr14 = ref this.Item4;
				T4 t4 = default(T4);
				int num14;
				if (t4 == null)
				{
					t4 = this.Item4;
					ptr14 = ref t4;
					if (t4 == null)
					{
						num14 = 0;
						goto IL_0391;
					}
				}
				num14 = ptr14.GetHashCode();
				IL_0391:
				ref T5 ptr15 = ref this.Item5;
				T5 t5 = default(T5);
				int num15;
				if (t5 == null)
				{
					t5 = this.Item5;
					ptr15 = ref t5;
					if (t5 == null)
					{
						num15 = 0;
						goto IL_03C9;
					}
				}
				num15 = ptr15.GetHashCode();
				IL_03C9:
				ref T6 ptr16 = ref this.Item6;
				T6 t6 = default(T6);
				int num16;
				if (t6 == null)
				{
					t6 = this.Item6;
					ptr16 = ref t6;
					if (t6 == null)
					{
						num16 = 0;
						goto IL_0401;
					}
				}
				num16 = ptr16.GetHashCode();
				IL_0401:
				ref T7 ptr17 = ref this.Item7;
				T7 t7 = default(T7);
				int num17;
				if (t7 == null)
				{
					t7 = this.Item7;
					ptr17 = ref t7;
					if (t7 == null)
					{
						num17 = 0;
						goto IL_0439;
					}
				}
				num17 = ptr17.GetHashCode();
				IL_0439:
				return ValueTuple.CombineHashCodes(num14, num15, num16, num17, valueTupleInternal.GetHashCode());
			}
			case 5:
			{
				ref T3 ptr18 = ref this.Item3;
				T3 t3 = default(T3);
				int num18;
				if (t3 == null)
				{
					t3 = this.Item3;
					ptr18 = ref t3;
					if (t3 == null)
					{
						num18 = 0;
						goto IL_047D;
					}
				}
				num18 = ptr18.GetHashCode();
				IL_047D:
				ref T4 ptr19 = ref this.Item4;
				T4 t4 = default(T4);
				int num19;
				if (t4 == null)
				{
					t4 = this.Item4;
					ptr19 = ref t4;
					if (t4 == null)
					{
						num19 = 0;
						goto IL_04B5;
					}
				}
				num19 = ptr19.GetHashCode();
				IL_04B5:
				ref T5 ptr20 = ref this.Item5;
				T5 t5 = default(T5);
				int num20;
				if (t5 == null)
				{
					t5 = this.Item5;
					ptr20 = ref t5;
					if (t5 == null)
					{
						num20 = 0;
						goto IL_04ED;
					}
				}
				num20 = ptr20.GetHashCode();
				IL_04ED:
				ref T6 ptr21 = ref this.Item6;
				T6 t6 = default(T6);
				int num21;
				if (t6 == null)
				{
					t6 = this.Item6;
					ptr21 = ref t6;
					if (t6 == null)
					{
						num21 = 0;
						goto IL_0525;
					}
				}
				num21 = ptr21.GetHashCode();
				IL_0525:
				ref T7 ptr22 = ref this.Item7;
				T7 t7 = default(T7);
				int num22;
				if (t7 == null)
				{
					t7 = this.Item7;
					ptr22 = ref t7;
					if (t7 == null)
					{
						num22 = 0;
						goto IL_055D;
					}
				}
				num22 = ptr22.GetHashCode();
				IL_055D:
				return ValueTuple.CombineHashCodes(num18, num19, num20, num21, num22, valueTupleInternal.GetHashCode());
			}
			case 6:
			{
				ref T2 ptr23 = ref this.Item2;
				T2 t2 = default(T2);
				int num23;
				if (t2 == null)
				{
					t2 = this.Item2;
					ptr23 = ref t2;
					if (t2 == null)
					{
						num23 = 0;
						goto IL_05A1;
					}
				}
				num23 = ptr23.GetHashCode();
				IL_05A1:
				ref T3 ptr24 = ref this.Item3;
				T3 t3 = default(T3);
				int num24;
				if (t3 == null)
				{
					t3 = this.Item3;
					ptr24 = ref t3;
					if (t3 == null)
					{
						num24 = 0;
						goto IL_05D9;
					}
				}
				num24 = ptr24.GetHashCode();
				IL_05D9:
				ref T4 ptr25 = ref this.Item4;
				T4 t4 = default(T4);
				int num25;
				if (t4 == null)
				{
					t4 = this.Item4;
					ptr25 = ref t4;
					if (t4 == null)
					{
						num25 = 0;
						goto IL_0611;
					}
				}
				num25 = ptr25.GetHashCode();
				IL_0611:
				ref T5 ptr26 = ref this.Item5;
				T5 t5 = default(T5);
				int num26;
				if (t5 == null)
				{
					t5 = this.Item5;
					ptr26 = ref t5;
					if (t5 == null)
					{
						num26 = 0;
						goto IL_0649;
					}
				}
				num26 = ptr26.GetHashCode();
				IL_0649:
				ref T6 ptr27 = ref this.Item6;
				T6 t6 = default(T6);
				int num27;
				if (t6 == null)
				{
					t6 = this.Item6;
					ptr27 = ref t6;
					if (t6 == null)
					{
						num27 = 0;
						goto IL_0681;
					}
				}
				num27 = ptr27.GetHashCode();
				IL_0681:
				ref T7 ptr28 = ref this.Item7;
				T7 t7 = default(T7);
				int num28;
				if (t7 == null)
				{
					t7 = this.Item7;
					ptr28 = ref t7;
					if (t7 == null)
					{
						num28 = 0;
						goto IL_06B9;
					}
				}
				num28 = ptr28.GetHashCode();
				IL_06B9:
				return ValueTuple.CombineHashCodes(num23, num24, num25, num26, num27, num28, valueTupleInternal.GetHashCode());
			}
			case 7:
			case 8:
			{
				ref T1 ptr29 = ref this.Item1;
				T1 t = default(T1);
				int num29;
				if (t == null)
				{
					t = this.Item1;
					ptr29 = ref t;
					if (t == null)
					{
						num29 = 0;
						goto IL_06FA;
					}
				}
				num29 = ptr29.GetHashCode();
				IL_06FA:
				ref T2 ptr30 = ref this.Item2;
				T2 t2 = default(T2);
				int num30;
				if (t2 == null)
				{
					t2 = this.Item2;
					ptr30 = ref t2;
					if (t2 == null)
					{
						num30 = 0;
						goto IL_0732;
					}
				}
				num30 = ptr30.GetHashCode();
				IL_0732:
				ref T3 ptr31 = ref this.Item3;
				T3 t3 = default(T3);
				int num31;
				if (t3 == null)
				{
					t3 = this.Item3;
					ptr31 = ref t3;
					if (t3 == null)
					{
						num31 = 0;
						goto IL_076A;
					}
				}
				num31 = ptr31.GetHashCode();
				IL_076A:
				ref T4 ptr32 = ref this.Item4;
				T4 t4 = default(T4);
				int num32;
				if (t4 == null)
				{
					t4 = this.Item4;
					ptr32 = ref t4;
					if (t4 == null)
					{
						num32 = 0;
						goto IL_07A2;
					}
				}
				num32 = ptr32.GetHashCode();
				IL_07A2:
				ref T5 ptr33 = ref this.Item5;
				T5 t5 = default(T5);
				int num33;
				if (t5 == null)
				{
					t5 = this.Item5;
					ptr33 = ref t5;
					if (t5 == null)
					{
						num33 = 0;
						goto IL_07DA;
					}
				}
				num33 = ptr33.GetHashCode();
				IL_07DA:
				ref T6 ptr34 = ref this.Item6;
				T6 t6 = default(T6);
				int num34;
				if (t6 == null)
				{
					t6 = this.Item6;
					ptr34 = ref t6;
					if (t6 == null)
					{
						num34 = 0;
						goto IL_0812;
					}
				}
				num34 = ptr34.GetHashCode();
				IL_0812:
				ref T7 ptr35 = ref this.Item7;
				T7 t7 = default(T7);
				int num35;
				if (t7 == null)
				{
					t7 = this.Item7;
					ptr35 = ref t7;
					if (t7 == null)
					{
						num35 = 0;
						goto IL_084A;
					}
				}
				num35 = ptr35.GetHashCode();
				IL_084A:
				return ValueTuple.CombineHashCodes(num29, num30, num31, num32, num33, num34, num35, valueTupleInternal.GetHashCode());
			}
			default:
				return -1;
			}
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			return this.GetHashCodeCore(comparer);
		}

		private int GetHashCodeCore(IEqualityComparer comparer)
		{
			IValueTupleInternal valueTupleInternal = this.Rest as IValueTupleInternal;
			if (valueTupleInternal == null)
			{
				return ValueTuple.CombineHashCodes(comparer.GetHashCode(this.Item1), comparer.GetHashCode(this.Item2), comparer.GetHashCode(this.Item3), comparer.GetHashCode(this.Item4), comparer.GetHashCode(this.Item5), comparer.GetHashCode(this.Item6), comparer.GetHashCode(this.Item7));
			}
			int length = valueTupleInternal.Length;
			if (length >= 8)
			{
				return valueTupleInternal.GetHashCode(comparer);
			}
			switch (8 - length)
			{
			case 1:
				return ValueTuple.CombineHashCodes(comparer.GetHashCode(this.Item7), valueTupleInternal.GetHashCode(comparer));
			case 2:
				return ValueTuple.CombineHashCodes(comparer.GetHashCode(this.Item6), comparer.GetHashCode(this.Item7), valueTupleInternal.GetHashCode(comparer));
			case 3:
				return ValueTuple.CombineHashCodes(comparer.GetHashCode(this.Item5), comparer.GetHashCode(this.Item6), comparer.GetHashCode(this.Item7), valueTupleInternal.GetHashCode(comparer));
			case 4:
				return ValueTuple.CombineHashCodes(comparer.GetHashCode(this.Item4), comparer.GetHashCode(this.Item5), comparer.GetHashCode(this.Item6), comparer.GetHashCode(this.Item7), valueTupleInternal.GetHashCode(comparer));
			case 5:
				return ValueTuple.CombineHashCodes(comparer.GetHashCode(this.Item3), comparer.GetHashCode(this.Item4), comparer.GetHashCode(this.Item5), comparer.GetHashCode(this.Item6), comparer.GetHashCode(this.Item7), valueTupleInternal.GetHashCode(comparer));
			case 6:
				return ValueTuple.CombineHashCodes(comparer.GetHashCode(this.Item2), comparer.GetHashCode(this.Item3), comparer.GetHashCode(this.Item4), comparer.GetHashCode(this.Item5), comparer.GetHashCode(this.Item6), comparer.GetHashCode(this.Item7), valueTupleInternal.GetHashCode(comparer));
			case 7:
			case 8:
				return ValueTuple.CombineHashCodes(comparer.GetHashCode(this.Item1), comparer.GetHashCode(this.Item2), comparer.GetHashCode(this.Item3), comparer.GetHashCode(this.Item4), comparer.GetHashCode(this.Item5), comparer.GetHashCode(this.Item6), comparer.GetHashCode(this.Item7), valueTupleInternal.GetHashCode(comparer));
			default:
				return -1;
			}
		}

		int IValueTupleInternal.GetHashCode(IEqualityComparer comparer)
		{
			return this.GetHashCodeCore(comparer);
		}

		public override string ToString()
		{
			IValueTupleInternal valueTupleInternal = this.Rest as IValueTupleInternal;
			T1 t;
			T2 t2;
			T3 t3;
			T4 t4;
			T5 t5;
			T6 t6;
			T7 t7;
			if (valueTupleInternal == null)
			{
				string[] array = new string[17];
				array[0] = "(";
				int num = 1;
				ref T1 ptr = ref this.Item1;
				t = default(T1);
				string text;
				if (t == null)
				{
					t = this.Item1;
					ptr = ref t;
					if (t == null)
					{
						text = null;
						goto IL_005D;
					}
				}
				text = ptr.ToString();
				IL_005D:
				array[num] = text;
				array[2] = ", ";
				int num2 = 3;
				ref T2 ptr2 = ref this.Item2;
				t2 = default(T2);
				string text2;
				if (t2 == null)
				{
					t2 = this.Item2;
					ptr2 = ref t2;
					if (t2 == null)
					{
						text2 = null;
						goto IL_009D;
					}
				}
				text2 = ptr2.ToString();
				IL_009D:
				array[num2] = text2;
				array[4] = ", ";
				int num3 = 5;
				ref T3 ptr3 = ref this.Item3;
				t3 = default(T3);
				string text3;
				if (t3 == null)
				{
					t3 = this.Item3;
					ptr3 = ref t3;
					if (t3 == null)
					{
						text3 = null;
						goto IL_00DD;
					}
				}
				text3 = ptr3.ToString();
				IL_00DD:
				array[num3] = text3;
				array[6] = ", ";
				int num4 = 7;
				ref T4 ptr4 = ref this.Item4;
				t4 = default(T4);
				string text4;
				if (t4 == null)
				{
					t4 = this.Item4;
					ptr4 = ref t4;
					if (t4 == null)
					{
						text4 = null;
						goto IL_0120;
					}
				}
				text4 = ptr4.ToString();
				IL_0120:
				array[num4] = text4;
				array[8] = ", ";
				int num5 = 9;
				ref T5 ptr5 = ref this.Item5;
				t5 = default(T5);
				string text5;
				if (t5 == null)
				{
					t5 = this.Item5;
					ptr5 = ref t5;
					if (t5 == null)
					{
						text5 = null;
						goto IL_0164;
					}
				}
				text5 = ptr5.ToString();
				IL_0164:
				array[num5] = text5;
				array[10] = ", ";
				int num6 = 11;
				ref T6 ptr6 = ref this.Item6;
				t6 = default(T6);
				string text6;
				if (t6 == null)
				{
					t6 = this.Item6;
					ptr6 = ref t6;
					if (t6 == null)
					{
						text6 = null;
						goto IL_01A9;
					}
				}
				text6 = ptr6.ToString();
				IL_01A9:
				array[num6] = text6;
				array[12] = ", ";
				int num7 = 13;
				ref T7 ptr7 = ref this.Item7;
				t7 = default(T7);
				string text7;
				if (t7 == null)
				{
					t7 = this.Item7;
					ptr7 = ref t7;
					if (t7 == null)
					{
						text7 = null;
						goto IL_01EE;
					}
				}
				text7 = ptr7.ToString();
				IL_01EE:
				array[num7] = text7;
				array[14] = ", ";
				array[15] = this.Rest.ToString();
				array[16] = ")";
				return string.Concat(array);
			}
			string[] array2 = new string[16];
			array2[0] = "(";
			int num8 = 1;
			ref T1 ptr8 = ref this.Item1;
			t = default(T1);
			string text8;
			if (t == null)
			{
				t = this.Item1;
				ptr8 = ref t;
				if (t == null)
				{
					text8 = null;
					goto IL_0262;
				}
			}
			text8 = ptr8.ToString();
			IL_0262:
			array2[num8] = text8;
			array2[2] = ", ";
			int num9 = 3;
			ref T2 ptr9 = ref this.Item2;
			t2 = default(T2);
			string text9;
			if (t2 == null)
			{
				t2 = this.Item2;
				ptr9 = ref t2;
				if (t2 == null)
				{
					text9 = null;
					goto IL_02A2;
				}
			}
			text9 = ptr9.ToString();
			IL_02A2:
			array2[num9] = text9;
			array2[4] = ", ";
			int num10 = 5;
			ref T3 ptr10 = ref this.Item3;
			t3 = default(T3);
			string text10;
			if (t3 == null)
			{
				t3 = this.Item3;
				ptr10 = ref t3;
				if (t3 == null)
				{
					text10 = null;
					goto IL_02E2;
				}
			}
			text10 = ptr10.ToString();
			IL_02E2:
			array2[num10] = text10;
			array2[6] = ", ";
			int num11 = 7;
			ref T4 ptr11 = ref this.Item4;
			t4 = default(T4);
			string text11;
			if (t4 == null)
			{
				t4 = this.Item4;
				ptr11 = ref t4;
				if (t4 == null)
				{
					text11 = null;
					goto IL_0325;
				}
			}
			text11 = ptr11.ToString();
			IL_0325:
			array2[num11] = text11;
			array2[8] = ", ";
			int num12 = 9;
			ref T5 ptr12 = ref this.Item5;
			t5 = default(T5);
			string text12;
			if (t5 == null)
			{
				t5 = this.Item5;
				ptr12 = ref t5;
				if (t5 == null)
				{
					text12 = null;
					goto IL_0369;
				}
			}
			text12 = ptr12.ToString();
			IL_0369:
			array2[num12] = text12;
			array2[10] = ", ";
			int num13 = 11;
			ref T6 ptr13 = ref this.Item6;
			t6 = default(T6);
			string text13;
			if (t6 == null)
			{
				t6 = this.Item6;
				ptr13 = ref t6;
				if (t6 == null)
				{
					text13 = null;
					goto IL_03AE;
				}
			}
			text13 = ptr13.ToString();
			IL_03AE:
			array2[num13] = text13;
			array2[12] = ", ";
			int num14 = 13;
			ref T7 ptr14 = ref this.Item7;
			t7 = default(T7);
			string text14;
			if (t7 == null)
			{
				t7 = this.Item7;
				ptr14 = ref t7;
				if (t7 == null)
				{
					text14 = null;
					goto IL_03F3;
				}
			}
			text14 = ptr14.ToString();
			IL_03F3:
			array2[num14] = text14;
			array2[14] = ", ";
			array2[15] = valueTupleInternal.ToStringEnd();
			return string.Concat(array2);
		}

		string IValueTupleInternal.ToStringEnd()
		{
			IValueTupleInternal valueTupleInternal = this.Rest as IValueTupleInternal;
			T1 t;
			T2 t2;
			T3 t3;
			T4 t4;
			T5 t5;
			T6 t6;
			T7 t7;
			if (valueTupleInternal == null)
			{
				string[] array = new string[16];
				int num = 0;
				ref T1 ptr = ref this.Item1;
				t = default(T1);
				string text;
				if (t == null)
				{
					t = this.Item1;
					ptr = ref t;
					if (t == null)
					{
						text = null;
						goto IL_0055;
					}
				}
				text = ptr.ToString();
				IL_0055:
				array[num] = text;
				array[1] = ", ";
				int num2 = 2;
				ref T2 ptr2 = ref this.Item2;
				t2 = default(T2);
				string text2;
				if (t2 == null)
				{
					t2 = this.Item2;
					ptr2 = ref t2;
					if (t2 == null)
					{
						text2 = null;
						goto IL_0095;
					}
				}
				text2 = ptr2.ToString();
				IL_0095:
				array[num2] = text2;
				array[3] = ", ";
				int num3 = 4;
				ref T3 ptr3 = ref this.Item3;
				t3 = default(T3);
				string text3;
				if (t3 == null)
				{
					t3 = this.Item3;
					ptr3 = ref t3;
					if (t3 == null)
					{
						text3 = null;
						goto IL_00D5;
					}
				}
				text3 = ptr3.ToString();
				IL_00D5:
				array[num3] = text3;
				array[5] = ", ";
				int num4 = 6;
				ref T4 ptr4 = ref this.Item4;
				t4 = default(T4);
				string text4;
				if (t4 == null)
				{
					t4 = this.Item4;
					ptr4 = ref t4;
					if (t4 == null)
					{
						text4 = null;
						goto IL_0118;
					}
				}
				text4 = ptr4.ToString();
				IL_0118:
				array[num4] = text4;
				array[7] = ", ";
				int num5 = 8;
				ref T5 ptr5 = ref this.Item5;
				t5 = default(T5);
				string text5;
				if (t5 == null)
				{
					t5 = this.Item5;
					ptr5 = ref t5;
					if (t5 == null)
					{
						text5 = null;
						goto IL_015B;
					}
				}
				text5 = ptr5.ToString();
				IL_015B:
				array[num5] = text5;
				array[9] = ", ";
				int num6 = 10;
				ref T6 ptr6 = ref this.Item6;
				t6 = default(T6);
				string text6;
				if (t6 == null)
				{
					t6 = this.Item6;
					ptr6 = ref t6;
					if (t6 == null)
					{
						text6 = null;
						goto IL_01A0;
					}
				}
				text6 = ptr6.ToString();
				IL_01A0:
				array[num6] = text6;
				array[11] = ", ";
				int num7 = 12;
				ref T7 ptr7 = ref this.Item7;
				t7 = default(T7);
				string text7;
				if (t7 == null)
				{
					t7 = this.Item7;
					ptr7 = ref t7;
					if (t7 == null)
					{
						text7 = null;
						goto IL_01E5;
					}
				}
				text7 = ptr7.ToString();
				IL_01E5:
				array[num7] = text7;
				array[13] = ", ";
				array[14] = this.Rest.ToString();
				array[15] = ")";
				return string.Concat(array);
			}
			string[] array2 = new string[15];
			int num8 = 0;
			ref T1 ptr8 = ref this.Item1;
			t = default(T1);
			string text8;
			if (t == null)
			{
				t = this.Item1;
				ptr8 = ref t;
				if (t == null)
				{
					text8 = null;
					goto IL_0251;
				}
			}
			text8 = ptr8.ToString();
			IL_0251:
			array2[num8] = text8;
			array2[1] = ", ";
			int num9 = 2;
			ref T2 ptr9 = ref this.Item2;
			t2 = default(T2);
			string text9;
			if (t2 == null)
			{
				t2 = this.Item2;
				ptr9 = ref t2;
				if (t2 == null)
				{
					text9 = null;
					goto IL_0291;
				}
			}
			text9 = ptr9.ToString();
			IL_0291:
			array2[num9] = text9;
			array2[3] = ", ";
			int num10 = 4;
			ref T3 ptr10 = ref this.Item3;
			t3 = default(T3);
			string text10;
			if (t3 == null)
			{
				t3 = this.Item3;
				ptr10 = ref t3;
				if (t3 == null)
				{
					text10 = null;
					goto IL_02D1;
				}
			}
			text10 = ptr10.ToString();
			IL_02D1:
			array2[num10] = text10;
			array2[5] = ", ";
			int num11 = 6;
			ref T4 ptr11 = ref this.Item4;
			t4 = default(T4);
			string text11;
			if (t4 == null)
			{
				t4 = this.Item4;
				ptr11 = ref t4;
				if (t4 == null)
				{
					text11 = null;
					goto IL_0314;
				}
			}
			text11 = ptr11.ToString();
			IL_0314:
			array2[num11] = text11;
			array2[7] = ", ";
			int num12 = 8;
			ref T5 ptr12 = ref this.Item5;
			t5 = default(T5);
			string text12;
			if (t5 == null)
			{
				t5 = this.Item5;
				ptr12 = ref t5;
				if (t5 == null)
				{
					text12 = null;
					goto IL_0357;
				}
			}
			text12 = ptr12.ToString();
			IL_0357:
			array2[num12] = text12;
			array2[9] = ", ";
			int num13 = 10;
			ref T6 ptr13 = ref this.Item6;
			t6 = default(T6);
			string text13;
			if (t6 == null)
			{
				t6 = this.Item6;
				ptr13 = ref t6;
				if (t6 == null)
				{
					text13 = null;
					goto IL_039C;
				}
			}
			text13 = ptr13.ToString();
			IL_039C:
			array2[num13] = text13;
			array2[11] = ", ";
			int num14 = 12;
			ref T7 ptr14 = ref this.Item7;
			t7 = default(T7);
			string text14;
			if (t7 == null)
			{
				t7 = this.Item7;
				ptr14 = ref t7;
				if (t7 == null)
				{
					text14 = null;
					goto IL_03E1;
				}
			}
			text14 = ptr14.ToString();
			IL_03E1:
			array2[num14] = text14;
			array2[13] = ", ";
			array2[14] = valueTupleInternal.ToStringEnd();
			return string.Concat(array2);
		}

		int ITuple.Length
		{
			get
			{
				IValueTupleInternal valueTupleInternal = this.Rest as IValueTupleInternal;
				if (valueTupleInternal != null)
				{
					return 7 + valueTupleInternal.Length;
				}
				return 8;
			}
		}

		object ITuple.this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return this.Item1;
				case 1:
					return this.Item2;
				case 2:
					return this.Item3;
				case 3:
					return this.Item4;
				case 4:
					return this.Item5;
				case 5:
					return this.Item6;
				case 6:
					return this.Item7;
				default:
				{
					IValueTupleInternal valueTupleInternal = this.Rest as IValueTupleInternal;
					if (valueTupleInternal != null)
					{
						return valueTupleInternal[index - 7];
					}
					if (index == 7)
					{
						return this.Rest;
					}
					throw new IndexOutOfRangeException();
				}
				}
			}
		}

		public T1 Item1;

		public T2 Item2;

		public T3 Item3;

		public T4 Item4;

		public T5 Item5;

		public T6 Item6;

		public T7 Item7;

		public TRest Rest;
	}
}
