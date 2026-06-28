using System;

namespace FileHelpers.MasterDetail
{
	public class MasterDetails<M, D> where M : class where D : class
	{
		public MasterDetails()
		{
			this.mDetails = MasterDetails<M, D>.mEmpty.mDetails;
		}

		public MasterDetails(M master, D[] details)
		{
			this.mMaster = master;
			this.mDetails = details;
		}

		public static MasterDetails<M, D> Empty
		{
			get
			{
				return MasterDetails<M, D>.mEmpty;
			}
		}

		public M Master
		{
			get
			{
				return this.mMaster;
			}
			set
			{
				this.mMaster = value;
			}
		}

		public D[] Details
		{
			get
			{
				return this.mDetails;
			}
			set
			{
				this.mDetails = value;
			}
		}

		private static readonly MasterDetails<M, D> mEmpty = new MasterDetails<M, D>(default(M), new D[0]);

		protected M mMaster;

		protected D[] mDetails;
	}
}
