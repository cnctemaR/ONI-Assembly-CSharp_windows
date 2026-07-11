using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
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
			this.fallbackResolver = fallbackResolver;
			this.mappings = new Dictionary<Uri, XmlPreloadedResolver.PreloadedData>(16, uriComparer);
			this.preloadedDtds = preloadedDtds;
			if (preloadedDtds != XmlKnownDtds.None)
			{
				if ((preloadedDtds & XmlKnownDtds.Xhtml10) != XmlKnownDtds.None)
				{
					this.AddKnownDtd(XmlPreloadedResolver.Xhtml10_Dtd);
				}
				if ((preloadedDtds & XmlKnownDtds.Rss091) != XmlKnownDtds.None)
				{
					this.AddKnownDtd(XmlPreloadedResolver.Rss091_Dtd);
				}
			}
		}

		public override Uri ResolveUri(Uri baseUri, string relativeUri)
		{
			if (relativeUri != null && relativeUri.StartsWith("-//", StringComparison.CurrentCulture))
			{
				if ((this.preloadedDtds & XmlKnownDtds.Xhtml10) != XmlKnownDtds.None && relativeUri.StartsWith("-//W3C//", StringComparison.CurrentCulture))
				{
					for (int i = 0; i < XmlPreloadedResolver.Xhtml10_Dtd.Length; i++)
					{
						if (relativeUri == XmlPreloadedResolver.Xhtml10_Dtd[i].publicId)
						{
							return new Uri(relativeUri, UriKind.Relative);
						}
					}
				}
				if ((this.preloadedDtds & XmlKnownDtds.Rss091) != XmlKnownDtds.None && relativeUri == XmlPreloadedResolver.Rss091_Dtd[0].publicId)
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
			if (!this.mappings.TryGetValue(absoluteUri, out preloadedData))
			{
				if (this.fallbackResolver != null)
				{
					return this.fallbackResolver.GetEntity(absoluteUri, role, ofObjectToReturn);
				}
				throw new XmlException(Res.GetString("Cannot resolve '{0}'.", new object[] { absoluteUri.ToString() }));
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
				throw new XmlException(Res.GetString("Object type is not supported."));
			}
		}

		public override ICredentials Credentials
		{
			set
			{
				if (this.fallbackResolver != null)
				{
					this.fallbackResolver.Credentials = value;
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
			if (this.mappings.TryGetValue(absoluteUri, out preloadedData))
			{
				return preloadedData.SupportsType(type);
			}
			if (this.fallbackResolver != null)
			{
				return this.fallbackResolver.SupportsType(absoluteUri, type);
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
				Array.Copy(memoryStream.GetBuffer(), array3, num3);
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
				return this.mappings.Keys;
			}
		}

		public void Remove(Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			this.mappings.Remove(uri);
		}

		private void Add(Uri uri, XmlPreloadedResolver.PreloadedData data)
		{
			if (this.mappings.ContainsKey(uri))
			{
				this.mappings[uri] = data;
				return;
			}
			this.mappings.Add(uri, data);
		}

		private void AddKnownDtd(XmlPreloadedResolver.XmlKnownDtdData[] dtdSet)
		{
			foreach (XmlPreloadedResolver.XmlKnownDtdData xmlKnownDtdData in dtdSet)
			{
				this.mappings.Add(new Uri(xmlKnownDtdData.publicId, UriKind.RelativeOrAbsolute), xmlKnownDtdData);
				this.mappings.Add(new Uri(xmlKnownDtdData.systemId, UriKind.RelativeOrAbsolute), xmlKnownDtdData);
			}
		}

		public override Task<object> GetEntityAsync(Uri absoluteUri, string role, Type ofObjectToReturn)
		{
			if (absoluteUri == null)
			{
				throw new ArgumentNullException("absoluteUri");
			}
			XmlPreloadedResolver.PreloadedData preloadedData;
			if (!this.mappings.TryGetValue(absoluteUri, out preloadedData))
			{
				if (this.fallbackResolver != null)
				{
					return this.fallbackResolver.GetEntityAsync(absoluteUri, role, ofObjectToReturn);
				}
				throw new XmlException(Res.GetString("Cannot resolve '{0}'.", new object[] { absoluteUri.ToString() }));
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
				throw new XmlException(Res.GetString("Object type is not supported."));
			}
		}

		private XmlResolver fallbackResolver;

		private Dictionary<Uri, XmlPreloadedResolver.PreloadedData> mappings;

		private XmlKnownDtds preloadedDtds;

		private static XmlPreloadedResolver.XmlKnownDtdData[] Xhtml10_Dtd = new XmlPreloadedResolver.XmlKnownDtdData[]
		{
			new XmlPreloadedResolver.XmlKnownDtdData("-//W3C//DTD XHTML 1.0 Strict//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd", "xhtml1-strict.dtd"),
			new XmlPreloadedResolver.XmlKnownDtdData("-//W3C//DTD XHTML 1.0 Transitional//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd", "xhtml1-transitional.dtd"),
			new XmlPreloadedResolver.XmlKnownDtdData("-//W3C//DTD XHTML 1.0 Frameset//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd", "xhtml1-frameset.dtd"),
			new XmlPreloadedResolver.XmlKnownDtdData("-//W3C//ENTITIES Latin 1 for XHTML//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml-lat1.ent", "xhtml-lat1.ent"),
			new XmlPreloadedResolver.XmlKnownDtdData("-//W3C//ENTITIES Symbols for XHTML//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml-symbol.ent", "xhtml-symbol.ent"),
			new XmlPreloadedResolver.XmlKnownDtdData("-//W3C//ENTITIES Special for XHTML//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml-special.ent", "xhtml-special.ent")
		};

		private static XmlPreloadedResolver.XmlKnownDtdData[] Rss091_Dtd = new XmlPreloadedResolver.XmlKnownDtdData[]
		{
			new XmlPreloadedResolver.XmlKnownDtdData("-//Netscape Communications//DTD RSS 0.91//EN", "http://my.netscape.com/publish/formats/rss-0.91.dtd", "rss-0.91.dtd")
		};

		private abstract class PreloadedData
		{
			internal abstract Stream AsStream();

			internal virtual TextReader AsTextReader()
			{
				throw new XmlException(Res.GetString("Object type is not supported."));
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
				this.resourceName = resourceName;
			}

			internal override Stream AsStream()
			{
				return Assembly.GetExecutingAssembly().GetManifestResourceStream(this.resourceName);
			}

			internal string publicId;

			internal string systemId;

			private string resourceName;
		}

		private class ByteArrayChunk : XmlPreloadedResolver.PreloadedData
		{
			internal ByteArrayChunk(byte[] array)
				: this(array, 0, array.Length)
			{
			}

			internal ByteArrayChunk(byte[] array, int offset, int length)
			{
				this.array = array;
				this.offset = offset;
				this.length = length;
			}

			internal override Stream AsStream()
			{
				return new MemoryStream(this.array, this.offset, this.length);
			}

			private byte[] array;

			private int offset;

			private int length;
		}

		private class StringData : XmlPreloadedResolver.PreloadedData
		{
			internal StringData(string str)
			{
				this.str = str;
			}

			internal override Stream AsStream()
			{
				return new MemoryStream(Encoding.Unicode.GetBytes(this.str));
			}

			internal override TextReader AsTextReader()
			{
				return new StringReader(this.str);
			}

			internal override bool SupportsType(Type type)
			{
				return type == typeof(TextReader) || base.SupportsType(type);
			}

			private string str;
		}
	}
}
