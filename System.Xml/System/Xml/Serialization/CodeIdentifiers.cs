using System;
using System.Collections;
using System.Globalization;

namespace System.Xml.Serialization
{
	public class CodeIdentifiers
	{
		public CodeIdentifiers()
			: this(true)
		{
		}

		public CodeIdentifiers(bool caseSensitive)
		{
			StringComparer stringComparer = ((!caseSensitive) ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
			this.table = new Hashtable(stringComparer);
			this.reserved = new Hashtable(stringComparer);
		}

		public bool UseCamelCasing
		{
			get
			{
				return this.useCamelCasing;
			}
			set
			{
				this.useCamelCasing = value;
			}
		}

		public void Add(string identifier, object value)
		{
			this.table.Add(identifier, value);
		}

		public void AddReserved(string identifier)
		{
			this.reserved.Add(identifier, identifier);
		}

		public string AddUnique(string identifier, object value)
		{
			string text = this.MakeUnique(identifier);
			this.Add(text, value);
			return text;
		}

		public void Clear()
		{
			this.table.Clear();
		}

		public bool IsInUse(string identifier)
		{
			return this.table.ContainsKey(identifier) || this.reserved.ContainsKey(identifier);
		}

		public string MakeRightCase(string identifier)
		{
			if (this.UseCamelCasing)
			{
				return CodeIdentifier.MakeCamel(identifier);
			}
			return CodeIdentifier.MakePascal(identifier);
		}

		public string MakeUnique(string identifier)
		{
			string text = identifier;
			int num = 1;
			while (this.IsInUse(text))
			{
				text = string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[] { identifier, num });
				num++;
			}
			return text;
		}

		public void Remove(string identifier)
		{
			this.table.Remove(identifier);
		}

		public void RemoveReserved(string identifier)
		{
			this.reserved.Remove(identifier);
		}

		public object ToArray(Type type)
		{
			Array array = Array.CreateInstance(type, this.table.Count);
			this.table.CopyTo(array, 0);
			return array;
		}

		private bool useCamelCasing;

		private Hashtable table;

		private Hashtable reserved;
	}
}
