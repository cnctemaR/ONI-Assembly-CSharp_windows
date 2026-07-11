using System;
using System.Collections;
using System.ComponentModel;
using System.Security.Permissions;
using System.Timers;

namespace Microsoft.Win32
{
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public sealed class SystemEvents
	{
		private SystemEvents()
		{
		}

		[global::System.MonoTODO]
		public static event EventHandler DisplaySettingsChanged
		{
			add
			{
			}
			remove
			{
			}
		}

		[global::System.MonoTODO("Currently does nothing on Mono")]
		public static event EventHandler DisplaySettingsChanging
		{
			add
			{
			}
			remove
			{
			}
		}

		[global::System.MonoTODO("Currently does nothing on Mono")]
		public static event EventHandler EventsThreadShutdown
		{
			add
			{
			}
			remove
			{
			}
		}

		[global::System.MonoTODO("Currently does nothing on Mono")]
		public static event EventHandler InstalledFontsChanged
		{
			add
			{
			}
			remove
			{
			}
		}

		[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
		[global::System.MonoTODO("Currently does nothing on Mono")]
		[Obsolete("")]
		[global::System.ComponentModel.Browsable(false)]
		public static event EventHandler LowMemory
		{
			add
			{
			}
			remove
			{
			}
		}

		[global::System.MonoTODO("Currently does nothing on Mono")]
		public static event EventHandler PaletteChanged
		{
			add
			{
			}
			remove
			{
			}
		}

		[global::System.MonoTODO("Currently does nothing on Mono")]
		public static event PowerModeChangedEventHandler PowerModeChanged
		{
			add
			{
			}
			remove
			{
			}
		}

		[global::System.MonoTODO("Currently does nothing on Mono")]
		public static event SessionEndedEventHandler SessionEnded
		{
			add
			{
			}
			remove
			{
			}
		}

		[global::System.MonoTODO("Currently does nothing on Mono")]
		public static event SessionEndingEventHandler SessionEnding
		{
			add
			{
			}
			remove
			{
			}
		}

		[global::System.MonoTODO("Currently does nothing on Mono")]
		public static event SessionSwitchEventHandler SessionSwitch
		{
			add
			{
			}
			remove
			{
			}
		}

		[global::System.MonoTODO("Currently does nothing on Mono")]
		public static event EventHandler TimeChanged
		{
			add
			{
			}
			remove
			{
			}
		}

		public static event TimerElapsedEventHandler TimerElapsed;

		[global::System.MonoTODO("Currently does nothing on Mono")]
		public static event UserPreferenceChangedEventHandler UserPreferenceChanged
		{
			add
			{
			}
			remove
			{
			}
		}

		[global::System.MonoTODO("Currently does nothing on Mono")]
		public static event UserPreferenceChangingEventHandler UserPreferenceChanging
		{
			add
			{
			}
			remove
			{
			}
		}

		public static IntPtr CreateTimer(int interval)
		{
			int hashCode = Guid.NewGuid().GetHashCode();
			global::System.Timers.Timer timer = new global::System.Timers.Timer((double)interval);
			timer.Elapsed += SystemEvents.InternalTimerElapsed;
			SystemEvents.TimerStore.Add(hashCode, timer);
			return new IntPtr(hashCode);
		}

		public static void KillTimer(IntPtr timerId)
		{
			global::System.Timers.Timer timer = (global::System.Timers.Timer)SystemEvents.TimerStore[timerId.GetHashCode()];
			timer.Stop();
			timer.Elapsed -= SystemEvents.InternalTimerElapsed;
			timer.Dispose();
			SystemEvents.TimerStore.Remove(timerId.GetHashCode());
		}

		private static void InternalTimerElapsed(object e, global::System.Timers.ElapsedEventArgs args)
		{
			if (SystemEvents.TimerElapsed != null)
			{
				SystemEvents.TimerElapsed(null, new TimerElapsedEventArgs(IntPtr.Zero));
			}
		}

		[global::System.MonoTODO]
		public static void InvokeOnEventsThread(Delegate method)
		{
			throw new NotImplementedException();
		}

		private static Hashtable TimerStore = new Hashtable();
	}
}
