using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using YamlDotNet.Core;
using YamlDotNet.Helpers;

namespace YamlDotNet.Serialization
{
	public sealed class YamlAttributeOverrides
	{
		public T GetAttribute<T>(Type type, string member) where T : Attribute
		{
			List<YamlAttributeOverrides.AttributeMapping> list;
			if (this.overrides.TryGetValue(new YamlAttributeOverrides.AttributeKey(typeof(T), member), out list))
			{
				int num = 0;
				YamlAttributeOverrides.AttributeMapping attributeMapping = null;
				foreach (YamlAttributeOverrides.AttributeMapping attributeMapping2 in list)
				{
					int num2 = attributeMapping2.Matches(type);
					if (num2 > num)
					{
						num = num2;
						attributeMapping = attributeMapping2;
					}
				}
				if (num > 0)
				{
					return (T)((object)attributeMapping.Attribute);
				}
			}
			return default(T);
		}

		public void Add(Type type, string member, Attribute attribute)
		{
			YamlAttributeOverrides.AttributeMapping attributeMapping = new YamlAttributeOverrides.AttributeMapping(type, attribute);
			YamlAttributeOverrides.AttributeKey attributeKey = new YamlAttributeOverrides.AttributeKey(attribute.GetType(), member);
			List<YamlAttributeOverrides.AttributeMapping> list;
			if (!this.overrides.TryGetValue(attributeKey, out list))
			{
				list = new List<YamlAttributeOverrides.AttributeMapping>();
				this.overrides.Add(attributeKey, list);
			}
			else if (list.Contains(attributeMapping))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Attribute ({2}) already set for Type {0}, Member {1}", new object[] { type.FullName, member, attribute }));
			}
			list.Add(attributeMapping);
		}

		public void Add<TClass>(Expression<Func<TClass, object>> propertyAccessor, Attribute attribute)
		{
			PropertyInfo propertyInfo = propertyAccessor.AsProperty();
			this.Add(typeof(TClass), propertyInfo.Name, attribute);
		}

		public YamlAttributeOverrides Clone()
		{
			YamlAttributeOverrides yamlAttributeOverrides = new YamlAttributeOverrides();
			foreach (KeyValuePair<YamlAttributeOverrides.AttributeKey, List<YamlAttributeOverrides.AttributeMapping>> keyValuePair in this.overrides)
			{
				foreach (YamlAttributeOverrides.AttributeMapping attributeMapping in keyValuePair.Value)
				{
					yamlAttributeOverrides.Add(attributeMapping.RegisteredType, keyValuePair.Key.PropertyName, attributeMapping.Attribute);
				}
			}
			return yamlAttributeOverrides;
		}

		private readonly Dictionary<YamlAttributeOverrides.AttributeKey, List<YamlAttributeOverrides.AttributeMapping>> overrides = new Dictionary<YamlAttributeOverrides.AttributeKey, List<YamlAttributeOverrides.AttributeMapping>>();

		private struct AttributeKey
		{
			public AttributeKey(Type attributeType, string propertyName)
			{
				this.AttributeType = attributeType;
				this.PropertyName = propertyName;
			}

			public override bool Equals(object obj)
			{
				YamlAttributeOverrides.AttributeKey attributeKey = (YamlAttributeOverrides.AttributeKey)obj;
				return this.AttributeType.Equals(attributeKey.AttributeType) && this.PropertyName.Equals(attributeKey.PropertyName);
			}

			public override int GetHashCode()
			{
				return HashCode.CombineHashCodes(this.AttributeType.GetHashCode(), this.PropertyName.GetHashCode());
			}

			public readonly Type AttributeType;

			public readonly string PropertyName;
		}

		private sealed class AttributeMapping
		{
			public AttributeMapping(Type registeredType, Attribute attribute)
			{
				this.RegisteredType = registeredType;
				this.Attribute = attribute;
			}

			public override bool Equals(object obj)
			{
				YamlAttributeOverrides.AttributeMapping attributeMapping = obj as YamlAttributeOverrides.AttributeMapping;
				return attributeMapping != null && this.RegisteredType.Equals(attributeMapping.RegisteredType) && this.Attribute.Equals(attributeMapping.Attribute);
			}

			public override int GetHashCode()
			{
				return HashCode.CombineHashCodes(this.RegisteredType.GetHashCode(), this.Attribute.GetHashCode());
			}

			public int Matches(Type matchType)
			{
				int num = 0;
				for (Type type = matchType; type != null; type = type.BaseType())
				{
					num++;
					if (type == this.RegisteredType)
					{
						return num;
					}
				}
				if (matchType.GetInterfaces().Contains(this.RegisteredType))
				{
					return num;
				}
				return 0;
			}

			public readonly Type RegisteredType;

			public readonly Attribute Attribute;
		}
	}
}
