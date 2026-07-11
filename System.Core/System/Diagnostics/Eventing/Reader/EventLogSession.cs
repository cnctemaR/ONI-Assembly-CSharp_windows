using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public class EventLogSession : IDisposable
	{
		[SecurityCritical]
		public EventLogSession()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventLogSession(string server)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		[SecurityCritical]
		public EventLogSession(string server, string domain, string user, SecureString password, SessionAuthentication logOnType)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public static EventLogSession GlobalSession
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public void CancelCurrentOperations()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public void ClearLog(string logName)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public void ClearLog(string logName, string backupPath)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
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

		public void ExportLog(string path, PathType pathType, string query, string targetFilePath)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public void ExportLog(string path, PathType pathType, string query, string targetFilePath, bool tolerateQueryErrors)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public void ExportLogAndMessages(string path, PathType pathType, string query, string targetFilePath)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public void ExportLogAndMessages(string path, PathType pathType, string query, string targetFilePath, bool tolerateQueryErrors, CultureInfo targetCultureInfo)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventLogInformation GetLogInformation(string logName, PathType pathType)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}

		[SecurityCritical]
		public IEnumerable<string> GetLogNames()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return 0;
		}

		[SecurityCritical]
		public IEnumerable<string> GetProviderNames()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return 0;
		}
	}
}
