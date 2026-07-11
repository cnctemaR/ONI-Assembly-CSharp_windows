using System;

namespace UnityEngine.Experimental.UIElements
{
	public interface IUxmlAttributes
	{
		string GetPropertyString(string propertyName);

		/// <summary>
		///   <para>Return the value of an attribute as a string, or the defaultValue if the property is not found.</para>
		/// </summary>
		/// <param name="propertyName">Attribute name.</param>
		/// <param name="defaultValue">Default value if the property is not found.</param>
		/// <returns>
		///   <para>The attribute value or the default value if not found.</para>
		/// </returns>
		string GetPropertyString(string propertyName, string defaultValue);

		/// <summary>
		///   <para>Return the value of an attribute as a float, or the defaultValue if the property is not found.</para>
		/// </summary>
		/// <param name="propertyName">Attribute name.</param>
		/// <param name="defaultValue">Default value if the property is not found.</param>
		/// <returns>
		///   <para>The attribute value or the default value if not found.</para>
		/// </returns>
		float GetPropertyFloat(string propertyName, float defaultValue);

		/// <summary>
		///   <para>Return the value of an attribute as a double, or the defaultValue if the property is not found.</para>
		/// </summary>
		/// <param name="defaultValue">Default value if the property is not found.</param>
		/// <param name="propertyName">AttributeName.</param>
		/// <returns>
		///   <para>The attribute value or the default value if not found.</para>
		/// </returns>
		double GetPropertyDouble(string propertyName, double defaultValue);

		/// <summary>
		///   <para>Return the value of an attribute as an int, or the defaultValue if the property is not found.</para>
		/// </summary>
		/// <param name="propertyName">Attribute name.</param>
		/// <param name="defaultValue">Default value if the property is not found.</param>
		/// <returns>
		///   <para>The attribute value or the default value if not found.</para>
		/// </returns>
		int GetPropertyInt(string propertyName, int defaultValue);

		/// <summary>
		///   <para>Return the value of an attribute as a long, or the defaultValue if the property is not found.</para>
		/// </summary>
		/// <param name="propertyName">Attribute name.</param>
		/// <param name="defaultValue">Default value if the property is not found.</param>
		/// <returns>
		///   <para>The attribute value or the default value if not found.</para>
		/// </returns>
		long GetPropertyLong(string propertyName, long defaultValue);

		/// <summary>
		///   <para>Return the value of an attribute as a bool, or the defaultValue if the property is not found.</para>
		/// </summary>
		/// <param name="propertyName">Attribute name.</param>
		/// <param name="defaultValue">Default value if the property is not found.</param>
		/// <returns>
		///   <para>The attribute value or the default value if not found.</para>
		/// </returns>
		bool GetPropertyBool(string propertyName, bool defaultValue);

		/// <summary>
		///   <para>Return the value of an attribute as a Color, or the defaultValue if the property is not found.</para>
		/// </summary>
		/// <param name="propertyName">Attribute name.</param>
		/// <param name="defaultValue">Default value if the property is not found.</param>
		/// <returns>
		///   <para>The attribute value or the default value if not found.</para>
		/// </returns>
		Color GetPropertyColor(string propertyName, Color defaultValue);

		T GetPropertyEnum<T>(string propertyName, T defaultValue);
	}
}
