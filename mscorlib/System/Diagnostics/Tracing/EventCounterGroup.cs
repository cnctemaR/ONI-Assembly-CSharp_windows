using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Diagnostics.Tracing
{
	internal class EventCounterGroup
	{
		internal EventCounterGroup(EventSource eventSource)
		{
			this._eventSource = eventSource;
			this._eventCounters = new List<EventCounter>();
			this.RegisterCommandCallback();
		}

		internal void Add(EventCounter eventCounter)
		{
			lock (this)
			{
				this._eventCounters.Add(eventCounter);
			}
		}

		internal void Remove(EventCounter eventCounter)
		{
			lock (this)
			{
				this._eventCounters.Remove(eventCounter);
			}
		}

		private void RegisterCommandCallback()
		{
			this._eventSource.EventCommandExecuted += this.OnEventSourceCommand;
		}

		private void OnEventSourceCommand(object sender, EventCommandEventArgs e)
		{
			string text;
			float num;
			if ((e.Command == EventCommand.Enable || e.Command == EventCommand.Update) && e.Arguments.TryGetValue("EventCounterIntervalSec", out text) && float.TryParse(text, out num))
			{
				lock (this)
				{
					this.EnableTimer(num);
				}
			}
		}

		private static void EnsureEventSourceIndexAvailable(int eventSourceIndex)
		{
			if (EventCounterGroup.s_eventCounterGroups == null)
			{
				EventCounterGroup.s_eventCounterGroups = new WeakReference<EventCounterGroup>[eventSourceIndex + 1];
				return;
			}
			if (eventSourceIndex >= EventCounterGroup.s_eventCounterGroups.Length)
			{
				WeakReference<EventCounterGroup>[] array = new WeakReference<EventCounterGroup>[eventSourceIndex + 1];
				Array.Copy(EventCounterGroup.s_eventCounterGroups, 0, array, 0, EventCounterGroup.s_eventCounterGroups.Length);
				EventCounterGroup.s_eventCounterGroups = array;
			}
		}

		internal static EventCounterGroup GetEventCounterGroup(EventSource eventSource)
		{
			object obj = EventCounterGroup.s_eventCounterGroupsLock;
			EventCounterGroup eventCounterGroup2;
			lock (obj)
			{
				int num = EventListenerHelper.EventSourceIndex(eventSource);
				EventCounterGroup.EnsureEventSourceIndexAvailable(num);
				WeakReference<EventCounterGroup> weakReference = EventCounterGroup.s_eventCounterGroups[num];
				EventCounterGroup eventCounterGroup = null;
				if (weakReference == null || !weakReference.TryGetTarget(out eventCounterGroup))
				{
					eventCounterGroup = new EventCounterGroup(eventSource);
					EventCounterGroup.s_eventCounterGroups[num] = new WeakReference<EventCounterGroup>(eventCounterGroup);
				}
				eventCounterGroup2 = eventCounterGroup;
			}
			return eventCounterGroup2;
		}

		private void DisposeTimer()
		{
			if (this._pollingTimer != null)
			{
				this._pollingTimer.Dispose();
				this._pollingTimer = null;
			}
		}

		private void EnableTimer(float pollingIntervalInSeconds)
		{
			if (pollingIntervalInSeconds <= 0f)
			{
				this.DisposeTimer();
				this._pollingIntervalInMilliseconds = 0;
			}
			else if (this._pollingIntervalInMilliseconds == 0 || pollingIntervalInSeconds * 1000f < (float)this._pollingIntervalInMilliseconds)
			{
				this._pollingIntervalInMilliseconds = (int)(pollingIntervalInSeconds * 1000f);
				this.DisposeTimer();
				this._timeStampSinceCollectionStarted = DateTime.UtcNow;
				this._pollingTimer = new Timer(new TimerCallback(this.OnTimer), null, this._pollingIntervalInMilliseconds, this._pollingIntervalInMilliseconds);
			}
			this.OnTimer(null);
		}

		private void OnTimer(object state)
		{
			lock (this)
			{
				if (this._eventSource.IsEnabled())
				{
					DateTime utcNow = DateTime.UtcNow;
					TimeSpan timeSpan = utcNow - this._timeStampSinceCollectionStarted;
					foreach (EventCounter eventCounter in this._eventCounters)
					{
						EventCounterPayload eventCounterPayload = eventCounter.GetEventCounterPayload();
						eventCounterPayload.IntervalSec = (float)timeSpan.TotalSeconds;
						this._eventSource.Write<EventCounterGroup.PayloadType>("EventCounters", new EventSourceOptions
						{
							Level = EventLevel.LogAlways
						}, new EventCounterGroup.PayloadType(eventCounterPayload));
					}
					this._timeStampSinceCollectionStarted = utcNow;
				}
				else
				{
					this.DisposeTimer();
				}
			}
		}

		private readonly EventSource _eventSource;

		private readonly List<EventCounter> _eventCounters;

		private static WeakReference<EventCounterGroup>[] s_eventCounterGroups;

		private static readonly object s_eventCounterGroupsLock = new object();

		private DateTime _timeStampSinceCollectionStarted;

		private int _pollingIntervalInMilliseconds;

		private Timer _pollingTimer;

		[EventData]
		private class PayloadType
		{
			public PayloadType(EventCounterPayload payload)
			{
				this.Payload = payload;
			}

			public EventCounterPayload Payload { get; set; }
		}
	}
}
