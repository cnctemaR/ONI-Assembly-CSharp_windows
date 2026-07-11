using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	internal struct XmlNavigatorStack
	{
		public void Push(XPathNavigator nav)
		{
			if (this.stkNav == null)
			{
				this.stkNav = new XPathNavigator[8];
			}
			else if (this.sp >= this.stkNav.Length)
			{
				Array array = this.stkNav;
				this.stkNav = new XPathNavigator[2 * this.sp];
				Array.Copy(array, this.stkNav, this.sp);
			}
			XPathNavigator[] array2 = this.stkNav;
			int num = this.sp;
			this.sp = num + 1;
			array2[num] = nav;
		}

		public XPathNavigator Pop()
		{
			XPathNavigator[] array = this.stkNav;
			int num = this.sp - 1;
			this.sp = num;
			return array[num];
		}

		public XPathNavigator Peek()
		{
			return this.stkNav[this.sp - 1];
		}

		public void Reset()
		{
			this.sp = 0;
		}

		public bool IsEmpty
		{
			get
			{
				return this.sp == 0;
			}
		}

		private XPathNavigator[] stkNav;

		private int sp;

		private const int InitialStackSize = 8;
	}
}
