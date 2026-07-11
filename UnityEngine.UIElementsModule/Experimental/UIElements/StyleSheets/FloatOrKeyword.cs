using System;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	internal struct FloatOrKeyword
	{
		public FloatOrKeyword(StyleValueKeyword kw)
		{
			this.isKeyword = true;
			this.keyword = kw;
			this.floatValue = 0f;
		}

		public FloatOrKeyword(float v)
		{
			this.isKeyword = false;
			this.keyword = StyleValueKeyword.Inherit;
			this.floatValue = v;
		}

		public bool isKeyword { get; private set; }

		public StyleValueKeyword keyword { get; private set; }

		public float floatValue { get; private set; }
	}
}
