using System;
using System.IO;
using System.Security;

namespace System.Configuration.Internal
{
	public class DelegatingConfigHost : IInternalConfigHost
	{
		protected DelegatingConfigHost()
		{
		}

		protected IInternalConfigHost Host
		{
			get
			{
				return this.host;
			}
			set
			{
				this.host = value;
			}
		}

		public virtual object CreateConfigurationContext(string configPath, string locationSubPath)
		{
			return this.host.CreateConfigurationContext(configPath, locationSubPath);
		}

		public virtual object CreateDeprecatedConfigContext(string configPath)
		{
			return this.host.CreateDeprecatedConfigContext(configPath);
		}

		public virtual string DecryptSection(string encryptedXml, ProtectedConfigurationProvider protectionProvider, ProtectedConfigurationSection protectedConfigSection)
		{
			return this.host.DecryptSection(encryptedXml, protectionProvider, protectedConfigSection);
		}

		public virtual void DeleteStream(string streamName)
		{
			this.host.DeleteStream(streamName);
		}

		public virtual string EncryptSection(string clearTextXml, ProtectedConfigurationProvider protectionProvider, ProtectedConfigurationSection protectedConfigSection)
		{
			return this.host.EncryptSection(clearTextXml, protectionProvider, protectedConfigSection);
		}

		public virtual string GetConfigPathFromLocationSubPath(string configPath, string locationSubPath)
		{
			return this.host.GetConfigPathFromLocationSubPath(configPath, locationSubPath);
		}

		public virtual Type GetConfigType(string typeName, bool throwOnError)
		{
			return this.host.GetConfigType(typeName, throwOnError);
		}

		public virtual string GetConfigTypeName(Type t)
		{
			return this.host.GetConfigTypeName(t);
		}

		public virtual void GetRestrictedPermissions(IInternalConfigRecord configRecord, out PermissionSet permissionSet, out bool isHostReady)
		{
			this.host.GetRestrictedPermissions(configRecord, out permissionSet, out isHostReady);
		}

		public virtual string GetStreamName(string configPath)
		{
			return this.host.GetStreamName(configPath);
		}

		public virtual string GetStreamNameForConfigSource(string streamName, string configSource)
		{
			return this.host.GetStreamNameForConfigSource(streamName, configSource);
		}

		public virtual object GetStreamVersion(string streamName)
		{
			return this.host.GetStreamVersion(streamName);
		}

		public virtual IDisposable Impersonate()
		{
			return this.host.Impersonate();
		}

		public virtual void Init(IInternalConfigRoot configRoot, params object[] hostInitParams)
		{
			this.host.Init(configRoot, hostInitParams);
		}

		public virtual void InitForConfiguration(ref string locationSubPath, out string configPath, out string locationConfigPath, IInternalConfigRoot configRoot, params object[] hostInitConfigurationParams)
		{
			this.host.InitForConfiguration(ref locationSubPath, out configPath, out locationConfigPath, configRoot, hostInitConfigurationParams);
		}

		public virtual bool IsAboveApplication(string configPath)
		{
			return this.host.IsAboveApplication(configPath);
		}

		public virtual bool IsConfigRecordRequired(string configPath)
		{
			return this.host.IsConfigRecordRequired(configPath);
		}

		public virtual bool IsDefinitionAllowed(string configPath, ConfigurationAllowDefinition allowDefinition, ConfigurationAllowExeDefinition allowExeDefinition)
		{
			return this.host.IsDefinitionAllowed(configPath, allowDefinition, allowExeDefinition);
		}

		public virtual bool IsInitDelayed(IInternalConfigRecord configRecord)
		{
			return this.host.IsInitDelayed(configRecord);
		}

		public virtual bool IsFile(string streamName)
		{
			return this.host.IsFile(streamName);
		}

		public virtual bool IsFullTrustSectionWithoutAptcaAllowed(IInternalConfigRecord configRecord)
		{
			return this.host.IsFullTrustSectionWithoutAptcaAllowed(configRecord);
		}

		public virtual bool IsLocationApplicable(string configPath)
		{
			return this.host.IsLocationApplicable(configPath);
		}

		public virtual bool IsRemote
		{
			get
			{
				return this.host.IsRemote;
			}
		}

		public virtual bool IsSecondaryRoot(string configPath)
		{
			return this.host.IsSecondaryRoot(configPath);
		}

		public virtual bool IsTrustedConfigPath(string configPath)
		{
			return this.host.IsTrustedConfigPath(configPath);
		}

		public virtual Stream OpenStreamForRead(string streamName)
		{
			return this.host.OpenStreamForRead(streamName);
		}

		public virtual Stream OpenStreamForRead(string streamName, bool assertPermissions)
		{
			return this.host.OpenStreamForRead(streamName, assertPermissions);
		}

		public virtual Stream OpenStreamForWrite(string streamName, string templateStreamName, ref object writeContext)
		{
			return this.host.OpenStreamForWrite(streamName, templateStreamName, ref writeContext);
		}

		public virtual Stream OpenStreamForWrite(string streamName, string templateStreamName, ref object writeContext, bool assertPermissions)
		{
			return this.host.OpenStreamForWrite(streamName, templateStreamName, ref writeContext, assertPermissions);
		}

		public virtual bool PrefetchAll(string configPath, string streamName)
		{
			return this.host.PrefetchAll(configPath, streamName);
		}

		public virtual bool PrefetchSection(string sectionGroupName, string sectionName)
		{
			return this.host.PrefetchSection(sectionGroupName, sectionName);
		}

		public virtual void RequireCompleteInit(IInternalConfigRecord configRecord)
		{
			this.host.RequireCompleteInit(configRecord);
		}

		public virtual object StartMonitoringStreamForChanges(string streamName, StreamChangeCallback callback)
		{
			return this.host.StartMonitoringStreamForChanges(streamName, callback);
		}

		public virtual void StopMonitoringStreamForChanges(string streamName, StreamChangeCallback callback)
		{
			this.host.StopMonitoringStreamForChanges(streamName, callback);
		}

		public virtual void VerifyDefinitionAllowed(string configPath, ConfigurationAllowDefinition allowDefinition, ConfigurationAllowExeDefinition allowExeDefinition, IConfigErrorInfo errorInfo)
		{
			this.host.VerifyDefinitionAllowed(configPath, allowDefinition, allowExeDefinition, errorInfo);
		}

		public virtual void WriteCompleted(string streamName, bool success, object writeContext)
		{
			this.host.WriteCompleted(streamName, success, writeContext);
		}

		public virtual void WriteCompleted(string streamName, bool success, object writeContext, bool assertPermissions)
		{
			this.host.WriteCompleted(streamName, success, writeContext, assertPermissions);
		}

		public virtual bool SupportsChangeNotifications
		{
			get
			{
				return this.host.SupportsChangeNotifications;
			}
		}

		public virtual bool SupportsLocation
		{
			get
			{
				return this.host.SupportsLocation;
			}
		}

		public virtual bool SupportsPath
		{
			get
			{
				return this.host.SupportsPath;
			}
		}

		public virtual bool SupportsRefresh
		{
			get
			{
				return this.host.SupportsRefresh;
			}
		}

		private IInternalConfigHost host;
	}
}
