using System;

namespace UnityEngine.UIElements.StyleSheets
{
	internal struct StyleValidationResult
	{
		public bool success
		{
			get
			{
				return this.status == StyleValidationStatus.Ok;
			}
		}

		public StyleValidationStatus status;

		public string message;

		public string errorValue;

		public string hint;
	}
}
