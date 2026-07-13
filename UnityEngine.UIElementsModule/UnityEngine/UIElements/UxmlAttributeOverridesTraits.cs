using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	[Obsolete("UxmlAttributeOverridesTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public class UxmlAttributeOverridesTraits : UxmlTraits
	{
		public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
		{
			get
			{
				yield break;
			}
		}

		internal const string k_ElementNameAttributeName = "element-name";

		private UxmlStringAttributeDescription m_ElementName = new UxmlStringAttributeDescription
		{
			name = "element-name",
			use = UxmlAttributeDescription.Use.Required
		};
	}
}
