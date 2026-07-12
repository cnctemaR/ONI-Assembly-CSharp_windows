using System;
using System.Collections.Generic;
using Mono.Collections.Generic;

namespace Mono.Cecil
{
	internal sealed class TypeDefinitionProjection
	{
		public TypeDefinitionProjection(TypeDefinition type, TypeDefinitionTreatment treatment, Collection<MethodDefinition> redirectedMethods, Collection<KeyValuePair<InterfaceImplementation, InterfaceImplementation>> redirectedInterfaces)
		{
			this.Attributes = type.Attributes;
			this.Name = type.Name;
			this.Treatment = treatment;
			this.RedirectedMethods = redirectedMethods;
			this.RedirectedInterfaces = redirectedInterfaces;
		}

		public readonly TypeAttributes Attributes;

		public readonly string Name;

		public readonly TypeDefinitionTreatment Treatment;

		public readonly Collection<MethodDefinition> RedirectedMethods;

		public readonly Collection<KeyValuePair<InterfaceImplementation, InterfaceImplementation>> RedirectedInterfaces;
	}
}
