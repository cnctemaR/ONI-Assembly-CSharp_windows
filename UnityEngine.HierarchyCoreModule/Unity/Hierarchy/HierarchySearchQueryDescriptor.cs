using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Scripting.LifecycleManagement;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Hierarchy
{
	[NativeHeader("Modules/HierarchyCore/Public/HierarchySearch.h")]
	[NativeAsStruct]
	[RequiredByNativeCode]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class HierarchySearchQueryDescriptor
	{
		public static HierarchySearchQueryDescriptor Empty
		{
			get
			{
				return HierarchySearchQueryDescriptor.s_Empty;
			}
		}

		public static HierarchySearchQueryDescriptor InvalidQuery
		{
			get
			{
				return HierarchySearchQueryDescriptor.s_InvalidQuery;
			}
		}

		public HierarchySearchFilter[] SystemFilters { get; private set; }

		public HierarchySearchFilter[] Filters { get; private set; }

		public string[] TextValues { get; private set; }

		public bool Strict { get; set; }

		public bool Invalid { get; set; }

		public bool IsValid
		{
			get
			{
				return !this.Invalid && !this.IsEmpty;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.Filters.Length == 0 && this.TextValues.Length == 0 && this.SystemFilters.Length == 0;
			}
		}

		public bool IsSystemOnlyQuery
		{
			get
			{
				return this.SystemFilters.Length != 0 && this.Filters.Length == 0 && this.TextValues.Length == 0;
			}
		}

		public string Query
		{
			get
			{
				bool flag = this.m_Query == null || (this.m_Query == "" && (this.SystemFilters.Length != 0 || this.TextValues.Length != 0 || this.Filters.Length != 0));
				if (flag)
				{
					this.m_Query = this.BuildQuery();
				}
				return this.m_Query;
			}
		}

		public unsafe HierarchySearchQueryDescriptor(HierarchySearchFilter[] filters = null, string[] textValues = null)
		{
			filters = filters ?? new HierarchySearchFilter[0];
			textValues = textValues ?? new string[0];
			this.Filters = HierarchySearchQueryDescriptor.Where<HierarchySearchFilter>(filters, (HierarchySearchFilter f) => !HierarchySearchQueryDescriptor.s_SystemFilters.Contains(f.Name));
			this.SystemFilters = HierarchySearchQueryDescriptor.Where<HierarchySearchFilter>(filters, (HierarchySearchFilter f) => HierarchySearchQueryDescriptor.s_SystemFilters.Contains(f.Name));
			this.TextValues = textValues;
			HierarchySearchFilter hierarchySearchFilter = *HierarchySearchFilter.Invalid;
			foreach (HierarchySearchFilter hierarchySearchFilter2 in this.SystemFilters)
			{
				bool flag = hierarchySearchFilter2.Name == "strict";
				if (flag)
				{
					hierarchySearchFilter = hierarchySearchFilter2;
					break;
				}
			}
			this.Invalid = false;
			this.Strict = !hierarchySearchFilter.IsValid || hierarchySearchFilter.Value == "true";
		}

		public HierarchySearchQueryDescriptor(HierarchySearchQueryDescriptor desc)
		{
			this.SystemFilters = new HierarchySearchFilter[desc.SystemFilters.Length];
			Array.Copy(desc.SystemFilters, this.SystemFilters, desc.SystemFilters.Length);
			this.Filters = new HierarchySearchFilter[desc.Filters.Length];
			Array.Copy(desc.Filters, this.Filters, desc.Filters.Length);
			this.TextValues = new string[desc.TextValues.Length];
			Array.Copy(desc.TextValues, this.TextValues, desc.TextValues.Length);
			this.Strict = desc.Strict;
			this.Invalid = desc.Invalid;
		}

		public override string ToString()
		{
			return this.Query;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
		internal string BuildFilterQuery()
		{
			return string.Join<HierarchySearchFilter>(" ", this.Filters);
		}

		internal string BuildSystemFilterQuery()
		{
			return string.Join<HierarchySearchFilter>(" ", this.SystemFilters);
		}

		internal string BuildTextQuery()
		{
			string[] array = new string[this.TextValues.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = HierarchySearchFilter.QuoteStringIfNeeded(this.TextValues[i]);
			}
			return string.Join(" ", array);
		}

		internal string BuildQuery()
		{
			string text = "";
			bool flag = this.SystemFilters.Length != 0;
			if (flag)
			{
				text += this.BuildSystemFilterQuery();
			}
			bool flag2 = this.Filters.Length != 0;
			if (flag2)
			{
				bool flag3 = text.Length > 0;
				if (flag3)
				{
					text += " ";
				}
				text += this.BuildFilterQuery();
			}
			bool flag4 = this.TextValues.Length != 0;
			if (flag4)
			{
				bool flag5 = text.Length > 0;
				if (flag5)
				{
					text += " ";
				}
				text += this.BuildTextQuery();
			}
			return text;
		}

		private static T[] Where<T>(IEnumerable<T> src, Func<T, bool> pred)
		{
			int num = 0;
			foreach (T t in src)
			{
				bool flag = pred(t);
				if (flag)
				{
					num++;
				}
			}
			T[] array = new T[num];
			int num2 = 0;
			foreach (T t2 in src)
			{
				bool flag2 = pred(t2);
				if (flag2)
				{
					array[num2++] = t2;
				}
			}
			return array;
		}

		[NoAutoStaticsCleanup]
		private static readonly HashSet<string> s_SystemFilters = new HashSet<string>(new string[] { "nodetype", "strict" });

		[NoAutoStaticsCleanup]
		private static readonly HierarchySearchQueryDescriptor s_Empty = new HierarchySearchQueryDescriptor(null, null);

		[NoAutoStaticsCleanup]
		private static readonly HierarchySearchQueryDescriptor s_InvalidQuery = new HierarchySearchQueryDescriptor(null, null)
		{
			Invalid = true
		};

		private string m_Query;
	}
}
