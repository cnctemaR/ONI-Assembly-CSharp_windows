using System;
using System.IO;
using System.Net;

namespace System.Xml
{
	public class XmlUrlResolver : XmlResolver
	{
		public override ICredentials Credentials
		{
			set
			{
				this.credential = value;
			}
		}

		public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
		{
			if (ofObjectToReturn == null)
			{
				ofObjectToReturn = typeof(Stream);
			}
			if (ofObjectToReturn != typeof(Stream))
			{
				throw new XmlException("This object type is not supported.");
			}
			if (!absoluteUri.IsAbsoluteUri)
			{
				throw new ArgumentException("uri must be absolute.", "absoluteUri");
			}
			if (!(absoluteUri.Scheme == "file"))
			{
				WebRequest webRequest = WebRequest.Create(absoluteUri);
				if (this.credential != null)
				{
					webRequest.Credentials = this.credential;
				}
				return webRequest.GetResponse().GetResponseStream();
			}
			if (absoluteUri.AbsolutePath == string.Empty)
			{
				throw new ArgumentException("uri must be absolute.", "absoluteUri");
			}
			return new FileStream(this.UnescapeRelativeUriBody(absoluteUri.LocalPath), FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		public override Uri ResolveUri(Uri baseUri, string relativeUri)
		{
			return base.ResolveUri(baseUri, relativeUri);
		}

		private string UnescapeRelativeUriBody(string src)
		{
			return src.Replace("%3C", "<").Replace("%3E", ">").Replace("%23", "#")
				.Replace("%22", "\"")
				.Replace("%20", " ")
				.Replace("%25", "%");
		}

		private ICredentials credential;
	}
}
