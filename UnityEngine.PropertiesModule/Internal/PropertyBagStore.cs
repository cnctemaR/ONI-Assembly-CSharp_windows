using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Unity.Properties.Internal
{
	internal static class PropertyBagStore
	{
		private static ReflectedPropertyBagProvider ReflectedPropertyBagProvider
		{
			get
			{
				ReflectedPropertyBagProvider reflectedPropertyBagProvider;
				if ((reflectedPropertyBagProvider = PropertyBagStore.s_PropertyBagProvider) == null)
				{
					reflectedPropertyBagProvider = (PropertyBagStore.s_PropertyBagProvider = new ReflectedPropertyBagProvider());
				}
				return reflectedPropertyBagProvider;
			}
		}

		internal static List<Type> AllTypes
		{
			get
			{
				return PropertyBagStore.s_RegisteredTypes;
			}
		}

		internal static void CreatePropertyBagProvider()
		{
			PropertyBagStore.s_PropertyBagProvider = new ReflectedPropertyBagProvider();
		}

		internal static void AddPropertyBag<TContainer>(IPropertyBag<TContainer> propertyBag)
		{
			bool flag = !TypeTraits<TContainer>.IsContainer;
			if (flag)
			{
				throw new Exception(string.Format("PropertyBagStore Type=[{0}] is not a valid container type. Type can not be primitive, enum or string.", typeof(TContainer)));
			}
			bool isAbstractOrInterface = TypeTraits<TContainer>.IsAbstractOrInterface;
			if (isAbstractOrInterface)
			{
				throw new Exception(string.Format("PropertyBagStore Type=[{0}] is not a valid container type. Type can not be abstract or interface.", typeof(TContainer)));
			}
			bool flag2 = PropertyBagStore.TypedStore<TContainer>.PropertyBag != null;
			if (flag2)
			{
				IPropertyBag<TContainer> propertyBag2 = PropertyBagStore.TypedStore<TContainer>.PropertyBag;
				bool flag3 = propertyBag2.GetType().Assembly == typeof(TContainer).Assembly;
				if (flag3)
				{
					return;
				}
				bool flag4 = propertyBag.GetType().GetCustomAttributes<CompilerGeneratedAttribute>().Any<CompilerGeneratedAttribute>();
				if (flag4)
				{
					bool flag5 = propertyBag.GetType().Assembly != typeof(TContainer).Assembly;
					if (flag5)
					{
						return;
					}
				}
			}
			PropertyBagStore.TypedStore<TContainer>.PropertyBag = propertyBag;
			bool flag6 = !PropertyBagStore.s_PropertyBags.ContainsKey(typeof(TContainer));
			if (flag6)
			{
				PropertyBagStore.s_RegisteredTypes.Add(typeof(TContainer));
			}
			PropertyBagStore.s_PropertyBags[typeof(TContainer)] = propertyBag;
		}

		internal static IPropertyBag<TContainer> GetPropertyBag<TContainer>()
		{
			bool flag = PropertyBagStore.TypedStore<TContainer>.PropertyBag != null;
			IPropertyBag<TContainer> propertyBag;
			if (flag)
			{
				propertyBag = PropertyBagStore.TypedStore<TContainer>.PropertyBag;
			}
			else
			{
				IPropertyBag propertyBag2 = PropertyBagStore.GetPropertyBag(typeof(TContainer));
				bool flag2 = propertyBag2 == null;
				if (flag2)
				{
					propertyBag = null;
				}
				else
				{
					IPropertyBag<TContainer> propertyBag3 = propertyBag2 as IPropertyBag<TContainer>;
					bool flag3 = propertyBag3 == null;
					if (flag3)
					{
						throw new InvalidOperationException("PropertyBag type container type mismatch.");
					}
					propertyBag = propertyBag3;
				}
			}
			return propertyBag;
		}

		internal static IPropertyBag GetPropertyBag(Type type)
		{
			IPropertyBag propertyBag;
			bool flag = PropertyBagStore.s_PropertyBags.TryGetValue(type, out propertyBag);
			IPropertyBag propertyBag2;
			if (flag)
			{
				propertyBag2 = propertyBag;
			}
			else
			{
				bool flag2 = !TypeTraits.IsContainer(type);
				if (flag2)
				{
					propertyBag2 = null;
				}
				else
				{
					bool flag3 = type.IsArray && type.GetArrayRank() != 1;
					if (flag3)
					{
						propertyBag2 = null;
					}
					else
					{
						bool flag4 = type.IsInterface || type.IsAbstract;
						if (flag4)
						{
							propertyBag2 = null;
						}
						else
						{
							bool flag5 = type == typeof(object);
							if (flag5)
							{
								propertyBag2 = null;
							}
							else
							{
								propertyBag = PropertyBagStore.ReflectedPropertyBagProvider.CreatePropertyBag(type);
								bool flag6 = propertyBag == null;
								if (flag6)
								{
									PropertyBagStore.s_PropertyBags.TryAdd(type, null);
									propertyBag2 = null;
								}
								else
								{
									IPropertyBagRegister propertyBagRegister = propertyBag as IPropertyBagRegister;
									if (propertyBagRegister != null)
									{
										propertyBagRegister.Register();
									}
									propertyBag2 = propertyBag;
								}
							}
						}
					}
				}
			}
			return propertyBag2;
		}

		internal static bool Exists<TContainer>()
		{
			return PropertyBagStore.TypedStore<TContainer>.PropertyBag != null;
		}

		internal static bool Exists(Type type)
		{
			return PropertyBagStore.s_PropertyBags.ContainsKey(type);
		}

		internal static bool Exists<TContainer>(ref TContainer value)
		{
			bool flag = !TypeTraits<TContainer>.CanBeNull;
			bool flag2;
			if (flag)
			{
				flag2 = PropertyBagStore.GetPropertyBag<TContainer>() != null;
			}
			else
			{
				bool flag3 = EqualityComparer<TContainer>.Default.Equals(value, default(TContainer));
				flag2 = !flag3 && PropertyBagStore.GetPropertyBag(value.GetType()) != null;
			}
			return flag2;
		}

		internal static bool TryGetPropertyBagForValue<TValue>(ref TValue value, out IPropertyBag propertyBag)
		{
			bool flag = !TypeTraits<TValue>.IsContainer;
			bool flag2;
			if (flag)
			{
				propertyBag = null;
				flag2 = false;
			}
			else
			{
				bool canBeNull = TypeTraits<TValue>.CanBeNull;
				if (canBeNull)
				{
					bool flag3 = EqualityComparer<TValue>.Default.Equals(value, default(TValue));
					if (flag3)
					{
						propertyBag = PropertyBagStore.GetPropertyBag<TValue>();
						return propertyBag != null;
					}
				}
				bool isValueType = TypeTraits<TValue>.IsValueType;
				if (isValueType)
				{
					propertyBag = PropertyBagStore.GetPropertyBag<TValue>();
					flag2 = propertyBag != null;
				}
				else
				{
					propertyBag = PropertyBagStore.GetPropertyBag(value.GetType());
					flag2 = propertyBag != null;
				}
			}
			return flag2;
		}

		private static readonly ConcurrentDictionary<Type, IPropertyBag> s_PropertyBags = new ConcurrentDictionary<Type, IPropertyBag>();

		private static readonly List<Type> s_RegisteredTypes = new List<Type>();

		private static ReflectedPropertyBagProvider s_PropertyBagProvider = null;

		internal struct TypedStore<TContainer>
		{
			public static IPropertyBag<TContainer> PropertyBag;
		}
	}
}
