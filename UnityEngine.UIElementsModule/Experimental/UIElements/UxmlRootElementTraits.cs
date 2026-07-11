using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	public class UxmlRootElementTraits : UxmlTraits
	{
		public UxmlRootElementTraits()
		{
			base.canHaveAnyAttribute = false;
		}

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
