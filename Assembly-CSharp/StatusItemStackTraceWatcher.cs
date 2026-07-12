using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class StatusItemStackTraceWatcher : IDisposable
{
	public bool GetShouldWatch()
	{
		return this.shouldWatch;
	}

	public void SetShouldWatch(bool shouldWatch)
	{
		if (this.shouldWatch == shouldWatch)
		{
			return;
		}
		this.shouldWatch = shouldWatch;
		this.Refresh();
	}

	public Option<StatusItemGroup> GetTarget()
	{
		return this.currentTarget;
	}

	public void SetTarget(Option<StatusItemGroup> nextTarget)
	{
		if (this.currentTarget.IsNone() && nextTarget.IsNone())
		{
			return;
		}
		if (this.currentTarget.IsSome() && nextTarget.IsSome() && this.currentTarget.Unwrap() == nextTarget.Unwrap())
		{
			return;
		}
		this.currentTarget = nextTarget;
		this.Refresh();
	}

	private void Refresh()
	{
		if (this.onCleanup != null)
		{
			global::System.Action action = this.onCleanup;
			if (action != null)
			{
				action();
			}
			this.onCleanup = null;
		}
		if (!this.shouldWatch)
		{
			return;
		}
		if (this.currentTarget.IsSome())
		{
			StatusItemGroup target = this.currentTarget.Unwrap();
			Action<StatusItemGroup.Entry, StatusItemCategory> onAddStatusItem = delegate(StatusItemGroup.Entry entry, StatusItemCategory category)
			{
				this.entryIdToStackTraceMap[entry.id] = new StackTrace(true);
			};
			StatusItemGroup target3 = target;
			target3.OnAddStatusItem = (Action<StatusItemGroup.Entry, StatusItemCategory>)Delegate.Combine(target3.OnAddStatusItem, onAddStatusItem);
			this.onCleanup = (global::System.Action)Delegate.Combine(this.onCleanup, new global::System.Action(delegate
			{
				StatusItemGroup target2 = target;
				target2.OnAddStatusItem = (Action<StatusItemGroup.Entry, StatusItemCategory>)Delegate.Remove(target2.OnAddStatusItem, onAddStatusItem);
			}));
			StatusItemStackTraceWatcher.StatusItemStackTraceWatcher_OnDestroyListenerMB destroyListener = this.currentTarget.Unwrap().gameObject.AddOrGet<StatusItemStackTraceWatcher.StatusItemStackTraceWatcher_OnDestroyListenerMB>();
			destroyListener.owner = this;
			this.onCleanup = (global::System.Action)Delegate.Combine(this.onCleanup, new global::System.Action(delegate
			{
				if (destroyListener.IsNullOrDestroyed())
				{
					return;
				}
				global::UnityEngine.Object.Destroy(destroyListener);
			}));
			this.onCleanup = (global::System.Action)Delegate.Combine(this.onCleanup, new global::System.Action(delegate
			{
				this.entryIdToStackTraceMap.Clear();
			}));
		}
	}

	public bool GetStackTraceForEntry(StatusItemGroup.Entry entry, out StackTrace stackTrace)
	{
		return this.entryIdToStackTraceMap.TryGetValue(entry.id, out stackTrace);
	}

	public void Dispose()
	{
		if (this.onCleanup != null)
		{
			global::System.Action action = this.onCleanup;
			if (action != null)
			{
				action();
			}
			this.onCleanup = null;
		}
	}

	private Dictionary<Guid, StackTrace> entryIdToStackTraceMap = new Dictionary<Guid, StackTrace>();

	private Option<StatusItemGroup> currentTarget;

	private bool shouldWatch;

	private global::System.Action onCleanup;

	public class StatusItemStackTraceWatcher_OnDestroyListenerMB : MonoBehaviour
	{
		private void OnDestroy()
		{
			bool flag = this.owner != null;
			bool flag2 = this.owner.currentTarget.IsSome() && this.owner.currentTarget.Unwrap().gameObject == base.gameObject;
			if (flag && flag2)
			{
				this.owner.SetTarget(Option.None);
			}
		}

		public StatusItemStackTraceWatcher owner;
	}
}
