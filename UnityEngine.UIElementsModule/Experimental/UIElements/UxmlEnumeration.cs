using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.Experimental.UIElements
{
	public class UxmlEnumeration : UxmlTypeRestriction
	{
		public override bool Equals(UxmlTypeRestriction other)
		{
			UxmlEnumeration uxmlEnumeration = other as UxmlEnumeration;
			return uxmlEnumeration != null && this.values.All<string>(new Func<string, bool>(uxmlEnumeration.values.Contains)) && this.values.Count == uxmlEnumeration.values.Count;
		}

		public List<string> values = new List<string>();
	}
}
