using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Iced.Intel
{
	[NullableContext(1)]
	[Nullable(0)]
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(InstructionListDebugView))]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class InstructionList : IList<Instruction>, ICollection<Instruction>, IEnumerable<Instruction>, IEnumerable, IReadOnlyList<Instruction>, IReadOnlyCollection<Instruction>, IList, ICollection
	{
		public int Count
		{
			get
			{
				return this.count;
			}
		}

		int ICollection<Instruction>.Count
		{
			get
			{
				return this.count;
			}
		}

		int ICollection.Count
		{
			get
			{
				return this.count;
			}
		}

		int IReadOnlyCollection<Instruction>.Count
		{
			get
			{
				return this.count;
			}
		}

		public int Capacity
		{
			get
			{
				return this.elements.Length;
			}
		}

		bool ICollection<Instruction>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		public ref Instruction this[int index]
		{
			get
			{
				return ref this.elements[index];
			}
		}

		Instruction IList<Instruction>.this[int index]
		{
			get
			{
				return this.elements[index];
			}
			set
			{
				this.elements[index] = value;
			}
		}

		Instruction IReadOnlyList<Instruction>.this[int index]
		{
			get
			{
				return this.elements[index];
			}
		}

		[Nullable(2)]
		object IList.this[int index]
		{
			get
			{
				return this.elements[index];
			}
			set
			{
				if (value == null)
				{
					ThrowHelper.ThrowArgumentNullException_value();
				}
				if (!(value is Instruction))
				{
					ThrowHelper.ThrowArgumentException();
				}
				this.elements[index] = (Instruction)value;
			}
		}

		public InstructionList()
		{
			this.elements = Array2.Empty<Instruction>();
		}

		public InstructionList(int capacity)
		{
			if (capacity < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_capacity();
			}
			this.elements = ((capacity == 0) ? Array2.Empty<Instruction>() : new Instruction[capacity]);
		}

		public InstructionList(InstructionList list)
		{
			if (list == null)
			{
				ThrowHelper.ThrowArgumentNullException_list();
			}
			int num = list.count;
			if (num == 0)
			{
				this.elements = Array2.Empty<Instruction>();
				return;
			}
			Instruction[] array = new Instruction[num];
			this.elements = array;
			this.count = num;
			Array.Copy(list.elements, 0, array, 0, num);
		}

		public InstructionList(IEnumerable<Instruction> collection)
		{
			if (collection == null)
			{
				ThrowHelper.ThrowArgumentNullException_collection();
			}
			ICollection<Instruction> collection2 = collection as ICollection<Instruction>;
			if (collection2 == null)
			{
				this.elements = Array2.Empty<Instruction>();
				foreach (Instruction instruction in collection)
				{
					this.Add(in instruction);
				}
				return;
			}
			int num = collection2.Count;
			if (num == 0)
			{
				this.elements = Array2.Empty<Instruction>();
				return;
			}
			Instruction[] array = new Instruction[num];
			this.elements = array;
			collection2.CopyTo(array, 0);
			this.count = num;
		}

		private void SetMinCapacity(int minCapacity)
		{
			Instruction[] array = this.elements;
			uint num = (uint)array.Length;
			if (minCapacity <= (int)num)
			{
				return;
			}
			uint num2 = num * 2U;
			if (num2 < 4U)
			{
				num2 = 4U;
			}
			if (num2 < (uint)minCapacity)
			{
				num2 = (uint)minCapacity;
			}
			if (num2 > 2146435071U)
			{
				num2 = 2146435071U;
			}
			Instruction[] array2 = new Instruction[num2];
			Array.Copy(array, 0, array2, 0, this.count);
			this.elements = array2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref Instruction AllocUninitializedElement()
		{
			int num = this.count;
			Instruction[] array = this.elements;
			if (num == array.Length)
			{
				this.SetMinCapacity(num + 1);
				array = this.elements;
			}
			this.count = num + 1;
			return ref array[num];
		}

		private void MakeRoom(int index, int extraLength)
		{
			this.SetMinCapacity(this.count + extraLength);
			int num = this.count - index;
			if (num != 0)
			{
				Instruction[] array = this.elements;
				Array.Copy(array, index, array, index + extraLength, num);
			}
		}

		public void Insert(int index, in Instruction instruction)
		{
			int num = this.count;
			if (index > num)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
			}
			this.MakeRoom(index, 1);
			this.elements[index] = instruction;
			this.count = num + 1;
		}

		void IList<Instruction>.Insert(int index, Instruction instruction)
		{
			this.Insert(index, in instruction);
		}

		void IList.Insert(int index, object value)
		{
			if (value == null)
			{
				ThrowHelper.ThrowArgumentNullException_value();
			}
			if (!(value is Instruction))
			{
				ThrowHelper.ThrowArgumentException();
			}
			Instruction instruction = (Instruction)value;
			this.Insert(index, in instruction);
		}

		public void RemoveAt(int index)
		{
			int num = this.count;
			if (index >= num)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
			}
			num--;
			this.count = num;
			int num2 = num - index;
			if (num2 != 0)
			{
				Instruction[] array = this.elements;
				Array.Copy(array, index + 1, array, index, num2);
			}
		}

		void IList<Instruction>.RemoveAt(int index)
		{
			this.RemoveAt(index);
		}

		void IList.RemoveAt(int index)
		{
			this.RemoveAt(index);
		}

		public void AddRange(IEnumerable<Instruction> collection)
		{
			this.InsertRange(this.count, collection);
		}

		public void InsertRange(int index, IEnumerable<Instruction> collection)
		{
			if (index > this.count)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
			}
			if (collection == null)
			{
				ThrowHelper.ThrowArgumentNullException_collection();
			}
			InstructionList instructionList = collection as InstructionList;
			if (instructionList != null)
			{
				int num = instructionList.count;
				if (num != 0)
				{
					this.MakeRoom(index, num);
					this.count += num;
					Array.Copy(instructionList.elements, 0, this.elements, index, num);
					return;
				}
			}
			else
			{
				IList<Instruction> list = collection as IList<Instruction>;
				if (list != null)
				{
					int num2 = list.Count;
					if (num2 != 0)
					{
						this.MakeRoom(index, num2);
						this.count += num2;
						Instruction[] array = this.elements;
						for (int i = 0; i < num2; i++)
						{
							array[index + i] = list[i];
						}
						return;
					}
				}
				else
				{
					IReadOnlyList<Instruction> readOnlyList = collection as IReadOnlyList<Instruction>;
					if (readOnlyList != null)
					{
						int num3 = readOnlyList.Count;
						if (num3 != 0)
						{
							this.MakeRoom(index, num3);
							this.count += num3;
							Instruction[] array2 = this.elements;
							for (int j = 0; j < num3; j++)
							{
								array2[index + j] = readOnlyList[j];
							}
							return;
						}
					}
					else
					{
						foreach (Instruction instruction in collection)
						{
							this.Insert(index++, in instruction);
						}
					}
				}
			}
		}

		public void RemoveRange(int index, int count)
		{
			if (index < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
			}
			if (count < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_count();
			}
			if (index + count > this.count)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_count();
			}
			int num = this.count;
			num -= count;
			this.count = num;
			int num2 = num - index;
			if (num2 != 0)
			{
				Instruction[] array = this.elements;
				Array.Copy(array, index + count, array, index, num2);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(in Instruction instruction)
		{
			int num = this.count;
			Instruction[] array = this.elements;
			if (num == array.Length)
			{
				this.SetMinCapacity(num + 1);
				array = this.elements;
			}
			array[num] = instruction;
			this.count = num + 1;
		}

		void ICollection<Instruction>.Add(Instruction instruction)
		{
			this.Add(in instruction);
		}

		int IList.Add(object value)
		{
			if (value == null)
			{
				ThrowHelper.ThrowArgumentNullException_value();
			}
			if (!(value is Instruction))
			{
				ThrowHelper.ThrowArgumentException();
			}
			Instruction instruction = (Instruction)value;
			this.Add(in instruction);
			return this.count - 1;
		}

		public void Clear()
		{
			this.count = 0;
		}

		void ICollection<Instruction>.Clear()
		{
			this.Clear();
		}

		void IList.Clear()
		{
			this.Clear();
		}

		public bool Contains(in Instruction instruction)
		{
			return this.IndexOf(in instruction) >= 0;
		}

		bool ICollection<Instruction>.Contains(Instruction instruction)
		{
			return this.Contains(in instruction);
		}

		bool IList.Contains(object value)
		{
			if (value is Instruction)
			{
				Instruction instruction = (Instruction)value;
				return this.Contains(in instruction);
			}
			return false;
		}

		public int IndexOf(in Instruction instruction)
		{
			Instruction[] array = this.elements;
			int num = this.count;
			for (int i = 0; i < num; i++)
			{
				if ((in array[i]) == (in instruction))
				{
					return i;
				}
			}
			return -1;
		}

		int IList<Instruction>.IndexOf(Instruction instruction)
		{
			return this.IndexOf(in instruction);
		}

		int IList.IndexOf(object value)
		{
			if (value is Instruction)
			{
				Instruction instruction = (Instruction)value;
				return this.IndexOf(in instruction);
			}
			return -1;
		}

		public int IndexOf(in Instruction instruction, int index)
		{
			int num = this.count;
			if (index > num)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
			}
			Instruction[] array = this.elements;
			for (int i = index; i < num; i++)
			{
				if ((in array[i]) == (in instruction))
				{
					return i;
				}
			}
			return -1;
		}

		public int IndexOf(in Instruction instruction, int index, int count)
		{
			if (index < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
			}
			if (count < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_count();
			}
			int num = index + count;
			if (num > this.count)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_count();
			}
			Instruction[] array = this.elements;
			for (int i = index; i < num; i++)
			{
				if ((in array[i]) == (in instruction))
				{
					return i;
				}
			}
			return -1;
		}

		public int LastIndexOf(in Instruction instruction)
		{
			for (int i = this.count - 1; i >= 0; i--)
			{
				if ((in this.elements[i]) == (in instruction))
				{
					return i;
				}
			}
			return -1;
		}

		public int LastIndexOf(in Instruction instruction, int index)
		{
			int num = this.count;
			if (index > num)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
			}
			Instruction[] array = this.elements;
			for (int i = num - 1; i >= index; i--)
			{
				if ((in array[i]) == (in instruction))
				{
					return i;
				}
			}
			return -1;
		}

		public int LastIndexOf(in Instruction instruction, int index, int count)
		{
			if (index < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
			}
			if (count < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_count();
			}
			int num = index + count;
			if (num > this.count)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_count();
			}
			Instruction[] array = this.elements;
			for (int i = num - 1; i >= index; i--)
			{
				if ((in array[i]) == (in instruction))
				{
					return i;
				}
			}
			return -1;
		}

		public bool Remove(in Instruction instruction)
		{
			int num = this.IndexOf(in instruction);
			if (num >= 0)
			{
				this.RemoveAt(num);
			}
			return num >= 0;
		}

		bool ICollection<Instruction>.Remove(Instruction instruction)
		{
			return this.Remove(in instruction);
		}

		void IList.Remove(object value)
		{
			if (value is Instruction)
			{
				Instruction instruction = (Instruction)value;
				this.Remove(in instruction);
			}
		}

		public void CopyTo(Instruction[] array)
		{
			this.CopyTo(array, 0);
		}

		public void CopyTo(Instruction[] array, int arrayIndex)
		{
			Array.Copy(this.elements, 0, array, arrayIndex, this.count);
		}

		void ICollection<Instruction>.CopyTo(Instruction[] array, int arrayIndex)
		{
			this.CopyTo(array, arrayIndex);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
			{
				ThrowHelper.ThrowArgumentNullException_array();
				return;
			}
			Instruction[] array2 = array as Instruction[];
			if (array2 != null)
			{
				this.CopyTo(array2, index);
				return;
			}
			ThrowHelper.ThrowArgumentException();
		}

		public void CopyTo(int index, Instruction[] array, int arrayIndex, int count)
		{
			Array.Copy(this.elements, index, array, arrayIndex, count);
		}

		public InstructionList GetRange(int index, int count)
		{
			if (index < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
			}
			if (count < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_count();
			}
			if (index + count > this.count)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_count();
			}
			InstructionList instructionList = new InstructionList(count);
			Array.Copy(this.elements, index, instructionList.elements, 0, count);
			instructionList.count = count;
			return instructionList;
		}

		public InstructionList.Enumerator GetEnumerator()
		{
			return new InstructionList.Enumerator(this);
		}

		IEnumerator<Instruction> IEnumerable<Instruction>.GetEnumerator()
		{
			return new InstructionList.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new InstructionList.Enumerator(this);
		}

		public ReadOnlyCollection<Instruction> AsReadOnly()
		{
			return new ReadOnlyCollection<Instruction>(this);
		}

		public Instruction[] ToArray()
		{
			int num = this.count;
			if (num == 0)
			{
				return Array2.Empty<Instruction>();
			}
			Instruction[] array = new Instruction[num];
			Array.Copy(this.elements, 0, array, 0, array.Length);
			return array;
		}

		private Instruction[] elements;

		private int count;

		[NullableContext(0)]
		public struct Enumerator : IEnumerator<Instruction>, IDisposable, IEnumerator
		{
			public ref Instruction Current
			{
				get
				{
					return ref this.list.elements[this.index];
				}
			}

			Instruction IEnumerator<Instruction>.Current
			{
				get
				{
					return this.list.elements[this.index];
				}
			}

			[Nullable(1)]
			object IEnumerator.Current
			{
				get
				{
					return this.list.elements[this.index];
				}
			}

			[NullableContext(1)]
			internal Enumerator(InstructionList list)
			{
				this.list = list;
				this.index = -1;
			}

			public bool MoveNext()
			{
				this.index++;
				return this.index < this.list.count;
			}

			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			public void Dispose()
			{
			}

			private readonly InstructionList list;

			private int index;
		}
	}
}
