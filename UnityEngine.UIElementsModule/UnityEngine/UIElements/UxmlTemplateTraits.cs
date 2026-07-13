using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	[Obsolete("UxmlTemplateTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public class UxmlTemplateTraits : UxmlTraits
	{
		public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
		{
			get
			{
				yield break;
			}
		}

		private UxmlStringAttributeDescription m_Name = new UxmlStringAttributeDescription
		{
			name = "name",
			use = UxmlAttributeDescription.Use.Required
		};

		private UxmlStringAttributeDescription m_Path = new UxmlStringAttributeDescription
		{
			name = "path"
		};

		private UxmlStringAttributeDescription m_Src = new UxmlStringAttributeDescription
		{
			name = "src"
		};
	}
}
