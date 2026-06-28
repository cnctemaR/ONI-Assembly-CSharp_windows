using System;

namespace System.IO
{
	[Flags]
	internal enum InotifyMask : uint
	{
		Access = 1U,
		Modify = 2U,
		Attrib = 4U,
		CloseWrite = 8U,
		CloseNoWrite = 16U,
		Open = 32U,
		MovedFrom = 64U,
		MovedTo = 128U,
		Create = 256U,
		Delete = 512U,
		DeleteSelf = 1024U,
		MoveSelf = 2048U,
		BaseEvents = 4095U,
		Umount = 8192U,
		Overflow = 16384U,
		Ignored = 32768U,
		OnlyDir = 16777216U,
		DontFollow = 33554432U,
		AddMask = 536870912U,
		Directory = 1073741824U,
		OneShot = 2147483648U
	}
}
