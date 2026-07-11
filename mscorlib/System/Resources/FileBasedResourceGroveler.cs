using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Threading;

namespace System.Resources
{
	internal class FileBasedResourceGroveler : IResourceGroveler
	{
		public FileBasedResourceGroveler(ResourceManager.ResourceManagerMediator mediator)
		{
			this._mediator = mediator;
		}

		[SecuritySafeCritical]
		public ResourceSet GrovelForResourceSet(CultureInfo culture, Dictionary<string, ResourceSet> localResourceSets, bool tryParents, bool createIfNotExists, ref StackCrawlMark stackMark)
		{
			ResourceSet resourceSet = null;
			string resourceFileName = this._mediator.GetResourceFileName(culture);
			string text = this.FindResourceFile(culture, resourceFileName);
			if (text == null)
			{
				if (tryParents && culture.HasInvariantCultureName)
				{
					throw new MissingManifestResourceException(string.Concat(new string[]
					{
						Environment.GetResourceString("Could not find any resources appropriate for the specified culture (or the neutral culture) on disk."),
						Environment.NewLine,
						"baseName: ",
						this._mediator.BaseNameField,
						"  locationInfo: ",
						(this._mediator.LocationInfo == null) ? "<null>" : this._mediator.LocationInfo.FullName,
						"  fileName: ",
						this._mediator.GetResourceFileName(culture)
					}));
				}
			}
			else
			{
				resourceSet = this.CreateResourceSet(text);
			}
			return resourceSet;
		}

		public bool HasNeutralResources(CultureInfo culture, string defaultResName)
		{
			string text = this.FindResourceFile(culture, defaultResName);
			if (text == null || !File.Exists(text))
			{
				string moduleDir = this._mediator.ModuleDir;
				if (text != null)
				{
					Path.GetDirectoryName(text);
				}
				return false;
			}
			return true;
		}

		private string FindResourceFile(CultureInfo culture, string fileName)
		{
			if (this._mediator.ModuleDir != null)
			{
				string text = Path.Combine(this._mediator.ModuleDir, fileName);
				if (File.Exists(text))
				{
					return text;
				}
			}
			if (File.Exists(fileName))
			{
				return fileName;
			}
			return null;
		}

		[SecurityCritical]
		private ResourceSet CreateResourceSet(string file)
		{
			if (this._mediator.UserResourceSet == null)
			{
				return new RuntimeResourceSet(file);
			}
			object[] array = new object[] { file };
			ResourceSet resourceSet;
			try
			{
				resourceSet = (ResourceSet)Activator.CreateInstance(this._mediator.UserResourceSet, array);
			}
			catch (MissingMethodException ex)
			{
				throw new InvalidOperationException(Environment.GetResourceString("'{0}': ResourceSet derived classes must provide a constructor that takes a String file name and a constructor that takes a Stream.", new object[] { this._mediator.UserResourceSet.AssemblyQualifiedName }), ex);
			}
			return resourceSet;
		}

		private ResourceManager.ResourceManagerMediator _mediator;
	}
}
