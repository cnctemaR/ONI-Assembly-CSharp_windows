using System;
using System.ComponentModel;

namespace System.Diagnostics
{
	public enum TraceEventType
	{
		Critical = 1,
		Error,
		Warning = 4,
		Information = 8,
		Verbose = 16,
		[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		Start = 256,
		[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		Stop = 512,
		[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		Suspend = 1024,
		[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		Resume = 2048,
		[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		Transfer = 4096
	}
}
