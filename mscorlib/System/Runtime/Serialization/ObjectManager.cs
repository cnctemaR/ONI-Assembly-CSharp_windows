using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public class ObjectManager
	{
		public ObjectManager(ISurrogateSelector selector, StreamingContext context)
		{
			this._selector = selector;
			this._context = context;
		}

		public virtual void DoFixups()
		{
			this._finalFixup = true;
			try
			{
				if (this._registeredObjectsCount < this._objectRecords.Count)
				{
					throw new SerializationException("There are some fixups that refer to objects that have not been registered");
				}
				ObjectRecord lastObjectRecord = this._lastObjectRecord;
				bool flag = true;
				ObjectRecord objectRecord2;
				for (ObjectRecord objectRecord = this._objectRecordChain; objectRecord != null; objectRecord = objectRecord2)
				{
					bool flag2 = !objectRecord.IsUnsolvedObjectReference || !flag;
					if (flag2)
					{
						flag2 = objectRecord.DoFixups(true, this, true);
					}
					if (flag2)
					{
						flag2 = objectRecord.LoadData(this, this._selector, this._context);
					}
					if (flag2)
					{
						if (objectRecord.OriginalObject is IDeserializationCallback)
						{
							this._deserializedRecords.Add(objectRecord);
						}
						SerializationCallbacks serializationCallbacks = SerializationCallbacks.GetSerializationCallbacks(objectRecord.OriginalObject.GetType());
						if (serializationCallbacks.HasDeserializedCallbacks)
						{
							this._onDeserializedCallbackRecords.Add(objectRecord);
						}
						objectRecord2 = objectRecord.Next;
					}
					else
					{
						if (objectRecord.ObjectInstance is IObjectReference && !flag)
						{
							if (objectRecord.Status == ObjectRecordStatus.ReferenceSolvingDelayed)
							{
								throw new SerializationException("The object with ID " + objectRecord.ObjectID + " could not be resolved");
							}
							objectRecord.Status = ObjectRecordStatus.ReferenceSolvingDelayed;
						}
						if (objectRecord != this._lastObjectRecord)
						{
							objectRecord2 = objectRecord.Next;
							objectRecord.Next = null;
							this._lastObjectRecord.Next = objectRecord;
							this._lastObjectRecord = objectRecord;
						}
						else
						{
							objectRecord2 = objectRecord;
						}
					}
					if (objectRecord == lastObjectRecord)
					{
						flag = false;
					}
				}
			}
			finally
			{
				this._finalFixup = false;
			}
		}

		internal ObjectRecord GetObjectRecord(long objectID)
		{
			ObjectRecord objectRecord = (ObjectRecord)this._objectRecords[objectID];
			if (objectRecord == null)
			{
				if (this._finalFixup)
				{
					throw new SerializationException("The object with Id " + objectID + " has not been registered");
				}
				objectRecord = new ObjectRecord();
				objectRecord.ObjectID = objectID;
				this._objectRecords[objectID] = objectRecord;
			}
			if (!objectRecord.IsRegistered && this._finalFixup)
			{
				throw new SerializationException("The object with Id " + objectID + " has not been registered");
			}
			return objectRecord;
		}

		public virtual object GetObject(long objectID)
		{
			if (objectID <= 0L)
			{
				throw new ArgumentOutOfRangeException("objectID", "The objectID parameter is less than or equal to zero");
			}
			ObjectRecord objectRecord = (ObjectRecord)this._objectRecords[objectID];
			if (objectRecord == null || !objectRecord.IsRegistered)
			{
				return null;
			}
			return objectRecord.ObjectInstance;
		}

		public virtual void RaiseDeserializationEvent()
		{
			for (int i = this._onDeserializedCallbackRecords.Count - 1; i >= 0; i--)
			{
				ObjectRecord objectRecord = (ObjectRecord)this._onDeserializedCallbackRecords[i];
				this.RaiseOnDeserializedEvent(objectRecord.OriginalObject);
			}
			for (int j = this._deserializedRecords.Count - 1; j >= 0; j--)
			{
				ObjectRecord objectRecord2 = (ObjectRecord)this._deserializedRecords[j];
				IDeserializationCallback deserializationCallback = objectRecord2.OriginalObject as IDeserializationCallback;
				if (deserializationCallback != null)
				{
					deserializationCallback.OnDeserialization(this);
				}
			}
		}

		public void RaiseOnDeserializingEvent(object obj)
		{
			SerializationCallbacks serializationCallbacks = SerializationCallbacks.GetSerializationCallbacks(obj.GetType());
			serializationCallbacks.RaiseOnDeserializing(obj, this._context);
		}

		private void RaiseOnDeserializedEvent(object obj)
		{
			SerializationCallbacks serializationCallbacks = SerializationCallbacks.GetSerializationCallbacks(obj.GetType());
			serializationCallbacks.RaiseOnDeserialized(obj, this._context);
		}

		private void AddFixup(BaseFixupRecord record)
		{
			record.ObjectToBeFixed.ChainFixup(record, true);
			record.ObjectRequired.ChainFixup(record, false);
		}

		public virtual void RecordArrayElementFixup(long arrayToBeFixed, int index, long objectRequired)
		{
			if (arrayToBeFixed <= 0L)
			{
				throw new ArgumentOutOfRangeException("arrayToBeFixed", "The arrayToBeFixed parameter is less than or equal to zero");
			}
			if (objectRequired <= 0L)
			{
				throw new ArgumentOutOfRangeException("objectRequired", "The objectRequired parameter is less than or equal to zero");
			}
			ArrayFixupRecord arrayFixupRecord = new ArrayFixupRecord(this.GetObjectRecord(arrayToBeFixed), index, this.GetObjectRecord(objectRequired));
			this.AddFixup(arrayFixupRecord);
		}

		public virtual void RecordArrayElementFixup(long arrayToBeFixed, int[] indices, long objectRequired)
		{
			if (arrayToBeFixed <= 0L)
			{
				throw new ArgumentOutOfRangeException("arrayToBeFixed", "The arrayToBeFixed parameter is less than or equal to zero");
			}
			if (objectRequired <= 0L)
			{
				throw new ArgumentOutOfRangeException("objectRequired", "The objectRequired parameter is less than or equal to zero");
			}
			if (indices == null)
			{
				throw new ArgumentNullException("indices");
			}
			MultiArrayFixupRecord multiArrayFixupRecord = new MultiArrayFixupRecord(this.GetObjectRecord(arrayToBeFixed), indices, this.GetObjectRecord(objectRequired));
			this.AddFixup(multiArrayFixupRecord);
		}

		public virtual void RecordDelayedFixup(long objectToBeFixed, string memberName, long objectRequired)
		{
			if (objectToBeFixed <= 0L)
			{
				throw new ArgumentOutOfRangeException("objectToBeFixed", "The objectToBeFixed parameter is less than or equal to zero");
			}
			if (objectRequired <= 0L)
			{
				throw new ArgumentOutOfRangeException("objectRequired", "The objectRequired parameter is less than or equal to zero");
			}
			if (memberName == null)
			{
				throw new ArgumentNullException("memberName");
			}
			DelayedFixupRecord delayedFixupRecord = new DelayedFixupRecord(this.GetObjectRecord(objectToBeFixed), memberName, this.GetObjectRecord(objectRequired));
			this.AddFixup(delayedFixupRecord);
		}

		public virtual void RecordFixup(long objectToBeFixed, MemberInfo member, long objectRequired)
		{
			if (objectToBeFixed <= 0L)
			{
				throw new ArgumentOutOfRangeException("objectToBeFixed", "The objectToBeFixed parameter is less than or equal to zero");
			}
			if (objectRequired <= 0L)
			{
				throw new ArgumentOutOfRangeException("objectRequired", "The objectRequired parameter is less than or equal to zero");
			}
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			FixupRecord fixupRecord = new FixupRecord(this.GetObjectRecord(objectToBeFixed), member, this.GetObjectRecord(objectRequired));
			this.AddFixup(fixupRecord);
		}

		private void RegisterObjectInternal(object obj, ObjectRecord record)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (!record.IsRegistered)
			{
				record.ObjectInstance = obj;
				record.OriginalObject = obj;
				if (obj is IObjectReference)
				{
					record.Status = ObjectRecordStatus.ReferenceUnsolved;
				}
				else
				{
					record.Status = ObjectRecordStatus.ReferenceSolved;
				}
				if (this._selector != null)
				{
					record.Surrogate = this._selector.GetSurrogate(obj.GetType(), this._context, out record.SurrogateSelector);
					if (record.Surrogate != null)
					{
						record.Status = ObjectRecordStatus.ReferenceUnsolved;
					}
				}
				record.DoFixups(true, this, false);
				record.DoFixups(false, this, false);
				this._registeredObjectsCount++;
				if (this._objectRecordChain == null)
				{
					this._objectRecordChain = record;
					this._lastObjectRecord = record;
				}
				else
				{
					this._lastObjectRecord.Next = record;
					this._lastObjectRecord = record;
				}
				return;
			}
			if (record.OriginalObject != obj)
			{
				throw new SerializationException("An object with Id " + record.ObjectID + " has already been registered");
			}
		}

		public virtual void RegisterObject(object obj, long objectID)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj", "The obj parameter is null.");
			}
			if (objectID <= 0L)
			{
				throw new ArgumentOutOfRangeException("objectID", "The objectID parameter is less than or equal to zero");
			}
			this.RegisterObjectInternal(obj, this.GetObjectRecord(objectID));
		}

		public void RegisterObject(object obj, long objectID, SerializationInfo info)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj", "The obj parameter is null.");
			}
			if (objectID <= 0L)
			{
				throw new ArgumentOutOfRangeException("objectID", "The objectID parameter is less than or equal to zero");
			}
			ObjectRecord objectRecord = this.GetObjectRecord(objectID);
			objectRecord.Info = info;
			this.RegisterObjectInternal(obj, objectRecord);
		}

		public void RegisterObject(object obj, long objectID, SerializationInfo info, long idOfContainingObj, MemberInfo member)
		{
			this.RegisterObject(obj, objectID, info, idOfContainingObj, member, null);
		}

		public void RegisterObject(object obj, long objectID, SerializationInfo info, long idOfContainingObj, MemberInfo member, int[] arrayIndex)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj", "The obj parameter is null.");
			}
			if (objectID <= 0L)
			{
				throw new ArgumentOutOfRangeException("objectID", "The objectID parameter is less than or equal to zero");
			}
			ObjectRecord objectRecord = this.GetObjectRecord(objectID);
			objectRecord.Info = info;
			objectRecord.IdOfContainingObj = idOfContainingObj;
			objectRecord.Member = member;
			objectRecord.ArrayIndex = arrayIndex;
			this.RegisterObjectInternal(obj, objectRecord);
		}

		private ObjectRecord _objectRecordChain;

		private ObjectRecord _lastObjectRecord;

		private ArrayList _deserializedRecords = new ArrayList();

		private ArrayList _onDeserializedCallbackRecords = new ArrayList();

		private Hashtable _objectRecords = new Hashtable();

		private bool _finalFixup;

		private ISurrogateSelector _selector;

		private StreamingContext _context;

		private int _registeredObjectsCount;
	}
}
