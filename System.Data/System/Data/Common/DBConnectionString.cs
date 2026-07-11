using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace System.Data.Common
{
	[Serializable]
	internal sealed class DBConnectionString
	{
		internal DBConnectionString(string value, string restrictions, KeyRestrictionBehavior behavior, Dictionary<string, string> synonyms, bool useOdbcRules)
			: this(new DbConnectionOptions(value, synonyms, useOdbcRules), restrictions, behavior, synonyms, false)
		{
		}

		internal DBConnectionString(DbConnectionOptions connectionOptions)
			: this(connectionOptions, null, KeyRestrictionBehavior.AllowOnly, null, true)
		{
		}

		private DBConnectionString(DbConnectionOptions connectionOptions, string restrictions, KeyRestrictionBehavior behavior, Dictionary<string, string> synonyms, bool mustCloneDictionary)
		{
			if (behavior <= KeyRestrictionBehavior.PreventUsage)
			{
				this._behavior = behavior;
				this._encryptedUsersConnectionString = connectionOptions.UsersConnectionString(false);
				this._hasPassword = connectionOptions._hasPasswordKeyword;
				this._parsetable = connectionOptions.Parsetable;
				this._keychain = connectionOptions._keyChain;
				if (this._hasPassword && !connectionOptions.HasPersistablePassword)
				{
					if (mustCloneDictionary)
					{
						this._parsetable = new Dictionary<string, string>(this._parsetable);
					}
					if (this._parsetable.ContainsKey("password"))
					{
						this._parsetable["password"] = "*";
					}
					if (this._parsetable.ContainsKey("pwd"))
					{
						this._parsetable["pwd"] = "*";
					}
					this._keychain = connectionOptions.ReplacePasswordPwd(out this._encryptedUsersConnectionString, true);
				}
				if (!string.IsNullOrEmpty(restrictions))
				{
					this._restrictionValues = DBConnectionString.ParseRestrictions(restrictions, synonyms);
					this._restrictions = restrictions;
				}
				return;
			}
			throw ADP.InvalidKeyRestrictionBehavior(behavior);
		}

		private DBConnectionString(DBConnectionString connectionString, string[] restrictionValues, KeyRestrictionBehavior behavior)
		{
			this._encryptedUsersConnectionString = connectionString._encryptedUsersConnectionString;
			this._parsetable = connectionString._parsetable;
			this._keychain = connectionString._keychain;
			this._hasPassword = connectionString._hasPassword;
			this._restrictionValues = restrictionValues;
			this._restrictions = null;
			this._behavior = behavior;
		}

		internal KeyRestrictionBehavior Behavior
		{
			get
			{
				return this._behavior;
			}
		}

		internal string ConnectionString
		{
			get
			{
				return this._encryptedUsersConnectionString;
			}
		}

		internal bool IsEmpty
		{
			get
			{
				return this._keychain == null;
			}
		}

		internal NameValuePair KeyChain
		{
			get
			{
				return this._keychain;
			}
		}

		internal string Restrictions
		{
			get
			{
				string text = this._restrictions;
				if (text == null)
				{
					string[] restrictionValues = this._restrictionValues;
					if (restrictionValues != null && restrictionValues.Length != 0)
					{
						StringBuilder stringBuilder = new StringBuilder();
						for (int i = 0; i < restrictionValues.Length; i++)
						{
							if (!string.IsNullOrEmpty(restrictionValues[i]))
							{
								stringBuilder.Append(restrictionValues[i]);
								stringBuilder.Append("=;");
							}
						}
						text = stringBuilder.ToString();
					}
				}
				if (text == null)
				{
					return "";
				}
				return text;
			}
		}

		internal string this[string keyword]
		{
			get
			{
				return this._parsetable[keyword];
			}
		}

		internal bool ContainsKey(string keyword)
		{
			return this._parsetable.ContainsKey(keyword);
		}

		internal DBConnectionString Intersect(DBConnectionString entry)
		{
			KeyRestrictionBehavior keyRestrictionBehavior = this._behavior;
			string[] array = null;
			if (entry == null)
			{
				keyRestrictionBehavior = KeyRestrictionBehavior.AllowOnly;
			}
			else if (this._behavior != entry._behavior)
			{
				keyRestrictionBehavior = KeyRestrictionBehavior.AllowOnly;
				if (entry._behavior == KeyRestrictionBehavior.AllowOnly)
				{
					if (!ADP.IsEmptyArray(this._restrictionValues))
					{
						if (!ADP.IsEmptyArray(entry._restrictionValues))
						{
							array = DBConnectionString.NewRestrictionAllowOnly(entry._restrictionValues, this._restrictionValues);
						}
					}
					else
					{
						array = entry._restrictionValues;
					}
				}
				else if (!ADP.IsEmptyArray(this._restrictionValues))
				{
					if (!ADP.IsEmptyArray(entry._restrictionValues))
					{
						array = DBConnectionString.NewRestrictionAllowOnly(this._restrictionValues, entry._restrictionValues);
					}
					else
					{
						array = this._restrictionValues;
					}
				}
			}
			else if (KeyRestrictionBehavior.PreventUsage == this._behavior)
			{
				if (ADP.IsEmptyArray(this._restrictionValues))
				{
					array = entry._restrictionValues;
				}
				else if (ADP.IsEmptyArray(entry._restrictionValues))
				{
					array = this._restrictionValues;
				}
				else
				{
					array = DBConnectionString.NoDuplicateUnion(this._restrictionValues, entry._restrictionValues);
				}
			}
			else if (!ADP.IsEmptyArray(this._restrictionValues) && !ADP.IsEmptyArray(entry._restrictionValues))
			{
				if (this._restrictionValues.Length <= entry._restrictionValues.Length)
				{
					array = DBConnectionString.NewRestrictionIntersect(this._restrictionValues, entry._restrictionValues);
				}
				else
				{
					array = DBConnectionString.NewRestrictionIntersect(entry._restrictionValues, this._restrictionValues);
				}
			}
			return new DBConnectionString(this, array, keyRestrictionBehavior);
		}

		[Conditional("DEBUG")]
		private void ValidateCombinedSet(DBConnectionString componentSet, DBConnectionString combinedSet)
		{
			if (componentSet != null && combinedSet._restrictionValues != null && componentSet._restrictionValues != null)
			{
				if (componentSet._behavior == KeyRestrictionBehavior.AllowOnly)
				{
					if (combinedSet._behavior != KeyRestrictionBehavior.AllowOnly)
					{
						KeyRestrictionBehavior behavior = combinedSet._behavior;
						return;
					}
				}
				else if (componentSet._behavior == KeyRestrictionBehavior.PreventUsage && combinedSet._behavior != KeyRestrictionBehavior.AllowOnly)
				{
					KeyRestrictionBehavior behavior2 = combinedSet._behavior;
				}
			}
		}

		private bool IsRestrictedKeyword(string key)
		{
			return this._restrictionValues == null || 0 > Array.BinarySearch<string>(this._restrictionValues, key, StringComparer.Ordinal);
		}

		internal bool IsSupersetOf(DBConnectionString entry)
		{
			KeyRestrictionBehavior behavior = this._behavior;
			if (behavior != KeyRestrictionBehavior.AllowOnly)
			{
				if (behavior != KeyRestrictionBehavior.PreventUsage)
				{
					throw ADP.InvalidKeyRestrictionBehavior(this._behavior);
				}
				if (this._restrictionValues != null)
				{
					foreach (string text in this._restrictionValues)
					{
						if (entry.ContainsKey(text))
						{
							return false;
						}
					}
				}
			}
			else
			{
				for (NameValuePair nameValuePair = entry.KeyChain; nameValuePair != null; nameValuePair = nameValuePair.Next)
				{
					if (!this.ContainsKey(nameValuePair.Name) && this.IsRestrictedKeyword(nameValuePair.Name))
					{
						return false;
					}
				}
			}
			return true;
		}

		private static string[] NewRestrictionAllowOnly(string[] allowonly, string[] preventusage)
		{
			List<string> list = null;
			for (int i = 0; i < allowonly.Length; i++)
			{
				if (0 > Array.BinarySearch<string>(preventusage, allowonly[i], StringComparer.Ordinal))
				{
					if (list == null)
					{
						list = new List<string>();
					}
					list.Add(allowonly[i]);
				}
			}
			string[] array = null;
			if (list != null)
			{
				array = list.ToArray();
			}
			return array;
		}

		private static string[] NewRestrictionIntersect(string[] a, string[] b)
		{
			List<string> list = null;
			for (int i = 0; i < a.Length; i++)
			{
				if (0 <= Array.BinarySearch<string>(b, a[i], StringComparer.Ordinal))
				{
					if (list == null)
					{
						list = new List<string>();
					}
					list.Add(a[i]);
				}
			}
			string[] array = null;
			if (list != null)
			{
				array = list.ToArray();
			}
			return array;
		}

		private static string[] NoDuplicateUnion(string[] a, string[] b)
		{
			List<string> list = new List<string>(a.Length + b.Length);
			for (int i = 0; i < a.Length; i++)
			{
				list.Add(a[i]);
			}
			for (int j = 0; j < b.Length; j++)
			{
				if (0 > Array.BinarySearch<string>(a, b[j], StringComparer.Ordinal))
				{
					list.Add(b[j]);
				}
			}
			string[] array = list.ToArray();
			Array.Sort<string>(array, StringComparer.Ordinal);
			return array;
		}

		private static string[] ParseRestrictions(string restrictions, Dictionary<string, string> synonyms)
		{
			List<string> list = new List<string>();
			StringBuilder stringBuilder = new StringBuilder(restrictions.Length);
			int i = 0;
			int length = restrictions.Length;
			while (i < length)
			{
				int num = i;
				string text;
				string text2;
				i = DbConnectionOptions.GetKeyValuePair(restrictions, num, stringBuilder, false, out text, out text2);
				if (!string.IsNullOrEmpty(text))
				{
					string text3 = ((synonyms != null) ? synonyms[text] : text);
					if (string.IsNullOrEmpty(text3))
					{
						throw ADP.KeywordNotSupported(text);
					}
					list.Add(text3);
				}
			}
			return DBConnectionString.RemoveDuplicates(list.ToArray());
		}

		internal static string[] RemoveDuplicates(string[] restrictions)
		{
			int num = restrictions.Length;
			if (0 < num)
			{
				Array.Sort<string>(restrictions, StringComparer.Ordinal);
				for (int i = 1; i < restrictions.Length; i++)
				{
					string text = restrictions[i - 1];
					if (text.Length == 0 || text == restrictions[i])
					{
						restrictions[i - 1] = null;
						num--;
					}
				}
				if (restrictions[restrictions.Length - 1].Length == 0)
				{
					restrictions[restrictions.Length - 1] = null;
					num--;
				}
				if (num != restrictions.Length)
				{
					string[] array = new string[num];
					num = 0;
					for (int j = 0; j < restrictions.Length; j++)
					{
						if (restrictions[j] != null)
						{
							array[num++] = restrictions[j];
						}
					}
					restrictions = array;
				}
			}
			return restrictions;
		}

		[Conditional("DEBUG")]
		private static void Verify(string[] restrictionValues)
		{
			if (restrictionValues != null)
			{
				for (int i = 1; i < restrictionValues.Length; i++)
				{
				}
			}
		}

		private readonly string _encryptedUsersConnectionString;

		private readonly Dictionary<string, string> _parsetable;

		private readonly NameValuePair _keychain;

		private readonly bool _hasPassword;

		private readonly string[] _restrictionValues;

		private readonly string _restrictions;

		private readonly KeyRestrictionBehavior _behavior;

		private readonly string _encryptedActualConnectionString;

		private static class KEY
		{
			internal const string Password = "password";

			internal const string PersistSecurityInfo = "persist security info";

			internal const string Pwd = "pwd";
		}
	}
}
