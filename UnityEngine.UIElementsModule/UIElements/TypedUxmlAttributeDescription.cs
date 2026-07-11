using System;

namespace UnityEngine.UIElements
{
	public abstract class TypedUxmlAttributeDescription<T> : UxmlAttributeDescription
	{
		public abstract T GetValueFromBag(IUxmlAttributes bag, CreationContext cc);

		public T defaultValue { get; set; }

		public override string defaultValueAsString
		{
			get
			{
				T defaultValue = this.defaultValue;
				return defaultValue.ToString();
			}
		}
	}
}
