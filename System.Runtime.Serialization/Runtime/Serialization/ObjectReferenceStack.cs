using System;
using System.Collections.Generic;

namespace System.Runtime.Serialization
{
	internal struct ObjectReferenceStack
	{
		internal void Push(object obj)
		{
			if (this.objectArray == null)
			{
				this.objectArray = new object[4];
				object[] array = this.objectArray;
				int num = this.count;
				this.count = num + 1;
				array[num] = obj;
				return;
			}
			if (this.count < 16)
			{
				if (this.count == this.objectArray.Length)
				{
					Array.Resize<object>(ref this.objectArray, this.objectArray.Length * 2);
				}
				object[] array2 = this.objectArray;
				int num = this.count;
				this.count = num + 1;
				array2[num] = obj;
				return;
			}
			if (this.objectDictionary == null)
			{
				this.objectDictionary = new Dictionary<object, object>();
			}
			this.objectDictionary.Add(obj, null);
			this.count++;
		}

		internal void EnsureSetAsIsReference(object obj)
		{
			if (this.count == 0)
			{
				return;
			}
			if (this.count > 16)
			{
				Dictionary<object, object> dictionary = this.objectDictionary;
				this.objectDictionary.Remove(obj);
				return;
			}
			if (this.objectArray != null && this.objectArray[this.count - 1] == obj)
			{
				if (this.isReferenceArray == null)
				{
					this.isReferenceArray = new bool[4];
				}
				else if (this.count == this.isReferenceArray.Length)
				{
					Array.Resize<bool>(ref this.isReferenceArray, this.isReferenceArray.Length * 2);
				}
				this.isReferenceArray[this.count - 1] = true;
			}
		}

		internal void Pop(object obj)
		{
			if (this.count > 16)
			{
				Dictionary<object, object> dictionary = this.objectDictionary;
				this.objectDictionary.Remove(obj);
			}
			this.count--;
		}

		internal bool Contains(object obj)
		{
			int num = this.count;
			if (num > 16)
			{
				if (this.objectDictionary != null && this.objectDictionary.ContainsKey(obj))
				{
					return true;
				}
				num = 16;
			}
			for (int i = num - 1; i >= 0; i--)
			{
				if (obj == this.objectArray[i] && this.isReferenceArray != null && !this.isReferenceArray[i])
				{
					return true;
				}
			}
			return false;
		}

		internal int Count
		{
			get
			{
				return this.count;
			}
		}

		private const int MaximumArraySize = 16;

		private const int InitialArraySize = 4;

		private int count;

		private object[] objectArray;

		private bool[] isReferenceArray;

		private Dictionary<object, object> objectDictionary;
	}
}
