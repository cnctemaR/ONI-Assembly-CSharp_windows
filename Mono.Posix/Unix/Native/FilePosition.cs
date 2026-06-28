using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Mono.Unix.Native
{
	public sealed class FilePosition : MarshalByRefObject, IEquatable<FilePosition>, IDisposable
	{
		public FilePosition()
		{
			IntPtr intPtr = Stdlib.CreateFilePosition();
			if (intPtr == IntPtr.Zero)
			{
				throw new OutOfMemoryException("Unable to malloc fpos_t!");
			}
			this.pos = new HandleRef(this, intPtr);
		}

		internal HandleRef Handle
		{
			get
			{
				return this.pos;
			}
		}

		public void Dispose()
		{
			this.Cleanup();
			GC.SuppressFinalize(this);
		}

		private void Cleanup()
		{
			if (this.pos.Handle != IntPtr.Zero)
			{
				Stdlib.free(this.pos.Handle);
				this.pos = new HandleRef(this, IntPtr.Zero);
			}
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"(",
				base.ToString(),
				" ",
				this.GetDump(),
				")"
			});
		}

		private string GetDump()
		{
			if (FilePosition.FilePositionDumpSize <= 0)
			{
				return "internal error";
			}
			StringBuilder stringBuilder = new StringBuilder(FilePosition.FilePositionDumpSize + 1);
			if (Stdlib.DumpFilePosition(stringBuilder, this.Handle, FilePosition.FilePositionDumpSize + 1) <= 0)
			{
				return "internal error dumping fpos_t";
			}
			return stringBuilder.ToString();
		}

		public override bool Equals(object obj)
		{
			FilePosition filePosition = obj as FilePosition;
			return obj != null && !(filePosition == null) && this.ToString().Equals(obj.ToString());
		}

		public bool Equals(FilePosition value)
		{
			return object.ReferenceEquals(this, value) || this.ToString().Equals(value.ToString());
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		~FilePosition()
		{
			this.Cleanup();
		}

		public static bool operator ==(FilePosition lhs, FilePosition rhs)
		{
			return object.Equals(lhs, rhs);
		}

		public static bool operator !=(FilePosition lhs, FilePosition rhs)
		{
			return !object.Equals(lhs, rhs);
		}

		private static readonly int FilePositionDumpSize = Stdlib.DumpFilePosition(null, new HandleRef(null, IntPtr.Zero), 0);

		private HandleRef pos;
	}
}
