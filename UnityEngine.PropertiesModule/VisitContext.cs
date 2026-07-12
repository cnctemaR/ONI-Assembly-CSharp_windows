using System;
using Unity.Properties.Internal;

namespace Unity.Properties
{
	public readonly struct VisitContext<TContainer, TValue>
	{
		internal static VisitContext<TContainer, TValue> FromProperty(PropertyVisitor visitor, ReadOnlyAdapterCollection.Enumerator enumerator, Property<TContainer, TValue> property)
		{
			return new VisitContext<TContainer, TValue>(visitor, enumerator, property);
		}

		public Property<TContainer, TValue> Property { get; }

		private VisitContext(PropertyVisitor visitor, ReadOnlyAdapterCollection.Enumerator enumerator, Property<TContainer, TValue> property)
		{
			this.m_Visitor = visitor;
			this.m_Enumerator = enumerator;
			this.Property = property;
		}

		public void ContinueVisitation(ref TContainer container, ref TValue value)
		{
			this.m_Visitor.ContinueVisitation<TContainer, TValue>(this.Property, this.m_Enumerator, ref container, ref value);
		}

		public void ContinueVisitationWithoutAdapters(ref TContainer container, ref TValue value)
		{
			this.m_Visitor.ContinueVisitationWithoutAdapters<TContainer, TValue>(this.Property, this.m_Enumerator, ref container, ref value);
		}

		private readonly ReadOnlyAdapterCollection.Enumerator m_Enumerator;

		private readonly PropertyVisitor m_Visitor;
	}
}
