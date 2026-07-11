using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

public class ProfilerBase
{
	public ProfilerBase(string file_prefix)
	{
		this.filePrefix = file_prefix;
		this.threadInfos = new Dictionary<int, ProfilerBase.ThreadInfo>();
		this.sw = new Stopwatch();
	}

	public static void WriteLine(StringBuilder sb, string category, string region_name, int tid, Stopwatch sw, string ph, string suffix)
	{
		sb.Append("{\"cat\":\"").Append(category).Append("\"");
		sb.Append(",\"name\":\"").Append(region_name).Append("\"");
		sb.Append(",\"pid\":0");
		sb.Append(",\"tid\":").Append(tid);
		long elapsedTicks = sw.ElapsedTicks;
		long frequency = Stopwatch.Frequency;
		long num = elapsedTicks * 1000000L / frequency;
		sb.Append(",\"ts\":").Append(num);
		sb.Append(",\"ph\":\"").Append(ph).Append("\"");
		sb.Append(suffix).Append("\n");
	}

	protected bool IsRecording()
	{
		return this.proFile != null;
	}

	public void Init()
	{
		this.proFile = null;
	}

	public void Finalise()
	{
		if (this.IsRecording())
		{
			this.StopRecording();
		}
	}

	public void ToggleRecording(string category = "GAME")
	{
		this.category = "G";
		if (!this.initialised)
		{
			this.initialised = true;
			this.Init();
		}
		if (this.IsRecording())
		{
			this.StopRecording();
		}
		else
		{
			this.StartRecording();
		}
	}

	public virtual void StartRecording()
	{
		foreach (KeyValuePair<int, ProfilerBase.ThreadInfo> keyValuePair in this.threadInfos)
		{
			keyValuePair.Value.Reset();
		}
		this.proFile = new StreamWriter(this.filePrefix + this.idx.ToString() + ".json");
		this.idx++;
		if (this.proFile != null)
		{
			this.proFile.WriteLine("{\"traceEvents\":[");
		}
		this.sw.Start();
	}

	public virtual void StopRecording()
	{
		this.sw.Stop();
		if (this.proFile == null)
		{
			return;
		}
		foreach (KeyValuePair<int, ProfilerBase.ThreadInfo> keyValuePair in this.threadInfos)
		{
			this.proFile.Write(keyValuePair.Value.sb.ToString());
			keyValuePair.Value.Reset();
		}
		ProfilerBase.ThreadInfo threadInfo = this.ManifestThreadInfo("Main");
		threadInfo.WriteLine(this.category, "end", this.sw, "B", "},");
		threadInfo.WriteLine(this.category, "end", this.sw, "E", "}]}");
		this.proFile.Write(threadInfo.sb.ToString());
		threadInfo.Reset();
		this.proFile.Close();
		this.proFile = null;
	}

	public virtual void BeginThreadProfiling(string threadGroupName, string threadName)
	{
		this.ManifestThreadInfo(threadName);
	}

	public virtual void EndThreadProfiling()
	{
		if (this.proFile != null)
		{
			this.proFile.Write(this.ManifestThreadInfo(null).sb.ToString());
		}
		object obj = this.threadInfos;
		lock (obj)
		{
			this.threadInfos.Remove(Thread.CurrentThread.ManagedThreadId);
		}
	}

	protected ProfilerBase.ThreadInfo ManifestThreadInfo(string name = null)
	{
		ProfilerBase.ThreadInfo threadInfo;
		if (!this.threadInfos.TryGetValue(Thread.CurrentThread.ManagedThreadId, out threadInfo))
		{
			threadInfo = new ProfilerBase.ThreadInfo(Thread.CurrentThread.ManagedThreadId);
			if (name != null)
			{
				threadInfo.name = name;
			}
			global::Debug.LogFormat("ManifestThreadInfo: {0}, {1}", new object[]
			{
				name,
				Thread.CurrentThread.ManagedThreadId
			});
			object obj = this.threadInfos;
			lock (obj)
			{
				this.threadInfos.Add(Thread.CurrentThread.ManagedThreadId, threadInfo);
			}
		}
		if (name != null && threadInfo.name != name)
		{
			global::Debug.LogFormat("ManifestThreadInfo: change name {0} to {1}, {2}", new object[]
			{
				name,
				threadInfo.name,
				Thread.CurrentThread.ManagedThreadId
			});
			threadInfo.name = name;
			object obj2 = this.threadInfos;
			lock (obj2)
			{
				this.threadInfos[threadInfo.id] = threadInfo;
			}
		}
		return threadInfo;
	}

	[Conditional("KPROFILER_VALIDATE_REGION_NAME")]
	private void ValidateRegionName(string region_name)
	{
		DebugUtil.Assert(!region_name.Contains("\""));
		region_name = "InvalidRegionName";
	}

	protected void Push(string region_name, string file, uint line)
	{
		if (!this.IsRecording())
		{
			return;
		}
		ProfilerBase.ThreadInfo threadInfo = this.ManifestThreadInfo(null);
		threadInfo.regionStack.Push(region_name);
		threadInfo.WriteLine(this.category, region_name, this.sw, "B", "},");
	}

	protected void Pop()
	{
		if (!this.IsRecording())
		{
			return;
		}
		ProfilerBase.ThreadInfo threadInfo = this.ManifestThreadInfo(null);
		if (threadInfo.regionStack.Count == 0)
		{
			return;
		}
		threadInfo.WriteLine(this.category, threadInfo.regionStack.Pop(), this.sw, "E", "},");
	}

	private bool initialised;

	private int idx;

	protected StreamWriter proFile;

	private string category = "GAME";

	private string filePrefix;

	protected Dictionary<int, ProfilerBase.ThreadInfo> threadInfos;

	public Stopwatch sw;

	protected struct ThreadInfo
	{
		public ThreadInfo(int id)
		{
			this.regionStack = new Stack<string>();
			this.sb = new StringBuilder();
			this.id = id;
			this.name = string.Empty;
		}

		public void Reset()
		{
			this.regionStack.Clear();
			this.sb.Length = 0;
		}

		public void WriteLine(string category, string region_name, Stopwatch sw, string ph, string suffix)
		{
			ProfilerBase.WriteLine(this.sb, category, region_name, this.id, sw, ph, suffix);
		}

		public Stack<string> regionStack;

		public StringBuilder sb;

		public int id;

		public string name;
	}
}
