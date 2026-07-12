using System;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Mono.Cecil
{
	internal sealed class GenericParameterResolver
	{
		internal static TypeReference ResolveReturnTypeIfNeeded(MethodReference methodReference)
		{
			if (methodReference.DeclaringType.IsArray && methodReference.Name == "Get")
			{
				return methodReference.ReturnType;
			}
			GenericInstanceMethod genericInstanceMethod = methodReference as GenericInstanceMethod;
			GenericInstanceType genericInstanceType = methodReference.DeclaringType as GenericInstanceType;
			if (genericInstanceMethod == null && genericInstanceType == null)
			{
				return methodReference.ReturnType;
			}
			return GenericParameterResolver.ResolveIfNeeded(genericInstanceMethod, genericInstanceType, methodReference.ReturnType);
		}

		internal static TypeReference ResolveFieldTypeIfNeeded(FieldReference fieldReference)
		{
			return GenericParameterResolver.ResolveIfNeeded(null, fieldReference.DeclaringType as GenericInstanceType, fieldReference.FieldType);
		}

		internal static TypeReference ResolveParameterTypeIfNeeded(MethodReference method, ParameterReference parameter)
		{
			GenericInstanceMethod genericInstanceMethod = method as GenericInstanceMethod;
			GenericInstanceType genericInstanceType = method.DeclaringType as GenericInstanceType;
			if (genericInstanceMethod == null && genericInstanceType == null)
			{
				return parameter.ParameterType;
			}
			return GenericParameterResolver.ResolveIfNeeded(genericInstanceMethod, genericInstanceType, parameter.ParameterType);
		}

		internal static TypeReference ResolveVariableTypeIfNeeded(MethodReference method, VariableReference variable)
		{
			GenericInstanceMethod genericInstanceMethod = method as GenericInstanceMethod;
			GenericInstanceType genericInstanceType = method.DeclaringType as GenericInstanceType;
			if (genericInstanceMethod == null && genericInstanceType == null)
			{
				return variable.VariableType;
			}
			return GenericParameterResolver.ResolveIfNeeded(genericInstanceMethod, genericInstanceType, variable.VariableType);
		}

		private static TypeReference ResolveIfNeeded(IGenericInstance genericInstanceMethod, IGenericInstance declaringGenericInstanceType, TypeReference parameterType)
		{
			ByReferenceType byReferenceType = parameterType as ByReferenceType;
			if (byReferenceType != null)
			{
				return GenericParameterResolver.ResolveIfNeeded(genericInstanceMethod, declaringGenericInstanceType, byReferenceType);
			}
			ArrayType arrayType = parameterType as ArrayType;
			if (arrayType != null)
			{
				return GenericParameterResolver.ResolveIfNeeded(genericInstanceMethod, declaringGenericInstanceType, arrayType);
			}
			GenericInstanceType genericInstanceType = parameterType as GenericInstanceType;
			if (genericInstanceType != null)
			{
				return GenericParameterResolver.ResolveIfNeeded(genericInstanceMethod, declaringGenericInstanceType, genericInstanceType);
			}
			GenericParameter genericParameter = parameterType as GenericParameter;
			if (genericParameter != null)
			{
				return GenericParameterResolver.ResolveIfNeeded(genericInstanceMethod, declaringGenericInstanceType, genericParameter);
			}
			RequiredModifierType requiredModifierType = parameterType as RequiredModifierType;
			if (requiredModifierType != null && GenericParameterResolver.ContainsGenericParameters(requiredModifierType))
			{
				return GenericParameterResolver.ResolveIfNeeded(genericInstanceMethod, declaringGenericInstanceType, requiredModifierType.ElementType);
			}
			if (GenericParameterResolver.ContainsGenericParameters(parameterType))
			{
				throw new Exception("Unexpected generic parameter.");
			}
			return parameterType;
		}

		private static TypeReference ResolveIfNeeded(IGenericInstance genericInstanceMethod, IGenericInstance genericInstanceType, GenericParameter genericParameterElement)
		{
			if (genericParameterElement.MetadataType != MetadataType.MVar)
			{
				return genericInstanceType.GenericArguments[genericParameterElement.Position];
			}
			if (genericInstanceMethod == null)
			{
				return genericParameterElement;
			}
			return genericInstanceMethod.GenericArguments[genericParameterElement.Position];
		}

		private static ArrayType ResolveIfNeeded(IGenericInstance genericInstanceMethod, IGenericInstance genericInstanceType, ArrayType arrayType)
		{
			return new ArrayType(GenericParameterResolver.ResolveIfNeeded(genericInstanceMethod, genericInstanceType, arrayType.ElementType), arrayType.Rank);
		}

		private static ByReferenceType ResolveIfNeeded(IGenericInstance genericInstanceMethod, IGenericInstance genericInstanceType, ByReferenceType byReferenceType)
		{
			return new ByReferenceType(GenericParameterResolver.ResolveIfNeeded(genericInstanceMethod, genericInstanceType, byReferenceType.ElementType));
		}

		private static GenericInstanceType ResolveIfNeeded(IGenericInstance genericInstanceMethod, IGenericInstance genericInstanceType, GenericInstanceType genericInstanceType1)
		{
			if (!GenericParameterResolver.ContainsGenericParameters(genericInstanceType1))
			{
				return genericInstanceType1;
			}
			GenericInstanceType genericInstanceType2 = new GenericInstanceType(genericInstanceType1.ElementType);
			foreach (TypeReference typeReference in genericInstanceType1.GenericArguments)
			{
				if (!typeReference.IsGenericParameter)
				{
					genericInstanceType2.GenericArguments.Add(GenericParameterResolver.ResolveIfNeeded(genericInstanceMethod, genericInstanceType, typeReference));
				}
				else
				{
					GenericParameter genericParameter = (GenericParameter)typeReference;
					GenericParameterType type = genericParameter.Type;
					if (type != GenericParameterType.Type)
					{
						if (type == GenericParameterType.Method)
						{
							if (genericInstanceMethod == null)
							{
								genericInstanceType2.GenericArguments.Add(genericParameter);
							}
							else
							{
								genericInstanceType2.GenericArguments.Add(genericInstanceMethod.GenericArguments[genericParameter.Position]);
							}
						}
					}
					else
					{
						if (genericInstanceType == null)
						{
							throw new NotSupportedException();
						}
						genericInstanceType2.GenericArguments.Add(genericInstanceType.GenericArguments[genericParameter.Position]);
					}
				}
			}
			return genericInstanceType2;
		}

		private static bool ContainsGenericParameters(TypeReference typeReference)
		{
			if (typeReference is GenericParameter)
			{
				return true;
			}
			ArrayType arrayType = typeReference as ArrayType;
			if (arrayType != null)
			{
				return GenericParameterResolver.ContainsGenericParameters(arrayType.ElementType);
			}
			PointerType pointerType = typeReference as PointerType;
			if (pointerType != null)
			{
				return GenericParameterResolver.ContainsGenericParameters(pointerType.ElementType);
			}
			ByReferenceType byReferenceType = typeReference as ByReferenceType;
			if (byReferenceType != null)
			{
				return GenericParameterResolver.ContainsGenericParameters(byReferenceType.ElementType);
			}
			SentinelType sentinelType = typeReference as SentinelType;
			if (sentinelType != null)
			{
				return GenericParameterResolver.ContainsGenericParameters(sentinelType.ElementType);
			}
			PinnedType pinnedType = typeReference as PinnedType;
			if (pinnedType != null)
			{
				return GenericParameterResolver.ContainsGenericParameters(pinnedType.ElementType);
			}
			RequiredModifierType requiredModifierType = typeReference as RequiredModifierType;
			if (requiredModifierType != null)
			{
				return GenericParameterResolver.ContainsGenericParameters(requiredModifierType.ElementType);
			}
			GenericInstanceType genericInstanceType = typeReference as GenericInstanceType;
			if (genericInstanceType != null)
			{
				using (Collection<TypeReference>.Enumerator enumerator = genericInstanceType.GenericArguments.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (GenericParameterResolver.ContainsGenericParameters(enumerator.Current))
						{
							return true;
						}
					}
				}
				return false;
			}
			if (typeReference is TypeSpecification)
			{
				throw new NotSupportedException();
			}
			return false;
		}
	}
}
