using System;

namespace System.Xml
{
	internal class XmlElementListListener
	{
		internal XmlElementListListener(XmlDocument doc, XmlElementList elemList)
		{
			this.doc = doc;
			this.elemList = new WeakReference(elemList);
			this.nodeChangeHandler = new XmlNodeChangedEventHandler(this.OnListChanged);
			doc.NodeInserted += this.nodeChangeHandler;
			doc.NodeRemoved += this.nodeChangeHandler;
		}

		private void OnListChanged(object sender, XmlNodeChangedEventArgs args)
		{
			lock (this)
			{
				if (this.elemList != null)
				{
					XmlElementList xmlElementList = (XmlElementList)this.elemList.Target;
					if (xmlElementList != null)
					{
						xmlElementList.ConcurrencyCheck(args);
					}
					else
					{
						this.doc.NodeInserted -= this.nodeChangeHandler;
						this.doc.NodeRemoved -= this.nodeChangeHandler;
						this.elemList = null;
					}
				}
			}
		}

		internal void Unregister()
		{
			lock (this)
			{
				if (this.elemList != null)
				{
					this.doc.NodeInserted -= this.nodeChangeHandler;
					this.doc.NodeRemoved -= this.nodeChangeHandler;
					this.elemList = null;
				}
			}
		}

		private WeakReference elemList;

		private XmlDocument doc;

		private XmlNodeChangedEventHandler nodeChangeHandler;
	}
}
