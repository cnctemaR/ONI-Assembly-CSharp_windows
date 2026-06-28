using System;

namespace System.EnterpriseServices.CompensatingResourceManager
{
	public class Compensator : ServicedComponent
	{
		[MonoTODO]
		public Compensator()
		{
			throw new NotImplementedException();
		}

		public Clerk Clerk
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		[MonoTODO]
		public virtual bool AbortRecord(LogRecord rec)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public virtual void BeginAbort(bool fRecovery)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public virtual void BeginCommit(bool fRecovery)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public virtual void BeginPrepare()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public virtual bool CommitRecord(LogRecord rec)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public virtual void EndAbort()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public virtual void EndCommit()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public virtual bool EndPrepare()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public virtual bool PrepareRecord(LogRecord rec)
		{
			throw new NotImplementedException();
		}
	}
}
