using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.UIElements.UIR
{
	internal class JobManager : IDisposable
	{
		public void Add(ref NudgeJobData job)
		{
			this.m_NudgeJobs.Add(ref job);
		}

		public void Add(ref ConvertMeshJobData job)
		{
			this.m_ConvertMeshJobs.Add(ref job);
		}

		public void Add(ref CopyClosingMeshJobData job)
		{
			this.m_CopyClosingMeshJobs.Add(ref job);
		}

		public void CompleteNudgeJobs()
		{
			foreach (NativeSlice<NudgeJobData> nativeSlice in this.m_NudgeJobs.GetPages())
			{
				this.m_JobMerger.Add(JobProcessor.ScheduleNudgeJobs((IntPtr)nativeSlice.GetUnsafePtr<NudgeJobData>(), nativeSlice.Length));
			}
			this.m_JobMerger.MergeAndReset().Complete();
			this.m_NudgeJobs.Reset();
		}

		public void CompleteConvertMeshJobs()
		{
			foreach (NativeSlice<ConvertMeshJobData> nativeSlice in this.m_ConvertMeshJobs.GetPages())
			{
				this.m_JobMerger.Add(JobProcessor.ScheduleConvertMeshJobs((IntPtr)nativeSlice.GetUnsafePtr<ConvertMeshJobData>(), nativeSlice.Length));
			}
			this.m_JobMerger.MergeAndReset().Complete();
			this.m_ConvertMeshJobs.Reset();
		}

		public void CompleteClosingMeshJobs()
		{
			foreach (NativeSlice<CopyClosingMeshJobData> nativeSlice in this.m_CopyClosingMeshJobs.GetPages())
			{
				this.m_JobMerger.Add(JobProcessor.ScheduleCopyClosingMeshJobs((IntPtr)nativeSlice.GetUnsafePtr<CopyClosingMeshJobData>(), nativeSlice.Length));
			}
			this.m_JobMerger.MergeAndReset().Complete();
			this.m_CopyClosingMeshJobs.Reset();
		}

		private protected bool disposed { protected get; private set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			bool disposed = this.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					this.m_NudgeJobs.Dispose();
					this.m_ConvertMeshJobs.Dispose();
					this.m_CopyClosingMeshJobs.Dispose();
					this.m_JobMerger.Dispose();
				}
				this.disposed = true;
			}
		}

		private NativePagedList<NudgeJobData> m_NudgeJobs = new NativePagedList<NudgeJobData>(64);

		private NativePagedList<ConvertMeshJobData> m_ConvertMeshJobs = new NativePagedList<ConvertMeshJobData>(64);

		private NativePagedList<CopyClosingMeshJobData> m_CopyClosingMeshJobs = new NativePagedList<CopyClosingMeshJobData>(64);

		private JobMerger m_JobMerger = new JobMerger(128);
	}
}
