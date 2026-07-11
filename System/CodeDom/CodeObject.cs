using System;
using System.Collections;
using System.Collections.Specialized;

namespace System.CodeDom
{
	[Serializable]
	public class CodeObject
	{
		public IDictionary UserData
		{
			get
			{
				IDictionary dictionary;
				if ((dictionary = this._userData) == null)
				{
					dictionary = (this._userData = new ListDictionary());
				}
				return dictionary;
			}
		}

		private IDictionary _userData;
	}
}
