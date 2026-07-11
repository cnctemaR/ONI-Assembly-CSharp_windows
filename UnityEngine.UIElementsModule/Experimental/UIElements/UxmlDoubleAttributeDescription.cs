using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Describes a XML double attribute.</para>
	/// </summary>
	public class UxmlDoubleAttributeDescription : UxmlAttributeDescription
	{
		/// <summary>
		///   <para>Constructor.</para>
		/// </summary>
		public UxmlDoubleAttributeDescription()
		{
			base.type = "double";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			this.defaultValue = 0.0;
		}

		/// <summary>
		///   <para>The default value for the attribute.</para>
		/// </summary>
		public double defaultValue { get; set; }

		/// <summary>
		///   <para>The default value for the attribute, as a string.</para>
		/// </summary>
		public override string defaultValueAsString
		{
			get
			{
				return this.defaultValue.ToString();
			}
		}

		/// <summary>
		///   <para>Retrieves the value of this attribute from the attribute bag. Returns it if it is found, otherwise return defaultValue.</para>
		/// </summary>
		/// <param name="bag">The bag of attributes.</param>
		/// <returns>
		///   <para>The value of the attribute.</para>
		/// </returns>
		public double GetValueFromBag(IUxmlAttributes bag)
		{
			return bag.GetPropertyDouble(base.name, this.defaultValue);
		}
	}
}
