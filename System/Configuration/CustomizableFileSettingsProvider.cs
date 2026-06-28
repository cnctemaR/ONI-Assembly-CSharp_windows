using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace System.Configuration
{
	internal class CustomizableFileSettingsProvider : SettingsProvider, IApplicationSettingsProvider
	{
		public override void Initialize(string name, global::System.Collections.Specialized.NameValueCollection config)
		{
			base.Initialize(name, config);
		}

		internal static string UserRoamingFullPath
		{
			get
			{
				return Path.Combine(CustomizableFileSettingsProvider.userRoamingPath, CustomizableFileSettingsProvider.userRoamingName);
			}
		}

		internal static string UserLocalFullPath
		{
			get
			{
				return Path.Combine(CustomizableFileSettingsProvider.userLocalPath, CustomizableFileSettingsProvider.userLocalName);
			}
		}

		public static string PrevUserRoamingFullPath
		{
			get
			{
				return Path.Combine(CustomizableFileSettingsProvider.userRoamingPathPrevVersion, CustomizableFileSettingsProvider.userRoamingName);
			}
		}

		public static string PrevUserLocalFullPath
		{
			get
			{
				return Path.Combine(CustomizableFileSettingsProvider.userLocalPathPrevVersion, CustomizableFileSettingsProvider.userLocalName);
			}
		}

		public static string UserRoamingPath
		{
			get
			{
				return CustomizableFileSettingsProvider.userRoamingPath;
			}
		}

		public static string UserLocalPath
		{
			get
			{
				return CustomizableFileSettingsProvider.userLocalPath;
			}
		}

		public static string UserRoamingName
		{
			get
			{
				return CustomizableFileSettingsProvider.userRoamingName;
			}
		}

		public static string UserLocalName
		{
			get
			{
				return CustomizableFileSettingsProvider.userLocalName;
			}
		}

		public static UserConfigLocationOption UserConfigSelector
		{
			get
			{
				return CustomizableFileSettingsProvider.userConfig;
			}
			set
			{
				CustomizableFileSettingsProvider.userConfig = value;
				if ((CustomizableFileSettingsProvider.userConfig & UserConfigLocationOption.Other) != (UserConfigLocationOption)0U)
				{
					CustomizableFileSettingsProvider.isVersionMajor = false;
					CustomizableFileSettingsProvider.isVersionMinor = false;
					CustomizableFileSettingsProvider.isVersionBuild = false;
					CustomizableFileSettingsProvider.isVersionRevision = false;
					CustomizableFileSettingsProvider.isCompany = false;
					return;
				}
				CustomizableFileSettingsProvider.isVersionRevision = (CustomizableFileSettingsProvider.userConfig & (UserConfigLocationOption)8U) != (UserConfigLocationOption)0U;
				CustomizableFileSettingsProvider.isVersionBuild = CustomizableFileSettingsProvider.isVersionRevision | ((CustomizableFileSettingsProvider.userConfig & (UserConfigLocationOption)4U) != (UserConfigLocationOption)0U);
				CustomizableFileSettingsProvider.isVersionMinor = CustomizableFileSettingsProvider.isVersionBuild | ((CustomizableFileSettingsProvider.userConfig & (UserConfigLocationOption)2U) != (UserConfigLocationOption)0U);
				CustomizableFileSettingsProvider.isVersionMajor = CustomizableFileSettingsProvider.IsVersionMinor | ((CustomizableFileSettingsProvider.userConfig & (UserConfigLocationOption)1U) != (UserConfigLocationOption)0U);
				CustomizableFileSettingsProvider.isCompany = (CustomizableFileSettingsProvider.userConfig & (UserConfigLocationOption)16U) != (UserConfigLocationOption)0U;
				CustomizableFileSettingsProvider.isProduct = (CustomizableFileSettingsProvider.userConfig & UserConfigLocationOption.Product) != (UserConfigLocationOption)0U;
			}
		}

		public static bool IsVersionMajor
		{
			get
			{
				return CustomizableFileSettingsProvider.isVersionMajor;
			}
			set
			{
				CustomizableFileSettingsProvider.isVersionMajor = value;
				CustomizableFileSettingsProvider.isVersionMinor = false;
				CustomizableFileSettingsProvider.isVersionBuild = false;
				CustomizableFileSettingsProvider.isVersionRevision = false;
			}
		}

		public static bool IsVersionMinor
		{
			get
			{
				return CustomizableFileSettingsProvider.isVersionMinor;
			}
			set
			{
				CustomizableFileSettingsProvider.isVersionMinor = value;
				if (CustomizableFileSettingsProvider.isVersionMinor)
				{
					CustomizableFileSettingsProvider.isVersionMajor = true;
				}
				CustomizableFileSettingsProvider.isVersionBuild = false;
				CustomizableFileSettingsProvider.isVersionRevision = false;
			}
		}

		public static bool IsVersionBuild
		{
			get
			{
				return CustomizableFileSettingsProvider.isVersionBuild;
			}
			set
			{
				CustomizableFileSettingsProvider.isVersionBuild = value;
				if (CustomizableFileSettingsProvider.isVersionBuild)
				{
					CustomizableFileSettingsProvider.isVersionMajor = true;
					CustomizableFileSettingsProvider.isVersionMinor = true;
				}
				CustomizableFileSettingsProvider.isVersionRevision = false;
			}
		}

		public static bool IsVersionRevision
		{
			get
			{
				return CustomizableFileSettingsProvider.isVersionRevision;
			}
			set
			{
				CustomizableFileSettingsProvider.isVersionRevision = value;
				if (CustomizableFileSettingsProvider.isVersionRevision)
				{
					CustomizableFileSettingsProvider.isVersionMajor = true;
					CustomizableFileSettingsProvider.isVersionMinor = true;
					CustomizableFileSettingsProvider.isVersionBuild = true;
				}
			}
		}

		public static bool IsCompany
		{
			get
			{
				return CustomizableFileSettingsProvider.isCompany;
			}
			set
			{
				CustomizableFileSettingsProvider.isCompany = value;
			}
		}

		public static bool IsEvidence
		{
			get
			{
				return CustomizableFileSettingsProvider.isEvidence;
			}
			set
			{
				CustomizableFileSettingsProvider.isEvidence = value;
			}
		}

		private static string GetCompanyName()
		{
			Assembly assembly = Assembly.GetEntryAssembly();
			if (assembly == null)
			{
				assembly = Assembly.GetCallingAssembly();
			}
			AssemblyCompanyAttribute[] array = (AssemblyCompanyAttribute[])assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
			if (array != null && array.Length > 0)
			{
				return array[0].Company;
			}
			MethodInfo entryPoint = assembly.EntryPoint;
			Type type = ((entryPoint == null) ? null : entryPoint.DeclaringType);
			if (type != null && !string.IsNullOrEmpty(type.Namespace))
			{
				int num = type.Namespace.IndexOf('.');
				return (num >= 0) ? type.Namespace.Substring(0, num) : type.Namespace;
			}
			return "Program";
		}

		private static string GetProductName()
		{
			Assembly assembly = Assembly.GetEntryAssembly();
			if (assembly == null)
			{
				assembly = Assembly.GetCallingAssembly();
			}
			byte[] publicKeyToken = assembly.GetName().GetPublicKeyToken();
			return string.Format("{0}_{1}_{2}", AppDomain.CurrentDomain.FriendlyName, (publicKeyToken == null) ? "Url" : "StrongName", CustomizableFileSettingsProvider.GetEvidenceHash());
		}

		private static string GetEvidenceHash()
		{
			Assembly assembly = Assembly.GetEntryAssembly();
			if (assembly == null)
			{
				assembly = Assembly.GetCallingAssembly();
			}
			byte[] publicKeyToken = assembly.GetName().GetPublicKeyToken();
			byte[] array = SHA1.Create().ComputeHash((publicKeyToken == null) ? Encoding.UTF8.GetBytes(assembly.EscapedCodeBase) : publicKeyToken);
			return Convert.ToBase64String(array);
		}

		private static string GetProductVersion()
		{
			Assembly assembly = Assembly.GetEntryAssembly();
			if (assembly == null)
			{
				assembly = Assembly.GetCallingAssembly();
			}
			if (assembly == null)
			{
				return string.Empty;
			}
			return assembly.GetName().Version.ToString();
		}

		private static void CreateUserConfigPath()
		{
			if (CustomizableFileSettingsProvider.userDefine)
			{
				return;
			}
			if (CustomizableFileSettingsProvider.ProductName == string.Empty)
			{
				CustomizableFileSettingsProvider.ProductName = CustomizableFileSettingsProvider.GetProductName();
			}
			if (CustomizableFileSettingsProvider.CompanyName == string.Empty)
			{
				CustomizableFileSettingsProvider.CompanyName = CustomizableFileSettingsProvider.GetCompanyName();
			}
			if (CustomizableFileSettingsProvider.ForceVersion == string.Empty)
			{
				CustomizableFileSettingsProvider.ProductVersion = CustomizableFileSettingsProvider.GetProductVersion().Split(new char[] { '.' });
			}
			if (CustomizableFileSettingsProvider.userRoamingBasePath == string.Empty)
			{
				CustomizableFileSettingsProvider.userRoamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			}
			else
			{
				CustomizableFileSettingsProvider.userRoamingPath = CustomizableFileSettingsProvider.userRoamingBasePath;
			}
			if (CustomizableFileSettingsProvider.userLocalBasePath == string.Empty)
			{
				CustomizableFileSettingsProvider.userLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			}
			else
			{
				CustomizableFileSettingsProvider.userLocalPath = CustomizableFileSettingsProvider.userLocalBasePath;
			}
			if (CustomizableFileSettingsProvider.isCompany)
			{
				CustomizableFileSettingsProvider.userRoamingPath = Path.Combine(CustomizableFileSettingsProvider.userRoamingPath, CustomizableFileSettingsProvider.CompanyName);
				CustomizableFileSettingsProvider.userLocalPath = Path.Combine(CustomizableFileSettingsProvider.userLocalPath, CustomizableFileSettingsProvider.CompanyName);
			}
			if (CustomizableFileSettingsProvider.isProduct)
			{
				if (CustomizableFileSettingsProvider.isEvidence)
				{
					Assembly assembly = Assembly.GetEntryAssembly();
					if (assembly == null)
					{
						assembly = Assembly.GetCallingAssembly();
					}
					byte[] publicKeyToken = assembly.GetName().GetPublicKeyToken();
					CustomizableFileSettingsProvider.ProductName = string.Format("{0}_{1}_{2}", CustomizableFileSettingsProvider.ProductName, (publicKeyToken == null) ? "Url" : "StrongName", CustomizableFileSettingsProvider.GetEvidenceHash());
				}
				CustomizableFileSettingsProvider.userRoamingPath = Path.Combine(CustomizableFileSettingsProvider.userRoamingPath, CustomizableFileSettingsProvider.ProductName);
				CustomizableFileSettingsProvider.userLocalPath = Path.Combine(CustomizableFileSettingsProvider.userLocalPath, CustomizableFileSettingsProvider.ProductName);
			}
			string text;
			if (CustomizableFileSettingsProvider.ForceVersion == string.Empty)
			{
				if (CustomizableFileSettingsProvider.isVersionRevision)
				{
					text = string.Format("{0}.{1}.{2}.{3}", new object[]
					{
						CustomizableFileSettingsProvider.ProductVersion[0],
						CustomizableFileSettingsProvider.ProductVersion[1],
						CustomizableFileSettingsProvider.ProductVersion[2],
						CustomizableFileSettingsProvider.ProductVersion[3]
					});
				}
				else if (CustomizableFileSettingsProvider.isVersionBuild)
				{
					text = string.Format("{0}.{1}.{2}", CustomizableFileSettingsProvider.ProductVersion[0], CustomizableFileSettingsProvider.ProductVersion[1], CustomizableFileSettingsProvider.ProductVersion[2]);
				}
				else if (CustomizableFileSettingsProvider.isVersionMinor)
				{
					text = string.Format("{0}.{1}", CustomizableFileSettingsProvider.ProductVersion[0], CustomizableFileSettingsProvider.ProductVersion[1]);
				}
				else if (CustomizableFileSettingsProvider.isVersionMajor)
				{
					text = CustomizableFileSettingsProvider.ProductVersion[0];
				}
				else
				{
					text = string.Empty;
				}
			}
			else
			{
				text = CustomizableFileSettingsProvider.ForceVersion;
			}
			string text2 = CustomizableFileSettingsProvider.PrevVersionPath(CustomizableFileSettingsProvider.userRoamingPath, text);
			string text3 = CustomizableFileSettingsProvider.PrevVersionPath(CustomizableFileSettingsProvider.userLocalPath, text);
			CustomizableFileSettingsProvider.userRoamingPath = Path.Combine(CustomizableFileSettingsProvider.userRoamingPath, text);
			CustomizableFileSettingsProvider.userLocalPath = Path.Combine(CustomizableFileSettingsProvider.userLocalPath, text);
			if (text2 != string.Empty)
			{
				CustomizableFileSettingsProvider.userRoamingPathPrevVersion = Path.Combine(CustomizableFileSettingsProvider.userRoamingPath, text2);
			}
			if (text3 != string.Empty)
			{
				CustomizableFileSettingsProvider.userLocalPathPrevVersion = Path.Combine(CustomizableFileSettingsProvider.userLocalPath, text3);
			}
		}

		private static string PrevVersionPath(string dirName, string currentVersion)
		{
			string text = string.Empty;
			if (!Directory.Exists(dirName))
			{
				return text;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(dirName);
			foreach (DirectoryInfo directoryInfo2 in directoryInfo.GetDirectories())
			{
				if (string.Compare(currentVersion, directoryInfo2.Name, StringComparison.Ordinal) > 0 && string.Compare(text, directoryInfo2.Name, StringComparison.Ordinal) < 0)
				{
					text = directoryInfo2.Name;
				}
			}
			return text;
		}

		public static bool SetUserRoamingPath(string configPath)
		{
			if (CustomizableFileSettingsProvider.CheckPath(configPath))
			{
				CustomizableFileSettingsProvider.userRoamingBasePath = configPath;
				return true;
			}
			return false;
		}

		public static bool SetUserLocalPath(string configPath)
		{
			if (CustomizableFileSettingsProvider.CheckPath(configPath))
			{
				CustomizableFileSettingsProvider.userLocalBasePath = configPath;
				return true;
			}
			return false;
		}

		private static bool CheckFileName(string configFile)
		{
			return configFile.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
		}

		public static bool SetUserRoamingFileName(string configFile)
		{
			if (CustomizableFileSettingsProvider.CheckFileName(configFile))
			{
				CustomizableFileSettingsProvider.userRoamingName = configFile;
				return true;
			}
			return false;
		}

		public static bool SetUserLocalFileName(string configFile)
		{
			if (CustomizableFileSettingsProvider.CheckFileName(configFile))
			{
				CustomizableFileSettingsProvider.userLocalName = configFile;
				return true;
			}
			return false;
		}

		public static bool SetCompanyName(string companyName)
		{
			if (CustomizableFileSettingsProvider.CheckFileName(companyName))
			{
				CustomizableFileSettingsProvider.CompanyName = companyName;
				return true;
			}
			return false;
		}

		public static bool SetProductName(string productName)
		{
			if (CustomizableFileSettingsProvider.CheckFileName(productName))
			{
				CustomizableFileSettingsProvider.ProductName = productName;
				return true;
			}
			return false;
		}

		public static bool SetVersion(int major)
		{
			CustomizableFileSettingsProvider.ForceVersion = string.Format("{0}", major);
			return true;
		}

		public static bool SetVersion(int major, int minor)
		{
			CustomizableFileSettingsProvider.ForceVersion = string.Format("{0}.{1}", major, minor);
			return true;
		}

		public static bool SetVersion(int major, int minor, int build)
		{
			CustomizableFileSettingsProvider.ForceVersion = string.Format("{0}.{1}.{2}", major, minor, build);
			return true;
		}

		public static bool SetVersion(int major, int minor, int build, int revision)
		{
			CustomizableFileSettingsProvider.ForceVersion = string.Format("{0}.{1}.{2}.{3}", new object[] { major, minor, build, revision });
			return true;
		}

		public static bool SetVersion(string forceVersion)
		{
			if (CustomizableFileSettingsProvider.CheckFileName(forceVersion))
			{
				CustomizableFileSettingsProvider.ForceVersion = forceVersion;
				return true;
			}
			return false;
		}

		private static bool CheckPath(string configPath)
		{
			char[] invalidPathChars = Path.GetInvalidPathChars();
			if (configPath.IndexOfAny(invalidPathChars) >= 0)
			{
				return false;
			}
			string text = configPath;
			string fileName;
			while ((fileName = Path.GetFileName(text)) != string.Empty)
			{
				if (!CustomizableFileSettingsProvider.CheckFileName(fileName))
				{
					return false;
				}
				text = Path.GetDirectoryName(text);
			}
			return true;
		}

		public override string Name
		{
			get
			{
				return base.Name;
			}
		}

		public override string ApplicationName
		{
			get
			{
				return this.app_name;
			}
			set
			{
				this.app_name = value;
			}
		}

		private void SaveProperties(ExeConfigurationFileMap exeMap, SettingsPropertyValueCollection collection, ConfigurationUserLevel level, SettingsContext context, bool checkUserLevel)
		{
			Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(exeMap, level);
			UserSettingsGroup userSettingsGroup = configuration.GetSectionGroup("userSettings") as UserSettingsGroup;
			bool flag = level == ConfigurationUserLevel.PerUserRoaming;
			if (userSettingsGroup == null)
			{
				userSettingsGroup = new UserSettingsGroup();
				configuration.SectionGroups.Add("userSettings", userSettingsGroup);
				ApplicationSettingsBase currentSettings = context.CurrentSettings;
				ClientSettingsSection clientSettingsSection = new ClientSettingsSection();
				userSettingsGroup.Sections.Add(((currentSettings == null) ? typeof(ApplicationSettingsBase) : currentSettings.GetType()).FullName, clientSettingsSection);
			}
			bool flag2 = false;
			foreach (object obj in userSettingsGroup.Sections)
			{
				ConfigurationSection configurationSection = (ConfigurationSection)obj;
				ClientSettingsSection clientSettingsSection2 = configurationSection as ClientSettingsSection;
				if (clientSettingsSection2 != null)
				{
					foreach (object obj2 in collection)
					{
						SettingsPropertyValue settingsPropertyValue = (SettingsPropertyValue)obj2;
						if (!checkUserLevel || settingsPropertyValue.Property.Attributes.Contains(typeof(SettingsManageabilityAttribute)) == flag)
						{
							flag2 = true;
							SettingElement settingElement = clientSettingsSection2.Settings.Get(settingsPropertyValue.Name);
							if (settingElement == null)
							{
								settingElement = new SettingElement(settingsPropertyValue.Name, settingsPropertyValue.Property.SerializeAs);
								clientSettingsSection2.Settings.Add(settingElement);
							}
							if (settingElement.Value.ValueXml == null)
							{
								settingElement.Value.ValueXml = new XmlDocument().CreateElement("value");
							}
							switch (settingsPropertyValue.Property.SerializeAs)
							{
							case SettingsSerializeAs.String:
								settingElement.Value.ValueXml.InnerText = settingsPropertyValue.SerializedValue as string;
								break;
							case SettingsSerializeAs.Xml:
								settingElement.Value.ValueXml.InnerXml = (settingsPropertyValue.SerializedValue as string) ?? string.Empty;
								break;
							case SettingsSerializeAs.Binary:
								settingElement.Value.ValueXml.InnerText = ((settingsPropertyValue.SerializedValue == null) ? string.Empty : Convert.ToBase64String(settingsPropertyValue.SerializedValue as byte[]));
								break;
							default:
								throw new NotImplementedException();
							}
						}
					}
				}
			}
			if (flag2)
			{
				configuration.Save(ConfigurationSaveMode.Minimal, true);
			}
		}

		private void LoadPropertyValue(SettingsPropertyCollection collection, SettingElement element, bool allowOverwrite)
		{
			SettingsProperty settingsProperty = collection[element.Name];
			if (settingsProperty == null)
			{
				settingsProperty = new SettingsProperty(element.Name);
				collection.Add(settingsProperty);
			}
			SettingsPropertyValue settingsPropertyValue = new SettingsPropertyValue(settingsProperty);
			settingsPropertyValue.IsDirty = false;
			if (element.Value.ValueXml != null)
			{
				switch (settingsPropertyValue.Property.SerializeAs)
				{
				case SettingsSerializeAs.String:
					settingsPropertyValue.SerializedValue = element.Value.ValueXml.InnerText;
					break;
				case SettingsSerializeAs.Xml:
					settingsPropertyValue.SerializedValue = element.Value.ValueXml.InnerXml;
					break;
				case SettingsSerializeAs.Binary:
					settingsPropertyValue.SerializedValue = Convert.FromBase64String(element.Value.ValueXml.InnerText);
					break;
				}
			}
			else
			{
				settingsPropertyValue.SerializedValue = settingsProperty.DefaultValue;
			}
			try
			{
				if (allowOverwrite)
				{
					this.values.Remove(element.Name);
				}
				this.values.Add(settingsPropertyValue);
			}
			catch (ArgumentException ex)
			{
				throw new ConfigurationErrorsException(string.Format(CultureInfo.InvariantCulture, "Failed to load value for '{0}'.", new object[] { element.Name }), ex);
			}
		}

		private void LoadProperties(ExeConfigurationFileMap exeMap, SettingsPropertyCollection collection, ConfigurationUserLevel level, string sectionGroupName, bool allowOverwrite, string groupName)
		{
			Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(exeMap, level);
			ConfigurationSectionGroup sectionGroup = configuration.GetSectionGroup(sectionGroupName);
			if (sectionGroup != null)
			{
				foreach (object obj in sectionGroup.Sections)
				{
					ConfigurationSection configurationSection = (ConfigurationSection)obj;
					if (!(configurationSection.SectionInformation.Name != groupName))
					{
						ClientSettingsSection clientSettingsSection = configurationSection as ClientSettingsSection;
						if (clientSettingsSection != null)
						{
							foreach (object obj2 in clientSettingsSection.Settings)
							{
								SettingElement settingElement = (SettingElement)obj2;
								this.LoadPropertyValue(collection, settingElement, allowOverwrite);
							}
							break;
						}
					}
				}
			}
		}

		public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
		{
			this.CreateExeMap();
			if (CustomizableFileSettingsProvider.UserLocalFullPath == CustomizableFileSettingsProvider.UserRoamingFullPath)
			{
				this.SaveProperties(this.exeMapCurrent, collection, ConfigurationUserLevel.PerUserRoaming, context, false);
			}
			else
			{
				this.SaveProperties(this.exeMapCurrent, collection, ConfigurationUserLevel.PerUserRoaming, context, true);
				this.SaveProperties(this.exeMapCurrent, collection, ConfigurationUserLevel.PerUserRoamingAndLocal, context, true);
			}
		}

		public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
		{
			this.CreateExeMap();
			if (this.values == null)
			{
				this.values = new SettingsPropertyValueCollection();
				string text = context["GroupName"] as string;
				this.LoadProperties(this.exeMapCurrent, collection, ConfigurationUserLevel.None, "applicationSettings", false, text);
				this.LoadProperties(this.exeMapCurrent, collection, ConfigurationUserLevel.None, "userSettings", false, text);
				this.LoadProperties(this.exeMapCurrent, collection, ConfigurationUserLevel.PerUserRoaming, "userSettings", true, text);
				this.LoadProperties(this.exeMapCurrent, collection, ConfigurationUserLevel.PerUserRoamingAndLocal, "userSettings", true, text);
				foreach (object obj in collection)
				{
					SettingsProperty settingsProperty = (SettingsProperty)obj;
					if (this.values[settingsProperty.Name] == null)
					{
						this.values.Add(new SettingsPropertyValue(settingsProperty));
					}
				}
			}
			return this.values;
		}

		private void CreateExeMap()
		{
			if (this.exeMapCurrent == null)
			{
				CustomizableFileSettingsProvider.CreateUserConfigPath();
				this.exeMapCurrent = new ExeConfigurationFileMap();
				Assembly assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
				this.exeMapCurrent.ExeConfigFilename = assembly.Location + ".config";
				this.exeMapCurrent.LocalUserConfigFilename = CustomizableFileSettingsProvider.UserLocalFullPath;
				this.exeMapCurrent.RoamingUserConfigFilename = CustomizableFileSettingsProvider.UserRoamingFullPath;
				if (CustomizableFileSettingsProvider.webConfigurationFileMapType != null && typeof(ConfigurationFileMap).IsAssignableFrom(CustomizableFileSettingsProvider.webConfigurationFileMapType))
				{
					try
					{
						ConfigurationFileMap configurationFileMap = Activator.CreateInstance(CustomizableFileSettingsProvider.webConfigurationFileMapType) as ConfigurationFileMap;
						if (configurationFileMap != null)
						{
							string machineConfigFilename = configurationFileMap.MachineConfigFilename;
							if (!string.IsNullOrEmpty(machineConfigFilename))
							{
								this.exeMapCurrent.ExeConfigFilename = machineConfigFilename;
							}
						}
					}
					catch
					{
					}
				}
				if (CustomizableFileSettingsProvider.PrevUserLocalFullPath != string.Empty && CustomizableFileSettingsProvider.PrevUserRoamingFullPath != string.Empty)
				{
					this.exeMapPrev = new ExeConfigurationFileMap();
					this.exeMapPrev.ExeConfigFilename = assembly.Location + ".config";
					this.exeMapPrev.LocalUserConfigFilename = CustomizableFileSettingsProvider.PrevUserLocalFullPath;
					this.exeMapPrev.RoamingUserConfigFilename = CustomizableFileSettingsProvider.PrevUserRoamingFullPath;
				}
			}
		}

		public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
		{
			return null;
		}

		public void Reset(SettingsContext context)
		{
			SettingsPropertyCollection settingsPropertyCollection = new SettingsPropertyCollection();
			this.GetPropertyValues(context, settingsPropertyCollection);
			foreach (object obj in this.values)
			{
				SettingsPropertyValue settingsPropertyValue = (SettingsPropertyValue)obj;
				settingsPropertyValue.PropertyValue = settingsPropertyValue.Reset();
			}
			this.SetPropertyValues(context, this.values);
		}

		public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
		{
		}

		public static void setCreate()
		{
			CustomizableFileSettingsProvider.CreateUserConfigPath();
		}

		private static Type webConfigurationFileMapType;

		private static string userRoamingPath = string.Empty;

		private static string userLocalPath = string.Empty;

		private static string userRoamingPathPrevVersion = string.Empty;

		private static string userLocalPathPrevVersion = string.Empty;

		private static string userRoamingName = "user.config";

		private static string userLocalName = "user.config";

		private static string userRoamingBasePath = string.Empty;

		private static string userLocalBasePath = string.Empty;

		private static string CompanyName = string.Empty;

		private static string ProductName = string.Empty;

		private static string ForceVersion = string.Empty;

		private static string[] ProductVersion;

		private static bool isVersionMajor;

		private static bool isVersionMinor;

		private static bool isVersionBuild;

		private static bool isVersionRevision;

		private static bool isCompany = true;

		private static bool isProduct = true;

		private static bool isEvidence;

		private static bool userDefine;

		private static UserConfigLocationOption userConfig = UserConfigLocationOption.Company_Product;

		private string app_name = string.Empty;

		private ExeConfigurationFileMap exeMapCurrent;

		private ExeConfigurationFileMap exeMapPrev;

		private SettingsPropertyValueCollection values;
	}
}
