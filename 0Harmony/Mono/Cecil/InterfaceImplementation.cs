using System;
using System.Threading;
using Mono.Collections.Generic;

namespace Mono.Cecil
{
	internal sealed class InterfaceImplementation : ICustomAttributeProvider, IMetadataTokenProvider
	{
		public TypeReference InterfaceType
		{
			get
			{
				return this.interface_type;
			}
			set
			{
				this.interface_type = value;
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
				return this.type != null && this.GetHasCustomAttributes(this.type.Module);
			}
		}

		public Collection<CustomAttribute> CustomAttributes
		{
			get
			{
				if (this.type == null)
				{
					if (this.custom_attributes == null)
					{
						Interlocked.CompareExchange<Collection<CustomAttribute>>(ref this.custom_attributes, new Collection<CustomAttribute>(), null);
					}
					return this.custom_attributes;
				}
				return this.custom_attributes ?? this.GetCustomAttributes(ref this.custom_attributes, this.type.Module);
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

		internal InterfaceImplementation(TypeReference interfaceType, MetadataToken token)
		{
			this.interface_type = interfaceType;
			this.token = token;
		}

		public InterfaceImplementation(TypeReference interfaceType)
		{
			Mixin.CheckType(interfaceType, Mixin.Argument.interfaceType);
			this.interface_type = interfaceType;
			this.token = new MetadataToken(TokenType.InterfaceImpl);
		}

		internal TypeDefinition type;

		internal MetadataToken token;

		private TypeReference interface_type;

		private Collection<CustomAttribute> custom_attributes;
	}
}
