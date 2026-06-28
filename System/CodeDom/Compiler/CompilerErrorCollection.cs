using System;
using System.Collections;

namespace System.CodeDom.Compiler
{
	[Serializable]
	public class CompilerErrorCollection : CollectionBase
	{
		public CompilerErrorCollection()
		{
		}

		public CompilerErrorCollection(CompilerErrorCollection value)
		{
			base.InnerList.AddRange(value.InnerList);
		}

		public CompilerErrorCollection(CompilerError[] value)
		{
			base.InnerList.AddRange(value);
		}

		public int Add(CompilerError value)
		{
			return base.InnerList.Add(value);
		}

		public void AddRange(CompilerError[] value)
		{
			base.InnerList.AddRange(value);
		}

		public void AddRange(CompilerErrorCollection value)
		{
			base.InnerList.AddRange(value.InnerList);
		}

		public bool Contains(CompilerError value)
		{
			return base.InnerList.Contains(value);
		}

		public void CopyTo(CompilerError[] array, int index)
		{
			base.InnerList.CopyTo(array, index);
		}

		public int IndexOf(CompilerError value)
		{
			return base.InnerList.IndexOf(value);
		}

		public void Insert(int index, CompilerError value)
		{
			base.InnerList.Insert(index, value);
		}

		public void Remove(CompilerError value)
		{
			base.InnerList.Remove(value);
		}

		public CompilerError this[int index]
		{
			get
			{
				return (CompilerError)base.InnerList[index];
			}
			set
			{
				base.InnerList[index] = value;
			}
		}

		public bool HasErrors
		{
			get
			{
				foreach (object obj in base.InnerList)
				{
					CompilerError compilerError = (CompilerError)obj;
					if (!compilerError.IsWarning)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool HasWarnings
		{
			get
			{
				foreach (object obj in base.InnerList)
				{
					CompilerError compilerError = (CompilerError)obj;
					if (compilerError.IsWarning)
					{
						return true;
					}
				}
				return false;
			}
		}
	}
}
