using System;
using System.Collections.Generic;

namespace System.Dynamic
{
	internal class ExpandoClass
	{
		internal ExpandoClass()
		{
			this._hashCode = 6551;
			this._keys = Array.Empty<string>();
		}

		internal ExpandoClass(string[] keys, int hashCode)
		{
			this._hashCode = hashCode;
			this._keys = keys;
		}

		internal ExpandoClass FindNewClass(string newKey)
		{
			int num = this._hashCode ^ newKey.GetHashCode();
			ExpandoClass expandoClass3;
			lock (this)
			{
				List<WeakReference> transitionList = this.GetTransitionList(num);
				for (int i = 0; i < transitionList.Count; i++)
				{
					ExpandoClass expandoClass = transitionList[i].Target as ExpandoClass;
					if (expandoClass == null)
					{
						transitionList.RemoveAt(i);
						i--;
					}
					else if (string.Equals(expandoClass._keys[expandoClass._keys.Length - 1], newKey, StringComparison.Ordinal))
					{
						return expandoClass;
					}
				}
				string[] array = new string[this._keys.Length + 1];
				Array.Copy(this._keys, 0, array, 0, this._keys.Length);
				array[this._keys.Length] = newKey;
				ExpandoClass expandoClass2 = new ExpandoClass(array, num);
				transitionList.Add(new WeakReference(expandoClass2));
				expandoClass3 = expandoClass2;
			}
			return expandoClass3;
		}

		private List<WeakReference> GetTransitionList(int hashCode)
		{
			if (this._transitions == null)
			{
				this._transitions = new Dictionary<int, List<WeakReference>>();
			}
			List<WeakReference> list;
			if (!this._transitions.TryGetValue(hashCode, out list))
			{
				list = (this._transitions[hashCode] = new List<WeakReference>());
			}
			return list;
		}

		internal int GetValueIndex(string name, bool caseInsensitive, ExpandoObject obj)
		{
			if (caseInsensitive)
			{
				return this.GetValueIndexCaseInsensitive(name, obj);
			}
			return this.GetValueIndexCaseSensitive(name);
		}

		internal int GetValueIndexCaseSensitive(string name)
		{
			for (int i = 0; i < this._keys.Length; i++)
			{
				if (string.Equals(this._keys[i], name, StringComparison.Ordinal))
				{
					return i;
				}
			}
			return -1;
		}

		private int GetValueIndexCaseInsensitive(string name, ExpandoObject obj)
		{
			int num = -1;
			object lockObject = obj.LockObject;
			lock (lockObject)
			{
				for (int i = this._keys.Length - 1; i >= 0; i--)
				{
					if (string.Equals(this._keys[i], name, StringComparison.OrdinalIgnoreCase) && !obj.IsDeletedMember(i))
					{
						if (num != -1)
						{
							return -2;
						}
						num = i;
					}
				}
			}
			return num;
		}

		internal string[] Keys
		{
			get
			{
				return this._keys;
			}
		}

		private readonly string[] _keys;

		private readonly int _hashCode;

		private Dictionary<int, List<WeakReference>> _transitions;

		private const int EmptyHashCode = 6551;

		internal static readonly ExpandoClass Empty = new ExpandoClass();
	}
}
