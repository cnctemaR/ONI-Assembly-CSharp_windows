using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	internal class EventCallbackRegistry
	{
		private static EventCallbackList GetCallbackList(EventCallbackList initializer = null)
		{
			return EventCallbackRegistry.s_ListPool.Get(initializer);
		}

		private static void ReleaseCallbackList(EventCallbackList toRelease)
		{
			EventCallbackRegistry.s_ListPool.Release(toRelease);
		}

		private ref EventCallbackRegistry.DynamicCallbackList GetDynamicCallbackList(TrickleDown useTrickleDown)
		{
			return ref useTrickleDown == TrickleDown.TrickleDown ? ref this.m_TrickleDownCallbacks : ref this.m_BubbleUpCallbacks;
		}

		public void RegisterCallback<TEventType>([NotNull] EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown, InvokePolicy invokePolicy = InvokePolicy.Default) where TEventType : EventBase<TEventType>, new()
		{
			long num = EventBase<TEventType>.TypeId();
			ref EventCallbackRegistry.DynamicCallbackList dynamicCallbackList = ref this.GetDynamicCallbackList(useTrickleDown);
			EventCallbackList eventCallbackList = dynamicCallbackList.GetCallbackListForReading();
			EventCallbackFunctor<TEventType> eventCallbackFunctor = eventCallbackList.Find(num, callback) as EventCallbackFunctor<TEventType>;
			bool flag = eventCallbackFunctor != null;
			if (flag)
			{
				eventCallbackFunctor.invokePolicy = invokePolicy;
			}
			else
			{
				eventCallbackList = dynamicCallbackList.GetCallbackListForWriting();
				eventCallbackList.Add(EventCallbackFunctor<TEventType>.GetPooled(num, callback, invokePolicy));
			}
		}

		public void RegisterCallback<TEventType, TCallbackArgs>([NotNull] EventCallback<TEventType, TCallbackArgs> callback, TCallbackArgs userArgs, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown, InvokePolicy invokePolicy = InvokePolicy.Default) where TEventType : EventBase<TEventType>, new()
		{
			long num = EventBase<TEventType>.TypeId();
			ref EventCallbackRegistry.DynamicCallbackList dynamicCallbackList = ref this.GetDynamicCallbackList(useTrickleDown);
			EventCallbackList eventCallbackList = dynamicCallbackList.GetCallbackListForReading();
			EventCallbackFunctor<TEventType, TCallbackArgs> eventCallbackFunctor = eventCallbackList.Find(num, callback) as EventCallbackFunctor<TEventType, TCallbackArgs>;
			bool flag = eventCallbackFunctor != null;
			if (flag)
			{
				eventCallbackFunctor.invokePolicy = invokePolicy;
				eventCallbackFunctor.userArgs = userArgs;
			}
			else
			{
				eventCallbackList = dynamicCallbackList.GetCallbackListForWriting();
				eventCallbackList.Add(EventCallbackFunctor<TEventType, TCallbackArgs>.GetPooled(num, callback, userArgs, invokePolicy));
			}
		}

		public bool UnregisterCallback<TEventType>([NotNull] EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			return this.GetDynamicCallbackList(useTrickleDown).UnregisterCallback(EventBase<TEventType>.TypeId(), callback);
		}

		public bool UnregisterCallback<TEventType, TCallbackArgs>([NotNull] EventCallback<TEventType, TCallbackArgs> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
		{
			return this.GetDynamicCallbackList(useTrickleDown).UnregisterCallback(EventBase<TEventType>.TypeId(), callback);
		}

		internal void InvokeCallbacks(EventBase evt, PropagationPhase propagationPhase)
		{
			VisualElement visualElement = (VisualElement)evt.currentTarget;
			BaseVisualElementPanel elementPanel = visualElement.elementPanel;
			if (propagationPhase != PropagationPhase.TrickleDown)
			{
				if (propagationPhase != PropagationPhase.BubbleUp)
				{
					throw new ArgumentOutOfRangeException("propagationPhase", "Propagation phases other than TrickleDown and BubbleUp are not supported");
				}
				this.GetDynamicCallbackList(TrickleDown.NoTrickleDown).Invoke(evt, elementPanel, visualElement);
			}
			else
			{
				this.GetDynamicCallbackList(TrickleDown.TrickleDown).Invoke(evt, elementPanel, visualElement);
			}
		}

		public bool HasTrickleDownHandlers()
		{
			return this.m_TrickleDownCallbacks.Count > 0;
		}

		public bool HasBubbleHandlers()
		{
			return this.m_BubbleUpCallbacks.Count > 0;
		}

		private static readonly EventCallbackListPool s_ListPool = new EventCallbackListPool();

		internal EventCallbackRegistry.DynamicCallbackList m_TrickleDownCallbacks = EventCallbackRegistry.DynamicCallbackList.Create(TrickleDown.TrickleDown);

		internal EventCallbackRegistry.DynamicCallbackList m_BubbleUpCallbacks = EventCallbackRegistry.DynamicCallbackList.Create(TrickleDown.NoTrickleDown);

		internal struct DynamicCallbackList
		{
			public int Count
			{
				get
				{
					return this.m_Callbacks.Count;
				}
			}

			public static EventCallbackRegistry.DynamicCallbackList Create(TrickleDown useTrickleDown)
			{
				return new EventCallbackRegistry.DynamicCallbackList
				{
					m_UseTrickleDown = useTrickleDown,
					m_Callbacks = EventCallbackList.EmptyList,
					m_TemporaryCallbacks = null,
					m_UnregisteredCallbacksDuringInvoke = null,
					m_IsInvoking = 0
				};
			}

			[NotNull]
			public EventCallbackList GetCallbackListForWriting()
			{
				EventCallbackList eventCallbackList;
				if (this.m_IsInvoking != 0)
				{
					if ((eventCallbackList = this.m_TemporaryCallbacks) == null)
					{
						eventCallbackList = (this.m_TemporaryCallbacks = EventCallbackRegistry.GetCallbackList(this.m_Callbacks));
					}
				}
				else
				{
					eventCallbackList = ((this.m_Callbacks != EventCallbackList.EmptyList) ? this.m_Callbacks : (this.m_Callbacks = EventCallbackRegistry.GetCallbackList(null)));
				}
				return eventCallbackList;
			}

			[NotNull]
			public readonly EventCallbackList GetCallbackListForReading()
			{
				return this.m_TemporaryCallbacks ?? this.m_Callbacks;
			}

			public bool UnregisterCallback(long eventTypeId, [NotNull] Delegate callback)
			{
				EventCallbackList callbackListForWriting = this.GetCallbackListForWriting();
				EventCallbackFunctorBase eventCallbackFunctorBase;
				bool flag = !callbackListForWriting.Remove(eventTypeId, callback, out eventCallbackFunctorBase);
				bool flag2;
				if (flag)
				{
					flag2 = false;
				}
				else
				{
					bool flag3 = this.m_IsInvoking > 0;
					if (flag3)
					{
						List<EventCallbackFunctorBase> list;
						if ((list = this.m_UnregisteredCallbacksDuringInvoke) == null)
						{
							list = (this.m_UnregisteredCallbacksDuringInvoke = CollectionPool<List<EventCallbackFunctorBase>, EventCallbackFunctorBase>.Get());
						}
						list.Add(eventCallbackFunctorBase);
					}
					else
					{
						eventCallbackFunctorBase.Dispose();
					}
					flag2 = true;
				}
				return flag2;
			}

			public unsafe void Invoke(EventBase evt, BaseVisualElementPanel panel, VisualElement target)
			{
				this.BeginInvoke();
				try
				{
					bool flag = !evt.skipDisabledElements || target.enabledInHierarchy;
					long eventTypeId = evt.eventTypeId;
					Span<EventCallbackFunctorBase> span = this.m_Callbacks.Span;
					for (int i = 0; i < span.Length; i++)
					{
						EventCallbackFunctorBase eventCallbackFunctorBase = *span[i];
						bool flag2 = eventCallbackFunctorBase.eventTypeId == eventTypeId && target.elementPanel == panel && (flag || (eventCallbackFunctorBase.invokePolicy & InvokePolicy.IncludeDisabled) > InvokePolicy.Default);
						if (flag2)
						{
							bool flag3 = (eventCallbackFunctorBase.invokePolicy & InvokePolicy.Once) > InvokePolicy.Default;
							if (flag3)
							{
								eventCallbackFunctorBase.UnregisterCallback(target, this.m_UseTrickleDown);
							}
							eventCallbackFunctorBase.Invoke(evt);
							bool isImmediatePropagationStopped = evt.isImmediatePropagationStopped;
							if (isImmediatePropagationStopped)
							{
								break;
							}
						}
					}
				}
				finally
				{
					this.EndInvoke();
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private void BeginInvoke()
			{
				this.m_IsInvoking++;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private void EndInvoke()
			{
				this.m_IsInvoking--;
				bool flag = this.m_IsInvoking == 0;
				if (flag)
				{
					bool flag2 = this.m_TemporaryCallbacks != null;
					if (flag2)
					{
						bool flag3 = this.m_Callbacks != EventCallbackList.EmptyList;
						if (flag3)
						{
							EventCallbackRegistry.ReleaseCallbackList(this.m_Callbacks);
						}
						this.m_Callbacks = EventCallbackRegistry.GetCallbackList(this.m_TemporaryCallbacks);
						EventCallbackRegistry.ReleaseCallbackList(this.m_TemporaryCallbacks);
						this.m_TemporaryCallbacks = null;
						bool flag4 = this.m_UnregisteredCallbacksDuringInvoke != null;
						if (flag4)
						{
							foreach (EventCallbackFunctorBase eventCallbackFunctorBase in this.m_UnregisteredCallbacksDuringInvoke)
							{
								eventCallbackFunctorBase.Dispose();
							}
							CollectionPool<List<EventCallbackFunctorBase>, EventCallbackFunctorBase>.Release(this.m_UnregisteredCallbacksDuringInvoke);
							this.m_UnregisteredCallbacksDuringInvoke = null;
						}
					}
				}
			}

			private TrickleDown m_UseTrickleDown;

			[NotNull]
			private EventCallbackList m_Callbacks;

			[CanBeNull]
			private EventCallbackList m_TemporaryCallbacks;

			[CanBeNull]
			private List<EventCallbackFunctorBase> m_UnregisteredCallbacksDuringInvoke;

			private int m_IsInvoking;
		}
	}
}
