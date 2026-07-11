using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices.Internal
{
	[Guid("d8013ff0-730b-45e2-ba24-874b7242c425")]
	public interface IComSoapMetadata
	{
		[DispId(1)]
		[return: MarshalAs(UnmanagedType.BStr)]
		string Generate([MarshalAs(UnmanagedType.BStr)] string SrcTypeLibFileName, [MarshalAs(UnmanagedType.BStr)] string OutPath);

		[DispId(2)]
		[return: MarshalAs(UnmanagedType.BStr)]
		string GenerateSigned([MarshalAs(UnmanagedType.BStr)] string SrcTypeLibFileName, [MarshalAs(UnmanagedType.BStr)] string OutPath, [MarshalAs(UnmanagedType.Bool)] bool InstallGac, [MarshalAs(UnmanagedType.BStr)] out string Error);
	}
}
