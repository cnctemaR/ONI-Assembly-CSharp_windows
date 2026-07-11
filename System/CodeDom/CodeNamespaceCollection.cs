using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Serializable]
	public class CodeNamespaceCollection : CollectionBase
	{
		public CodeNamespaceCollection()
		{
		}

		public CodeNamespaceCollection(CodeNamespace[] value)
		{
			this.AddRange(value);
		}

		public CodeNamespaceCollection(CodeNamespaceCollection value)
		{
			this.AddRange(value);
		}

		public CodeNamespace this[int index]
		{
			get
			{
				return (CodeNamespace)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		public int Add(CodeNamespace value)
		{
			return base.List.Add(value);
		}

		public void AddRange(CodeNamespace[] value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			for (int i = 0; i < value.Length; i++)
			{
				this.Add(value[i]);
			}
		}

		public void AddRange(CodeNamespaceCollection value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			int count = value.Count;
			for (int i = 0; i < count; i++)
			{
				this.Add(value[i]);
			}
		}

		public bool Contains(CodeNamespace value)
		{
			return base.List.Contains(value);
		}

		public void CopyTo(CodeNamespace[] array, int index)
		{
			base.List.CopyTo(array, index);
		}

		public int IndexOf(CodeNamespace value)
		{
			return base.List.IndexOf(value);
		}

		public void Insert(int index, CodeNamespace value)
		{
			base.List.Insert(index, value);
		}

		public void Remove(CodeNamespace value)
		{
			base.List.Remove(value);
		}
	}
}
