using System;
using System.Configuration.Internal;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace System.Configuration
{
	internal abstract class InternalConfigurationHost : IInternalConfigHost
	{
		public virtual object CreateConfigurationContext(string configPath, string locationSubPath)
		{
			return null;
		}

		public virtual object CreateDeprecatedConfigContext(string configPath)
		{
			return null;
		}

		public virtual void DeleteStream(string streamName)
		{
			File.Delete(streamName);
		}

		string IInternalConfigHost.DecryptSection(string encryptedXml, ProtectedConfigurationProvider protectionProvider, ProtectedConfigurationSection protectedSection)
		{
			return protectedSection.DecryptSection(encryptedXml, protectionProvider);
		}

		string IInternalConfigHost.EncryptSection(string clearXml, ProtectedConfigurationProvider protectionProvider, ProtectedConfigurationSection protectedSection)
		{
			return protectedSection.EncryptSection(clearXml, protectionProvider);
		}

		public virtual string GetConfigPathFromLocationSubPath(string configPath, string locationSubPath)
		{
			return configPath;
		}

		public virtual Type GetConfigType(string typeName, bool throwOnError)
		{
			Type type = Type.GetType(typeName);
			if (type == null)
			{
				type = Type.GetType(typeName + ",System");
			}
			if (type == null && throwOnError)
			{
				throw new ConfigurationErrorsException("Type '" + typeName + "' not found.");
			}
			return type;
		}

		public virtual string GetConfigTypeName(Type t)
		{
			return t.AssemblyQualifiedName;
		}

		public virtual void GetRestrictedPermissions(IInternalConfigRecord configRecord, out PermissionSet permissionSet, out bool isHostReady)
		{
			throw new NotImplementedException();
		}

		public abstract string GetStreamName(string configPath);

		public abstract void Init(IInternalConfigRoot root, params object[] hostInitParams);

		public abstract void InitForConfiguration(ref string locationSubPath, out string configPath, out string locationConfigPath, IInternalConfigRoot root, params object[] hostInitConfigurationParams);

		[MonoNotSupported("mono does not support remote configuration")]
		public virtual string GetStreamNameForConfigSource(string streamName, string configSource)
		{
			throw new NotSupportedException("mono does not support remote configuration");
		}

		public virtual object GetStreamVersion(string streamName)
		{
			throw new NotImplementedException();
		}

		public virtual IDisposable Impersonate()
		{
			throw new NotImplementedException();
		}

		public virtual bool IsAboveApplication(string configPath)
		{
			throw new NotImplementedException();
		}

		public virtual bool IsConfigRecordRequired(string configPath)
		{
			throw new NotImplementedException();
		}

		public virtual bool IsDefinitionAllowed(string configPath, ConfigurationAllowDefinition allowDefinition, ConfigurationAllowExeDefinition allowExeDefinition)
		{
			if (allowDefinition != ConfigurationAllowDefinition.MachineOnly)
			{
				return allowDefinition != ConfigurationAllowDefinition.MachineToApplication || configPath == "machine" || configPath == "exe";
			}
			return configPath == "machine";
		}

		public virtual bool IsFile(string streamName)
		{
			throw new NotImplementedException();
		}

		public virtual bool IsFullTrustSectionWithoutAptcaAllowed(IInternalConfigRecord configRecord)
		{
			throw new NotImplementedException();
		}

		public virtual bool IsInitDelayed(IInternalConfigRecord configRecord)
		{
			throw new NotImplementedException();
		}

		public virtual bool IsLocationApplicable(string configPath)
		{
			throw new NotImplementedException();
		}

		public virtual bool IsRemote
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsSecondaryRoot(string configPath)
		{
			throw new NotImplementedException();
		}

		public virtual bool IsTrustedConfigPath(string configPath)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string get_bundled_machine_config();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string get_bundled_app_config();

		public virtual Stream OpenStreamForRead(string streamName)
		{
			if (string.CompareOrdinal(streamName, RuntimeEnvironment.SystemConfigurationFile) == 0)
			{
				string bundled_machine_config = InternalConfigurationHost.get_bundled_machine_config();
				if (bundled_machine_config != null)
				{
					return new MemoryStream(Encoding.UTF8.GetBytes(bundled_machine_config));
				}
			}
			if (string.CompareOrdinal(streamName, AppDomain.CurrentDomain.SetupInformation.ConfigurationFile) == 0)
			{
				string bundled_app_config = InternalConfigurationHost.get_bundled_app_config();
				if (bundled_app_config != null)
				{
					return new MemoryStream(Encoding.UTF8.GetBytes(bundled_app_config));
				}
			}
			if (!File.Exists(streamName))
			{
				return null;
			}
			return new FileStream(streamName, FileMode.Open, FileAccess.Read);
		}

		public virtual Stream OpenStreamForRead(string streamName, bool assertPermissions)
		{
			throw new NotImplementedException();
		}

		public virtual Stream OpenStreamForWrite(string streamName, string templateStreamName, ref object writeContext)
		{
			string directoryName = Path.GetDirectoryName(streamName);
			if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			return new FileStream(streamName, FileMode.Create, FileAccess.Write);
		}

		public virtual Stream OpenStreamForWrite(string streamName, string templateStreamName, ref object writeContext, bool assertPermissions)
		{
			throw new NotImplementedException();
		}

		public virtual bool PrefetchAll(string configPath, string streamName)
		{
			throw new NotImplementedException();
		}

		public virtual bool PrefetchSection(string sectionGroupName, string sectionName)
		{
			throw new NotImplementedException();
		}

		public virtual void RequireCompleteInit(IInternalConfigRecord configRecord)
		{
			throw new NotImplementedException();
		}

		public virtual object StartMonitoringStreamForChanges(string streamName, StreamChangeCallback callback)
		{
			throw new NotImplementedException();
		}

		public virtual void StopMonitoringStreamForChanges(string streamName, StreamChangeCallback callback)
		{
			throw new NotImplementedException();
		}

		public virtual void VerifyDefinitionAllowed(string configPath, ConfigurationAllowDefinition allowDefinition, ConfigurationAllowExeDefinition allowExeDefinition, IConfigErrorInfo errorInfo)
		{
			if (!this.IsDefinitionAllowed(configPath, allowDefinition, allowExeDefinition))
			{
				throw new ConfigurationErrorsException("The section can't be defined in this file (the allowed definition context is '" + allowDefinition.ToString() + "').", errorInfo.Filename, errorInfo.LineNumber);
			}
		}

		public virtual void WriteCompleted(string streamName, bool success, object writeContext)
		{
		}

		public virtual void WriteCompleted(string streamName, bool success, object writeContext, bool assertPermissions)
		{
		}

		public virtual bool SupportsChangeNotifications
		{
			get
			{
				return false;
			}
		}

		public virtual bool SupportsLocation
		{
			get
			{
				return false;
			}
		}

		public virtual bool SupportsPath
		{
			get
			{
				return false;
			}
		}

		public virtual bool SupportsRefresh
		{
			get
			{
				return false;
			}
		}
	}
}
