using System;
using System.Collections;

namespace System.Diagnostics
{
	[Serializable]
	public class CounterCreationDataCollection : CollectionBase
	{
		public CounterCreationDataCollection()
		{
		}

		public CounterCreationDataCollection(CounterCreationData[] value)
		{
			this.AddRange(value);
		}

		public CounterCreationDataCollection(CounterCreationDataCollection value)
		{
			this.AddRange(value);
		}

		public CounterCreationData this[int index]
		{
			get
			{
				return (CounterCreationData)base.InnerList[index];
			}
			set
			{
				base.InnerList[index] = value;
			}
		}

		public int Add(CounterCreationData value)
		{
			return base.InnerList.Add(value);
		}

		public void AddRange(CounterCreationData[] value)
		{
			foreach (CounterCreationData counterCreationData in value)
			{
				this.Add(counterCreationData);
			}
		}

		public void AddRange(CounterCreationDataCollection value)
		{
			foreach (object obj in value)
			{
				CounterCreationData counterCreationData = (CounterCreationData)obj;
				this.Add(counterCreationData);
			}
		}

		public bool Contains(CounterCreationData value)
		{
			return base.InnerList.Contains(value);
		}

		public void CopyTo(CounterCreationData[] array, int index)
		{
			base.InnerList.CopyTo(array, index);
		}

		public int IndexOf(CounterCreationData value)
		{
			return base.InnerList.IndexOf(value);
		}

		public void Insert(int index, CounterCreationData value)
		{
			base.InnerList.Insert(index, value);
		}

		protected override void OnValidate(object value)
		{
			if (!(value is CounterCreationData))
			{
				throw new NotSupportedException(global::Locale.GetText("You can only insert CounterCreationData objects into the collection"));
			}
		}

		public virtual void Remove(CounterCreationData value)
		{
			base.InnerList.Remove(value);
		}
	}
}
