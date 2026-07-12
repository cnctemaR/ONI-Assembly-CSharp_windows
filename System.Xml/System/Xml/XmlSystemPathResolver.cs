using System;
using System.IO;
using System.Threading.Tasks;

namespace System.Xml
{
	internal class XmlSystemPathResolver : XmlResolver
	{
		public override object GetEntity(Uri uri, string role, Type typeOfObjectToReturn)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (typeOfObjectToReturn != null && typeOfObjectToReturn != typeof(Stream) && typeOfObjectToReturn != typeof(object))
			{
				throw new XmlException("Object type is not supported.", string.Empty);
			}
			string text = uri.OriginalString;
			if (uri.IsAbsoluteUri)
			{
				if (!uri.IsFile)
				{
					throw new XmlException("Cannot open '{0}'. The Uri parameter must be a file system relative or absolute path.", uri.ToString());
				}
				text = uri.LocalPath;
			}
			object obj;
			try
			{
				obj = new FileStream(text, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			catch (ArgumentException ex)
			{
				throw new XmlException("Cannot open '{0}'. The Uri parameter must be a file system relative or absolute path.", uri.ToString(), ex, null);
			}
			return obj;
		}

		public override Task<object> GetEntityAsync(Uri absoluteUri, string role, Type ofObjectToReturn)
		{
			return Task.FromResult<object>(this.GetEntity(absoluteUri, role, ofObjectToReturn));
		}
	}
}
