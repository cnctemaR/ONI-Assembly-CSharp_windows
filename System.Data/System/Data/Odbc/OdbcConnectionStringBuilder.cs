using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.Odbc
{
	public sealed class OdbcConnectionStringBuilder : DbConnectionStringBuilder
	{
		static OdbcConnectionStringBuilder()
		{
			string[] array = new string[] { null, "Driver" };
			array[0] = "Dsn";
			OdbcConnectionStringBuilder.s_validKeywords = array;
			OdbcConnectionStringBuilder.s_keywords = new Dictionary<string, OdbcConnectionStringBuilder.Keywords>(2, StringComparer.OrdinalIgnoreCase)
			{
				{
					"Driver",
					OdbcConnectionStringBuilder.Keywords.Driver
				},
				{
					"Dsn",
					OdbcConnectionStringBuilder.Keywords.Dsn
				}
			};
		}

		public OdbcConnectionStringBuilder()
			: this(null)
		{
		}

		public OdbcConnectionStringBuilder(string connectionString)
			: base(true)
		{
			if (!string.IsNullOrEmpty(connectionString))
			{
				base.ConnectionString = connectionString;
			}
		}

		public override object this[string keyword]
		{
			get
			{
				ADP.CheckArgumentNull(keyword, "keyword");
				OdbcConnectionStringBuilder.Keywords keywords;
				if (OdbcConnectionStringBuilder.s_keywords.TryGetValue(keyword, out keywords))
				{
					return this.GetAt(keywords);
				}
				return base[keyword];
			}
			set
			{
				ADP.CheckArgumentNull(keyword, "keyword");
				if (value == null)
				{
					this.Remove(keyword);
					return;
				}
				OdbcConnectionStringBuilder.Keywords keywords;
				if (!OdbcConnectionStringBuilder.s_keywords.TryGetValue(keyword, out keywords))
				{
					base[keyword] = value;
					base.ClearPropertyDescriptors();
					this._knownKeywords = null;
					return;
				}
				if (keywords == OdbcConnectionStringBuilder.Keywords.Dsn)
				{
					this.Dsn = OdbcConnectionStringBuilder.ConvertToString(value);
					return;
				}
				if (keywords == OdbcConnectionStringBuilder.Keywords.Driver)
				{
					this.Driver = OdbcConnectionStringBuilder.ConvertToString(value);
					return;
				}
				throw ADP.KeywordNotSupported(keyword);
			}
		}

		[DisplayName("Driver")]
		public string Driver
		{
			get
			{
				return this._driver;
			}
			set
			{
				this.SetValue("Driver", value);
				this._driver = value;
			}
		}

		[DisplayName("Dsn")]
		public string Dsn
		{
			get
			{
				return this._dsn;
			}
			set
			{
				this.SetValue("Dsn", value);
				this._dsn = value;
			}
		}

		public override ICollection Keys
		{
			get
			{
				string[] array = this._knownKeywords;
				if (array == null)
				{
					array = OdbcConnectionStringBuilder.s_validKeywords;
					int num = 0;
					foreach (object obj in base.Keys)
					{
						string text = (string)obj;
						bool flag = true;
						string[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							if (array2[i] == text)
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							num++;
						}
					}
					if (0 < num)
					{
						string[] array3 = new string[array.Length + num];
						array.CopyTo(array3, 0);
						int num2 = array.Length;
						foreach (object obj2 in base.Keys)
						{
							string text2 = (string)obj2;
							bool flag2 = true;
							string[] array2 = array;
							for (int i = 0; i < array2.Length; i++)
							{
								if (array2[i] == text2)
								{
									flag2 = false;
									break;
								}
							}
							if (flag2)
							{
								array3[num2++] = text2;
							}
						}
						array = array3;
					}
					this._knownKeywords = array;
				}
				return new ReadOnlyCollection<string>(array);
			}
		}

		public override void Clear()
		{
			base.Clear();
			for (int i = 0; i < OdbcConnectionStringBuilder.s_validKeywords.Length; i++)
			{
				this.Reset((OdbcConnectionStringBuilder.Keywords)i);
			}
			this._knownKeywords = OdbcConnectionStringBuilder.s_validKeywords;
		}

		public override bool ContainsKey(string keyword)
		{
			ADP.CheckArgumentNull(keyword, "keyword");
			return OdbcConnectionStringBuilder.s_keywords.ContainsKey(keyword) || base.ContainsKey(keyword);
		}

		private static string ConvertToString(object value)
		{
			return DbConnectionStringBuilderUtil.ConvertToString(value);
		}

		private object GetAt(OdbcConnectionStringBuilder.Keywords index)
		{
			if (index == OdbcConnectionStringBuilder.Keywords.Dsn)
			{
				return this.Dsn;
			}
			if (index == OdbcConnectionStringBuilder.Keywords.Driver)
			{
				return this.Driver;
			}
			throw ADP.KeywordNotSupported(OdbcConnectionStringBuilder.s_validKeywords[(int)index]);
		}

		public override bool Remove(string keyword)
		{
			ADP.CheckArgumentNull(keyword, "keyword");
			if (base.Remove(keyword))
			{
				OdbcConnectionStringBuilder.Keywords keywords;
				if (OdbcConnectionStringBuilder.s_keywords.TryGetValue(keyword, out keywords))
				{
					this.Reset(keywords);
				}
				else
				{
					base.ClearPropertyDescriptors();
					this._knownKeywords = null;
				}
				return true;
			}
			return false;
		}

		private void Reset(OdbcConnectionStringBuilder.Keywords index)
		{
			if (index == OdbcConnectionStringBuilder.Keywords.Dsn)
			{
				this._dsn = "";
				return;
			}
			if (index == OdbcConnectionStringBuilder.Keywords.Driver)
			{
				this._driver = "";
				return;
			}
			throw ADP.KeywordNotSupported(OdbcConnectionStringBuilder.s_validKeywords[(int)index]);
		}

		private void SetValue(string keyword, string value)
		{
			ADP.CheckArgumentNull(value, keyword);
			base[keyword] = value;
		}

		public override bool TryGetValue(string keyword, out object value)
		{
			ADP.CheckArgumentNull(keyword, "keyword");
			OdbcConnectionStringBuilder.Keywords keywords;
			if (OdbcConnectionStringBuilder.s_keywords.TryGetValue(keyword, out keywords))
			{
				value = this.GetAt(keywords);
				return true;
			}
			return base.TryGetValue(keyword, out value);
		}

		private static readonly string[] s_validKeywords;

		private static readonly Dictionary<string, OdbcConnectionStringBuilder.Keywords> s_keywords;

		private string[] _knownKeywords;

		private string _dsn = "";

		private string _driver = "";

		private enum Keywords
		{
			Dsn,
			Driver
		}
	}
}
