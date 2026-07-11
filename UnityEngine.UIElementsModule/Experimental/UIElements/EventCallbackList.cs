using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	internal class EventCallbackList
	{
		public EventCallbackList()
		{
			this.m_List = new List<EventCallbackFunctorBase>();
			this.trickleDownCallbackCount = 0;
			this.bubbleUpCallbackCount = 0;
		}

		public EventCallbackList(EventCallbackList source)
		{
			this.m_List = new List<EventCallbackFunctorBase>(source.m_List);
			this.trickleDownCallbackCount = 0;
			this.bubbleUpCallbackCount = 0;
		}

		public int trickleDownCallbackCount { get; private set; }

		public int bubbleUpCallbackCount { get; private set; }

		[Obsolete("Use trickleDownCallbackCount instead of capturingCallbackCount.")]
		public int capturingCallbackCount
		{
			get
			{
				return this.trickleDownCallbackCount;
			}
		}

		[Obsolete("Use bubbleUpCallbackCount instead of bubblingCallbackCount.")]
		public int bubblingCallbackCount
		{
			get
			{
				return this.bubbleUpCallbackCount;
			}
		}

		public bool Contains(long eventTypeId, Delegate callback, CallbackPhase phase)
		{
			return this.Find(eventTypeId, callback, phase) != null;
		}

		public EventCallbackFunctorBase Find(long eventTypeId, Delegate callback, CallbackPhase phase)
		{
			for (int i = 0; i < this.m_List.Count; i++)
			{
				if (this.m_List[i].IsEquivalentTo(eventTypeId, callback, phase))
				{
					return this.m_List[i];
				}
			}
			return null;
		}

		public bool Remove(long eventTypeId, Delegate callback, CallbackPhase phase)
		{
			for (int i = 0; i < this.m_List.Count; i++)
			{
				if (this.m_List[i].IsEquivalentTo(eventTypeId, callback, phase))
				{
					this.m_List.RemoveAt(i);
					if (phase == CallbackPhase.TrickleDownAndTarget)
					{
						this.trickleDownCallbackCount--;
					}
					else if (phase == CallbackPhase.TargetAndBubbleUp)
					{
						this.bubbleUpCallbackCount--;
					}
					return true;
				}
			}
			return false;
		}

		public void Add(EventCallbackFunctorBase item)
		{
			this.m_List.Add(item);
			if (item.phase == CallbackPhase.TrickleDownAndTarget)
			{
				this.trickleDownCallbackCount++;
			}
			else if (item.phase == CallbackPhase.TargetAndBubbleUp)
			{
				this.bubbleUpCallbackCount++;
			}
		}

		public void AddRange(EventCallbackList list)
		{
			this.m_List.AddRange(list.m_List);
			foreach (EventCallbackFunctorBase eventCallbackFunctorBase in list.m_List)
			{
				if (eventCallbackFunctorBase.phase == CallbackPhase.TrickleDownAndTarget)
				{
					this.trickleDownCallbackCount++;
				}
				else if (eventCallbackFunctorBase.phase == CallbackPhase.TargetAndBubbleUp)
				{
					this.bubbleUpCallbackCount++;
				}
			}
		}

		public int Count
		{
			get
			{
				return this.m_List.Count;
			}
		}

		public EventCallbackFunctorBase this[int i]
		{
			get
			{
				return this.m_List[i];
			}
			set
			{
				this.m_List[i] = value;
			}
		}

		public void Clear()
		{
			this.m_List.Clear();
			this.trickleDownCallbackCount = 0;
			this.bubbleUpCallbackCount = 0;
		}

		private List<EventCallbackFunctorBase> m_List;
	}
}
