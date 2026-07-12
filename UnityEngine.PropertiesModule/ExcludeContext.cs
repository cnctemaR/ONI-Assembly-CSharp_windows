using System;

namespace Unity.Properties
{
	public readonly struct ExcludeContext<TContainer, TValue>
	{
		internal static ExcludeContext<TContainer, TValue> FromProperty(PropertyVisitor visitor, Property<TContainer, TValue> property)
		{
			return new ExcludeContext<TContainer, TValue>(visitor, property);
		}

		public Property<TContainer, TValue> Property { get; }

		private ExcludeContext(PropertyVisitor visitor, Property<TContainer, TValue> property)
		{
			this.m_Visitor = visitor;
			this.Property = property;
		}

		private readonly PropertyVisitor m_Visitor;
	}
}
