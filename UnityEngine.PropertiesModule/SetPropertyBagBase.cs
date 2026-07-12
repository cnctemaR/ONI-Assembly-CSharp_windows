using System;
using System.Collections.Generic;

namespace Unity.Properties
{
	public class SetPropertyBagBase<TSet, TElement> : PropertyBag<TSet>, ISetPropertyBag<TSet, TElement>, ICollectionPropertyBag<TSet, TElement>, IPropertyBag<TSet>, IPropertyBag, ICollectionPropertyBagAccept<TSet>, ISetPropertyBagAccept<TSet>, ISetPropertyAccept<TSet>, IKeyedProperties<TSet, object> where TSet : ISet<TElement>
	{
		public override PropertyCollection<TSet> GetProperties()
		{
			return PropertyCollection<TSet>.Empty;
		}

		public override PropertyCollection<TSet> GetProperties(ref TSet container)
		{
			return new PropertyCollection<TSet>(this.GetPropertiesEnumerable(container));
		}

		private IEnumerable<IProperty<TSet>> GetPropertiesEnumerable(TSet container)
		{
			foreach (TElement element in container)
			{
				this.m_Property.m_Value = element;
				yield return this.m_Property;
				element = default(TElement);
			}
			IEnumerator<TElement> enumerator = null;
			yield break;
			yield break;
		}

		void ICollectionPropertyBagAccept<TSet>.Accept(ICollectionPropertyBagVisitor visitor, ref TSet container)
		{
			visitor.Visit<TSet, TElement>(this, ref container);
		}

		void ISetPropertyBagAccept<TSet>.Accept(ISetPropertyBagVisitor visitor, ref TSet container)
		{
			visitor.Visit<TSet, TElement>(this, ref container);
		}

		void ISetPropertyAccept<TSet>.Accept<TContainer>(ISetPropertyVisitor visitor, Property<TContainer, TSet> property, ref TContainer container, ref TSet dictionary)
		{
			using (new AttributesScope(this.m_Property, property))
			{
				visitor.Visit<TContainer, TSet, TElement>(property, ref container, ref dictionary);
			}
		}

		public bool TryGetProperty(ref TSet container, object key, out IProperty<TSet> property)
		{
			bool flag = container.Contains((TElement)((object)key));
			bool flag2;
			if (flag)
			{
				property = new SetPropertyBagBase<TSet, TElement>.SetElementProperty
				{
					m_Value = (TElement)((object)key)
				};
				flag2 = true;
			}
			else
			{
				property = null;
				flag2 = false;
			}
			return flag2;
		}

		private readonly SetPropertyBagBase<TSet, TElement>.SetElementProperty m_Property = new SetPropertyBagBase<TSet, TElement>.SetElementProperty();

		private class SetElementProperty : Property<TSet, TElement>, ISetElementProperty<TElement>, ISetElementProperty, ICollectionElementProperty
		{
			public override string Name
			{
				get
				{
					return this.m_Value.ToString();
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override TElement GetValue(ref TSet container)
			{
				return this.m_Value;
			}

			public override void SetValue(ref TSet container, TElement value)
			{
				throw new InvalidOperationException("Property is ReadOnly.");
			}

			public TElement Key
			{
				get
				{
					return this.m_Value;
				}
			}

			public object ObjectKey
			{
				get
				{
					return this.m_Value;
				}
			}

			internal TElement m_Value;
		}
	}
}
