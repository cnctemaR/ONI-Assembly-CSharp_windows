using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	[Obsolete("UxmlRootElementTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public class UxmlRootElementTraits : UxmlTraits
	{
		public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
		{
			get
			{
				yield return new UxmlChildElementDescription(typeof(VisualElement));
				yield break;
			}
		}

		protected UxmlStringAttributeDescription m_Name = new UxmlStringAttributeDescription
		{
			name = "name"
		};

		private UxmlStringAttributeDescription m_Class = new UxmlStringAttributeDescription
		{
			name = "class"
		};
	}
}
