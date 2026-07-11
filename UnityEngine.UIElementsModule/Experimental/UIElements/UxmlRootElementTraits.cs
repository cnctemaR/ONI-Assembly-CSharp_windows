using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>UxmlTraits for the UXML root element.</para>
	/// </summary>
	public class UxmlRootElementTraits : UxmlTraits
	{
		/// <summary>
		///   <para>Constructor.</para>
		/// </summary>
		public UxmlRootElementTraits()
		{
			base.canHaveAnyAttribute = false;
		}

		/// <summary>
		///   <para>Returns an enumerable containing UxmlChildElementDescription(typeof(VisualElement)), since the root element can contain VisualElements.</para>
		/// </summary>
		public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
		{
			get
			{
				return new UxmlChildElementDescription[]
				{
					new UxmlChildElementDescription(typeof(VisualElement))
				};
			}
		}
	}
}
