using System;

namespace System.Reflection
{
	public class ManifestResourceInfo
	{
		public ManifestResourceInfo(Assembly containingAssembly, string containingFileName, ResourceLocation resourceLocation)
		{
			this.ReferencedAssembly = containingAssembly;
			this.FileName = containingFileName;
			this.ResourceLocation = resourceLocation;
		}

		public virtual Assembly ReferencedAssembly { get; }

		public virtual string FileName { get; }

		public virtual ResourceLocation ResourceLocation { get; }
	}
}
