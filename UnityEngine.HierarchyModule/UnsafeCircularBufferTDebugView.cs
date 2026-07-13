using System;

namespace Unity.Hierarchy
{
	internal sealed class UnsafeCircularBufferTDebugView<T> where T : class
	{
		public UnsafeCircularBufferTDebugView(CircularBuffer<T> buffer)
		{
			this.m_Buffer = buffer;
		}

		public T[] Items
		{
			get
			{
				return this.m_Buffer.ToArray();
			}
		}

		private readonly CircularBuffer<T> m_Buffer;
	}
}
