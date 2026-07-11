using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	public class ManifestResourceInfo
	{
		public ManifestResourceInfo(Assembly containingAssembly, string containingFileName, ResourceLocation resourceLocation)
		{
			this._containingAssembly = containingAssembly;
			this._containingFileName = containingFileName;
			this._resourceLocation = resourceLocation;
		}

		public virtual Assembly ReferencedAssembly
		{
			get
			{
				return this._containingAssembly;
			}
		}

		public virtual string FileName
		{
			get
			{
				return this._containingFileName;
			}
		}

		public virtual ResourceLocation ResourceLocation
		{
			get
			{
				return this._resourceLocation;
			}
		}

		private Assembly _containingAssembly;

		private string _containingFileName;

		private ResourceLocation _resourceLocation;
	}
}
