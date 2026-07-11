using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Resources
{
	[ComVisible(true)]
	[Serializable]
	public class ResourceManager
	{
		protected ResourceManager()
		{
		}

		public ResourceManager(Type resourceSource)
		{
			if (resourceSource == null)
			{
				throw new ArgumentNullException("resourceSource");
			}
			this.resourceSource = resourceSource;
			this.BaseNameField = resourceSource.Name;
			this.MainAssembly = resourceSource.Assembly;
			this.ResourceSets = ResourceManager.GetResourceSets(this.MainAssembly, this.BaseNameField);
			this.neutral_culture = ResourceManager.GetNeutralResourcesLanguage(this.MainAssembly);
		}

		public ResourceManager(string baseName, Assembly assembly)
		{
			if (baseName == null)
			{
				throw new ArgumentNullException("baseName");
			}
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			this.BaseNameField = baseName;
			this.MainAssembly = assembly;
			this.ResourceSets = ResourceManager.GetResourceSets(this.MainAssembly, this.BaseNameField);
			this.neutral_culture = ResourceManager.GetNeutralResourcesLanguage(this.MainAssembly);
		}

		public ResourceManager(string baseName, Assembly assembly, Type usingResourceSet)
		{
			if (baseName == null)
			{
				throw new ArgumentNullException("baseName");
			}
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			this.BaseNameField = baseName;
			this.MainAssembly = assembly;
			this.ResourceSets = ResourceManager.GetResourceSets(this.MainAssembly, this.BaseNameField);
			this.resourceSetType = this.CheckResourceSetType(usingResourceSet, true);
			this.neutral_culture = ResourceManager.GetNeutralResourcesLanguage(this.MainAssembly);
		}

		private ResourceManager(string baseName, string resourceDir, Type usingResourceSet)
		{
			if (baseName == null)
			{
				throw new ArgumentNullException("baseName");
			}
			if (resourceDir == null)
			{
				throw new ArgumentNullException("resourceDir");
			}
			this.BaseNameField = baseName;
			this.resourceDir = resourceDir;
			this.resourceSetType = this.CheckResourceSetType(usingResourceSet, false);
			this.ResourceSets = ResourceManager.GetResourceSets(this.MainAssembly, this.BaseNameField);
		}

		private static Hashtable GetResourceSets(Assembly assembly, string basename)
		{
			Hashtable resourceCache = ResourceManager.ResourceCache;
			Hashtable hashtable2;
			lock (resourceCache)
			{
				string text = string.Empty;
				if (assembly != null)
				{
					text = assembly.FullName;
				}
				else
				{
					text = basename.GetHashCode().ToString() + "@@";
				}
				if (basename != null && basename != string.Empty)
				{
					text = text + "!" + basename;
				}
				else
				{
					text = text + "!" + text.GetHashCode();
				}
				Hashtable hashtable = ResourceManager.ResourceCache[text] as Hashtable;
				if (hashtable == null)
				{
					hashtable = Hashtable.Synchronized(new Hashtable());
					ResourceManager.ResourceCache[text] = hashtable;
				}
				hashtable2 = hashtable;
			}
			return hashtable2;
		}

		private Type CheckResourceSetType(Type usingResourceSet, bool verifyType)
		{
			if (usingResourceSet == null)
			{
				return this.resourceSetType;
			}
			if (verifyType && !typeof(ResourceSet).IsAssignableFrom(usingResourceSet))
			{
				throw new ArgumentException("Type parameter must refer to a subclass of ResourceSet.", "usingResourceSet");
			}
			return usingResourceSet;
		}

		public static ResourceManager CreateFileBasedResourceManager(string baseName, string resourceDir, Type usingResourceSet)
		{
			return new ResourceManager(baseName, resourceDir, usingResourceSet);
		}

		public virtual string BaseName
		{
			get
			{
				return this.BaseNameField;
			}
		}

		public virtual bool IgnoreCase
		{
			get
			{
				return this.ignoreCase;
			}
			set
			{
				this.ignoreCase = value;
			}
		}

		public virtual Type ResourceSetType
		{
			get
			{
				return this.resourceSetType;
			}
		}

		public virtual object GetObject(string name)
		{
			return this.GetObject(name, null);
		}

		public virtual object GetObject(string name, CultureInfo culture)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (culture == null)
			{
				culture = CultureInfo.CurrentUICulture;
			}
			lock (this)
			{
				ResourceSet resourceSet = this.InternalGetResourceSet(culture, true, true);
				object obj;
				if (resourceSet != null)
				{
					obj = resourceSet.GetObject(name, this.ignoreCase);
					if (obj != null)
					{
						return obj;
					}
				}
				for (;;)
				{
					culture = culture.Parent;
					resourceSet = this.InternalGetResourceSet(culture, true, true);
					if (resourceSet != null)
					{
						obj = resourceSet.GetObject(name, this.ignoreCase);
						if (obj != null)
						{
							break;
						}
					}
					if (culture.Equals(this.neutral_culture) || culture.Equals(CultureInfo.InvariantCulture))
					{
						goto IL_00A7;
					}
				}
				return obj;
				IL_00A7:;
			}
			return null;
		}

		public virtual ResourceSet GetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			ResourceSet resourceSet;
			lock (this)
			{
				resourceSet = this.InternalGetResourceSet(culture, createIfNotExists, tryParents);
			}
			return resourceSet;
		}

		public virtual string GetString(string name)
		{
			return this.GetString(name, null);
		}

		public virtual string GetString(string name, CultureInfo culture)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (culture == null)
			{
				culture = CultureInfo.CurrentUICulture;
			}
			lock (this)
			{
				ResourceSet resourceSet = this.InternalGetResourceSet(culture, true, true);
				string text;
				if (resourceSet != null)
				{
					text = resourceSet.GetString(name, this.ignoreCase);
					if (text != null)
					{
						return text;
					}
				}
				for (;;)
				{
					culture = culture.Parent;
					resourceSet = this.InternalGetResourceSet(culture, true, true);
					if (resourceSet != null)
					{
						text = resourceSet.GetString(name, this.ignoreCase);
						if (text != null)
						{
							break;
						}
					}
					if (culture.Equals(this.neutral_culture) || culture.Equals(CultureInfo.InvariantCulture))
					{
						goto IL_00A7;
					}
				}
				return text;
				IL_00A7:;
			}
			return null;
		}

		protected virtual string GetResourceFileName(CultureInfo culture)
		{
			if (culture.Equals(CultureInfo.InvariantCulture))
			{
				return this.BaseNameField + ".resources";
			}
			return this.BaseNameField + "." + culture.Name + ".resources";
		}

		private string GetResourceFilePath(CultureInfo culture)
		{
			if (this.resourceDir != null)
			{
				return Path.Combine(this.resourceDir, this.GetResourceFileName(culture));
			}
			return this.GetResourceFileName(culture);
		}

		private Stream GetManifestResourceStreamNoCase(Assembly ass, string fn)
		{
			string manifestResourceName = this.GetManifestResourceName(fn);
			foreach (string text in ass.GetManifestResourceNames())
			{
				if (string.Compare(manifestResourceName, text, true, CultureInfo.InvariantCulture) == 0)
				{
					return ass.GetManifestResourceStream(text);
				}
			}
			return null;
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public UnmanagedMemoryStream GetStream(string name)
		{
			return this.GetStream(name, null);
		}

		[CLSCompliant(false)]
		[ComVisible(false)]
		public UnmanagedMemoryStream GetStream(string name, CultureInfo culture)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (culture == null)
			{
				culture = CultureInfo.CurrentUICulture;
			}
			ResourceSet resourceSet = this.InternalGetResourceSet(culture, true, true);
			return resourceSet.GetStream(name, this.ignoreCase);
		}

		protected virtual ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("key");
			}
			ResourceSet resourceSet = (ResourceSet)this.ResourceSets[culture];
			if (resourceSet != null)
			{
				return resourceSet;
			}
			if (ResourceManager.NonExistent.Contains(culture))
			{
				return null;
			}
			if (this.MainAssembly != null)
			{
				CultureInfo cultureInfo = culture;
				if (culture.Equals(this.neutral_culture))
				{
					cultureInfo = CultureInfo.InvariantCulture;
				}
				Stream stream = null;
				string resourceFileName = this.GetResourceFileName(cultureInfo);
				if (!cultureInfo.Equals(CultureInfo.InvariantCulture))
				{
					Version satelliteContractVersion = ResourceManager.GetSatelliteContractVersion(this.MainAssembly);
					try
					{
						Assembly satelliteAssemblyNoThrow = this.MainAssembly.GetSatelliteAssemblyNoThrow(cultureInfo, satelliteContractVersion);
						if (satelliteAssemblyNoThrow != null)
						{
							stream = satelliteAssemblyNoThrow.GetManifestResourceStream(resourceFileName);
							if (stream == null)
							{
								stream = this.GetManifestResourceStreamNoCase(satelliteAssemblyNoThrow, resourceFileName);
							}
						}
					}
					catch (Exception)
					{
					}
				}
				else
				{
					stream = this.MainAssembly.GetManifestResourceStream(this.resourceSource, resourceFileName);
					if (stream == null)
					{
						stream = this.GetManifestResourceStreamNoCase(this.MainAssembly, resourceFileName);
					}
				}
				if (stream != null && createIfNotExists)
				{
					object[] array = new object[] { stream };
					resourceSet = (ResourceSet)Activator.CreateInstance(this.resourceSetType, array);
				}
				else if (cultureInfo.Equals(CultureInfo.InvariantCulture))
				{
					throw this.AssemblyResourceMissing(resourceFileName);
				}
			}
			else if (this.resourceDir != null || this.BaseNameField != null)
			{
				string resourceFilePath = this.GetResourceFilePath(culture);
				if (createIfNotExists && File.Exists(resourceFilePath))
				{
					object[] array2 = new object[] { resourceFilePath };
					resourceSet = (ResourceSet)Activator.CreateInstance(this.resourceSetType, array2);
				}
				else if (culture.Equals(CultureInfo.InvariantCulture))
				{
					string text = string.Format("Could not find any resources appropriate for the specified culture (or the neutral culture) on disk.{0}baseName: {1}  locationInfo: {2}  fileName: {3}", new object[]
					{
						Environment.NewLine,
						this.BaseNameField,
						"<null>",
						this.GetResourceFileName(culture)
					});
					throw new MissingManifestResourceException(text);
				}
			}
			if (resourceSet == null && tryParents && !culture.Equals(CultureInfo.InvariantCulture))
			{
				resourceSet = this.InternalGetResourceSet(culture.Parent, createIfNotExists, tryParents);
			}
			if (resourceSet != null)
			{
				this.ResourceSets[culture] = resourceSet;
			}
			else
			{
				ResourceManager.NonExistent[culture] = culture;
			}
			return resourceSet;
		}

		public virtual void ReleaseAllResources()
		{
			lock (this)
			{
				foreach (object obj in this.ResourceSets.Values)
				{
					ResourceSet resourceSet = (ResourceSet)obj;
					resourceSet.Close();
				}
				this.ResourceSets.Clear();
			}
		}

		protected static CultureInfo GetNeutralResourcesLanguage(Assembly a)
		{
			object[] customAttributes = a.GetCustomAttributes(typeof(NeutralResourcesLanguageAttribute), false);
			if (customAttributes.Length == 0)
			{
				return CultureInfo.InvariantCulture;
			}
			NeutralResourcesLanguageAttribute neutralResourcesLanguageAttribute = (NeutralResourcesLanguageAttribute)customAttributes[0];
			return new CultureInfo(neutralResourcesLanguageAttribute.CultureName);
		}

		protected static Version GetSatelliteContractVersion(Assembly a)
		{
			object[] customAttributes = a.GetCustomAttributes(typeof(SatelliteContractVersionAttribute), false);
			if (customAttributes.Length == 0)
			{
				return null;
			}
			SatelliteContractVersionAttribute satelliteContractVersionAttribute = (SatelliteContractVersionAttribute)customAttributes[0];
			return new Version(satelliteContractVersionAttribute.Version);
		}

		[MonoTODO("the property exists but is not respected")]
		protected UltimateResourceFallbackLocation FallbackLocation
		{
			get
			{
				return this.fallbackLocation;
			}
			set
			{
				this.fallbackLocation = value;
			}
		}

		private MissingManifestResourceException AssemblyResourceMissing(string fileName)
		{
			AssemblyName assemblyName = ((this.MainAssembly == null) ? null : this.MainAssembly.GetName());
			string manifestResourceName = this.GetManifestResourceName(fileName);
			string text = string.Format("Could not find any resources appropriate for the specified culture or the neutral culture.  Make sure \"{0}\" was correctly embedded or linked into assembly \"{1}\" at compile time, or that all the satellite assemblies required are loadable and fully signed.", manifestResourceName, (assemblyName == null) ? string.Empty : assemblyName.Name);
			throw new MissingManifestResourceException(text);
		}

		private string GetManifestResourceName(string fn)
		{
			string text;
			if (this.resourceSource != null)
			{
				if (this.resourceSource.Namespace != null && this.resourceSource.Namespace.Length > 0)
				{
					text = this.resourceSource.Namespace + "." + fn;
				}
				else
				{
					text = fn;
				}
			}
			else
			{
				text = fn;
			}
			return text;
		}

		private static Hashtable ResourceCache = new Hashtable();

		private static Hashtable NonExistent = Hashtable.Synchronized(new Hashtable());

		public static readonly int HeaderVersionNumber = 1;

		public static readonly int MagicNumber = -1091581234;

		protected string BaseNameField;

		protected Assembly MainAssembly;

		protected Hashtable ResourceSets;

		private bool ignoreCase;

		private Type resourceSource;

		private Type resourceSetType = typeof(RuntimeResourceSet);

		private string resourceDir;

		private CultureInfo neutral_culture;

		private UltimateResourceFallbackLocation fallbackLocation;
	}
}
