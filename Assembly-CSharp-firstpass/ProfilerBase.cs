using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

public class ProfilerBase
{
	public ProfilerBase(string file_prefix)
	{
		this.filePrefix = file_prefix;
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

	public void StartRecording()
	{
		this.regionStack.Clear();
		this.proFile = new StreamWriter(this.filePrefix + this.idx.ToString() + ".json");
		this.idx++;
		this.sw = Stopwatch.StartNew();
		if (this.proFile != null)
		{
			this.proFile.WriteLine("{\"traceEvents\":[");
		}
	}

	public void StopRecording()
	{
		if (this.proFile == null)
		{
			return;
		}
		this.WriteLine("end", "B", "},");
		this.WriteLine("end", "E", "}]}");
		this.proFile.Close();
		this.proFile = null;
	}

	protected void Push(string region_name, string file, uint line)
	{
		if (!this.IsRecording())
		{
			return;
		}
		this.regionStack.Push(region_name);
		if (this.proFile != null)
		{
			this.WriteLine(region_name, "B", "},");
		}
	}

	protected void Pop()
	{
		if (!this.IsRecording() || this.regionStack.Count == 0)
		{
			return;
		}
		this.WriteLine(this.regionStack.Pop(), "E", "},");
	}

	protected void WriteLine(string region_name, string ph, string suffix)
	{
		if (this.proFile != null)
		{
			this.sb.Append("{\"cat\":\"");
			this.sb.Append(this.category);
			this.sb.Append("\",\"name\":\"");
			this.sb.Append(region_name);
			this.sb.Append("\",\"pid\":0");
			this.sb.Append(",\"ts\":");
			long elapsedTicks = this.sw.ElapsedTicks;
			long frequency = Stopwatch.Frequency;
			long num = elapsedTicks * 1000000L / frequency;
			this.sb.Append(num.ToString());
			this.sb.Append(",\"ph\":\"");
			this.sb.Append(ph);
			this.sb.Append("\"");
			this.sb.Append(suffix);
			this.proFile.WriteLine(this.sb.ToString());
			this.sb.Length = 0;
		}
	}

	private bool initialised;

	private int idx;

	protected StreamWriter proFile;

	private Stopwatch sw;

	private Stack<string> regionStack = new Stack<string>();

	private StringBuilder sb = new StringBuilder();

	private string category = "GAME";

	private string filePrefix;
}
