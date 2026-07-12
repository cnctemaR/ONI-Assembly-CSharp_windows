using System;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	public class TimeNotificationBehaviour : PlayableBehaviour
	{
		public Playable timeSource
		{
			set
			{
				this.m_TimeSource = value;
			}
		}

		public static ScriptPlayable<TimeNotificationBehaviour> Create(PlayableGraph graph, double duration, DirectorWrapMode loopMode)
		{
			ScriptPlayable<TimeNotificationBehaviour> scriptPlayable = ScriptPlayable<TimeNotificationBehaviour>.Create(graph, 0);
			scriptPlayable.SetDuration(duration);
			scriptPlayable.SetTimeWrapMode(loopMode);
			scriptPlayable.SetPropagateSetTime(true);
			return scriptPlayable;
		}

		public void AddNotification(double time, INotification payload, NotificationFlags flags = NotificationFlags.Retroactive)
		{
			this.m_Notifications.Add(new TimeNotificationBehaviour.NotificationEntry
			{
				time = time,
				payload = payload,
				flags = flags
			});
			this.m_NeedSortNotifications = true;
		}

		public override void OnGraphStart(Playable playable)
		{
			this.SortNotifications();
			double time = playable.GetTime<Playable>();
			for (int i = 0; i < this.m_Notifications.Count; i++)
			{
				if (this.m_Notifications[i].time > time && !this.m_Notifications[i].triggerOnce)
				{
					TimeNotificationBehaviour.NotificationEntry notificationEntry = this.m_Notifications[i];
					notificationEntry.notificationFired = false;
					this.m_Notifications[i] = notificationEntry;
				}
			}
			this.m_PreviousTime = playable.GetTime<Playable>();
		}

		public override void OnBehaviourPause(Playable playable, FrameData info)
		{
			if (playable.IsDone<Playable>())
			{
				this.SortNotifications();
				for (int i = 0; i < this.m_Notifications.Count; i++)
				{
					TimeNotificationBehaviour.NotificationEntry notificationEntry = this.m_Notifications[i];
					if (!notificationEntry.notificationFired)
					{
						double duration = playable.GetDuration<Playable>();
						if (this.m_PreviousTime <= notificationEntry.time && notificationEntry.time <= duration)
						{
							TimeNotificationBehaviour.Trigger_internal(playable, info.output, ref notificationEntry);
							this.m_Notifications[i] = notificationEntry;
						}
					}
				}
			}
		}

		public override void PrepareFrame(Playable playable, FrameData info)
		{
			if (info.evaluationType == FrameData.EvaluationType.Evaluate)
			{
				return;
			}
			this.SyncDurationWithExternalSource(playable);
			this.SortNotifications();
			double time = playable.GetTime<Playable>();
			if (info.timeLooped)
			{
				double duration = playable.GetDuration<Playable>();
				this.TriggerNotificationsInRange(this.m_PreviousTime, duration, info, playable, true);
				double num = playable.GetDuration<Playable>() - this.m_PreviousTime;
				int num2 = (int)(((double)(info.deltaTime * info.effectiveSpeed) - num) / playable.GetDuration<Playable>());
				for (int i = 0; i < num2; i++)
				{
					this.TriggerNotificationsInRange(0.0, duration, info, playable, false);
				}
				this.TriggerNotificationsInRange(0.0, time, info, playable, false);
			}
			else
			{
				double time2 = playable.GetTime<Playable>();
				this.TriggerNotificationsInRange(this.m_PreviousTime, time2, info, playable, true);
			}
			for (int j = 0; j < this.m_Notifications.Count; j++)
			{
				TimeNotificationBehaviour.NotificationEntry notificationEntry = this.m_Notifications[j];
				if (notificationEntry.notificationFired && TimeNotificationBehaviour.CanRestoreNotification(notificationEntry, info, time, this.m_PreviousTime))
				{
					TimeNotificationBehaviour.Restore_internal(ref notificationEntry);
					this.m_Notifications[j] = notificationEntry;
				}
			}
			this.m_PreviousTime = playable.GetTime<Playable>();
		}

		private void SortNotifications()
		{
			if (this.m_NeedSortNotifications)
			{
				this.m_Notifications.Sort((TimeNotificationBehaviour.NotificationEntry x, TimeNotificationBehaviour.NotificationEntry y) => x.time.CompareTo(y.time));
				this.m_NeedSortNotifications = false;
			}
		}

		private static bool CanRestoreNotification(TimeNotificationBehaviour.NotificationEntry e, FrameData info, double currentTime, double previousTime)
		{
			return !e.triggerOnce && (info.timeLooped || (previousTime > currentTime && currentTime <= e.time));
		}

		private void TriggerNotificationsInRange(double start, double end, FrameData info, Playable playable, bool checkState)
		{
			if (start <= end)
			{
				bool isPlaying = Application.isPlaying;
				for (int i = 0; i < this.m_Notifications.Count; i++)
				{
					TimeNotificationBehaviour.NotificationEntry notificationEntry = this.m_Notifications[i];
					if (!notificationEntry.notificationFired || (!checkState && !notificationEntry.triggerOnce))
					{
						double time = notificationEntry.time;
						if (notificationEntry.prewarm && time < end && (notificationEntry.triggerInEditor || isPlaying))
						{
							TimeNotificationBehaviour.Trigger_internal(playable, info.output, ref notificationEntry);
							this.m_Notifications[i] = notificationEntry;
						}
						else if (time >= start && time <= end && (notificationEntry.triggerInEditor || isPlaying))
						{
							TimeNotificationBehaviour.Trigger_internal(playable, info.output, ref notificationEntry);
							this.m_Notifications[i] = notificationEntry;
						}
					}
				}
			}
		}

		private void SyncDurationWithExternalSource(Playable playable)
		{
			if (this.m_TimeSource.IsValid<Playable>())
			{
				playable.SetDuration(this.m_TimeSource.GetDuration<Playable>());
				playable.SetTimeWrapMode(this.m_TimeSource.GetTimeWrapMode<Playable>());
			}
		}

		private static void Trigger_internal(Playable playable, PlayableOutput output, ref TimeNotificationBehaviour.NotificationEntry e)
		{
			output.PushNotification(playable, e.payload, null);
			e.notificationFired = true;
		}

		private static void Restore_internal(ref TimeNotificationBehaviour.NotificationEntry e)
		{
			e.notificationFired = false;
		}

		private readonly List<TimeNotificationBehaviour.NotificationEntry> m_Notifications = new List<TimeNotificationBehaviour.NotificationEntry>();

		private double m_PreviousTime;

		private bool m_NeedSortNotifications;

		private Playable m_TimeSource;

		private struct NotificationEntry
		{
			public bool triggerInEditor
			{
				get
				{
					return (this.flags & NotificationFlags.TriggerInEditMode) > (NotificationFlags)0;
				}
			}

			public bool prewarm
			{
				get
				{
					return (this.flags & NotificationFlags.Retroactive) > (NotificationFlags)0;
				}
			}

			public bool triggerOnce
			{
				get
				{
					return (this.flags & NotificationFlags.TriggerOnce) > (NotificationFlags)0;
				}
			}

			public double time;

			public INotification payload;

			public bool notificationFired;

			public NotificationFlags flags;
		}
	}
}
