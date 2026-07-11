using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Factory for the root UXML element.</para>
	/// </summary>
	public class UxmlRootElementFactory : UxmlFactory<VisualElement, UxmlRootElementTraits>
	{
		/// <summary>
		///   <para>Returns "UXML".</para>
		/// </summary>
		public override string uxmlName
		{
			get
			{
				return "UXML";
			}
		}

		/// <summary>
		///   <para>Returns the qualified name for this element.</para>
		/// </summary>
		public override string uxmlQualifiedName
		{
			get
			{
				return this.uxmlNamespace + "." + this.uxmlName;
			}
		}

		/// <summary>
		///   <para>Returns the empty string, as the root element can not appear anywhere else bit at the root of the document.</para>
		/// </summary>
		public override string substituteForTypeName
		{
			get
			{
				return string.Empty;
			}
		}

		/// <summary>
		///   <para>Returns the empty string, as the root element can not appear anywhere else bit at the root of the document.</para>
		/// </summary>
		public override string substituteForTypeNamespace
		{
			get
			{
				return string.Empty;
			}
		}

		/// <summary>
		///   <para>Returns the empty string, as the root element can not appear anywhere else bit at the root of the document.</para>
		/// </summary>
		public override string substituteForTypeQualifiedName
		{
			get
			{
				return string.Empty;
			}
		}

		/// <summary>
		///   <para>Returns null.</para>
		/// </summary>
		/// <param name="bag"></param>
		/// <param name="cc"></param>
		public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
		{
			return null;
		}
	}
}
