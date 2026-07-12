using System;
using System.Collections.Generic;
using Unity.Properties.Internal;

namespace Unity.Properties
{
	public static class PropertyBag
	{
		public static void AcceptWithSpecializedVisitor<TContainer>(IPropertyBag<TContainer> properties, IPropertyBagVisitor visitor, ref TContainer container)
		{
			bool flag = properties == null;
			if (flag)
			{
				throw new ArgumentNullException("properties");
			}
			IDictionaryPropertyBagAccept<TContainer> dictionaryPropertyBagAccept = properties as IDictionaryPropertyBagAccept<TContainer>;
			if (dictionaryPropertyBagAccept != null)
			{
				IDictionaryPropertyBagVisitor dictionaryPropertyBagVisitor = visitor as IDictionaryPropertyBagVisitor;
				if (dictionaryPropertyBagVisitor != null)
				{
					dictionaryPropertyBagAccept.Accept(dictionaryPropertyBagVisitor, ref container);
					return;
				}
			}
			IListPropertyBagAccept<TContainer> listPropertyBagAccept = properties as IListPropertyBagAccept<TContainer>;
			if (listPropertyBagAccept != null)
			{
				IListPropertyBagVisitor listPropertyBagVisitor = visitor as IListPropertyBagVisitor;
				if (listPropertyBagVisitor != null)
				{
					listPropertyBagAccept.Accept(listPropertyBagVisitor, ref container);
					return;
				}
			}
			ISetPropertyBagAccept<TContainer> setPropertyBagAccept = properties as ISetPropertyBagAccept<TContainer>;
			if (setPropertyBagAccept != null)
			{
				ISetPropertyBagVisitor setPropertyBagVisitor = visitor as ISetPropertyBagVisitor;
				if (setPropertyBagVisitor != null)
				{
					setPropertyBagAccept.Accept(setPropertyBagVisitor, ref container);
					return;
				}
			}
			ICollectionPropertyBagAccept<TContainer> collectionPropertyBagAccept = properties as ICollectionPropertyBagAccept<TContainer>;
			if (collectionPropertyBagAccept != null)
			{
				ICollectionPropertyBagVisitor collectionPropertyBagVisitor = visitor as ICollectionPropertyBagVisitor;
				if (collectionPropertyBagVisitor != null)
				{
					collectionPropertyBagAccept.Accept(collectionPropertyBagVisitor, ref container);
					return;
				}
			}
			properties.Accept(visitor, ref container);
		}

		public static void Register<TContainer>(PropertyBag<TContainer> propertyBag)
		{
			PropertyBagStore.AddPropertyBag<TContainer>(propertyBag);
		}

		public static void RegisterArray<TElement>()
		{
			bool flag = PropertyBagStore.TypedStore<IPropertyBag<TElement[]>>.PropertyBag == null;
			if (flag)
			{
				PropertyBagStore.AddPropertyBag<TElement[]>(new ArrayPropertyBag<TElement>());
			}
		}

		public static void RegisterArray<TContainer, TElement>()
		{
			PropertyBag.RegisterArray<TElement>();
		}

		public static void RegisterList<TElement>()
		{
			bool flag = PropertyBagStore.TypedStore<IPropertyBag<TElement[]>>.PropertyBag == null;
			if (flag)
			{
				PropertyBagStore.AddPropertyBag<List<TElement>>(new ListPropertyBag<TElement>());
			}
		}

		public static void RegisterList<TContainer, TElement>()
		{
			PropertyBag.RegisterList<TElement>();
		}

		public static void RegisterHashSet<TElement>()
		{
			bool flag = PropertyBagStore.TypedStore<IPropertyBag<HashSet<TElement>>>.PropertyBag == null;
			if (flag)
			{
				PropertyBagStore.AddPropertyBag<HashSet<TElement>>(new HashSetPropertyBag<TElement>());
			}
		}

		public static void RegisterHashSet<TContainer, TElement>()
		{
			PropertyBag.RegisterHashSet<TElement>();
		}

		public static void RegisterDictionary<TKey, TValue>()
		{
			bool flag = PropertyBagStore.TypedStore<IPropertyBag<Dictionary<TKey, TValue>>>.PropertyBag == null;
			if (flag)
			{
				PropertyBagStore.AddPropertyBag<Dictionary<TKey, TValue>>(new DictionaryPropertyBag<TKey, TValue>());
			}
		}

		public static void RegisterDictionary<TContainer, TKey, TValue>()
		{
			PropertyBag.RegisterDictionary<TKey, TValue>();
		}

		public static void RegisterIList<TList, TElement>() where TList : IList<TElement>
		{
			bool flag = PropertyBagStore.TypedStore<IPropertyBag<TList>>.PropertyBag == null;
			if (flag)
			{
				PropertyBagStore.AddPropertyBag<TList>(new IndexedCollectionPropertyBag<TList, TElement>());
			}
		}

		public static void RegisterIList<TContainer, TList, TElement>() where TList : IList<TElement>
		{
			PropertyBag.RegisterIList<TList, TElement>();
		}

		public static void RegisterISet<TSet, TElement>() where TSet : ISet<TElement>
		{
			bool flag = PropertyBagStore.TypedStore<IPropertyBag<TSet>>.PropertyBag == null;
			if (flag)
			{
				PropertyBagStore.AddPropertyBag<TSet>(new SetPropertyBagBase<TSet, TElement>());
			}
		}

		public static void RegisterISet<TContainer, TSet, TElement>() where TSet : ISet<TElement>
		{
			PropertyBag.RegisterISet<TSet, TElement>();
		}

		public static void RegisterIDictionary<TDictionary, TKey, TValue>() where TDictionary : IDictionary<TKey, TValue>
		{
			bool flag = PropertyBagStore.TypedStore<IPropertyBag<TDictionary>>.PropertyBag == null;
			if (flag)
			{
				PropertyBagStore.AddPropertyBag<TDictionary>(new KeyValueCollectionPropertyBag<TDictionary, TKey, TValue>());
				PropertyBagStore.AddPropertyBag<KeyValuePair<TKey, TValue>>(new KeyValuePairPropertyBag<TKey, TValue>());
			}
		}

		public static void RegisterIDictionary<TContainer, TDictionary, TKey, TValue>() where TDictionary : IDictionary<TKey, TValue>
		{
			PropertyBag.RegisterIDictionary<TDictionary, TKey, TValue>();
		}

		public static TContainer CreateInstance<TContainer>()
		{
			IPropertyBag<TContainer> propertyBag = PropertyBagStore.GetPropertyBag<TContainer>();
			bool flag = propertyBag == null;
			if (flag)
			{
				throw new MissingPropertyBagException(typeof(TContainer));
			}
			return propertyBag.CreateInstance();
		}

		public static IPropertyBag GetPropertyBag(Type type)
		{
			return PropertyBagStore.GetPropertyBag(type);
		}

		public static IPropertyBag<TContainer> GetPropertyBag<TContainer>()
		{
			return PropertyBagStore.GetPropertyBag<TContainer>();
		}

		public static bool TryGetPropertyBagForValue<TValue>(ref TValue value, out IPropertyBag propertyBag)
		{
			return PropertyBagStore.TryGetPropertyBagForValue<TValue>(ref value, out propertyBag);
		}

		public static bool Exists<TContainer>()
		{
			return PropertyBagStore.Exists<TContainer>();
		}

		public static bool Exists(Type type)
		{
			return PropertyBagStore.Exists(type);
		}

		public static IEnumerable<Type> GetAllTypesWithAPropertyBag()
		{
			return PropertyBagStore.AllTypes;
		}
	}
}
