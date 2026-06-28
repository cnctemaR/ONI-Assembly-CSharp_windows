using System;
using System.Collections;

namespace System.Xml.Serialization
{
	public abstract class XmlMapping
	{
		internal XmlMapping()
		{
		}

		internal XmlMapping(string elementName, string ns)
		{
			this._elementName = elementName;
			this._namespace = ns;
		}

		[MonoTODO]
		public string XsdElementName
		{
			get
			{
				return this._elementName;
			}
		}

		public string ElementName
		{
			get
			{
				return this._elementName;
			}
		}

		public string Namespace
		{
			get
			{
				return this._namespace;
			}
		}

		public void SetKey(string key)
		{
			this.key = key;
		}

		internal string GetKey()
		{
			return this.key;
		}

		internal ObjectMap ObjectMap
		{
			get
			{
				return this.map;
			}
			set
			{
				this.map = value;
			}
		}

		internal ArrayList RelatedMaps
		{
			get
			{
				return this.relatedMaps;
			}
			set
			{
				this.relatedMaps = value;
			}
		}

		internal SerializationFormat Format
		{
			get
			{
				return this.format;
			}
			set
			{
				this.format = value;
			}
		}

		internal SerializationSource Source
		{
			get
			{
				return this.source;
			}
			set
			{
				this.source = value;
			}
		}

		private ObjectMap map;

		private ArrayList relatedMaps;

		private SerializationFormat format;

		private SerializationSource source;

		internal string _elementName;

		internal string _namespace;

		private string key;
	}
}
