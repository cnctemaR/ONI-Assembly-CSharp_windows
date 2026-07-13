using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Playables
{
	[RequiredByNativeCode]
	[NativeHeader("Modules/Director/ScriptBindings/DataPlayableOutput.bindings.h")]
	[NativeHeader("Modules/Director/ScriptBindings/DataPlayableOutputExtensions.bindings.h")]
	[NativeHeader("Modules/Director/DataPlayableOutput.h")]
	[NativeHeader("Runtime/Director/Core/HPlayableGraph.h")]
	[NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
	[StaticAccessor("DataPlayableOutputBindings", StaticAccessorType.DoubleColon)]
	internal struct DataPlayableOutput : IPlayableOutput
	{
		public Type GetStreamType()
		{
			return DataPlayableOutput.InternalGetType(ref this.m_Handle);
		}

		public bool GetConnectionChanged()
		{
			return DataPlayableOutput.InternalGetConnectionChanged(ref this.m_Handle);
		}

		public void ClearConnectionChanged()
		{
			DataPlayableOutput.InternalClearConnectionChanged(ref this.m_Handle);
		}

		public TDataStream GetDataStream<TDataStream>() where TDataStream : new()
		{
			object obj = DataPlayableOutput.InternalGetStream(ref this.m_Handle);
			bool flag = obj is TDataStream;
			TDataStream tdataStream;
			if (flag)
			{
				tdataStream = (TDataStream)((object)obj);
			}
			else
			{
				tdataStream = default(TDataStream);
			}
			return tdataStream;
		}

		public void SetDataStream<TDataStream>(TDataStream stream) where TDataStream : new()
		{
			Type streamType = this.GetStreamType();
			bool flag = !streamType.IsAssignableFrom(typeof(TDataStream));
			if (flag)
			{
				throw new ArgumentException(string.Format("{0} is of the wrong type. This output only accepts streams with type {1} or inheriting from type {2}", "stream", streamType, streamType), "stream");
			}
			DataPlayableOutput.InternalSetStream(ref this.m_Handle, stream);
		}

		public static DataPlayableOutput Create<TDataStream>(PlayableGraph graph, string name) where TDataStream : new()
		{
			PlayableOutputHandle playableOutputHandle;
			bool flag = !DataPlayableOutputExtensions.InternalCreateDataOutput(ref graph, name, typeof(TDataStream), out playableOutputHandle);
			DataPlayableOutput dataPlayableOutput;
			if (flag)
			{
				dataPlayableOutput = DataPlayableOutput.Null;
			}
			else
			{
				DataPlayableOutput dataPlayableOutput2 = new DataPlayableOutput(playableOutputHandle);
				dataPlayableOutput = dataPlayableOutput2;
			}
			return dataPlayableOutput;
		}

		internal DataPlayableOutput(PlayableOutputHandle handle)
		{
			bool flag = handle.IsValid();
			if (flag)
			{
				bool flag2 = !handle.IsPlayableOutputOfType<DataPlayableOutput>();
				if (flag2)
				{
					throw new InvalidCastException("Can't set handle: the playable is not a DataPlayableOutput.");
				}
			}
			this.m_Handle = handle;
		}

		public static DataPlayableOutput Null
		{
			get
			{
				return new DataPlayableOutput(PlayableOutputHandle.Null);
			}
		}

		public PlayableOutputHandle GetHandle()
		{
			return this.m_Handle;
		}

		public static implicit operator PlayableOutput(DataPlayableOutput output)
		{
			return new PlayableOutput(output.GetHandle());
		}

		public static explicit operator DataPlayableOutput(PlayableOutput output)
		{
			return new DataPlayableOutput(output.GetHandle());
		}

		public IDataPlayer GetPlayer()
		{
			return DataPlayableOutput.InternalGetPlayer(ref this.m_Handle) as IDataPlayer;
		}

		public void SetPlayer<TPlayer>(TPlayer player) where TPlayer : Object, IDataPlayer
		{
			DataPlayableOutput.InternalSetPlayer(ref this.m_Handle, player);
		}

		[NativeThrows]
		private static Object InternalGetPlayer(ref PlayableOutputHandle handle)
		{
			return Unmarshal.UnmarshalUnityObject<Object>(DataPlayableOutput.InternalGetPlayer_Injected(ref handle));
		}

		[NativeThrows]
		private static void InternalSetPlayer(ref PlayableOutputHandle handle, Object player)
		{
			DataPlayableOutput.InternalSetPlayer_Injected(ref handle, Object.MarshalledUnityObject.Marshal<Object>(player));
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Type InternalGetType(ref PlayableOutputHandle handle);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetStream(ref PlayableOutputHandle handle, object stream);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object InternalGetStream(ref PlayableOutputHandle handle);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalGetConnectionChanged(ref PlayableOutputHandle handle);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalClearConnectionChanged(ref PlayableOutputHandle handle);

		[RequiredByNativeCode]
		private static void Internal_CallOnPlayerChanged(PlayableOutputHandle handle, object previousPlayer, object currentPlayer)
		{
			DataPlayableOutput dataPlayableOutput = new DataPlayableOutput(handle);
			IDataPlayer dataPlayer = previousPlayer as IDataPlayer;
			bool flag = dataPlayer != null;
			if (flag)
			{
				dataPlayer.Release(dataPlayableOutput);
			}
			IDataPlayer dataPlayer2 = currentPlayer as IDataPlayer;
			bool flag2 = dataPlayer2 != null;
			if (flag2)
			{
				dataPlayer2.Bind(dataPlayableOutput);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InternalGetPlayer_Injected(ref PlayableOutputHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetPlayer_Injected(ref PlayableOutputHandle handle, IntPtr player);

		private PlayableOutputHandle m_Handle;
	}
}
