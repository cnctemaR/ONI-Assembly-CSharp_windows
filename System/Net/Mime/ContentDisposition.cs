using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;

namespace System.Net.Mime
{
	public class ContentDisposition
	{
		public ContentDisposition()
			: this("attachment")
		{
		}

		public ContentDisposition(string disposition)
		{
			if (disposition == null)
			{
				throw new ArgumentNullException();
			}
			if (disposition.Length < 1)
			{
				throw new FormatException();
			}
			this.Size = -1L;
			try
			{
				int num = disposition.IndexOf(';');
				if (num < 0)
				{
					this.dispositionType = disposition.Trim();
				}
				else
				{
					string[] array = disposition.Split(new char[] { ';' });
					this.dispositionType = array[0].Trim();
					for (int i = 1; i < array.Length; i++)
					{
						this.Parse(array[i]);
					}
				}
			}
			catch
			{
				throw new FormatException();
			}
		}

		private void Parse(string pair)
		{
			if (pair == null || pair.Length < 0)
			{
				return;
			}
			string[] array = pair.Split(new char[] { '=' });
			if (array.Length == 2)
			{
				this.parameters.Add(array[0].Trim(), array[1].Trim());
				return;
			}
			throw new FormatException();
		}

		public DateTime CreationDate
		{
			get
			{
				if (this.parameters.ContainsKey("creation-date"))
				{
					return DateTime.ParseExact(this.parameters["creation-date"], "dd MMM yyyy HH':'mm':'ss zz00", null);
				}
				return DateTime.MinValue;
			}
			set
			{
				if (value > DateTime.MinValue)
				{
					this.parameters["creation-date"] = value.ToString("dd MMM yyyy HH':'mm':'ss zz00");
				}
				else
				{
					this.parameters.Remove("modification-date");
				}
			}
		}

		public string DispositionType
		{
			get
			{
				return this.dispositionType;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				if (value.Length < 1)
				{
					throw new ArgumentException();
				}
				this.dispositionType = value;
			}
		}

		public string FileName
		{
			get
			{
				return this.parameters["filename"];
			}
			set
			{
				this.parameters["filename"] = value;
			}
		}

		public bool Inline
		{
			get
			{
				return string.Compare(this.dispositionType, "inline", true, CultureInfo.InvariantCulture) == 0;
			}
			set
			{
				if (value)
				{
					this.dispositionType = "inline";
				}
				else
				{
					this.dispositionType = "attachment";
				}
			}
		}

		public DateTime ModificationDate
		{
			get
			{
				if (this.parameters.ContainsKey("modification-date"))
				{
					return DateTime.ParseExact(this.parameters["modification-date"], "dd MMM yyyy HH':'mm':'ss zz00", null);
				}
				return DateTime.MinValue;
			}
			set
			{
				if (value > DateTime.MinValue)
				{
					this.parameters["modification-date"] = value.ToString("dd MMM yyyy HH':'mm':'ss zz00");
				}
				else
				{
					this.parameters.Remove("modification-date");
				}
			}
		}

		public global::System.Collections.Specialized.StringDictionary Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		public DateTime ReadDate
		{
			get
			{
				if (this.parameters.ContainsKey("read-date"))
				{
					return DateTime.ParseExact(this.parameters["read-date"], "dd MMM yyyy HH':'mm':'ss zz00", null);
				}
				return DateTime.MinValue;
			}
			set
			{
				if (value > DateTime.MinValue)
				{
					this.parameters["read-date"] = value.ToString("dd MMM yyyy HH':'mm':'ss zz00");
				}
				else
				{
					this.parameters.Remove("read-date");
				}
			}
		}

		public long Size
		{
			get
			{
				if (this.parameters.ContainsKey("size"))
				{
					return long.Parse(this.parameters["size"]);
				}
				return -1L;
			}
			set
			{
				if (value > -1L)
				{
					this.parameters["size"] = value.ToString();
				}
				else
				{
					this.parameters.Remove("size");
				}
			}
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ContentDisposition);
		}

		private bool Equals(ContentDisposition other)
		{
			return other != null && this.ToString() == other.ToString();
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.DispositionType.ToLower());
			if (this.Parameters != null && this.Parameters.Count > 0)
			{
				foreach (object obj in this.Parameters)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					if (dictionaryEntry.Value != null && dictionaryEntry.Value.ToString().Length > 0)
					{
						stringBuilder.Append("; ");
						stringBuilder.Append(dictionaryEntry.Key);
						stringBuilder.Append("=");
						string text = dictionaryEntry.Key.ToString();
						string text2 = dictionaryEntry.Value.ToString();
						bool flag = (text == "filename" && text2.IndexOf(' ') != -1) || text.EndsWith("date");
						if (flag)
						{
							stringBuilder.Append("\"");
						}
						stringBuilder.Append(text2);
						if (flag)
						{
							stringBuilder.Append("\"");
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		private const string rfc822 = "dd MMM yyyy HH':'mm':'ss zz00";

		private string dispositionType;

		private global::System.Collections.Specialized.StringDictionary parameters = new global::System.Collections.Specialized.StringDictionary();
	}
}
