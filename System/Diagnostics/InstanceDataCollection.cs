using System;
using System.Collections;

namespace System.Diagnostics
{
	public class InstanceDataCollection : DictionaryBase
	{
		private static void CheckNull(object value, string name)
		{
			if (value == null)
			{
				throw new ArgumentNullException(name);
			}
		}

		[Obsolete("Use InstanceDataCollectionCollection indexer instead.")]
		public InstanceDataCollection(string counterName)
		{
			InstanceDataCollection.CheckNull(counterName, "counterName");
			this.counterName = counterName;
		}

		public string CounterName
		{
			get
			{
				return this.counterName;
			}
		}

		public InstanceData this[string instanceName]
		{
			get
			{
				InstanceDataCollection.CheckNull(instanceName, "instanceName");
				return (InstanceData)base.Dictionary[instanceName];
			}
		}

		public ICollection Keys
		{
			get
			{
				return base.Dictionary.Keys;
			}
		}

		public ICollection Values
		{
			get
			{
				return base.Dictionary.Values;
			}
		}

		public bool Contains(string instanceName)
		{
			InstanceDataCollection.CheckNull(instanceName, "instanceName");
			return base.Dictionary.Contains(instanceName);
		}

		public void CopyTo(InstanceData[] instances, int index)
		{
			base.Dictionary.CopyTo(instances, index);
		}

		private string counterName;
	}
}
