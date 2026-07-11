using System;
using System.Reflection;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.IlGen
{
	internal class XmlILAnnotation : ListBase<object>
	{
		public static XmlILAnnotation Write(QilNode nd)
		{
			XmlILAnnotation xmlILAnnotation = nd.Annotation as XmlILAnnotation;
			if (xmlILAnnotation == null)
			{
				xmlILAnnotation = new XmlILAnnotation(nd.Annotation);
				nd.Annotation = xmlILAnnotation;
			}
			return xmlILAnnotation;
		}

		private XmlILAnnotation(object annPrev)
		{
			this.annPrev = annPrev;
		}

		public MethodInfo FunctionBinding
		{
			get
			{
				return this.funcMethod;
			}
			set
			{
				this.funcMethod = value;
			}
		}

		public int ArgumentPosition
		{
			get
			{
				return this.argPos;
			}
			set
			{
				this.argPos = value;
			}
		}

		public IteratorDescriptor CachedIteratorDescriptor
		{
			get
			{
				return this.iterInfo;
			}
			set
			{
				this.iterInfo = value;
			}
		}

		public XmlILConstructInfo ConstructInfo
		{
			get
			{
				return this.constrInfo;
			}
			set
			{
				this.constrInfo = value;
			}
		}

		public OptimizerPatterns Patterns
		{
			get
			{
				return this.optPatt;
			}
			set
			{
				this.optPatt = value;
			}
		}

		public override int Count
		{
			get
			{
				if (this.annPrev == null)
				{
					return 2;
				}
				return 3;
			}
		}

		public override object this[int index]
		{
			get
			{
				if (this.annPrev != null)
				{
					if (index == 0)
					{
						return this.annPrev;
					}
					index--;
				}
				if (index == 0)
				{
					return this.constrInfo;
				}
				if (index != 1)
				{
					throw new IndexOutOfRangeException();
				}
				return this.optPatt;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		private object annPrev;

		private MethodInfo funcMethod;

		private int argPos;

		private IteratorDescriptor iterInfo;

		private XmlILConstructInfo constrInfo;

		private OptimizerPatterns optPatt;
	}
}
