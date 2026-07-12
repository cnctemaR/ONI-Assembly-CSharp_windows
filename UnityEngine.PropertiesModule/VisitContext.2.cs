using System;
using Unity.Properties.Internal;

namespace Unity.Properties
{
	public readonly struct VisitContext<TContainer>
	{
		internal static VisitContext<TContainer> FromProperty<TValue>(PropertyVisitor visitor, ReadOnlyAdapterCollection.Enumerator enumerator, Property<TContainer, TValue> property)
		{
			return new VisitContext<TContainer>(visitor, enumerator, property, delegate(PropertyVisitor v, ReadOnlyAdapterCollection.Enumerator e, IProperty<TContainer> p, ref TContainer c)
			{
				Property<TContainer, TValue> property2 = (Property<TContainer, TValue>)p;
				TValue value = property2.GetValue(ref c);
				v.ContinueVisitation<TContainer, TValue>(property2, e, ref c, ref value);
			}, delegate(PropertyVisitor v, IProperty<TContainer> p, ref TContainer c)
			{
				Property<TContainer, TValue> property3 = (Property<TContainer, TValue>)p;
				TValue value2 = property3.GetValue(ref c);
				v.ContinueVisitation<TContainer, TValue>(property3, ref c, ref value2);
			});
		}

		public IProperty<TContainer> Property { get; }

		private VisitContext(PropertyVisitor visitor, ReadOnlyAdapterCollection.Enumerator enumerator, IProperty<TContainer> property, VisitContext<TContainer>.VisitDelegate continueVisitation, VisitContext<TContainer>.VisitWithoutAdaptersDelegate continueVisitationWithoutAdapters)
		{
			this.m_Visitor = visitor;
			this.m_Enumerator = enumerator;
			this.Property = property;
			this.m_Continue = continueVisitation;
			this.m_ContinueWithoutAdapters = continueVisitationWithoutAdapters;
		}

		public void ContinueVisitation(ref TContainer container)
		{
			this.m_Continue(this.m_Visitor, this.m_Enumerator, this.Property, ref container);
		}

		public void ContinueVisitationWithoutAdapters(ref TContainer container)
		{
			this.m_ContinueWithoutAdapters(this.m_Visitor, this.Property, ref container);
		}

		private readonly ReadOnlyAdapterCollection.Enumerator m_Enumerator;

		private readonly PropertyVisitor m_Visitor;

		private readonly VisitContext<TContainer>.VisitDelegate m_Continue;

		private readonly VisitContext<TContainer>.VisitWithoutAdaptersDelegate m_ContinueWithoutAdapters;

		private delegate void VisitDelegate(PropertyVisitor visitor, ReadOnlyAdapterCollection.Enumerator enumerator, IProperty<TContainer> property, ref TContainer container);

		private delegate void VisitWithoutAdaptersDelegate(PropertyVisitor visitor, IProperty<TContainer> property, ref TContainer container);
	}
}
