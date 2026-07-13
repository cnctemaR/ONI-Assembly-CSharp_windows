using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mono.Cecil.Rocks
{
	internal class DocCommentId
	{
		private DocCommentId(IMemberDefinition member)
		{
			this.commentMember = member;
			this.id = new StringBuilder();
		}

		private void WriteField(FieldDefinition field)
		{
			this.WriteDefinition('F', field);
		}

		private void WriteEvent(EventDefinition @event)
		{
			this.WriteDefinition('E', @event);
		}

		private void WriteType(TypeDefinition type)
		{
			this.id.Append('T').Append(':');
			this.WriteTypeFullName(type);
		}

		private void WriteMethod(MethodDefinition method)
		{
			this.WriteDefinition('M', method);
			if (method.HasGenericParameters)
			{
				this.id.Append('`').Append('`');
				this.id.Append(method.GenericParameters.Count);
			}
			if (method.HasParameters)
			{
				this.WriteParameters(method.Parameters);
			}
			if (DocCommentId.IsConversionOperator(method))
			{
				this.WriteReturnType(method);
			}
		}

		private static bool IsConversionOperator(MethodDefinition self)
		{
			if (self == null)
			{
				throw new ArgumentNullException("self");
			}
			return self.IsSpecialName && (self.Name == "op_Explicit" || self.Name == "op_Implicit");
		}

		private void WriteReturnType(MethodDefinition method)
		{
			this.id.Append('~');
			this.WriteTypeSignature(method.ReturnType);
		}

		private void WriteProperty(PropertyDefinition property)
		{
			this.WriteDefinition('P', property);
			if (property.HasParameters)
			{
				this.WriteParameters(property.Parameters);
			}
		}

		private void WriteParameters(IList<ParameterDefinition> parameters)
		{
			this.id.Append('(');
			this.WriteList<ParameterDefinition>(parameters, delegate(ParameterDefinition p)
			{
				this.WriteTypeSignature(p.ParameterType);
			});
			this.id.Append(')');
		}

		private void WriteTypeSignature(TypeReference type)
		{
			MetadataType metadataType = type.MetadataType;
			switch (metadataType)
			{
			case MetadataType.Pointer:
				this.WriteTypeSignature(((PointerType)type).ElementType);
				this.id.Append('*');
				return;
			case MetadataType.ByReference:
				this.WriteTypeSignature(((ByReferenceType)type).ElementType);
				this.id.Append('@');
				return;
			case MetadataType.ValueType:
			case MetadataType.Class:
				break;
			case MetadataType.Var:
				if (this.IsGenericMethodTypeParameter(type))
				{
					this.id.Append('`');
				}
				this.id.Append('`');
				this.id.Append(((GenericParameter)type).Position);
				return;
			case MetadataType.Array:
				this.WriteArrayTypeSignature((ArrayType)type);
				return;
			case MetadataType.GenericInstance:
				this.WriteGenericInstanceTypeSignature((GenericInstanceType)type);
				return;
			default:
				switch (metadataType)
				{
				case MetadataType.FunctionPointer:
					this.WriteFunctionPointerTypeSignature((FunctionPointerType)type);
					return;
				case MetadataType.MVar:
					this.id.Append('`').Append('`');
					this.id.Append(((GenericParameter)type).Position);
					return;
				case MetadataType.RequiredModifier:
					this.WriteModiferTypeSignature((RequiredModifierType)type, '|');
					return;
				case MetadataType.OptionalModifier:
					this.WriteModiferTypeSignature((OptionalModifierType)type, '!');
					return;
				}
				break;
			}
			this.WriteTypeFullName(type);
		}

		private bool IsGenericMethodTypeParameter(TypeReference type)
		{
			MethodDefinition methodDefinition = this.commentMember as MethodDefinition;
			if (methodDefinition != null)
			{
				GenericParameter genericParameter = type as GenericParameter;
				if (genericParameter != null)
				{
					return methodDefinition.GenericParameters.Any<GenericParameter>((GenericParameter i) => i.Name == genericParameter.Name);
				}
			}
			return false;
		}

		private void WriteGenericInstanceTypeSignature(GenericInstanceType type)
		{
			if (type.ElementType.IsTypeSpecification())
			{
				throw new NotSupportedException();
			}
			DocCommentId.GenericTypeOptions genericTypeOptions = new DocCommentId.GenericTypeOptions
			{
				IsArgument = true,
				IsNestedType = type.IsNested,
				Arguments = type.GenericArguments
			};
			this.WriteTypeFullName(type.ElementType, genericTypeOptions);
		}

		private void WriteList<T>(IList<T> list, Action<T> action)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (i > 0)
				{
					this.id.Append(',');
				}
				action(list[i]);
			}
		}

		private void WriteModiferTypeSignature(IModifierType type, char id)
		{
			this.WriteTypeSignature(type.ElementType);
			this.id.Append(id);
			this.WriteTypeSignature(type.ModifierType);
		}

		private void WriteFunctionPointerTypeSignature(FunctionPointerType type)
		{
			this.id.Append("=FUNC:");
			this.WriteTypeSignature(type.ReturnType);
			if (type.HasParameters)
			{
				this.WriteParameters(type.Parameters);
			}
		}

		private void WriteArrayTypeSignature(ArrayType type)
		{
			this.WriteTypeSignature(type.ElementType);
			if (type.IsVector)
			{
				this.id.Append("[]");
				return;
			}
			this.id.Append("[");
			this.WriteList<ArrayDimension>(type.Dimensions, delegate(ArrayDimension dimension)
			{
				if (dimension.LowerBound != null)
				{
					this.id.Append(dimension.LowerBound.Value);
				}
				this.id.Append(':');
				if (dimension.UpperBound != null)
				{
					this.id.Append(dimension.UpperBound.Value - (dimension.LowerBound.GetValueOrDefault() + 1));
				}
			});
			this.id.Append("]");
		}

		private void WriteDefinition(char id, IMemberDefinition member)
		{
			this.id.Append(id).Append(':');
			this.WriteTypeFullName(member.DeclaringType);
			this.id.Append('.');
			this.WriteItemName(member.Name);
		}

		private void WriteTypeFullName(TypeReference type)
		{
			this.WriteTypeFullName(type, DocCommentId.GenericTypeOptions.Empty());
		}

		private void WriteTypeFullName(TypeReference type, DocCommentId.GenericTypeOptions options)
		{
			if (type.DeclaringType != null)
			{
				this.WriteTypeFullName(type.DeclaringType, options);
				this.id.Append('.');
			}
			if (!string.IsNullOrEmpty(type.Namespace))
			{
				this.id.Append(type.Namespace);
				this.id.Append('.');
			}
			string text = type.Name;
			if (options.IsArgument)
			{
				int num = text.LastIndexOf('`');
				if (num > 0)
				{
					text = text.Substring(0, num);
				}
			}
			this.id.Append(text);
			this.WriteGenericTypeParameters(type, options);
		}

		private void WriteGenericTypeParameters(TypeReference type, DocCommentId.GenericTypeOptions options)
		{
			if (options.IsArgument && DocCommentId.IsGenericType(type))
			{
				this.id.Append('{');
				this.WriteList<TypeReference>(this.GetGenericTypeArguments(type, options), new Action<TypeReference>(this.WriteTypeSignature));
				this.id.Append('}');
			}
		}

		private static bool IsGenericType(TypeReference type)
		{
			if (type.HasGenericParameters)
			{
				string text = string.Empty;
				int num = type.Name.LastIndexOf('`');
				if (num >= 0)
				{
					text = type.Name.Substring(0, num);
				}
				return type.Name.LastIndexOf('`') == text.Length;
			}
			return false;
		}

		private IList<TypeReference> GetGenericTypeArguments(TypeReference type, DocCommentId.GenericTypeOptions options)
		{
			if (options.IsNestedType)
			{
				int count = type.GenericParameters.Count;
				IList<TypeReference> list = options.Arguments.Skip<TypeReference>(options.ArgumentIndex).Take<TypeReference>(count).ToList<TypeReference>();
				options.ArgumentIndex += count;
				return list;
			}
			return options.Arguments;
		}

		private void WriteItemName(string name)
		{
			this.id.Append(name.Replace('.', '#').Replace('<', '{').Replace('>', '}'));
		}

		public override string ToString()
		{
			return this.id.ToString();
		}

		public static string GetDocCommentId(IMemberDefinition member)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			DocCommentId docCommentId = new DocCommentId(member);
			TokenType tokenType = member.MetadataToken.TokenType;
			if (tokenType <= TokenType.Field)
			{
				if (tokenType == TokenType.TypeDef)
				{
					docCommentId.WriteType((TypeDefinition)member);
					goto IL_00AA;
				}
				if (tokenType == TokenType.Field)
				{
					docCommentId.WriteField((FieldDefinition)member);
					goto IL_00AA;
				}
			}
			else
			{
				if (tokenType == TokenType.Method)
				{
					docCommentId.WriteMethod((MethodDefinition)member);
					goto IL_00AA;
				}
				if (tokenType == TokenType.Event)
				{
					docCommentId.WriteEvent((EventDefinition)member);
					goto IL_00AA;
				}
				if (tokenType == TokenType.Property)
				{
					docCommentId.WriteProperty((PropertyDefinition)member);
					goto IL_00AA;
				}
			}
			throw new NotSupportedException(member.FullName);
			IL_00AA:
			return docCommentId.ToString();
		}

		private IMemberDefinition commentMember;

		private StringBuilder id;

		private class GenericTypeOptions
		{
			public bool IsArgument { get; set; }

			public bool IsNestedType { get; set; }

			public IList<TypeReference> Arguments { get; set; }

			public int ArgumentIndex { get; set; }

			public static DocCommentId.GenericTypeOptions Empty()
			{
				return new DocCommentId.GenericTypeOptions();
			}
		}
	}
}
