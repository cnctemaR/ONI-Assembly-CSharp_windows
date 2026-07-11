using System;
using System.Collections;

namespace System.Diagnostics
{
	public class ProcessThreadCollection : ReadOnlyCollectionBase
	{
		protected ProcessThreadCollection()
		{
		}

		public ProcessThreadCollection(ProcessThread[] processThreads)
		{
			base.InnerList.AddRange(processThreads);
		}

		internal static ProcessThreadCollection GetEmpty()
		{
			return new ProcessThreadCollection();
		}

		public ProcessThread this[int index]
		{
			get
			{
				return (ProcessThread)base.InnerList[index];
			}
		}

		public int Add(ProcessThread thread)
		{
			return base.InnerList.Add(thread);
		}

		public bool Contains(ProcessThread thread)
		{
			return base.InnerList.Contains(thread);
		}

		public void CopyTo(ProcessThread[] array, int index)
		{
			base.InnerList.CopyTo(array, index);
		}

		public int IndexOf(ProcessThread thread)
		{
			return base.InnerList.IndexOf(thread);
		}

		public void Insert(int index, ProcessThread thread)
		{
			base.InnerList.Insert(index, thread);
		}

		public void Remove(ProcessThread thread)
		{
			base.InnerList.Remove(thread);
		}
	}
}
