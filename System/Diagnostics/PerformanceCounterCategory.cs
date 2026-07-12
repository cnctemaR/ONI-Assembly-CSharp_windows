using System;
using System.Runtime.CompilerServices;
using System.Security.Permissions;

namespace System.Diagnostics
{
	[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
	public sealed class PerformanceCounterCategory
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern bool CategoryDelete_icall(char* name, int name_length);

		private unsafe static bool CategoryDelete(string name)
		{
			char* ptr = name;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			return PerformanceCounterCategory.CategoryDelete_icall(ptr, (name != null) ? name.Length : 0);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern string CategoryHelp_icall(char* category, int category_length);

		private unsafe static string CategoryHelpInternal(string category)
		{
			char* ptr = category;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			return PerformanceCounterCategory.CategoryHelp_icall(ptr, (category != null) ? category.Length : 0);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern bool CounterCategoryExists_icall(char* counter, int counter_length, char* category, int category_length);

		private unsafe static bool CounterCategoryExists(string counter, string category)
		{
			char* ptr = counter;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			char* ptr2 = category;
			if (ptr2 != null)
			{
				ptr2 += RuntimeHelpers.OffsetToStringData / 2;
			}
			return PerformanceCounterCategory.CounterCategoryExists_icall(ptr, (counter != null) ? counter.Length : 0, ptr2, (category != null) ? category.Length : 0);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern bool Create_icall(char* categoryName, int categoryName_length, char* categoryHelp, int categoryHelp_length, PerformanceCounterCategoryType categoryType, CounterCreationData[] items);

		private unsafe static bool Create(string categoryName, string categoryHelp, PerformanceCounterCategoryType categoryType, CounterCreationData[] items)
		{
			char* ptr = categoryName;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			char* ptr2 = categoryHelp;
			if (ptr2 != null)
			{
				ptr2 += RuntimeHelpers.OffsetToStringData / 2;
			}
			return PerformanceCounterCategory.Create_icall(ptr, (categoryName != null) ? categoryName.Length : 0, ptr2, (categoryHelp != null) ? categoryHelp.Length : 0, categoryType, items);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern bool InstanceExistsInternal_icall(char* instance, int instance_length, char* category, int category_length);

		private unsafe static bool InstanceExistsInternal(string instance, string category)
		{
			char* ptr = instance;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			char* ptr2 = category;
			if (ptr2 != null)
			{
				ptr2 += RuntimeHelpers.OffsetToStringData / 2;
			}
			return PerformanceCounterCategory.InstanceExistsInternal_icall(ptr, (instance != null) ? instance.Length : 0, ptr2, (category != null) ? category.Length : 0);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetCategoryNames();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern string[] GetCounterNames_icall(char* category, int category_length);

		private unsafe static string[] GetCounterNames(string category)
		{
			char* ptr = category;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			return PerformanceCounterCategory.GetCounterNames_icall(ptr, (category != null) ? category.Length : 0);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern string[] GetInstanceNames_icall(char* category, int category_length);

		private unsafe static string[] GetInstanceNames(string category)
		{
			char* ptr = category;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			return PerformanceCounterCategory.GetInstanceNames_icall(ptr, (category != null) ? category.Length : 0);
		}

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

		private static bool IsValidMachine(string machine)
		{
			return machine == ".";
		}

		public string CategoryHelp
		{
			get
			{
				string text = null;
				if (PerformanceCounterCategory.IsValidMachine(this.machineName))
				{
					text = PerformanceCounterCategory.CategoryHelpInternal(this.categoryName);
				}
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
			return PerformanceCounterCategory.IsValidMachine(machineName) && PerformanceCounterCategory.CounterCategoryExists(counterName, categoryName);
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
			return PerformanceCounterCategory.IsValidMachine(machineName) && PerformanceCounterCategory.CounterCategoryExists(null, categoryName);
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
			if (!PerformanceCounterCategory.IsValidMachine(machineName))
			{
				return Array.Empty<PerformanceCounterCategory>();
			}
			string[] categoryNames = PerformanceCounterCategory.GetCategoryNames();
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
			if (!PerformanceCounterCategory.IsValidMachine(this.machineName))
			{
				return Array.Empty<PerformanceCounter>();
			}
			string[] counterNames = PerformanceCounterCategory.GetCounterNames(this.categoryName);
			PerformanceCounter[] array = new PerformanceCounter[counterNames.Length];
			for (int i = 0; i < counterNames.Length; i++)
			{
				array[i] = new PerformanceCounter(this.categoryName, counterNames[i], instanceName, this.machineName);
			}
			return array;
		}

		public string[] GetInstanceNames()
		{
			if (!PerformanceCounterCategory.IsValidMachine(this.machineName))
			{
				return Array.Empty<string>();
			}
			return PerformanceCounterCategory.GetInstanceNames(this.categoryName);
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
			return PerformanceCounterCategory.InstanceExistsInternal(instanceName, categoryName);
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
