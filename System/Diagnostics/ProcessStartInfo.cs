using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32;

namespace System.Diagnostics
{
	[global::System.ComponentModel.TypeConverter(typeof(global::System.ComponentModel.ExpandableObjectConverter))]
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public sealed class ProcessStartInfo
	{
		public ProcessStartInfo()
		{
		}

		public ProcessStartInfo(string filename)
		{
			this.filename = filename;
		}

		public ProcessStartInfo(string filename, string arguments)
		{
			this.filename = filename;
			this.arguments = arguments;
		}

		[global::System.ComponentModel.TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[global::System.ComponentModel.RecommendedAsConfigurable(true)]
		[global::System.ComponentModel.DefaultValue("")]
		[MonitoringDescription("Command line agruments for this process.")]
		[global::System.ComponentModel.NotifyParentProperty(true)]
		public string Arguments
		{
			get
			{
				return this.arguments;
			}
			set
			{
				this.arguments = value;
			}
		}

		[global::System.ComponentModel.DefaultValue(false)]
		[MonitoringDescription("Start this process with a new window.")]
		[global::System.ComponentModel.NotifyParentProperty(true)]
		public bool CreateNoWindow
		{
			get
			{
				return this.create_no_window;
			}
			set
			{
				this.create_no_window = value;
			}
		}

		[MonitoringDescription("Environment variables used for this process.")]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Content)]
		[global::System.ComponentModel.DefaultValue(null)]
		[global::System.ComponentModel.Editor("System.Diagnostics.Design.StringDictionaryEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[global::System.ComponentModel.NotifyParentProperty(true)]
		public global::System.Collections.Specialized.StringDictionary EnvironmentVariables
		{
			get
			{
				if (this.envVars == null)
				{
					this.envVars = new global::System.Collections.Specialized.ProcessStringDictionary();
					foreach (object obj in Environment.GetEnvironmentVariables())
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
						this.envVars.Add((string)dictionaryEntry.Key, (string)dictionaryEntry.Value);
					}
				}
				return this.envVars;
			}
		}

		internal bool HaveEnvVars
		{
			get
			{
				return this.envVars != null;
			}
		}

		[global::System.ComponentModel.DefaultValue(false)]
		[MonitoringDescription("Thread shows dialogboxes for errors.")]
		[global::System.ComponentModel.NotifyParentProperty(true)]
		public bool ErrorDialog
		{
			get
			{
				return this.error_dialog;
			}
			set
			{
				this.error_dialog = value;
			}
		}

		[global::System.ComponentModel.Browsable(false)]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public IntPtr ErrorDialogParentHandle
		{
			get
			{
				return this.error_dialog_parent_handle;
			}
			set
			{
				this.error_dialog_parent_handle = value;
			}
		}

		[global::System.ComponentModel.DefaultValue("")]
		[global::System.ComponentModel.RecommendedAsConfigurable(true)]
		[global::System.ComponentModel.Editor("System.Diagnostics.Design.StartFileNameEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[global::System.ComponentModel.TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[MonitoringDescription("The name of the resource to start this process.")]
		[global::System.ComponentModel.NotifyParentProperty(true)]
		public string FileName
		{
			get
			{
				return this.filename;
			}
			set
			{
				this.filename = value;
			}
		}

		[global::System.ComponentModel.DefaultValue(false)]
		[global::System.ComponentModel.NotifyParentProperty(true)]
		[MonitoringDescription("Errors of this process are redirected.")]
		public bool RedirectStandardError
		{
			get
			{
				return this.redirect_standard_error;
			}
			set
			{
				this.redirect_standard_error = value;
			}
		}

		[MonitoringDescription("Standard input of this process is redirected.")]
		[global::System.ComponentModel.NotifyParentProperty(true)]
		[global::System.ComponentModel.DefaultValue(false)]
		public bool RedirectStandardInput
		{
			get
			{
				return this.redirect_standard_input;
			}
			set
			{
				this.redirect_standard_input = value;
			}
		}

		[global::System.ComponentModel.DefaultValue(false)]
		[MonitoringDescription("Standart output of this process is redirected.")]
		[global::System.ComponentModel.NotifyParentProperty(true)]
		public bool RedirectStandardOutput
		{
			get
			{
				return this.redirect_standard_output;
			}
			set
			{
				this.redirect_standard_output = value;
			}
		}

		public Encoding StandardErrorEncoding
		{
			get
			{
				return this.encoding_stderr;
			}
			set
			{
				this.encoding_stderr = value;
			}
		}

		public Encoding StandardOutputEncoding
		{
			get
			{
				return this.encoding_stdout;
			}
			set
			{
				this.encoding_stdout = value;
			}
		}

		[global::System.ComponentModel.NotifyParentProperty(true)]
		[MonitoringDescription("Use the shell to start this process.")]
		[global::System.ComponentModel.DefaultValue(true)]
		public bool UseShellExecute
		{
			get
			{
				return this.use_shell_execute;
			}
			set
			{
				this.use_shell_execute = value;
			}
		}

		[MonitoringDescription("The verb to apply to a used document.")]
		[global::System.ComponentModel.NotifyParentProperty(true)]
		[global::System.ComponentModel.DefaultValue("")]
		[global::System.ComponentModel.TypeConverter("System.Diagnostics.Design.VerbConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public string Verb
		{
			get
			{
				return this.verb;
			}
			set
			{
				this.verb = value;
			}
		}

		[global::System.ComponentModel.Browsable(false)]
		[global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public string[] Verbs
		{
			get
			{
				string text = ((!((this.filename == null) | (this.filename.Length == 0))) ? Path.GetExtension(this.filename) : null);
				if (text == null)
				{
					return ProcessStartInfo.empty;
				}
				PlatformID platform = Environment.OSVersion.Platform;
				switch (platform)
				{
				case PlatformID.Unix:
				case PlatformID.MacOSX:
					break;
				default:
					if (platform != (PlatformID)128)
					{
						RegistryKey registryKey = null;
						RegistryKey registryKey2 = null;
						RegistryKey registryKey3 = null;
						string[] array;
						try
						{
							registryKey = Registry.ClassesRoot.OpenSubKey(text);
							string text2 = ((registryKey == null) ? null : (registryKey.GetValue(null) as string));
							registryKey2 = ((text2 == null) ? null : Registry.ClassesRoot.OpenSubKey(text2));
							registryKey3 = ((registryKey2 == null) ? null : registryKey2.OpenSubKey("shell"));
							array = ((registryKey3 == null) ? null : registryKey3.GetSubKeyNames());
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
					break;
				}
				return ProcessStartInfo.empty;
			}
		}

		[global::System.ComponentModel.NotifyParentProperty(true)]
		[global::System.ComponentModel.DefaultValue(typeof(ProcessWindowStyle), "Normal")]
		[MonitoringDescription("The window style used to start this process.")]
		public ProcessWindowStyle WindowStyle
		{
			get
			{
				return this.window_style;
			}
			set
			{
				this.window_style = value;
			}
		}

		[global::System.ComponentModel.NotifyParentProperty(true)]
		[global::System.ComponentModel.TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[MonitoringDescription("The initial directory for this process.")]
		[global::System.ComponentModel.Editor("System.Diagnostics.Design.WorkingDirectoryEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[global::System.ComponentModel.RecommendedAsConfigurable(true)]
		[global::System.ComponentModel.DefaultValue("")]
		public string WorkingDirectory
		{
			get
			{
				return this.working_directory;
			}
			set
			{
				this.working_directory = ((value != null) ? value : string.Empty);
			}
		}

		[global::System.ComponentModel.NotifyParentProperty(true)]
		public bool LoadUserProfile
		{
			get
			{
				return this.load_user_profile;
			}
			set
			{
				this.load_user_profile = value;
			}
		}

		[global::System.ComponentModel.NotifyParentProperty(true)]
		public string UserName
		{
			get
			{
				return this.username;
			}
			set
			{
				this.username = value;
			}
		}

		[global::System.ComponentModel.NotifyParentProperty(true)]
		public string Domain
		{
			get
			{
				return this.domain;
			}
			set
			{
				this.domain = value;
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

		private string arguments = string.Empty;

		private IntPtr error_dialog_parent_handle = (IntPtr)0;

		private string filename = string.Empty;

		private string verb = string.Empty;

		private string working_directory = string.Empty;

		private global::System.Collections.Specialized.ProcessStringDictionary envVars;

		private bool create_no_window;

		private bool error_dialog;

		private bool redirect_standard_error;

		private bool redirect_standard_input;

		private bool redirect_standard_output;

		private bool use_shell_execute = true;

		private ProcessWindowStyle window_style;

		private Encoding encoding_stderr;

		private Encoding encoding_stdout;

		private string username;

		private string domain;

		private SecureString password;

		private bool load_user_profile;

		private static readonly string[] empty = new string[0];
	}
}
