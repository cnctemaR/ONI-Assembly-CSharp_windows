using System;
using System.Collections.Generic;

public class UIStringFormatter
{
	private List<UIStringFormatter.Entry> entries = new List<UIStringFormatter.Entry>();

	private struct Entry
	{
		public string format;

		public string key;

		public string value;

		public string result;
	}
}
