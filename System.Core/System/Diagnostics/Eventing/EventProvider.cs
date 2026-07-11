using System;
using System.Security;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public class EventProvider : IDisposable
	{
		[SecuritySafeCritical]
		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		public EventProvider(Guid providerGuid)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public virtual void Close()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		[SecurityCritical]
		public static Guid CreateActivityId()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return default(Guid);
		}

		public void Dispose()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		[SecuritySafeCritical]
		protected virtual void Dispose(bool disposing)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public static EventProvider.WriteEventErrorCode GetLastWriteEventError()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return EventProvider.WriteEventErrorCode.NoError;
		}

		public bool IsEnabled()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		public bool IsEnabled(byte level, long keywords)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		[SecurityCritical]
		public static void SetActivityId(ref Guid id)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		[SecurityCritical]
		protected bool WriteEvent(ref EventDescriptor eventDescriptor, int dataCount, IntPtr data)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		public bool WriteEvent(ref EventDescriptor eventDescriptor, object[] eventPayload)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		[SecurityCritical]
		public bool WriteEvent(ref EventDescriptor eventDescriptor, string data)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		public bool WriteMessageEvent(string eventMessage)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		[SecurityCritical]
		public bool WriteMessageEvent(string eventMessage, byte eventLevel, long eventKeywords)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		[SecurityCritical]
		protected bool WriteTransferEvent(ref EventDescriptor eventDescriptor, Guid relatedActivityId, int dataCount, IntPtr data)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		[SecurityCritical]
		public bool WriteTransferEvent(ref EventDescriptor eventDescriptor, Guid relatedActivityId, object[] eventPayload)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		public enum WriteEventErrorCode
		{
			EventTooBig = 2,
			NoError = 0,
			NoFreeBuffers
		}
	}
}
