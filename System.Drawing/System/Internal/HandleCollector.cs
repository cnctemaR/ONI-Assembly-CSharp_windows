using System;
using System.Threading;

namespace System.Internal
{
	internal sealed class HandleCollector
	{
		internal static event HandleChangeEventHandler HandleAdded;

		internal static event HandleChangeEventHandler HandleRemoved;

		internal static IntPtr Add(IntPtr handle, int type)
		{
			HandleCollector.s_handleTypes[type - 1].Add(handle);
			return handle;
		}

		internal static int RegisterType(string typeName, int expense, int initialThreshold)
		{
			object obj = HandleCollector.s_internalSyncObject;
			int num;
			lock (obj)
			{
				if (HandleCollector.s_handleTypeCount == 0 || HandleCollector.s_handleTypeCount == HandleCollector.s_handleTypes.Length)
				{
					HandleCollector.HandleType[] array = new HandleCollector.HandleType[HandleCollector.s_handleTypeCount + 10];
					if (HandleCollector.s_handleTypes != null)
					{
						Array.Copy(HandleCollector.s_handleTypes, 0, array, 0, HandleCollector.s_handleTypeCount);
					}
					HandleCollector.s_handleTypes = array;
				}
				HandleCollector.s_handleTypes[HandleCollector.s_handleTypeCount++] = new HandleCollector.HandleType(typeName, expense, initialThreshold);
				num = HandleCollector.s_handleTypeCount;
			}
			return num;
		}

		internal static IntPtr Remove(IntPtr handle, int type)
		{
			return HandleCollector.s_handleTypes[type - 1].Remove(handle);
		}

		private static HandleCollector.HandleType[] s_handleTypes;

		private static int s_handleTypeCount;

		private static object s_internalSyncObject = new object();

		private class HandleType
		{
			internal HandleType(string name, int expense, int initialThreshHold)
			{
				this.name = name;
				this._initialThreshHold = initialThreshHold;
				this._threshHold = initialThreshHold;
				this._deltaPercent = 100 - expense;
			}

			internal void Add(IntPtr handle)
			{
				if (handle == IntPtr.Zero)
				{
					return;
				}
				bool flag = false;
				int num = 0;
				lock (this)
				{
					this._handleCount++;
					flag = this.NeedCollection();
					num = this._handleCount;
				}
				object s_internalSyncObject = HandleCollector.s_internalSyncObject;
				lock (s_internalSyncObject)
				{
					HandleChangeEventHandler handleAdded = HandleCollector.HandleAdded;
					if (handleAdded != null)
					{
						handleAdded(this.name, handle, num);
					}
				}
				if (!flag)
				{
					return;
				}
				if (flag)
				{
					GC.Collect();
					Thread.Sleep((100 - this._deltaPercent) / 4);
				}
			}

			internal bool NeedCollection()
			{
				if (this._handleCount > this._threshHold)
				{
					this._threshHold = this._handleCount + this._handleCount * this._deltaPercent / 100;
					return true;
				}
				int num = 100 * this._threshHold / (100 + this._deltaPercent);
				if (num >= this._initialThreshHold && this._handleCount < (int)((float)num * 0.9f))
				{
					this._threshHold = num;
				}
				return false;
			}

			internal IntPtr Remove(IntPtr handle)
			{
				if (handle == IntPtr.Zero)
				{
					return handle;
				}
				int num = 0;
				lock (this)
				{
					this._handleCount--;
					if (this._handleCount < 0)
					{
						this._handleCount = 0;
					}
					num = this._handleCount;
				}
				object s_internalSyncObject = HandleCollector.s_internalSyncObject;
				lock (s_internalSyncObject)
				{
					HandleChangeEventHandler handleRemoved = HandleCollector.HandleRemoved;
					if (handleRemoved != null)
					{
						handleRemoved(this.name, handle, num);
					}
				}
				return handle;
			}

			internal readonly string name;

			private int _initialThreshHold;

			private int _threshHold;

			private int _handleCount;

			private readonly int _deltaPercent;
		}
	}
}
