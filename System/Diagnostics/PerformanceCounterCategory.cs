using System;
using System.Runtime.CompilerServices;
using System.Security.Permissions;

namespace System.Diagnostics
{
	[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
	public sealed class PerformanceCounterCategory
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CategoryDelete(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string CategoryHelpInternal(string category, string machine);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CounterCategoryExists(string counter, string category, string machine);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Create(string categoryName, string categoryHelp, PerformanceCounterCategoryType categoryType, CounterCreationData[] items);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int InstanceExistsInternal(string instance, string category, string machine);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetCategoryNames(string machine);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetCounterNames(string category, string machine);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetInstanceNames(string category, string machine);

		private static void CheckCategory(string categoryName)
		{
			if (categoryName == null)
			{
				throw new ArgumentNullException("categoryName");
			}
			if (categoryName == "")
			{
				throw new ArgumentException("categoryName");
			}
		}

		public PerformanceCounterCategory()
			: this("", ".")
		{
		}

		public PerformanceCounterCategory(string categoryName)
			: this(categoryName, ".")
		{
		}

		public PerformanceCounterCategory(string categoryName, string machineName)
		{
			PerformanceCounterCategory.CheckCategory(categoryName);
			if (machineName == null)
			{
				throw new ArgumentNullException("machineName");
			}
			this.categoryName = categoryName;
			this.machineName = machineName;
		}

		public string CategoryHelp
		{
			get
			{
				string text = PerformanceCounterCategory.CategoryHelpInternal(this.categoryName, this.machineName);
				if (text != null)
				{
					return text;
				}
				throw new InvalidOperationException();
			}
		}

		public string CategoryName
		{
			get
			{
				return this.categoryName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value == "")
				{
					throw new ArgumentException("value");
				}
				this.categoryName = value;
			}
		}

		public string MachineName
		{
			get
			{
				return this.machineName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value == "")
				{
					throw new ArgumentException("value");
				}
				this.machineName = value;
			}
		}

		public PerformanceCounterCategoryType CategoryType
		{
			get
			{
				return this.type;
			}
		}

		public bool CounterExists(string counterName)
		{
			return PerformanceCounterCategory.CounterExists(counterName, this.categoryName, this.machineName);
		}

		public static bool CounterExists(string counterName, string categoryName)
		{
			return PerformanceCounterCategory.CounterExists(counterName, categoryName, ".");
		}

		public static bool CounterExists(string counterName, string categoryName, string machineName)
		{
			if (counterName == null)
			{
				throw new ArgumentNullException("counterName");
			}
			PerformanceCounterCategory.CheckCategory(categoryName);
			if (machineName == null)
			{
				throw new ArgumentNullException("machineName");
			}
			return PerformanceCounterCategory.CounterCategoryExists(counterName, categoryName, machineName);
		}

		[Obsolete("Use another overload that uses PerformanceCounterCategoryType instead")]
		public static PerformanceCounterCategory Create(string categoryName, string categoryHelp, CounterCreationDataCollection counterData)
		{
			return PerformanceCounterCategory.Create(categoryName, categoryHelp, PerformanceCounterCategoryType.Unknown, counterData);
		}

		[Obsolete("Use another overload that uses PerformanceCounterCategoryType instead")]
		public static PerformanceCounterCategory Create(string categoryName, string categoryHelp, string counterName, string counterHelp)
		{
			return PerformanceCounterCategory.Create(categoryName, categoryHelp, PerformanceCounterCategoryType.Unknown, counterName, counterHelp);
		}

		public static PerformanceCounterCategory Create(string categoryName, string categoryHelp, PerformanceCounterCategoryType categoryType, CounterCreationDataCollection counterData)
		{
			PerformanceCounterCategory.CheckCategory(categoryName);
			if (counterData == null)
			{
				throw new ArgumentNullException("counterData");
			}
			if (counterData.Count == 0)
			{
				throw new ArgumentException("counterData");
			}
			CounterCreationData[] array = new CounterCreationData[counterData.Count];
			counterData.CopyTo(array, 0);
			if (!PerformanceCounterCategory.Create(categoryName, categoryHelp, categoryType, array))
			{
				throw new InvalidOperationException();
			}
			return new PerformanceCounterCategory(categoryName, categoryHelp);
		}

		public static PerformanceCounterCategory Create(string categoryName, string categoryHelp, PerformanceCounterCategoryType categoryType, string counterName, string counterHelp)
		{
			PerformanceCounterCategory.CheckCategory(categoryName);
			if (!PerformanceCounterCategory.Create(categoryName, categoryHelp, categoryType, new CounterCreationData[]
			{
				new CounterCreationData(counterName, counterHelp, PerformanceCounterType.NumberOfItems32)
			}))
			{
				throw new InvalidOperationException();
			}
			return new PerformanceCounterCategory(categoryName, categoryHelp);
		}

		public static void Delete(string categoryName)
		{
			PerformanceCounterCategory.CheckCategory(categoryName);
			if (!PerformanceCounterCategory.CategoryDelete(categoryName))
			{
				throw new InvalidOperationException();
			}
		}

		public static bool Exists(string categoryName)
		{
			return PerformanceCounterCategory.Exists(categoryName, ".");
		}

		public static bool Exists(string categoryName, string machineName)
		{
			PerformanceCounterCategory.CheckCategory(categoryName);
			return PerformanceCounterCategory.CounterCategoryExists(null, categoryName, machineName);
		}

		public static PerformanceCounterCategory[] GetCategories()
		{
			return PerformanceCounterCategory.GetCategories(".");
		}

		public static PerformanceCounterCategory[] GetCategories(string machineName)
		{
			if (machineName == null)
			{
				throw new ArgumentNullException("machineName");
			}
			string[] categoryNames = PerformanceCounterCategory.GetCategoryNames(machineName);
			PerformanceCounterCategory[] array = new PerformanceCounterCategory[categoryNames.Length];
			for (int i = 0; i < categoryNames.Length; i++)
			{
				array[i] = new PerformanceCounterCategory(categoryNames[i], machineName);
			}
			return array;
		}

		public PerformanceCounter[] GetCounters()
		{
			return this.GetCounters("");
		}

		public PerformanceCounter[] GetCounters(string instanceName)
		{
			string[] counterNames = PerformanceCounterCategory.GetCounterNames(this.categoryName, this.machineName);
			PerformanceCounter[] array = new PerformanceCounter[counterNames.Length];
			for (int i = 0; i < counterNames.Length; i++)
			{
				array[i] = new PerformanceCounter(this.categoryName, counterNames[i], instanceName, this.machineName);
			}
			return array;
		}

		public string[] GetInstanceNames()
		{
			return PerformanceCounterCategory.GetInstanceNames(this.categoryName, this.machineName);
		}

		public bool InstanceExists(string instanceName)
		{
			return PerformanceCounterCategory.InstanceExists(instanceName, this.categoryName, this.machineName);
		}

		public static bool InstanceExists(string instanceName, string categoryName)
		{
			return PerformanceCounterCategory.InstanceExists(instanceName, categoryName, ".");
		}

		public static bool InstanceExists(string instanceName, string categoryName, string machineName)
		{
			if (instanceName == null)
			{
				throw new ArgumentNullException("instanceName");
			}
			PerformanceCounterCategory.CheckCategory(categoryName);
			if (machineName == null)
			{
				throw new ArgumentNullException("machineName");
			}
			int num = PerformanceCounterCategory.InstanceExistsInternal(instanceName, categoryName, machineName);
			if (num == 0)
			{
				return false;
			}
			if (num == 1)
			{
				return true;
			}
			throw new InvalidOperationException();
		}

		[MonoTODO]
		public InstanceDataCollectionCollection ReadCategory()
		{
			throw new NotImplementedException();
		}

		private string categoryName;

		private string machineName;

		private PerformanceCounterCategoryType type = PerformanceCounterCategoryType.Unknown;
	}
}
