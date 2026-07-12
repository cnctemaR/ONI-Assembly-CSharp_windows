using System;
using System.Collections.Generic;

namespace System.Xml.Linq
{
	public abstract class XObject : IXmlLineInfo
	{
		internal XObject()
		{
		}

		public string BaseUri
		{
			get
			{
				XObject xobject = this;
				BaseUriAnnotation baseUriAnnotation;
				for (;;)
				{
					if (xobject == null || xobject.annotations != null)
					{
						if (xobject == null)
						{
							goto IL_0033;
						}
						baseUriAnnotation = xobject.Annotation<BaseUriAnnotation>();
						if (baseUriAnnotation != null)
						{
							break;
						}
						xobject = xobject.parent;
					}
					else
					{
						xobject = xobject.parent;
					}
				}
				return baseUriAnnotation.baseUri;
				IL_0033:
				return string.Empty;
			}
		}

		public XDocument Document
		{
			get
			{
				XObject xobject = this;
				while (xobject.parent != null)
				{
					xobject = xobject.parent;
				}
				return xobject as XDocument;
			}
		}

		public abstract XmlNodeType NodeType { get; }

		public XElement Parent
		{
			get
			{
				return this.parent as XElement;
			}
		}

		public void AddAnnotation(object annotation)
		{
			if (annotation == null)
			{
				throw new ArgumentNullException("annotation");
			}
			if (this.annotations == null)
			{
				object obj;
				if (!(annotation is object[]))
				{
					obj = annotation;
				}
				else
				{
					(obj = new object[1])[0] = annotation;
				}
				this.annotations = obj;
				return;
			}
			object[] array = this.annotations as object[];
			if (array == null)
			{
				this.annotations = new object[] { this.annotations, annotation };
				return;
			}
			int num = 0;
			while (num < array.Length && array[num] != null)
			{
				num++;
			}
			if (num == array.Length)
			{
				Array.Resize<object>(ref array, num * 2);
				this.annotations = array;
			}
			array[num] = annotation;
		}

		public object Annotation(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (this.annotations != null)
			{
				object[] array = this.annotations as object[];
				if (array == null)
				{
					if (XHelper.IsInstanceOfType(this.annotations, type))
					{
						return this.annotations;
					}
				}
				else
				{
					foreach (object obj in array)
					{
						if (obj == null)
						{
							break;
						}
						if (XHelper.IsInstanceOfType(obj, type))
						{
							return obj;
						}
					}
				}
			}
			return null;
		}

		private object AnnotationForSealedType(Type type)
		{
			if (this.annotations != null)
			{
				object[] array = this.annotations as object[];
				if (array == null)
				{
					if (this.annotations.GetType() == type)
					{
						return this.annotations;
					}
				}
				else
				{
					foreach (object obj in array)
					{
						if (obj == null)
						{
							break;
						}
						if (obj.GetType() == type)
						{
							return obj;
						}
					}
				}
			}
			return null;
		}

		public T Annotation<T>() where T : class
		{
			if (this.annotations != null)
			{
				object[] array = this.annotations as object[];
				if (array == null)
				{
					return this.annotations as T;
				}
				foreach (object obj in array)
				{
					if (obj == null)
					{
						break;
					}
					T t = obj as T;
					if (t != null)
					{
						return t;
					}
				}
			}
			return default(T);
		}

		public IEnumerable<object> Annotations(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			return this.AnnotationsIterator(type);
		}

		private IEnumerable<object> AnnotationsIterator(Type type)
		{
			if (this.annotations != null)
			{
				object[] a = this.annotations as object[];
				if (a == null)
				{
					if (XHelper.IsInstanceOfType(this.annotations, type))
					{
						yield return this.annotations;
					}
				}
				else
				{
					int num;
					for (int i = 0; i < a.Length; i = num + 1)
					{
						object obj = a[i];
						if (obj == null)
						{
							break;
						}
						if (XHelper.IsInstanceOfType(obj, type))
						{
							yield return obj;
						}
						num = i;
					}
				}
				a = null;
			}
			yield break;
		}

		public IEnumerable<T> Annotations<T>() where T : class
		{
			if (this.annotations != null)
			{
				object[] a = this.annotations as object[];
				if (a == null)
				{
					T t = this.annotations as T;
					if (t != null)
					{
						yield return t;
					}
				}
				else
				{
					int num;
					for (int i = 0; i < a.Length; i = num + 1)
					{
						object obj = a[i];
						if (obj == null)
						{
							break;
						}
						T t2 = obj as T;
						if (t2 != null)
						{
							yield return t2;
						}
						num = i;
					}
				}
				a = null;
			}
			yield break;
		}

		public void RemoveAnnotations(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (this.annotations != null)
			{
				object[] array = this.annotations as object[];
				if (array == null)
				{
					if (XHelper.IsInstanceOfType(this.annotations, type))
					{
						this.annotations = null;
						return;
					}
				}
				else
				{
					int i = 0;
					int j = 0;
					while (i < array.Length)
					{
						object obj = array[i];
						if (obj == null)
						{
							break;
						}
						if (!XHelper.IsInstanceOfType(obj, type))
						{
							array[j++] = obj;
						}
						i++;
					}
					if (j == 0)
					{
						this.annotations = null;
						return;
					}
					while (j < i)
					{
						array[j++] = null;
					}
				}
			}
		}

		public void RemoveAnnotations<T>() where T : class
		{
			if (this.annotations != null)
			{
				object[] array = this.annotations as object[];
				if (array == null)
				{
					if (this.annotations is T)
					{
						this.annotations = null;
						return;
					}
				}
				else
				{
					int i = 0;
					int j = 0;
					while (i < array.Length)
					{
						object obj = array[i];
						if (obj == null)
						{
							break;
						}
						if (!(obj is T))
						{
							array[j++] = obj;
						}
						i++;
					}
					if (j == 0)
					{
						this.annotations = null;
						return;
					}
					while (j < i)
					{
						array[j++] = null;
					}
				}
			}
		}

		public event EventHandler<XObjectChangeEventArgs> Changed
		{
			add
			{
				if (value == null)
				{
					return;
				}
				XObjectChangeAnnotation xobjectChangeAnnotation = this.Annotation<XObjectChangeAnnotation>();
				if (xobjectChangeAnnotation == null)
				{
					xobjectChangeAnnotation = new XObjectChangeAnnotation();
					this.AddAnnotation(xobjectChangeAnnotation);
				}
				XObjectChangeAnnotation xobjectChangeAnnotation2 = xobjectChangeAnnotation;
				xobjectChangeAnnotation2.changed = (EventHandler<XObjectChangeEventArgs>)Delegate.Combine(xobjectChangeAnnotation2.changed, value);
			}
			remove
			{
				if (value == null)
				{
					return;
				}
				XObjectChangeAnnotation xobjectChangeAnnotation = this.Annotation<XObjectChangeAnnotation>();
				if (xobjectChangeAnnotation == null)
				{
					return;
				}
				XObjectChangeAnnotation xobjectChangeAnnotation2 = xobjectChangeAnnotation;
				xobjectChangeAnnotation2.changed = (EventHandler<XObjectChangeEventArgs>)Delegate.Remove(xobjectChangeAnnotation2.changed, value);
				if (xobjectChangeAnnotation.changing == null && xobjectChangeAnnotation.changed == null)
				{
					this.RemoveAnnotations<XObjectChangeAnnotation>();
				}
			}
		}

		public event EventHandler<XObjectChangeEventArgs> Changing
		{
			add
			{
				if (value == null)
				{
					return;
				}
				XObjectChangeAnnotation xobjectChangeAnnotation = this.Annotation<XObjectChangeAnnotation>();
				if (xobjectChangeAnnotation == null)
				{
					xobjectChangeAnnotation = new XObjectChangeAnnotation();
					this.AddAnnotation(xobjectChangeAnnotation);
				}
				XObjectChangeAnnotation xobjectChangeAnnotation2 = xobjectChangeAnnotation;
				xobjectChangeAnnotation2.changing = (EventHandler<XObjectChangeEventArgs>)Delegate.Combine(xobjectChangeAnnotation2.changing, value);
			}
			remove
			{
				if (value == null)
				{
					return;
				}
				XObjectChangeAnnotation xobjectChangeAnnotation = this.Annotation<XObjectChangeAnnotation>();
				if (xobjectChangeAnnotation == null)
				{
					return;
				}
				XObjectChangeAnnotation xobjectChangeAnnotation2 = xobjectChangeAnnotation;
				xobjectChangeAnnotation2.changing = (EventHandler<XObjectChangeEventArgs>)Delegate.Remove(xobjectChangeAnnotation2.changing, value);
				if (xobjectChangeAnnotation.changing == null && xobjectChangeAnnotation.changed == null)
				{
					this.RemoveAnnotations<XObjectChangeAnnotation>();
				}
			}
		}

		bool IXmlLineInfo.HasLineInfo()
		{
			return this.Annotation<LineInfoAnnotation>() != null;
		}

		int IXmlLineInfo.LineNumber
		{
			get
			{
				LineInfoAnnotation lineInfoAnnotation = this.Annotation<LineInfoAnnotation>();
				if (lineInfoAnnotation != null)
				{
					return lineInfoAnnotation.lineNumber;
				}
				return 0;
			}
		}

		int IXmlLineInfo.LinePosition
		{
			get
			{
				LineInfoAnnotation lineInfoAnnotation = this.Annotation<LineInfoAnnotation>();
				if (lineInfoAnnotation != null)
				{
					return lineInfoAnnotation.linePosition;
				}
				return 0;
			}
		}

		internal bool HasBaseUri
		{
			get
			{
				return this.Annotation<BaseUriAnnotation>() != null;
			}
		}

		internal bool NotifyChanged(object sender, XObjectChangeEventArgs e)
		{
			bool flag = false;
			XObject xobject = this;
			for (;;)
			{
				if (xobject == null || xobject.annotations != null)
				{
					if (xobject == null)
					{
						break;
					}
					XObjectChangeAnnotation xobjectChangeAnnotation = xobject.Annotation<XObjectChangeAnnotation>();
					if (xobjectChangeAnnotation != null)
					{
						flag = true;
						if (xobjectChangeAnnotation.changed != null)
						{
							xobjectChangeAnnotation.changed(sender, e);
						}
					}
					xobject = xobject.parent;
				}
				else
				{
					xobject = xobject.parent;
				}
			}
			return flag;
		}

		internal bool NotifyChanging(object sender, XObjectChangeEventArgs e)
		{
			bool flag = false;
			XObject xobject = this;
			for (;;)
			{
				if (xobject == null || xobject.annotations != null)
				{
					if (xobject == null)
					{
						break;
					}
					XObjectChangeAnnotation xobjectChangeAnnotation = xobject.Annotation<XObjectChangeAnnotation>();
					if (xobjectChangeAnnotation != null)
					{
						flag = true;
						if (xobjectChangeAnnotation.changing != null)
						{
							xobjectChangeAnnotation.changing(sender, e);
						}
					}
					xobject = xobject.parent;
				}
				else
				{
					xobject = xobject.parent;
				}
			}
			return flag;
		}

		internal void SetBaseUri(string baseUri)
		{
			this.AddAnnotation(new BaseUriAnnotation(baseUri));
		}

		internal void SetLineInfo(int lineNumber, int linePosition)
		{
			this.AddAnnotation(new LineInfoAnnotation(lineNumber, linePosition));
		}

		internal bool SkipNotify()
		{
			XObject xobject = this;
			for (;;)
			{
				if (xobject == null || xobject.annotations != null)
				{
					if (xobject == null)
					{
						break;
					}
					if (xobject.Annotation<XObjectChangeAnnotation>() != null)
					{
						return false;
					}
					xobject = xobject.parent;
				}
				else
				{
					xobject = xobject.parent;
				}
			}
			return true;
		}

		internal SaveOptions GetSaveOptionsFromAnnotations()
		{
			XObject xobject = this;
			object obj;
			for (;;)
			{
				if (xobject == null || xobject.annotations != null)
				{
					if (xobject == null)
					{
						break;
					}
					obj = xobject.AnnotationForSealedType(typeof(SaveOptions));
					if (obj != null)
					{
						goto Block_3;
					}
					xobject = xobject.parent;
				}
				else
				{
					xobject = xobject.parent;
				}
			}
			return SaveOptions.None;
			Block_3:
			return (SaveOptions)obj;
		}

		internal XContainer parent;

		internal object annotations;
	}
}
