using System;
using System.Collections;

namespace System.ComponentModel.Design.Serialization
{
	public sealed class ContextStack
	{
		public ContextStack()
		{
			this._contextList = new ArrayList();
		}

		public object Current
		{
			get
			{
				int count = this._contextList.Count;
				if (count > 0)
				{
					return this._contextList[count - 1];
				}
				return null;
			}
		}

		public object this[Type type]
		{
			get
			{
				if (type == null)
				{
					throw new ArgumentNullException("type");
				}
				for (int i = this._contextList.Count - 1; i >= 0; i--)
				{
					object obj = this._contextList[i];
					if (type.IsInstanceOfType(obj))
					{
						return obj;
					}
				}
				return null;
			}
		}

		public object this[int level]
		{
			get
			{
				if (level < 0)
				{
					throw new ArgumentOutOfRangeException("level");
				}
				int count = this._contextList.Count;
				if (count > 0 && count > level)
				{
					return this._contextList[count - 1 - level];
				}
				return null;
			}
		}

		public object Pop()
		{
			object obj = null;
			int count = this._contextList.Count;
			if (count > 0)
			{
				int num = count - 1;
				obj = this._contextList[num];
				this._contextList.RemoveAt(num);
			}
			return obj;
		}

		public void Push(object context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			this._contextList.Add(context);
		}

		public void Append(object context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			this._contextList.Insert(0, context);
		}

		private ArrayList _contextList;
	}
}
