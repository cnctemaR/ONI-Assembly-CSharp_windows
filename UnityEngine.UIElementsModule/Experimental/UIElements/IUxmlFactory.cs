using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	public interface IUxmlFactory
	{
		/// <summary>
		///   <para>The name of the UXML element read by the factory.</para>
		/// </summary>
		string uxmlName { get; }

		/// <summary>
		///   <para>The namespace of the UXML element read by the factory.</para>
		/// </summary>
		string uxmlNamespace { get; }

		/// <summary>
		///   <para>The fully qualified name of the UXML element read by the factory.</para>
		/// </summary>
		string uxmlQualifiedName { get; }

		/// <summary>
		///   <para>Must return true if the UXML element attributes are not restricted to the values enumerated by uxmlAttributesDescription.</para>
		/// </summary>
		bool canHaveAnyAttribute { get; }

		/// <summary>
		///   <para>Describes the UXML attributes expected by the element. The attributes enumerated here will appear in the UXML schema.</para>
		/// </summary>
		IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription { get; }

		/// <summary>
		///   <para>Describes the types of element that can appear as children of this element in a UXML file.</para>
		/// </summary>
		IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription { get; }

		/// <summary>
		///   <para>The type of element for which this element type can substitute for.</para>
		/// </summary>
		string substituteForTypeName { get; }

		/// <summary>
		///   <para>The UXML namespace for the type returned by substituteForTypeName.</para>
		/// </summary>
		string substituteForTypeNamespace { get; }

		/// <summary>
		///   <para>The fully qualified XML name for the type returned by substituteForTypeName.</para>
		/// </summary>
		string substituteForTypeQualifiedName { get; }

		/// <summary>
		///   <para>Returns true if the factory accepts the content of the attribute bag.</para>
		/// </summary>
		/// <param name="bag">The attribute bag.</param>
		/// <returns>
		///   <para>True if the factory accepts the content of the attribute bag. False otherwise.</para>
		/// </returns>
		bool AcceptsAttributeBag(IUxmlAttributes bag);

		/// <summary>
		///   <para>Instanciate and initialize an object of type T0.</para>
		/// </summary>
		/// <param name="bag">A bag of name-value pairs, one for each attribute of the UXML element. This can be used to initialize the properties of the created object.</param>
		/// <param name="cc">When the element is created as part of a template instance inserted in another document, this contains information about the insertion point.</param>
		/// <returns>
		///   <para>The created object.</para>
		/// </returns>
		VisualElement Create(IUxmlAttributes bag, CreationContext cc);

		[Obsolete("Use uxmlName and uxmlNamespace instead.")]
		Type CreatesType { get; }
	}
}
