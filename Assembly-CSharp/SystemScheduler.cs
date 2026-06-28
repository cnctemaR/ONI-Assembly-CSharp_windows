using System;
using System.Collections.Generic;
using System.Diagnostics;

public class SystemScheduler
{
	private SystemScheduler()
	{
		this.timer = new Stopwatch();
		this.timer.Start();
	}

	public static void Initialize()
	{
		SystemScheduler.instance = new SystemScheduler();
		for (int i = 0; i < 3; i++)
		{
			SystemScheduler.instance.prioritizedEntries[i] = new List<SystemScheduler.Entry>();
		}
	}

	public static void Shutdown()
	{
		SystemScheduler.instance = null;
	}

	public void Update()
	{
		long elapsedMilliseconds = this.timer.ElapsedMilliseconds;
		long num;
		if (elapsedMilliseconds <= 16L)
		{
			num = Math.Min(8L, 16L - elapsedMilliseconds);
		}
		else
		{
			num = Math.Min(16L, 33L - elapsedMilliseconds);
		}
		num = Math.Max(3L, num);
		this.timer.Reset();
		for (int i = 0; i < 3; i++)
		{
			List<SystemScheduler.Entry> list = this.prioritizedEntries[i];
			if (list.Count > 0)
			{
				list.RemoveAll((SystemScheduler.Entry x) => x.callback == null);
				int j;
				for (j = 0; j < list.Count; j++)
				{
					SystemScheduler.Entry entry = list[j];
					if (entry.callback != null)
					{
						entry.callback(entry.data);
					}
					if (this.timer.ElapsedMilliseconds >= num)
					{
						break;
					}
				}
				list.RemoveRange(0, j);
				if (this.timer.ElapsedMilliseconds >= num)
				{
					break;
				}
			}
		}
		this.timer.Reset();
		this.RebuildIndices();
	}

	public Guid AddTask(Guid guid, SystemScheduler.Priority priority, Action<object> callback, object data, string name)
	{
		List<SystemScheduler.Entry> list = this.prioritizedEntries[(int)priority];
		this.indices[guid] = new Pair<SystemScheduler.Priority, int>(priority, list.Count);
		list.Add(new SystemScheduler.Entry
		{
			guid = guid,
			callback = callback,
			data = data,
			name = name
		});
		return guid;
	}

	public bool RemoveTask(Guid guid)
	{
		bool flag = false;
		Pair<SystemScheduler.Priority, int> pair;
		if (this.indices.TryGetValue(guid, out pair))
		{
			List<SystemScheduler.Entry> list = this.prioritizedEntries[(int)pair.first];
			for (int i = 0; i < list.Count; i++)
			{
				SystemScheduler.Entry entry = list[i];
				if (entry.guid == guid)
				{
					entry.guid = Guid.Empty;
					entry.callback = null;
					entry.data = null;
					list[i] = entry;
					flag = true;
				}
			}
		}
		return flag;
	}

	public void Clear()
	{
		for (int i = 0; i < this.prioritizedEntries.Length; i++)
		{
			this.prioritizedEntries[i].Clear();
		}
	}

	private void RebuildIndices()
	{
		this.indices.Clear();
		Pair<SystemScheduler.Priority, int> pair = default(Pair<SystemScheduler.Priority, int>);
		for (int i = 0; i < this.prioritizedEntries.Length; i++)
		{
			SystemScheduler.Priority priority = (SystemScheduler.Priority)i;
			pair.first = priority;
			List<SystemScheduler.Entry> list = this.prioritizedEntries[i];
			for (int j = 0; j < list.Count; j++)
			{
				pair.second = j;
				SystemScheduler.Entry entry = list[j];
				if (entry.guid != Guid.Empty)
				{
					this.indices[entry.guid] = pair;
				}
			}
		}
	}

	public static SystemScheduler instance;

	private List<SystemScheduler.Entry>[] prioritizedEntries = new List<SystemScheduler.Entry>[3];

	private Dictionary<Guid, Pair<SystemScheduler.Priority, int>> indices = new Dictionary<Guid, Pair<SystemScheduler.Priority, int>>();

	private Stopwatch timer;

	public enum Priority
	{
		Highest,
		Default,
		Lowest,
		Count
	}

	[DebuggerDisplay("{data}, {name}, {guid}")]
	private struct Entry
	{
		public Guid guid;

		public Action<object> callback;

		public object data;

		public string name;
	}
}
