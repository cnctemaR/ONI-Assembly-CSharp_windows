using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements
{
	internal readonly struct VisualNodeChildren : IEnumerable<VisualNode>, IEnumerable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe VisualNodeChildrenData* GetDataPtr()
		{
			return (VisualNodeChildrenData*)this.m_Manager.GetChildrenPtr(in this.m_Handle).ToPointer();
		}

		public int Count
		{
			get
			{
				return this.m_Manager.GetChildrenCount(in this.m_Handle);
			}
		}

		public unsafe VisualNode this[int index]
		{
			get
			{
				VisualNodeChildrenData* dataPtr = this.GetDataPtr();
				bool flag = (ulong)index >= (ulong)((long)dataPtr->Count);
				if (flag)
				{
					throw new IndexOutOfRangeException();
				}
				return new VisualNode(this.m_Manager, dataPtr->ElementAt(index));
			}
		}

		public VisualNodeChildren(VisualManager manager, VisualNodeHandle handle)
		{
			this.m_Manager = manager;
			this.m_Handle = handle;
		}

		public void Add(in VisualNode child)
		{
			VisualManager manager = this.m_Manager;
			VisualNodeHandle handle = child.Handle;
			manager.AddChild(in this.m_Handle, in handle);
		}

		public bool Remove(in VisualNode child)
		{
			VisualManager manager = this.m_Manager;
			VisualNodeHandle handle = child.Handle;
			return manager.RemoveChild(in this.m_Handle, in handle);
		}

		public unsafe VisualNodeChildren.Enumerator GetEnumerator()
		{
			return new VisualNodeChildren.Enumerator(this.m_Manager, in *this.GetDataPtr());
		}

		IEnumerator<VisualNode> IEnumerable<VisualNode>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private readonly VisualManager m_Manager;

		private readonly VisualNodeHandle m_Handle;

		public struct Enumerator : IEnumerator<VisualNode>, IEnumerator, IDisposable
		{
			internal Enumerator(VisualManager manager, in VisualNodeChildrenData children)
			{
				this.m_Manager = manager;
				this.m_Children = children;
				this.m_Position = -1;
			}

			public bool MoveNext()
			{
				int num = this.m_Position + 1;
				this.m_Position = num;
				return num < this.m_Children.Count;
			}

			public void Reset()
			{
				this.m_Position = -1;
			}

			public VisualNode Current
			{
				get
				{
					return new VisualNode(this.m_Manager, this.m_Children[this.m_Position]);
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

			private readonly VisualNodeChildrenData m_Children;

			private int m_Position;
		}
	}
}
