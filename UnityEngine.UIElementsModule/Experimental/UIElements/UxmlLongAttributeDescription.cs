using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Describes a XML long attribute.</para>
	/// </summary>
	public class UxmlLongAttributeDescription : UxmlAttributeDescription
	{
		/// <summary>
		///   <para>Constructor.</para>
		/// </summary>
		public UxmlLongAttributeDescription()
		{
			base.type = "long";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			this.defaultValue = 0L;
		}

		/// <summary>
		///   <para>The default value for the attribute.</para>
		/// </summary>
		public long defaultValue { get; set; }

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
		public long GetValueFromBag(IUxmlAttributes bag)
		{
			return bag.GetPropertyLong(base.name, this.defaultValue);
		}
	}
}
