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

		public void Add(ref CopyMeshJobData job)
		{
			this.m_CopyMeshJobs.Add(ref job);
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

		public void CompleteCopyMeshJobs()
		{
			foreach (NativeSlice<CopyMeshJobData> nativeSlice in this.m_CopyMeshJobs.GetPages())
			{
				this.m_JobMerger.Add(JobProcessor.ScheduleCopyMeshJobs((IntPtr)nativeSlice.GetUnsafePtr<CopyMeshJobData>(), nativeSlice.Length));
			}
			this.m_JobMerger.MergeAndReset().Complete();
			this.m_CopyMeshJobs.Reset();
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
					this.m_CopyMeshJobs.Dispose();
					this.m_JobMerger.Dispose();
				}
				this.disposed = true;
			}
		}

		private const string k_JobManagerName = "Renderer.JobManager";

		private NativePagedList<NudgeJobData> m_NudgeJobs = new NativePagedList<NudgeJobData>(64, "Renderer.JobManager", Allocator.Persistent, Allocator.Persistent);

		private NativePagedList<ConvertMeshJobData> m_ConvertMeshJobs = new NativePagedList<ConvertMeshJobData>(64, "Renderer.JobManager", Allocator.Persistent, Allocator.Persistent);

		private NativePagedList<CopyMeshJobData> m_CopyMeshJobs = new NativePagedList<CopyMeshJobData>(64, "Renderer.JobManager", Allocator.Persistent, Allocator.Persistent);

		private JobMerger m_JobMerger = new JobMerger(128);
	}
}
