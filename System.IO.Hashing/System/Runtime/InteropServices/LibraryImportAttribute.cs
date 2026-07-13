using System;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	internal sealed class LibraryImportAttribute : Attribute
	{
		public LibraryImportAttribute(string libraryName)
		{
			this.LibraryName = libraryName;
		}

		public string LibraryName { get; }

		public string EntryPoint { get; set; }

		public StringMarshalling StringMarshalling { get; set; }

		public Type StringMarshallingCustomType { get; set; }

		public bool SetLastError { get; set; }
	}
}
