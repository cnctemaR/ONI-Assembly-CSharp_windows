using System;
using System.Collections.Generic;
using Unity.IntegerTime;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.InputForUI
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static class EventProvider
	{
		public static IEventProviderImpl provider
		{
			get
			{
				return EventProvider.s_impl;
			}
		}

		public static void Subscribe(EventConsumer handler, int priority = 0, int? playerId = null, params Event.Type[] type)
		{
			EventProvider.Bootstrap();
			EventProvider._registrations.Add(new EventProvider.Registration
			{
				handler = handler,
				priority = priority,
				playerId = playerId,
				_types = new HashSet<Event.Type>(type)
			});
			EventProvider._registrations.Sort((EventProvider.Registration a, EventProvider.Registration b) => a.priority.CompareTo(b.priority));
		}

		public static void Unsubscribe(EventConsumer handler)
		{
			EventProvider._registrations.RemoveAll((EventProvider.Registration x) => x.handler == handler);
		}

		public static void SetEnabled(bool enable)
		{
			EventProvider.m_IsEnabled = enable;
			if (enable)
			{
				EventProvider.Initialize();
			}
			else
			{
				EventProvider.Shutdown();
			}
		}

		internal static void Dispatch(in Event ev)
		{
			bool flag = EventProvider._registrations.Count == 0;
			if (!flag)
			{
				EventProvider.s_sanitizer.Inspect(in ev);
				foreach (EventProvider.Registration registration in EventProvider._registrations)
				{
					bool flag2;
					if (registration._types.Count > 0)
					{
						HashSet<Event.Type> types = registration._types;
						Event @event = ev;
						flag2 = !types.Contains(@event.type);
					}
					else
					{
						flag2 = false;
					}
					bool flag3 = flag2;
					if (!flag3)
					{
						bool flag4 = registration.handler(in ev);
						if (flag4)
						{
							break;
						}
					}
				}
			}
		}

		public static void RequestCurrentState(params Event.Type[] types)
		{
			foreach (Event.Type type in (types != null && types.Length > 0) ? types : Event.TypesWithState)
			{
				IEventProviderImpl eventProviderImpl = EventProvider.s_impl;
				bool flag = !((eventProviderImpl != null) ? new bool?(eventProviderImpl.RequestCurrentState(type)) : null).GetValueOrDefault();
				if (flag)
				{
					Debug.LogWarning(string.Format("Can't provide state for type {0}", type));
				}
			}
		}

		public static uint playerCount
		{
			get
			{
				IEventProviderImpl eventProviderImpl = EventProvider.s_impl;
				return (eventProviderImpl != null) ? eventProviderImpl.playerCount : 0U;
			}
		}

		private static void Bootstrap()
		{
			bool isEnabled = EventProvider.m_IsEnabled;
			if (isEnabled)
			{
				EventProvider.Initialize();
			}
		}

		private static void Initialize()
		{
			bool isInitialized = EventProvider.m_IsInitialized;
			if (!isInitialized)
			{
				EventProvider.s_sanitizer.Reset();
				IEventProviderImpl eventProviderImpl = EventProvider.s_impl;
				if (eventProviderImpl != null)
				{
					eventProviderImpl.Initialize();
				}
				bool flag = !EventProvider.s_focusChangedRegistered;
				if (flag)
				{
					Application.focusChanged += EventProvider.OnFocusChanged;
					EventProvider.s_focusChangedRegistered = true;
				}
				EventProvider.m_IsInitialized = true;
			}
		}

		private static void Shutdown()
		{
			bool flag = !EventProvider.m_IsInitialized;
			if (!flag)
			{
				EventProvider.m_IsInitialized = false;
				bool flag2 = EventProvider.s_focusChangedRegistered;
				if (flag2)
				{
					EventProvider.s_focusChangedRegistered = false;
					Application.focusChanged -= EventProvider.OnFocusChanged;
				}
				IEventProviderImpl eventProviderImpl = EventProvider.s_impl;
				if (eventProviderImpl != null)
				{
					eventProviderImpl.Shutdown();
				}
			}
		}

		private static void OnFocusChanged(bool focus)
		{
			IEventProviderImpl eventProviderImpl = EventProvider.s_impl;
			if (eventProviderImpl != null)
			{
				eventProviderImpl.OnFocusChanged(focus);
			}
		}

		[RequiredByNativeCode]
		internal static void NotifyUpdate()
		{
			bool flag = !Application.isPlaying || EventProvider._registrations.Count == 0 || !EventProvider.m_IsInitialized;
			if (!flag)
			{
				EventProvider.s_sanitizer.BeforeProviderUpdate();
				IEventProviderImpl eventProviderImpl = EventProvider.s_impl;
				if (eventProviderImpl != null)
				{
					eventProviderImpl.Update();
				}
				EventProvider.s_sanitizer.AfterProviderUpdate();
			}
		}

		internal static void SetInputSystemProvider(IEventProviderImpl impl)
		{
			bool isInitialized = EventProvider.m_IsInitialized;
			EventProvider.Shutdown();
			EventProvider.s_impl = impl;
			bool flag = isInitialized;
			if (flag)
			{
				EventProvider.Initialize();
			}
		}

		internal static void SetMockProvider(IEventProviderImpl impl)
		{
			bool flag = EventProvider.s_implMockBackup == null;
			if (flag)
			{
				EventProvider.s_implMockBackup = EventProvider.s_impl;
			}
			EventProvider.s_focusStateBeforeMock = Application.isFocused;
			EventProvider.Shutdown();
			EventProvider.s_impl = impl;
			EventProvider.Initialize();
		}

		internal static void ClearMockProvider()
		{
			EventProvider.Shutdown();
			EventProvider.s_impl = EventProvider.s_implMockBackup;
			EventProvider.s_implMockBackup = null;
			EventProvider.Initialize();
			bool flag = EventProvider.s_focusStateBeforeMock != Application.isFocused;
			if (flag)
			{
				IEventProviderImpl eventProviderImpl = EventProvider.s_impl;
				if (eventProviderImpl != null)
				{
					eventProviderImpl.OnFocusChanged(Application.isFocused);
				}
			}
		}

		internal static string _providerClassName
		{
			get
			{
				IEventProviderImpl eventProviderImpl = EventProvider.s_impl;
				return (eventProviderImpl != null) ? eventProviderImpl.GetType().Name : null;
			}
		}

		internal static RationalTime doubleClickTime
		{
			get
			{
				int doubleClickTime = Event.GetDoubleClickTime();
				return new RationalTime((long)doubleClickTime, new RationalTime.TicksPerSecond(1000U, 1U));
			}
		}

		private static IEventProviderImpl s_impl = new InputManagerProvider();

		private static EventSanitizer s_sanitizer;

		private static IEventProviderImpl s_implMockBackup = null;

		private static bool s_focusStateBeforeMock;

		private static bool s_focusChangedRegistered;

		private static bool m_IsEnabled = true;

		private static bool m_IsInitialized = false;

		private static List<EventProvider.Registration> _registrations = new List<EventProvider.Registration>();

		private struct Registration
		{
			public EventConsumer handler;

			public int priority;

			public int? playerId;

			public HashSet<Event.Type> _types;
		}
	}
}
