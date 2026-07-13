using System;
using JetBrains.Annotations;

namespace UnityEngine.UIElements
{
	internal class EventCallbackList
	{
		public EventCallbackList()
		{
			this.m_Array = EventCallbackList.EmptyArray;
		}

		public EventCallbackList(EventCallbackList source)
		{
			this.m_Count = source.m_Count;
			this.m_Array = new EventCallbackFunctorBase[this.m_Count];
			Array.Copy(source.m_Array, this.m_Array, this.m_Count);
		}

		public bool Contains(long eventTypeId, [NotNull] Delegate callback)
		{
			return this.Find(eventTypeId, callback) != null;
		}

		public EventCallbackFunctorBase Find(long eventTypeId, [NotNull] Delegate callback)
		{
			for (int i = 0; i < this.m_Count; i++)
			{
				bool flag = this.m_Array[i].IsEquivalentTo(eventTypeId, callback);
				if (flag)
				{
					return this.m_Array[i];
				}
			}
			return null;
		}

		public bool Remove(long eventTypeId, [NotNull] Delegate callback, out EventCallbackFunctorBase removedFunctor)
		{
			for (int i = 0; i < this.m_Count; i++)
			{
				bool flag = this.m_Array[i].IsEquivalentTo(eventTypeId, callback);
				if (flag)
				{
					removedFunctor = this.m_Array[i];
					this.m_Count--;
					Array.Copy(this.m_Array, i + 1, this.m_Array, i, this.m_Count - i);
					this.m_Array[this.m_Count] = null;
					return true;
				}
			}
			removedFunctor = null;
			return false;
		}

		public void Add(EventCallbackFunctorBase item)
		{
			bool flag = this.m_Count >= this.m_Array.Length;
			if (flag)
			{
				Array.Resize<EventCallbackFunctorBase>(ref this.m_Array, Mathf.NextPowerOfTwo(this.m_Count + 4));
			}
			EventCallbackFunctorBase[] array = this.m_Array;
			int count = this.m_Count;
			this.m_Count = count + 1;
			array[count] = item;
		}

		public void AddRange(EventCallbackList list)
		{
			bool flag = this.m_Count + list.m_Count > this.m_Array.Length;
			if (flag)
			{
				Array.Resize<EventCallbackFunctorBase>(ref this.m_Array, Mathf.NextPowerOfTwo(this.m_Count + list.m_Count));
			}
			Array.Copy(list.m_Array, 0, this.m_Array, this.m_Count, list.m_Count);
			this.m_Count += list.m_Count;
		}

		public int Count
		{
			get
			{
				return this.m_Count;
			}
		}

		public Span<EventCallbackFunctorBase> Span
		{
			get
			{
				return new Span<EventCallbackFunctorBase>(this.m_Array, 0, this.m_Count);
			}
		}

		public EventCallbackFunctorBase this[int i]
		{
			get
			{
				return this.m_Array[i];
			}
			set
			{
				this.m_Array[i] = value;
			}
		}

		public void Clear()
		{
			Array.Clear(this.m_Array, 0, this.m_Count);
			this.m_Count = 0;
		}

		public static readonly EventCallbackList EmptyList = new EventCallbackList();

		private static readonly EventCallbackFunctorBase[] EmptyArray = new EventCallbackFunctorBase[0];

		private EventCallbackFunctorBase[] m_Array;

		private int m_Count;
	}
}
