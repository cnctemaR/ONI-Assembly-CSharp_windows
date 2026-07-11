using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Describes a VisualElement derived class for the parsing of UXML files and the generation of UXML schema definition.</para>
	/// </summary>
	public abstract class UxmlTraits
	{
		protected UxmlTraits()
		{
			this.canHaveAnyAttribute = true;
		}

		/// <summary>
		///   <para>Must return true if the UXML element attributes are not restricted to the values enumerated by UxmlTraits.uxmlAttributesDescription.</para>
		/// </summary>
		public bool canHaveAnyAttribute { get; protected set; }

		/// <summary>
		///   <para>Describes the UXML attributes expected by the element. The attributes enumerated here will appear in the UXML schema.</para>
		/// </summary>
		public virtual IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription
		{
			get
			{
				yield break;
			}
		}

		/// <summary>
		///   <para>Describes the types of element that can appear as children of this element in a UXML file.</para>
		/// </summary>
		public virtual IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
		{
			get
			{
				yield break;
			}
		}

		/// <summary>
		///   <para>Initialize a VisualElement instance with values from the UXML element attributes.</para>
		/// </summary>
		/// <param name="ve">The VisualElement to initialize.</param>
		/// <param name="bag">A bag of name-value pairs, one for each attribute of the UXML element.</param>
		/// <param name="cc">When the element is created as part of a template instance inserted in another document, this contains information about the insertion point.</param>
		public virtual void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
		}
	}
}
