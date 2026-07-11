using System;

namespace UnityEngine.Experimental.UIElements
{
	public class UxmlValueBounds : UxmlTypeRestriction
	{
		public override bool Equals(UxmlTypeRestriction other)
		{
			UxmlValueBounds uxmlValueBounds = other as UxmlValueBounds;
			return uxmlValueBounds != null && (this.min == uxmlValueBounds.min && this.max == uxmlValueBounds.max && this.excludeMin == uxmlValueBounds.excludeMin) && this.excludeMax == uxmlValueBounds.excludeMax;
		}

		public string min;

		public string max;

		public bool excludeMin;

		public bool excludeMax;
	}
}
