using System;

namespace UnityEngine.Experimental.UIElements
{
	public class UxmlValueMatches : UxmlTypeRestriction
	{
		public override bool Equals(UxmlTypeRestriction other)
		{
			UxmlValueMatches uxmlValueMatches = other as UxmlValueMatches;
			return uxmlValueMatches != null && this.regex == uxmlValueMatches.regex;
		}

		public string regex;
	}
}
