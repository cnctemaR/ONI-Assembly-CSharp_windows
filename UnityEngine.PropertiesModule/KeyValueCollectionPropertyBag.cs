using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace Unity.Properties
{
	public class KeyValueCollectionPropertyBag<TDictionary, TKey, TValue> : PropertyBag<TDictionary>, IDictionaryPropertyBag<TDictionary, TKey, TValue>, ICollectionPropertyBag<TDictionary, KeyValuePair<TKey, TValue>>, IPropertyBag<TDictionary>, IPropertyBag, ICollectionPropertyBagAccept<TDictionary>, IDictionaryPropertyBagAccept<TDictionary>, IDictionaryPropertyAccept<TDictionary>, IKeyedProperties<TDictionary, object> where TDictionary : IDictionary<TKey, TValue>
	{
		public override PropertyCollection<TDictionary> GetProperties()
		{
			return PropertyCollection<TDictionary>.Empty;
		}

		public override PropertyCollection<TDictionary> GetProperties(ref TDictionary container)
		{
			return new PropertyCollection<TDictionary>(new KeyValueCollectionPropertyBag<TDictionary, TKey, TValue>.Enumerable(container, this.m_KeyValuePairProperty));
		}

		void ICollectionPropertyBagAccept<TDictionary>.Accept(ICollectionPropertyBagVisitor visitor, ref TDictionary container)
		{
			visitor.Visit<TDictionary, KeyValuePair<TKey, TValue>>(this, ref container);
		}

		void IDictionaryPropertyBagAccept<TDictionary>.Accept(IDictionaryPropertyBagVisitor visitor, ref TDictionary container)
		{
			visitor.Visit<TDictionary, TKey, TValue>(this, ref container);
		}

		void IDictionaryPropertyAccept<TDictionary>.Accept<TContainer>(IDictionaryPropertyVisitor visitor, Property<TContainer, TDictionary> property, ref TContainer container, ref TDictionary dictionary)
		{
			using (new AttributesScope(this.m_KeyValuePairProperty, property))
			{
				visitor.Visit<TContainer, TDictionary, TKey, TValue>(property, ref container, ref dictionary);
			}
		}

		bool IKeyedProperties<TDictionary, object>.TryGetProperty(ref TDictionary container, object key, out IProperty<TDictionary> property)
		{
			bool flag = container.ContainsKey((TKey)((object)key));
			bool flag2;
			if (flag)
			{
				property = new KeyValueCollectionPropertyBag<TDictionary, TKey, TValue>.KeyValuePairProperty
				{
					Key = (TKey)((object)key)
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

		private readonly KeyValueCollectionPropertyBag<TDictionary, TKey, TValue>.KeyValuePairProperty m_KeyValuePairProperty = new KeyValueCollectionPropertyBag<TDictionary, TKey, TValue>.KeyValuePairProperty();

		private class KeyValuePairProperty : Property<TDictionary, KeyValuePair<TKey, TValue>>, IDictionaryElementProperty<TKey>, IDictionaryElementProperty, ICollectionElementProperty
		{
			public override string Name
			{
				get
				{
					TKey key = this.Key;
					return key.ToString();
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public override KeyValuePair<TKey, TValue> GetValue(ref TDictionary container)
			{
				return new KeyValuePair<TKey, TValue>(this.Key, container[this.Key]);
			}

			public override void SetValue(ref TDictionary container, KeyValuePair<TKey, TValue> value)
			{
				container[value.Key] = value.Value;
			}

			public TKey Key { get; internal set; }

			public object ObjectKey
			{
				get
				{
					return this.Key;
				}
			}
		}

		private readonly struct Enumerable : IEnumerable<IProperty<TDictionary>>, IEnumerable
		{
			public Enumerable(TDictionary dictionary, KeyValueCollectionPropertyBag<TDictionary, TKey, TValue>.KeyValuePairProperty property)
			{
				this.m_Dictionary = dictionary;
				this.m_Property = property;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new KeyValueCollectionPropertyBag<TDictionary, TKey, TValue>.Enumerable.Enumerator(this.m_Dictionary, this.m_Property);
			}

			IEnumerator<IProperty<TDictionary>> IEnumerable<IProperty<TDictionary>>.GetEnumerator()
			{
				return new KeyValueCollectionPropertyBag<TDictionary, TKey, TValue>.Enumerable.Enumerator(this.m_Dictionary, this.m_Property);
			}

			private readonly TDictionary m_Dictionary;

			private readonly KeyValueCollectionPropertyBag<TDictionary, TKey, TValue>.KeyValuePairProperty m_Property;

			private class Enumerator : IEnumerator<IProperty<TDictionary>>, IEnumerator, IDisposable
			{
				public Enumerator(TDictionary dictionary, KeyValueCollectionPropertyBag<TDictionary, TKey, TValue>.KeyValuePairProperty property)
				{
					this.m_Dictionary = dictionary;
					this.m_Property = property;
					this.m_Previous = property.Key;
					this.m_Position = -1;
					this.m_Keys = CollectionPool<List<TKey>, TKey>.Get();
					this.m_Keys.AddRange(this.m_Dictionary.Keys);
				}

				public IProperty<TDictionary> Current
				{
					get
					{
						return this.m_Property;
					}
				}

				object IEnumerator.Current
				{
					get
					{
						return this.Current;
					}
				}

				public bool MoveNext()
				{
					this.m_Position++;
					int position = this.m_Position;
					TDictionary dictionary = this.m_Dictionary;
					bool flag = position < dictionary.Count;
					bool flag2;
					if (flag)
					{
						this.m_Property.Key = this.m_Keys[this.m_Position];
						flag2 = true;
					}
					else
					{
						this.m_Property.Key = this.m_Previous;
						flag2 = false;
					}
					return flag2;
				}

				public void Reset()
				{
					this.m_Position = -1;
					this.m_Property.Key = this.m_Previous;
				}

				public void Dispose()
				{
					CollectionPool<List<TKey>, TKey>.Release(this.m_Keys);
				}

				private readonly TDictionary m_Dictionary;

				private readonly KeyValueCollectionPropertyBag<TDictionary, TKey, TValue>.KeyValuePairProperty m_Property;

				private readonly TKey m_Previous;

				private readonly List<TKey> m_Keys;

				private int m_Position;
			}
		}
	}
}
