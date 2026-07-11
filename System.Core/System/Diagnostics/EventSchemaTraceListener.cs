using System;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics
{
	[HostProtection(SecurityAction.LinkDemand, Synchronization = true)]
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public class EventSchemaTraceListener : TextWriterTraceListener
	{
		public EventSchemaTraceListener(string fileName)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventSchemaTraceListener(string fileName, string name)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventSchemaTraceListener(string fileName, string name, int bufferSize)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventSchemaTraceListener(string fileName, string name, int bufferSize, TraceLogRetentionOption logRetentionOption)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventSchemaTraceListener(string fileName, string name, int bufferSize, TraceLogRetentionOption logRetentionOption, long maximumFileSize)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventSchemaTraceListener(string fileName, string name, int bufferSize, TraceLogRetentionOption logRetentionOption, long maximumFileSize, int maximumNumberOfFiles)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public int BufferSize
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		public long MaximumFileSize
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0L;
			}
		}

		public int MaximumNumberOfFiles
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		public TraceLogRetentionOption TraceLogRetentionOption
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return TraceLogRetentionOption.UnlimitedSequentialFiles;
			}
		}
	}
}
