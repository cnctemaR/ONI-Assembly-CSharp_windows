using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Describes a XML string attribute.</para>
	/// </summary>
	public class UxmlStringAttributeDescription : UxmlAttributeDescription
	{
		/// <summary>
		///   <para>Constructor.</para>
		/// </summary>
		public UxmlStringAttributeDescription()
		{
			base.type = "string";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			this.defaultValue = "";
		}

		/// <summary>
		///   <para>The default value for the attribute.</para>
		/// </summary>
		public string defaultValue { get; set; }

		/// <summary>
		///   <para>The default value for the attribute, as a string.</para>
		/// </summary>
		public override string defaultValueAsString
		{
			get
			{
				return this.defaultValue;
			}
		}

		/// <summary>
		///   <para>Retrieves the value of this attribute from the attribute bag. Returns it if it is found, otherwise return defaultValue.</para>
		/// </summary>
		/// <param name="bag">The bag of attributes.</param>
		/// <returns>
		///   <para>The value of the attribute.</para>
		/// </returns>
		public string GetValueFromBag(IUxmlAttributes bag)
		{
			return bag.GetPropertyString(base.name, this.defaultValue);
		}
	}
}
