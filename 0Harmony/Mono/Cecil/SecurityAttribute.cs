using System;
using System.Diagnostics;
using System.Threading;
using Mono.Collections.Generic;

namespace Mono.Cecil
{
	[DebuggerDisplay("{AttributeType}")]
	internal sealed class SecurityAttribute : ICustomAttribute
	{
		public TypeReference AttributeType
		{
			get
			{
				return this.attribute_type;
			}
			set
			{
				this.attribute_type = value;
			}
		}

		public bool HasFields
		{
			get
			{
				return !this.fields.IsNullOrEmpty<CustomAttributeNamedArgument>();
			}
		}

		public Collection<CustomAttributeNamedArgument> Fields
		{
			get
			{
				if (this.fields == null)
				{
					Interlocked.CompareExchange<Collection<CustomAttributeNamedArgument>>(ref this.fields, new Collection<CustomAttributeNamedArgument>(), null);
				}
				return this.fields;
			}
		}

		public bool HasProperties
		{
			get
			{
				return !this.properties.IsNullOrEmpty<CustomAttributeNamedArgument>();
			}
		}

		public Collection<CustomAttributeNamedArgument> Properties
		{
			get
			{
				if (this.properties == null)
				{
					Interlocked.CompareExchange<Collection<CustomAttributeNamedArgument>>(ref this.properties, new Collection<CustomAttributeNamedArgument>(), null);
				}
				return this.properties;
			}
		}

		public SecurityAttribute(TypeReference attributeType)
		{
			this.attribute_type = attributeType;
		}

		bool ICustomAttribute.HasConstructorArguments
		{
			get
			{
				return false;
			}
		}

		Collection<CustomAttributeArgument> ICustomAttribute.ConstructorArguments
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		private TypeReference attribute_type;

		internal Collection<CustomAttributeNamedArgument> fields;

		internal Collection<CustomAttributeNamedArgument> properties;
	}
}
