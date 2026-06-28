using System;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Assembly)]
	[Serializable]
	public sealed class DefaultDependencyAttribute : Attribute
	{
		public DefaultDependencyAttribute(LoadHint loadHintArgument)
		{
			this.hint = loadHintArgument;
		}

		public LoadHint LoadHint
		{
			get
			{
				return this.hint;
			}
		}

		private LoadHint hint;
	}
}
