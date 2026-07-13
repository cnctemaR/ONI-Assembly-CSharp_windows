using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class ReflectionHelper
	{
		private static MemberInfo _Cache(string cacheKey, MemberInfo value)
		{
			if (cacheKey != null && value == null)
			{
				bool flag;
				MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new MMDbgLog.DebugLogErrorStringHandler(21, 1, out flag);
				if (flag)
				{
					debugLogErrorStringHandler.AppendLiteral("ResolveRefl failure: ");
					debugLogErrorStringHandler.AppendFormatted(cacheKey);
				}
				MMDbgLog.Error(ref debugLogErrorStringHandler);
			}
			if (cacheKey != null && value != null)
			{
				Dictionary<string, WeakReference> resolveReflectionCache = ReflectionHelper.ResolveReflectionCache;
				lock (resolveReflectionCache)
				{
					ReflectionHelper.ResolveReflectionCache[cacheKey] = new WeakReference(value);
				}
			}
			return value;
		}

		public static Assembly Load(ModuleDefinition module)
		{
			Helpers.ThrowIfArgumentNull<ModuleDefinition>(module, "module");
			Assembly assembly;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				module.Write(memoryStream);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				assembly = ReflectionHelper.Load(memoryStream);
			}
			return assembly;
		}

		public static Assembly Load(Stream stream)
		{
			Helpers.ThrowIfArgumentNull<Stream>(stream, "stream");
			MemoryStream memoryStream = stream as MemoryStream;
			Assembly asm;
			if (memoryStream != null)
			{
				asm = Assembly.Load(memoryStream.GetBuffer());
			}
			else
			{
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					stream.CopyTo(memoryStream2);
					memoryStream2.Seek(0L, SeekOrigin.Begin);
					asm = Assembly.Load(memoryStream2.GetBuffer());
				}
			}
			AppDomain.CurrentDomain.AssemblyResolve += delegate(object s, ResolveEventArgs e)
			{
				if (!(e.Name == asm.FullName))
				{
					return null;
				}
				return asm;
			};
			return asm;
		}

		[return: Nullable(2)]
		public static Type GetType(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			Type type = Type.GetType(name);
			if (type != null)
			{
				return type;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				type = assemblies[i].GetType(name);
				if (type != null)
				{
					return type;
				}
			}
			return null;
		}

		public static bool HashIs(this AssemblyNameReference asmRef, Assembly asm, bool defaultIfNoHash = true)
		{
			Helpers.ThrowIfArgumentNull<AssemblyNameReference>(asmRef, "asmRef");
			Helpers.ThrowIfArgumentNull<Assembly>(asm, "asm");
			byte[] hash = asmRef.Hash;
			int? num = ((hash != null) ? new int?(hash.Length) : null);
			int num2 = ReflectionHelper.AssemblyHashPrefix.Length + 4;
			if ((num.GetValueOrDefault() == num2) & (num != null))
			{
				byte[] hash2 = asmRef.Hash;
				for (int i = 0; i < ReflectionHelper.AssemblyHashPrefix.Length; i++)
				{
					if (hash2[i] != ReflectionHelper.AssemblyHashPrefix[i])
					{
						return false;
					}
				}
				byte[] bytes = BitConverter.GetBytes(asm.GetHashCode());
				for (int j = 0; j < 4; j++)
				{
					if (hash2[ReflectionHelper.AssemblyHashPrefix.Length + j] != bytes[j])
					{
						return false;
					}
				}
				return true;
			}
			return defaultIfNoHash;
		}

		public static void ApplyRuntimeHash(this AssemblyNameReference asmRef, Assembly asm)
		{
			Helpers.ThrowIfArgumentNull<AssemblyNameReference>(asmRef, "asmRef");
			Helpers.ThrowIfArgumentNull<Assembly>(asm, "asm");
			byte[] array = new byte[ReflectionHelper.AssemblyHashPrefix.Length + 4];
			Array.Copy(ReflectionHelper.AssemblyHashPrefix, 0, array, 0, ReflectionHelper.AssemblyHashPrefix.Length);
			Array.Copy(BitConverter.GetBytes(asm.GetHashCode()), 0, array, ReflectionHelper.AssemblyHashPrefix.Length, 4);
			asmRef.HashAlgorithm = (AssemblyHashAlgorithm)4294967295U;
			asmRef.Hash = array;
		}

		public static string GetRuntimeHashedFullName(this Assembly asm)
		{
			Helpers.ThrowIfArgumentNull<Assembly>(asm, "asm");
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 3);
			defaultInterpolatedStringHandler.AppendFormatted(asm.FullName);
			defaultInterpolatedStringHandler.AppendFormatted(ReflectionHelper.AssemblyHashNameTag);
			defaultInterpolatedStringHandler.AppendFormatted<int>(asm.GetHashCode());
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}

		public static string GetRuntimeHashedFullName(this AssemblyNameReference asm)
		{
			Helpers.ThrowIfArgumentNull<AssemblyNameReference>(asm, "asm");
			if (asm.HashAlgorithm != (AssemblyHashAlgorithm)4294967295U)
			{
				return asm.FullName;
			}
			byte[] hash = asm.Hash;
			if (hash.Length != ReflectionHelper.AssemblyHashPrefix.Length + 4)
			{
				return asm.FullName;
			}
			for (int i = 0; i < ReflectionHelper.AssemblyHashPrefix.Length; i++)
			{
				if (hash[i] != ReflectionHelper.AssemblyHashPrefix[i])
				{
					return asm.FullName;
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 3);
			defaultInterpolatedStringHandler.AppendFormatted(asm.FullName);
			defaultInterpolatedStringHandler.AppendFormatted(ReflectionHelper.AssemblyHashNameTag);
			defaultInterpolatedStringHandler.AppendFormatted<int>(BitConverter.ToInt32(hash, ReflectionHelper.AssemblyHashPrefix.Length));
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}

		public static Type ResolveReflection(this TypeReference mref)
		{
			return (Type)ReflectionHelper._ResolveReflection(mref, null);
		}

		public static MethodBase ResolveReflection(this MethodReference mref)
		{
			return (MethodBase)ReflectionHelper._ResolveReflection(mref, null);
		}

		public static FieldInfo ResolveReflection(this FieldReference mref)
		{
			return (FieldInfo)ReflectionHelper._ResolveReflection(mref, null);
		}

		public static PropertyInfo ResolveReflection(this PropertyReference mref)
		{
			return (PropertyInfo)ReflectionHelper._ResolveReflection(mref, null);
		}

		public static EventInfo ResolveReflection(this EventReference mref)
		{
			return (EventInfo)ReflectionHelper._ResolveReflection(mref, null);
		}

		public static MemberInfo ResolveReflection(this MemberReference mref)
		{
			return ReflectionHelper._ResolveReflection(mref, null);
		}

		[NullableContext(2)]
		[return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("mref")]
		private static MemberInfo _ResolveReflection(MemberReference mref, [Nullable(new byte[] { 2, 1 })] Module[] modules)
		{
			if (mref == null)
			{
				return null;
			}
			DynamicMethodReference dynamicMethodReference = mref as DynamicMethodReference;
			if (dynamicMethodReference != null)
			{
				return dynamicMethodReference.DynamicMethod;
			}
			MethodReference methodReference = mref as MethodReference;
			string text = ((methodReference != null) ? methodReference.GetID(null, null, true, false) : null) ?? mref.FullName;
			TypeReference typeReference;
			if ((typeReference = mref.DeclaringType) == null)
			{
				typeReference = (mref as TypeReference) ?? null;
			}
			TypeReference typeReference2 = typeReference;
			ValueTuple<string, string> valueTuple = ReflectionHelper.<_ResolveReflection>g__GetScope|21_0(mref);
			string asmName = valueTuple.Item1;
			string moduleName = valueTuple.Item2;
			if (mref is IGenericInstance)
			{
				IEnumerable<string> enumerable = ReflectionHelper.<_ResolveReflection>g__GetGenericArgumentsRecursive|21_2(mref).Select<MemberReference, string>(delegate(MemberReference x)
				{
					ValueTuple<string, string> valueTuple2 = ReflectionHelper.<_ResolveReflection>g__GetScope|21_0(x);
					string item = valueTuple2.Item1;
					string item2 = valueTuple2.Item2;
					return ReflectionHelper.<_ResolveReflection>g__ToCacheKeyPart|21_1(item, item2);
				});
				text += string.Concat(enumerable.ToArray<string>());
			}
			else
			{
				text += ReflectionHelper.<_ResolveReflection>g__ToCacheKeyPart|21_1(asmName, moduleName);
			}
			Dictionary<string, WeakReference> dictionary = ReflectionHelper.ResolveReflectionCache;
			lock (dictionary)
			{
				WeakReference weakReference;
				if (ReflectionHelper.ResolveReflectionCache.TryGetValue(text, out weakReference) && weakReference != null)
				{
					MemberInfo memberInfo = weakReference.SafeGetTarget() as MemberInfo;
					if (memberInfo != null)
					{
						return memberInfo;
					}
				}
			}
			if (mref is GenericParameter)
			{
				throw new NotSupportedException("ResolveReflection on GenericParameter currently not supported");
			}
			MethodReference methodReference2 = mref as MethodReference;
			Type type;
			if (methodReference2 != null && mref.DeclaringType is ArrayType)
			{
				type = (Type)ReflectionHelper._ResolveReflection(mref.DeclaringType, modules);
				string methodID = methodReference2.GetID(null, null, false, false);
				MethodBase methodBase = type.GetMethods((BindingFlags)(-1)).Cast<MethodBase>().Concat<MethodBase>(type.GetConstructors((BindingFlags)(-1)))
					.FirstOrDefault<MethodBase>((MethodBase m) => m.GetID(null, null, false, false, false) == methodID);
				if (methodBase != null)
				{
					return ReflectionHelper._Cache(text, methodBase);
				}
			}
			if (typeReference2 == null)
			{
				throw new ArgumentException("MemberReference hasn't got a DeclaringType / isn't a TypeReference in itself");
			}
			if (asmName == null && moduleName == null)
			{
				throw new NotSupportedException("Unsupported scope type " + typeReference2.Scope.GetType().FullName);
			}
			bool flag2 = true;
			bool flag3 = false;
			bool flag4 = false;
			Func<Type, bool> <>9__24;
			Func<MethodInfo, bool> <>9__25;
			Func<FieldInfo, bool> <>9__26;
			TypeSpecification typeSpecification;
			MemberInfo memberInfo2;
			for (;;)
			{
				if (flag4)
				{
					modules = null;
				}
				flag4 = true;
				if (modules == null)
				{
					Assembly[] array = null;
					if (flag2 && flag3)
					{
						flag3 = false;
						flag2 = false;
					}
					if (flag2)
					{
						dictionary = ReflectionHelper.AssemblyCache;
						lock (dictionary)
						{
							WeakReference weakReference2;
							if (ReflectionHelper.AssemblyCache.TryGetValue(asmName, out weakReference2))
							{
								Assembly assembly = weakReference2.SafeGetTarget() as Assembly;
								if (assembly != null)
								{
									array = new Assembly[] { assembly };
								}
							}
						}
					}
					if (array == null && !flag3)
					{
						Dictionary<string, WeakReference[]> dictionary2 = ReflectionHelper.AssembliesCache;
						lock (dictionary2)
						{
							WeakReference[] array2;
							if (ReflectionHelper.AssembliesCache.TryGetValue(asmName, out array2))
							{
								array = (from asmRef in array2
									select asmRef.SafeGetTarget() as Assembly into asm
									where asm != null
									select asm).ToArray<Assembly>();
							}
						}
					}
					if (array == null)
					{
						int num = asmName.IndexOf(ReflectionHelper.AssemblyHashNameTag, StringComparison.Ordinal);
						int hash;
						if (num != -1 && int.TryParse(asmName.Substring(num + 2), out hash))
						{
							array = (from other in AppDomain.CurrentDomain.GetAssemblies()
								where other.GetHashCode() == hash
								select other).ToArray<Assembly>();
							if (array.Length == 0)
							{
								array = null;
							}
							asmName = asmName.Substring(0, num);
						}
						if (array == null)
						{
							array = (from other in AppDomain.CurrentDomain.GetAssemblies()
								where other.GetName().FullName == asmName
								select other).ToArray<Assembly>();
							if (array.Length == 0)
							{
								array = (from other in AppDomain.CurrentDomain.GetAssemblies()
									where other.GetName().Name == asmName
									select other).ToArray<Assembly>();
							}
							if (array.Length == 0)
							{
								Assembly assembly2 = Assembly.Load(new AssemblyName(asmName));
								if (assembly2 != null)
								{
									array = new Assembly[] { assembly2 };
								}
							}
						}
						if (array.Length != 0)
						{
							Dictionary<string, WeakReference[]> dictionary2 = ReflectionHelper.AssembliesCache;
							lock (dictionary2)
							{
								ReflectionHelper.AssembliesCache[asmName] = array.Select<Assembly, WeakReference>((Assembly asm) => new WeakReference(asm)).ToArray<WeakReference>();
							}
						}
					}
					IEnumerable<Module> enumerable2;
					if (!string.IsNullOrEmpty(moduleName))
					{
						enumerable2 = array.Select<Assembly, Module>((Assembly asm) => asm.GetModule(moduleName));
					}
					else
					{
						enumerable2 = array.SelectMany<Assembly, Module>((Assembly asm) => asm.GetModules());
					}
					modules = enumerable2.Where<Module>((Module mod) => mod != null).ToArray<Module>();
					if (modules.Length == 0)
					{
						break;
					}
				}
				TypeReference typeReference3 = mref as TypeReference;
				if (typeReference3 != null)
				{
					if (typeReference3.FullName == "<Module>")
					{
						goto Block_40;
					}
					typeSpecification = mref as TypeSpecification;
					if (typeSpecification != null)
					{
						goto Block_41;
					}
					type = modules.Select<Module, Type>((Module module) => module.GetType(mref.FullName.Replace("/", "+", StringComparison.Ordinal), false, false)).FirstOrDefault<Type>((Type m) => m != null);
					if (type == null)
					{
						type = modules.Select<Module, Type>(delegate(Module module)
						{
							IEnumerable<Type> types = module.GetTypes();
							Func<Type, bool> func;
							if ((func = <>9__24) == null)
							{
								func = (<>9__24 = (Type m) => mref.Is(m));
							}
							return types.FirstOrDefault<Type>(func);
						}).FirstOrDefault<Type>((Type m) => m != null);
					}
					if (!(type == null) || flag3)
					{
						goto IL_06F2;
					}
				}
				else
				{
					TypeReference declaringType = mref.DeclaringType;
					bool flag5 = ((declaringType != null) ? declaringType.FullName : null) == "<Module>";
					GenericInstanceMethod genericInstanceMethod = mref as GenericInstanceMethod;
					if (genericInstanceMethod != null)
					{
						memberInfo2 = ReflectionHelper._ResolveReflection(genericInstanceMethod.ElementMethod, modules);
						MethodInfo methodInfo = memberInfo2 as MethodInfo;
						MemberInfo memberInfo3;
						if (methodInfo == null)
						{
							memberInfo3 = null;
						}
						else
						{
							memberInfo3 = methodInfo.MakeGenericMethod(genericInstanceMethod.GenericArguments.Select<TypeReference, Type>((TypeReference arg) => ReflectionHelper._ResolveReflection(arg, null) as Type).ToArray<Type>());
						}
						memberInfo2 = memberInfo3;
					}
					else if (flag5)
					{
						if (mref is MethodReference)
						{
							memberInfo2 = modules.Select<Module, MethodInfo>(delegate(Module module)
							{
								IEnumerable<MethodInfo> methods = module.GetMethods((BindingFlags)(-1));
								Func<MethodInfo, bool> func2;
								if ((func2 = <>9__25) == null)
								{
									func2 = (<>9__25 = (MethodInfo m) => mref.Is(m));
								}
								return methods.FirstOrDefault<MethodInfo>(func2);
							}).FirstOrDefault<MethodInfo>((MethodInfo m) => m != null);
						}
						else
						{
							if (!(mref is FieldReference))
							{
								goto IL_0823;
							}
							memberInfo2 = modules.Select<Module, FieldInfo>(delegate(Module module)
							{
								IEnumerable<FieldInfo> fields = module.GetFields((BindingFlags)(-1));
								Func<FieldInfo, bool> func3;
								if ((func3 = <>9__26) == null)
								{
									func3 = (<>9__26 = (FieldInfo m) => mref.Is(m));
								}
								return fields.FirstOrDefault<FieldInfo>(func3);
							}).FirstOrDefault<FieldInfo>((FieldInfo m) => m != null);
						}
					}
					else
					{
						Type type2 = (Type)ReflectionHelper._ResolveReflection(mref.DeclaringType, modules);
						if (mref is MethodReference)
						{
							memberInfo2 = type2.GetMethods((BindingFlags)(-1)).Cast<MethodBase>().Concat<MethodBase>(type2.GetConstructors((BindingFlags)(-1)))
								.FirstOrDefault<MethodBase>((MethodBase m) => mref.Is(m));
						}
						else if (mref is FieldReference)
						{
							memberInfo2 = type2.GetFields((BindingFlags)(-1)).FirstOrDefault<FieldInfo>((FieldInfo m) => mref.Is(m));
						}
						else
						{
							memberInfo2 = type2.GetMembers((BindingFlags)(-1)).FirstOrDefault<MemberInfo>((MemberInfo m) => mref.Is(m));
						}
					}
					if (!(memberInfo2 == null) || flag3)
					{
						goto IL_08ED;
					}
				}
				flag3 = true;
			}
			throw new MissingMemberException("Cannot resolve assembly / module " + asmName + " / " + moduleName);
			Block_40:
			throw new ArgumentException("Type <Module> cannot be resolved to a runtime reflection type");
			Block_41:
			type = (Type)ReflectionHelper._ResolveReflection(typeSpecification.ElementType, null);
			if (typeSpecification.IsByReference)
			{
				return ReflectionHelper._Cache(text, type.MakeByRefType());
			}
			if (typeSpecification.IsPointer)
			{
				return ReflectionHelper._Cache(text, type.MakePointerType());
			}
			if (typeSpecification.IsArray)
			{
				return ReflectionHelper._Cache(text, ((ArrayType)typeSpecification).IsVector ? type.MakeArrayType() : type.MakeArrayType(((ArrayType)typeSpecification).Dimensions.Count));
			}
			if (typeSpecification.IsGenericInstance)
			{
				return ReflectionHelper._Cache(text, type.MakeGenericType(((GenericInstanceType)typeSpecification).GenericArguments.Select<TypeReference, Type>((TypeReference arg) => ReflectionHelper._ResolveReflection(arg, null) as Type).ToArray<Type>()));
			}
			IL_06F2:
			return ReflectionHelper._Cache(text, type);
			IL_0823:
			throw new NotSupportedException("Unsupported <Module> member type " + mref.GetType().FullName);
			IL_08ED:
			return ReflectionHelper._Cache(text, memberInfo2);
		}

		public static SignatureHelper ResolveReflection(this Mono.Cecil.CallSite csite, Module context)
		{
			return csite.ResolveReflectionSignature(context);
		}

		public static SignatureHelper ResolveReflectionSignature(this IMethodSignature csite, Module context)
		{
			Helpers.ThrowIfArgumentNull<IMethodSignature>(csite, "csite");
			Helpers.ThrowIfArgumentNull<Module>(context, "context");
			SignatureHelper signatureHelper;
			switch (csite.CallingConvention)
			{
			case MethodCallingConvention.C:
				signatureHelper = ReflectionHelper.GetUnmanagedSigHelper(context, CallingConvention.Cdecl, csite.ReturnType.ResolveReflection());
				break;
			case MethodCallingConvention.StdCall:
				signatureHelper = ReflectionHelper.GetUnmanagedSigHelper(context, CallingConvention.StdCall, csite.ReturnType.ResolveReflection());
				break;
			case MethodCallingConvention.ThisCall:
				signatureHelper = ReflectionHelper.GetUnmanagedSigHelper(context, CallingConvention.ThisCall, csite.ReturnType.ResolveReflection());
				break;
			case MethodCallingConvention.FastCall:
				signatureHelper = ReflectionHelper.GetUnmanagedSigHelper(context, CallingConvention.FastCall, csite.ReturnType.ResolveReflection());
				break;
			case MethodCallingConvention.VarArg:
				signatureHelper = SignatureHelper.GetMethodSigHelper(context, CallingConventions.VarArgs, csite.ReturnType.ResolveReflection());
				break;
			default:
				if (csite.ExplicitThis)
				{
					signatureHelper = SignatureHelper.GetMethodSigHelper(context, CallingConventions.ExplicitThis, csite.ReturnType.ResolveReflection());
				}
				else
				{
					signatureHelper = SignatureHelper.GetMethodSigHelper(context, CallingConventions.Standard, csite.ReturnType.ResolveReflection());
				}
				break;
			}
			if (context != null)
			{
				List<Type> list = new List<Type>();
				List<Type> list2 = new List<Type>();
				using (Collection<ParameterDefinition>.Enumerator enumerator = csite.Parameters.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ParameterDefinition parameterDefinition = enumerator.Current;
						if (parameterDefinition.ParameterType.IsSentinel)
						{
							signatureHelper.AddSentinel();
						}
						if (parameterDefinition.ParameterType.IsPinned)
						{
							signatureHelper.AddArgument(parameterDefinition.ParameterType.ResolveReflection(), true);
						}
						else
						{
							list2.Clear();
							list.Clear();
							TypeReference typeReference = parameterDefinition.ParameterType;
							for (;;)
							{
								TypeSpecification typeSpecification = typeReference as TypeSpecification;
								if (typeSpecification == null)
								{
									break;
								}
								RequiredModifierType requiredModifierType = typeReference as RequiredModifierType;
								if (requiredModifierType == null)
								{
									OptionalModifierType optionalModifierType = typeReference as OptionalModifierType;
									if (optionalModifierType != null)
									{
										list2.Add(optionalModifierType.ModifierType.ResolveReflection());
									}
								}
								else
								{
									list.Add(requiredModifierType.ModifierType.ResolveReflection());
								}
								typeReference = typeSpecification.ElementType;
							}
							signatureHelper.AddArgument(parameterDefinition.ParameterType.ResolveReflection(), list.ToArray(), list2.ToArray());
						}
					}
					return signatureHelper;
				}
			}
			foreach (ParameterDefinition parameterDefinition2 in csite.Parameters)
			{
				signatureHelper.AddArgument(parameterDefinition2.ParameterType.ResolveReflection());
			}
			return signatureHelper;
		}

		static ReflectionHelper()
		{
			MethodInfo getUnmanagedSigHelperMethod = ReflectionHelper.GetUnmanagedSigHelperMethod;
			ReflectionHelper.GetUnmanagedSigHelper = ((getUnmanagedSigHelperMethod != null) ? getUnmanagedSigHelperMethod.TryCreateDelegate<ReflectionHelper.GetUnmanagedSigHelperDelegate>() : null) ?? delegate(Module _, CallingConvention _, Type _)
			{
				throw new NotImplementedException("Unmanaged calling conventions are not supported");
			};
			object[] array = new object[2];
			array[0] = 0;
			ReflectionHelper._CacheGetterArgs = array;
			ReflectionHelper.t_RuntimeType = typeof(Type).Assembly.GetType("System.RuntimeType");
			Type type = ReflectionHelper.t_RuntimeType;
			ReflectionHelper.t_RuntimeTypeCache = ((type != null) ? type.GetNestedType("RuntimeTypeCache", BindingFlags.Public | BindingFlags.NonPublic) : null);
			PropertyInfo propertyInfo;
			if (!(ReflectionHelper.t_RuntimeTypeCache == null))
			{
				Type type2 = ReflectionHelper.t_RuntimeType;
				propertyInfo = ((type2 != null) ? type2.GetProperty("Cache", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, ReflectionHelper.t_RuntimeTypeCache, Type.EmptyTypes, null) : null);
			}
			else
			{
				propertyInfo = null;
			}
			ReflectionHelper.p_RuntimeType_Cache = propertyInfo;
			Type type3 = ReflectionHelper.t_RuntimeTypeCache;
			ReflectionHelper.m_RuntimeTypeCache_GetFieldList = ((type3 != null) ? type3.GetMethod("GetFieldList", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) : null);
			Type type4 = ReflectionHelper.t_RuntimeTypeCache;
			ReflectionHelper.m_RuntimeTypeCache_GetPropertyList = ((type4 != null) ? type4.GetMethod("GetPropertyList", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) : null);
			ReflectionHelper._CacheFixed = new ConditionalWeakTable<Type, ReflectionHelper.CacheFixEntry>();
			ReflectionHelper.t_RuntimeModule = typeof(Module).Assembly.GetType("System.Reflection.RuntimeModule");
			Type type5 = typeof(Module).Assembly.GetType("System.Reflection.RuntimeModule");
			ReflectionHelper.p_RuntimeModule_RuntimeType = ((type5 != null) ? type5.GetProperty("RuntimeType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) : null);
			Type type6 = typeof(Module).Assembly.GetType("System.Reflection.RuntimeModule");
			ReflectionHelper.f_RuntimeModule__impl = ((type6 != null) ? type6.GetField("_impl", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) : null);
			Type type7 = typeof(Module).Assembly.GetType("System.Reflection.RuntimeModule");
			ReflectionHelper.m_RuntimeModule_GetGlobalType = ((type7 != null) ? type7.GetMethod("GetGlobalType", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) : null);
			ReflectionHelper.f_SignatureHelper_module = typeof(SignatureHelper).GetField("m_module", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ?? typeof(SignatureHelper).GetField("module", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		public static void FixReflectionCacheAuto(this Type type)
		{
			type.FixReflectionCache();
		}

		[NullableContext(2)]
		public static void FixReflectionCache(this Type type)
		{
			if (ReflectionHelper.t_RuntimeType == null || ReflectionHelper.p_RuntimeType_Cache == null || ReflectionHelper.m_RuntimeTypeCache_GetFieldList == null || ReflectionHelper.m_RuntimeTypeCache_GetPropertyList == null)
			{
				return;
			}
			while (type != null)
			{
				if (ReflectionHelper.t_RuntimeType.IsInstanceOfType(type))
				{
					ReflectionHelper.CacheFixEntry value = ReflectionHelper._CacheFixed.GetValue(type, delegate(Type rt)
					{
						ReflectionHelper.CacheFixEntry cacheFixEntry2 = new ReflectionHelper.CacheFixEntry();
						object obj = (cacheFixEntry2.Cache = ReflectionHelper.p_RuntimeType_Cache.GetValue(rt, ArrayEx.Empty<object>()));
						Array array = (cacheFixEntry2.Properties = ReflectionHelper._GetArray(obj, ReflectionHelper.m_RuntimeTypeCache_GetPropertyList));
						Array array2 = (cacheFixEntry2.Fields = ReflectionHelper._GetArray(obj, ReflectionHelper.m_RuntimeTypeCache_GetFieldList));
						ReflectionHelper._FixReflectionCacheOrder<PropertyInfo>(array);
						ReflectionHelper._FixReflectionCacheOrder<FieldInfo>(array2);
						cacheFixEntry2.NeedsVerify = false;
						return cacheFixEntry2;
					});
					if (value.NeedsVerify && !ReflectionHelper._Verify(value, type))
					{
						ReflectionHelper.CacheFixEntry cacheFixEntry = value;
						lock (cacheFixEntry)
						{
							ReflectionHelper._FixReflectionCacheOrder<PropertyInfo>(value.Properties);
							ReflectionHelper._FixReflectionCacheOrder<FieldInfo>(value.Fields);
						}
					}
					value.NeedsVerify = true;
				}
				type = type.DeclaringType;
			}
		}

		private static bool _Verify(ReflectionHelper.CacheFixEntry entry, Type type)
		{
			object value;
			if (entry.Cache != (value = ReflectionHelper.p_RuntimeType_Cache.GetValue(type, ArrayEx.Empty<object>())))
			{
				entry.Cache = value;
				entry.Properties = ReflectionHelper._GetArray(value, ReflectionHelper.m_RuntimeTypeCache_GetPropertyList);
				entry.Fields = ReflectionHelper._GetArray(value, ReflectionHelper.m_RuntimeTypeCache_GetFieldList);
				return false;
			}
			Array array;
			if (entry.Properties != (array = ReflectionHelper._GetArray(value, ReflectionHelper.m_RuntimeTypeCache_GetPropertyList)))
			{
				entry.Properties = array;
				entry.Fields = ReflectionHelper._GetArray(value, ReflectionHelper.m_RuntimeTypeCache_GetFieldList);
				return false;
			}
			Array array2;
			if (entry.Fields != (array2 = ReflectionHelper._GetArray(value, ReflectionHelper.m_RuntimeTypeCache_GetFieldList)))
			{
				entry.Fields = array2;
				return false;
			}
			return true;
		}

		private static Array _GetArray([Nullable(2)] object cache, MethodInfo getter)
		{
			getter.Invoke(cache, ReflectionHelper._CacheGetterArgs);
			object obj = getter.Invoke(cache, ReflectionHelper._CacheGetterArgs);
			Array array = obj as Array;
			if (array != null)
			{
				return array;
			}
			Type returnType = getter.ReturnType;
			if (returnType != null && returnType.Namespace == "System.Reflection" && returnType.Name == "CerArrayList`1")
			{
				return (Array)returnType.GetField("m_array", (BindingFlags)(-1)).GetValue(obj);
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Unknown reflection cache type ");
			defaultInterpolatedStringHandler.AppendFormatted<Type>(obj.GetType());
			throw new InvalidOperationException(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		[NullableContext(0)]
		private static void _FixReflectionCacheOrder<T>([Nullable(2)] Array orig) where T : MemberInfo
		{
			if (orig == null)
			{
				return;
			}
			List<T> list = new List<T>(orig.Length);
			for (int i = 0; i < orig.Length; i++)
			{
				list.Add((T)((object)orig.GetValue(i)));
			}
			list.Sort(delegate(T a, T b)
			{
				if (a == b)
				{
					return 0;
				}
				if (a == null)
				{
					return 1;
				}
				if (b == null)
				{
					return -1;
				}
				return a.MetadataToken - b.MetadataToken;
			});
			for (int j = orig.Length - 1; j >= 0; j--)
			{
				orig.SetValue(list[j], j);
			}
		}

		[NullableContext(2)]
		public static Type GetModuleType(this Module module)
		{
			if (module == null || ReflectionHelper.t_RuntimeModule == null || !ReflectionHelper.t_RuntimeModule.IsInstanceOfType(module))
			{
				return null;
			}
			if (ReflectionHelper.p_RuntimeModule_RuntimeType != null)
			{
				return (Type)ReflectionHelper.p_RuntimeModule_RuntimeType.GetValue(module, ArrayEx.Empty<object>());
			}
			if (ReflectionHelper.f_RuntimeModule__impl != null && ReflectionHelper.m_RuntimeModule_GetGlobalType != null)
			{
				return (Type)ReflectionHelper.m_RuntimeModule_GetGlobalType.Invoke(null, new object[] { ReflectionHelper.f_RuntimeModule__impl.GetValue(module) });
			}
			return null;
		}

		[return: Nullable(2)]
		public static Type GetRealDeclaringType(this MemberInfo member)
		{
			Type type;
			if ((type = Helpers.ThrowIfNull<MemberInfo>(member, "member").DeclaringType) == null)
			{
				Module module = member.Module;
				if (module == null)
				{
					return null;
				}
				type = module.GetModuleType();
			}
			return type;
		}

		private static Module GetSignatureHelperModule(SignatureHelper signature)
		{
			if (ReflectionHelper.f_SignatureHelper_module == null)
			{
				throw new InvalidOperationException("Unable to find module field for SignatureHelper");
			}
			return (Module)ReflectionHelper.f_SignatureHelper_module.GetValue(signature);
		}

		public static Mono.Cecil.CallSite ImportCallSite(this ModuleDefinition moduleTo, ICallSiteGenerator signature)
		{
			return Helpers.ThrowIfNull<ICallSiteGenerator>(signature, "signature").ToCallSite(moduleTo);
		}

		public static Mono.Cecil.CallSite ImportCallSite(this ModuleDefinition moduleTo, SignatureHelper signature)
		{
			return Helpers.ThrowIfNull<ModuleDefinition>(moduleTo, "moduleTo").ImportCallSite(ReflectionHelper.GetSignatureHelperModule(signature), Helpers.ThrowIfNull<SignatureHelper>(signature, "signature").GetSignature());
		}

		public static Mono.Cecil.CallSite ImportCallSite(this ModuleDefinition moduleTo, Module moduleFrom, int token)
		{
			return Helpers.ThrowIfNull<ModuleDefinition>(moduleTo, "moduleTo").ImportCallSite(moduleFrom, Helpers.ThrowIfNull<Module>(moduleFrom, "moduleFrom").ResolveSignature(token));
		}

		public static Mono.Cecil.CallSite ImportCallSite(this ModuleDefinition moduleTo, Module moduleFrom, byte[] data)
		{
			ReflectionHelper.<>c__DisplayClass52_0 CS$<>8__locals1;
			CS$<>8__locals1.moduleTo = moduleTo;
			CS$<>8__locals1.moduleFrom = moduleFrom;
			Helpers.ThrowIfArgumentNull<ModuleDefinition>(CS$<>8__locals1.moduleTo, "moduleTo");
			Helpers.ThrowIfArgumentNull<Module>(CS$<>8__locals1.moduleFrom, "moduleFrom");
			Helpers.ThrowIfArgumentNull<byte[]>(data, "data");
			Mono.Cecil.CallSite callSite = new Mono.Cecil.CallSite(CS$<>8__locals1.moduleTo.TypeSystem.Void);
			Mono.Cecil.CallSite callSite2;
			using (MemoryStream memoryStream = new MemoryStream(data, false))
			{
				ReflectionHelper.<>c__DisplayClass52_1 CS$<>8__locals2;
				CS$<>8__locals2.reader = new BinaryReader(memoryStream);
				try
				{
					ReflectionHelper.<ImportCallSite>g__ReadMethodSignature|52_0(callSite, ref CS$<>8__locals1, ref CS$<>8__locals2);
					callSite2 = callSite;
				}
				finally
				{
					if (CS$<>8__locals2.reader != null)
					{
						((IDisposable)CS$<>8__locals2.reader).Dispose();
					}
				}
			}
			return callSite2;
		}

		[CompilerGenerated]
		[return: Nullable(new byte[] { 0, 2, 2 })]
		internal static ValueTuple<string, string> <_ResolveReflection>g__GetScope|21_0(MemberReference mref)
		{
			TypeReference typeReference;
			if ((typeReference = mref.DeclaringType) == null)
			{
				typeReference = (mref as TypeReference) ?? null;
			}
			TypeReference typeReference2 = typeReference;
			IMetadataScope metadataScope = ((typeReference2 != null) ? typeReference2.Scope : null);
			AssemblyNameReference assemblyNameReference = metadataScope as AssemblyNameReference;
			ValueTuple<string, string> valueTuple;
			if (assemblyNameReference == null)
			{
				ModuleDefinition moduleDefinition = metadataScope as ModuleDefinition;
				if (moduleDefinition == null)
				{
					if (!(metadataScope is ModuleReference))
					{
						valueTuple = new ValueTuple<string, string>(null, null);
					}
					else
					{
						valueTuple = new ValueTuple<string, string>(typeReference2.Module.Assembly.Name.GetRuntimeHashedFullName(), typeReference2.Module.Name);
					}
				}
				else
				{
					valueTuple = new ValueTuple<string, string>(moduleDefinition.Assembly.Name.GetRuntimeHashedFullName(), moduleDefinition.Name);
				}
			}
			else
			{
				valueTuple = new ValueTuple<string, string>(assemblyNameReference.GetRuntimeHashedFullName(), null);
			}
			return valueTuple;
		}

		[NullableContext(2)]
		[CompilerGenerated]
		[return: Nullable(1)]
		internal static string <_ResolveReflection>g__ToCacheKeyPart|21_1(string asmName, string moduleName)
		{
			return " | " + (asmName ?? "NOASSEMBLY") + ", " + (moduleName ?? "NOMODULE");
		}

		[CompilerGenerated]
		internal static IEnumerable<MemberReference> <_ResolveReflection>g__GetGenericArgumentsRecursive|21_2(MemberReference mref)
		{
			ReflectionHelper.<<_ResolveReflection>g__GetGenericArgumentsRecursive|21_2>d <<_ResolveReflection>g__GetGenericArgumentsRecursive|21_2>d = new ReflectionHelper.<<_ResolveReflection>g__GetGenericArgumentsRecursive|21_2>d(-2);
			<<_ResolveReflection>g__GetGenericArgumentsRecursive|21_2>d.<>3__mref = mref;
			return <<_ResolveReflection>g__GetGenericArgumentsRecursive|21_2>d;
		}

		[CompilerGenerated]
		internal static void <ImportCallSite>g__ReadMethodSignature|52_0(IMethodSignature method, ref ReflectionHelper.<>c__DisplayClass52_0 A_1, ref ReflectionHelper.<>c__DisplayClass52_1 A_2)
		{
			byte b = A_2.reader.ReadByte();
			if ((b & 32) != 0)
			{
				method.HasThis = true;
				b = (byte)((int)b & -33);
			}
			if ((b & 64) != 0)
			{
				method.ExplicitThis = true;
				b = (byte)((int)b & -65);
			}
			method.CallingConvention = (MethodCallingConvention)b;
			if ((b & 16) != 0)
			{
				ReflectionHelper.<ImportCallSite>g__ReadCompressedUInt32|52_1(ref A_2);
			}
			uint num = ReflectionHelper.<ImportCallSite>g__ReadCompressedUInt32|52_1(ref A_2);
			method.MethodReturnType.ReturnType = ReflectionHelper.<ImportCallSite>g__ReadTypeSignature|52_4(ref A_1, ref A_2);
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				method.Parameters.Add(new ParameterDefinition(ReflectionHelper.<ImportCallSite>g__ReadTypeSignature|52_4(ref A_1, ref A_2)));
				num2++;
			}
		}

		[CompilerGenerated]
		internal static uint <ImportCallSite>g__ReadCompressedUInt32|52_1(ref ReflectionHelper.<>c__DisplayClass52_1 A_0)
		{
			byte b = A_0.reader.ReadByte();
			if ((b & 128) == 0)
			{
				return (uint)b;
			}
			if ((b & 64) == 0)
			{
				return (((uint)b & 4294967167U) << 8) | (uint)A_0.reader.ReadByte();
			}
			return (uint)((((int)b & -193) << 24) | ((int)A_0.reader.ReadByte() << 16) | ((int)A_0.reader.ReadByte() << 8) | (int)A_0.reader.ReadByte());
		}

		[CompilerGenerated]
		internal static int <ImportCallSite>g__ReadCompressedInt32|52_2(ref ReflectionHelper.<>c__DisplayClass52_1 A_0)
		{
			byte b = A_0.reader.ReadByte();
			A_0.reader.BaseStream.Seek(-1L, SeekOrigin.Current);
			uint num = ReflectionHelper.<ImportCallSite>g__ReadCompressedUInt32|52_1(ref A_0);
			int num2 = (int)num >> 1;
			if ((num & 1U) == 0U)
			{
				return num2;
			}
			int num3 = (int)(b & 192);
			if (num3 == 0 || num3 == 64)
			{
				return num2 - 64;
			}
			if (num3 != 128)
			{
				return num2 - 268435456;
			}
			return num2 - 8192;
		}

		[CompilerGenerated]
		internal static TypeReference <ImportCallSite>g__GetTypeDefOrRef|52_3(ref ReflectionHelper.<>c__DisplayClass52_0 A_0, ref ReflectionHelper.<>c__DisplayClass52_1 A_1)
		{
			uint num = ReflectionHelper.<ImportCallSite>g__ReadCompressedUInt32|52_1(ref A_1);
			uint num2 = num >> 2;
			uint num3;
			switch (num & 3U)
			{
			case 0U:
				num3 = 33554432U | num2;
				break;
			case 1U:
				num3 = 16777216U | num2;
				break;
			case 2U:
				num3 = 452984832U | num2;
				break;
			default:
				num3 = 0U;
				break;
			}
			return A_0.moduleTo.ImportReference(A_0.moduleFrom.ResolveType((int)num3));
		}

		[CompilerGenerated]
		internal static TypeReference <ImportCallSite>g__ReadTypeSignature|52_4(ref ReflectionHelper.<>c__DisplayClass52_0 A_0, ref ReflectionHelper.<>c__DisplayClass52_1 A_1)
		{
			MetadataType metadataType = (MetadataType)A_1.reader.ReadByte();
			switch (metadataType)
			{
			case MetadataType.Void:
				return A_0.moduleTo.TypeSystem.Void;
			case MetadataType.Boolean:
				return A_0.moduleTo.TypeSystem.Boolean;
			case MetadataType.Char:
				return A_0.moduleTo.TypeSystem.Char;
			case MetadataType.SByte:
				return A_0.moduleTo.TypeSystem.SByte;
			case MetadataType.Byte:
				return A_0.moduleTo.TypeSystem.Byte;
			case MetadataType.Int16:
				return A_0.moduleTo.TypeSystem.Int16;
			case MetadataType.UInt16:
				return A_0.moduleTo.TypeSystem.UInt16;
			case MetadataType.Int32:
				return A_0.moduleTo.TypeSystem.Int32;
			case MetadataType.UInt32:
				return A_0.moduleTo.TypeSystem.UInt32;
			case MetadataType.Int64:
				return A_0.moduleTo.TypeSystem.Int64;
			case MetadataType.UInt64:
				return A_0.moduleTo.TypeSystem.UInt64;
			case MetadataType.Single:
				return A_0.moduleTo.TypeSystem.Single;
			case MetadataType.Double:
				return A_0.moduleTo.TypeSystem.Double;
			case MetadataType.String:
				return A_0.moduleTo.TypeSystem.String;
			case MetadataType.Pointer:
				return new PointerType(ReflectionHelper.<ImportCallSite>g__ReadTypeSignature|52_4(ref A_0, ref A_1));
			case MetadataType.ByReference:
				return new ByReferenceType(ReflectionHelper.<ImportCallSite>g__ReadTypeSignature|52_4(ref A_0, ref A_1));
			case MetadataType.ValueType:
			case MetadataType.Class:
				return ReflectionHelper.<ImportCallSite>g__GetTypeDefOrRef|52_3(ref A_0, ref A_1);
			case MetadataType.Var:
			case MetadataType.GenericInstance:
			case MetadataType.MVar:
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Unsupported generic callsite element: ");
				defaultInterpolatedStringHandler.AppendFormatted<MetadataType>(metadataType);
				throw new NotSupportedException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			case MetadataType.Array:
			{
				ArrayType arrayType = new ArrayType(ReflectionHelper.<ImportCallSite>g__ReadTypeSignature|52_4(ref A_0, ref A_1));
				uint num = ReflectionHelper.<ImportCallSite>g__ReadCompressedUInt32|52_1(ref A_1);
				uint[] array = new uint[ReflectionHelper.<ImportCallSite>g__ReadCompressedUInt32|52_1(ref A_1)];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = ReflectionHelper.<ImportCallSite>g__ReadCompressedUInt32|52_1(ref A_1);
				}
				int[] array2 = new int[ReflectionHelper.<ImportCallSite>g__ReadCompressedUInt32|52_1(ref A_1)];
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = ReflectionHelper.<ImportCallSite>g__ReadCompressedInt32|52_2(ref A_1);
				}
				arrayType.Dimensions.Clear();
				int num2 = 0;
				while ((long)num2 < (long)((ulong)num))
				{
					int? num3 = null;
					int? num4 = null;
					if (num2 < array2.Length)
					{
						num3 = new int?(array2[num2]);
					}
					if (num2 < array.Length)
					{
						int? num5 = num3;
						int num6 = (int)array[num2];
						num4 = ((num5 != null) ? new int?(num5.GetValueOrDefault() + num6 - 1) : null);
					}
					arrayType.Dimensions.Add(new ArrayDimension(num3, num4));
					num2++;
				}
				return arrayType;
			}
			case MetadataType.TypedByReference:
				return A_0.moduleTo.TypeSystem.TypedReference;
			case (MetadataType)23:
			case (MetadataType)26:
				break;
			case MetadataType.IntPtr:
				return A_0.moduleTo.TypeSystem.IntPtr;
			case MetadataType.UIntPtr:
				return A_0.moduleTo.TypeSystem.UIntPtr;
			case MetadataType.FunctionPointer:
			{
				FunctionPointerType functionPointerType = new FunctionPointerType();
				ReflectionHelper.<ImportCallSite>g__ReadMethodSignature|52_0(functionPointerType, ref A_0, ref A_1);
				return functionPointerType;
			}
			case MetadataType.Object:
				return A_0.moduleTo.TypeSystem.Object;
			case (MetadataType)29:
				return new ArrayType(ReflectionHelper.<ImportCallSite>g__ReadTypeSignature|52_4(ref A_0, ref A_1));
			case MetadataType.RequiredModifier:
				return new RequiredModifierType(ReflectionHelper.<ImportCallSite>g__GetTypeDefOrRef|52_3(ref A_0, ref A_1), ReflectionHelper.<ImportCallSite>g__ReadTypeSignature|52_4(ref A_0, ref A_1));
			case MetadataType.OptionalModifier:
				return new OptionalModifierType(ReflectionHelper.<ImportCallSite>g__GetTypeDefOrRef|52_3(ref A_0, ref A_1), ReflectionHelper.<ImportCallSite>g__ReadTypeSignature|52_4(ref A_0, ref A_1));
			default:
				if (metadataType == MetadataType.Sentinel)
				{
					return new SentinelType(ReflectionHelper.<ImportCallSite>g__ReadTypeSignature|52_4(ref A_0, ref A_1));
				}
				if (metadataType == MetadataType.Pinned)
				{
					return new PinnedType(ReflectionHelper.<ImportCallSite>g__ReadTypeSignature|52_4(ref A_0, ref A_1));
				}
				break;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(30, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("Unsupported callsite element: ");
			defaultInterpolatedStringHandler2.AppendFormatted<MetadataType>(metadataType);
			throw new NotSupportedException(defaultInterpolatedStringHandler2.ToStringAndClear());
		}

		internal static readonly bool IsCoreBCL = typeof(object).Assembly.GetName().Name == "System.Private.CoreLib";

		internal static readonly Dictionary<string, WeakReference> AssemblyCache = new Dictionary<string, WeakReference>();

		internal static readonly Dictionary<string, WeakReference[]> AssembliesCache = new Dictionary<string, WeakReference[]>();

		internal static readonly Dictionary<string, WeakReference> ResolveReflectionCache = new Dictionary<string, WeakReference>();

		public static readonly byte[] AssemblyHashPrefix = new UTF8Encoding(false).GetBytes("MonoModRefl").Concat<byte>(new byte[1]).ToArray<byte>();

		public static readonly string AssemblyHashNameTag = "@#";

		private const BindingFlags _BindingFlagsAll = (BindingFlags)(-1);

		[Nullable(2)]
		private static readonly MethodInfo GetUnmanagedSigHelperMethod = typeof(SignatureHelper).GetMethod("GetMethodSigHelper", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
		{
			typeof(Module),
			typeof(CallingConvention),
			typeof(Type)
		}, null);

		private static readonly ReflectionHelper.GetUnmanagedSigHelperDelegate GetUnmanagedSigHelper;

		[Nullable(new byte[] { 1, 2 })]
		private static readonly object[] _CacheGetterArgs;

		[Nullable(2)]
		private static Type t_RuntimeType;

		[Nullable(2)]
		private static Type t_RuntimeTypeCache;

		[Nullable(2)]
		private static PropertyInfo p_RuntimeType_Cache;

		[Nullable(2)]
		private static MethodInfo m_RuntimeTypeCache_GetFieldList;

		[Nullable(2)]
		private static MethodInfo m_RuntimeTypeCache_GetPropertyList;

		private static readonly ConditionalWeakTable<Type, ReflectionHelper.CacheFixEntry> _CacheFixed;

		[Nullable(2)]
		private static Type t_RuntimeModule;

		[Nullable(2)]
		private static PropertyInfo p_RuntimeModule_RuntimeType;

		[Nullable(2)]
		private static FieldInfo f_RuntimeModule__impl;

		[Nullable(2)]
		private static MethodInfo m_RuntimeModule_GetGlobalType;

		[Nullable(2)]
		private static readonly FieldInfo f_SignatureHelper_module;

		[NullableContext(0)]
		[return: Nullable(1)]
		private delegate SignatureHelper GetUnmanagedSigHelperDelegate(Module module, CallingConvention callConv, Type returnType);

		[NullableContext(2)]
		[Nullable(0)]
		private class CacheFixEntry
		{
			public object Cache;

			public Array Properties;

			public Array Fields;

			public bool NeedsVerify;
		}
	}
}
