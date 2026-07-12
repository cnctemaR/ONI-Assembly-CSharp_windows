using System;
using Mono.Collections.Generic;

namespace Mono.Cecil
{
	public abstract class PropertyReference : MemberReference
	{
		public TypeReference PropertyType
		{
			get
			{
				return this.property_type;
			}
			set
			{
				this.property_type = value;
			}
		}

		public abstract Collection<ParameterDefinition> Parameters { get; }

		internal PropertyReference(string name, TypeReference propertyType)
			: base(name)
		{
			Mixin.CheckType(propertyType, Mixin.Argument.propertyType);
			this.property_type = propertyType;
		}

		protected override IMemberDefinition ResolveDefinition()
		{
			return this.Resolve();
		}

		public new abstract PropertyDefinition Resolve();

		private TypeReference property_type;
	}
}
