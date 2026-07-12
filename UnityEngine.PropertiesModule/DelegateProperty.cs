using System;

namespace Unity.Properties
{
	public class DelegateProperty<TContainer, TValue> : Property<TContainer, TValue>
	{
		public override string Name { get; }

		public override bool IsReadOnly
		{
			get
			{
				return this.m_Setter == null;
			}
		}

		public DelegateProperty(string name, PropertyGetter<TContainer, TValue> getter, PropertySetter<TContainer, TValue> setter = null)
		{
			this.Name = name;
			if (getter == null)
			{
				throw new ArgumentException("getter");
			}
			this.m_Getter = getter;
			this.m_Setter = setter;
		}

		public override TValue GetValue(ref TContainer container)
		{
			return this.m_Getter(ref container);
		}

		public override void SetValue(ref TContainer container, TValue value)
		{
			bool isReadOnly = this.IsReadOnly;
			if (isReadOnly)
			{
				throw new InvalidOperationException("Property is ReadOnly.");
			}
			this.m_Setter(ref container, value);
		}

		private readonly PropertyGetter<TContainer, TValue> m_Getter;

		private readonly PropertySetter<TContainer, TValue> m_Setter;
	}
}
