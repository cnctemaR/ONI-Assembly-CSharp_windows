using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.ComponentModel.Design
{
	internal class RuntimeLicenseContext : LicenseContext
	{
		private void LoadKeys()
		{
			if (this.keys != null)
			{
				return;
			}
			this.keys = new Hashtable();
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			if (entryAssembly != null)
			{
				this.LoadAssemblyLicenses(this.keys, entryAssembly);
			}
			else
			{
				foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					this.LoadAssemblyLicenses(this.keys, assembly);
				}
			}
		}

		private void LoadAssemblyLicenses(Hashtable targetkeys, Assembly asm)
		{
			if (asm is AssemblyBuilder)
			{
				return;
			}
			string fileName = Path.GetFileName(asm.Location);
			string text = fileName + ".licenses";
			try
			{
				foreach (string text2 in asm.GetManifestResourceNames())
				{
					if (!(text2 != text))
					{
						using (Stream manifestResourceStream = asm.GetManifestResourceStream(text2))
						{
							BinaryFormatter binaryFormatter = new BinaryFormatter();
							object[] array = binaryFormatter.Deserialize(manifestResourceStream) as object[];
							if (string.Compare((string)array[0], fileName, true) == 0)
							{
								Hashtable hashtable = (Hashtable)array[1];
								foreach (object obj in hashtable)
								{
									DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
									targetkeys.Add(dictionaryEntry.Key, dictionaryEntry.Value);
								}
							}
						}
					}
				}
			}
			catch (InvalidCastException)
			{
			}
		}

		public override string GetSavedLicenseKey(Type type, Assembly resourceAssembly)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (resourceAssembly != null)
			{
				if (this.extraassemblies == null)
				{
					this.extraassemblies = new Hashtable();
				}
				Hashtable hashtable = this.extraassemblies[resourceAssembly.FullName] as Hashtable;
				if (hashtable == null)
				{
					hashtable = new Hashtable();
					this.LoadAssemblyLicenses(hashtable, resourceAssembly);
					this.extraassemblies[resourceAssembly.FullName] = hashtable;
				}
				return (string)hashtable[type.AssemblyQualifiedName];
			}
			this.LoadKeys();
			return (string)this.keys[type.AssemblyQualifiedName];
		}

		public override void SetSavedLicenseKey(Type type, string key)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			this.LoadKeys();
			this.keys[type.AssemblyQualifiedName] = key;
		}

		private Hashtable extraassemblies;

		private Hashtable keys;
	}
}
