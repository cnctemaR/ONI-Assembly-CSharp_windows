using System;

namespace System.Diagnostics.Tracing
{
	public enum EventCommand
	{
		Update,
		SendManifest = -1,
		Enable = -2,
		Disable = -3
	}
}
