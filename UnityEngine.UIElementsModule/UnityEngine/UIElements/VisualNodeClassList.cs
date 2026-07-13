using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal readonly struct VisualNodeClassList : IList<string>, ICollection<string>, IEnumerable<string>, IEnumerable
	{
		public int Count
		{
			get
			{
				return this.m_Manager.GetProperty<VisualNodeClassData>(this.m_Handle).Count;
			}
		}

		public string this[int index]
		{
			get
			{
				ref VisualNodeClassData property = ref this.m_Manager.GetProperty<VisualNodeClassData>(this.m_Handle);
				return this.m_Manager.ClassNameStore.GetClassNameManaged(property[index]);
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public VisualNodeClassList(VisualManager store, VisualNodeHandle handle)
		{
			this.m_Manager = store;
			this.m_Handle = handle;
		}

		public void Add(string className)
		{
			this.m_Manager.AddToClassList(in this.m_Handle, className);
		}

		public bool Remove(string className)
		{
			return this.m_Manager.RemoveFromClassList(in this.m_Handle, className);
		}

		public bool Contains(string className)
		{
			return this.m_Manager.ClassListContains(in this.m_Handle, className);
		}

		public void Clear()
		{
			this.m_Manager.ClearClassList(in this.m_Handle);
		}

		bool ICollection<string>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		void ICollection<string>.CopyTo(string[] array, int arrayIndex)
		{
			int i = 0;
			int num = arrayIndex;
			while (i < this.Count)
			{
				array[num] = this[i];
				i++;
				num++;
			}
		}

		public VisualNodeClassList.Enumerator GetEnumerator()
		{
			return new VisualNodeClassList.Enumerator(this.m_Manager, this.m_Manager.GetProperty<VisualNodeClassData>(this.m_Handle));
		}

		IEnumerator<string> IEnumerable<string>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		int IList<string>.IndexOf(string item)
		{
			throw new NotImplementedException();
		}

		void IList<string>.Insert(int index, string item)
		{
			throw new NotImplementedException();
		}

		void IList<string>.RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		private readonly VisualManager m_Manager;

		private readonly VisualNodeHandle m_Handle;

		public struct Enumerator : IEnumerator<string>, IEnumerator, IDisposable
		{
			internal Enumerator(VisualManager manager, in VisualNodeClassData data)
			{
				this.m_Manager = manager;
				this.m_Data = data;
				this.m_Position = -1;
			}

			public bool MoveNext()
			{
				int num = this.m_Position + 1;
				this.m_Position = num;
				return num < this.m_Data.Count;
			}

			public void Reset()
			{
				this.m_Position = -1;
			}

			public string Current
			{
				get
				{
					return this.m_Manager.ClassNameStore.GetClassNameManaged(this.m_Data[this.m_Position]);
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public void Dispose()
			{
			}

			private readonly VisualManager m_Manager;

			private readonly VisualNodeClassData m_Data;

			private int m_Position;
		}
	}
}
