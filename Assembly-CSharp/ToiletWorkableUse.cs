using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;

public class ToiletWorkableUse : Workable, IGameObjectEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.showProgressBar = true;
		this.resetProgressOnStop = true;
		this.attributeConverter = Db.Get().AttributeConverters.ToiletSpeed;
		base.SetWorkTime(8.5f);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		base.GetComponent<KAnimControllerBase>().Play(Workable.DefaultWorkAnims, KAnim.PlayMode.Loop);
		Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject);
		if (roomOfGameObject != null)
		{
			RoomType roomType = roomOfGameObject.roomType;
			foreach (KeyValuePair<string, string> keyValuePair in ToiletWorkableUse.roomEffects)
			{
				if (keyValuePair.Key == roomType.Id)
				{
					worker.GetComponent<Effects>().Add(keyValuePair.Value, true);
				}
				else
				{
					worker.GetComponent<Effects>().Remove(keyValuePair.Value);
				}
			}
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		AmountInstance amountInstance = Db.Get().Amounts.Bladder.Lookup(worker);
		amountInstance.SetValue(0f);
		this.timesUsed++;
		base.OnCompleteWork(worker);
	}

	[Serialize]
	public int timesUsed;

	private static Dictionary<string, string> roomEffects = new Dictionary<string, string>
	{
		{ "Latrine", "RoomLatrine" },
		{ "PlumbedBathroom", "RoomBathroom" }
	};
}
