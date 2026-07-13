using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;

namespace Unity.Properties.Internal
{
	internal class ReflectedPropertyBagProvider
	{
		public ReflectedPropertyBagProvider()
		{
			this.m_CreatePropertyMethod = typeof(ReflectedPropertyBagProvider).GetMethod("CreateProperty", BindingFlags.Instance | BindingFlags.NonPublic);
			this.m_CreatePropertyBagMethod = typeof(ReflectedPropertyBagProvider).GetMethods(BindingFlags.Instance | BindingFlags.Public).First<MethodInfo>((MethodInfo x) => x.Name == "CreatePropertyBag" && x.IsGenericMethod);
			this.m_CreateIndexedCollectionPropertyBagMethod = typeof(ReflectedPropertyBagProvider).GetMethod("CreateIndexedCollectionPropertyBag", BindingFlags.Instance | BindingFlags.NonPublic);
			this.m_CreateSetPropertyBagMethod = typeof(ReflectedPropertyBagProvider).GetMethod("CreateSetPropertyBag", BindingFlags.Instance | BindingFlags.NonPublic);
			this.m_CreateKeyValueCollectionPropertyBagMethod = typeof(ReflectedPropertyBagProvider).GetMethod("CreateKeyValueCollectionPropertyBag", BindingFlags.Instance | BindingFlags.NonPublic);
			this.m_CreateKeyValuePairPropertyBagMethod = typeof(ReflectedPropertyBagProvider).GetMethod("CreateKeyValuePairPropertyBag", BindingFlags.Instance | BindingFlags.NonPublic);
			this.m_CreateArrayPropertyBagMethod = typeof(ReflectedPropertyBagProvider).GetMethod("CreateArrayPropertyBag", BindingFlags.Instance | BindingFlags.NonPublic);
			this.m_CreateListPropertyBagMethod = typeof(ReflectedPropertyBagProvider).GetMethod("CreateListPropertyBag", BindingFlags.Instance | BindingFlags.NonPublic);
			this.m_CreateHashSetPropertyBagMethod = typeof(ReflectedPropertyBagProvider).GetMethod("CreateHashSetPropertyBag", BindingFlags.Instance | BindingFlags.NonPublic);
			this.m_CreateDictionaryPropertyBagMethod = typeof(ReflectedPropertyBagProvider).GetMethod("CreateDictionaryPropertyBag", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		public IPropertyBag CreatePropertyBag(Type type)
		{
			bool isGenericTypeDefinition = type.IsGenericTypeDefinition;
			IPropertyBag propertyBag;
			if (isGenericTypeDefinition)
			{
				propertyBag = null;
			}
			else
			{
				propertyBag = (IPropertyBag)this.m_CreatePropertyBagMethod.MakeGenericMethod(new Type[] { type }).Invoke(this, null);
			}
			return propertyBag;
		}

		public IPropertyBag<TContainer> CreatePropertyBag<TContainer>()
		{
			bool flag = !TypeTraits<TContainer>.IsContainer || TypeTraits<TContainer>.IsObject;
			if (flag)
			{
				throw new InvalidOperationException("Invalid container type.");
			}
			bool isArray = typeof(TContainer).IsArray;
			IPropertyBag<TContainer> propertyBag;
			if (isArray)
			{
				bool flag2 = typeof(TContainer).GetArrayRank() != 1;
				if (flag2)
				{
					throw new InvalidOperationException("Properties does not support multidimensional arrays.");
				}
				propertyBag = (IPropertyBag<TContainer>)this.m_CreateArrayPropertyBagMethod.MakeGenericMethod(new Type[] { typeof(TContainer).GetElementType() }).Invoke(this, new object[0]);
			}
			else
			{
				bool flag3 = typeof(TContainer).IsGenericType && typeof(TContainer).GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
				if (flag3)
				{
					propertyBag = (IPropertyBag<TContainer>)this.m_CreateListPropertyBagMethod.MakeGenericMethod(new Type[] { typeof(TContainer).GetGenericArguments().First<Type>() }).Invoke(this, new object[0]);
				}
				else
				{
					bool flag4 = typeof(TContainer).IsGenericType && typeof(TContainer).GetGenericTypeDefinition().IsAssignableFrom(typeof(HashSet<>));
					if (flag4)
					{
						propertyBag = (IPropertyBag<TContainer>)this.m_CreateHashSetPropertyBagMethod.MakeGenericMethod(new Type[] { typeof(TContainer).GetGenericArguments().First<Type>() }).Invoke(this, new object[0]);
					}
					else
					{
						bool flag5 = typeof(TContainer).IsGenericType && typeof(TContainer).GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<, >));
						if (flag5)
						{
							propertyBag = (IPropertyBag<TContainer>)this.m_CreateDictionaryPropertyBagMethod.MakeGenericMethod(new Type[]
							{
								typeof(TContainer).GetGenericArguments().First<Type>(),
								typeof(TContainer).GetGenericArguments().ElementAt<Type>(1)
							}).Invoke(this, new object[0]);
						}
						else
						{
							bool flag6 = typeof(TContainer).IsGenericType && typeof(TContainer).GetGenericTypeDefinition().IsAssignableFrom(typeof(IList<>));
							if (flag6)
							{
								propertyBag = (IPropertyBag<TContainer>)this.m_CreateIndexedCollectionPropertyBagMethod.MakeGenericMethod(new Type[]
								{
									typeof(TContainer),
									typeof(TContainer).GetGenericArguments().First<Type>()
								}).Invoke(this, new object[0]);
							}
							else
							{
								bool flag7 = typeof(TContainer).IsGenericType && typeof(TContainer).GetGenericTypeDefinition().IsAssignableFrom(typeof(ISet<>));
								if (flag7)
								{
									propertyBag = (IPropertyBag<TContainer>)this.m_CreateSetPropertyBagMethod.MakeGenericMethod(new Type[]
									{
										typeof(TContainer),
										typeof(TContainer).GetGenericArguments().First<Type>()
									}).Invoke(this, new object[0]);
								}
								else
								{
									bool flag8 = typeof(TContainer).IsGenericType && typeof(TContainer).GetGenericTypeDefinition().IsAssignableFrom(typeof(IDictionary<, >));
									if (flag8)
									{
										propertyBag = (IPropertyBag<TContainer>)this.m_CreateKeyValueCollectionPropertyBagMethod.MakeGenericMethod(new Type[]
										{
											typeof(TContainer),
											typeof(TContainer).GetGenericArguments().First<Type>(),
											typeof(TContainer).GetGenericArguments().ElementAt<Type>(1)
										}).Invoke(this, new object[0]);
									}
									else
									{
										bool flag9 = typeof(TContainer).IsGenericType && typeof(TContainer).GetGenericTypeDefinition().IsAssignableFrom(typeof(KeyValuePair<, >));
										if (flag9)
										{
											Type[] array = typeof(TContainer).GetGenericArguments().ToArray<Type>();
											propertyBag = (IPropertyBag<TContainer>)this.m_CreateKeyValuePairPropertyBagMethod.MakeGenericMethod(new Type[]
											{
												array[0],
												array[1]
											}).Invoke(this, new object[0]);
										}
										else
										{
											ReflectedPropertyBag<TContainer> reflectedPropertyBag = new ReflectedPropertyBag<TContainer>();
											foreach (MemberInfo memberInfo in ReflectedPropertyBagProvider.GetPropertyMembers(typeof(TContainer)))
											{
												MemberInfo memberInfo2 = memberInfo;
												MemberInfo memberInfo3 = memberInfo2;
												FieldInfo fieldInfo = memberInfo3 as FieldInfo;
												IMemberInfo memberInfo4;
												if (fieldInfo == null)
												{
													PropertyInfo propertyInfo = memberInfo3 as PropertyInfo;
													if (propertyInfo == null)
													{
														throw new InvalidOperationException();
													}
													memberInfo4 = new PropertyMember(propertyInfo);
												}
												else
												{
													memberInfo4 = new FieldMember(fieldInfo);
												}
												this.m_CreatePropertyMethod.MakeGenericMethod(new Type[]
												{
													typeof(TContainer),
													memberInfo4.ValueType
												}).Invoke(this, new object[] { memberInfo4, reflectedPropertyBag });
											}
											propertyBag = reflectedPropertyBag;
										}
									}
								}
							}
						}
					}
				}
			}
			return propertyBag;
		}

		[Preserve]
		private void CreateProperty<TContainer, TValue>(IMemberInfo member, ReflectedPropertyBag<TContainer> propertyBag)
		{
			bool isPointer = typeof(TValue).IsPointer;
			if (!isPointer)
			{
				propertyBag.AddProperty<TValue>(new ReflectedMemberProperty<TContainer, TValue>(member, member.Name));
			}
		}

		[Preserve]
		private IPropertyBag<TList> CreateIndexedCollectionPropertyBag<TList, TElement>() where TList : IList<TElement>
		{
			return new IndexedCollectionPropertyBag<TList, TElement>();
		}

		[Preserve]
		private IPropertyBag<TSet> CreateSetPropertyBag<TSet, TValue>() where TSet : ISet<TValue>
		{
			return new SetPropertyBagBase<TSet, TValue>();
		}

		[Preserve]
		private IPropertyBag<TDictionary> CreateKeyValueCollectionPropertyBag<TDictionary, TKey, TValue>() where TDictionary : IDictionary<TKey, TValue>
		{
			return new KeyValueCollectionPropertyBag<TDictionary, TKey, TValue>();
		}

		[Preserve]
		private IPropertyBag<KeyValuePair<TKey, TValue>> CreateKeyValuePairPropertyBag<TKey, TValue>()
		{
			return new KeyValuePairPropertyBag<TKey, TValue>();
		}

		[Preserve]
		private IPropertyBag<TElement[]> CreateArrayPropertyBag<TElement>()
		{
			return new ArrayPropertyBag<TElement>();
		}

		[Preserve]
		private IPropertyBag<List<TElement>> CreateListPropertyBag<TElement>()
		{
			return new ListPropertyBag<TElement>();
		}

		[Preserve]
		private IPropertyBag<HashSet<TElement>> CreateHashSetPropertyBag<TElement>()
		{
			return new HashSetPropertyBag<TElement>();
		}

		[Preserve]
		private IPropertyBag<Dictionary<TKey, TValue>> CreateDictionaryPropertyBag<TKey, TValue>()
		{
			return new DictionaryPropertyBag<TKey, TValue>();
		}

		private static IEnumerable<MemberInfo> GetPropertyMembers(Type type)
		{
			do
			{
				IOrderedEnumerable<MemberInfo> members = from x in type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
					orderby x.MetadataToken
					select x;
				foreach (MemberInfo member in members)
				{
					bool flag = member.MemberType != MemberTypes.Field && member.MemberType != MemberTypes.Property;
					if (!flag)
					{
						bool flag2 = member.DeclaringType != type;
						if (!flag2)
						{
							bool flag3 = !ReflectedPropertyBagProvider.IsValidMember(member);
							if (!flag3)
							{
								bool hasDontCreatePropertyAttribute = member.GetCustomAttribute<DontCreatePropertyAttribute>() != null;
								bool hasCreatePropertyAttribute = member.GetCustomAttribute<CreatePropertyAttribute>() != null;
								bool hasNonSerializedAttribute = member.GetCustomAttribute<NonSerializedAttribute>() != null;
								bool hasSerializedFieldAttribute = member.GetCustomAttribute<SerializeField>() != null;
								bool hasSerializeReferenceAttribute = member.GetCustomAttribute<SerializeReference>() != null;
								bool flag4 = hasDontCreatePropertyAttribute;
								if (!flag4)
								{
									bool flag5 = hasCreatePropertyAttribute;
									if (flag5)
									{
										yield return member;
									}
									else
									{
										bool flag6 = hasNonSerializedAttribute;
										if (!flag6)
										{
											bool flag7 = hasSerializedFieldAttribute;
											if (flag7)
											{
												yield return member;
											}
											else
											{
												bool flag8 = hasSerializeReferenceAttribute;
												if (flag8)
												{
													yield return member;
												}
												else
												{
													FieldInfo field = member as FieldInfo;
													bool flag9 = field != null && field.IsPublic;
													if (flag9)
													{
														yield return member;
													}
													field = null;
													member = null;
												}
											}
										}
									}
								}
							}
						}
					}
				}
				IEnumerator<MemberInfo> enumerator = null;
				type = type.BaseType;
				members = null;
			}
			while (type != null && type != typeof(object));
			yield break;
			yield break;
		}

		private static bool IsValidMember(MemberInfo memberInfo)
		{
			FieldInfo fieldInfo = memberInfo as FieldInfo;
			bool flag;
			if (fieldInfo == null)
			{
				PropertyInfo propertyInfo = memberInfo as PropertyInfo;
				flag = propertyInfo != null && (null != propertyInfo.GetMethod && !propertyInfo.GetMethod.IsStatic) && ReflectedPropertyBagProvider.IsValidPropertyType(propertyInfo.PropertyType);
			}
			else
			{
				flag = !fieldInfo.IsStatic && ReflectedPropertyBagProvider.IsValidPropertyType(fieldInfo.FieldType);
			}
			return flag;
		}

		private static bool IsValidPropertyType(Type type)
		{
			bool isPointer = type.IsPointer;
			return !isPointer && (!type.IsGenericType || type.GetGenericArguments().All<Type>(new Func<Type, bool>(ReflectedPropertyBagProvider.IsValidPropertyType)));
		}

		private readonly MethodInfo m_CreatePropertyMethod;

		private readonly MethodInfo m_CreatePropertyBagMethod;

		private readonly MethodInfo m_CreateIndexedCollectionPropertyBagMethod;

		private readonly MethodInfo m_CreateSetPropertyBagMethod;

		private readonly MethodInfo m_CreateKeyValueCollectionPropertyBagMethod;

		private readonly MethodInfo m_CreateKeyValuePairPropertyBagMethod;

		private readonly MethodInfo m_CreateArrayPropertyBagMethod;

		private readonly MethodInfo m_CreateListPropertyBagMethod;

		private readonly MethodInfo m_CreateHashSetPropertyBagMethod;

		private readonly MethodInfo m_CreateDictionaryPropertyBagMethod;
	}
}
