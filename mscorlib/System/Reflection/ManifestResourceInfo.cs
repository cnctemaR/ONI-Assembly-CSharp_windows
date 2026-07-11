using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	public class ManifestResourceInfo
	{
		internal ManifestResourceInfo()
		{
		}

		internal ManifestResourceInfo(Assembly assembly, string filename, ResourceLocation location)
		{
			this._assembly = assembly;
			this._filename = filename;
			this._location = location;
		}

		public virtual string FileName
		{
			get
			{
				return this._filename;
			}
		}

		public virtual Assembly ReferencedAssembly
		{
			get
			{
				return this._assembly;
			}
		}

		public virtual ResourceLocation ResourceLocation
		{
			get
			{
				return this._location;
			}
		}

		private Assembly _assembly;

		private string _filename;

		private ResourceLocation _location;
	}
}
