using System;

namespace UnityEngine.UIElements.StyleSheets
{
	internal struct MatchResult
	{
		public bool success
		{
			get
			{
				return this.errorCode == MatchResultErrorCode.None;
			}
		}

		public MatchResultErrorCode errorCode;

		public string errorValue;
	}
}
