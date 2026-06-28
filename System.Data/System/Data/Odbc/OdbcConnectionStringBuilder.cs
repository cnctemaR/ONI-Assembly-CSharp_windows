using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.Odbc
{
	[DefaultProperty("Driver")]
	[TypeConverter("System.Data.Odbc.OdbcConnectionStringBuilder+OdbcConnectionStringBuilderConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public sealed class OdbcConnectionStringBuilder : DbConnectionStringBuilder
	{
		public OdbcConnectionStringBuilder()
			: base(true)
		{
		}

		public OdbcConnectionStringBuilder(string connectionString)
			: base(true)
		{
			if (connectionString == null)
			{
				base.ConnectionString = string.Empty;
				return;
			}
			base.ConnectionString = connectionString;
		}

		public override object this[string keyword]
		{
			get
			{
				if (keyword == null)
				{
					throw new ArgumentNullException("keyword");
				}
				if (string.Compare(keyword, "Driver", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					return this.Driver;
				}
				if (string.Compare(keyword, "Dsn", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					return this.Dsn;
				}
				return base[keyword];
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
				string text = value.ToString();
				if (string.Compare(keyword, "Driver", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					this.Driver = text;
					return;
				}
				if (string.Compare(keyword, "Dsn", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					this.dsn = text;
				}
				else if (value.ToString().IndexOf(';') != -1)
				{
					text = "{" + text + "}";
				}
				base[keyword] = value;
			}
		}

		public override ICollection Keys
		{
			get
			{
				List<string> list = new List<string>();
				list.Add("Dsn");
				list.Add("Driver");
				ICollection keys = base.Keys;
				foreach (object obj in keys)
				{
					string text = (string)obj;
					if (string.Compare(text, "Driver", StringComparison.InvariantCultureIgnoreCase) != 0)
					{
						if (string.Compare(text, "Dsn", StringComparison.InvariantCultureIgnoreCase) != 0)
						{
							list.Add(text);
						}
					}
				}
				string[] array = new string[list.Count];
				list.CopyTo(array);
				return array;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Driver")]
		public string Driver
		{
			get
			{
				if (this.driver == null)
				{
					return string.Empty;
				}
				return this.driver;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Driver");
				}
				this.driver = value;
				if (value.Length > 0)
				{
					int num = value.IndexOf('{');
					int num2 = value.IndexOf('}');
					if (num == -1 || num2 == -1)
					{
						value = "{" + value + "}";
					}
					else if (num > 0 || num2 < value.Length - 1)
					{
						value = "{" + value + "}";
					}
				}
				base["Driver"] = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Dsn")]
		public string Dsn
		{
			get
			{
				if (this.dsn == null)
				{
					return string.Empty;
				}
				return this.dsn;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Dsn");
				}
				this.dsn = value;
				base["Dsn"] = this.dsn;
			}
		}

		public override bool ContainsKey(string keyword)
		{
			if (keyword == null)
			{
				throw new ArgumentNullException("keyword");
			}
			return string.Compare(keyword, "Driver", StringComparison.InvariantCultureIgnoreCase) == 0 || string.Compare(keyword, "Dsn", StringComparison.InvariantCultureIgnoreCase) == 0 || base.ContainsKey(keyword);
		}

		public override bool Remove(string keyword)
		{
			if (keyword == null)
			{
				throw new ArgumentNullException("keyword");
			}
			if (string.Compare(keyword, "Driver", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				this.driver = string.Empty;
			}
			else if (string.Compare(keyword, "Dsn", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				this.dsn = string.Empty;
			}
			return base.Remove(keyword);
		}

		public override void Clear()
		{
			this.driver = null;
			this.dsn = null;
			base.Clear();
		}

		public override bool TryGetValue(string keyword, out object value)
		{
			if (keyword == null)
			{
				throw new ArgumentNullException("keyword");
			}
			bool flag = base.TryGetValue(keyword, out value);
			if (flag)
			{
				return flag;
			}
			if (string.Compare(keyword, "Driver", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				value = string.Empty;
				return true;
			}
			if (string.Compare(keyword, "Dsn", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				value = string.Empty;
				return true;
			}
			return false;
		}

		private string driver;

		private string dsn;
	}
}
