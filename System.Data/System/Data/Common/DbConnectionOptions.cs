using System;
using System.Collections;
using System.Collections.Specialized;
using System.Security;
using System.Text;

namespace System.Data.Common
{
	internal class DbConnectionOptions
	{
		internal DbConnectionOptions()
		{
		}

		protected internal DbConnectionOptions(DbConnectionOptions connectionOptions)
		{
			this.options = connectionOptions.options;
		}

		public DbConnectionOptions(string connectionString)
		{
			this.options = new NameValueCollection();
			this.ParseConnectionString(connectionString);
		}

		[MonoTODO]
		public DbConnectionOptions(string connectionString, Hashtable synonyms, bool useFirstKeyValuePair)
			: this(connectionString)
		{
		}

		[MonoTODO]
		public bool IsEmpty
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string this[string keyword]
		{
			get
			{
				return this.options[keyword];
			}
		}

		public ICollection Keys
		{
			get
			{
				return this.options.Keys;
			}
		}

		[MonoTODO]
		protected void BuildConnectionString(StringBuilder builder, string[] withoutOptions, string insertValue)
		{
			throw new NotImplementedException();
		}

		public bool ContainsKey(string keyword)
		{
			return this.options.Get(keyword) != null;
		}

		public bool ConvertValueToBoolean(string keyname, bool defaultvalue)
		{
			if (this.ContainsKey(keyname))
			{
				return bool.Parse(this[keyname].Trim());
			}
			return defaultvalue;
		}

		public int ConvertValueToInt32(string keyname, int defaultvalue)
		{
			if (this.ContainsKey(keyname))
			{
				return int.Parse(this[keyname].Trim());
			}
			return defaultvalue;
		}

		[MonoTODO]
		public bool ConvertValueToIntegratedSecurity()
		{
			throw new NotImplementedException();
		}

		public string ConvertValueToString(string keyname, string defaultValue)
		{
			if (this.ContainsKey(keyname))
			{
				return this[keyname];
			}
			return defaultValue;
		}

		[MonoTODO]
		protected internal virtual PermissionSet CreatePermissionSet()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected internal virtual string Expand()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static string RemoveKeyValuePairs(string connectionString, string[] keynames)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public string UsersConnectionString(bool hisPasswordPwd)
		{
			throw new NotImplementedException();
		}

		internal void ParseConnectionString(string connectionString)
		{
			if (connectionString.Length == 0)
			{
				return;
			}
			connectionString += ";";
			bool flag = false;
			bool flag2 = false;
			bool flag3 = true;
			string text = string.Empty;
			string text2 = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < connectionString.Length; i++)
			{
				char c = connectionString[i];
				char c2;
				if (i == connectionString.Length - 1)
				{
					c2 = '\0';
				}
				else
				{
					c2 = connectionString[i + 1];
				}
				char c3 = c;
				switch (c3)
				{
				case ' ':
					if (flag || flag2)
					{
						stringBuilder.Append(c);
					}
					else if (stringBuilder.Length > 0 && !c2.Equals(';'))
					{
						stringBuilder.Append(c);
					}
					break;
				default:
					switch (c3)
					{
					case ';':
						if (flag2 || flag)
						{
							stringBuilder.Append(c);
						}
						else
						{
							if (text != string.Empty && text != null)
							{
								text2 = stringBuilder.ToString();
								this.options[text.Trim()] = text2;
							}
							flag3 = true;
							text = string.Empty;
							text2 = string.Empty;
							stringBuilder = new StringBuilder();
						}
						break;
					default:
						if (c3 != '\'')
						{
							stringBuilder.Append(c);
						}
						else if (flag2)
						{
							stringBuilder.Append(c);
						}
						else if (c2.Equals(c))
						{
							stringBuilder.Append(c);
							i++;
						}
						else
						{
							flag = !flag;
						}
						break;
					case '=':
						if (flag2 || flag || !flag3)
						{
							stringBuilder.Append(c);
						}
						else if (c2.Equals(c))
						{
							stringBuilder.Append(c);
							i++;
						}
						else
						{
							text = stringBuilder.ToString();
							stringBuilder = new StringBuilder();
							flag3 = false;
						}
						break;
					}
					break;
				case '"':
					if (flag)
					{
						stringBuilder.Append(c);
					}
					else if (c2.Equals(c))
					{
						stringBuilder.Append(c);
						i++;
					}
					else
					{
						flag2 = !flag2;
					}
					break;
				}
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			ArrayList arrayList = new ArrayList();
			arrayList.AddRange(this.Keys);
			arrayList.Sort();
			foreach (object obj in arrayList)
			{
				string text3 = (string)obj;
				string text4 = string.Format("{0}=\"{1}\";", text3, this[text3].Replace("\"", "\"\""));
				stringBuilder2.Append(text4);
			}
			this.normalizedConnectionString = stringBuilder2.ToString();
		}

		internal NameValueCollection options;

		internal string normalizedConnectionString;
	}
}
