using System;

namespace System.Xml.Serialization
{
	public class UnreferencedObjectEventArgs : EventArgs
	{
		public UnreferencedObjectEventArgs(object o, string id)
		{
			this.unreferencedObject = o;
			this.unreferencedId = id;
		}

		public string UnreferencedId
		{
			get
			{
				return this.unreferencedId;
			}
		}

		public object UnreferencedObject
		{
			get
			{
				return this.unreferencedObject;
			}
		}

		private object unreferencedObject;

		private string unreferencedId;
	}
}
