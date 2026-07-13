using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class PlatformTripleDetourFactory : IDetourFactory
	{
		public PlatformTripleDetourFactory(PlatformTriple triple)
		{
			this.triple = triple;
		}

		public ICoreDetour CreateDetour(CreateDetourRequest request)
		{
			Helpers.ThrowIfArgumentNull<MethodBase>(request.Source, "request.Source");
			Helpers.ThrowIfArgumentNull<MethodBase>(request.Target, "request.Target");
			if (!this.triple.TryDisableInlining(request.Source))
			{
				bool flag;
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler debugLogWarningStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler(66, 1, out flag);
				if (flag)
				{
					debugLogWarningStringHandler.AppendLiteral("Could not disable inlining of method ");
					debugLogWarningStringHandler.AppendFormatted<MethodBase>(request.Source);
					debugLogWarningStringHandler.AppendLiteral("; detours may not be reliable");
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Warning(ref debugLogWarningStringHandler);
			}
			PlatformTripleDetourFactory.Detour detour = new PlatformTripleDetourFactory.Detour(this.triple, request.Source, request.Target);
			if (request.ApplyByDefault)
			{
				detour.Apply();
			}
			return detour;
		}

		public ICoreNativeDetour CreateNativeDetour(CreateNativeDetourRequest request)
		{
			PlatformTripleDetourFactory.NativeDetour nativeDetour = new PlatformTripleDetourFactory.NativeDetour(this.triple, request.Source, request.Target);
			if (request.ApplyByDefault)
			{
				nativeDetour.Apply();
			}
			return nativeDetour;
		}

		public bool SupportsNativeDetourOrigEntrypoint
		{
			get
			{
				return this.triple.SupportedFeatures.Architecture.Has(ArchitectureFeature.CreateAltEntryPoint);
			}
		}

		private readonly PlatformTriple triple;

		[Nullable(0)]
		private abstract class DetourBase : ICoreDetourBase, IDisposable
		{
			protected DetourBase(PlatformTriple triple)
			{
				this.Triple = triple;
				this.DetourBox = null;
			}

			protected TBox GetDetourBox<[Nullable(0)] TBox>() where TBox : PlatformTripleDetourFactory.DetourBase.DetourBoxBase
			{
				return Unsafe.As<TBox>(this.DetourBox);
			}

			public bool IsApplied
			{
				get
				{
					return this.DetourBox.IsApplied;
				}
			}

			[NullableContext(2)]
			protected static void ReplaceDetourInLock([Nullable(1)] PlatformTripleDetourFactory.DetourBase.DetourBoxBase nativeDetour, SimpleNativeDetour newDetour, out SimpleNativeDetour oldDetour)
			{
				Thread.MemoryBarrier();
				oldDetour = Interlocked.Exchange<SimpleNativeDetour>(ref nativeDetour.Detour, newDetour);
				if (oldDetour != null)
				{
					nativeDetour.OldDetours.Add(oldDetour);
				}
			}

			protected abstract SimpleNativeDetour CreateDetour();

			public void Apply()
			{
				PlatformTripleDetourFactory.DetourBase.DetourBoxBase detourBox = this.DetourBox;
				lock (detourBox)
				{
					if (this.IsApplied)
					{
						throw new InvalidOperationException("Cannot apply a detour which is already applied");
					}
					try
					{
						this.DetourBox.IsApplying = true;
						this.DetourBox.IsApplied = true;
						SimpleNativeDetour simpleNativeDetour;
						PlatformTripleDetourFactory.DetourBase.ReplaceDetourInLock(this.DetourBox, this.CreateDetour(), out simpleNativeDetour);
					}
					catch
					{
						this.DetourBox.IsApplied = false;
						throw;
					}
					finally
					{
						this.DetourBox.IsApplying = false;
					}
				}
			}

			protected abstract void BeforeUndo();

			protected abstract void AfterUndo();

			public void Undo()
			{
				PlatformTripleDetourFactory.DetourBase.DetourBoxBase detourBox = this.DetourBox;
				lock (detourBox)
				{
					if (!this.IsApplied)
					{
						throw new InvalidOperationException("Cannot undo a detour which is not applied");
					}
					try
					{
						this.DetourBox.IsApplying = true;
						SimpleNativeDetour simpleNativeDetour;
						this.UndoCore(out simpleNativeDetour);
						this.DetourBox.ClearOldDetours();
					}
					finally
					{
						this.DetourBox.IsApplying = false;
					}
				}
			}

			[NullableContext(2)]
			private void UndoCore(out SimpleNativeDetour oldDetour)
			{
				this.BeforeUndo();
				this.DetourBox.IsApplied = false;
				PlatformTripleDetourFactory.DetourBase.ReplaceDetourInLock(this.DetourBox, null, out oldDetour);
				this.AfterUndo();
			}

			protected abstract void BeforeDispose();

			private void Dispose(bool disposing)
			{
				if (!this.disposedValue)
				{
					this.BeforeDispose();
					PlatformTripleDetourFactory.DetourBase.DetourBoxBase detourBox = this.DetourBox;
					lock (detourBox)
					{
						SimpleNativeDetour simpleNativeDetour;
						this.UndoCore(out simpleNativeDetour);
						this.DetourBox.ClearOldDetours();
					}
					this.disposedValue = true;
				}
			}

			~DetourBase()
			{
				this.Dispose(false);
			}

			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected readonly PlatformTriple Triple;

			protected PlatformTripleDetourFactory.DetourBase.DetourBoxBase DetourBox;

			private bool disposedValue;

			[Nullable(0)]
			protected abstract class DetourBoxBase
			{
				public bool IsApplied
				{
					get
					{
						return Volatile.Read(ref this.applyDetours);
					}
					set
					{
						Volatile.Write(ref this.applyDetours, value);
						Thread.MemoryBarrier();
					}
				}

				public bool IsApplying
				{
					get
					{
						return Volatile.Read(ref this.isApplying);
					}
					set
					{
						Volatile.Write(ref this.isApplying, value);
						Thread.MemoryBarrier();
					}
				}

				protected DetourBoxBase(PlatformTriple triple)
				{
					this.Triple = triple;
					this.applyDetours = false;
					this.isApplying = false;
				}

				public void ClearOldDetours()
				{
					foreach (SimpleNativeDetour simpleNativeDetour in this.OldDetours)
					{
						simpleNativeDetour.Dispose();
					}
					this.OldDetours.Clear();
				}

				[Nullable(2)]
				public SimpleNativeDetour Detour;

				public readonly List<SimpleNativeDetour> OldDetours = new List<SimpleNativeDetour>();

				protected readonly PlatformTriple Triple;

				protected readonly object Sync = new object();

				private bool applyDetours;

				private bool isApplying;
			}
		}

		[Nullable(0)]
		private sealed class Detour : PlatformTripleDetourFactory.DetourBase, ICoreDetour, ICoreDetourBase, IDisposable
		{
			private new PlatformTripleDetourFactory.Detour.ManagedDetourBox DetourBox
			{
				get
				{
					return base.GetDetourBox<PlatformTripleDetourFactory.Detour.ManagedDetourBox>();
				}
			}

			public Detour(PlatformTriple triple, MethodBase src, MethodBase dst)
				: base(triple)
			{
				this.Source = triple.GetIdentifiable(src);
				this.Target = dst;
				this.realTarget = triple.GetRealDetourTarget(src, dst);
				this.DetourBox = new PlatformTripleDetourFactory.Detour.ManagedDetourBox(triple, this.Source, this.realTarget);
				if (triple.SupportedFeatures.Has(RuntimeFeature.CompileMethodHook))
				{
					PlatformTripleDetourFactory.Detour.EnsureSubscribed(triple);
					this.DetourBox.SubscribeCompileMethod();
				}
			}

			private static void EnsureSubscribed(PlatformTriple triple)
			{
				if (Volatile.Read(ref PlatformTripleDetourFactory.Detour.hasSubscribed))
				{
					return;
				}
				object obj = PlatformTripleDetourFactory.Detour.subLock;
				lock (obj)
				{
					if (!Volatile.Read(ref PlatformTripleDetourFactory.Detour.hasSubscribed))
					{
						Volatile.Write(ref PlatformTripleDetourFactory.Detour.hasSubscribed, true);
						IRuntime runtime = triple.Runtime;
						OnMethodCompiledCallback onMethodCompiledCallback;
						if ((onMethodCompiledCallback = PlatformTripleDetourFactory.Detour.<>O.<0>__OnMethodCompiled) == null)
						{
							onMethodCompiledCallback = (PlatformTripleDetourFactory.Detour.<>O.<0>__OnMethodCompiled = new OnMethodCompiledCallback(PlatformTripleDetourFactory.Detour.OnMethodCompiled));
						}
						runtime.OnMethodCompiled += onMethodCompiledCallback;
					}
				}
			}

			private static void AddRelatedDetour(MethodBase m, PlatformTripleDetourFactory.Detour.ManagedDetourBox cmh)
			{
				for (;;)
				{
					PlatformTripleDetourFactory.Detour.RelatedDetourBag orAdd = PlatformTripleDetourFactory.Detour.relatedDetours.GetOrAdd(m, (MethodBase m) => new PlatformTripleDetourFactory.Detour.RelatedDetourBag(m));
					PlatformTripleDetourFactory.Detour.RelatedDetourBag relatedDetourBag = orAdd;
					lock (relatedDetourBag)
					{
						if (!orAdd.IsValid)
						{
							continue;
						}
						orAdd.RelatedDetours.Add(cmh);
						if (orAdd.RelatedDetours.Count > 1)
						{
							bool flag2;
							<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler debugLogWarningStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler(115, 1, out flag2);
							if (flag2)
							{
								debugLogWarningStringHandler.AppendLiteral("Multiple related detours for method ");
								debugLogWarningStringHandler.AppendFormatted<MethodBase>(m);
								debugLogWarningStringHandler.AppendLiteral("! This means that the method has been detoured twice. Detour cleanup will fail.");
							}
							<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Warning(ref debugLogWarningStringHandler);
						}
					}
					break;
				}
			}

			private static void RemoveRelatedDetour(MethodBase m, PlatformTripleDetourFactory.Detour.ManagedDetourBox cmh)
			{
				PlatformTripleDetourFactory.Detour.RelatedDetourBag relatedDetourBag;
				if (PlatformTripleDetourFactory.Detour.relatedDetours.TryGetValue(m, out relatedDetourBag))
				{
					PlatformTripleDetourFactory.Detour.RelatedDetourBag relatedDetourBag2 = relatedDetourBag;
					lock (relatedDetourBag2)
					{
						relatedDetourBag.RelatedDetours.Remove(cmh);
						if (relatedDetourBag.RelatedDetours.Count == 0)
						{
							relatedDetourBag.IsValid = false;
							PlatformTripleDetourFactory.Detour.RelatedDetourBag relatedDetourBag3;
							Helpers.Assert(PlatformTripleDetourFactory.Detour.relatedDetours.TryRemove(relatedDetourBag.Method, out relatedDetourBag3), null, "relatedDetours.TryRemove(related.Method, out _)");
						}
						return;
					}
				}
				bool flag;
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler debugLogWarningStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler(79, 1, out flag);
				if (flag)
				{
					debugLogWarningStringHandler.AppendLiteral("Attempted to remove a related detour from method ");
					debugLogWarningStringHandler.AppendFormatted<MethodBase>(m);
					debugLogWarningStringHandler.AppendLiteral(" which has no RelatedDetourBag");
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Warning(ref debugLogWarningStringHandler);
			}

			[NullableContext(2)]
			private static void OnMethodCompiled(RuntimeMethodHandle methodHandle, MethodBase method, IntPtr codeStart, IntPtr codeStartRw, ulong codeSize)
			{
				if (method == null)
				{
					return;
				}
				method = PlatformTriple.Current.GetIdentifiable(method);
				PlatformTripleDetourFactory.Detour.RelatedDetourBag relatedDetourBag;
				if (PlatformTripleDetourFactory.Detour.relatedDetours.TryGetValue(method, out relatedDetourBag))
				{
					PlatformTripleDetourFactory.Detour.RelatedDetourBag relatedDetourBag2 = relatedDetourBag;
					lock (relatedDetourBag2)
					{
						foreach (PlatformTripleDetourFactory.Detour.ManagedDetourBox managedDetourBox in relatedDetourBag.RelatedDetours)
						{
							managedDetourBox.OnMethodCompiled(method, codeStart, codeStartRw, codeSize);
						}
					}
				}
			}

			public MethodBase Source { get; }

			public MethodBase Target { get; }

			protected override SimpleNativeDetour CreateDetour()
			{
				bool flag;
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(33, 2, out flag);
				if (flag)
				{
					debugLogTraceStringHandler.AppendLiteral("Applying managed detour from ");
					debugLogTraceStringHandler.AppendFormatted<MethodBase>(this.Source);
					debugLogTraceStringHandler.AppendLiteral(" to ");
					debugLogTraceStringHandler.AppendFormatted<MethodBase>(this.realTarget);
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
				this.srcPin = this.Triple.PinMethodIfNeeded(this.Source);
				this.dstPin = this.Triple.PinMethodIfNeeded(this.realTarget);
				this.Triple.Compile(this.Source);
				IntPtr nativeMethodBody = this.Triple.GetNativeMethodBody(this.Source);
				this.Triple.Compile(this.realTarget);
				IntPtr functionPointer = this.Triple.Runtime.GetMethodHandle(this.realTarget).GetFunctionPointer();
				return this.Triple.CreateSimpleDetour(nativeMethodBody, functionPointer, -1, (IntPtr)0);
			}

			protected override void BeforeUndo()
			{
				bool flag;
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(32, 2, out flag);
				if (flag)
				{
					debugLogTraceStringHandler.AppendLiteral("Undoing managed detour from ");
					debugLogTraceStringHandler.AppendFormatted<MethodBase>(this.Source);
					debugLogTraceStringHandler.AppendLiteral(" to ");
					debugLogTraceStringHandler.AppendFormatted<MethodBase>(this.realTarget);
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
			}

			protected override void AfterUndo()
			{
				IDisposable disposable = Interlocked.Exchange<IDisposable>(ref this.srcPin, null);
				if (disposable != null)
				{
					disposable.Dispose();
				}
				IDisposable disposable2 = Interlocked.Exchange<IDisposable>(ref this.dstPin, null);
				if (disposable2 == null)
				{
					return;
				}
				disposable2.Dispose();
			}

			protected override void BeforeDispose()
			{
				if (this.Triple.SupportedFeatures.Has(RuntimeFeature.CompileMethodHook))
				{
					this.DetourBox.UnsubscribeCompileMethod();
				}
			}

			private readonly MethodBase realTarget;

			private static readonly object subLock = new object();

			private static bool hasSubscribed;

			private static readonly ConcurrentDictionary<MethodBase, PlatformTripleDetourFactory.Detour.RelatedDetourBag> relatedDetours = new ConcurrentDictionary<MethodBase, PlatformTripleDetourFactory.Detour.RelatedDetourBag>();

			[Nullable(2)]
			private IDisposable srcPin;

			[Nullable(2)]
			private IDisposable dstPin;

			[Nullable(0)]
			private sealed class ManagedDetourBox : PlatformTripleDetourFactory.DetourBase.DetourBoxBase
			{
				public ManagedDetourBox(PlatformTriple triple, MethodBase src, MethodBase target)
					: base(triple)
				{
					this.src = src;
					this.target = target;
					this.Detour = null;
				}

				public void SubscribeCompileMethod()
				{
					PlatformTripleDetourFactory.Detour.AddRelatedDetour(this.src, this);
				}

				public void UnsubscribeCompileMethod()
				{
					PlatformTripleDetourFactory.Detour.RemoveRelatedDetour(this.src, this);
				}

				public void OnMethodCompiled(MethodBase method, IntPtr codeStart, IntPtr codeStartRw, ulong codeSize)
				{
					if (!base.IsApplied)
					{
						return;
					}
					method = this.Triple.GetIdentifiable(method);
					object sync = this.Sync;
					lock (sync)
					{
						if (base.IsApplied)
						{
							if (!base.IsApplying)
							{
								bool flag2;
								<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(43, 4, out flag2);
								if (flag2)
								{
									debugLogTraceStringHandler.AppendLiteral("Updating detour from ");
									debugLogTraceStringHandler.AppendFormatted<MethodBase>(this.src);
									debugLogTraceStringHandler.AppendLiteral(" to ");
									debugLogTraceStringHandler.AppendFormatted<MethodBase>(this.target);
									debugLogTraceStringHandler.AppendLiteral(" (recompiled ");
									debugLogTraceStringHandler.AppendFormatted<MethodBase>(method);
									debugLogTraceStringHandler.AppendLiteral(" to ");
									debugLogTraceStringHandler.AppendFormatted<IntPtr>(codeStart, "x16");
									debugLogTraceStringHandler.AppendLiteral(")");
								}
								<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
								try
								{
									base.IsApplying = true;
									SimpleNativeDetour detour = this.Detour;
									IntPtr intPtr;
									IntPtr intPtr2;
									IntPtr intPtr3;
									if (detour != null)
									{
										IntPtr source = detour.Source;
										intPtr = detour.Destination;
										intPtr2 = codeStart;
										intPtr3 = codeStartRw;
									}
									else
									{
										intPtr2 = codeStart;
										intPtr3 = codeStartRw;
										intPtr = this.Triple.Runtime.GetMethodHandle(this.target).GetFunctionPointer();
									}
									SimpleNativeDetour simpleNativeDetour = this.Triple.CreateSimpleDetour(intPtr2, intPtr, (int)codeSize, intPtr3);
									SimpleNativeDetour simpleNativeDetour2;
									PlatformTripleDetourFactory.DetourBase.ReplaceDetourInLock(this, simpleNativeDetour, out simpleNativeDetour2);
								}
								finally
								{
									base.IsApplying = false;
								}
							}
						}
					}
				}

				private readonly MethodBase src;

				private readonly MethodBase target;
			}

			[Nullable(0)]
			private sealed class RelatedDetourBag
			{
				public RelatedDetourBag(MethodBase method)
				{
					this.Method = method;
				}

				public readonly MethodBase Method;

				public readonly List<PlatformTripleDetourFactory.Detour.ManagedDetourBox> RelatedDetours = new List<PlatformTripleDetourFactory.Detour.ManagedDetourBox>();

				public bool IsValid = true;
			}

			[CompilerGenerated]
			private static class <>O
			{
				[Nullable(0)]
				public static OnMethodCompiledCallback <0>__OnMethodCompiled;
			}
		}

		[NullableContext(0)]
		private sealed class NativeDetour : PlatformTripleDetourFactory.DetourBase, ICoreNativeDetour, ICoreDetourBase, IDisposable
		{
			public IntPtr Source
			{
				get
				{
					return this.DetourBox.From;
				}
			}

			public IntPtr Target
			{
				get
				{
					return this.DetourBox.To;
				}
			}

			public bool HasOrigEntrypoint
			{
				get
				{
					return this.OrigEntrypoint != IntPtr.Zero;
				}
			}

			public IntPtr OrigEntrypoint { get; private set; }

			[Nullable(1)]
			private new PlatformTripleDetourFactory.NativeDetour.NativeDetourBox DetourBox
			{
				[NullableContext(1)]
				get
				{
					return base.GetDetourBox<PlatformTripleDetourFactory.NativeDetour.NativeDetourBox>();
				}
			}

			[NullableContext(1)]
			public NativeDetour(PlatformTriple triple, IntPtr from, IntPtr to)
				: base(triple)
			{
				this.DetourBox = new PlatformTripleDetourFactory.NativeDetour.NativeDetourBox(triple, from, to);
			}

			[NullableContext(1)]
			protected override SimpleNativeDetour CreateDetour()
			{
				bool flag;
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(32, 2, out flag);
				if (flag)
				{
					debugLogTraceStringHandler.AppendLiteral("Applying native detour from ");
					debugLogTraceStringHandler.AppendFormatted<IntPtr>(this.Source, "x16");
					debugLogTraceStringHandler.AppendLiteral(" to ");
					debugLogTraceStringHandler.AppendFormatted<IntPtr>(this.Target, "x16");
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
				SimpleNativeDetour simpleNativeDetour;
				IntPtr intPtr;
				IDisposable disposable;
				this.Triple.CreateNativeDetour(this.Source, this.Target, -1, (IntPtr)0).Deconstruct(out simpleNativeDetour, out intPtr, out disposable);
				SimpleNativeDetour simpleNativeDetour2 = simpleNativeDetour;
				IntPtr intPtr2 = intPtr;
				IDisposable disposable2 = disposable;
				IDisposable disposable3 = this.origHandle;
				disposable = disposable2;
				this.origHandle = disposable;
				this.OrigEntrypoint = intPtr2;
				return simpleNativeDetour2;
			}

			protected override void BeforeUndo()
			{
				bool flag;
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(31, 2, out flag);
				if (flag)
				{
					debugLogTraceStringHandler.AppendLiteral("Undoing native detour from ");
					debugLogTraceStringHandler.AppendFormatted<IntPtr>(this.Source, "x16");
					debugLogTraceStringHandler.AppendLiteral(" to ");
					debugLogTraceStringHandler.AppendFormatted<IntPtr>(this.Target, "x16");
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
			}

			protected override void AfterUndo()
			{
				this.OrigEntrypoint = IntPtr.Zero;
				IDisposable disposable = this.origHandle;
				if (disposable != null)
				{
					disposable.Dispose();
				}
				this.origHandle = null;
			}

			protected override void BeforeDispose()
			{
			}

			[Nullable(2)]
			private IDisposable origHandle;

			private sealed class NativeDetourBox : PlatformTripleDetourFactory.DetourBase.DetourBoxBase
			{
				[NullableContext(1)]
				public NativeDetourBox(PlatformTriple triple, IntPtr from, IntPtr to)
					: base(triple)
				{
					this.From = from;
					this.To = to;
				}

				public readonly IntPtr From;

				public readonly IntPtr To;
			}
		}
	}
}
