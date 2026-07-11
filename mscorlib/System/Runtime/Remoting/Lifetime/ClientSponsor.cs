using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Lifetime
{
	[ComVisible(true)]
	public class ClientSponsor : MarshalByRefObject, ISponsor
	{
		public ClientSponsor()
		{
			this.renewal_time = new TimeSpan(0, 2, 0);
		}

		public ClientSponsor(TimeSpan renewalTime)
		{
			this.renewal_time = renewalTime;
		}

		public TimeSpan RenewalTime
		{
			get
			{
				return this.renewal_time;
			}
			set
			{
				this.renewal_time = value;
			}
		}

		public void Close()
		{
			foreach (object obj in this.registered_objects.Values)
			{
				MarshalByRefObject marshalByRefObject = (MarshalByRefObject)obj;
				ILease lease = marshalByRefObject.GetLifetimeService() as ILease;
				lease.Unregister(this);
			}
			this.registered_objects.Clear();
		}

		~ClientSponsor()
		{
			this.Close();
		}

		public override object InitializeLifetimeService()
		{
			return base.InitializeLifetimeService();
		}

		public bool Register(MarshalByRefObject obj)
		{
			if (this.registered_objects.ContainsKey(obj))
			{
				return false;
			}
			ILease lease = obj.GetLifetimeService() as ILease;
			if (lease == null)
			{
				return false;
			}
			lease.Register(this);
			this.registered_objects.Add(obj, obj);
			return true;
		}

		public TimeSpan Renewal(ILease lease)
		{
			return this.renewal_time;
		}

		public void Unregister(MarshalByRefObject obj)
		{
			if (!this.registered_objects.ContainsKey(obj))
			{
				return;
			}
			ILease lease = obj.GetLifetimeService() as ILease;
			lease.Unregister(this);
			this.registered_objects.Remove(obj);
		}

		private TimeSpan renewal_time;

		private Hashtable registered_objects = new Hashtable();
	}
}
