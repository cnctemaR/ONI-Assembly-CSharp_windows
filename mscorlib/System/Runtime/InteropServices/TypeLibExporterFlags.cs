using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Flags]
	[Serializable]
	public enum TypeLibExporterFlags
	{
		OnlyReferenceRegistered = 1,
		None = 0,
		CallerResolvedReferences = 2,
		OldNames = 4,
		ExportAs32Bit = 16,
		ExportAs64Bit = 32
	}
}
