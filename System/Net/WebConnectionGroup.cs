using System;
using System.Collections;

namespace System.Net
{
	internal class WebConnectionGroup
	{
		public WebConnectionGroup(ServicePoint sPoint, string name)
		{
			this.sPoint = sPoint;
			this.name = name;
			this.connections = new ArrayList(1);
			this.queue = new Queue();
		}

		public void Close()
		{
			ArrayList arrayList = this.connections;
			lock (arrayList)
			{
				int count = this.connections.Count;
				for (int i = 0; i < count; i++)
				{
					WeakReference weakReference = (WeakReference)this.connections[i];
					WebConnection webConnection = weakReference.Target as WebConnection;
					if (webConnection != null)
					{
						webConnection.Close(false);
					}
				}
				this.connections.Clear();
			}
		}

		public WebConnection GetConnection(HttpWebRequest request)
		{
			WebConnection webConnection = null;
			ArrayList arrayList = this.connections;
			lock (arrayList)
			{
				int count = this.connections.Count;
				ArrayList arrayList2 = null;
				for (int i = 0; i < count; i++)
				{
					WeakReference weakReference = (WeakReference)this.connections[i];
					webConnection = weakReference.Target as WebConnection;
					if (webConnection == null)
					{
						if (arrayList2 == null)
						{
							arrayList2 = new ArrayList(1);
						}
						arrayList2.Add(i);
					}
				}
				if (arrayList2 != null)
				{
					for (int j = arrayList2.Count - 1; j >= 0; j--)
					{
						this.connections.RemoveAt((int)arrayList2[j]);
					}
				}
				webConnection = this.CreateOrReuseConnection(request);
			}
			return webConnection;
		}

		private static void PrepareSharingNtlm(WebConnection cnc, HttpWebRequest request)
		{
			if (!cnc.NtlmAuthenticated)
			{
				return;
			}
			bool flag = false;
			NetworkCredential ntlmCredential = cnc.NtlmCredential;
			NetworkCredential credential = request.Credentials.GetCredential(request.RequestUri, "NTLM");
			if (ntlmCredential.Domain != credential.Domain || ntlmCredential.UserName != credential.UserName || ntlmCredential.Password != credential.Password)
			{
				flag = true;
			}
			if (!flag)
			{
				bool unsafeAuthenticatedConnectionSharing = request.UnsafeAuthenticatedConnectionSharing;
				bool unsafeAuthenticatedConnectionSharing2 = cnc.UnsafeAuthenticatedConnectionSharing;
				flag = !unsafeAuthenticatedConnectionSharing || unsafeAuthenticatedConnectionSharing != unsafeAuthenticatedConnectionSharing2;
			}
			if (flag)
			{
				cnc.Close(false);
				cnc.ResetNtlm();
			}
		}

		private WebConnection CreateOrReuseConnection(HttpWebRequest request)
		{
			int num = this.connections.Count;
			WebConnection webConnection;
			for (int i = 0; i < num; i++)
			{
				WeakReference weakReference = this.connections[i] as WeakReference;
				webConnection = weakReference.Target as WebConnection;
				if (webConnection == null)
				{
					this.connections.RemoveAt(i);
					num--;
					i--;
				}
				else if (!webConnection.Busy)
				{
					WebConnectionGroup.PrepareSharingNtlm(webConnection, request);
					return webConnection;
				}
			}
			if (this.sPoint.ConnectionLimit > num)
			{
				webConnection = new WebConnection(this, this.sPoint);
				this.connections.Add(new WeakReference(webConnection));
				return webConnection;
			}
			if (this.rnd == null)
			{
				this.rnd = new Random();
			}
			int num2 = ((num <= 1) ? 0 : this.rnd.Next(0, num - 1));
			WeakReference weakReference2 = (WeakReference)this.connections[num2];
			webConnection = weakReference2.Target as WebConnection;
			if (webConnection == null)
			{
				webConnection = new WebConnection(this, this.sPoint);
				this.connections.RemoveAt(num2);
				this.connections.Add(new WeakReference(webConnection));
			}
			return webConnection;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		internal Queue Queue
		{
			get
			{
				return this.queue;
			}
		}

		private ServicePoint sPoint;

		private string name;

		private ArrayList connections;

		private Random rnd;

		private Queue queue;
	}
}
