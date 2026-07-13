using System;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	internal class StyleValuePropertyBag<TContainer, TValue> : ContainerPropertyBag<TContainer> where TContainer : IStyleValue<TValue>
	{
		public StyleValuePropertyBag()
		{
			base.AddProperty<TValue>(new StyleValuePropertyBag<TContainer, TValue>.ValueProperty());
			base.AddProperty<StyleKeyword>(new StyleValuePropertyBag<TContainer, TValue>.KeywordProperty());
		}

		private class ValueProperty : Property<TContainer, TValue>
		{
			public override string Name { get; } = "value";

			public override bool IsReadOnly { get; } = false;

			public override TValue GetValue(ref TContainer container)
			{
				return container.value;
			}

			public override void SetValue(ref TContainer container, TValue value)
			{
				container.value = value;
			}
		}

		private class KeywordProperty : Property<TContainer, StyleKeyword>
		{
			public override string Name { get; } = "keyword";

			public override bool IsReadOnly { get; } = false;

			public override StyleKeyword GetValue(ref TContainer container)
			{
				return container.keyword;
			}

			public override void SetValue(ref TContainer container, StyleKeyword value)
			{
				container.keyword = value;
			}
		}
	}
}
