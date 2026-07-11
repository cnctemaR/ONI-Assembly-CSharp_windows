using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Describes a XML attribute representing a Color as a string.</para>
	/// </summary>
	public class UxmlColorAttributeDescription : UxmlAttributeDescription
	{
		/// <summary>
		///   <para>Constructor.</para>
		/// </summary>
		public UxmlColorAttributeDescription()
		{
			base.type = "string";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			this.defaultValue = new Color(0f, 0f, 0f, 1f);
		}

		/// <summary>
		///   <para>The default value for the attribute.</para>
		/// </summary>
		public Color defaultValue { get; set; }

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
		public Color GetValueFromBag(IUxmlAttributes bag)
		{
			return bag.GetPropertyColor(base.name, this.defaultValue);
		}
	}
}
