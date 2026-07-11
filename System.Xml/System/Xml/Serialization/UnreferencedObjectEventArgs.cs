using System;

namespace System.Xml.Serialization
{
	public class UnreferencedObjectEventArgs : EventArgs
	{
		public UnreferencedObjectEventArgs(object o, string id)
		{
			this.o = o;
			this.id = id;
		}

		public object UnreferencedObject
		{
			get
			{
				return this.o;
			}
		}

		public string UnreferencedId
		{
			get
			{
				return this.id;
			}
		}

		private object o;

		private string id;
	}
}
