using System;

namespace Unity.Properties
{
	public readonly struct ExcludeContext<TContainer>
	{
		internal static ExcludeContext<TContainer> FromProperty<TValue>(PropertyVisitor visitor, Property<TContainer, TValue> property)
		{
			return new ExcludeContext<TContainer>(visitor, property);
		}

		public IProperty<TContainer> Property { get; }

		private ExcludeContext(PropertyVisitor visitor, IProperty<TContainer> property)
		{
			this.m_Visitor = visitor;
			this.Property = property;
		}

		private readonly PropertyVisitor m_Visitor;
	}
}
