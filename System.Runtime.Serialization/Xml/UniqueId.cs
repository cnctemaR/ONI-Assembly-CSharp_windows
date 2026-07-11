using System;
using System.Collections.Generic;
using System.Security;

namespace System.Xml
{
	public class UniqueId
	{
		public UniqueId()
			: this(Guid.NewGuid())
		{
		}

		public UniqueId(byte[] id)
			: this(id, 0)
		{
		}

		public UniqueId(Guid id)
		{
			this.guid = id;
		}

		[SecurityCritical]
		[SecurityTreatAsSafe]
		public UniqueId(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value cannot be null", "value");
			}
			if (value.Length == 0)
			{
				throw new FormatException("UniqueId cannot be zero length");
			}
			this.id = value;
		}

		[SecurityCritical]
		[SecurityTreatAsSafe]
		public UniqueId(byte[] id, int offset)
		{
			if (id == null)
			{
				throw new ArgumentNullException();
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (offset >= id.Length)
			{
				throw new ArgumentException("id too small.", "offset");
			}
			if (id.Length - offset != 16)
			{
				throw new ArgumentException("id and offset provide less than 16 bytes");
			}
			if (offset == 0)
			{
				this.guid = new Guid(id);
			}
			else
			{
				List<byte> list = new List<byte>(id);
				list.RemoveRange(0, offset);
				this.guid = new Guid(list.ToArray());
			}
		}

		[SecurityCritical]
		[SecurityTreatAsSafe]
		public UniqueId(char[] id, int offset, int count)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (offset < 0 || offset >= id.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || id.Length - offset < count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (count == 0)
			{
				throw new FormatException();
			}
			if (count > 8 && id[offset] == 'u' && id[offset + 1] == 'r' && id[offset + 2] == 'n' && id[offset + 3] == ':' && id[offset + 4] == 'u' && id[offset + 5] == 'u' && id[offset + 6] == 'i' && id[offset + 7] == 'd' && id[offset + 8] == ':')
			{
				if (count != 45)
				{
					throw new ArgumentOutOfRangeException("Invalid Guid");
				}
				this.guid = new Guid(new string(id, offset + 9, count - 9));
			}
			else
			{
				this.id = new string(id, offset, count);
			}
		}

		public int CharArrayLength
		{
			[SecurityCritical]
			[SecurityTreatAsSafe]
			get
			{
				return (this.id == null) ? 45 : this.id.Length;
			}
		}

		public bool IsGuid
		{
			get
			{
				return this.guid != default(Guid);
			}
		}

		public override bool Equals(object obj)
		{
			UniqueId uniqueId = obj as UniqueId;
			if (uniqueId == null)
			{
				return false;
			}
			if (this.IsGuid && uniqueId.IsGuid)
			{
				return this.guid.Equals(uniqueId.guid);
			}
			return this.id == uniqueId.id;
		}

		[MonoTODO("Determine semantics when IsGuid==true")]
		public override int GetHashCode()
		{
			return (this.id == null) ? this.guid.GetHashCode() : this.id.GetHashCode();
		}

		[SecurityCritical]
		[SecurityTreatAsSafe]
		public int ToCharArray(char[] array, int offset)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (offset < 0 || offset >= array.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			string text = this.ToString();
			text.CopyTo(0, array, offset, text.Length);
			return text.Length;
		}

		[SecurityCritical]
		[SecurityTreatAsSafe]
		public override string ToString()
		{
			if (this.id == null)
			{
				return "urn:uuid:" + this.guid;
			}
			return this.id;
		}

		public bool TryGetGuid(out Guid guid)
		{
			if (this.IsGuid)
			{
				guid = this.guid;
				return true;
			}
			guid = default(Guid);
			return false;
		}

		[SecurityCritical]
		[SecurityTreatAsSafe]
		public bool TryGetGuid(byte[] buffer, int offset)
		{
			if (!this.IsGuid)
			{
				return false;
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset >= buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (buffer.Length - offset < 16)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			this.guid.ToByteArray().CopyTo(buffer, offset);
			return true;
		}

		public static bool operator ==(UniqueId id1, UniqueId id2)
		{
			return object.Equals(id1, id2);
		}

		public static bool operator !=(UniqueId id1, UniqueId id2)
		{
			return !(id1 == id2);
		}

		private Guid guid;

		private string id;
	}
}
