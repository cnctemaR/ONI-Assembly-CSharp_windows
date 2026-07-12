using System;

namespace MonoMod.Utils
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	internal class DynDllImportAttribute : Attribute
	{
		public string LibraryName { get; set; }

		public string[] EntryPoints { get; set; }

		public DynDllImportAttribute(string libraryName, params string[] entryPoints)
		{
			this.LibraryName = libraryName;
			this.EntryPoints = entryPoints;
		}
	}
}
