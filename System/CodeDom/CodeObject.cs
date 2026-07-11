using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace System.CodeDom
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Serializable]
	public class CodeObject
	{
		public IDictionary UserData
		{
			get
			{
				if (this.userData == null)
				{
					this.userData = new global::System.Collections.Specialized.ListDictionary();
				}
				return this.userData;
			}
		}

		internal virtual void Accept(ICodeDomVisitor visitor)
		{
			throw new NotImplementedException();
		}

		private IDictionary userData;
	}
}
