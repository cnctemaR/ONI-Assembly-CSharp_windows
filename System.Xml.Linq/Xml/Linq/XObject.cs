using System;
using System.Collections.Generic;

namespace System.Xml.Linq
{
	public abstract class XObject : IXmlLineInfo
	{
		internal XObject()
		{
		}

		public event EventHandler<XObjectChangeEventArgs> Changing;

		public event EventHandler<XObjectChangeEventArgs> Changed;

		int IXmlLineInfo.LineNumber
		{
			get
			{
				return this.LineNumber;
			}
		}

		int IXmlLineInfo.LinePosition
		{
			get
			{
				return this.LinePosition;
			}
		}

		bool IXmlLineInfo.HasLineInfo()
		{
			return this.line > 0;
		}

		public string BaseUri
		{
			get
			{
				return this.baseuri;
			}
			internal set
			{
				this.baseuri = value;
			}
		}

		public XDocument Document
		{
			get
			{
				if (this is XDocument)
				{
					return (XDocument)this;
				}
				for (XContainer xcontainer = this.owner; xcontainer != null; xcontainer = xcontainer.owner)
				{
					if (xcontainer is XDocument)
					{
						return (XDocument)xcontainer;
					}
				}
				return null;
			}
		}

		public abstract XmlNodeType NodeType { get; }

		public XElement Parent
		{
			get
			{
				return this.owner as XElement;
			}
		}

		internal XContainer Owner
		{
			get
			{
				return this.owner;
			}
		}

		internal void SetOwner(XContainer node)
		{
			this.owner = node;
		}

		public void AddAnnotation(object annotation)
		{
			if (annotation == null)
			{
				throw new ArgumentNullException("annotation");
			}
			if (this.annotations == null)
			{
				this.annotations = new List<object>();
			}
			this.annotations.Add(annotation);
		}

		public T Annotation<T>() where T : class
		{
			return (T)((object)this.Annotation(typeof(T)));
		}

		public object Annotation(Type type)
		{
			if (this.annotations != null)
			{
				foreach (object obj in this.annotations)
				{
					if (obj.GetType() == type)
					{
						return obj;
					}
				}
			}
			return null;
		}

		public IEnumerable<T> Annotations<T>() where T : class
		{
			foreach (object obj in this.Annotations(typeof(T)))
			{
				T o = (T)((object)obj);
				yield return o;
			}
			yield break;
		}

		public IEnumerable<object> Annotations(Type type)
		{
			if (this.annotations == null)
			{
				yield break;
			}
			foreach (object o in this.annotations)
			{
				if (o.GetType() == type)
				{
					yield return o;
				}
			}
			yield break;
		}

		public void RemoveAnnotations<T>() where T : class
		{
			this.RemoveAnnotations(typeof(T));
		}

		public void RemoveAnnotations(Type type)
		{
			if (this.annotations == null)
			{
				return;
			}
			for (int i = 0; i < this.annotations.Count; i++)
			{
				if (this.annotations[i].GetType() == type)
				{
					this.annotations.RemoveAt(i);
				}
			}
		}

		internal int LineNumber
		{
			get
			{
				return this.line;
			}
			set
			{
				this.line = value;
			}
		}

		internal int LinePosition
		{
			get
			{
				return this.column;
			}
			set
			{
				this.column = value;
			}
		}

		internal void FillLineInfoAndBaseUri(XmlReader r, LoadOptions options)
		{
			if ((options & LoadOptions.SetLineInfo) != LoadOptions.None)
			{
				IXmlLineInfo xmlLineInfo = r as IXmlLineInfo;
				if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
				{
					this.LineNumber = xmlLineInfo.LineNumber;
					this.LinePosition = xmlLineInfo.LinePosition;
				}
			}
			if ((options & LoadOptions.SetBaseUri) != LoadOptions.None)
			{
				this.BaseUri = r.BaseURI;
			}
		}

		private XContainer owner;

		private List<object> annotations;

		private string baseuri;

		private int line;

		private int column;
	}
}
