using System;

namespace MonoMod.Utils
{
	internal sealed class DynDllMapping
	{
		public string LibraryName { get; set; }

		public int? Flags { get; set; }

		public DynDllMapping(string libraryName, int? flags = null)
		{
			if (libraryName == null)
			{
				throw new ArgumentNullException("libraryName");
			}
			this.LibraryName = libraryName;
			this.Flags = flags;
		}

		public static implicit operator DynDllMapping(string libraryName)
		{
			return new DynDllMapping(libraryName, null);
		}
	}
}
