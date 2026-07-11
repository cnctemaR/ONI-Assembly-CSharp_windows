using System;
using System.Collections;
using System.Collections.Specialized;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	internal class SchemaObjectCache
	{
		private Hashtable Graph
		{
			get
			{
				if (this.graph == null)
				{
					this.graph = new Hashtable();
				}
				return this.graph;
			}
		}

		private Hashtable Hash
		{
			get
			{
				if (this.hash == null)
				{
					this.hash = new Hashtable();
				}
				return this.hash;
			}
		}

		private Hashtable ObjectCache
		{
			get
			{
				if (this.objectCache == null)
				{
					this.objectCache = new Hashtable();
				}
				return this.objectCache;
			}
		}

		internal StringCollection Warnings
		{
			get
			{
				if (this.warnings == null)
				{
					this.warnings = new StringCollection();
				}
				return this.warnings;
			}
		}

		internal XmlSchemaObject AddItem(XmlSchemaObject item, XmlQualifiedName qname, XmlSchemas schemas)
		{
			if (item == null)
			{
				return null;
			}
			if (qname == null || qname.IsEmpty)
			{
				return null;
			}
			string text = item.GetType().Name + ":" + qname.ToString();
			ArrayList arrayList = (ArrayList)this.ObjectCache[text];
			if (arrayList == null)
			{
				arrayList = new ArrayList();
				this.ObjectCache[text] = arrayList;
			}
			for (int i = 0; i < arrayList.Count; i++)
			{
				XmlSchemaObject xmlSchemaObject = (XmlSchemaObject)arrayList[i];
				if (xmlSchemaObject == item)
				{
					return xmlSchemaObject;
				}
				if (this.Match(xmlSchemaObject, item, true))
				{
					return xmlSchemaObject;
				}
				this.Warnings.Add(Res.GetString("Warning: Cannot share {0} named '{1}' from '{2}' namespace. Several mismatched schema declarations were found.", new object[]
				{
					item.GetType().Name,
					qname.Name,
					qname.Namespace
				}));
				this.Warnings.Add("DEBUG:Cached item key:\r\n" + (string)this.looks[xmlSchemaObject] + "\r\nnew item key:\r\n" + (string)this.looks[item]);
			}
			arrayList.Add(item);
			return item;
		}

		internal bool Match(XmlSchemaObject o1, XmlSchemaObject o2, bool shareTypes)
		{
			if (o1 == o2)
			{
				return true;
			}
			if (o1.GetType() != o2.GetType())
			{
				return false;
			}
			if (this.Hash[o1] == null)
			{
				this.Hash[o1] = this.GetHash(o1);
			}
			int num = (int)this.Hash[o1];
			int num2 = this.GetHash(o2);
			return num == num2 && (!shareTypes || this.CompositeHash(o1, num) == this.CompositeHash(o2, num2));
		}

		private ArrayList GetDependencies(XmlSchemaObject o, ArrayList deps, Hashtable refs)
		{
			if (refs[o] == null)
			{
				refs[o] = o;
				deps.Add(o);
				ArrayList arrayList = this.Graph[o] as ArrayList;
				if (arrayList != null)
				{
					for (int i = 0; i < arrayList.Count; i++)
					{
						this.GetDependencies((XmlSchemaObject)arrayList[i], deps, refs);
					}
				}
			}
			return deps;
		}

		private int CompositeHash(XmlSchemaObject o, int hash)
		{
			ArrayList dependencies = this.GetDependencies(o, new ArrayList(), new Hashtable());
			double num = 0.0;
			for (int i = 0; i < dependencies.Count; i++)
			{
				object obj = this.Hash[dependencies[i]];
				if (obj is int)
				{
					num += (double)((int)obj / dependencies.Count);
				}
			}
			return (int)num;
		}

		internal void GenerateSchemaGraph(XmlSchemas schemas)
		{
			ArrayList items = new SchemaGraph(this.Graph, schemas).GetItems();
			for (int i = 0; i < items.Count; i++)
			{
				this.GetHash((XmlSchemaObject)items[i]);
			}
		}

		private int GetHash(XmlSchemaObject o)
		{
			object obj = this.Hash[o];
			if (obj != null && !(obj is XmlSchemaObject))
			{
				return (int)obj;
			}
			string text = this.ToString(o, new SchemaObjectWriter());
			this.looks[o] = text;
			int hashCode = text.GetHashCode();
			this.Hash[o] = hashCode;
			return hashCode;
		}

		private string ToString(XmlSchemaObject o, SchemaObjectWriter writer)
		{
			return writer.WriteXmlSchemaObject(o);
		}

		private Hashtable graph;

		private Hashtable hash;

		private Hashtable objectCache;

		private StringCollection warnings;

		internal Hashtable looks = new Hashtable();
	}
}
