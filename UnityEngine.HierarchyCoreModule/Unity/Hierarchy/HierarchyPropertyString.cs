using System;
using UnityEngine.Internal;

namespace Unity.Hierarchy
{
	public readonly struct HierarchyPropertyString : IEquatable<HierarchyPropertyString>, IHierarchyProperty<string>
	{
		public bool IsCreated
		{
			get
			{
				bool flag;
				if ((in this.m_Property) != HierarchyPropertyId.Null)
				{
					Hierarchy hierarchy = this.m_Hierarchy;
					flag = hierarchy != null && hierarchy.IsCreated;
				}
				else
				{
					flag = false;
				}
				return flag;
			}
		}

		internal HierarchyPropertyString(Hierarchy hierarchy, in HierarchyPropertyId property)
		{
			bool flag = hierarchy == null;
			if (flag)
			{
				throw new ArgumentNullException("hierarchy");
			}
			bool flag2 = (in property) == HierarchyPropertyId.Null;
			if (flag2)
			{
				throw new ArgumentException("property");
			}
			this.m_Hierarchy = hierarchy;
			this.m_Property = property;
		}

		public string GetValue(in HierarchyNode node)
		{
			bool flag = this.m_Hierarchy == null;
			if (flag)
			{
				throw new NullReferenceException("Hierarchy reference has not been set.");
			}
			bool flag2 = !this.m_Hierarchy.IsCreated;
			if (flag2)
			{
				throw new InvalidOperationException("Hierarchy has been disposed.");
			}
			return this.m_Hierarchy.GetPropertyString(in this.m_Property, in node);
		}

		public void SetValue(in HierarchyNode node, string value)
		{
			bool flag = this.m_Hierarchy == null;
			if (flag)
			{
				throw new NullReferenceException("Hierarchy reference has not been set.");
			}
			bool flag2 = !this.m_Hierarchy.IsCreated;
			if (flag2)
			{
				throw new InvalidOperationException("Hierarchy has been disposed.");
			}
			this.m_Hierarchy.SetPropertyString(in this.m_Property, in node, value);
		}

		public void ClearValue(in HierarchyNode node)
		{
			bool flag = this.m_Hierarchy == null;
			if (flag)
			{
				throw new NullReferenceException("Hierarchy reference has not been set.");
			}
			bool flag2 = !this.m_Hierarchy.IsCreated;
			if (flag2)
			{
				throw new InvalidOperationException("Hierarchy has been disposed.");
			}
			this.m_Hierarchy.ClearProperty(in this.m_Property, in node);
		}

		[ExcludeFromDocs]
		public static bool operator ==(in HierarchyPropertyString lhs, in HierarchyPropertyString rhs)
		{
			return lhs.m_Hierarchy == rhs.m_Hierarchy && (in lhs.m_Property) == (in rhs.m_Property);
		}

		[ExcludeFromDocs]
		public static bool operator !=(in HierarchyPropertyString lhs, in HierarchyPropertyString rhs)
		{
			return !((in lhs) == (in rhs));
		}

		[ExcludeFromDocs]
		public bool Equals(HierarchyPropertyString other)
		{
			return this.m_Hierarchy == other.m_Hierarchy && (in this.m_Property) == (in other.m_Property);
		}

		[ExcludeFromDocs]
		public override string ToString()
		{
			return this.m_Property.ToString();
		}

		[ExcludeFromDocs]
		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is HierarchyPropertyString)
			{
				HierarchyPropertyString hierarchyPropertyString = (HierarchyPropertyString)obj;
				flag = this.Equals(hierarchyPropertyString);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		[ExcludeFromDocs]
		public override int GetHashCode()
		{
			return this.m_Property.GetHashCode();
		}

		string IHierarchyProperty<string>.GetValue(in HierarchyNode node)
		{
			return this.GetValue(in node);
		}

		void IHierarchyProperty<string>.SetValue(in HierarchyNode node, string value)
		{
			this.SetValue(in node, value);
		}

		void IHierarchyProperty<string>.ClearValue(in HierarchyNode node)
		{
			this.ClearValue(in node);
		}

		private readonly Hierarchy m_Hierarchy;

		internal readonly HierarchyPropertyId m_Property;
	}
}
