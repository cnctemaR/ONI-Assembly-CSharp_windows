using System;
using System.Collections;

namespace System.Data.Common
{
	[Serializable]
	internal sealed class NameValuePermission : IComparable
	{
		internal NameValuePermission()
		{
		}

		private NameValuePermission(string keyword)
		{
			this._value = keyword;
		}

		private NameValuePermission(string value, DBConnectionString entry)
		{
			this._value = value;
			this._entry = entry;
		}

		private NameValuePermission(NameValuePermission permit)
		{
			this._value = permit._value;
			this._entry = permit._entry;
			this._tree = permit._tree;
			if (this._tree != null)
			{
				NameValuePermission[] array = this._tree.Clone() as NameValuePermission[];
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] != null)
					{
						array[i] = array[i].CopyNameValue();
					}
				}
				this._tree = array;
			}
		}

		int IComparable.CompareTo(object a)
		{
			return StringComparer.Ordinal.Compare(this._value, ((NameValuePermission)a)._value);
		}

		internal static void AddEntry(NameValuePermission kvtree, ArrayList entries, DBConnectionString entry)
		{
			if (entry.KeyChain != null)
			{
				for (NameValuePair nameValuePair = entry.KeyChain; nameValuePair != null; nameValuePair = nameValuePair.Next)
				{
					NameValuePermission nameValuePermission = kvtree.CheckKeyForValue(nameValuePair.Name);
					if (nameValuePermission == null)
					{
						nameValuePermission = new NameValuePermission(nameValuePair.Name);
						kvtree.Add(nameValuePermission);
					}
					kvtree = nameValuePermission;
					nameValuePermission = kvtree.CheckKeyForValue(nameValuePair.Value);
					if (nameValuePermission == null)
					{
						DBConnectionString dbconnectionString = ((nameValuePair.Next != null) ? null : entry);
						nameValuePermission = new NameValuePermission(nameValuePair.Value, dbconnectionString);
						kvtree.Add(nameValuePermission);
						if (dbconnectionString != null)
						{
							entries.Add(dbconnectionString);
						}
					}
					else if (nameValuePair.Next == null)
					{
						if (nameValuePermission._entry != null)
						{
							entries.Remove(nameValuePermission._entry);
							nameValuePermission._entry = nameValuePermission._entry.Intersect(entry);
						}
						else
						{
							nameValuePermission._entry = entry;
						}
						entries.Add(nameValuePermission._entry);
					}
					kvtree = nameValuePermission;
				}
				return;
			}
			DBConnectionString entry2 = kvtree._entry;
			if (entry2 != null)
			{
				entries.Remove(entry2);
				kvtree._entry = entry2.Intersect(entry);
			}
			else
			{
				kvtree._entry = entry;
			}
			entries.Add(kvtree._entry);
		}

		internal void Intersect(ArrayList entries, NameValuePermission target)
		{
			if (target == null)
			{
				this._tree = null;
				this._entry = null;
				return;
			}
			if (this._entry != null)
			{
				entries.Remove(this._entry);
				this._entry = this._entry.Intersect(target._entry);
				entries.Add(this._entry);
			}
			else if (target._entry != null)
			{
				this._entry = target._entry.Intersect(null);
				entries.Add(this._entry);
			}
			if (this._tree != null)
			{
				int num = this._tree.Length;
				for (int i = 0; i < this._tree.Length; i++)
				{
					NameValuePermission nameValuePermission = target.CheckKeyForValue(this._tree[i]._value);
					if (nameValuePermission != null)
					{
						this._tree[i].Intersect(entries, nameValuePermission);
					}
					else
					{
						this._tree[i] = null;
						num--;
					}
				}
				if (num == 0)
				{
					this._tree = null;
					return;
				}
				if (num < this._tree.Length)
				{
					NameValuePermission[] array = new NameValuePermission[num];
					int j = 0;
					int num2 = 0;
					while (j < this._tree.Length)
					{
						if (this._tree[j] != null)
						{
							array[num2++] = this._tree[j];
						}
						j++;
					}
					this._tree = array;
				}
			}
		}

		private void Add(NameValuePermission permit)
		{
			NameValuePermission[] tree = this._tree;
			int num = ((tree != null) ? tree.Length : 0);
			NameValuePermission[] array = new NameValuePermission[1 + num];
			for (int i = 0; i < array.Length - 1; i++)
			{
				array[i] = tree[i];
			}
			array[num] = permit;
			Array.Sort<NameValuePermission>(array);
			this._tree = array;
		}

		internal bool CheckValueForKeyPermit(DBConnectionString parsetable)
		{
			if (parsetable == null)
			{
				return false;
			}
			bool flag = false;
			NameValuePermission[] tree = this._tree;
			if (tree != null)
			{
				flag = parsetable.IsEmpty;
				if (!flag)
				{
					foreach (NameValuePermission nameValuePermission in tree)
					{
						if (nameValuePermission != null)
						{
							string value = nameValuePermission._value;
							if (parsetable.ContainsKey(value))
							{
								string text = parsetable[value];
								NameValuePermission nameValuePermission2 = nameValuePermission.CheckKeyForValue(text);
								if (nameValuePermission2 == null)
								{
									return false;
								}
								if (!nameValuePermission2.CheckValueForKeyPermit(parsetable))
								{
									return false;
								}
								flag = true;
							}
						}
					}
				}
			}
			DBConnectionString entry = this._entry;
			if (entry != null)
			{
				flag = entry.IsSupersetOf(parsetable);
			}
			return flag;
		}

		private NameValuePermission CheckKeyForValue(string keyInQuestion)
		{
			NameValuePermission[] tree = this._tree;
			if (tree != null)
			{
				foreach (NameValuePermission nameValuePermission in tree)
				{
					if (string.Equals(keyInQuestion, nameValuePermission._value, StringComparison.OrdinalIgnoreCase))
					{
						return nameValuePermission;
					}
				}
			}
			return null;
		}

		internal NameValuePermission CopyNameValue()
		{
			return new NameValuePermission(this);
		}

		private string _value;

		private DBConnectionString _entry;

		private NameValuePermission[] _tree;

		internal static readonly NameValuePermission Default;
	}
}
