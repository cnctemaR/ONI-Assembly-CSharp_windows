using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements.Experimental
{
	internal class EventDebugger
	{
		public IPanel panel { get; set; }

		public bool isReplaying { get; private set; }

		public float playbackSpeed { get; set; } = 1f;

		public bool isPlaybackPaused { get; set; }

		public void UpdateModificationCount()
		{
			bool flag = this.panel == null;
			if (!flag)
			{
				long num;
				bool flag2 = !this.m_ModificationCount.TryGetValue(this.panel, out num);
				if (flag2)
				{
					num = 0L;
				}
				num += 1L;
				this.m_ModificationCount[this.panel] = num;
			}
		}

		public void BeginProcessEvent(EventBase evt, IEventHandler mouseCapture)
		{
			this.AddBeginProcessEvent(evt, mouseCapture);
			this.UpdateModificationCount();
		}

		public void EndProcessEvent(EventBase evt, long duration, IEventHandler mouseCapture)
		{
			this.AddEndProcessEvent(evt, duration, mouseCapture);
			this.UpdateModificationCount();
		}

		public void LogCall(int cbHashCode, string cbName, EventBase evt, bool propagationHasStopped, bool immediatePropagationHasStopped, long duration, IEventHandler mouseCapture)
		{
			this.AddCallObject(cbHashCode, cbName, evt, propagationHasStopped, immediatePropagationHasStopped, duration, mouseCapture);
			this.UpdateModificationCount();
		}

		public void LogIMGUICall(EventBase evt, long duration, IEventHandler mouseCapture)
		{
			this.AddIMGUICall(evt, duration, mouseCapture);
			this.UpdateModificationCount();
		}

		public void LogExecuteDefaultAction(EventBase evt, PropagationPhase phase, long duration, IEventHandler mouseCapture)
		{
			this.AddExecuteDefaultAction(evt, phase, duration, mouseCapture);
			this.UpdateModificationCount();
		}

		public static void LogPropagationPaths(EventBase evt, PropagationPaths paths)
		{
		}

		private void LogPropagationPathsInternal(EventBase evt, PropagationPaths paths)
		{
			this.AddPropagationPaths(evt, paths);
			this.UpdateModificationCount();
		}

		public List<EventDebuggerCallTrace> GetCalls(IPanel panel, EventDebuggerEventRecord evt = null)
		{
			List<EventDebuggerCallTrace> list;
			bool flag = !this.m_EventCalledObjects.TryGetValue(panel, out list);
			List<EventDebuggerCallTrace> list2;
			if (flag)
			{
				list2 = null;
			}
			else
			{
				bool flag2 = evt != null && list != null;
				if (flag2)
				{
					List<EventDebuggerCallTrace> list3 = new List<EventDebuggerCallTrace>();
					foreach (EventDebuggerCallTrace eventDebuggerCallTrace in list)
					{
						bool flag3 = eventDebuggerCallTrace.eventBase.eventId == evt.eventId;
						if (flag3)
						{
							list3.Add(eventDebuggerCallTrace);
						}
					}
					list = list3;
				}
				list2 = list;
			}
			return list2;
		}

		public List<EventDebuggerDefaultActionTrace> GetDefaultActions(IPanel panel, EventDebuggerEventRecord evt = null)
		{
			List<EventDebuggerDefaultActionTrace> list;
			bool flag = !this.m_EventDefaultActionObjects.TryGetValue(panel, out list);
			List<EventDebuggerDefaultActionTrace> list2;
			if (flag)
			{
				list2 = null;
			}
			else
			{
				bool flag2 = evt != null && list != null;
				if (flag2)
				{
					List<EventDebuggerDefaultActionTrace> list3 = new List<EventDebuggerDefaultActionTrace>();
					foreach (EventDebuggerDefaultActionTrace eventDebuggerDefaultActionTrace in list)
					{
						bool flag3 = eventDebuggerDefaultActionTrace.eventBase.eventId == evt.eventId;
						if (flag3)
						{
							list3.Add(eventDebuggerDefaultActionTrace);
						}
					}
					list = list3;
				}
				list2 = list;
			}
			return list2;
		}

		public List<EventDebuggerPathTrace> GetPropagationPaths(IPanel panel, EventDebuggerEventRecord evt = null)
		{
			List<EventDebuggerPathTrace> list;
			bool flag = !this.m_EventPathObjects.TryGetValue(panel, out list);
			List<EventDebuggerPathTrace> list2;
			if (flag)
			{
				list2 = null;
			}
			else
			{
				bool flag2 = evt != null && list != null;
				if (flag2)
				{
					List<EventDebuggerPathTrace> list3 = new List<EventDebuggerPathTrace>();
					foreach (EventDebuggerPathTrace eventDebuggerPathTrace in list)
					{
						bool flag3 = eventDebuggerPathTrace.eventBase.eventId == evt.eventId;
						if (flag3)
						{
							list3.Add(eventDebuggerPathTrace);
						}
					}
					list = list3;
				}
				list2 = list;
			}
			return list2;
		}

		public List<EventDebuggerTrace> GetBeginEndProcessedEvents(IPanel panel, EventDebuggerEventRecord evt = null)
		{
			List<EventDebuggerTrace> list;
			bool flag = !this.m_EventProcessedEvents.TryGetValue(panel, out list);
			List<EventDebuggerTrace> list2;
			if (flag)
			{
				list2 = null;
			}
			else
			{
				bool flag2 = evt != null && list != null;
				if (flag2)
				{
					List<EventDebuggerTrace> list3 = new List<EventDebuggerTrace>();
					foreach (EventDebuggerTrace eventDebuggerTrace in list)
					{
						bool flag3 = eventDebuggerTrace.eventBase.eventId == evt.eventId;
						if (flag3)
						{
							list3.Add(eventDebuggerTrace);
						}
					}
					list = list3;
				}
				list2 = list;
			}
			return list2;
		}

		public long GetModificationCount(IPanel panel)
		{
			bool flag = panel == null;
			long num;
			if (flag)
			{
				num = -1L;
			}
			else
			{
				long num2;
				bool flag2 = !this.m_ModificationCount.TryGetValue(panel, out num2);
				if (flag2)
				{
					num2 = -1L;
				}
				num = num2;
			}
			return num;
		}

		public void ClearLogs()
		{
			this.UpdateModificationCount();
			bool flag = this.panel == null;
			if (flag)
			{
				this.m_EventCalledObjects.Clear();
				this.m_EventDefaultActionObjects.Clear();
				this.m_EventPathObjects.Clear();
				this.m_EventProcessedEvents.Clear();
				this.m_StackOfProcessedEvent.Clear();
				this.m_EventTypeProcessedCount.Clear();
			}
			else
			{
				this.m_EventCalledObjects.Remove(this.panel);
				this.m_EventDefaultActionObjects.Remove(this.panel);
				this.m_EventPathObjects.Remove(this.panel);
				this.m_EventProcessedEvents.Remove(this.panel);
				this.m_StackOfProcessedEvent.Remove(this.panel);
				Dictionary<long, int> dictionary;
				bool flag2 = this.m_EventTypeProcessedCount.TryGetValue(this.panel, out dictionary);
				if (flag2)
				{
					dictionary.Clear();
				}
			}
		}

		public void SaveReplaySessionFromSelection(string path, List<EventDebuggerEventRecord> eventList)
		{
			bool flag = string.IsNullOrEmpty(path);
			if (!flag)
			{
				EventDebuggerRecordList eventDebuggerRecordList = new EventDebuggerRecordList
				{
					eventList = eventList
				};
				string text = JsonUtility.ToJson(eventDebuggerRecordList);
				File.WriteAllText(path, text);
				Debug.Log("Saved under: " + path);
			}
		}

		public EventDebuggerRecordList LoadReplaySession(string path)
		{
			bool flag = string.IsNullOrEmpty(path);
			EventDebuggerRecordList eventDebuggerRecordList;
			if (flag)
			{
				eventDebuggerRecordList = null;
			}
			else
			{
				string text = File.ReadAllText(path);
				eventDebuggerRecordList = JsonUtility.FromJson<EventDebuggerRecordList>(text);
			}
			return eventDebuggerRecordList;
		}

		public IEnumerator ReplayEvents(IEnumerable<EventDebuggerEventRecord> eventBases, Action<int, int> refreshList)
		{
			bool flag = eventBases == null;
			if (flag)
			{
				yield break;
			}
			this.isReplaying = true;
			IEnumerator doReplay = this.DoReplayEvents(eventBases, refreshList);
			while (doReplay.MoveNext())
			{
				yield return null;
			}
			yield break;
		}

		public void StopPlayback()
		{
			this.isReplaying = false;
			this.isPlaybackPaused = false;
		}

		private IEnumerator DoReplayEvents(IEnumerable<EventDebuggerEventRecord> eventBases, Action<int, int> refreshList)
		{
			EventDebugger.<>c__DisplayClass34_0 CS$<>8__locals1 = new EventDebugger.<>c__DisplayClass34_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.p = this.panel as BaseVisualElementPanel;
			CS$<>8__locals1.sortedEvents = eventBases.OrderBy<EventDebuggerEventRecord, long>((EventDebuggerEventRecord e) => e.timestamp).ToList<EventDebuggerEventRecord>();
			int sortedEventsCount = CS$<>8__locals1.sortedEvents.Count;
			int i = 0;
			while (i < sortedEventsCount)
			{
				bool flag = !this.isReplaying;
				if (flag)
				{
					break;
				}
				EventDebuggerEventRecord eventBase = CS$<>8__locals1.sortedEvents[i];
				Event newEvent = new Event
				{
					button = eventBase.button,
					clickCount = eventBase.clickCount,
					modifiers = eventBase.modifiers,
					mousePosition = eventBase.mousePosition
				};
				bool flag2 = eventBase.eventTypeId == EventBase<PointerMoveEvent>.TypeId();
				if (flag2)
				{
					newEvent.type = EventType.MouseMove;
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|3(UIElementsUtility.CreateEvent(newEvent, EventType.MouseMove));
					goto IL_06D6;
				}
				bool flag3 = eventBase.eventTypeId == EventBase<PointerDownEvent>.TypeId();
				if (flag3)
				{
					newEvent.type = EventType.MouseDown;
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|3(UIElementsUtility.CreateEvent(newEvent, EventType.MouseDown));
					goto IL_06D6;
				}
				bool flag4 = eventBase.eventTypeId == EventBase<PointerUpEvent>.TypeId();
				if (flag4)
				{
					newEvent.type = EventType.MouseUp;
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|3(UIElementsUtility.CreateEvent(newEvent, EventType.MouseUp));
					goto IL_06D6;
				}
				bool flag5 = eventBase.eventTypeId == EventBase<ContextClickEvent>.TypeId();
				if (flag5)
				{
					newEvent.type = EventType.ContextClick;
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|3(UIElementsUtility.CreateEvent(newEvent, EventType.ContextClick));
					goto IL_06D6;
				}
				bool flag6 = eventBase.eventTypeId == EventBase<MouseEnterWindowEvent>.TypeId();
				if (flag6)
				{
					newEvent.type = EventType.MouseEnterWindow;
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|3(UIElementsUtility.CreateEvent(newEvent, EventType.MouseEnterWindow));
					goto IL_06D6;
				}
				bool flag7 = eventBase.eventTypeId == EventBase<MouseLeaveWindowEvent>.TypeId();
				if (flag7)
				{
					newEvent.type = EventType.MouseLeaveWindow;
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|3(UIElementsUtility.CreateEvent(newEvent, EventType.MouseLeaveWindow));
					goto IL_06D6;
				}
				bool flag8 = eventBase.eventTypeId == EventBase<WheelEvent>.TypeId();
				if (flag8)
				{
					newEvent.type = EventType.ScrollWheel;
					newEvent.delta = eventBase.delta;
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|3(UIElementsUtility.CreateEvent(newEvent, EventType.ScrollWheel));
					goto IL_06D6;
				}
				bool flag9 = eventBase.eventTypeId == EventBase<KeyDownEvent>.TypeId();
				if (flag9)
				{
					newEvent.type = EventType.KeyDown;
					newEvent.character = eventBase.character;
					newEvent.keyCode = eventBase.keyCode;
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|3(UIElementsUtility.CreateEvent(newEvent, EventType.KeyDown));
					goto IL_06D6;
				}
				bool flag10 = eventBase.eventTypeId == EventBase<KeyUpEvent>.TypeId();
				if (flag10)
				{
					newEvent.type = EventType.KeyUp;
					newEvent.character = eventBase.character;
					newEvent.keyCode = eventBase.keyCode;
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|3(UIElementsUtility.CreateEvent(newEvent, EventType.KeyUp));
					goto IL_06D6;
				}
				bool flag11 = eventBase.eventTypeId == EventBase<NavigationMoveEvent>.TypeId();
				if (flag11)
				{
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|3(NavigationMoveEvent.GetPooled(eventBase.navigationDirection, eventBase.deviceType, eventBase.modifiers));
					goto IL_06D6;
				}
				bool flag12 = eventBase.eventTypeId == EventBase<NavigationSubmitEvent>.TypeId();
				if (flag12)
				{
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|3(NavigationEventBase<NavigationSubmitEvent>.GetPooled(eventBase.deviceType, eventBase.modifiers));
					goto IL_06D6;
				}
				bool flag13 = eventBase.eventTypeId == EventBase<NavigationCancelEvent>.TypeId();
				if (flag13)
				{
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|3(NavigationEventBase<NavigationCancelEvent>.GetPooled(eventBase.deviceType, eventBase.modifiers));
					goto IL_06D6;
				}
				bool flag14 = eventBase.eventTypeId == EventBase<ValidateCommandEvent>.TypeId();
				if (flag14)
				{
					newEvent.type = EventType.ValidateCommand;
					newEvent.commandName = eventBase.commandName;
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|3(UIElementsUtility.CreateEvent(newEvent, EventType.ValidateCommand));
					goto IL_06D6;
				}
				bool flag15 = eventBase.eventTypeId == EventBase<ExecuteCommandEvent>.TypeId();
				if (flag15)
				{
					newEvent.type = EventType.ExecuteCommand;
					newEvent.commandName = eventBase.commandName;
					CS$<>8__locals1.<DoReplayEvents>g__SendEvent|3(UIElementsUtility.CreateEvent(newEvent, EventType.ExecuteCommand));
					goto IL_06D6;
				}
				bool flag16 = eventBase.eventTypeId == EventBase<IMGUIEvent>.TypeId();
				if (flag16)
				{
					string text = "Skipped IMGUI event (";
					string eventBaseName = eventBase.eventBaseName;
					string text2 = "): ";
					EventDebuggerEventRecord eventDebuggerEventRecord = eventBase;
					Debug.Log(text + eventBaseName + text2 + ((eventDebuggerEventRecord != null) ? eventDebuggerEventRecord.ToString() : null));
					IEnumerator awaitSkipped = CS$<>8__locals1.<DoReplayEvents>g__AwaitForNextEvent|2(i);
					while (awaitSkipped.MoveNext())
					{
						yield return null;
					}
				}
				else
				{
					string text3 = "Skipped event (";
					string eventBaseName2 = eventBase.eventBaseName;
					string text4 = "): ";
					EventDebuggerEventRecord eventDebuggerEventRecord2 = eventBase;
					Debug.Log(text3 + eventBaseName2 + text4 + ((eventDebuggerEventRecord2 != null) ? eventDebuggerEventRecord2.ToString() : null));
					IEnumerator awaitSkipped2 = CS$<>8__locals1.<DoReplayEvents>g__AwaitForNextEvent|2(i);
					while (awaitSkipped2.MoveNext())
					{
						yield return null;
					}
				}
				IL_0780:
				int num = i;
				i = num + 1;
				continue;
				IL_06D6:
				if (refreshList != null)
				{
					refreshList(i, sortedEventsCount);
				}
				Debug.Log(string.Format("Replayed event {0} ({1}): {2}", eventBase.eventId.ToString(), eventBase.eventBaseName, newEvent));
				IEnumerator await = CS$<>8__locals1.<DoReplayEvents>g__AwaitForNextEvent|2(i);
				while (await.MoveNext())
				{
					yield return null;
				}
				eventBase = null;
				newEvent = null;
				await = null;
				goto IL_0780;
			}
			this.isReplaying = false;
			yield break;
		}

		public Dictionary<string, EventDebugger.HistogramRecord> ComputeHistogram(List<EventDebuggerEventRecord> eventBases)
		{
			List<EventDebuggerTrace> list;
			bool flag = this.panel == null || !this.m_EventProcessedEvents.TryGetValue(this.panel, out list);
			Dictionary<string, EventDebugger.HistogramRecord> dictionary;
			if (flag)
			{
				dictionary = null;
			}
			else
			{
				bool flag2 = list == null;
				if (flag2)
				{
					dictionary = null;
				}
				else
				{
					Dictionary<string, EventDebugger.HistogramRecord> dictionary2 = new Dictionary<string, EventDebugger.HistogramRecord>();
					foreach (EventDebuggerTrace eventDebuggerTrace in list)
					{
						bool flag3 = eventBases == null || eventBases.Count == 0 || eventBases.Contains(eventDebuggerTrace.eventBase);
						if (flag3)
						{
							string eventBaseName = eventDebuggerTrace.eventBase.eventBaseName;
							long num = eventDebuggerTrace.duration;
							long num2 = 1L;
							EventDebugger.HistogramRecord histogramRecord;
							bool flag4 = dictionary2.TryGetValue(eventBaseName, out histogramRecord);
							if (flag4)
							{
								num += histogramRecord.duration;
								num2 += histogramRecord.count;
							}
							dictionary2[eventBaseName] = new EventDebugger.HistogramRecord
							{
								count = num2,
								duration = num
							};
						}
					}
					dictionary = dictionary2;
				}
			}
			return dictionary;
		}

		public Dictionary<long, int> eventTypeProcessedCount
		{
			get
			{
				Dictionary<long, int> dictionary;
				return this.m_EventTypeProcessedCount.TryGetValue(this.panel, out dictionary) ? dictionary : null;
			}
		}

		public bool suspended { get; set; }

		public EventDebugger()
		{
			this.m_EventCalledObjects = new Dictionary<IPanel, List<EventDebuggerCallTrace>>();
			this.m_EventDefaultActionObjects = new Dictionary<IPanel, List<EventDebuggerDefaultActionTrace>>();
			this.m_EventPathObjects = new Dictionary<IPanel, List<EventDebuggerPathTrace>>();
			this.m_StackOfProcessedEvent = new Dictionary<IPanel, Stack<EventDebuggerTrace>>();
			this.m_EventProcessedEvents = new Dictionary<IPanel, List<EventDebuggerTrace>>();
			this.m_EventTypeProcessedCount = new Dictionary<IPanel, Dictionary<long, int>>();
			this.m_ModificationCount = new Dictionary<IPanel, long>();
			this.m_Log = true;
		}

		private void AddCallObject(int cbHashCode, string cbName, EventBase evt, bool propagationHasStopped, bool immediatePropagationHasStopped, long duration, IEventHandler mouseCapture)
		{
			bool suspended = this.suspended;
			if (!suspended)
			{
				bool log = this.m_Log;
				if (log)
				{
					EventDebuggerCallTrace eventDebuggerCallTrace = new EventDebuggerCallTrace(this.panel, evt, cbHashCode, cbName, propagationHasStopped, immediatePropagationHasStopped, duration, mouseCapture);
					List<EventDebuggerCallTrace> list;
					bool flag = !this.m_EventCalledObjects.TryGetValue(this.panel, out list);
					if (flag)
					{
						list = new List<EventDebuggerCallTrace>();
						this.m_EventCalledObjects.Add(this.panel, list);
					}
					list.Add(eventDebuggerCallTrace);
				}
			}
		}

		private void AddExecuteDefaultAction(EventBase evt, PropagationPhase phase, long duration, IEventHandler mouseCapture)
		{
			bool suspended = this.suspended;
			if (!suspended)
			{
				bool log = this.m_Log;
				if (log)
				{
					EventDebuggerDefaultActionTrace eventDebuggerDefaultActionTrace = new EventDebuggerDefaultActionTrace(this.panel, evt, phase, duration, mouseCapture);
					List<EventDebuggerDefaultActionTrace> list;
					bool flag = !this.m_EventDefaultActionObjects.TryGetValue(this.panel, out list);
					if (flag)
					{
						list = new List<EventDebuggerDefaultActionTrace>();
						this.m_EventDefaultActionObjects.Add(this.panel, list);
					}
					list.Add(eventDebuggerDefaultActionTrace);
				}
			}
		}

		private void AddPropagationPaths(EventBase evt, PropagationPaths paths)
		{
			bool suspended = this.suspended;
			if (!suspended)
			{
				bool log = this.m_Log;
				if (log)
				{
					EventDebuggerPathTrace eventDebuggerPathTrace = new EventDebuggerPathTrace(this.panel, evt, new PropagationPaths(paths));
					List<EventDebuggerPathTrace> list;
					bool flag = !this.m_EventPathObjects.TryGetValue(this.panel, out list);
					if (flag)
					{
						list = new List<EventDebuggerPathTrace>();
						this.m_EventPathObjects.Add(this.panel, list);
					}
					list.Add(eventDebuggerPathTrace);
				}
			}
		}

		private void AddIMGUICall(EventBase evt, long duration, IEventHandler mouseCapture)
		{
			bool suspended = this.suspended;
			if (!suspended)
			{
				bool log = this.m_Log;
				if (log)
				{
					EventDebuggerCallTrace eventDebuggerCallTrace = new EventDebuggerCallTrace(this.panel, evt, 0, "OnGUI", false, false, duration, mouseCapture);
					List<EventDebuggerCallTrace> list;
					bool flag = !this.m_EventCalledObjects.TryGetValue(this.panel, out list);
					if (flag)
					{
						list = new List<EventDebuggerCallTrace>();
						this.m_EventCalledObjects.Add(this.panel, list);
					}
					list.Add(eventDebuggerCallTrace);
				}
			}
		}

		private void AddBeginProcessEvent(EventBase evt, IEventHandler mouseCapture)
		{
			bool suspended = this.suspended;
			if (!suspended)
			{
				EventDebuggerTrace eventDebuggerTrace = new EventDebuggerTrace(this.panel, evt, -1L, mouseCapture);
				Stack<EventDebuggerTrace> stack;
				bool flag = !this.m_StackOfProcessedEvent.TryGetValue(this.panel, out stack);
				if (flag)
				{
					stack = new Stack<EventDebuggerTrace>();
					this.m_StackOfProcessedEvent.Add(this.panel, stack);
				}
				List<EventDebuggerTrace> list;
				bool flag2 = !this.m_EventProcessedEvents.TryGetValue(this.panel, out list);
				if (flag2)
				{
					list = new List<EventDebuggerTrace>();
					this.m_EventProcessedEvents.Add(this.panel, list);
				}
				list.Add(eventDebuggerTrace);
				stack.Push(eventDebuggerTrace);
				Dictionary<long, int> dictionary;
				bool flag3 = !this.m_EventTypeProcessedCount.TryGetValue(this.panel, out dictionary);
				if (!flag3)
				{
					int num;
					bool flag4 = !dictionary.TryGetValue(eventDebuggerTrace.eventBase.eventTypeId, out num);
					if (flag4)
					{
						num = 0;
					}
					dictionary[eventDebuggerTrace.eventBase.eventTypeId] = num + 1;
				}
			}
		}

		private void AddEndProcessEvent(EventBase evt, long duration, IEventHandler mouseCapture)
		{
			bool suspended = this.suspended;
			if (!suspended)
			{
				bool flag = false;
				Stack<EventDebuggerTrace> stack;
				bool flag2 = this.m_StackOfProcessedEvent.TryGetValue(this.panel, out stack);
				if (flag2)
				{
					bool flag3 = stack.Count > 0;
					if (flag3)
					{
						EventDebuggerTrace eventDebuggerTrace = stack.Peek();
						bool flag4 = eventDebuggerTrace.eventBase.eventId == evt.eventId;
						if (flag4)
						{
							stack.Pop();
							eventDebuggerTrace.duration = duration;
							bool flag5 = eventDebuggerTrace.eventBase.target == null;
							if (flag5)
							{
								eventDebuggerTrace.eventBase.target = evt.target;
							}
							flag = true;
						}
					}
				}
				bool flag6 = !flag;
				if (flag6)
				{
					EventDebuggerTrace eventDebuggerTrace2 = new EventDebuggerTrace(this.panel, evt, duration, mouseCapture);
					List<EventDebuggerTrace> list;
					bool flag7 = !this.m_EventProcessedEvents.TryGetValue(this.panel, out list);
					if (flag7)
					{
						list = new List<EventDebuggerTrace>();
						this.m_EventProcessedEvents.Add(this.panel, list);
					}
					list.Add(eventDebuggerTrace2);
					Dictionary<long, int> dictionary;
					bool flag8 = !this.m_EventTypeProcessedCount.TryGetValue(this.panel, out dictionary);
					if (!flag8)
					{
						int num;
						bool flag9 = !dictionary.TryGetValue(eventDebuggerTrace2.eventBase.eventTypeId, out num);
						if (flag9)
						{
							num = 0;
						}
						dictionary[eventDebuggerTrace2.eventBase.eventTypeId] = num + 1;
					}
				}
			}
		}

		public static string GetObjectDisplayName(object obj, bool withHashCode = true)
		{
			bool flag = obj == null;
			string text;
			if (flag)
			{
				text = string.Empty;
			}
			else
			{
				Type type = obj.GetType();
				string text2 = EventDebugger.GetTypeDisplayName(type);
				bool flag2 = obj is VisualElement;
				if (flag2)
				{
					VisualElement visualElement = obj as VisualElement;
					bool flag3 = !string.IsNullOrEmpty(visualElement.name);
					if (flag3)
					{
						text2 = text2 + "#" + visualElement.name;
					}
				}
				if (withHashCode)
				{
					text2 = text2 + " (" + obj.GetHashCode().ToString("x8") + ")";
				}
				text = text2;
			}
			return text;
		}

		public static string GetTypeDisplayName(Type type)
		{
			return type.IsGenericType ? (type.Name.TrimEnd(new char[] { '`', '1' }) + "<" + type.GetGenericArguments()[0].Name + ">") : type.Name;
		}

		[CompilerGenerated]
		internal static long <DoReplayEvents>g__CurrentTimeMs|34_1(BaseVisualElementPanel p)
		{
			return (p != null) ? p.TimeSinceStartupMs() : 0L;
		}

		private Dictionary<IPanel, List<EventDebuggerCallTrace>> m_EventCalledObjects;

		private Dictionary<IPanel, List<EventDebuggerDefaultActionTrace>> m_EventDefaultActionObjects;

		private Dictionary<IPanel, List<EventDebuggerPathTrace>> m_EventPathObjects;

		private Dictionary<IPanel, List<EventDebuggerTrace>> m_EventProcessedEvents;

		private Dictionary<IPanel, Stack<EventDebuggerTrace>> m_StackOfProcessedEvent;

		private Dictionary<IPanel, Dictionary<long, int>> m_EventTypeProcessedCount;

		private readonly Dictionary<IPanel, long> m_ModificationCount;

		private readonly bool m_Log;

		internal struct HistogramRecord
		{
			public long count;

			public long duration;
		}
	}
}
