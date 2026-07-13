using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class MMReflectionImporter : IReflectionImporter
	{
		public bool UseDefault { get; set; }

		public MMReflectionImporter(ModuleDefinition module)
		{
			Helpers.ThrowIfArgumentNull<ModuleDefinition>(module, "module");
			this.Module = module;
			this.Default = new DefaultReflectionImporter(module);
			this.ElementTypes = new Dictionary<Type, TypeReference>
			{
				{
					typeof(void),
					module.TypeSystem.Void
				},
				{
					typeof(bool),
					module.TypeSystem.Boolean
				},
				{
					typeof(char),
					module.TypeSystem.Char
				},
				{
					typeof(sbyte),
					module.TypeSystem.SByte
				},
				{
					typeof(byte),
					module.TypeSystem.Byte
				},
				{
					typeof(short),
					module.TypeSystem.Int16
				},
				{
					typeof(ushort),
					module.TypeSystem.UInt16
				},
				{
					typeof(int),
					module.TypeSystem.Int32
				},
				{
					typeof(uint),
					module.TypeSystem.UInt32
				},
				{
					typeof(long),
					module.TypeSystem.Int64
				},
				{
					typeof(ulong),
					module.TypeSystem.UInt64
				},
				{
					typeof(float),
					module.TypeSystem.Single
				},
				{
					typeof(double),
					module.TypeSystem.Double
				},
				{
					typeof(string),
					module.TypeSystem.String
				},
				{
					typeof(TypedReference),
					module.TypeSystem.TypedReference
				},
				{
					typeof(IntPtr),
					module.TypeSystem.IntPtr
				},
				{
					typeof(UIntPtr),
					module.TypeSystem.UIntPtr
				},
				{
					typeof(object),
					module.TypeSystem.Object
				}
			};
		}

		private bool TryGetCachedType(Type type, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TypeReference typeRef, MMReflectionImporter.GenericImportKind importKind)
		{
			if (importKind == MMReflectionImporter.GenericImportKind.Definition)
			{
				typeRef = null;
				return false;
			}
			return this.CachedTypes.TryGetValue(type, out typeRef);
		}

		private TypeReference SetCachedType(Type type, TypeReference typeRef, MMReflectionImporter.GenericImportKind importKind)
		{
			if (importKind == MMReflectionImporter.GenericImportKind.Definition)
			{
				return typeRef;
			}
			this.CachedTypes[type] = typeRef;
			return typeRef;
		}

		[Obsolete("Please use the Assembly overload instead.")]
		public AssemblyNameReference ImportReference(AssemblyName reference)
		{
			Helpers.ThrowIfArgumentNull<AssemblyName>(reference, "reference");
			return this.Default.ImportReference(reference);
		}

		public AssemblyNameReference ImportReference(Assembly asm)
		{
			MMReflectionImporter.<>c__DisplayClass20_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.asm = asm;
			Helpers.ThrowIfArgumentNull<Assembly>(CS$<>8__locals1.asm, "asm");
			AssemblyNameReference assemblyNameReference;
			if (this.CachedAsms.TryGetValue(CS$<>8__locals1.asm, out assemblyNameReference))
			{
				return assemblyNameReference;
			}
			assemblyNameReference = this.<ImportReference>g__ImportReference|20_1(CS$<>8__locals1.asm.GetName(), ref CS$<>8__locals1);
			assemblyNameReference.ApplyRuntimeHash(CS$<>8__locals1.asm);
			return this.CachedAsms[CS$<>8__locals1.asm] = assemblyNameReference;
		}

		public TypeReference ImportModuleType(Module module, [Nullable(2)] IGenericParameterProvider context)
		{
			Helpers.ThrowIfArgumentNull<Module>(module, "module");
			TypeReference typeReference;
			if (this.CachedModuleTypes.TryGetValue(module, out typeReference))
			{
				return typeReference;
			}
			return this.CachedModuleTypes[module] = new TypeReference(string.Empty, "<Module>", this.Module, this.ImportReference(module.Assembly));
		}

		public TypeReference ImportReference(Type type, [Nullable(2)] IGenericParameterProvider context)
		{
			Helpers.ThrowIfArgumentNull<Type>(type, "type");
			return this._ImportReference(type, context, (context != null) ? MMReflectionImporter.GenericImportKind.Open : MMReflectionImporter.GenericImportKind.Definition);
		}

		private static bool _IsGenericInstance(Type type, MMReflectionImporter.GenericImportKind importKind)
		{
			Helpers.ThrowIfArgumentNull<Type>(type, "type");
			return (type.IsGenericType && !type.IsGenericTypeDefinition) || (type.IsGenericType && type.IsGenericTypeDefinition && importKind == MMReflectionImporter.GenericImportKind.Open);
		}

		private GenericInstanceType _ImportGenericInstance(Type type, [Nullable(2)] IGenericParameterProvider context, TypeReference typeRef)
		{
			Helpers.ThrowIfArgumentNull<Type>(type, "type");
			Helpers.ThrowIfArgumentNull<TypeReference>(typeRef, "typeRef");
			GenericInstanceType genericInstanceType = new GenericInstanceType(typeRef);
			foreach (Type type2 in type.GetGenericArguments())
			{
				genericInstanceType.GenericArguments.Add(this._ImportReference(type2, context, MMReflectionImporter.GenericImportKind.Open));
			}
			return genericInstanceType;
		}

		private TypeReference _ImportReference(Type type, [Nullable(2)] IGenericParameterProvider context, MMReflectionImporter.GenericImportKind importKind = MMReflectionImporter.GenericImportKind.Open)
		{
			Helpers.ThrowIfArgumentNull<Type>(type, "type");
			TypeReference typeReference;
			if (this.TryGetCachedType(type, out typeReference, importKind))
			{
				if (!MMReflectionImporter._IsGenericInstance(type, importKind))
				{
					return typeReference;
				}
				return this._ImportGenericInstance(type, context, typeReference);
			}
			else
			{
				if (this.UseDefault)
				{
					return this.SetCachedType(type, this.Default.ImportReference(type, context), importKind);
				}
				if (type.HasElementType)
				{
					if (type.IsByRef)
					{
						return this.SetCachedType(type, new ByReferenceType(this._ImportReference(type.GetElementType(), context, MMReflectionImporter.GenericImportKind.Open)), importKind);
					}
					if (type.IsPointer)
					{
						return this.SetCachedType(type, new PointerType(this._ImportReference(type.GetElementType(), context, MMReflectionImporter.GenericImportKind.Open)), importKind);
					}
					if (type.IsArray)
					{
						ArrayType arrayType = new ArrayType(this._ImportReference(type.GetElementType(), context, MMReflectionImporter.GenericImportKind.Open), type.GetArrayRank());
						if (type != type.GetElementType().MakeArrayType())
						{
							for (int i = 0; i < arrayType.Rank; i++)
							{
								arrayType.Dimensions[i] = new ArrayDimension(new int?(0), null);
							}
						}
						return this.CachedTypes[type] = arrayType;
					}
				}
				if (MMReflectionImporter._IsGenericInstance(type, importKind))
				{
					return this._ImportGenericInstance(type, context, this._ImportReference(type.GetGenericTypeDefinition(), context, MMReflectionImporter.GenericImportKind.Definition));
				}
				if (type.IsGenericParameter)
				{
					return this.SetCachedType(type, MMReflectionImporter.ImportGenericParameter(type, context), importKind);
				}
				if (this.ElementTypes.TryGetValue(type, out typeReference))
				{
					return this.SetCachedType(type, typeReference, importKind);
				}
				typeReference = new TypeReference(string.Empty, type.Name, this.Module, this.ImportReference(type.Assembly), type.IsValueType);
				if (type.IsNested)
				{
					typeReference.DeclaringType = this._ImportReference(type.DeclaringType, context, importKind);
				}
				else if (type.Namespace != null)
				{
					typeReference.Namespace = type.Namespace;
				}
				if (type.IsGenericType)
				{
					foreach (Type type2 in type.GetGenericArguments())
					{
						typeReference.GenericParameters.Add(new GenericParameter(type2.Name, typeReference));
					}
				}
				return this.SetCachedType(type, typeReference, importKind);
			}
		}

		private static GenericParameter ImportGenericParameter(Type type, [Nullable(2)] IGenericParameterProvider context)
		{
			Helpers.ThrowIfArgumentNull<Type>(type, "type");
			MethodReference methodReference = context as MethodReference;
			if (methodReference != null)
			{
				if (type.DeclaringMethod != null)
				{
					return methodReference.GenericParameters[type.GenericParameterPosition];
				}
				context = methodReference.DeclaringType;
			}
			Type declaringType = type.DeclaringType;
			if (declaringType == null)
			{
				throw new InvalidOperationException();
			}
			Type type2 = declaringType;
			TypeReference typeReference = context as TypeReference;
			if (typeReference != null)
			{
				while (typeReference != null)
				{
					TypeReference elementType = typeReference.GetElementType();
					if (elementType.Is(type2))
					{
						return elementType.GenericParameters[type.GenericParameterPosition];
					}
					if (typeReference.Is(type2))
					{
						return typeReference.GenericParameters[type.GenericParameterPosition];
					}
					typeReference = typeReference.DeclaringType;
				}
			}
			throw new NotSupportedException();
		}

		public FieldReference ImportReference(FieldInfo field, [Nullable(2)] IGenericParameterProvider context)
		{
			Helpers.ThrowIfArgumentNull<FieldInfo>(field, "field");
			FieldReference fieldReference;
			if (this.CachedFields.TryGetValue(field, out fieldReference))
			{
				return fieldReference;
			}
			if (this.UseDefault)
			{
				return this.CachedFields[field] = this.Default.ImportReference(field, context);
			}
			Type declaringType = field.DeclaringType;
			TypeReference typeReference = ((declaringType != null) ? this.ImportReference(declaringType, context) : this.ImportModuleType(field.Module, context));
			FieldInfo fieldInfo = field;
			if (declaringType != null && declaringType.IsGenericType)
			{
				field = field.Module.ResolveField(field.MetadataToken);
			}
			TypeReference typeReference2 = this._ImportReference(field.FieldType, typeReference, MMReflectionImporter.GenericImportKind.Open);
			Type[] requiredCustomModifiers = field.GetRequiredCustomModifiers();
			Type[] optionalCustomModifiers = field.GetOptionalCustomModifiers();
			foreach (Type type in requiredCustomModifiers)
			{
				typeReference2 = new RequiredModifierType(this._ImportReference(type, typeReference, MMReflectionImporter.GenericImportKind.Open), typeReference2);
			}
			foreach (Type type2 in optionalCustomModifiers)
			{
				typeReference2 = new OptionalModifierType(this._ImportReference(type2, typeReference, MMReflectionImporter.GenericImportKind.Open), typeReference2);
			}
			return this.CachedFields[fieldInfo] = new FieldReference(field.Name, typeReference2, typeReference);
		}

		public MethodReference ImportReference(MethodBase method, [Nullable(2)] IGenericParameterProvider context)
		{
			Helpers.ThrowIfArgumentNull<MethodBase>(method, "method");
			return this._ImportReference(method, context, (context != null) ? MMReflectionImporter.GenericImportKind.Open : MMReflectionImporter.GenericImportKind.Definition);
		}

		private MethodReference _ImportReference(MethodBase method, [Nullable(2)] IGenericParameterProvider context, MMReflectionImporter.GenericImportKind importKind)
		{
			Helpers.ThrowIfArgumentNull<MethodBase>(method, "method");
			MethodReference methodReference;
			if (this.CachedMethods.TryGetValue(method, out methodReference) && importKind == MMReflectionImporter.GenericImportKind.Open)
			{
				return methodReference;
			}
			MethodInfo methodInfo = method as MethodInfo;
			if (methodInfo != null && methodInfo.IsDynamicMethod())
			{
				return new DynamicMethodReference(this.Module, methodInfo);
			}
			if (this.UseDefault)
			{
				return this.CachedMethods[method] = this.Default.ImportReference(method, context);
			}
			if ((method.IsGenericMethod && !method.IsGenericMethodDefinition) || (method.IsGenericMethod && method.IsGenericMethodDefinition && importKind == MMReflectionImporter.GenericImportKind.Open))
			{
				GenericInstanceMethod genericInstanceMethod = new GenericInstanceMethod(this._ImportReference(((MethodInfo)method).GetGenericMethodDefinition(), context, MMReflectionImporter.GenericImportKind.Definition));
				foreach (Type type in method.GetGenericArguments())
				{
					genericInstanceMethod.GenericArguments.Add(this._ImportReference(type, context, MMReflectionImporter.GenericImportKind.Open));
				}
				return this.CachedMethods[method] = genericInstanceMethod;
			}
			Type declaringType = method.DeclaringType;
			methodReference = new MethodReference(method.Name, this._ImportReference(typeof(void), context, MMReflectionImporter.GenericImportKind.Open), (declaringType != null) ? this._ImportReference(declaringType, context, MMReflectionImporter.GenericImportKind.Definition) : this.ImportModuleType(method.Module, context));
			methodReference.HasThis = (method.CallingConvention & CallingConventions.HasThis) > (CallingConventions)0;
			methodReference.ExplicitThis = (method.CallingConvention & CallingConventions.ExplicitThis) > (CallingConventions)0;
			if ((method.CallingConvention & CallingConventions.VarArgs) != (CallingConventions)0)
			{
				methodReference.CallingConvention = MethodCallingConvention.VarArg;
			}
			MethodBase methodBase = method;
			if (declaringType != null && declaringType.IsGenericType)
			{
				method = method.Module.ResolveMethod(method.MetadataToken);
			}
			if (method.IsGenericMethodDefinition)
			{
				foreach (Type type2 in method.GetGenericArguments())
				{
					methodReference.GenericParameters.Add(new GenericParameter(type2.Name, methodReference));
				}
			}
			MethodReference methodReference2 = methodReference;
			MethodInfo methodInfo2 = method as MethodInfo;
			methodReference2.ReturnType = this._ImportReference(((methodInfo2 != null) ? methodInfo2.ReturnType : null) ?? typeof(void), methodReference, MMReflectionImporter.GenericImportKind.Open);
			foreach (ParameterInfo parameterInfo in method.GetParameters())
			{
				methodReference.Parameters.Add(new ParameterDefinition(parameterInfo.Name, (Mono.Cecil.ParameterAttributes)parameterInfo.Attributes, this._ImportReference(parameterInfo.ParameterType, methodReference, MMReflectionImporter.GenericImportKind.Open)));
			}
			return this.CachedMethods[methodBase] = methodReference;
		}

		[CompilerGenerated]
		private bool <ImportReference>g__TryGetAssemblyNameReference|20_0(AssemblyName name, [Nullable(2)] [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out AssemblyNameReference assembly_reference, ref MMReflectionImporter.<>c__DisplayClass20_0 A_3)
		{
			Collection<AssemblyNameReference> assemblyReferences = this.Module.AssemblyReferences;
			for (int i = 0; i < assemblyReferences.Count; i++)
			{
				AssemblyNameReference assemblyNameReference = assemblyReferences[i];
				if (!(name.FullName != assemblyNameReference.FullName) && assemblyNameReference.HashIs(A_3.asm, true))
				{
					assembly_reference = assemblyNameReference;
					return true;
				}
			}
			assembly_reference = null;
			return false;
		}

		[CompilerGenerated]
		private AssemblyNameReference <ImportReference>g__ImportReference|20_1(AssemblyName name, ref MMReflectionImporter.<>c__DisplayClass20_0 A_2)
		{
			AssemblyNameReference assemblyNameReference;
			if (this.<ImportReference>g__TryGetAssemblyNameReference|20_0(name, out assemblyNameReference, ref A_2))
			{
				return assemblyNameReference;
			}
			assemblyNameReference = new AssemblyNameReference(name.Name, name.Version)
			{
				PublicKeyToken = name.GetPublicKeyToken(),
				Culture = name.CultureInfo.Name,
				HashAlgorithm = (AssemblyHashAlgorithm)name.HashAlgorithm
			};
			this.Module.AssemblyReferences.Add(assemblyNameReference);
			return assemblyNameReference;
		}

		public static readonly IReflectionImporterProvider Provider = new MMReflectionImporter._Provider();

		public static readonly IReflectionImporterProvider ProviderNoDefault = new MMReflectionImporter._Provider
		{
			UseDefault = new bool?(false)
		};

		private readonly ModuleDefinition Module;

		private readonly DefaultReflectionImporter Default;

		private readonly Dictionary<Assembly, AssemblyNameReference> CachedAsms = new Dictionary<Assembly, AssemblyNameReference>();

		private readonly Dictionary<Module, TypeReference> CachedModuleTypes = new Dictionary<Module, TypeReference>();

		private readonly Dictionary<Type, TypeReference> CachedTypes = new Dictionary<Type, TypeReference>();

		private readonly Dictionary<FieldInfo, FieldReference> CachedFields = new Dictionary<FieldInfo, FieldReference>();

		private readonly Dictionary<MethodBase, MethodReference> CachedMethods = new Dictionary<MethodBase, MethodReference>();

		private readonly Dictionary<Type, TypeReference> ElementTypes;

		[NullableContext(0)]
		private class _Provider : IReflectionImporterProvider
		{
			[NullableContext(1)]
			public IReflectionImporter GetReflectionImporter(ModuleDefinition module)
			{
				Helpers.ThrowIfArgumentNull<ModuleDefinition>(module, "module");
				MMReflectionImporter mmreflectionImporter = new MMReflectionImporter(module);
				if (this.UseDefault != null)
				{
					mmreflectionImporter.UseDefault = this.UseDefault.Value;
				}
				return mmreflectionImporter;
			}

			public bool? UseDefault;
		}

		[NullableContext(0)]
		private enum GenericImportKind
		{
			Open,
			Definition
		}
	}
}
