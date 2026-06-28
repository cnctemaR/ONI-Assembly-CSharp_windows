using System;
using System.Collections.Generic;

public class UIFloatFormatter
{
	public string Format(string format, float value)
	{
		return this.Replace(format, "{0}", value);
	}

	private string Replace(string format, string key, float value)
	{
		UIFloatFormatter.Entry entry = default(UIFloatFormatter.Entry);
		if (this.activeStringCount >= this.entries.Count)
		{
			entry.format = format;
			entry.key = key;
			entry.value = value;
			entry.result = entry.format.Replace(key, value.ToString());
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
				entry.result = entry.format.Replace(key, value.ToString());
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

	private List<UIFloatFormatter.Entry> entries = new List<UIFloatFormatter.Entry>();

	private struct Entry
	{
		public string format;

		public string key;

		public float value;

		public string result;
	}
}
