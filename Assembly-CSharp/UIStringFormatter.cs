using System;
using System.Collections.Generic;

public class UIStringFormatter
{
	public string Format(string format, string s0)
	{
		return this.Replace(format, "{0}", s0);
	}

	public string Format(string format, string s0, string s1)
	{
		return this.Replace(this.Replace(format, "{0}", s0), "{1}", s1);
	}

	private string Replace(string format, string key, string value)
	{
		UIStringFormatter.Entry entry = default(UIStringFormatter.Entry);
		if (this.activeStringCount >= this.entries.Count)
		{
			entry.format = format;
			entry.key = key;
			entry.value = value;
			entry.result = entry.format.Replace(key, value);
			this.entries.Add(entry);
		}
		else
		{
			entry = this.entries[this.activeStringCount];
			if (entry.format != format || entry.key != key || entry.value != value)
			{
				entry.format = format;
				entry.key = key;
				entry.value = value;
				entry.result = entry.format.Replace(key, value);
				this.entries[this.activeStringCount] = entry;
			}
		}
		this.activeStringCount++;
		return entry.result;
	}

	public void BeginDrawing()
	{
		this.activeStringCount = 0;
	}

	public void EndDrawing()
	{
	}

	private int activeStringCount;

	private List<UIStringFormatter.Entry> entries = new List<UIStringFormatter.Entry>();

	private struct Entry
	{
		public string format;

		public string key;

		public string value;

		public string result;
	}
}
