using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Xml.Serialization.Advanced
{
	public class SchemaImporterExtensionCollection : CollectionBase
	{
		public int Add(SchemaImporterExtension extension)
		{
			if (extension == null)
			{
				throw new ArgumentNullException("extension");
			}
			return base.List.Add(extension);
		}

		public int Add(string key, Type type)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (!type.IsSubclassOf(typeof(SchemaImporterExtension)))
			{
				throw new ArgumentException("The type argument must be subclass of SchemaImporterExtension.");
			}
			SchemaImporterExtension schemaImporterExtension = (SchemaImporterExtension)Activator.CreateInstance(type);
			if (this.named_items.ContainsKey(key))
			{
				throw new InvalidOperationException(string.Format("A SchemaImporterExtension keyed by '{0}' already exists.", key));
			}
			int num = this.Add(schemaImporterExtension);
			this.named_items.Add(key, schemaImporterExtension);
			return num;
		}

		public new void Clear()
		{
			this.named_items.Clear();
			base.List.Clear();
		}

		public bool Contains(SchemaImporterExtension extension)
		{
			if (extension == null)
			{
				throw new ArgumentNullException("extension");
			}
			foreach (object obj in base.List)
			{
				SchemaImporterExtension schemaImporterExtension = (SchemaImporterExtension)obj;
				if (extension.Equals(schemaImporterExtension))
				{
					return true;
				}
			}
			return false;
		}

		public void CopyTo(SchemaImporterExtension[] array, int index)
		{
			base.List.CopyTo(array, index);
		}

		public int IndexOf(SchemaImporterExtension extension)
		{
			if (extension == null)
			{
				throw new ArgumentNullException("extension");
			}
			int num = 0;
			foreach (object obj in base.List)
			{
				SchemaImporterExtension schemaImporterExtension = (SchemaImporterExtension)obj;
				if (extension.Equals(schemaImporterExtension))
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		public void Insert(int index, SchemaImporterExtension extension)
		{
			if (extension == null)
			{
				throw new ArgumentNullException("extension");
			}
			base.List.Insert(index, extension);
		}

		public void Remove(SchemaImporterExtension extension)
		{
			int num = this.IndexOf(extension);
			if (num >= 0)
			{
				base.List.RemoveAt(num);
			}
		}

		public void Remove(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (!this.named_items.ContainsKey(name))
			{
				return;
			}
			SchemaImporterExtension schemaImporterExtension = this.named_items[name];
			this.Remove(schemaImporterExtension);
			this.named_items.Remove(name);
		}

		public SchemaImporterExtension this[int index]
		{
			get
			{
				return (SchemaImporterExtension)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		private Dictionary<string, SchemaImporterExtension> named_items = new Dictionary<string, SchemaImporterExtension>();
	}
}
