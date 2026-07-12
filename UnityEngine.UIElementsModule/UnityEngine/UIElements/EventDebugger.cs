using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal class EventDebugger
	{
		public IPanel panel { get; set; }

		public void UpdateModificationCount()
		{
			bool flag = this.panel == null;
			if (!flag)
			{
				long num = 0L;
				bool flag2 = this.m_ModificationCount.ContainsKey(this.panel);
				if (flag2)
				{
					num = this.m_ModificationCount[this.panel];
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

		public void LogCall(int cbHashCode, string cbName, EventBase evt, bool propagationHasStopped, bool immediatePropagationHasStopped, bool defaultHasBeenPrevented, long duration, IEventHandler mouseCapture)
		{
			this.AddCallObject(cbHashCode, cbName, evt, propagationHasStopped, immediatePropagationHasStopped, defaultHasBeenPrevented, duration, mouseCapture);
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
			PropagationPaths propagationPaths = ((paths == null) ? new PropagationPaths() : new PropagationPaths(paths));
			this.AddPropagationPaths(evt, propagationPaths);
			this.UpdateModificationCount();
		}

		public List<EventDebuggerCallTrace> GetCalls(IPanel panel, EventDebuggerEventRecord evt = null)
		{
			List<EventDebuggerCallTrace> list = null;
			bool flag = this.m_EventCalledObjects.ContainsKey(panel);
			if (flag)
			{
				list = this.m_EventCalledObjects[panel];
			}
			bool flag2 = evt != null && list != null;
			if (flag2)
			{
				List<EventDebuggerCallTrace> list2 = new List<EventDebuggerCallTrace>();
				foreach (EventDebuggerCallTrace eventDebuggerCallTrace in list)
				{
					bool flag3 = eventDebuggerCallTrace.eventBase.eventId == evt.eventId;
					if (flag3)
					{
						list2.Add(eventDebuggerCallTrace);
					}
				}
				list = list2;
			}
			return list;
		}

		public List<EventDebuggerDefaultActionTrace> GetDefaultActions(IPanel panel, EventDebuggerEventRecord evt = null)
		{
			List<EventDebuggerDefaultActionTrace> list = null;
			bool flag = this.m_EventDefaultActionObjects.ContainsKey(panel);
			if (flag)
			{
				list = this.m_EventDefaultActionObjects[panel];
			}
			bool flag2 = evt != null && list != null;
			if (flag2)
			{
				List<EventDebuggerDefaultActionTrace> list2 = new List<EventDebuggerDefaultActionTrace>();
				foreach (EventDebuggerDefaultActionTrace eventDebuggerDefaultActionTrace in list)
				{
					bool flag3 = eventDebuggerDefaultActionTrace.eventBase.eventId == evt.eventId;
					if (flag3)
					{
						list2.Add(eventDebuggerDefaultActionTrace);
					}
				}
				list = list2;
			}
			return list;
		}

		public List<EventDebuggerPathTrace> GetPropagationPaths(IPanel panel, EventDebuggerEventRecord evt = null)
		{
			List<EventDebuggerPathTrace> list = null;
			bool flag = this.m_EventPathObjects.ContainsKey(panel);
			if (flag)
			{
				list = this.m_EventPathObjects[panel];
			}
			bool flag2 = evt != null && list != null;
			if (flag2)
			{
				List<EventDebuggerPathTrace> list2 = new List<EventDebuggerPathTrace>();
				foreach (EventDebuggerPathTrace eventDebuggerPathTrace in list)
				{
					bool flag3 = eventDebuggerPathTrace.eventBase.eventId == evt.eventId;
					if (flag3)
					{
						list2.Add(eventDebuggerPathTrace);
					}
				}
				list = list2;
			}
			return list;
		}

		public List<EventDebuggerTrace> GetBeginEndProcessedEvents(IPanel panel, EventDebuggerEventRecord evt = null)
		{
			List<EventDebuggerTrace> list = null;
			bool flag = this.m_EventProcessedEvents.ContainsKey(panel);
			if (flag)
			{
				list = this.m_EventProcessedEvents[panel];
			}
			bool flag2 = evt != null && list != null;
			if (flag2)
			{
				List<EventDebuggerTrace> list2 = new List<EventDebuggerTrace>();
				foreach (EventDebuggerTrace eventDebuggerTrace in list)
				{
					bool flag3 = eventDebuggerTrace.eventBase.eventId == evt.eventId;
					if (flag3)
					{
						list2.Add(eventDebuggerTrace);
					}
				}
				list = list2;
			}
			return list;
		}

		public long GetModificationCount(IPanel panel)
		{
			long num = -1L;
			bool flag = panel != null && this.m_ModificationCount.ContainsKey(panel);
			if (flag)
			{
				num = this.m_ModificationCount[panel];
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
			}
			else
			{
				this.m_EventCalledObjects.Remove(this.panel);
				this.m_EventDefaultActionObjects.Remove(this.panel);
				this.m_EventPathObjects.Remove(this.panel);
				this.m_EventProcessedEvents.Remove(this.panel);
				this.m_StackOfProcessedEvent.Remove(this.panel);
			}
		}

		public void ReplayEvents(List<EventDebuggerEventRecord> eventBases)
		{
			bool flag = eventBases == null;
			if (!flag)
			{
				foreach (EventDebuggerEventRecord eventDebuggerEventRecord in eventBases)
				{
					Event @event = new Event
					{
						button = eventDebuggerEventRecord.button,
						clickCount = eventDebuggerEventRecord.clickCount,
						modifiers = eventDebuggerEventRecord.modifiers,
						mousePosition = eventDebuggerEventRecord.mousePosition
					};
					bool flag2 = eventDebuggerEventRecord.eventTypeId == EventBase<MouseMoveEvent>.TypeId() && eventDebuggerEventRecord.hasUnderlyingPhysicalEvent;
					if (flag2)
					{
						@event.type = EventType.MouseMove;
						this.panel.dispatcher.Dispatch(UIElementsUtility.CreateEvent(@event, EventType.MouseMove), this.panel, DispatchMode.Default);
					}
					else
					{
						bool flag3 = eventDebuggerEventRecord.eventTypeId == EventBase<MouseDownEvent>.TypeId() && eventDebuggerEventRecord.hasUnderlyingPhysicalEvent;
						if (flag3)
						{
							@event.type = EventType.MouseDown;
							this.panel.dispatcher.Dispatch(UIElementsUtility.CreateEvent(@event, EventType.MouseDown), this.panel, DispatchMode.Default);
						}
						else
						{
							bool flag4 = eventDebuggerEventRecord.eventTypeId == EventBase<MouseUpEvent>.TypeId() && eventDebuggerEventRecord.hasUnderlyingPhysicalEvent;
							if (flag4)
							{
								@event.type = EventType.MouseUp;
								this.panel.dispatcher.Dispatch(UIElementsUtility.CreateEvent(@event, EventType.MouseUp), this.panel, DispatchMode.Default);
							}
							else
							{
								bool flag5 = eventDebuggerEventRecord.eventTypeId == EventBase<ContextClickEvent>.TypeId() && eventDebuggerEventRecord.hasUnderlyingPhysicalEvent;
								if (flag5)
								{
									@event.type = EventType.ContextClick;
									this.panel.dispatcher.Dispatch(UIElementsUtility.CreateEvent(@event, EventType.ContextClick), this.panel, DispatchMode.Default);
								}
								else
								{
									bool flag6 = eventDebuggerEventRecord.eventTypeId == EventBase<MouseEnterWindowEvent>.TypeId() && eventDebuggerEventRecord.hasUnderlyingPhysicalEvent;
									if (flag6)
									{
										@event.type = EventType.MouseEnterWindow;
										this.panel.dispatcher.Dispatch(UIElementsUtility.CreateEvent(@event, EventType.MouseEnterWindow), this.panel, DispatchMode.Default);
									}
									else
									{
										bool flag7 = eventDebuggerEventRecord.eventTypeId == EventBase<MouseLeaveWindowEvent>.TypeId() && eventDebuggerEventRecord.hasUnderlyingPhysicalEvent;
										if (flag7)
										{
											@event.type = EventType.MouseLeaveWindow;
											this.panel.dispatcher.Dispatch(UIElementsUtility.CreateEvent(@event, EventType.MouseLeaveWindow), this.panel, DispatchMode.Default);
										}
										else
										{
											bool flag8 = eventDebuggerEventRecord.eventTypeId == EventBase<WheelEvent>.TypeId() && eventDebuggerEventRecord.hasUnderlyingPhysicalEvent;
											if (flag8)
											{
												@event.type = EventType.ScrollWheel;
												@event.delta = eventDebuggerEventRecord.delta;
												this.panel.dispatcher.Dispatch(UIElementsUtility.CreateEvent(@event, EventType.ScrollWheel), this.panel, DispatchMode.Default);
											}
											else
											{
												bool flag9 = eventDebuggerEventRecord.eventTypeId == EventBase<KeyDownEvent>.TypeId();
												if (flag9)
												{
													@event.type = EventType.KeyDown;
													@event.character = eventDebuggerEventRecord.character;
													@event.keyCode = eventDebuggerEventRecord.keyCode;
													this.panel.dispatcher.Dispatch(UIElementsUtility.CreateEvent(@event, EventType.KeyDown), this.panel, DispatchMode.Default);
												}
												else
												{
													bool flag10 = eventDebuggerEventRecord.eventTypeId == EventBase<KeyUpEvent>.TypeId();
													if (flag10)
													{
														@event.type = EventType.KeyUp;
														@event.character = eventDebuggerEventRecord.character;
														@event.keyCode = eventDebuggerEventRecord.keyCode;
														this.panel.dispatcher.Dispatch(UIElementsUtility.CreateEvent(@event, EventType.KeyUp), this.panel, DispatchMode.Default);
													}
													else
													{
														bool flag11 = eventDebuggerEventRecord.eventTypeId == EventBase<DragUpdatedEvent>.TypeId();
														if (flag11)
														{
															@event.type = EventType.DragUpdated;
															this.panel.dispatcher.Dispatch(UIElementsUtility.CreateEvent(@event, EventType.DragUpdated), this.panel, DispatchMode.Default);
														}
														else
														{
															bool flag12 = eventDebuggerEventRecord.eventTypeId == EventBase<DragPerformEvent>.TypeId();
															if (flag12)
															{
																@event.type = EventType.DragPerform;
																this.panel.dispatcher.Dispatch(UIElementsUtility.CreateEvent(@event, EventType.DragPerform), this.panel, DispatchMode.Default);
															}
															else
															{
																bool flag13 = eventDebuggerEventRecord.eventTypeId == EventBase<DragExitedEvent>.TypeId();
																if (flag13)
																{
																	@event.type = EventType.DragExited;
																	this.panel.dispatcher.Dispatch(UIElementsUtility.CreateEvent(@event, EventType.DragExited), this.panel, DispatchMode.Default);
																}
																else
																{
																	bool flag14 = eventDebuggerEventRecord.eventTypeId == EventBase<ValidateCommandEvent>.TypeId();
																	if (flag14)
																	{
																		@event.type = EventType.ValidateCommand;
																		@event.commandName = eventDebuggerEventRecord.commandName;
																		this.panel.dispatcher.Dispatch(UIElementsUtility.CreateEvent(@event, EventType.ValidateCommand), this.panel, DispatchMode.Default);
																	}
																	else
																	{
																		bool flag15 = eventDebuggerEventRecord.eventTypeId == EventBase<ExecuteCommandEvent>.TypeId();
																		if (flag15)
																		{
																			@event.type = EventType.ExecuteCommand;
																			@event.commandName = eventDebuggerEventRecord.commandName;
																			this.panel.dispatcher.Dispatch(UIElementsUtility.CreateEvent(@event, EventType.ExecuteCommand), this.panel, DispatchMode.Default);
																		}
																		else
																		{
																			bool flag16 = eventDebuggerEventRecord.eventTypeId == EventBase<IMGUIEvent>.TypeId();
																			if (flag16)
																			{
																				string text = "Skipped IMGUI event (";
																				string eventBaseName = eventDebuggerEventRecord.eventBaseName;
																				string text2 = "): ";
																				EventDebuggerEventRecord eventDebuggerEventRecord2 = eventDebuggerEventRecord;
																				Debug.Log(text + eventBaseName + text2 + ((eventDebuggerEventRecord2 != null) ? eventDebuggerEventRecord2.ToString() : null));
																				continue;
																			}
																			string text3 = "Skipped event (";
																			string eventBaseName2 = eventDebuggerEventRecord.eventBaseName;
																			string text4 = "): ";
																			EventDebuggerEventRecord eventDebuggerEventRecord3 = eventDebuggerEventRecord;
																			Debug.Log(text3 + eventBaseName2 + text4 + ((eventDebuggerEventRecord3 != null) ? eventDebuggerEventRecord3.ToString() : null));
																			continue;
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
					string text5 = "Replayed event (";
					string eventBaseName3 = eventDebuggerEventRecord.eventBaseName;
					string text6 = "): ";
					Event event2 = @event;
					Debug.Log(text5 + eventBaseName3 + text6 + ((event2 != null) ? event2.ToString() : null));
				}
			}
		}

		public Dictionary<string, long> ComputeHistogram(List<EventDebuggerEventRecord> eventBases)
		{
			bool flag = this.panel == null || !this.m_EventProcessedEvents.ContainsKey(this.panel);
			Dictionary<string, long> dictionary;
			if (flag)
			{
				dictionary = null;
			}
			else
			{
				List<EventDebuggerTrace> list = this.m_EventProcessedEvents[this.panel];
				bool flag2 = list == null;
				if (flag2)
				{
					dictionary = null;
				}
				else
				{
					Dictionary<string, long> dictionary2 = new Dictionary<string, long>();
					foreach (EventDebuggerTrace eventDebuggerTrace in list)
					{
						bool flag3 = eventBases == null || eventBases.Count == 0 || eventBases.Contains(eventDebuggerTrace.eventBase);
						if (flag3)
						{
							string eventBaseName = eventDebuggerTrace.eventBase.eventBaseName;
							long num = eventDebuggerTrace.duration;
							bool flag4 = dictionary2.ContainsKey(eventBaseName);
							if (flag4)
							{
								long num2 = dictionary2[eventBaseName];
								num += num2;
							}
							dictionary2[eventBaseName] = num;
						}
					}
					dictionary = dictionary2;
				}
			}
			return dictionary;
		}

		public EventDebugger()
		{
			this.m_EventCalledObjects = new Dictionary<IPanel, List<EventDebuggerCallTrace>>();
			this.m_EventDefaultActionObjects = new Dictionary<IPanel, List<EventDebuggerDefaultActionTrace>>();
			this.m_EventPathObjects = new Dictionary<IPanel, List<EventDebuggerPathTrace>>();
			this.m_StackOfProcessedEvent = new Dictionary<IPanel, Stack<EventDebuggerTrace>>();
			this.m_EventProcessedEvents = new Dictionary<IPanel, List<EventDebuggerTrace>>();
			this.m_ModificationCount = new Dictionary<IPanel, long>();
			this.m_Log = true;
		}

		private void AddCallObject(int cbHashCode, string cbName, EventBase evt, bool propagationHasStopped, bool immediatePropagationHasStopped, bool defaultHasBeenPrevented, long duration, IEventHandler mouseCapture)
		{
			bool log = this.m_Log;
			if (log)
			{
				EventDebuggerCallTrace eventDebuggerCallTrace = new EventDebuggerCallTrace(this.panel, evt, cbHashCode, cbName, propagationHasStopped, immediatePropagationHasStopped, defaultHasBeenPrevented, duration, mouseCapture);
				bool flag = this.m_EventCalledObjects.ContainsKey(this.panel);
				List<EventDebuggerCallTrace> list;
				if (flag)
				{
					list = this.m_EventCalledObjects[this.panel];
				}
				else
				{
					list = new List<EventDebuggerCallTrace>();
					this.m_EventCalledObjects.Add(this.panel, list);
				}
				list.Add(eventDebuggerCallTrace);
			}
		}

		private void AddExecuteDefaultAction(EventBase evt, PropagationPhase phase, long duration, IEventHandler mouseCapture)
		{
			bool log = this.m_Log;
			if (log)
			{
				EventDebuggerDefaultActionTrace eventDebuggerDefaultActionTrace = new EventDebuggerDefaultActionTrace(this.panel, evt, phase, duration, mouseCapture);
				bool flag = this.m_EventDefaultActionObjects.ContainsKey(this.panel);
				List<EventDebuggerDefaultActionTrace> list;
				if (flag)
				{
					list = this.m_EventDefaultActionObjects[this.panel];
				}
				else
				{
					list = new List<EventDebuggerDefaultActionTrace>();
					this.m_EventDefaultActionObjects.Add(this.panel, list);
				}
				list.Add(eventDebuggerDefaultActionTrace);
			}
		}

		private void AddPropagationPaths(EventBase evt, PropagationPaths paths)
		{
			bool log = this.m_Log;
			if (log)
			{
				EventDebuggerPathTrace eventDebuggerPathTrace = new EventDebuggerPathTrace(this.panel, evt, paths);
				bool flag = this.m_EventPathObjects.ContainsKey(this.panel);
				List<EventDebuggerPathTrace> list;
				if (flag)
				{
					list = this.m_EventPathObjects[this.panel];
				}
				else
				{
					list = new List<EventDebuggerPathTrace>();
					this.m_EventPathObjects.Add(this.panel, list);
				}
				list.Add(eventDebuggerPathTrace);
			}
		}

		private void AddIMGUICall(EventBase evt, long duration, IEventHandler mouseCapture)
		{
			bool log = this.m_Log;
			if (log)
			{
				EventDebuggerCallTrace eventDebuggerCallTrace = new EventDebuggerCallTrace(this.panel, evt, 0, "OnGUI", false, false, false, duration, mouseCapture);
				bool flag = this.m_EventCalledObjects.ContainsKey(this.panel);
				List<EventDebuggerCallTrace> list;
				if (flag)
				{
					list = this.m_EventCalledObjects[this.panel];
				}
				else
				{
					list = new List<EventDebuggerCallTrace>();
					this.m_EventCalledObjects.Add(this.panel, list);
				}
				list.Add(eventDebuggerCallTrace);
			}
		}

		private void AddBeginProcessEvent(EventBase evt, IEventHandler mouseCapture)
		{
			EventDebuggerTrace eventDebuggerTrace = new EventDebuggerTrace(this.panel, evt, -1L, mouseCapture);
			bool flag = this.m_StackOfProcessedEvent.ContainsKey(this.panel);
			Stack<EventDebuggerTrace> stack;
			if (flag)
			{
				stack = this.m_StackOfProcessedEvent[this.panel];
			}
			else
			{
				stack = new Stack<EventDebuggerTrace>();
				this.m_StackOfProcessedEvent.Add(this.panel, stack);
			}
			bool flag2 = this.m_EventProcessedEvents.ContainsKey(this.panel);
			List<EventDebuggerTrace> list;
			if (flag2)
			{
				list = this.m_EventProcessedEvents[this.panel];
			}
			else
			{
				list = new List<EventDebuggerTrace>();
				this.m_EventProcessedEvents.Add(this.panel, list);
			}
			list.Add(eventDebuggerTrace);
			stack.Push(eventDebuggerTrace);
		}

		private void AddEndProcessEvent(EventBase evt, long duration, IEventHandler mouseCapture)
		{
			bool flag = false;
			bool flag2 = this.m_StackOfProcessedEvent.ContainsKey(this.panel);
			if (flag2)
			{
				Stack<EventDebuggerTrace> stack = this.m_StackOfProcessedEvent[this.panel];
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
				bool flag7 = this.m_EventProcessedEvents.ContainsKey(this.panel);
				List<EventDebuggerTrace> list;
				if (flag7)
				{
					list = this.m_EventProcessedEvents[this.panel];
				}
				else
				{
					list = new List<EventDebuggerTrace>();
					this.m_EventProcessedEvents.Add(this.panel, list);
				}
				list.Add(eventDebuggerTrace2);
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
				string text2 = obj.GetType().Name;
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

		private Dictionary<IPanel, List<EventDebuggerCallTrace>> m_EventCalledObjects;

		private Dictionary<IPanel, List<EventDebuggerDefaultActionTrace>> m_EventDefaultActionObjects;

		private Dictionary<IPanel, List<EventDebuggerPathTrace>> m_EventPathObjects;

		private Dictionary<IPanel, List<EventDebuggerTrace>> m_EventProcessedEvents;

		private Dictionary<IPanel, Stack<EventDebuggerTrace>> m_StackOfProcessedEvent;

		private readonly Dictionary<IPanel, long> m_ModificationCount;

		private readonly bool m_Log;
	}
}
