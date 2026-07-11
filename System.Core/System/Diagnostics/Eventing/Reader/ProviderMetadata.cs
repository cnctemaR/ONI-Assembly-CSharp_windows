using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public class ProviderMetadata : IDisposable
	{
		public ProviderMetadata(string providerName)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public ProviderMetadata(string providerName, EventLogSession session, CultureInfo targetCultureInfo)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public string DisplayName
		{
			[SecurityCritical]
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public IEnumerable<EventMetadata> Events
		{
			[SecurityCritical]
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		public Uri HelpLink
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public Guid Id
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return default(Guid);
			}
		}

		public IList<EventKeyword> Keywords
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		public IList<EventLevel> Levels
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		public IList<EventLogLink> LogLinks
		{
			[SecurityCritical]
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		public string MessageFilePath
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public string Name
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public IList<EventOpcode> Opcodes
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		public string ParameterFilePath
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public string ResourceFilePath
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public IList<EventTask> Tasks
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return 0;
			}
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
	}
}
