using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.Audio;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Audio
{
	public readonly struct ProcessorInstance : IEquatable<ProcessorInstance>
	{
		public bool Equals(ProcessorInstance other)
		{
			return this.Handle.Equals(other.Handle);
		}

		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3;
				if (obj is ProcessorInstance)
				{
					ProcessorInstance processorInstance = (ProcessorInstance)obj;
					flag3 = this.Equals(processorInstance);
				}
				else
				{
					flag3 = false;
				}
				flag2 = flag3;
			}
			return flag2;
		}

		public static bool operator ==(ProcessorInstance a, ProcessorInstance b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(ProcessorInstance a, ProcessorInstance b)
		{
			return !a.Equals(b);
		}

		public override int GetHashCode()
		{
			return this.Handle.GetHashCode();
		}

		internal unsafe ProcessorInstance(Handle handle, ProcessorHeader* header)
		{
			this.Handle = handle;
			this.Header = header;
		}

		internal readonly Handle Handle;

		internal unsafe readonly ProcessorHeader* Header;

		public struct CreationParameters
		{
			[Obsolete("processorUpdateSetting has been deprecated. Use realtimeUpdateSetting instead.", true)]
			public ProcessorInstance.UpdateSetting processorUpdateSetting
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public ProcessorInstance.UpdateSetting controlUpdateSetting { readonly get; set; }

			public ProcessorInstance.UpdateSetting realtimeUpdateSetting { readonly get; set; }

			internal readonly ProcessorInstance.InitializationFlags BuildInitializationFlags()
			{
				ProcessorInstance.InitializationFlags initializationFlags = (ProcessorInstance.InitializationFlags)0U;
				bool flag = this.controlUpdateSetting == ProcessorInstance.UpdateSetting.UpdateIfDataIsAvailable;
				if (flag)
				{
					initializationFlags |= ProcessorInstance.InitializationFlags.UpdateControlIfDataIsAvailable;
				}
				else
				{
					bool flag2 = this.controlUpdateSetting == ProcessorInstance.UpdateSetting.UpdateAlways;
					if (flag2)
					{
						initializationFlags |= ProcessorInstance.InitializationFlags.UpdateControlAlways;
					}
				}
				bool flag3 = this.realtimeUpdateSetting == ProcessorInstance.UpdateSetting.UpdateIfDataIsAvailable;
				if (flag3)
				{
					initializationFlags |= ProcessorInstance.InitializationFlags.UpdateProcessorIfDataIsAvailable;
				}
				else
				{
					bool flag4 = this.realtimeUpdateSetting == ProcessorInstance.UpdateSetting.UpdateAlways;
					if (flag4)
					{
						initializationFlags |= ProcessorInstance.InitializationFlags.UpdateProcessorAlways;
					}
				}
				return initializationFlags;
			}
		}

		[Obsolete("IProcessor has been deprecated. Use IRealtime instead. (UnityUpgradable) -> ProcessorInstance/IRealtime", true)]
		public interface IProcessor
		{
		}

		[Obsolete("MessageStatus has been deprecated. Use Response instead. (UnityUpgradable) -> ProcessorInstance/Response", true)]
		public enum MessageStatus
		{

		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public interface IContext
		{
			ProcessorInstance.AvailableData GetAvailableData(Handle handle);

			unsafe bool SendData(Handle handle, void* data, int size, int align, long typehash);
		}

		public enum UpdateSetting
		{
			Default,
			NeverUpdate,
			UpdateIfDataIsAvailable,
			UpdateAlways
		}

		[Flags]
		internal enum InitializationFlags : uint
		{
			UpdateControlIfDataIsAvailable = 2U,
			UpdateControlAlways = 4U,
			UpdateProcessorIfDataIsAvailable = 8U,
			UpdateProcessorAlways = 16U
		}

		public struct UpdatedDataContext : ProcessorInstance.IContext
		{
			ProcessorInstance.AvailableData ProcessorInstance.IContext.GetAvailableData(Handle handle)
			{
				return default(ProcessorInstance.AvailableData);
			}

			unsafe bool ProcessorInstance.IContext.SendData(Handle handle, void* data, int size, int align, long typehash)
			{
				ScriptableProcessorBindings.ReturnDataFromProcessor(in this.Access, in handle, data, size, align, typehash);
				return true;
			}

			internal UpdatedDataContext(in RealtimeAccess access)
			{
				this.Access = access;
			}

			internal readonly RealtimeAccess Access;
		}

		public interface IRealtime
		{
			void Update(ProcessorInstance.UpdatedDataContext context, ProcessorInstance.Pipe pipe);
		}

		public interface IControl<[global::System.Runtime.CompilerServices.IsUnmanaged] TRealtime> where TRealtime : struct, ValueType, ProcessorInstance.IRealtime
		{
			void Dispose(ControlContext context, ref TRealtime realtime);

			void Update(ControlContext context, ProcessorInstance.Pipe pipe);

			ProcessorInstance.Response OnMessage(ControlContext context, ProcessorInstance.Pipe pipe, ProcessorInstance.Message message);
		}

		public ref struct Pipe
		{
			public readonly ProcessorInstance.AvailableData GetAvailableData<[global::System.Runtime.CompilerServices.IsUnmanaged] TAudioContext>(TAudioContext context) where TAudioContext : struct, ValueType, ProcessorInstance.IContext
			{
				bool flag = !this.DualThreadHandle.Valid;
				if (flag)
				{
					throw new InvalidOperationException("DualThreadHandle is not valid, cannot get available data.");
				}
				return (this.Head != null) ? new ProcessorInstance.AvailableData(this.Head) : context.GetAvailableData(this.DualThreadHandle);
			}

			public unsafe readonly bool SendData<[global::System.Runtime.CompilerServices.IsUnmanaged] TAudioContext, [global::System.Runtime.CompilerServices.IsUnmanaged] T>(TAudioContext context, in T data) where TAudioContext : struct, ValueType, ProcessorInstance.IContext where T : struct, ValueType
			{
				fixed (T* ptr = &data)
				{
					T* ptr2 = ptr;
					return context.SendData(this.DualThreadHandle, (void*)ptr2, sizeof(T), UnsafeUtility.AlignOf<T>(), BurstRuntime.GetHashCode64<T>());
				}
			}

			internal unsafe Pipe(Handle dualThreadHandle, ProcessorInstance.AvailableData.Element* head = null)
			{
				this.Head = head;
				this.DualThreadHandle = dualThreadHandle;
			}

			internal unsafe readonly ProcessorInstance.AvailableData.Element* Head;

			internal readonly Handle DualThreadHandle;
		}

		public ref struct Message
		{
			public readonly bool Is<T>()
			{
				return this.TypeHash == BurstRuntime.GetHashCode64<T>();
			}

			public unsafe readonly ref T Get<[global::System.Runtime.CompilerServices.IsUnmanaged] T>() where T : struct, ValueType
			{
				bool flag = !this.Is<T>();
				if (flag)
				{
					throw new InvalidCastException(string.Format("Message does not contain data of type {0}", typeof(T)));
				}
				return ref *(T*)this.Data;
			}

			internal long TypeHash;

			internal unsafe void* Data;

			internal IntPtr ManagedHandle;
		}

		public enum Response
		{
			Unhandled,
			Handled
		}

		public ref struct AvailableData
		{
			public unsafe ProcessorInstance.AvailableData.Element Current
			{
				get
				{
					return *this.m_CurrentElement;
				}
			}

			public ProcessorInstance.AvailableData GetEnumerator()
			{
				return this;
			}

			public unsafe bool MoveNext()
			{
				bool moveNextCalled = this.m_MoveNextCalled;
				if (moveNextCalled)
				{
					bool flag = this.m_CurrentElement == null;
					if (flag)
					{
						return false;
					}
					this.m_CurrentElement = this.m_CurrentElement->Next();
				}
				else
				{
					this.m_MoveNextCalled = true;
				}
				return this.m_CurrentElement != null;
			}

			internal unsafe AvailableData(ProcessorInstance.AvailableData.Element* element)
			{
				this.m_CurrentElement = element;
				this.m_MoveNextCalled = false;
			}

			private unsafe ProcessorInstance.AvailableData.Element* m_CurrentElement;

			private bool m_MoveNextCalled;

			public ref struct Element
			{
				public unsafe bool TryGetData<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(out T data) where T : struct, ValueType
				{
					long hashCode = BurstRuntime.GetHashCode64<T>();
					bool flag = hashCode == this.TypeHash;
					bool flag2;
					if (flag)
					{
						data = *(T*)this.m_Data;
						flag2 = true;
					}
					else
					{
						data = default(T);
						flag2 = false;
					}
					return flag2;
				}

				internal unsafe readonly ProcessorInstance.AvailableData.Element* Next()
				{
					return this.m_NextElement;
				}

				internal long TypeHash;

				private unsafe void* m_Data;

				private int m_Size;

				private int m_Align;

				private Handle m_AudioHandle;

				private unsafe ProcessorInstance.AvailableData.Element* m_NextElement;
			}
		}
	}
}
