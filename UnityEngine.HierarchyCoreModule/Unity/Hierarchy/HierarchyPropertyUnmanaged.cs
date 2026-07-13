using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Internal;

namespace Unity.Hierarchy
{
	public readonly struct HierarchyPropertyUnmanaged<[global::System.Runtime.CompilerServices.IsUnmanaged] T> : IEquatable<HierarchyPropertyUnmanaged<T>>, IHierarchyProperty<T> where T : struct, ValueType
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

		internal HierarchyPropertyUnmanaged(Hierarchy hierarchy, in HierarchyPropertyId property)
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

		public unsafe void SetValue(in HierarchyNode node, T value)
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
			this.m_Hierarchy.SetPropertyRaw(in this.m_Property, in node, (void*)(&value), sizeof(T));
		}

		public unsafe T GetValue(in HierarchyNode node)
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
			int num;
			void* propertyRaw = this.m_Hierarchy.GetPropertyRaw(in this.m_Property, in node, out num);
			bool flag3 = propertyRaw == null || num != sizeof(T);
			T t;
			if (flag3)
			{
				t = default(T);
			}
			else
			{
				t = *UnsafeUtility.AsRef<T>(propertyRaw);
			}
			return t;
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
		public static bool operator ==(in HierarchyPropertyUnmanaged<T> lhs, in HierarchyPropertyUnmanaged<T> rhs)
		{
			return lhs.m_Hierarchy == rhs.m_Hierarchy && (in lhs.m_Property) == (in rhs.m_Property);
		}

		[ExcludeFromDocs]
		public static bool operator !=(in HierarchyPropertyUnmanaged<T> lhs, in HierarchyPropertyUnmanaged<T> rhs)
		{
			return !((in lhs) == (in rhs));
		}

		[ExcludeFromDocs]
		public bool Equals(HierarchyPropertyUnmanaged<T> other)
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
			if (obj is HierarchyPropertyUnmanaged<T>)
			{
				HierarchyPropertyUnmanaged<T> hierarchyPropertyUnmanaged = (HierarchyPropertyUnmanaged<T>)obj;
				flag = this.Equals(hierarchyPropertyUnmanaged);
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

		T IHierarchyProperty<T>.GetValue(in HierarchyNode node)
		{
			return this.GetValue(in node);
		}

		void IHierarchyProperty<T>.SetValue(in HierarchyNode node, T value)
		{
			this.SetValue(in node, value);
		}

		void IHierarchyProperty<T>.ClearValue(in HierarchyNode node)
		{
			this.ClearValue(in node);
		}

		private readonly Hierarchy m_Hierarchy;

		internal readonly HierarchyPropertyId m_Property;
	}
}
