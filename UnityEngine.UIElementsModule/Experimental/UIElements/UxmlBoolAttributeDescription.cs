using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Describes a XML bool attribute.</para>
	/// </summary>
	public class UxmlBoolAttributeDescription : UxmlAttributeDescription
	{
		/// <summary>
		///   <para>Constructor.</para>
		/// </summary>
		public UxmlBoolAttributeDescription()
		{
			base.type = "boolean";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			this.defaultValue = false;
		}

		/// <summary>
		///   <para>The default value for the attribute.</para>
		/// </summary>
		public bool defaultValue { get; set; }

		/// <summary>
		///   <para>The default value for the attribute, as a string.</para>
		/// </summary>
		public override string defaultValueAsString
		{
			get
			{
				return this.defaultValue.ToString().ToLower();
			}
		}

		/// <summary>
		///   <para>Retrieves the value of this attribute from the attribute bag. Returns it if it is found, otherwise return defaultValue.</para>
		/// </summary>
		/// <param name="bag">The bag of attributes.</param>
		/// <returns>
		///   <para>The value of the attribute.</para>
		/// </returns>
		public bool GetValueFromBag(IUxmlAttributes bag)
		{
			return bag.GetPropertyBool(base.name, this.defaultValue);
		}
	}
}
