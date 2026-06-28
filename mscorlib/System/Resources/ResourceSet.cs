using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Resources
{
	[ComVisible(true)]
	[Serializable]
	public class ResourceSet : IEnumerable, IDisposable
	{
		protected ResourceSet()
		{
			this.Table = new Hashtable();
			this.resources_read = true;
		}

		public ResourceSet(IResourceReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			this.Table = new Hashtable();
			this.Reader = reader;
		}

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public ResourceSet(Stream stream)
		{
			this.Table = new Hashtable();
			this.Reader = new ResourceReader(stream);
		}

		internal ResourceSet(UnmanagedMemoryStream stream)
		{
			this.Table = new Hashtable();
			this.Reader = new ResourceReader(stream);
		}

		public ResourceSet(string fileName)
		{
			this.Table = new Hashtable();
			this.Reader = new ResourceReader(fileName);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public virtual void Close()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.Reader != null)
			{
				this.Reader.Close();
			}
			this.Reader = null;
			this.Table = null;
			this.disposed = true;
		}

		public virtual Type GetDefaultReader()
		{
			return typeof(ResourceReader);
		}

		public virtual Type GetDefaultWriter()
		{
			return typeof(ResourceWriter);
		}

		[ComVisible(false)]
		public virtual IDictionaryEnumerator GetEnumerator()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("ResourceSet is closed.");
			}
			this.ReadResources();
			return this.Table.GetEnumerator();
		}

		private object GetObjectInternal(string name, bool ignoreCase)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (this.disposed)
			{
				throw new ObjectDisposedException("ResourceSet is closed.");
			}
			this.ReadResources();
			object obj = this.Table[name];
			if (obj != null)
			{
				return obj;
			}
			if (ignoreCase)
			{
				foreach (object obj2 in this.Table)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj2;
					string text = (string)dictionaryEntry.Key;
					if (string.Compare(text, name, true, CultureInfo.InvariantCulture) == 0)
					{
						return dictionaryEntry.Value;
					}
				}
			}
			return null;
		}

		public virtual object GetObject(string name)
		{
			return this.GetObjectInternal(name, false);
		}

		public virtual object GetObject(string name, bool ignoreCase)
		{
			return this.GetObjectInternal(name, ignoreCase);
		}

		private string GetStringInternal(string name, bool ignoreCase)
		{
			object @object = this.GetObject(name, ignoreCase);
			if (@object == null)
			{
				return null;
			}
			string text = @object as string;
			if (text == null)
			{
				throw new InvalidOperationException(string.Format("Resource '{0}' is not a String. Use GetObject instead.", name));
			}
			return text;
		}

		public virtual string GetString(string name)
		{
			return this.GetStringInternal(name, false);
		}

		public virtual string GetString(string name, bool ignoreCase)
		{
			return this.GetStringInternal(name, ignoreCase);
		}

		protected virtual void ReadResources()
		{
			if (this.resources_read)
			{
				return;
			}
			if (this.Reader == null)
			{
				throw new ObjectDisposedException("ResourceSet is closed.");
			}
			Hashtable table = this.Table;
			lock (table)
			{
				if (!this.resources_read)
				{
					IDictionaryEnumerator enumerator = this.Reader.GetEnumerator();
					enumerator.Reset();
					while (enumerator.MoveNext())
					{
						this.Table.Add(enumerator.Key, enumerator.Value);
					}
					this.resources_read = true;
				}
			}
		}

		internal UnmanagedMemoryStream GetStream(string name, bool ignoreCase)
		{
			if (this.Reader == null)
			{
				throw new ObjectDisposedException("ResourceSet is closed.");
			}
			IDictionaryEnumerator enumerator = this.Reader.GetEnumerator();
			enumerator.Reset();
			while (enumerator.MoveNext())
			{
				if (string.Compare(name, (string)enumerator.Key, ignoreCase) == 0)
				{
					return ((ResourceReader.ResourceEnumerator)enumerator).ValueAsStream;
				}
			}
			return null;
		}

		[NonSerialized]
		protected IResourceReader Reader;

		protected Hashtable Table;

		private bool resources_read;

		[NonSerialized]
		private bool disposed;
	}
}
