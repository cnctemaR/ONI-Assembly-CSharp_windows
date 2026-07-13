using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32;

namespace System.Diagnostics
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	[HostProtection(SecurityAction.LinkDemand, SharedState = true, SelfAffectingProcessMgmt = true)]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class ProcessStartInfo
	{
		public ProcessStartInfo()
		{
		}

		internal ProcessStartInfo(Process parent)
		{
			this.weakParentProcess = new WeakReference(parent);
		}

		public ProcessStartInfo(string fileName)
		{
			this.fileName = fileName;
		}

		public ProcessStartInfo(string fileName, string arguments)
		{
			this.fileName = fileName;
			this.arguments = arguments;
		}

		public Collection<string> ArgumentList
		{
			get
			{
				if (this._argumentList == null)
				{
					this._argumentList = new Collection<string>();
				}
				return this._argumentList;
			}
		}

		[DefaultValue("")]
		[TypeConverter("System.Diagnostics.Design.VerbConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[MonitoringDescription("The verb to apply to the document specified by the FileName property.")]
		[NotifyParentProperty(true)]
		public string Verb
		{
			get
			{
				if (this.verb == null)
				{
					return string.Empty;
				}
				return this.verb;
			}
			set
			{
				this.verb = value;
			}
		}

		[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[NotifyParentProperty(true)]
		[SettingsBindable(true)]
		[DefaultValue("")]
		[MonitoringDescription("Command line arguments that will be passed to the application specified by the FileName property.")]
		public string Arguments
		{
			get
			{
				if (this.arguments == null)
				{
					return string.Empty;
				}
				return this.arguments;
			}
			set
			{
				this.arguments = value;
			}
		}

		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		[MonitoringDescription("Whether to start the process without creating a new window to contain it.")]
		public bool CreateNoWindow
		{
			get
			{
				return this.createNoWindow;
			}
			set
			{
				this.createNoWindow = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DefaultValue(null)]
		[MonitoringDescription("Set of environment variables that apply to this process and child processes.")]
		[NotifyParentProperty(true)]
		[Editor("System.Diagnostics.Design.StringDictionaryEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public StringDictionary EnvironmentVariables
		{
			get
			{
				if (this.environmentVariables == null)
				{
					this.environmentVariables = new CaseSensitiveStringDictionary();
					if (this.weakParentProcess == null || !this.weakParentProcess.IsAlive || ((Component)this.weakParentProcess.Target).Site == null || !((Component)this.weakParentProcess.Target).Site.DesignMode)
					{
						foreach (object obj in global::System.Environment.GetEnvironmentVariables())
						{
							DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
							this.environmentVariables.Add((string)dictionaryEntry.Key, (string)dictionaryEntry.Value);
						}
					}
				}
				return this.environmentVariables;
			}
		}

		[DefaultValue(null)]
		[NotifyParentProperty(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IDictionary<string, string> Environment
		{
			get
			{
				if (this.environment == null)
				{
					this.environment = this.EnvironmentVariables.AsGenericDictionary();
				}
				return this.environment;
			}
		}

		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		[MonitoringDescription("Whether the process command input is read from the Process instance's StandardInput member.")]
		public bool RedirectStandardInput
		{
			get
			{
				return this.redirectStandardInput;
			}
			set
			{
				this.redirectStandardInput = value;
			}
		}

		[NotifyParentProperty(true)]
		[MonitoringDescription("Whether the process output is written to the Process instance's StandardOutput member.")]
		[DefaultValue(false)]
		public bool RedirectStandardOutput
		{
			get
			{
				return this.redirectStandardOutput;
			}
			set
			{
				this.redirectStandardOutput = value;
			}
		}

		[MonitoringDescription("Whether the process's error output is written to the Process instance's StandardError member.")]
		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		public bool RedirectStandardError
		{
			get
			{
				return this.redirectStandardError;
			}
			set
			{
				this.redirectStandardError = value;
			}
		}

		public Encoding StandardErrorEncoding
		{
			get
			{
				return this.standardErrorEncoding;
			}
			set
			{
				this.standardErrorEncoding = value;
			}
		}

		public Encoding StandardOutputEncoding
		{
			get
			{
				return this.standardOutputEncoding;
			}
			set
			{
				this.standardOutputEncoding = value;
			}
		}

		[NotifyParentProperty(true)]
		[MonitoringDescription("Whether to use the operating system shell to start the process.")]
		[DefaultValue(true)]
		public bool UseShellExecute
		{
			get
			{
				return this.useShellExecute;
			}
			set
			{
				this.useShellExecute = value;
			}
		}

		[NotifyParentProperty(true)]
		public string UserName
		{
			get
			{
				if (this.userName == null)
				{
					return string.Empty;
				}
				return this.userName;
			}
			set
			{
				this.userName = value;
			}
		}

		public SecureString Password
		{
			get
			{
				return this.password;
			}
			set
			{
				this.password = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string PasswordInClearText
		{
			get
			{
				return this.passwordInClearText;
			}
			set
			{
				this.passwordInClearText = value;
			}
		}

		[NotifyParentProperty(true)]
		public string Domain
		{
			get
			{
				if (this.domain == null)
				{
					return string.Empty;
				}
				return this.domain;
			}
			set
			{
				this.domain = value;
			}
		}

		[NotifyParentProperty(true)]
		public bool LoadUserProfile
		{
			get
			{
				return this.loadUserProfile;
			}
			set
			{
				this.loadUserProfile = value;
			}
		}

		[NotifyParentProperty(true)]
		[DefaultValue("")]
		[Editor("System.Diagnostics.Design.StartFileNameEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[MonitoringDescription("The name of the application, document or URL to start.")]
		[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[SettingsBindable(true)]
		public string FileName
		{
			get
			{
				if (this.fileName == null)
				{
					return string.Empty;
				}
				return this.fileName;
			}
			set
			{
				this.fileName = value;
			}
		}

		[MonitoringDescription("The initial working directory for the process.")]
		[DefaultValue("")]
		[NotifyParentProperty(true)]
		[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[SettingsBindable(true)]
		[Editor("System.Diagnostics.Design.WorkingDirectoryEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public string WorkingDirectory
		{
			get
			{
				if (this.directory == null)
				{
					return string.Empty;
				}
				return this.directory;
			}
			set
			{
				this.directory = value;
			}
		}

		[DefaultValue(false)]
		[NotifyParentProperty(true)]
		[MonitoringDescription("Whether to show an error dialog to the user if there is an error.")]
		public bool ErrorDialog
		{
			get
			{
				return this.errorDialog;
			}
			set
			{
				this.errorDialog = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IntPtr ErrorDialogParentHandle
		{
			get
			{
				return this.errorDialogParentHandle;
			}
			set
			{
				this.errorDialogParentHandle = value;
			}
		}

		[DefaultValue(ProcessWindowStyle.Normal)]
		[MonitoringDescription("How the main window should be created when the process starts.")]
		[NotifyParentProperty(true)]
		public ProcessWindowStyle WindowStyle
		{
			get
			{
				return this.windowStyle;
			}
			set
			{
				if (!Enum.IsDefined(typeof(ProcessWindowStyle), value))
				{
					throw new InvalidEnumArgumentException("value", (int)value, typeof(ProcessWindowStyle));
				}
				this.windowStyle = value;
			}
		}

		internal bool HaveEnvVars
		{
			get
			{
				return this.environmentVariables != null;
			}
		}

		public Encoding StandardInputEncoding { get; set; }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string[] Verbs
		{
			get
			{
				PlatformID platform = global::System.Environment.OSVersion.Platform;
				if (platform == PlatformID.Unix || platform == PlatformID.MacOSX || platform == (PlatformID)128)
				{
					return ProcessStartInfo.empty;
				}
				string text = (string.IsNullOrEmpty(this.fileName) ? null : Path.GetExtension(this.fileName));
				if (text == null)
				{
					return ProcessStartInfo.empty;
				}
				RegistryKey registryKey = null;
				RegistryKey registryKey2 = null;
				RegistryKey registryKey3 = null;
				string[] array;
				try
				{
					registryKey = Registry.ClassesRoot.OpenSubKey(text);
					string text2 = ((registryKey != null) ? (registryKey.GetValue(null) as string) : null);
					registryKey2 = ((text2 != null) ? Registry.ClassesRoot.OpenSubKey(text2) : null);
					registryKey3 = ((registryKey2 != null) ? registryKey2.OpenSubKey("shell") : null);
					array = ((registryKey3 != null) ? registryKey3.GetSubKeyNames() : null);
				}
				finally
				{
					if (registryKey3 != null)
					{
						registryKey3.Close();
					}
					if (registryKey2 != null)
					{
						registryKey2.Close();
					}
					if (registryKey != null)
					{
						registryKey.Close();
					}
				}
				return array;
			}
		}

		private string fileName;

		private string arguments;

		private string directory;

		private string verb;

		private ProcessWindowStyle windowStyle;

		private bool errorDialog;

		private IntPtr errorDialogParentHandle;

		private bool useShellExecute = true;

		private string userName;

		private string domain;

		private SecureString password;

		private string passwordInClearText;

		private bool loadUserProfile;

		private bool redirectStandardInput;

		private bool redirectStandardOutput;

		private bool redirectStandardError;

		private Encoding standardOutputEncoding;

		private Encoding standardErrorEncoding;

		private bool createNoWindow;

		private WeakReference weakParentProcess;

		internal StringDictionary environmentVariables;

		private static readonly string[] empty = new string[0];

		private Collection<string> _argumentList;

		private IDictionary<string, string> environment;
	}
}
