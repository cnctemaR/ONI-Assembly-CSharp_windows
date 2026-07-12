using System;
using System.Collections.Generic;

namespace Mono.Cecil
{
	internal sealed class TypeReferenceEqualityComparer : EqualityComparer<TypeReference>
	{
		public override bool Equals(TypeReference x, TypeReference y)
		{
			return TypeReferenceEqualityComparer.AreEqual(x, y, TypeComparisonMode.Exact);
		}

		public override int GetHashCode(TypeReference obj)
		{
			return TypeReferenceEqualityComparer.GetHashCodeFor(obj);
		}

		public static bool AreEqual(TypeReference a, TypeReference b, TypeComparisonMode comparisonMode = TypeComparisonMode.Exact)
		{
			if (a == b)
			{
				return true;
			}
			if (a == null || b == null)
			{
				return false;
			}
			MetadataType metadataType = a.MetadataType;
			MetadataType metadataType2 = b.MetadataType;
			if (metadataType == MetadataType.GenericInstance || metadataType2 == MetadataType.GenericInstance)
			{
				return metadataType == metadataType2 && TypeReferenceEqualityComparer.AreEqual((GenericInstanceType)a, (GenericInstanceType)b, comparisonMode);
			}
			if (metadataType == MetadataType.Array || metadataType2 == MetadataType.Array)
			{
				if (metadataType != metadataType2)
				{
					return false;
				}
				ArrayType arrayType = (ArrayType)a;
				ArrayType arrayType2 = (ArrayType)b;
				return arrayType.Rank == arrayType2.Rank && TypeReferenceEqualityComparer.AreEqual(arrayType.ElementType, arrayType2.ElementType, comparisonMode);
			}
			else
			{
				if (metadataType == MetadataType.Var || metadataType2 == MetadataType.Var)
				{
					return metadataType == metadataType2 && TypeReferenceEqualityComparer.AreEqual((GenericParameter)a, (GenericParameter)b, comparisonMode);
				}
				if (metadataType == MetadataType.MVar || metadataType2 == MetadataType.MVar)
				{
					return metadataType == metadataType2 && TypeReferenceEqualityComparer.AreEqual((GenericParameter)a, (GenericParameter)b, comparisonMode);
				}
				if (metadataType == MetadataType.ByReference || metadataType2 == MetadataType.ByReference)
				{
					return metadataType == metadataType2 && TypeReferenceEqualityComparer.AreEqual(((ByReferenceType)a).ElementType, ((ByReferenceType)b).ElementType, comparisonMode);
				}
				if (metadataType == MetadataType.Pointer || metadataType2 == MetadataType.Pointer)
				{
					return metadataType == metadataType2 && TypeReferenceEqualityComparer.AreEqual(((PointerType)a).ElementType, ((PointerType)b).ElementType, comparisonMode);
				}
				if (metadataType == MetadataType.RequiredModifier || metadataType2 == MetadataType.RequiredModifier)
				{
					if (metadataType != metadataType2)
					{
						return false;
					}
					RequiredModifierType requiredModifierType = (RequiredModifierType)a;
					RequiredModifierType requiredModifierType2 = (RequiredModifierType)b;
					return TypeReferenceEqualityComparer.AreEqual(requiredModifierType.ModifierType, requiredModifierType2.ModifierType, comparisonMode) && TypeReferenceEqualityComparer.AreEqual(requiredModifierType.ElementType, requiredModifierType2.ElementType, comparisonMode);
				}
				else if (metadataType == MetadataType.OptionalModifier || metadataType2 == MetadataType.OptionalModifier)
				{
					if (metadataType != metadataType2)
					{
						return false;
					}
					OptionalModifierType optionalModifierType = (OptionalModifierType)a;
					OptionalModifierType optionalModifierType2 = (OptionalModifierType)b;
					return TypeReferenceEqualityComparer.AreEqual(optionalModifierType.ModifierType, optionalModifierType2.ModifierType, comparisonMode) && TypeReferenceEqualityComparer.AreEqual(optionalModifierType.ElementType, optionalModifierType2.ElementType, comparisonMode);
				}
				else
				{
					if (metadataType == MetadataType.Pinned || metadataType2 == MetadataType.Pinned)
					{
						return metadataType == metadataType2 && TypeReferenceEqualityComparer.AreEqual(((PinnedType)a).ElementType, ((PinnedType)b).ElementType, comparisonMode);
					}
					if (metadataType == MetadataType.Sentinel || metadataType2 == MetadataType.Sentinel)
					{
						return metadataType == metadataType2 && TypeReferenceEqualityComparer.AreEqual(((SentinelType)a).ElementType, ((SentinelType)b).ElementType, comparisonMode);
					}
					if (!a.Name.Equals(b.Name) || !a.Namespace.Equals(b.Namespace))
					{
						return false;
					}
					TypeDefinition typeDefinition = a.Resolve();
					TypeDefinition typeDefinition2 = b.Resolve();
					if (comparisonMode == TypeComparisonMode.SignatureOnlyLoose)
					{
						return !(typeDefinition.Module.Name != typeDefinition2.Module.Name) && !(typeDefinition.Module.Assembly.Name.Name != typeDefinition2.Module.Assembly.Name.Name) && typeDefinition.FullName == typeDefinition2.FullName;
					}
					return typeDefinition == typeDefinition2;
				}
			}
		}

		private static bool AreEqual(GenericParameter a, GenericParameter b, TypeComparisonMode comparisonMode = TypeComparisonMode.Exact)
		{
			if (a == b)
			{
				return true;
			}
			if (a.Position != b.Position)
			{
				return false;
			}
			if (a.Type != b.Type)
			{
				return false;
			}
			TypeReference typeReference = a.Owner as TypeReference;
			if (typeReference != null && TypeReferenceEqualityComparer.AreEqual(typeReference, b.Owner as TypeReference, comparisonMode))
			{
				return true;
			}
			MethodReference methodReference = a.Owner as MethodReference;
			return (methodReference != null && comparisonMode != TypeComparisonMode.SignatureOnlyLoose && MethodReferenceComparer.AreEqual(methodReference, b.Owner as MethodReference)) || comparisonMode == TypeComparisonMode.SignatureOnly || comparisonMode == TypeComparisonMode.SignatureOnlyLoose;
		}

		private static bool AreEqual(GenericInstanceType a, GenericInstanceType b, TypeComparisonMode comparisonMode = TypeComparisonMode.Exact)
		{
			if (a == b)
			{
				return true;
			}
			int count = a.GenericArguments.Count;
			if (count != b.GenericArguments.Count)
			{
				return false;
			}
			if (!TypeReferenceEqualityComparer.AreEqual(a.ElementType, b.ElementType, comparisonMode))
			{
				return false;
			}
			for (int i = 0; i < count; i++)
			{
				if (!TypeReferenceEqualityComparer.AreEqual(a.GenericArguments[i], b.GenericArguments[i], comparisonMode))
				{
					return false;
				}
			}
			return true;
		}

		public static int GetHashCodeFor(TypeReference obj)
		{
			MetadataType metadataType = obj.MetadataType;
			if (metadataType == MetadataType.GenericInstance)
			{
				GenericInstanceType genericInstanceType = (GenericInstanceType)obj;
				int num = TypeReferenceEqualityComparer.GetHashCodeFor(genericInstanceType.ElementType) * 486187739 + 31;
				for (int i = 0; i < genericInstanceType.GenericArguments.Count; i++)
				{
					num = num * 486187739 + TypeReferenceEqualityComparer.GetHashCodeFor(genericInstanceType.GenericArguments[i]);
				}
				return num;
			}
			if (metadataType == MetadataType.Array)
			{
				ArrayType arrayType = (ArrayType)obj;
				return TypeReferenceEqualityComparer.GetHashCodeFor(arrayType.ElementType) * 486187739 + arrayType.Rank.GetHashCode();
			}
			if (metadataType == MetadataType.Var || metadataType == MetadataType.MVar)
			{
				GenericParameter genericParameter = (GenericParameter)obj;
				int num2 = genericParameter.Position.GetHashCode() * 486187739;
				int num3 = (int)metadataType;
				int num4 = num2 + num3.GetHashCode();
				TypeReference typeReference = genericParameter.Owner as TypeReference;
				if (typeReference != null)
				{
					return num4 * 486187739 + TypeReferenceEqualityComparer.GetHashCodeFor(typeReference);
				}
				MethodReference methodReference = genericParameter.Owner as MethodReference;
				if (methodReference != null)
				{
					return num4 * 486187739 + MethodReferenceComparer.GetHashCodeFor(methodReference);
				}
				throw new InvalidOperationException("Generic parameter encountered with invalid owner");
			}
			else
			{
				if (metadataType == MetadataType.ByReference)
				{
					return TypeReferenceEqualityComparer.GetHashCodeFor(((ByReferenceType)obj).ElementType) * 486187739 * 37;
				}
				if (metadataType == MetadataType.Pointer)
				{
					return TypeReferenceEqualityComparer.GetHashCodeFor(((PointerType)obj).ElementType) * 486187739 * 41;
				}
				if (metadataType == MetadataType.RequiredModifier)
				{
					RequiredModifierType requiredModifierType = (RequiredModifierType)obj;
					return TypeReferenceEqualityComparer.GetHashCodeFor(requiredModifierType.ElementType) * 43 * 486187739 + TypeReferenceEqualityComparer.GetHashCodeFor(requiredModifierType.ModifierType);
				}
				if (metadataType == MetadataType.OptionalModifier)
				{
					OptionalModifierType optionalModifierType = (OptionalModifierType)obj;
					return TypeReferenceEqualityComparer.GetHashCodeFor(optionalModifierType.ElementType) * 47 * 486187739 + TypeReferenceEqualityComparer.GetHashCodeFor(optionalModifierType.ModifierType);
				}
				if (metadataType == MetadataType.Pinned)
				{
					return TypeReferenceEqualityComparer.GetHashCodeFor(((PinnedType)obj).ElementType) * 486187739 * 53;
				}
				if (metadataType == MetadataType.Sentinel)
				{
					return TypeReferenceEqualityComparer.GetHashCodeFor(((SentinelType)obj).ElementType) * 486187739 * 59;
				}
				if (metadataType == MetadataType.FunctionPointer)
				{
					throw new NotImplementedException("We currently don't handle function pointer types.");
				}
				return obj.Namespace.GetHashCode() * 486187739 + obj.FullName.GetHashCode();
			}
		}
	}
}
