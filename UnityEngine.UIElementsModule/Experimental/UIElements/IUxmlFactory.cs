using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	public interface IUxmlFactory
	{
		string uxmlName { get; }

		string uxmlNamespace { get; }

		string uxmlQualifiedName { get; }

		bool canHaveAnyAttribute { get; }

		IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription { get; }

		IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription { get; }

		string substituteForTypeName { get; }

		string substituteForTypeNamespace { get; }

		string substituteForTypeQualifiedName { get; }

		bool AcceptsAttributeBag(IUxmlAttributes bag, CreationContext cc);

		VisualElement Create(IUxmlAttributes bag, CreationContext cc);

		[Obsolete("Use uxmlName and uxmlNamespace instead.")]
		Type CreatesType { get; }
	}
}
