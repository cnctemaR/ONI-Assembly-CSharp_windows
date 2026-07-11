using System;

namespace YamlDotNet.Samples.Helpers
{
	internal class SampleAttribute : Attribute
	{
		public string DisplayName { get; private set; }

		public string Title
		{
			get
			{
				return this.title;
			}
			set
			{
				this.title = value;
				this.DisplayName = "Sample: " + value;
			}
		}

		public string Description { get; set; }

		private string title;
	}
}
