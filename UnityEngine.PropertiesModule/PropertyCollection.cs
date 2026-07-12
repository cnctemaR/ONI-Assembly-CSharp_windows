using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.Properties
{
	public readonly struct PropertyCollection<TContainer> : IEnumerable<IProperty<TContainer>>, IEnumerable
	{
		public static PropertyCollection<TContainer> Empty { get; } = default(PropertyCollection<TContainer>);

		public PropertyCollection(IEnumerable<IProperty<TContainer>> enumerable)
		{
			this.m_Type = PropertyCollection<TContainer>.EnumeratorType.Enumerable;
			this.m_Enumerable = enumerable;
			this.m_Properties = null;
			this.m_IndexedCollectionPropertyBag = default(IndexedCollectionPropertyBagEnumerable<TContainer>);
		}

		public PropertyCollection(List<IProperty<TContainer>> properties)
		{
			this.m_Type = PropertyCollection<TContainer>.EnumeratorType.List;
			this.m_Enumerable = null;
			this.m_Properties = properties;
			this.m_IndexedCollectionPropertyBag = default(IndexedCollectionPropertyBagEnumerable<TContainer>);
		}

		internal PropertyCollection(IndexedCollectionPropertyBagEnumerable<TContainer> enumerable)
		{
			this.m_Type = PropertyCollection<TContainer>.EnumeratorType.IndexedCollectionPropertyBag;
			this.m_Enumerable = null;
			this.m_Properties = null;
			this.m_IndexedCollectionPropertyBag = enumerable;
		}

		public PropertyCollection<TContainer>.Enumerator GetEnumerator()
		{
			PropertyCollection<TContainer>.Enumerator enumerator;
			switch (this.m_Type)
			{
			case PropertyCollection<TContainer>.EnumeratorType.Empty:
				enumerator = default(PropertyCollection<TContainer>.Enumerator);
				break;
			case PropertyCollection<TContainer>.EnumeratorType.Enumerable:
				enumerator = new PropertyCollection<TContainer>.Enumerator(this.m_Enumerable.GetEnumerator());
				break;
			case PropertyCollection<TContainer>.EnumeratorType.List:
				enumerator = new PropertyCollection<TContainer>.Enumerator(this.m_Properties.GetEnumerator());
				break;
			case PropertyCollection<TContainer>.EnumeratorType.IndexedCollectionPropertyBag:
				enumerator = new PropertyCollection<TContainer>.Enumerator(this.m_IndexedCollectionPropertyBag.GetEnumerator());
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return enumerator;
		}

		IEnumerator<IProperty<TContainer>> IEnumerable<IProperty<TContainer>>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private readonly PropertyCollection<TContainer>.EnumeratorType m_Type;

		private readonly IEnumerable<IProperty<TContainer>> m_Enumerable;

		private readonly List<IProperty<TContainer>> m_Properties;

		private readonly IndexedCollectionPropertyBagEnumerable<TContainer> m_IndexedCollectionPropertyBag;

		private enum EnumeratorType
		{
			Empty,
			Enumerable,
			List,
			IndexedCollectionPropertyBag
		}

		public struct Enumerator : IEnumerator<IProperty<TContainer>>, IEnumerator, IDisposable
		{
			public IProperty<TContainer> Current { readonly get; private set; }

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal Enumerator(IEnumerator<IProperty<TContainer>> enumerator)
			{
				this.m_Type = PropertyCollection<TContainer>.EnumeratorType.Enumerable;
				this.m_Enumerator = enumerator;
				this.m_Properties = default(List<IProperty<TContainer>>.Enumerator);
				this.m_IndexedCollectionPropertyBag = default(IndexedCollectionPropertyBagEnumerator<TContainer>);
				this.Current = null;
			}

			internal Enumerator(List<IProperty<TContainer>>.Enumerator properties)
			{
				this.m_Type = PropertyCollection<TContainer>.EnumeratorType.List;
				this.m_Enumerator = null;
				this.m_Properties = properties;
				this.m_IndexedCollectionPropertyBag = default(IndexedCollectionPropertyBagEnumerator<TContainer>);
				this.Current = null;
			}

			internal Enumerator(IndexedCollectionPropertyBagEnumerator<TContainer> enumerator)
			{
				this.m_Type = PropertyCollection<TContainer>.EnumeratorType.IndexedCollectionPropertyBag;
				this.m_Enumerator = null;
				this.m_Properties = default(List<IProperty<TContainer>>.Enumerator);
				this.m_IndexedCollectionPropertyBag = enumerator;
				this.Current = null;
			}

			public bool MoveNext()
			{
				bool flag;
				switch (this.m_Type)
				{
				case PropertyCollection<TContainer>.EnumeratorType.Empty:
					return false;
				case PropertyCollection<TContainer>.EnumeratorType.Enumerable:
					flag = this.m_Enumerator.MoveNext();
					this.Current = this.m_Enumerator.Current;
					break;
				case PropertyCollection<TContainer>.EnumeratorType.List:
					flag = this.m_Properties.MoveNext();
					this.Current = this.m_Properties.Current;
					break;
				case PropertyCollection<TContainer>.EnumeratorType.IndexedCollectionPropertyBag:
					flag = this.m_IndexedCollectionPropertyBag.MoveNext();
					this.Current = this.m_IndexedCollectionPropertyBag.Current;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				return flag;
			}

			public void Reset()
			{
				switch (this.m_Type)
				{
				case PropertyCollection<TContainer>.EnumeratorType.Empty:
					break;
				case PropertyCollection<TContainer>.EnumeratorType.Enumerable:
					this.m_Enumerator.Reset();
					break;
				case PropertyCollection<TContainer>.EnumeratorType.List:
					((IEnumerator)this.m_Properties).Reset();
					break;
				case PropertyCollection<TContainer>.EnumeratorType.IndexedCollectionPropertyBag:
					this.m_IndexedCollectionPropertyBag.Reset();
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}

			public void Dispose()
			{
				switch (this.m_Type)
				{
				case PropertyCollection<TContainer>.EnumeratorType.Empty:
					break;
				case PropertyCollection<TContainer>.EnumeratorType.Enumerable:
					this.m_Enumerator.Dispose();
					break;
				case PropertyCollection<TContainer>.EnumeratorType.List:
					break;
				case PropertyCollection<TContainer>.EnumeratorType.IndexedCollectionPropertyBag:
					this.m_IndexedCollectionPropertyBag.Dispose();
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}

			private readonly PropertyCollection<TContainer>.EnumeratorType m_Type;

			private IEnumerator<IProperty<TContainer>> m_Enumerator;

			private List<IProperty<TContainer>>.Enumerator m_Properties;

			private IndexedCollectionPropertyBagEnumerator<TContainer> m_IndexedCollectionPropertyBag;
		}
	}
}
