using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Base class for describing an XML attribute.</para>
	/// </summary>
	public abstract class UxmlAttributeDescription
	{
		/// <summary>
		///   <para>Constructor.</para>
		/// </summary>
		public UxmlAttributeDescription()
		{
			this.use = UxmlAttributeDescription.Use.Optional;
			this.restriction = null;
		}

		/// <summary>
		///   <para>The attribute name.</para>
		/// </summary>
		public string name { get; set; }

		/// <summary>
		///   <para>Attribute type.</para>
		/// </summary>
		public string type { get; protected set; }

		/// <summary>
		///   <para>Attribute namespace.</para>
		/// </summary>
		public string typeNamespace { get; protected set; }

		/// <summary>
		///   <para>The default value for the attribute, as a string.</para>
		/// </summary>
		public abstract string defaultValueAsString { get; }

		/// <summary>
		///   <para>Whether the attribute is optional, required or prohibited.</para>
		/// </summary>
		public UxmlAttributeDescription.Use use { get; set; }

		/// <summary>
		///   <para>Restrictions on the possible values of the attribute.</para>
		/// </summary>
		public UxmlTypeRestriction restriction { get; set; }

		protected const string k_XmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema";

		/// <summary>
		///   <para>An enum to describe attribute use.</para>
		/// </summary>
		public enum Use
		{
			/// <summary>
			///   <para>There is no restriction on the use of this attribute with the element.</para>
			/// </summary>
			None,
			/// <summary>
			///   <para>The attribute is optional for the element.</para>
			/// </summary>
			Optional,
			/// <summary>
			///   <para>The attribute should not appear for the element.</para>
			/// </summary>
			Prohibited,
			/// <summary>
			///   <para>The attribute must appear in the element tag.</para>
			/// </summary>
			Required
		}
	}
}
