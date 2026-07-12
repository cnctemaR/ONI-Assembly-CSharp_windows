using System;

namespace Klei.Actions
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ActionAttribute : Attribute
	{
		public ActionAttribute(string actionName)
		{
			this.ActionName = actionName;
		}

		public readonly string ActionName;
	}
}
