using System;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	public struct StylePropertyName : IEquatable<StylePropertyName>
	{
		internal readonly StylePropertyId id
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get;
		}

		private readonly string name { get; }

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static StylePropertyId StylePropertyIdFromString(string name)
		{
			StylePropertyId stylePropertyId;
			bool flag = StylePropertyUtil.s_NameToId.TryGetValue(name, out stylePropertyId);
			StylePropertyId stylePropertyId2;
			if (flag)
			{
				stylePropertyId2 = stylePropertyId;
			}
			else
			{
				stylePropertyId2 = StylePropertyId.Unknown;
			}
			return stylePropertyId2;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal StylePropertyName(StylePropertyId stylePropertyId)
		{
			this.id = stylePropertyId;
			this.name = null;
			string text;
			bool flag = StylePropertyUtil.s_IdToName.TryGetValue(stylePropertyId, out text);
			if (flag)
			{
				this.name = text;
			}
		}

		public StylePropertyName(string name)
		{
			this.id = StylePropertyName.StylePropertyIdFromString(name);
			this.name = null;
			bool flag = this.id > StylePropertyId.Unknown;
			if (flag)
			{
				this.name = name;
			}
		}

		public static bool IsNullOrEmpty(StylePropertyName propertyName)
		{
			return propertyName.id == StylePropertyId.Unknown;
		}

		public static bool operator ==(StylePropertyName lhs, StylePropertyName rhs)
		{
			return lhs.id == rhs.id;
		}

		public static bool operator !=(StylePropertyName lhs, StylePropertyName rhs)
		{
			return lhs.id != rhs.id;
		}

		public static implicit operator StylePropertyName(string name)
		{
			return new StylePropertyName(name);
		}

		public override int GetHashCode()
		{
			return (int)this.id;
		}

		public override bool Equals(object other)
		{
			return other is StylePropertyName && this.Equals((StylePropertyName)other);
		}

		public bool Equals(StylePropertyName other)
		{
			return this == other;
		}

		public override string ToString()
		{
			return this.name;
		}

		internal class PropertyBag : ContainerPropertyBag<StylePropertyName>
		{
			public PropertyBag()
			{
				base.AddProperty<StylePropertyId>(new StylePropertyName.PropertyBag.IdProperty());
				base.AddProperty<string>(new StylePropertyName.PropertyBag.NameProperty());
			}

			private class IdProperty : Property<StylePropertyName, StylePropertyId>
			{
				public override string Name { get; } = "id";

				public override bool IsReadOnly { get; } = true;

				public override StylePropertyId GetValue(ref StylePropertyName container)
				{
					return container.id;
				}

				public override void SetValue(ref StylePropertyName container, StylePropertyId value)
				{
				}
			}

			private class NameProperty : Property<StylePropertyName, string>
			{
				public override string Name { get; } = "name";

				public override bool IsReadOnly { get; } = true;

				public override string GetValue(ref StylePropertyName container)
				{
					return container.name;
				}

				public override void SetValue(ref StylePropertyName container, string value)
				{
				}
			}
		}
	}
}
