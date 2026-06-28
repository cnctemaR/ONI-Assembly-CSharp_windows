using System;
using System.Collections.Generic;

namespace UnityEngine.Networking
{
	[AddComponentMenu("Network/NetworkProximityChecker")]
	[RequireComponent(typeof(NetworkIdentity))]
	public class NetworkProximityChecker : NetworkBehaviour
	{
		private void Update()
		{
			if (NetworkServer.active)
			{
				if (Time.time - this.m_VisUpdateTime > this.visUpdateInterval)
				{
					base.GetComponent<NetworkIdentity>().RebuildObservers(false);
					this.m_VisUpdateTime = Time.time;
				}
			}
		}

		public override bool OnCheckObserver(NetworkConnection newObserver)
		{
			bool flag;
			if (this.forceHidden)
			{
				flag = false;
			}
			else
			{
				GameObject gameObject = null;
				for (int i = 0; i < newObserver.playerControllers.Count; i++)
				{
					PlayerController playerController = newObserver.playerControllers[i];
					if (playerController != null && playerController.gameObject != null)
					{
						gameObject = playerController.gameObject;
						break;
					}
				}
				if (gameObject == null)
				{
					flag = false;
				}
				else
				{
					Vector3 position = gameObject.transform.position;
					flag = (position - base.transform.position).magnitude < (float)this.visRange;
				}
			}
			return flag;
		}

		public override bool OnRebuildObservers(HashSet<NetworkConnection> observers, bool initial)
		{
			bool flag;
			if (this.forceHidden)
			{
				NetworkIdentity component = base.GetComponent<NetworkIdentity>();
				if (component.connectionToClient != null)
				{
					observers.Add(component.connectionToClient);
				}
				flag = true;
			}
			else
			{
				NetworkProximityChecker.CheckMethod checkMethod = this.checkMethod;
				if (checkMethod != NetworkProximityChecker.CheckMethod.Physics3D)
				{
					if (checkMethod != NetworkProximityChecker.CheckMethod.Physics2D)
					{
						flag = false;
					}
					else
					{
						foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(base.transform.position, (float)this.visRange))
						{
							NetworkIdentity component2 = collider2D.GetComponent<NetworkIdentity>();
							if (component2 != null && component2.connectionToClient != null)
							{
								observers.Add(component2.connectionToClient);
							}
						}
						flag = true;
					}
				}
				else
				{
					foreach (Collider collider in Physics.OverlapSphere(base.transform.position, (float)this.visRange))
					{
						NetworkIdentity component3 = collider.GetComponent<NetworkIdentity>();
						if (component3 != null && component3.connectionToClient != null)
						{
							observers.Add(component3.connectionToClient);
						}
					}
					flag = true;
				}
			}
			return flag;
		}

		public override void OnSetLocalVisibility(bool vis)
		{
			NetworkProximityChecker.SetVis(base.gameObject, vis);
		}

		private static void SetVis(GameObject go, bool vis)
		{
			foreach (Renderer renderer in go.GetComponents<Renderer>())
			{
				renderer.enabled = vis;
			}
			for (int j = 0; j < go.transform.childCount; j++)
			{
				Transform child = go.transform.GetChild(j);
				NetworkProximityChecker.SetVis(child.gameObject, vis);
			}
		}

		public int visRange = 10;

		public float visUpdateInterval = 1f;

		public NetworkProximityChecker.CheckMethod checkMethod = NetworkProximityChecker.CheckMethod.Physics3D;

		public bool forceHidden = false;

		private float m_VisUpdateTime;

		public enum CheckMethod
		{
			Physics3D,
			Physics2D
		}
	}
}
