using System;
using System.Collections.Generic;
using Unity.Properties.Internal;

namespace Unity.Properties
{
	public abstract class Property<TContainer, TValue> : IProperty<TContainer>, IProperty, IPropertyAccept<TContainer>, IAttributes
	{
		List<Attribute> IAttributes.Attributes
		{
			get
			{
				return this.m_Attributes;
			}
			set
			{
				this.m_Attributes = value;
			}
		}

		public abstract string Name { get; }

		public abstract bool IsReadOnly { get; }

		public Type DeclaredValueType()
		{
			return typeof(TValue);
		}

		public void Accept(IPropertyVisitor visitor, ref TContainer container)
		{
			visitor.Visit<TContainer, TValue>(this, ref container);
		}

		object IProperty<TContainer>.GetValue(ref TContainer container)
		{
			return this.GetValue(ref container);
		}

		void IProperty<TContainer>.SetValue(ref TContainer container, object value)
		{
			this.SetValue(ref container, TypeConversion.Convert<object, TValue>(ref value));
		}

		public abstract TValue GetValue(ref TContainer container);

		public abstract void SetValue(ref TContainer container, TValue value);

		protected void AddAttribute(Attribute attribute)
		{
			((IAttributes)this).AddAttribute(attribute);
		}

		protected void AddAttributes(IEnumerable<Attribute> attributes)
		{
			((IAttributes)this).AddAttributes(attributes);
		}

		void IAttributes.AddAttribute(Attribute attribute)
		{
			bool flag = attribute == null || attribute.GetType() == typeof(CreatePropertyAttribute);
			if (!flag)
			{
				bool flag2 = this.m_Attributes == null;
				if (flag2)
				{
					this.m_Attributes = new List<Attribute>();
				}
				this.m_Attributes.Add(attribute);
			}
		}

		void IAttributes.AddAttributes(IEnumerable<Attribute> attributes)
		{
			bool flag = this.m_Attributes == null;
			if (flag)
			{
				this.m_Attributes = new List<Attribute>();
			}
			foreach (Attribute attribute in attributes)
			{
				bool flag2 = attribute == null;
				if (!flag2)
				{
					this.m_Attributes.Add(attribute);
				}
			}
		}

		public bool HasAttribute<TAttribute>() where TAttribute : Attribute
		{
			int num = 0;
			for (;;)
			{
				int num2 = num;
				List<Attribute> attributes = this.m_Attributes;
				int? num3 = ((attributes != null) ? new int?(attributes.Count) : null);
				if (!((num2 < num3.GetValueOrDefault()) & (num3 != null)))
				{
					goto Block_3;
				}
				bool flag = this.m_Attributes[num] is TAttribute;
				if (flag)
				{
					break;
				}
				num++;
			}
			return true;
			Block_3:
			return false;
		}

		public TAttribute GetAttribute<TAttribute>() where TAttribute : Attribute
		{
			int num = 0;
			TAttribute tattribute;
			for (;;)
			{
				int num2 = num;
				List<Attribute> attributes = this.m_Attributes;
				int? num3 = ((attributes != null) ? new int?(attributes.Count) : null);
				if (!((num2 < num3.GetValueOrDefault()) & (num3 != null)))
				{
					goto Block_3;
				}
				tattribute = this.m_Attributes[num] as TAttribute;
				bool flag = tattribute != null;
				if (flag)
				{
					break;
				}
				num++;
			}
			return tattribute;
			Block_3:
			return default(TAttribute);
		}

		public IEnumerable<TAttribute> GetAttributes<TAttribute>() where TAttribute : Attribute
		{
			int i = 0;
			for (;;)
			{
				int num = i;
				List<Attribute> attributes = this.m_Attributes;
				int? num2 = ((attributes != null) ? new int?(attributes.Count) : null);
				if (!((num < num2.GetValueOrDefault()) & (num2 != null)))
				{
					break;
				}
				Attribute attribute = this.m_Attributes[i];
				TAttribute typed = attribute as TAttribute;
				bool flag = typed != null;
				if (flag)
				{
					yield return typed;
				}
				typed = default(TAttribute);
				int num3 = i;
				i = num3 + 1;
			}
			yield break;
		}

		public IEnumerable<Attribute> GetAttributes()
		{
			int i = 0;
			for (;;)
			{
				int num = i;
				List<Attribute> attributes = this.m_Attributes;
				int? num2 = ((attributes != null) ? new int?(attributes.Count) : null);
				if (!((num < num2.GetValueOrDefault()) & (num2 != null)))
				{
					break;
				}
				yield return this.m_Attributes[i];
				int num3 = i;
				i = num3 + 1;
			}
			yield break;
		}

		AttributesScope IAttributes.CreateAttributesScope(IAttributes attributes)
		{
			return new AttributesScope(this, (attributes != null) ? attributes.Attributes : null);
		}

		private List<Attribute> m_Attributes;
	}
}
