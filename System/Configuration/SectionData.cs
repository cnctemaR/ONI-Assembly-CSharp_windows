using System;

namespace System.Configuration
{
	internal class SectionData
	{
		public SectionData(string sectionName, string typeName, bool allowLocation, AllowDefinition allowDefinition, bool requirePermission)
		{
			this.SectionName = sectionName;
			this.TypeName = typeName;
			this.AllowLocation = allowLocation;
			this.AllowDefinition = allowDefinition;
			this.RequirePermission = requirePermission;
		}

		public readonly string SectionName;

		public readonly string TypeName;

		public readonly bool AllowLocation;

		public readonly AllowDefinition AllowDefinition;

		public string FileName;

		public readonly bool RequirePermission;
	}
}
