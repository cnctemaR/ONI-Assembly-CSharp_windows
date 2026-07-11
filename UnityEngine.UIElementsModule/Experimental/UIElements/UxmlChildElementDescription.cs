using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Describe an allowed child element for an element.</para>
	/// </summary>
	public class UxmlChildElementDescription
	{
		/// <summary>
		///   <para>Constructor.</para>
		/// </summary>
		/// <param name="t"></param>
		public UxmlChildElementDescription(Type t)
		{
			this.elementName = t.Name;
			this.elementNamespace = t.Namespace;
		}

		/// <summary>
		///   <para>The name of the allowed child element.</para>
		/// </summary>
		public string elementName { get; protected set; }

		/// <summary>
		///   <para>The namespace name of the allowed child element.</para>
		/// </summary>
		public string elementNamespace { get; protected set; }
	}
}
