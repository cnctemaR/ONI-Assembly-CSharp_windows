using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace System.Xml.Resolvers
{
	public class XmlPreloadedResolver : XmlResolver
	{
		public XmlPreloadedResolver()
			: this(null)
		{
		}

		public XmlPreloadedResolver(XmlKnownDtds preloadedDtds)
			: this(null, preloadedDtds, null)
		{
		}

		public XmlPreloadedResolver(XmlResolver fallbackResolver)
			: this(fallbackResolver, XmlKnownDtds.All, null)
		{
		}

		public XmlPreloadedResolver(XmlResolver fallbackResolver, XmlKnownDtds preloadedDtds)
			: this(fallbackResolver, preloadedDtds, null)
		{
		}

		public XmlPreloadedResolver(XmlResolver fallbackResolver, XmlKnownDtds preloadedDtds, IEqualityComparer<Uri> uriComparer)
		{
			this._fallbackResolver = fallbackResolver;
			this._mappings = new Dictionary<Uri, XmlPreloadedResolver.PreloadedData>(16, uriComparer);
			this._preloadedDtds = preloadedDtds;
			if (preloadedDtds != XmlKnownDtds.None)
			{
				if ((preloadedDtds & XmlKnownDtds.Xhtml10) != XmlKnownDtds.None)
				{
					this.AddKnownDtd(XmlPreloadedResolver.s_xhtml10_Dtd);
				}
				if ((preloadedDtds & XmlKnownDtds.Rss091) != XmlKnownDtds.None)
				{
					this.AddKnownDtd(XmlPreloadedResolver.s_rss091_Dtd);
				}
			}
		}

		public override Uri ResolveUri(Uri baseUri, string relativeUri)
		{
			if (relativeUri != null && relativeUri.StartsWith("-//", StringComparison.CurrentCulture))
			{
				if ((this._preloadedDtds & XmlKnownDtds.Xhtml10) != XmlKnownDtds.None && relativeUri.StartsWith("-//W3C//", StringComparison.CurrentCulture))
				{
					for (int i = 0; i < XmlPreloadedResolver.s_xhtml10_Dtd.Length; i++)
					{
						if (relativeUri == XmlPreloadedResolver.s_xhtml10_Dtd[i].publicId)
						{
							return new Uri(relativeUri, UriKind.Relative);
						}
					}
				}
				if ((this._preloadedDtds & XmlKnownDtds.Rss091) != XmlKnownDtds.None && relativeUri == XmlPreloadedResolver.s_rss091_Dtd[0].publicId)
				{
					return new Uri(relativeUri, UriKind.Relative);
				}
			}
			return base.ResolveUri(baseUri, relativeUri);
		}

		public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
		{
			if (absoluteUri == null)
			{
				throw new ArgumentNullException("absoluteUri");
			}
			XmlPreloadedResolver.PreloadedData preloadedData;
			if (!this._mappings.TryGetValue(absoluteUri, out preloadedData))
			{
				if (this._fallbackResolver != null)
				{
					return this._fallbackResolver.GetEntity(absoluteUri, role, ofObjectToReturn);
				}
				throw new XmlException(SR.Format("Cannot resolve '{0}'.", absoluteUri.ToString()));
			}
			else
			{
				if (ofObjectToReturn == null || ofObjectToReturn == typeof(Stream) || ofObjectToReturn == typeof(object))
				{
					return preloadedData.AsStream();
				}
				if (ofObjectToReturn == typeof(TextReader))
				{
					return preloadedData.AsTextReader();
				}
				throw new XmlException("Object type is not supported.");
			}
		}

		public override ICredentials Credentials
		{
			set
			{
				if (this._fallbackResolver != null)
				{
					this._fallbackResolver.Credentials = value;
				}
			}
		}

		public override bool SupportsType(Uri absoluteUri, Type type)
		{
			if (absoluteUri == null)
			{
				throw new ArgumentNullException("absoluteUri");
			}
			XmlPreloadedResolver.PreloadedData preloadedData;
			if (this._mappings.TryGetValue(absoluteUri, out preloadedData))
			{
				return preloadedData.SupportsType(type);
			}
			if (this._fallbackResolver != null)
			{
				return this._fallbackResolver.SupportsType(absoluteUri, type);
			}
			return base.SupportsType(absoluteUri, type);
		}

		public void Add(Uri uri, byte[] value)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.Add(uri, new XmlPreloadedResolver.ByteArrayChunk(value, 0, value.Length));
		}

		public void Add(Uri uri, byte[] value, int offset, int count)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (value.Length - offset < count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			this.Add(uri, new XmlPreloadedResolver.ByteArrayChunk(value, offset, count));
		}

		public void Add(Uri uri, Stream value)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			checked
			{
				if (value.CanSeek)
				{
					int num = (int)value.Length;
					byte[] array = new byte[num];
					value.Read(array, 0, num);
					this.Add(uri, new XmlPreloadedResolver.ByteArrayChunk(array));
					return;
				}
				MemoryStream memoryStream = new MemoryStream();
				byte[] array2 = new byte[4096];
				int num2;
				while ((num2 = value.Read(array2, 0, array2.Length)) > 0)
				{
					memoryStream.Write(array2, 0, num2);
				}
				int num3 = (int)memoryStream.Position;
				byte[] array3 = new byte[num3];
				Array.Copy(memoryStream.ToArray(), array3, num3);
				this.Add(uri, new XmlPreloadedResolver.ByteArrayChunk(array3));
			}
		}

		public void Add(Uri uri, string value)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.Add(uri, new XmlPreloadedResolver.StringData(value));
		}

		public IEnumerable<Uri> PreloadedUris
		{
			get
			{
				return this._mappings.Keys;
			}
		}

		public void Remove(Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			this._mappings.Remove(uri);
		}

		private void Add(Uri uri, XmlPreloadedResolver.PreloadedData data)
		{
			if (this._mappings.ContainsKey(uri))
			{
				this._mappings[uri] = data;
				return;
			}
			this._mappings.Add(uri, data);
		}

		private void AddKnownDtd(XmlPreloadedResolver.XmlKnownDtdData[] dtdSet)
		{
			foreach (XmlPreloadedResolver.XmlKnownDtdData xmlKnownDtdData in dtdSet)
			{
				this._mappings.Add(new Uri(xmlKnownDtdData.publicId, UriKind.RelativeOrAbsolute), xmlKnownDtdData);
				this._mappings.Add(new Uri(xmlKnownDtdData.systemId, UriKind.RelativeOrAbsolute), xmlKnownDtdData);
			}
		}

		public override Task<object> GetEntityAsync(Uri absoluteUri, string role, Type ofObjectToReturn)
		{
			if (absoluteUri == null)
			{
				throw new ArgumentNullException("absoluteUri");
			}
			XmlPreloadedResolver.PreloadedData preloadedData;
			if (!this._mappings.TryGetValue(absoluteUri, out preloadedData))
			{
				if (this._fallbackResolver != null)
				{
					return this._fallbackResolver.GetEntityAsync(absoluteUri, role, ofObjectToReturn);
				}
				throw new XmlException(SR.Format("Cannot resolve '{0}'.", absoluteUri.ToString()));
			}
			else
			{
				if (ofObjectToReturn == null || ofObjectToReturn == typeof(Stream) || ofObjectToReturn == typeof(object))
				{
					return Task.FromResult<object>(preloadedData.AsStream());
				}
				if (ofObjectToReturn == typeof(TextReader))
				{
					return Task.FromResult<object>(preloadedData.AsTextReader());
				}
				throw new XmlException("Object type is not supported.");
			}
		}

		private XmlResolver _fallbackResolver;

		private Dictionary<Uri, XmlPreloadedResolver.PreloadedData> _mappings;

		private XmlKnownDtds _preloadedDtds;

		private static XmlPreloadedResolver.XmlKnownDtdData[] s_xhtml10_Dtd = new XmlPreloadedResolver.XmlKnownDtdData[]
		{
			new XmlPreloadedResolver.XmlKnownDtdData("-//W3C//DTD XHTML 1.0 Strict//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd", "xhtml1-strict.dtd"),
			new XmlPreloadedResolver.XmlKnownDtdData("-//W3C//DTD XHTML 1.0 Transitional//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd", "xhtml1-transitional.dtd"),
			new XmlPreloadedResolver.XmlKnownDtdData("-//W3C//DTD XHTML 1.0 Frameset//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd", "xhtml1-frameset.dtd"),
			new XmlPreloadedResolver.XmlKnownDtdData("-//W3C//ENTITIES Latin 1 for XHTML//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml-lat1.ent", "xhtml-lat1.ent"),
			new XmlPreloadedResolver.XmlKnownDtdData("-//W3C//ENTITIES Symbols for XHTML//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml-symbol.ent", "xhtml-symbol.ent"),
			new XmlPreloadedResolver.XmlKnownDtdData("-//W3C//ENTITIES Special for XHTML//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml-special.ent", "xhtml-special.ent")
		};

		private static XmlPreloadedResolver.XmlKnownDtdData[] s_rss091_Dtd = new XmlPreloadedResolver.XmlKnownDtdData[]
		{
			new XmlPreloadedResolver.XmlKnownDtdData("-//Netscape Communications//DTD RSS 0.91//EN", "http://my.netscape.com/publish/formats/rss-0.91.dtd", "rss-0.91.dtd")
		};

		private abstract class PreloadedData
		{
			internal abstract Stream AsStream();

			internal virtual TextReader AsTextReader()
			{
				throw new XmlException("Object type is not supported.");
			}

			internal virtual bool SupportsType(Type type)
			{
				return type == null || type == typeof(Stream);
			}
		}

		private class XmlKnownDtdData : XmlPreloadedResolver.PreloadedData
		{
			internal XmlKnownDtdData(string publicId, string systemId, string resourceName)
			{
				this.publicId = publicId;
				this.systemId = systemId;
				this._resourceName = resourceName;
			}

			internal override Stream AsStream()
			{
				return base.GetType().Assembly.GetManifestResourceStream(this._resourceName);
			}

			internal string publicId;

			internal string systemId;

			private string _resourceName;
		}

		private class ByteArrayChunk : XmlPreloadedResolver.PreloadedData
		{
			internal ByteArrayChunk(byte[] array)
				: this(array, 0, array.Length)
			{
			}

			internal ByteArrayChunk(byte[] array, int offset, int length)
			{
				this._array = array;
				this._offset = offset;
				this._length = length;
			}

			internal override Stream AsStream()
			{
				return new MemoryStream(this._array, this._offset, this._length);
			}

			private byte[] _array;

			private int _offset;

			private int _length;
		}

		private class StringData : XmlPreloadedResolver.PreloadedData
		{
			internal StringData(string str)
			{
				this._str = str;
			}

			internal override Stream AsStream()
			{
				return new MemoryStream(Encoding.Unicode.GetBytes(this._str));
			}

			internal override TextReader AsTextReader()
			{
				return new StringReader(this._str);
			}

			internal override bool SupportsType(Type type)
			{
				return type == typeof(TextReader) || base.SupportsType(type);
			}

			private string _str;
		}
	}
}
