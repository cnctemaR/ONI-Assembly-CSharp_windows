using System;
using System.Threading;
using Mono.Collections.Generic;

namespace Mono.Cecil
{
	internal sealed class GenericParameterConstraint : ICustomAttributeProvider, IMetadataTokenProvider
	{
		public TypeReference ConstraintType
		{
			get
			{
				return this.constraint_type;
			}
			set
			{
				this.constraint_type = value;
			}
		}

		public bool HasCustomAttributes
		{
			get
			{
				if (this.custom_attributes != null)
				{
					return this.custom_attributes.Count > 0;
				}
				return this.generic_parameter != null && this.GetHasCustomAttributes(this.generic_parameter.Module);
			}
		}

		public Collection<CustomAttribute> CustomAttributes
		{
			get
			{
				if (this.generic_parameter == null)
				{
					if (this.custom_attributes == null)
					{
						Interlocked.CompareExchange<Collection<CustomAttribute>>(ref this.custom_attributes, new Collection<CustomAttribute>(), null);
					}
					return this.custom_attributes;
				}
				return this.custom_attributes ?? this.GetCustomAttributes(ref this.custom_attributes, this.generic_parameter.Module);
			}
		}

		public MetadataToken MetadataToken
		{
			get
			{
				return this.token;
			}
			set
			{
				this.token = value;
			}
		}

		internal GenericParameterConstraint(TypeReference constraintType, MetadataToken token)
		{
			this.constraint_type = constraintType;
			this.token = token;
		}

		public GenericParameterConstraint(TypeReference constraintType)
		{
			Mixin.CheckType(constraintType, Mixin.Argument.constraintType);
			this.constraint_type = constraintType;
			this.token = new MetadataToken(TokenType.GenericParamConstraint);
		}

		internal GenericParameter generic_parameter;

		internal MetadataToken token;

		private TypeReference constraint_type;

		private Collection<CustomAttribute> custom_attributes;
	}
}
