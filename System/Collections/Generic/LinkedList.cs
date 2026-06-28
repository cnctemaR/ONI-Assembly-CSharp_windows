using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Collections.Generic
{
	[ComVisible(false)]
	[Serializable]
	public class LinkedList<T> : IEnumerable<T>, ICollection, IEnumerable, IDeserializationCallback, ICollection<T>, ISerializable
	{
		public LinkedList()
		{
			this.syncRoot = new object();
			this.first = null;
			this.count = (this.version = 0U);
		}

		public LinkedList(IEnumerable<T> collection)
			: this()
		{
			foreach (T t in collection)
			{
				this.AddLast(t);
			}
		}

		protected LinkedList(SerializationInfo info, StreamingContext context)
			: this()
		{
			this.si = info;
			this.syncRoot = new object();
		}

		void ICollection<T>.Add(T value)
		{
			this.AddLast(value);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			T[] array2 = array as T[];
			if (array2 == null)
			{
				throw new ArgumentException("array");
			}
			this.CopyTo(array2, index);
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		bool ICollection<T>.IsReadOnly
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
				return this.syncRoot;
			}
		}

		private void VerifyReferencedNode(LinkedListNode<T> node)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (node.List != this)
			{
				throw new InvalidOperationException();
			}
		}

		private static void VerifyBlankNode(LinkedListNode<T> newNode)
		{
			if (newNode == null)
			{
				throw new ArgumentNullException("newNode");
			}
			if (newNode.List != null)
			{
				throw new InvalidOperationException();
			}
		}

		public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value)
		{
			this.VerifyReferencedNode(node);
			LinkedListNode<T> linkedListNode = new LinkedListNode<T>(this, value, node, node.forward);
			this.count += 1U;
			this.version += 1U;
			return linkedListNode;
		}

		public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode)
		{
			this.VerifyReferencedNode(node);
			LinkedList<T>.VerifyBlankNode(newNode);
			newNode.InsertBetween(node, node.forward, this);
			this.count += 1U;
			this.version += 1U;
		}

		public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value)
		{
			this.VerifyReferencedNode(node);
			LinkedListNode<T> linkedListNode = new LinkedListNode<T>(this, value, node.back, node);
			this.count += 1U;
			this.version += 1U;
			if (node == this.first)
			{
				this.first = linkedListNode;
			}
			return linkedListNode;
		}

		public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
		{
			this.VerifyReferencedNode(node);
			LinkedList<T>.VerifyBlankNode(newNode);
			newNode.InsertBetween(node.back, node, this);
			this.count += 1U;
			this.version += 1U;
			if (node == this.first)
			{
				this.first = newNode;
			}
		}

		public void AddFirst(LinkedListNode<T> node)
		{
			LinkedList<T>.VerifyBlankNode(node);
			if (this.first == null)
			{
				node.SelfReference(this);
			}
			else
			{
				node.InsertBetween(this.first.back, this.first, this);
			}
			this.count += 1U;
			this.version += 1U;
			this.first = node;
		}

		public LinkedListNode<T> AddFirst(T value)
		{
			LinkedListNode<T> linkedListNode;
			if (this.first == null)
			{
				linkedListNode = new LinkedListNode<T>(this, value);
			}
			else
			{
				linkedListNode = new LinkedListNode<T>(this, value, this.first.back, this.first);
			}
			this.count += 1U;
			this.version += 1U;
			this.first = linkedListNode;
			return linkedListNode;
		}

		public LinkedListNode<T> AddLast(T value)
		{
			LinkedListNode<T> linkedListNode;
			if (this.first == null)
			{
				linkedListNode = new LinkedListNode<T>(this, value);
				this.first = linkedListNode;
			}
			else
			{
				linkedListNode = new LinkedListNode<T>(this, value, this.first.back, this.first);
			}
			this.count += 1U;
			this.version += 1U;
			return linkedListNode;
		}

		public void AddLast(LinkedListNode<T> node)
		{
			LinkedList<T>.VerifyBlankNode(node);
			if (this.first == null)
			{
				node.SelfReference(this);
				this.first = node;
			}
			else
			{
				node.InsertBetween(this.first.back, this.first, this);
			}
			this.count += 1U;
			this.version += 1U;
		}

		public void Clear()
		{
			while (this.first != null)
			{
				this.RemoveLast();
			}
		}

		public bool Contains(T value)
		{
			LinkedListNode<T> forward = this.first;
			if (forward == null)
			{
				return false;
			}
			while (!value.Equals(forward.Value))
			{
				forward = forward.forward;
				if (forward == this.first)
				{
					return false;
				}
			}
			return true;
		}

		public void CopyTo(T[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < array.GetLowerBound(0))
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException("array", "Array is multidimensional");
			}
			if ((long)(array.Length - index + array.GetLowerBound(0)) < (long)((ulong)this.count))
			{
				throw new ArgumentException("number of items exceeds capacity");
			}
			LinkedListNode<T> forward = this.first;
			if (this.first == null)
			{
				return;
			}
			do
			{
				array[index] = forward.Value;
				index++;
				forward = forward.forward;
			}
			while (forward != this.first);
		}

		public LinkedListNode<T> Find(T value)
		{
			LinkedListNode<T> forward = this.first;
			if (forward == null)
			{
				return null;
			}
			while ((value != null || forward.Value != null) && (value == null || !value.Equals(forward.Value)))
			{
				forward = forward.forward;
				if (forward == this.first)
				{
					return null;
				}
			}
			return forward;
		}

		public LinkedListNode<T> FindLast(T value)
		{
			LinkedListNode<T> back = this.first;
			if (back == null)
			{
				return null;
			}
			for (;;)
			{
				back = back.back;
				if (value.Equals(back.Value))
				{
					break;
				}
				if (back == this.first)
				{
					goto Block_3;
				}
			}
			return back;
			Block_3:
			return null;
		}

		public LinkedList<T>.Enumerator GetEnumerator()
		{
			return new LinkedList<T>.Enumerator(this);
		}

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			T[] array = new T[this.count];
			this.CopyTo(array, 0);
			info.AddValue("DataArray", array, typeof(T[]));
			info.AddValue("version", this.version);
		}

		public virtual void OnDeserialization(object sender)
		{
			if (this.si != null)
			{
				T[] array = (T[])this.si.GetValue("DataArray", typeof(T[]));
				if (array != null)
				{
					foreach (T t in array)
					{
						this.AddLast(t);
					}
				}
				this.version = this.si.GetUInt32("version");
				this.si = null;
			}
		}

		public bool Remove(T value)
		{
			LinkedListNode<T> linkedListNode = this.Find(value);
			if (linkedListNode == null)
			{
				return false;
			}
			this.Remove(linkedListNode);
			return true;
		}

		public void Remove(LinkedListNode<T> node)
		{
			this.VerifyReferencedNode(node);
			this.count -= 1U;
			if (this.count == 0U)
			{
				this.first = null;
			}
			if (node == this.first)
			{
				this.first = this.first.forward;
			}
			this.version += 1U;
			node.Detach();
		}

		public void RemoveFirst()
		{
			if (this.first != null)
			{
				this.Remove(this.first);
			}
		}

		public void RemoveLast()
		{
			if (this.first != null)
			{
				this.Remove(this.first.back);
			}
		}

		public int Count
		{
			get
			{
				return (int)this.count;
			}
		}

		public LinkedListNode<T> First
		{
			get
			{
				return this.first;
			}
		}

		public LinkedListNode<T> Last
		{
			get
			{
				return (this.first == null) ? null : this.first.back;
			}
		}

		private const string DataArrayKey = "DataArray";

		private const string VersionKey = "version";

		private uint count;

		private uint version;

		private object syncRoot;

		internal LinkedListNode<T> first;

		internal SerializationInfo si;

		[Serializable]
		public struct Enumerator : IEnumerator, IDisposable, IEnumerator<T>, IDeserializationCallback, ISerializable
		{
			internal Enumerator(SerializationInfo info, StreamingContext context)
			{
				this.si = info;
				this.list = (LinkedList<T>)this.si.GetValue("list", typeof(LinkedList<T>));
				this.index = this.si.GetInt32("index");
				this.version = this.si.GetUInt32("version");
				this.current = null;
			}

			internal Enumerator(LinkedList<T> parent)
			{
				this.si = null;
				this.list = parent;
				this.current = null;
				this.index = -1;
				this.version = parent.version;
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			void IEnumerator.Reset()
			{
				if (this.list == null)
				{
					throw new ObjectDisposedException(null);
				}
				if (this.version != this.list.version)
				{
					throw new InvalidOperationException("list modified");
				}
				this.current = null;
				this.index = -1;
			}

			[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
			void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
			{
				if (this.list == null)
				{
					throw new ObjectDisposedException(null);
				}
				info.AddValue("version", this.version);
				info.AddValue("index", this.index);
			}

			void IDeserializationCallback.OnDeserialization(object sender)
			{
				if (this.si == null)
				{
					return;
				}
				if (this.list.si != null)
				{
					((IDeserializationCallback)this.list).OnDeserialization(this);
				}
				this.si = null;
				if (this.version == this.list.version && this.index != -1)
				{
					LinkedListNode<T> linkedListNode = this.list.First;
					for (int i = 0; i < this.index; i++)
					{
						linkedListNode = linkedListNode.forward;
					}
					this.current = linkedListNode;
				}
			}

			public T Current
			{
				get
				{
					if (this.list == null)
					{
						throw new ObjectDisposedException(null);
					}
					if (this.current == null)
					{
						throw new InvalidOperationException();
					}
					return this.current.Value;
				}
			}

			public bool MoveNext()
			{
				if (this.list == null)
				{
					throw new ObjectDisposedException(null);
				}
				if (this.version != this.list.version)
				{
					throw new InvalidOperationException("list modified");
				}
				if (this.current == null)
				{
					this.current = this.list.first;
				}
				else
				{
					this.current = this.current.forward;
					if (this.current == this.list.first)
					{
						this.current = null;
					}
				}
				if (this.current == null)
				{
					this.index = -1;
					return false;
				}
				this.index++;
				return true;
			}

			public void Dispose()
			{
				if (this.list == null)
				{
					throw new ObjectDisposedException(null);
				}
				this.current = null;
				this.list = null;
			}

			private const string VersionKey = "version";

			private const string IndexKey = "index";

			private const string ListKey = "list";

			private LinkedList<T> list;

			private LinkedListNode<T> current;

			private int index;

			private uint version;

			private SerializationInfo si;
		}
	}
}
