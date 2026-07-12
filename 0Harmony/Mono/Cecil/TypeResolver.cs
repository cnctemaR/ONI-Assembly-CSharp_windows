using System;
using Mono.Cecil.Cil;

namespace Mono.Cecil
{
	internal sealed class TypeResolver
	{
		public static TypeResolver For(TypeReference typeReference)
		{
			if (!typeReference.IsGenericInstance)
			{
				return new TypeResolver();
			}
			return new TypeResolver((GenericInstanceType)typeReference);
		}

		public static TypeResolver For(TypeReference typeReference, MethodReference methodReference)
		{
			return new TypeResolver(typeReference as GenericInstanceType, methodReference as GenericInstanceMethod);
		}

		public TypeResolver()
		{
		}

		public TypeResolver(GenericInstanceType typeDefinitionContext)
		{
			this._typeDefinitionContext = typeDefinitionContext;
		}

		public TypeResolver(GenericInstanceMethod methodDefinitionContext)
		{
			this._methodDefinitionContext = methodDefinitionContext;
		}

		public TypeResolver(GenericInstanceType typeDefinitionContext, GenericInstanceMethod methodDefinitionContext)
		{
			this._typeDefinitionContext = typeDefinitionContext;
			this._methodDefinitionContext = methodDefinitionContext;
		}

		public MethodReference Resolve(MethodReference method)
		{
			MethodReference methodReference = method;
			if (this.IsDummy())
			{
				return methodReference;
			}
			TypeReference typeReference = this.Resolve(method.DeclaringType);
			GenericInstanceMethod genericInstanceMethod = method as GenericInstanceMethod;
			if (genericInstanceMethod != null)
			{
				methodReference = new MethodReference(method.Name, method.ReturnType, typeReference);
				foreach (ParameterDefinition parameterDefinition in method.Parameters)
				{
					methodReference.Parameters.Add(new ParameterDefinition(parameterDefinition.Name, parameterDefinition.Attributes, parameterDefinition.ParameterType));
				}
				foreach (GenericParameter genericParameter in genericInstanceMethod.ElementMethod.GenericParameters)
				{
					methodReference.GenericParameters.Add(new GenericParameter(genericParameter.Name, methodReference));
				}
				methodReference.HasThis = method.HasThis;
				GenericInstanceMethod genericInstanceMethod2 = new GenericInstanceMethod(methodReference);
				foreach (TypeReference typeReference2 in genericInstanceMethod.GenericArguments)
				{
					genericInstanceMethod2.GenericArguments.Add(this.Resolve(typeReference2));
				}
				methodReference = genericInstanceMethod2;
			}
			else
			{
				methodReference = new MethodReference(method.Name, method.ReturnType, typeReference);
				foreach (GenericParameter genericParameter2 in method.GenericParameters)
				{
					methodReference.GenericParameters.Add(new GenericParameter(genericParameter2.Name, methodReference));
				}
				foreach (ParameterDefinition parameterDefinition2 in method.Parameters)
				{
					methodReference.Parameters.Add(new ParameterDefinition(parameterDefinition2.Name, parameterDefinition2.Attributes, parameterDefinition2.ParameterType));
				}
				methodReference.HasThis = method.HasThis;
			}
			return methodReference;
		}

		public FieldReference Resolve(FieldReference field)
		{
			TypeReference typeReference = this.Resolve(field.DeclaringType);
			if (typeReference == field.DeclaringType)
			{
				return field;
			}
			return new FieldReference(field.Name, field.FieldType, typeReference);
		}

		public TypeReference ResolveReturnType(MethodReference method)
		{
			return this.Resolve(GenericParameterResolver.ResolveReturnTypeIfNeeded(method));
		}

		public TypeReference ResolveParameterType(MethodReference method, ParameterReference parameter)
		{
			return this.Resolve(GenericParameterResolver.ResolveParameterTypeIfNeeded(method, parameter));
		}

		public TypeReference ResolveVariableType(MethodReference method, VariableReference variable)
		{
			return this.Resolve(GenericParameterResolver.ResolveVariableTypeIfNeeded(method, variable));
		}

		public TypeReference ResolveFieldType(FieldReference field)
		{
			return this.Resolve(GenericParameterResolver.ResolveFieldTypeIfNeeded(field));
		}

		public TypeReference Resolve(TypeReference typeReference)
		{
			return this.Resolve(typeReference, true);
		}

		public TypeReference Resolve(TypeReference typeReference, bool includeTypeDefinitions)
		{
			if (this.IsDummy())
			{
				return typeReference;
			}
			if (this._typeDefinitionContext != null && this._typeDefinitionContext.GenericArguments.Contains(typeReference))
			{
				return typeReference;
			}
			if (this._methodDefinitionContext != null && this._methodDefinitionContext.GenericArguments.Contains(typeReference))
			{
				return typeReference;
			}
			GenericParameter genericParameter = typeReference as GenericParameter;
			if (genericParameter != null)
			{
				if (this._typeDefinitionContext != null && this._typeDefinitionContext.GenericArguments.Contains(genericParameter))
				{
					return genericParameter;
				}
				if (this._methodDefinitionContext != null && this._methodDefinitionContext.GenericArguments.Contains(genericParameter))
				{
					return genericParameter;
				}
				return this.ResolveGenericParameter(genericParameter);
			}
			else
			{
				ArrayType arrayType = typeReference as ArrayType;
				if (arrayType != null)
				{
					return new ArrayType(this.Resolve(arrayType.ElementType), arrayType.Rank);
				}
				PointerType pointerType = typeReference as PointerType;
				if (pointerType != null)
				{
					return new PointerType(this.Resolve(pointerType.ElementType));
				}
				ByReferenceType byReferenceType = typeReference as ByReferenceType;
				if (byReferenceType != null)
				{
					return new ByReferenceType(this.Resolve(byReferenceType.ElementType));
				}
				PinnedType pinnedType = typeReference as PinnedType;
				if (pinnedType != null)
				{
					return new PinnedType(this.Resolve(pinnedType.ElementType));
				}
				GenericInstanceType genericInstanceType = typeReference as GenericInstanceType;
				if (genericInstanceType != null)
				{
					GenericInstanceType genericInstanceType2 = new GenericInstanceType(genericInstanceType.ElementType);
					foreach (TypeReference typeReference2 in genericInstanceType.GenericArguments)
					{
						genericInstanceType2.GenericArguments.Add(this.Resolve(typeReference2));
					}
					return genericInstanceType2;
				}
				RequiredModifierType requiredModifierType = typeReference as RequiredModifierType;
				if (requiredModifierType != null)
				{
					return this.Resolve(requiredModifierType.ElementType, includeTypeDefinitions);
				}
				if (includeTypeDefinitions)
				{
					TypeDefinition typeDefinition = typeReference as TypeDefinition;
					if (typeDefinition != null && typeDefinition.HasGenericParameters)
					{
						GenericInstanceType genericInstanceType3 = new GenericInstanceType(typeDefinition);
						foreach (GenericParameter genericParameter2 in typeDefinition.GenericParameters)
						{
							genericInstanceType3.GenericArguments.Add(this.Resolve(genericParameter2));
						}
						return genericInstanceType3;
					}
				}
				if (typeReference is TypeSpecification)
				{
					throw new NotSupportedException(string.Format("The type {0} cannot be resolved correctly.", typeReference.FullName));
				}
				return typeReference;
			}
		}

		internal TypeResolver Nested(GenericInstanceMethod genericInstanceMethod)
		{
			return new TypeResolver(this._typeDefinitionContext as GenericInstanceType, genericInstanceMethod);
		}

		private TypeReference ResolveGenericParameter(GenericParameter genericParameter)
		{
			if (genericParameter.Owner == null)
			{
				return this.HandleOwnerlessInvalidILCode(genericParameter);
			}
			if (!(genericParameter.Owner is MemberReference))
			{
				throw new NotSupportedException();
			}
			if (genericParameter.Type == GenericParameterType.Type)
			{
				return this._typeDefinitionContext.GenericArguments[genericParameter.Position];
			}
			if (this._methodDefinitionContext == null)
			{
				return genericParameter;
			}
			return this._methodDefinitionContext.GenericArguments[genericParameter.Position];
		}

		private TypeReference HandleOwnerlessInvalidILCode(GenericParameter genericParameter)
		{
			if (genericParameter.Type == GenericParameterType.Method && this._typeDefinitionContext != null && genericParameter.Position < this._typeDefinitionContext.GenericArguments.Count)
			{
				return this._typeDefinitionContext.GenericArguments[genericParameter.Position];
			}
			return genericParameter.Module.TypeSystem.Object;
		}

		private bool IsDummy()
		{
			return this._typeDefinitionContext == null && this._methodDefinitionContext == null;
		}

		private readonly IGenericInstance _typeDefinitionContext;

		private readonly IGenericInstance _methodDefinitionContext;
	}
}
