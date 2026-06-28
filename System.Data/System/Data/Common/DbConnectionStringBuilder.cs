using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Threading;

namespace System.Data.Common
{
	public class DbConnectionStringBuilder : IEnumerable, ICustomTypeDescriptor, ICollection, IDictionary
	{
		public DbConnectionStringBuilder()
			: this(false)
		{
		}

		public DbConnectionStringBuilder(bool useOdbcRules)
		{
			this.useOdbcRules = useOdbcRules;
			this._dictionary = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		object IDictionary.this[object keyword]
		{
			get
			{
				return this[(string)keyword];
			}
			set
			{
				this[(string)keyword] = value;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			KeyValuePair<string, object>[] array2 = array as KeyValuePair<string, object>[];
			if (array2 == null)
			{
				throw new ArgumentException("Target array type is not compatible with the type of items in the collection");
			}
			((ICollection<KeyValuePair<string, object>>)this._dictionary).CopyTo(array2, index);
		}

		void IDictionary.Add(object keyword, object value)
		{
			this.Add((string)keyword, value);
		}

		bool IDictionary.Contains(object keyword)
		{
			return this.ContainsKey((string)keyword);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return this._dictionary.GetEnumerator();
		}

		void IDictionary.Remove(object keyword)
		{
			this.Remove((string)keyword);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._dictionary.GetEnumerator();
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			object obj = DbConnectionStringBuilder._staticAttributeCollection;
			if (obj == null)
			{
				CLSCompliantAttribute clscompliantAttribute = new CLSCompliantAttribute(true);
				DefaultMemberAttribute defaultMemberAttribute = new DefaultMemberAttribute("Item");
				Attribute[] array = new Attribute[] { clscompliantAttribute, defaultMemberAttribute };
				obj = new AttributeCollection(array);
			}
			Interlocked.CompareExchange(ref DbConnectionStringBuilder._staticAttributeCollection, obj, null);
			return DbConnectionStringBuilder._staticAttributeCollection as AttributeCollection;
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			return base.GetType().ToString();
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			return null;
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return new CollectionConverter();
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return null;
		}

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return null;
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return null;
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return EventDescriptorCollection.Empty;
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return EventDescriptorCollection.Empty;
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return PropertyDescriptorCollection.Empty;
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			return PropertyDescriptorCollection.Empty;
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			throw new NotImplementedException();
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[DesignOnly(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool BrowsableConnectionString
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		public string ConnectionString
		{
			get
			{
				IDictionary<string, object> dictionary = this._dictionary;
				StringBuilder stringBuilder = new StringBuilder();
				foreach (object obj in this.Keys)
				{
					string text = (string)obj;
					object obj2 = null;
					if (dictionary.TryGetValue(text, out obj2))
					{
						string text2 = obj2.ToString();
						DbConnectionStringBuilder.AppendKeyValuePair(stringBuilder, text, text2, this.useOdbcRules);
					}
				}
				return stringBuilder.ToString();
			}
			set
			{
				this.Clear();
				if (value == null)
				{
					return;
				}
				if (value.Trim().Length == 0)
				{
					return;
				}
				this.ParseConnectionString(value);
			}
		}

		[Browsable(false)]
		public virtual int Count
		{
			get
			{
				return this._dictionary.Count;
			}
		}

		[Browsable(false)]
		public virtual bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		[Browsable(false)]
		public bool IsReadOnly
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[Browsable(false)]
		public virtual object this[string keyword]
		{
			get
			{
				if (this.ContainsKey(keyword))
				{
					return this._dictionary[keyword];
				}
				throw new ArgumentException(string.Format("Keyword '{0}' does not exist", keyword));
			}
			set
			{
				if (value == null)
				{
					this.Remove(keyword);
					return;
				}
				if (keyword == null)
				{
					throw new ArgumentNullException("keyword");
				}
				if (keyword.Length == 0)
				{
					throw DbConnectionStringBuilder.CreateInvalidKeywordException(keyword);
				}
				for (int i = 0; i < keyword.Length; i++)
				{
					char c = keyword[i];
					if (i == 0 && (char.IsWhiteSpace(c) || c == ';'))
					{
						throw DbConnectionStringBuilder.CreateInvalidKeywordException(keyword);
					}
					if (i == keyword.Length - 1 && char.IsWhiteSpace(c))
					{
						throw DbConnectionStringBuilder.CreateInvalidKeywordException(keyword);
					}
					if (char.IsControl(c))
					{
						throw DbConnectionStringBuilder.CreateInvalidKeywordException(keyword);
					}
				}
				if (this.ContainsKey(keyword))
				{
					this._dictionary[keyword] = value;
				}
				else
				{
					this._dictionary.Add(keyword, value);
				}
			}
		}

		[Browsable(false)]
		public virtual ICollection Keys
		{
			get
			{
				string[] array = new string[this._dictionary.Keys.Count];
				((ICollection<string>)this._dictionary.Keys).CopyTo(array, 0);
				return new ReadOnlyCollection<string>(array);
			}
		}

		[Browsable(false)]
		public virtual ICollection Values
		{
			get
			{
				object[] array = new object[this._dictionary.Values.Count];
				((ICollection<object>)this._dictionary.Values).CopyTo(array, 0);
				return new ReadOnlyCollection<object>(array);
			}
		}

		public void Add(string keyword, object value)
		{
			this[keyword] = value;
		}

		public static void AppendKeyValuePair(StringBuilder builder, string keyword, string value, bool useOdbcRules)
		{
			if (builder == null)
			{
				throw new ArgumentNullException("builder");
			}
			if (keyword == null)
			{
				throw new ArgumentNullException("keyName");
			}
			if (keyword.Length == 0)
			{
				throw new ArgumentException("Empty keyword is not valid.");
			}
			if (builder.Length > 0)
			{
				builder.Append(';');
			}
			if (!useOdbcRules)
			{
				builder.Append(keyword.Replace("=", "=="));
			}
			else
			{
				builder.Append(keyword);
			}
			builder.Append('=');
			if (value == null || value.Length == 0)
			{
				return;
			}
			if (!useOdbcRules)
			{
				bool flag = value.IndexOf('"') > -1;
				bool flag2 = value.IndexOf('\'') > -1;
				if (flag && flag2)
				{
					builder.Append('"');
					builder.Append(value.Replace("\"", "\"\""));
					builder.Append('"');
				}
				else if (flag)
				{
					builder.Append('\'');
					builder.Append(value);
					builder.Append('\'');
				}
				else if (flag2 || value.IndexOf('=') > -1 || value.IndexOf(';') > -1)
				{
					builder.Append('"');
					builder.Append(value);
					builder.Append('"');
				}
				else if (DbConnectionStringBuilder.ValueNeedsQuoting(value))
				{
					builder.Append('"');
					builder.Append(value);
					builder.Append('"');
				}
				else
				{
					builder.Append(value);
				}
			}
			else
			{
				int num = 0;
				bool flag3 = false;
				int length = value.Length;
				bool flag4 = false;
				int num2 = -1;
				int i = 0;
				while (i < length)
				{
					int num3;
					if (i == length - 1)
					{
						num3 = -1;
					}
					else
					{
						num3 = (int)value[i + 1];
					}
					char c = value[i];
					char c2 = c;
					switch (c2)
					{
					case '{':
						num++;
						goto IL_0237;
					default:
						if (c2 != ';')
						{
							goto IL_0237;
						}
						flag3 = true;
						goto IL_0237;
					case '}':
						if (!num3.Equals((int)c))
						{
							num--;
							if (num3 != -1)
							{
								flag4 = true;
							}
							goto IL_0237;
						}
						i++;
						break;
					}
					IL_023B:
					i++;
					continue;
					IL_0237:
					num2 = (int)c;
					goto IL_023B;
				}
				if (value[0] == '{' && (num2 != 125 || (num == 0 && flag4)))
				{
					builder.Append('{');
					builder.Append(value.Replace("}", "}}"));
					builder.Append('}');
					return;
				}
				bool flag5 = string.Compare(keyword, "Driver", StringComparison.InvariantCultureIgnoreCase) == 0;
				if (flag5)
				{
					if (value[0] == '{' && num2 == 125 && !flag4)
					{
						builder.Append(value);
						return;
					}
					builder.Append('{');
					builder.Append(value.Replace("}", "}}"));
					builder.Append('}');
					return;
				}
				else
				{
					if (value[0] == '{' && (num != 0 || num2 != 125) && flag4)
					{
						builder.Append('{');
						builder.Append(value.Replace("}", "}}"));
						builder.Append('}');
						return;
					}
					if (value[0] != '{' && flag3)
					{
						builder.Append('{');
						builder.Append(value.Replace("}", "}}"));
						builder.Append('}');
						return;
					}
					builder.Append(value);
				}
			}
		}

		public static void AppendKeyValuePair(StringBuilder builder, string keyword, string value)
		{
			DbConnectionStringBuilder.AppendKeyValuePair(builder, keyword, value, false);
		}

		public virtual void Clear()
		{
			this._dictionary.Clear();
		}

		public virtual bool ContainsKey(string keyword)
		{
			if (keyword == null)
			{
				throw new ArgumentNullException("keyword");
			}
			return this._dictionary.ContainsKey(keyword);
		}

		public virtual bool EquivalentTo(DbConnectionStringBuilder connectionStringBuilder)
		{
			bool flag = true;
			try
			{
				if (this.Count != connectionStringBuilder.Count)
				{
					flag = false;
				}
				else
				{
					foreach (object obj in this.Keys)
					{
						string text = (string)obj;
						if (!this[text].Equals(connectionStringBuilder[text]))
						{
							flag = false;
							break;
						}
					}
				}
			}
			catch (ArgumentException)
			{
				flag = false;
			}
			return flag;
		}

		[MonoTODO]
		protected virtual void GetProperties(Hashtable propertyDescriptors)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected internal void ClearPropertyDescriptors()
		{
			throw new NotImplementedException();
		}

		public virtual bool Remove(string keyword)
		{
			if (keyword == null)
			{
				throw new ArgumentNullException("keyword");
			}
			return this._dictionary.Remove(keyword);
		}

		public virtual bool ShouldSerialize(string keyword)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return this.ConnectionString;
		}

		public virtual bool TryGetValue(string keyword, out object value)
		{
			bool flag = this.ContainsKey(keyword);
			if (flag)
			{
				value = this[keyword];
			}
			else
			{
				value = null;
			}
			return flag;
		}

		private static ArgumentException CreateInvalidKeywordException(string keyword)
		{
			return new ArgumentException("A keyword cannot contain control characters, leading semicolons or leading or trailing whitespace.", keyword);
		}

		private static ArgumentException CreateConnectionStringInvalidException(int index)
		{
			return new ArgumentException("Format of initialization string does not conform to specifications at index " + index + ".");
		}

		private static bool ValueNeedsQuoting(string value)
		{
			foreach (char c in value)
			{
				if (char.IsWhiteSpace(c))
				{
					return true;
				}
			}
			return false;
		}

		private void ParseConnectionString(string connectionString)
		{
			if (this.useOdbcRules)
			{
				this.ParseConnectionStringOdbc(connectionString);
			}
			else
			{
				this.ParseConnectionStringNonOdbc(connectionString);
			}
		}

		private void ParseConnectionStringOdbc(string connectionString)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = true;
			bool flag4 = false;
			string text = string.Empty;
			string text2 = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			int length = connectionString.Length;
			for (int i = 0; i < length; i++)
			{
				char c = connectionString[i];
				int num = ((i != length - 1) ? ((int)connectionString[i + 1]) : (-1));
				char c2 = c;
				switch (c2)
				{
				case ';':
					if (flag3 || flag4)
					{
						stringBuilder.Append(c);
					}
					else
					{
						if (text.Length > 0 && stringBuilder.Length > 0)
						{
							text2 = stringBuilder.ToString();
							text = text.ToLower().TrimEnd(new char[0]);
							this[text] = text2;
						}
						else if (stringBuilder.Length > 0)
						{
							throw DbConnectionStringBuilder.CreateConnectionStringInvalidException((int)c);
						}
						flag3 = true;
						text = string.Empty;
						stringBuilder.Length = 0;
					}
					break;
				default:
					switch (c2)
					{
					case '{':
						if (flag3)
						{
							stringBuilder.Append(c);
							goto IL_0297;
						}
						if (stringBuilder.Length == 0)
						{
							flag4 = true;
						}
						stringBuilder.Append(c);
						goto IL_0297;
					case '}':
						if (flag3 || !flag4)
						{
							stringBuilder.Append(c);
							goto IL_0297;
						}
						if (num == -1)
						{
							stringBuilder.Append(c);
							flag4 = false;
						}
						else if (num.Equals((int)c))
						{
							stringBuilder.Append(c);
							stringBuilder.Append(c);
							i++;
						}
						else
						{
							int num2 = DbConnectionStringBuilder.NextNonWhitespaceChar(connectionString, i);
							if (num2 != -1 && (ushort)num2 != 59)
							{
								throw DbConnectionStringBuilder.CreateConnectionStringInvalidException(num2);
							}
							stringBuilder.Append(c);
							flag4 = false;
						}
						goto IL_0297;
					}
					if (flag2 || flag || flag4)
					{
						stringBuilder.Append(c);
					}
					else if (char.IsWhiteSpace(c))
					{
						if (stringBuilder.Length > 0)
						{
							int num3 = DbConnectionStringBuilder.SkipTrailingWhitespace(connectionString, i);
							if (num3 == -1)
							{
								stringBuilder.Append(c);
							}
							else
							{
								i = num3;
							}
						}
					}
					else
					{
						stringBuilder.Append(c);
					}
					break;
				case '=':
					if (flag4 || !flag3)
					{
						stringBuilder.Append(c);
					}
					else
					{
						text = stringBuilder.ToString();
						if (text.Length == 0)
						{
							throw DbConnectionStringBuilder.CreateConnectionStringInvalidException((int)c);
						}
						stringBuilder.Length = 0;
						flag3 = false;
					}
					break;
				}
				IL_0297:;
			}
			if ((flag3 && stringBuilder.Length > 0) || flag2 || flag || flag4)
			{
				throw DbConnectionStringBuilder.CreateConnectionStringInvalidException(length - 1);
			}
			if (text.Length > 0 && stringBuilder.Length > 0)
			{
				text2 = stringBuilder.ToString();
				text = text.ToLower().TrimEnd(new char[0]);
				this[text] = text2;
			}
		}

		private void ParseConnectionStringNonOdbc(string connectionString)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = true;
			string text = string.Empty;
			string text2 = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			int length = connectionString.Length;
			for (int i = 0; i < length; i++)
			{
				char c = connectionString[i];
				int num = ((i != length - 1) ? ((int)connectionString[i + 1]) : (-1));
				char c2 = c;
				switch (c2)
				{
				case ';':
					if (flag3)
					{
						stringBuilder.Append(c);
					}
					else if (flag2 || flag)
					{
						stringBuilder.Append(c);
					}
					else
					{
						if (text.Length > 0 && stringBuilder.Length > 0)
						{
							text2 = stringBuilder.ToString();
							text = text.ToLower().TrimEnd(new char[0]);
							this[text] = text2;
						}
						else if (stringBuilder.Length > 0)
						{
							throw DbConnectionStringBuilder.CreateConnectionStringInvalidException((int)c);
						}
						flag3 = true;
						text = string.Empty;
						stringBuilder.Length = 0;
					}
					break;
				default:
					if (c2 != '"')
					{
						if (c2 != '\'')
						{
							if (flag2 || flag)
							{
								stringBuilder.Append(c);
							}
							else if (char.IsWhiteSpace(c))
							{
								if (stringBuilder.Length > 0)
								{
									int num2 = DbConnectionStringBuilder.SkipTrailingWhitespace(connectionString, i);
									if (num2 == -1)
									{
										stringBuilder.Append(c);
									}
									else
									{
										i = num2;
									}
								}
							}
							else
							{
								stringBuilder.Append(c);
							}
						}
						else if (flag3)
						{
							stringBuilder.Append(c);
						}
						else if (flag2)
						{
							stringBuilder.Append(c);
						}
						else if (flag)
						{
							if (num == -1)
							{
								flag = false;
							}
							else if (num.Equals((int)c))
							{
								stringBuilder.Append(c);
								i++;
							}
							else
							{
								int num3 = DbConnectionStringBuilder.NextNonWhitespaceChar(connectionString, i);
								if (num3 != -1 && (ushort)num3 != 59)
								{
									throw DbConnectionStringBuilder.CreateConnectionStringInvalidException(num3);
								}
								flag = false;
							}
							if (!flag)
							{
								text2 = stringBuilder.ToString();
								text = text.ToLower().TrimEnd(new char[0]);
								this[text] = text2;
								flag3 = true;
								text = string.Empty;
								stringBuilder.Length = 0;
							}
						}
						else if (stringBuilder.Length == 0)
						{
							flag = true;
						}
						else
						{
							stringBuilder.Append(c);
						}
					}
					else if (flag3)
					{
						stringBuilder.Append(c);
					}
					else if (flag)
					{
						stringBuilder.Append(c);
					}
					else if (flag2)
					{
						if (num == -1)
						{
							flag2 = false;
						}
						else if (num.Equals((int)c))
						{
							stringBuilder.Append(c);
							i++;
						}
						else
						{
							int num4 = DbConnectionStringBuilder.NextNonWhitespaceChar(connectionString, i);
							if (num4 != -1 && (ushort)num4 != 59)
							{
								throw DbConnectionStringBuilder.CreateConnectionStringInvalidException(num4);
							}
							flag2 = false;
						}
					}
					else if (stringBuilder.Length == 0)
					{
						flag2 = true;
					}
					else
					{
						stringBuilder.Append(c);
					}
					break;
				case '=':
					if (flag2 || flag || !flag3)
					{
						stringBuilder.Append(c);
					}
					else if (num != -1 && num.Equals((int)c))
					{
						stringBuilder.Append(c);
						i++;
					}
					else
					{
						text = stringBuilder.ToString();
						if (text.Length == 0)
						{
							throw DbConnectionStringBuilder.CreateConnectionStringInvalidException((int)c);
						}
						stringBuilder.Length = 0;
						flag3 = false;
					}
					break;
				}
			}
			if ((flag3 && stringBuilder.Length > 0) || flag2 || flag)
			{
				throw DbConnectionStringBuilder.CreateConnectionStringInvalidException(length - 1);
			}
			if (text.Length > 0 && stringBuilder.Length > 0)
			{
				text2 = stringBuilder.ToString();
				text = text.ToLower().TrimEnd(new char[0]);
				this[text] = text2;
			}
		}

		private static int SkipTrailingWhitespace(string value, int index)
		{
			int length = value.Length;
			for (int i = index + 1; i < length; i++)
			{
				char c = value[i];
				if (c == ';')
				{
					return i - 1;
				}
				if (!char.IsWhiteSpace(c))
				{
					return -1;
				}
			}
			return length - 1;
		}

		private static int NextNonWhitespaceChar(string value, int index)
		{
			int length = value.Length;
			for (int i = index + 1; i < length; i++)
			{
				char c = value[i];
				if (!char.IsWhiteSpace(c))
				{
					return (int)c;
				}
			}
			return -1;
		}

		private readonly Dictionary<string, object> _dictionary;

		private readonly bool useOdbcRules;

		private static object _staticAttributeCollection;
	}
}
