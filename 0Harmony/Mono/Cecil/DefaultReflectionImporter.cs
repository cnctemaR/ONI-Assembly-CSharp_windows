using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil.Metadata;
using Mono.Collections.Generic;

namespace Mono.Cecil
{
	internal class DefaultReflectionImporter : IReflectionImporter
	{
		public DefaultReflectionImporter(ModuleDefinition module)
		{
			Mixin.CheckModule(module);
			this.module = module;
		}

		private TypeReference ImportType(Type type, ImportGenericContext context, Type[] required_modifiers, Type[] optional_modifiers)
		{
			TypeReference typeReference = this.ImportType(type, context);
			foreach (Type type2 in required_modifiers)
			{
				typeReference = new RequiredModifierType(this.ImportType(type2, context), typeReference);
			}
			foreach (Type type3 in optional_modifiers)
			{
				typeReference = new OptionalModifierType(this.ImportType(type3, context), typeReference);
			}
			return typeReference;
		}

		private TypeReference ImportType(Type type, ImportGenericContext context)
		{
			return this.ImportType(type, context, DefaultReflectionImporter.ImportGenericKind.Open);
		}

		private TypeReference ImportType(Type type, ImportGenericContext context, DefaultReflectionImporter.ImportGenericKind import_kind)
		{
			if (DefaultReflectionImporter.IsTypeSpecification(type) || DefaultReflectionImporter.ImportOpenGenericType(type, import_kind))
			{
				return this.ImportTypeSpecification(type, context);
			}
			TypeReference typeReference = new TypeReference(string.Empty, type.Name, this.module, this.ImportScope(type), type.IsValueType);
			typeReference.etype = DefaultReflectionImporter.ImportElementType(type);
			if (DefaultReflectionImporter.IsNestedType(type))
			{
				typeReference.DeclaringType = this.ImportType(type.DeclaringType, context, import_kind);
			}
			else
			{
				typeReference.Namespace = type.Namespace ?? string.Empty;
			}
			if (type.IsGenericType)
			{
				DefaultReflectionImporter.ImportGenericParameters(typeReference, type.GetGenericArguments());
			}
			return typeReference;
		}

		protected virtual IMetadataScope ImportScope(Type type)
		{
			return this.ImportScope(type.Assembly);
		}

		private static bool ImportOpenGenericType(Type type, DefaultReflectionImporter.ImportGenericKind import_kind)
		{
			return type.IsGenericType && type.IsGenericTypeDefinition && import_kind == DefaultReflectionImporter.ImportGenericKind.Open;
		}

		private static bool ImportOpenGenericMethod(MethodBase method, DefaultReflectionImporter.ImportGenericKind import_kind)
		{
			return method.IsGenericMethod && method.IsGenericMethodDefinition && import_kind == DefaultReflectionImporter.ImportGenericKind.Open;
		}

		private static bool IsNestedType(Type type)
		{
			return type.IsNested;
		}

		private TypeReference ImportTypeSpecification(Type type, ImportGenericContext context)
		{
			if (type.IsByRef)
			{
				return new ByReferenceType(this.ImportType(type.GetElementType(), context));
			}
			if (type.IsPointer)
			{
				return new PointerType(this.ImportType(type.GetElementType(), context));
			}
			if (type.IsArray)
			{
				return new ArrayType(this.ImportType(type.GetElementType(), context), type.GetArrayRank());
			}
			if (type.IsGenericType)
			{
				return this.ImportGenericInstance(type, context);
			}
			if (type.IsGenericParameter)
			{
				return DefaultReflectionImporter.ImportGenericParameter(type, context);
			}
			throw new NotSupportedException(type.FullName);
		}

		private static TypeReference ImportGenericParameter(Type type, ImportGenericContext context)
		{
			if (context.IsEmpty)
			{
				throw new InvalidOperationException();
			}
			if (type.DeclaringMethod != null)
			{
				return context.MethodParameter(DefaultReflectionImporter.NormalizeMethodName(type.DeclaringMethod), type.GenericParameterPosition);
			}
			if (type.DeclaringType != null)
			{
				return context.TypeParameter(DefaultReflectionImporter.NormalizeTypeFullName(type.DeclaringType), type.GenericParameterPosition);
			}
			throw new InvalidOperationException();
		}

		private static string NormalizeMethodName(MethodBase method)
		{
			return DefaultReflectionImporter.NormalizeTypeFullName(method.DeclaringType) + "." + method.Name;
		}

		private static string NormalizeTypeFullName(Type type)
		{
			if (DefaultReflectionImporter.IsNestedType(type))
			{
				return DefaultReflectionImporter.NormalizeTypeFullName(type.DeclaringType) + "/" + type.Name;
			}
			return type.FullName;
		}

		private TypeReference ImportGenericInstance(Type type, ImportGenericContext context)
		{
			TypeReference typeReference = this.ImportType(type.GetGenericTypeDefinition(), context, DefaultReflectionImporter.ImportGenericKind.Definition);
			Type[] genericArguments = type.GetGenericArguments();
			GenericInstanceType genericInstanceType = new GenericInstanceType(typeReference, genericArguments.Length);
			Collection<TypeReference> genericArguments2 = genericInstanceType.GenericArguments;
			context.Push(typeReference);
			TypeReference typeReference2;
			try
			{
				for (int i = 0; i < genericArguments.Length; i++)
				{
					genericArguments2.Add(this.ImportType(genericArguments[i], context));
				}
				typeReference2 = genericInstanceType;
			}
			finally
			{
				context.Pop();
			}
			return typeReference2;
		}

		private static bool IsTypeSpecification(Type type)
		{
			return type.HasElementType || DefaultReflectionImporter.IsGenericInstance(type) || type.IsGenericParameter;
		}

		private static bool IsGenericInstance(Type type)
		{
			return type.IsGenericType && !type.IsGenericTypeDefinition;
		}

		private static ElementType ImportElementType(Type type)
		{
			ElementType elementType;
			if (!DefaultReflectionImporter.type_etype_mapping.TryGetValue(type, out elementType))
			{
				return ElementType.None;
			}
			return elementType;
		}

		protected AssemblyNameReference ImportScope(Assembly assembly)
		{
			return this.ImportReference(assembly.GetName());
		}

		public virtual AssemblyNameReference ImportReference(AssemblyName name)
		{
			Mixin.CheckName(name);
			AssemblyNameReference assemblyNameReference;
			if (this.TryGetAssemblyNameReference(name, out assemblyNameReference))
			{
				return assemblyNameReference;
			}
			assemblyNameReference = new AssemblyNameReference(name.Name, name.Version)
			{
				PublicKeyToken = name.GetPublicKeyToken(),
				Culture = name.CultureInfo.Name,
				HashAlgorithm = (AssemblyHashAlgorithm)name.HashAlgorithm
			};
			this.module.AssemblyReferences.Add(assemblyNameReference);
			return assemblyNameReference;
		}

		private bool TryGetAssemblyNameReference(AssemblyName name, out AssemblyNameReference assembly_reference)
		{
			Collection<AssemblyNameReference> assemblyReferences = this.module.AssemblyReferences;
			for (int i = 0; i < assemblyReferences.Count; i++)
			{
				AssemblyNameReference assemblyNameReference = assemblyReferences[i];
				if (!(name.FullName != assemblyNameReference.FullName))
				{
					assembly_reference = assemblyNameReference;
					return true;
				}
			}
			assembly_reference = null;
			return false;
		}

		private FieldReference ImportField(FieldInfo field, ImportGenericContext context)
		{
			TypeReference typeReference = this.ImportType(field.DeclaringType, context);
			if (DefaultReflectionImporter.IsGenericInstance(field.DeclaringType))
			{
				field = DefaultReflectionImporter.ResolveFieldDefinition(field);
			}
			context.Push(typeReference);
			FieldReference fieldReference;
			try
			{
				fieldReference = new FieldReference
				{
					Name = field.Name,
					DeclaringType = typeReference,
					FieldType = this.ImportType(field.FieldType, context, field.GetRequiredCustomModifiers(), field.GetOptionalCustomModifiers())
				};
			}
			finally
			{
				context.Pop();
			}
			return fieldReference;
		}

		private static FieldInfo ResolveFieldDefinition(FieldInfo field)
		{
			return field.Module.ResolveField(field.MetadataToken);
		}

		private static MethodBase ResolveMethodDefinition(MethodBase method)
		{
			return method.Module.ResolveMethod(method.MetadataToken);
		}

		private MethodReference ImportMethod(MethodBase method, ImportGenericContext context, DefaultReflectionImporter.ImportGenericKind import_kind)
		{
			if (DefaultReflectionImporter.IsMethodSpecification(method) || DefaultReflectionImporter.ImportOpenGenericMethod(method, import_kind))
			{
				return this.ImportMethodSpecification(method, context);
			}
			TypeReference typeReference = this.ImportType(method.DeclaringType, context);
			if (DefaultReflectionImporter.IsGenericInstance(method.DeclaringType))
			{
				method = DefaultReflectionImporter.ResolveMethodDefinition(method);
			}
			MethodReference methodReference = new MethodReference
			{
				Name = method.Name,
				HasThis = DefaultReflectionImporter.HasCallingConvention(method, CallingConventions.HasThis),
				ExplicitThis = DefaultReflectionImporter.HasCallingConvention(method, CallingConventions.ExplicitThis),
				DeclaringType = this.ImportType(method.DeclaringType, context, DefaultReflectionImporter.ImportGenericKind.Definition)
			};
			if (DefaultReflectionImporter.HasCallingConvention(method, CallingConventions.VarArgs))
			{
				methodReference.CallingConvention &= MethodCallingConvention.VarArg;
			}
			if (method.IsGenericMethod)
			{
				DefaultReflectionImporter.ImportGenericParameters(methodReference, method.GetGenericArguments());
			}
			context.Push(methodReference);
			MethodReference methodReference2;
			try
			{
				MethodInfo methodInfo = method as MethodInfo;
				methodReference.ReturnType = ((methodInfo != null) ? this.ImportType(methodInfo.ReturnType, context, methodInfo.ReturnParameter.GetRequiredCustomModifiers(), methodInfo.ReturnParameter.GetOptionalCustomModifiers()) : this.ImportType(typeof(void), default(ImportGenericContext)));
				ParameterInfo[] parameters = method.GetParameters();
				Collection<ParameterDefinition> parameters2 = methodReference.Parameters;
				foreach (ParameterInfo parameterInfo in parameters)
				{
					parameters2.Add(new ParameterDefinition(this.ImportType(parameterInfo.ParameterType, context, parameterInfo.GetRequiredCustomModifiers(), parameterInfo.GetOptionalCustomModifiers())));
				}
				methodReference.DeclaringType = typeReference;
				methodReference2 = methodReference;
			}
			finally
			{
				context.Pop();
			}
			return methodReference2;
		}

		private static void ImportGenericParameters(IGenericParameterProvider provider, Type[] arguments)
		{
			Collection<GenericParameter> genericParameters = provider.GenericParameters;
			for (int i = 0; i < arguments.Length; i++)
			{
				genericParameters.Add(new GenericParameter(arguments[i].Name, provider));
			}
		}

		private static bool IsMethodSpecification(MethodBase method)
		{
			return method.IsGenericMethod && !method.IsGenericMethodDefinition;
		}

		private MethodReference ImportMethodSpecification(MethodBase method, ImportGenericContext context)
		{
			MethodInfo methodInfo = method as MethodInfo;
			if (methodInfo == null)
			{
				throw new InvalidOperationException();
			}
			MethodReference methodReference = this.ImportMethod(methodInfo.GetGenericMethodDefinition(), context, DefaultReflectionImporter.ImportGenericKind.Definition);
			GenericInstanceMethod genericInstanceMethod = new GenericInstanceMethod(methodReference);
			Type[] genericArguments = method.GetGenericArguments();
			Collection<TypeReference> genericArguments2 = genericInstanceMethod.GenericArguments;
			context.Push(methodReference);
			MethodReference methodReference2;
			try
			{
				for (int i = 0; i < genericArguments.Length; i++)
				{
					genericArguments2.Add(this.ImportType(genericArguments[i], context));
				}
				methodReference2 = genericInstanceMethod;
			}
			finally
			{
				context.Pop();
			}
			return methodReference2;
		}

		private static bool HasCallingConvention(MethodBase method, CallingConventions conventions)
		{
			return (method.CallingConvention & conventions) > (CallingConventions)0;
		}

		public virtual TypeReference ImportReference(Type type, IGenericParameterProvider context)
		{
			Mixin.CheckType(type);
			return this.ImportType(type, ImportGenericContext.For(context), (context != null) ? DefaultReflectionImporter.ImportGenericKind.Open : DefaultReflectionImporter.ImportGenericKind.Definition);
		}

		public virtual FieldReference ImportReference(FieldInfo field, IGenericParameterProvider context)
		{
			Mixin.CheckField(field);
			return this.ImportField(field, ImportGenericContext.For(context));
		}

		public virtual MethodReference ImportReference(MethodBase method, IGenericParameterProvider context)
		{
			Mixin.CheckMethod(method);
			return this.ImportMethod(method, ImportGenericContext.For(context), (context != null) ? DefaultReflectionImporter.ImportGenericKind.Open : DefaultReflectionImporter.ImportGenericKind.Definition);
		}

		protected readonly ModuleDefinition module;

		private static readonly Dictionary<Type, ElementType> type_etype_mapping = new Dictionary<Type, ElementType>(18)
		{
			{
				typeof(void),
				ElementType.Void
			},
			{
				typeof(bool),
				ElementType.Boolean
			},
			{
				typeof(char),
				ElementType.Char
			},
			{
				typeof(sbyte),
				ElementType.I1
			},
			{
				typeof(byte),
				ElementType.U1
			},
			{
				typeof(short),
				ElementType.I2
			},
			{
				typeof(ushort),
				ElementType.U2
			},
			{
				typeof(int),
				ElementType.I4
			},
			{
				typeof(uint),
				ElementType.U4
			},
			{
				typeof(long),
				ElementType.I8
			},
			{
				typeof(ulong),
				ElementType.U8
			},
			{
				typeof(float),
				ElementType.R4
			},
			{
				typeof(double),
				ElementType.R8
			},
			{
				typeof(string),
				ElementType.String
			},
			{
				typeof(TypedReference),
				ElementType.TypedByRef
			},
			{
				typeof(IntPtr),
				ElementType.I
			},
			{
				typeof(UIntPtr),
				ElementType.U
			},
			{
				typeof(object),
				ElementType.Object
			}
		};

		private enum ImportGenericKind
		{
			Definition,
			Open
		}
	}
}
