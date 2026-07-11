using System;
using System.Security;
using System.Threading;

namespace System.Diagnostics.Tracing
{
	internal class ActivityTracker
	{
		public void OnStart(string providerName, string activityName, int task, ref Guid activityId, ref Guid relatedActivityId, EventActivityOptions options)
		{
			if (this.m_current == null)
			{
				if (this.m_checkedForEnable)
				{
					return;
				}
				this.m_checkedForEnable = true;
				this.Enable();
				if (this.m_current == null)
				{
					return;
				}
			}
			ActivityTracker.ActivityInfo activityInfo = this.m_current.Value;
			string text = this.NormalizeActivityName(providerName, activityName, task);
			TplEtwProvider log = TplEtwProvider.Log;
			if (log.Debug)
			{
				log.DebugFacilityMessage("OnStartEnter", text);
				log.DebugFacilityMessage("OnStartEnterActivityState", ActivityTracker.ActivityInfo.LiveActivities(activityInfo));
			}
			if (activityInfo != null)
			{
				if (activityInfo.m_level >= 100)
				{
					activityId = Guid.Empty;
					relatedActivityId = Guid.Empty;
					if (log.Debug)
					{
						log.DebugFacilityMessage("OnStartRET", "Fail");
					}
					return;
				}
				if ((options & EventActivityOptions.Recursive) == EventActivityOptions.None && this.FindActiveActivity(text, activityInfo) != null)
				{
					this.OnStop(providerName, activityName, task, ref activityId);
					activityInfo = this.m_current.Value;
				}
			}
			long num;
			if (activityInfo == null)
			{
				num = Interlocked.Increment(ref ActivityTracker.m_nextId);
			}
			else
			{
				num = Interlocked.Increment(ref activityInfo.m_lastChildID);
			}
			relatedActivityId = EventSource.CurrentThreadActivityId;
			ActivityTracker.ActivityInfo activityInfo2 = new ActivityTracker.ActivityInfo(text, num, activityInfo, relatedActivityId, options);
			this.m_current.Value = activityInfo2;
			activityId = activityInfo2.ActivityId;
			if (log.Debug)
			{
				log.DebugFacilityMessage("OnStartRetActivityState", ActivityTracker.ActivityInfo.LiveActivities(activityInfo2));
				log.DebugFacilityMessage1("OnStartRet", activityId.ToString(), relatedActivityId.ToString());
			}
		}

		public void OnStop(string providerName, string activityName, int task, ref Guid activityId)
		{
			if (this.m_current == null)
			{
				return;
			}
			string text = this.NormalizeActivityName(providerName, activityName, task);
			TplEtwProvider log = TplEtwProvider.Log;
			if (log.Debug)
			{
				log.DebugFacilityMessage("OnStopEnter", text);
				log.DebugFacilityMessage("OnStopEnterActivityState", ActivityTracker.ActivityInfo.LiveActivities(this.m_current.Value));
			}
			ActivityTracker.ActivityInfo activityInfo;
			for (;;)
			{
				ActivityTracker.ActivityInfo value = this.m_current.Value;
				activityInfo = null;
				ActivityTracker.ActivityInfo activityInfo2 = this.FindActiveActivity(text, value);
				if (activityInfo2 == null)
				{
					break;
				}
				activityId = activityInfo2.ActivityId;
				ActivityTracker.ActivityInfo activityInfo3 = value;
				while (activityInfo3 != activityInfo2 && activityInfo3 != null)
				{
					if (activityInfo3.m_stopped != 0)
					{
						activityInfo3 = activityInfo3.m_creator;
					}
					else
					{
						if (activityInfo3.CanBeOrphan())
						{
							if (activityInfo == null)
							{
								activityInfo = activityInfo3;
							}
						}
						else
						{
							activityInfo3.m_stopped = 1;
						}
						activityInfo3 = activityInfo3.m_creator;
					}
				}
				if (Interlocked.CompareExchange(ref activityInfo2.m_stopped, 1, 0) == 0)
				{
					goto Block_9;
				}
			}
			activityId = Guid.Empty;
			if (log.Debug)
			{
				log.DebugFacilityMessage("OnStopRET", "Fail");
			}
			return;
			Block_9:
			if (activityInfo == null)
			{
				ActivityTracker.ActivityInfo activityInfo2;
				activityInfo = activityInfo2.m_creator;
			}
			this.m_current.Value = activityInfo;
			if (log.Debug)
			{
				log.DebugFacilityMessage("OnStopRetActivityState", ActivityTracker.ActivityInfo.LiveActivities(activityInfo));
				log.DebugFacilityMessage("OnStopRet", activityId.ToString());
			}
		}

		[SecuritySafeCritical]
		public void Enable()
		{
			if (this.m_current == null)
			{
				this.m_current = new AsyncLocal<ActivityTracker.ActivityInfo>(new Action<AsyncLocalValueChangedArgs<ActivityTracker.ActivityInfo>>(this.ActivityChanging));
			}
		}

		public static ActivityTracker Instance
		{
			get
			{
				return ActivityTracker.s_activityTrackerInstance;
			}
		}

		private Guid CurrentActivityId
		{
			get
			{
				return this.m_current.Value.ActivityId;
			}
		}

		private ActivityTracker.ActivityInfo FindActiveActivity(string name, ActivityTracker.ActivityInfo startLocation)
		{
			for (ActivityTracker.ActivityInfo activityInfo = startLocation; activityInfo != null; activityInfo = activityInfo.m_creator)
			{
				if (name == activityInfo.m_name && activityInfo.m_stopped == 0)
				{
					return activityInfo;
				}
			}
			return null;
		}

		private string NormalizeActivityName(string providerName, string activityName, int task)
		{
			if (activityName.EndsWith("Start"))
			{
				activityName = activityName.Substring(0, activityName.Length - "Start".Length);
			}
			else if (activityName.EndsWith("Stop"))
			{
				activityName = activityName.Substring(0, activityName.Length - "Stop".Length);
			}
			else if (task != 0)
			{
				activityName = "task" + task.ToString();
			}
			return providerName + activityName;
		}

		private void ActivityChanging(AsyncLocalValueChangedArgs<ActivityTracker.ActivityInfo> args)
		{
			ActivityTracker.ActivityInfo activityInfo = args.CurrentValue;
			ActivityTracker.ActivityInfo previousValue = args.PreviousValue;
			if (previousValue != null && previousValue.m_creator == activityInfo && (activityInfo == null || previousValue.m_activityIdToRestore != activityInfo.ActivityId))
			{
				EventSource.SetCurrentThreadActivityId(previousValue.m_activityIdToRestore);
				return;
			}
			while (activityInfo != null)
			{
				if (activityInfo.m_stopped == 0)
				{
					EventSource.SetCurrentThreadActivityId(activityInfo.ActivityId);
					return;
				}
				activityInfo = activityInfo.m_creator;
			}
		}

		private AsyncLocal<ActivityTracker.ActivityInfo> m_current;

		private bool m_checkedForEnable;

		private static ActivityTracker s_activityTrackerInstance = new ActivityTracker();

		private static long m_nextId = 0L;

		private const ushort MAX_ACTIVITY_DEPTH = 100;

		private class ActivityInfo
		{
			public ActivityInfo(string name, long uniqueId, ActivityTracker.ActivityInfo creator, Guid activityIDToRestore, EventActivityOptions options)
			{
				this.m_name = name;
				this.m_eventOptions = options;
				this.m_creator = creator;
				this.m_uniqueId = uniqueId;
				this.m_level = ((creator != null) ? (creator.m_level + 1) : 0);
				this.m_activityIdToRestore = activityIDToRestore;
				this.CreateActivityPathGuid(out this.m_guid, out this.m_activityPathGuidOffset);
			}

			public Guid ActivityId
			{
				get
				{
					return this.m_guid;
				}
			}

			public static string Path(ActivityTracker.ActivityInfo activityInfo)
			{
				if (activityInfo == null)
				{
					return "";
				}
				return ActivityTracker.ActivityInfo.Path(activityInfo.m_creator) + "/" + activityInfo.m_uniqueId;
			}

			public override string ToString()
			{
				string text = "";
				if (this.m_stopped != 0)
				{
					text = ",DEAD";
				}
				return string.Concat(new string[]
				{
					this.m_name,
					"(",
					ActivityTracker.ActivityInfo.Path(this),
					text,
					")"
				});
			}

			public static string LiveActivities(ActivityTracker.ActivityInfo list)
			{
				if (list == null)
				{
					return "";
				}
				return list.ToString() + ";" + ActivityTracker.ActivityInfo.LiveActivities(list.m_creator);
			}

			public bool CanBeOrphan()
			{
				return (this.m_eventOptions & EventActivityOptions.Detachable) != EventActivityOptions.None;
			}

			[SecuritySafeCritical]
			private unsafe void CreateActivityPathGuid(out Guid idRet, out int activityPathGuidOffset)
			{
				fixed (Guid* ptr = &idRet)
				{
					Guid* ptr2 = ptr;
					int num = 0;
					if (this.m_creator != null)
					{
						num = this.m_creator.m_activityPathGuidOffset;
						idRet = this.m_creator.m_guid;
					}
					else
					{
						int domainID = Thread.GetDomainID();
						num = ActivityTracker.ActivityInfo.AddIdToGuid(ptr2, num, (uint)domainID, false);
					}
					activityPathGuidOffset = ActivityTracker.ActivityInfo.AddIdToGuid(ptr2, num, (uint)this.m_uniqueId, false);
					if (12 < activityPathGuidOffset)
					{
						this.CreateOverflowGuid(ptr2);
					}
				}
			}

			[SecurityCritical]
			private unsafe void CreateOverflowGuid(Guid* outPtr)
			{
				for (ActivityTracker.ActivityInfo activityInfo = this.m_creator; activityInfo != null; activityInfo = activityInfo.m_creator)
				{
					if (activityInfo.m_activityPathGuidOffset <= 10)
					{
						uint num = (uint)Interlocked.Increment(ref activityInfo.m_lastChildID);
						*outPtr = activityInfo.m_guid;
						if (ActivityTracker.ActivityInfo.AddIdToGuid(outPtr, activityInfo.m_activityPathGuidOffset, num, true) <= 12)
						{
							break;
						}
					}
				}
			}

			[SecurityCritical]
			private unsafe static int AddIdToGuid(Guid* outPtr, int whereToAddId, uint id, bool overflow = false)
			{
				byte* ptr = (byte*)outPtr;
				byte* ptr2 = ptr + 12;
				ptr += whereToAddId;
				if (ptr2 == ptr)
				{
					return 13;
				}
				if (0U < id && id <= 10U && !overflow)
				{
					ActivityTracker.ActivityInfo.WriteNibble(ref ptr, ptr2, id);
				}
				else
				{
					uint num = 4U;
					if (id <= 255U)
					{
						num = 1U;
					}
					else if (id <= 65535U)
					{
						num = 2U;
					}
					else if (id <= 16777215U)
					{
						num = 3U;
					}
					if (overflow)
					{
						if (ptr2 == ptr + 2)
						{
							return 13;
						}
						ActivityTracker.ActivityInfo.WriteNibble(ref ptr, ptr2, 11U);
					}
					ActivityTracker.ActivityInfo.WriteNibble(ref ptr, ptr2, 12U + (num - 1U));
					if (ptr < ptr2 && *ptr != 0)
					{
						if (id < 4096U)
						{
							*ptr = (byte)(192U + (id >> 8));
							id &= 255U;
						}
						ptr++;
					}
					while (0U < num)
					{
						if (ptr2 == ptr)
						{
							ptr++;
							break;
						}
						*(ptr++) = (byte)id;
						id >>= 8;
						num -= 1U;
					}
				}
				*(int*)(outPtr + (IntPtr)3 * 4 / (IntPtr)sizeof(Guid)) = (int)(*(uint*)outPtr + *(uint*)(outPtr + 4 / sizeof(Guid)) + *(uint*)(outPtr + (IntPtr)2 * 4 / (IntPtr)sizeof(Guid)) + 1503500717U);
				return (int)((long)((byte*)ptr - (byte*)outPtr));
			}

			[SecurityCritical]
			private unsafe static void WriteNibble(ref byte* ptr, byte* endPtr, uint value)
			{
				if (*ptr != 0)
				{
					byte* ptr2 = ptr;
					ptr = ptr2 + 1;
					byte* ptr3 = ptr2;
					*ptr3 |= (byte)value;
					return;
				}
				*ptr = (byte)(value << 4);
			}

			internal readonly string m_name;

			private readonly long m_uniqueId;

			internal readonly Guid m_guid;

			internal readonly int m_activityPathGuidOffset;

			internal readonly int m_level;

			internal readonly EventActivityOptions m_eventOptions;

			internal long m_lastChildID;

			internal int m_stopped;

			internal readonly ActivityTracker.ActivityInfo m_creator;

			internal readonly Guid m_activityIdToRestore;

			private enum NumberListCodes : byte
			{
				End,
				LastImmediateValue = 10,
				PrefixCode,
				MultiByte1
			}
		}
	}
}
