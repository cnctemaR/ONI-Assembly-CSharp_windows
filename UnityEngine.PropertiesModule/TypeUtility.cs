using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity.Properties.Internal;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Scripting;

namespace Unity.Properties
{
	public static class TypeUtility
	{
		static TypeUtility()
		{
			TypeUtility.SetExplicitInstantiationMethod<string>(() => string.Empty);
			foreach (MethodInfo methodInfo in typeof(TypeUtility).GetMethods(BindingFlags.Static | BindingFlags.NonPublic))
			{
				bool flag = methodInfo.Name != "CreateTypeConstructor" || !methodInfo.IsGenericMethod;
				if (!flag)
				{
					TypeUtility.s_CreateTypeConstructor = methodInfo;
					break;
				}
			}
			bool flag2 = null == TypeUtility.s_CreateTypeConstructor;
			if (flag2)
			{
				throw new InvalidProgramException();
			}
		}

		public static string GetTypeDisplayName(Type type)
		{
			string typeDisplayName;
			bool flag = TypeUtility.s_CachedResolvedName.TryGetValue(type, out typeDisplayName);
			string text;
			if (flag)
			{
				text = typeDisplayName;
			}
			else
			{
				int num = 0;
				typeDisplayName = TypeUtility.GetTypeDisplayName(type, type.GetGenericArguments(), ref num);
				TypeUtility.s_CachedResolvedName[type] = typeDisplayName;
				text = typeDisplayName;
			}
			return text;
		}

		private static string GetTypeDisplayName(Type type, IReadOnlyList<Type> args, ref int argIndex)
		{
			bool flag = type == typeof(int);
			string text;
			if (flag)
			{
				text = "int";
			}
			else
			{
				bool flag2 = type == typeof(uint);
				if (flag2)
				{
					text = "uint";
				}
				else
				{
					bool flag3 = type == typeof(short);
					if (flag3)
					{
						text = "short";
					}
					else
					{
						bool flag4 = type == typeof(ushort);
						if (flag4)
						{
							text = "ushort";
						}
						else
						{
							bool flag5 = type == typeof(byte);
							if (flag5)
							{
								text = "byte";
							}
							else
							{
								bool flag6 = type == typeof(char);
								if (flag6)
								{
									text = "char";
								}
								else
								{
									bool flag7 = type == typeof(bool);
									if (flag7)
									{
										text = "bool";
									}
									else
									{
										bool flag8 = type == typeof(long);
										if (flag8)
										{
											text = "long";
										}
										else
										{
											bool flag9 = type == typeof(ulong);
											if (flag9)
											{
												text = "ulong";
											}
											else
											{
												bool flag10 = type == typeof(float);
												if (flag10)
												{
													text = "float";
												}
												else
												{
													bool flag11 = type == typeof(double);
													if (flag11)
													{
														text = "double";
													}
													else
													{
														bool flag12 = type == typeof(string);
														if (flag12)
														{
															text = "string";
														}
														else
														{
															string text2 = type.Name;
															bool isGenericParameter = type.IsGenericParameter;
															if (isGenericParameter)
															{
																text = text2;
															}
															else
															{
																bool isNested = type.IsNested;
																if (isNested)
																{
																	text2 = TypeUtility.GetTypeDisplayName(type.DeclaringType, args, ref argIndex) + "." + text2;
																}
																bool flag13 = !type.IsGenericType;
																if (flag13)
																{
																	text = text2;
																}
																else
																{
																	int num = text2.IndexOf('`');
																	int num2 = type.GetGenericArguments().Length;
																	bool flag14 = num > -1;
																	if (flag14)
																	{
																		num2 = int.Parse(text2.Substring(num + 1));
																		text2 = text2.Remove(num);
																	}
																	StringBuilder stringBuilder = null;
																	object obj = TypeUtility.syncedPoolObject;
																	lock (obj)
																	{
																		stringBuilder = TypeUtility.s_Builders.Get();
																	}
																	try
																	{
																		int num3 = 0;
																		while (num3 < num2 && argIndex < args.Count)
																		{
																			bool flag16 = num3 != 0;
																			if (flag16)
																			{
																				stringBuilder.Append(", ");
																			}
																			stringBuilder.Append(TypeUtility.GetTypeDisplayName(args[argIndex]));
																			num3++;
																			argIndex++;
																		}
																		bool flag17 = stringBuilder.Length > 0;
																		if (flag17)
																		{
																			text2 = string.Format("{0}<{1}>", text2, stringBuilder);
																		}
																	}
																	finally
																	{
																		object obj2 = TypeUtility.syncedPoolObject;
																		lock (obj2)
																		{
																			TypeUtility.s_Builders.Release(stringBuilder);
																		}
																	}
																	text = text2;
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return text;
		}

		public static Type GetRootType(this Type type)
		{
			bool isInterface = type.IsInterface;
			Type type2;
			if (isInterface)
			{
				type2 = null;
			}
			else
			{
				Type type3 = (type.IsValueType ? typeof(ValueType) : typeof(object));
				while (type3 != type.BaseType)
				{
					type = type.BaseType;
				}
				type2 = type;
			}
			return type2;
		}

		[Preserve]
		private static TypeUtility.ITypeConstructor CreateTypeConstructor(Type type)
		{
			IPropertyBag propertyBag = PropertyBagStore.GetPropertyBag(type);
			bool flag = propertyBag != null;
			TypeUtility.ITypeConstructor typeConstructor;
			if (flag)
			{
				TypeUtility.TypeConstructorVisitor typeConstructorVisitor = new TypeUtility.TypeConstructorVisitor();
				propertyBag.Accept(typeConstructorVisitor);
				typeConstructor = typeConstructorVisitor.TypeConstructor;
			}
			else
			{
				bool containsGenericParameters = type.ContainsGenericParameters;
				if (containsGenericParameters)
				{
					TypeUtility.NonConstructable nonConstructable = new TypeUtility.NonConstructable();
					TypeUtility.s_TypeConstructors[type] = nonConstructable;
					typeConstructor = nonConstructable;
				}
				else
				{
					typeConstructor = TypeUtility.s_CreateTypeConstructor.MakeGenericMethod(new Type[] { type }).Invoke(null, null) as TypeUtility.ITypeConstructor;
				}
			}
			return typeConstructor;
		}

		private static TypeUtility.ITypeConstructor<T> CreateTypeConstructor<T>()
		{
			TypeUtility.TypeConstructor<T> typeConstructor = new TypeUtility.TypeConstructor<T>();
			TypeUtility.Cache<T>.TypeConstructor = typeConstructor;
			TypeUtility.s_TypeConstructors[typeof(T)] = typeConstructor;
			return typeConstructor;
		}

		private static TypeUtility.ITypeConstructor GetTypeConstructor(Type type)
		{
			TypeUtility.ITypeConstructor typeConstructor;
			return TypeUtility.s_TypeConstructors.TryGetValue(type, out typeConstructor) ? typeConstructor : TypeUtility.CreateTypeConstructor(type);
		}

		private static TypeUtility.ITypeConstructor<T> GetTypeConstructor<T>()
		{
			return (TypeUtility.Cache<T>.TypeConstructor != null) ? TypeUtility.Cache<T>.TypeConstructor : TypeUtility.CreateTypeConstructor<T>();
		}

		public static bool CanBeInstantiated(Type type)
		{
			return TypeUtility.GetTypeConstructor(type).CanBeInstantiated;
		}

		public static bool CanBeInstantiated<T>()
		{
			return TypeUtility.GetTypeConstructor<T>().CanBeInstantiated;
		}

		public static void SetExplicitInstantiationMethod<T>(Func<T> constructor)
		{
			TypeUtility.GetTypeConstructor<T>().SetExplicitConstructor(constructor);
		}

		public static T Instantiate<T>()
		{
			TypeUtility.ITypeConstructor<T> typeConstructor = TypeUtility.GetTypeConstructor<T>();
			TypeUtility.CheckCanBeInstantiated<T>(typeConstructor);
			return typeConstructor.Instantiate();
		}

		public static bool TryInstantiate<T>(out T instance)
		{
			TypeUtility.ITypeConstructor<T> typeConstructor = TypeUtility.GetTypeConstructor<T>();
			bool canBeInstantiated = typeConstructor.CanBeInstantiated;
			bool flag;
			if (canBeInstantiated)
			{
				instance = typeConstructor.Instantiate();
				flag = true;
			}
			else
			{
				instance = default(T);
				flag = false;
			}
			return flag;
		}

		public static T Instantiate<T>(Type derivedType)
		{
			TypeUtility.ITypeConstructor typeConstructor = TypeUtility.GetTypeConstructor(derivedType);
			TypeUtility.CheckIsAssignableFrom(typeof(T), derivedType);
			TypeUtility.CheckCanBeInstantiated(typeConstructor, derivedType);
			return (T)((object)typeConstructor.Instantiate());
		}

		public static bool TryInstantiate<T>(Type derivedType, out T value)
		{
			bool flag = !typeof(T).IsAssignableFrom(derivedType);
			bool flag2;
			if (flag)
			{
				value = default(T);
				value = default(T);
				flag2 = false;
			}
			else
			{
				TypeUtility.ITypeConstructor typeConstructor = TypeUtility.GetTypeConstructor(derivedType);
				bool flag3 = !typeConstructor.CanBeInstantiated;
				if (flag3)
				{
					value = default(T);
					flag2 = false;
				}
				else
				{
					value = (T)((object)typeConstructor.Instantiate());
					flag2 = true;
				}
			}
			return flag2;
		}

		public static TArray InstantiateArray<TArray>(int count = 0)
		{
			bool flag = count < 0;
			if (flag)
			{
				throw new ArgumentException(string.Format("{0}: Cannot construct an array with {1}={2}", "TypeUtility", "count", count));
			}
			IPropertyBag<TArray> propertyBag = PropertyBagStore.GetPropertyBag<TArray>();
			IConstructorWithCount<TArray> constructorWithCount = propertyBag as IConstructorWithCount<TArray>;
			bool flag2 = constructorWithCount != null;
			TArray tarray;
			if (flag2)
			{
				tarray = constructorWithCount.InstantiateWithCount(count);
			}
			else
			{
				Type typeFromHandle = typeof(TArray);
				bool flag3 = !typeFromHandle.IsArray;
				if (flag3)
				{
					throw new ArgumentException("TypeUtility: Cannot construct an array, since " + typeof(TArray).Name + " is not an array type.");
				}
				Type elementType = typeFromHandle.GetElementType();
				bool flag4 = null == elementType;
				if (flag4)
				{
					throw new ArgumentException("TypeUtility: Cannot construct an array, since " + typeof(TArray).Name + ".GetElementType() returned null.");
				}
				tarray = (TArray)((object)Array.CreateInstance(elementType, count));
			}
			return tarray;
		}

		public static bool TryInstantiateArray<TArray>(int count, out TArray instance)
		{
			bool flag = count < 0;
			bool flag2;
			if (flag)
			{
				instance = default(TArray);
				flag2 = false;
			}
			else
			{
				IPropertyBag<TArray> propertyBag = PropertyBagStore.GetPropertyBag<TArray>();
				IConstructorWithCount<TArray> constructorWithCount = propertyBag as IConstructorWithCount<TArray>;
				bool flag3 = constructorWithCount != null;
				if (flag3)
				{
					try
					{
						instance = constructorWithCount.InstantiateWithCount(count);
						return true;
					}
					catch
					{
					}
				}
				Type typeFromHandle = typeof(TArray);
				bool flag4 = !typeFromHandle.IsArray;
				if (flag4)
				{
					instance = default(TArray);
					flag2 = false;
				}
				else
				{
					Type elementType = typeFromHandle.GetElementType();
					bool flag5 = null == elementType;
					if (flag5)
					{
						instance = default(TArray);
						flag2 = false;
					}
					else
					{
						instance = (TArray)((object)Array.CreateInstance(elementType, count));
						flag2 = true;
					}
				}
			}
			return flag2;
		}

		public static TArray InstantiateArray<TArray>(Type derivedType, int count = 0)
		{
			bool flag = count < 0;
			if (flag)
			{
				throw new ArgumentException(string.Format("{0}: Cannot instantiate an array with {1}={2}", "TypeUtility", "count", count));
			}
			IPropertyBag propertyBag = PropertyBagStore.GetPropertyBag(derivedType);
			IConstructorWithCount<TArray> constructorWithCount = propertyBag as IConstructorWithCount<TArray>;
			bool flag2 = constructorWithCount != null;
			TArray tarray;
			if (flag2)
			{
				tarray = constructorWithCount.InstantiateWithCount(count);
			}
			else
			{
				Type typeFromHandle = typeof(TArray);
				bool flag3 = !typeFromHandle.IsArray;
				if (flag3)
				{
					throw new ArgumentException("TypeUtility: Cannot instantiate an array, since " + typeof(TArray).Name + " is not an array type.");
				}
				Type elementType = typeFromHandle.GetElementType();
				bool flag4 = null == elementType;
				if (flag4)
				{
					throw new ArgumentException("TypeUtility: Cannot instantiate an array, since " + typeof(TArray).Name + ".GetElementType() returned null.");
				}
				tarray = (TArray)((object)Array.CreateInstance(elementType, count));
			}
			return tarray;
		}

		private static void CheckIsAssignableFrom(Type type, Type derivedType)
		{
			bool flag = !type.IsAssignableFrom(derivedType);
			if (flag)
			{
				throw new ArgumentException(string.Concat(new string[] { "Could not create instance of type `", derivedType.Name, "` and convert to `", type.Name, "`: The given type is not assignable to target type." }));
			}
		}

		private static void CheckCanBeInstantiated<T>(TypeUtility.ITypeConstructor<T> constructor)
		{
			bool flag = !constructor.CanBeInstantiated;
			if (flag)
			{
				throw new InvalidOperationException("Type `" + typeof(T).Name + "` could not be instantiated. A parameter-less constructor or an explicit construction method is required.");
			}
		}

		private static void CheckCanBeInstantiated(TypeUtility.ITypeConstructor constructor, Type type)
		{
			bool flag = !constructor.CanBeInstantiated;
			if (flag)
			{
				throw new InvalidOperationException("Type `" + type.Name + "` could not be instantiated. A parameter-less constructor or an explicit construction method is required.");
			}
		}

		private static readonly ConcurrentDictionary<Type, TypeUtility.ITypeConstructor> s_TypeConstructors = new ConcurrentDictionary<Type, TypeUtility.ITypeConstructor>();

		private static readonly MethodInfo s_CreateTypeConstructor;

		private static readonly ConcurrentDictionary<Type, string> s_CachedResolvedName = new ConcurrentDictionary<Type, string>();

		private static readonly ObjectPool<StringBuilder> s_Builders = new ObjectPool<StringBuilder>(() => new StringBuilder(), null, delegate(StringBuilder sb)
		{
			sb.Clear();
		}, null, true, 10, 10000);

		private static readonly object syncedPoolObject = new object();

		private interface ITypeConstructor
		{
			bool CanBeInstantiated { get; }

			object Instantiate();
		}

		private interface ITypeConstructor<T> : TypeUtility.ITypeConstructor
		{
			T Instantiate();

			void SetExplicitConstructor(Func<T> constructor);
		}

		private class TypeConstructor<T> : TypeUtility.ITypeConstructor<T>, TypeUtility.ITypeConstructor
		{
			bool TypeUtility.ITypeConstructor.CanBeInstantiated
			{
				get
				{
					bool flag = this.m_ExplicitConstructor != null;
					bool flag2;
					if (flag)
					{
						flag2 = true;
					}
					else
					{
						bool flag3 = this.m_OverrideConstructor != null;
						if (flag3)
						{
							bool flag4 = this.m_OverrideConstructor.InstantiationKind == InstantiationKind.NotInstantiatable;
							if (flag4)
							{
								return false;
							}
							bool flag5 = this.m_OverrideConstructor.InstantiationKind == InstantiationKind.PropertyBagOverride;
							if (flag5)
							{
								return true;
							}
						}
						flag2 = this.m_ImplicitConstructor != null;
					}
					return flag2;
				}
			}

			public TypeConstructor()
			{
				this.m_OverrideConstructor = PropertyBagStore.GetPropertyBag<T>() as IConstructor<T>;
				this.SetImplicitConstructor();
			}

			private void SetImplicitConstructor()
			{
				Type typeFromHandle = typeof(T);
				bool isValueType = typeFromHandle.IsValueType;
				if (isValueType)
				{
					this.m_ImplicitConstructor = new Func<T>(TypeUtility.TypeConstructor<T>.CreateValueTypeInstance);
				}
				else
				{
					bool isAbstract = typeFromHandle.IsAbstract;
					if (!isAbstract)
					{
						bool flag = typeof(ScriptableObject).IsAssignableFrom(typeFromHandle);
						if (flag)
						{
							this.m_ImplicitConstructor = new Func<T>(TypeUtility.TypeConstructor<T>.CreateScriptableObjectInstance);
						}
						else
						{
							bool flag2 = null != typeFromHandle.GetConstructor(Array.Empty<Type>());
							if (flag2)
							{
								this.m_ImplicitConstructor = new Func<T>(TypeUtility.TypeConstructor<T>.CreateClassInstance);
							}
						}
					}
				}
			}

			private static T CreateValueTypeInstance()
			{
				return default(T);
			}

			private static T CreateScriptableObjectInstance()
			{
				return (T)((object)ScriptableObject.CreateInstance(typeof(T)));
			}

			private static T CreateClassInstance()
			{
				return Activator.CreateInstance<T>();
			}

			public void SetExplicitConstructor(Func<T> constructor)
			{
				this.m_ExplicitConstructor = constructor;
			}

			T TypeUtility.ITypeConstructor<T>.Instantiate()
			{
				bool flag = this.m_ExplicitConstructor != null;
				T t;
				if (flag)
				{
					t = this.m_ExplicitConstructor();
				}
				else
				{
					bool flag2 = this.m_OverrideConstructor != null;
					if (flag2)
					{
						bool flag3 = this.m_OverrideConstructor.InstantiationKind == InstantiationKind.NotInstantiatable;
						if (flag3)
						{
							throw new InvalidOperationException("The type '" + typeof(T).Name + "' is not constructable.");
						}
						bool flag4 = this.m_OverrideConstructor.InstantiationKind == InstantiationKind.PropertyBagOverride;
						if (flag4)
						{
							return this.m_OverrideConstructor.Instantiate();
						}
					}
					bool flag5 = this.m_ImplicitConstructor != null;
					if (!flag5)
					{
						throw new InvalidOperationException("The type '" + typeof(T).Name + "' is not constructable.");
					}
					t = this.m_ImplicitConstructor();
				}
				return t;
			}

			object TypeUtility.ITypeConstructor.Instantiate()
			{
				return ((TypeUtility.ITypeConstructor<T>)this).Instantiate();
			}

			private Func<T> m_ExplicitConstructor;

			private Func<T> m_ImplicitConstructor;

			private IConstructor<T> m_OverrideConstructor;
		}

		private class NonConstructable : TypeUtility.ITypeConstructor
		{
			bool TypeUtility.ITypeConstructor.CanBeInstantiated
			{
				get
				{
					return false;
				}
			}

			public object Instantiate()
			{
				throw new InvalidOperationException("The type is not instantiatable.");
			}
		}

		private struct Cache<T>
		{
			public static TypeUtility.ITypeConstructor<T> TypeConstructor;
		}

		private class TypeConstructorVisitor : ITypeVisitor
		{
			public void Visit<TContainer>()
			{
				this.TypeConstructor = TypeUtility.CreateTypeConstructor<TContainer>();
			}

			public TypeUtility.ITypeConstructor TypeConstructor;
		}
	}
}
