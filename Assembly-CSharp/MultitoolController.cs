using System;
using System.Collections.Generic;
using UnityEngine;

public class MultitoolController : GameStateMachine<MultitoolController, MultitoolController.Instance, Worker>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.pre;
		base.Target(this.worker);
		this.root.ToggleSnapOn("dig");
		this.pre.Enter(delegate(MultitoolController.Instance smi)
		{
			smi.PlayPre();
			this.worker.Get<Facing>(smi).Face(this.workable.Get(smi).transform.GetPosition());
		}).OnAnimQueueComplete(this.loop);
		this.loop.Enter("PlayLoop", delegate(MultitoolController.Instance smi)
		{
			smi.PlayLoop();
		}).Enter("CreateHitEffect", delegate(MultitoolController.Instance smi)
		{
			smi.CreateHitEffect();
		}).Exit("DestroyHitEffect", delegate(MultitoolController.Instance smi)
		{
			smi.DestroyHitEffect();
		})
			.EventTransition(GameHashes.WorkerPlayPostAnim, this.pst, null);
		this.pst.Enter("PlayPost", delegate(MultitoolController.Instance smi)
		{
			smi.PlayPost();
		});
	}

	public static string[] GetAnimationStrings(Workable workable, Worker worker, string toolString = "dig")
	{
		string[][][] array;
		if (!MultitoolController.TOOL_ANIM_SETS.TryGetValue(toolString, out array))
		{
			array = new string[MultitoolController.ANIM_BASE.Length][][];
			MultitoolController.TOOL_ANIM_SETS[toolString] = array;
			for (int i = 0; i < array.Length; i++)
			{
				string[][] array2 = MultitoolController.ANIM_BASE[i];
				string[][] array3 = new string[array2.Length][];
				array[i] = array3;
				for (int j = 0; j < array3.Length; j++)
				{
					string[] array4 = array2[j];
					string[] array5 = new string[array4.Length];
					array3[j] = array5;
					for (int k = 0; k < array5.Length; k++)
					{
						array5[k] = array4[k].Replace("{verb}", toolString);
					}
				}
			}
		}
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		MultitoolController.GetTargetPoints(workable, worker, out zero2, out zero);
		Vector2 vector = new Vector2(zero.x - zero2.x, zero.y - zero2.y);
		Vector2 normalized = vector.normalized;
		float num = Vector2.Angle(new Vector2(0f, -1f), normalized);
		float num2 = Mathf.Lerp(0f, 1f, num / 180f);
		int num3 = array.Length;
		int num4 = (int)(num2 * (float)num3);
		num4 = Math.Min(num4, num3 - 1);
		NavType currentNavType = worker.GetComponent<Navigator>().CurrentNavType;
		int num5 = 0;
		if (currentNavType == NavType.Ladder)
		{
			num5 = 1;
		}
		else if (currentNavType == NavType.Pole)
		{
			num5 = 2;
		}
		else if (currentNavType == NavType.Hover)
		{
			num5 = 3;
		}
		return array[num4][num5];
	}

	private static void GetTargetPoints(Workable workable, Worker worker, out Vector3 source, out Vector3 target)
	{
		target = workable.GetTargetPoint();
		source = worker.transform.GetPosition();
		source.y += 0.7f;
	}

	public GameStateMachine<MultitoolController, MultitoolController.Instance, Worker, object>.State pre;

	public GameStateMachine<MultitoolController, MultitoolController.Instance, Worker, object>.State loop;

	public GameStateMachine<MultitoolController, MultitoolController.Instance, Worker, object>.State pst;

	public StateMachine<MultitoolController, MultitoolController.Instance, Worker, object>.TargetParameter worker;

	public StateMachine<MultitoolController, MultitoolController.Instance, Worker, object>.TargetParameter workable;

	private static readonly string[][][] ANIM_BASE = new string[][][]
	{
		new string[][]
		{
			new string[] { "{verb}_dn_pre", "{verb}_dn_loop", "{verb}_dn_pst" },
			new string[] { "ladder_{verb}_dn_pre", "ladder_{verb}_dn_loop", "ladder_{verb}_dn_pst" },
			new string[] { "pole_{verb}_dn_pre", "pole_{verb}_dn_loop", "pole_{verb}_dn_pst" },
			new string[] { "jetpack_{verb}_dn_pre", "jetpack_{verb}_dn_loop", "jetpack_{verb}_dn_pst" }
		},
		new string[][]
		{
			new string[] { "{verb}_diag_dn_pre", "{verb}_diag_dn_loop", "{verb}_diag_dn_pst" },
			new string[] { "ladder_{verb}_diag_dn_pre", "ladder_{verb}_loop_diag_dn", "ladder_{verb}_diag_dn_pst" },
			new string[] { "pole_{verb}_diag_dn_pre", "pole_{verb}_loop_diag_dn", "pole_{verb}_diag_dn_pst" },
			new string[] { "jetpack_{verb}_diag_dn_pre", "jetpack_{verb}_diag_dn_loop", "jetpack_{verb}_diag_dn_pst" }
		},
		new string[][]
		{
			new string[] { "{verb}_fwd_pre", "{verb}_fwd_loop", "{verb}_fwd_pst" },
			new string[] { "ladder_{verb}_pre", "ladder_{verb}_loop", "ladder_{verb}_pst" },
			new string[] { "pole_{verb}_pre", "pole_{verb}_loop", "pole_{verb}_pst" },
			new string[] { "jetpack_{verb}_fwd_pre", "jetpack_{verb}_fwd_loop", "jetpack_{verb}_fwd_pst" }
		},
		new string[][]
		{
			new string[] { "{verb}_diag_up_pre", "{verb}_diag_up_loop", "{verb}_diag_up_pst" },
			new string[] { "ladder_{verb}_diag_up_pre", "ladder_{verb}_loop_diag_up", "ladder_{verb}_diag_up_pst" },
			new string[] { "pole_{verb}_diag_up_pre", "pole_{verb}_loop_diag_up", "pole_{verb}_diag_up_pst" },
			new string[] { "jetpack_{verb}_diag_up_pre", "jetpack_{verb}_diag_up_loop", "jetpack_{verb}_diag_up_pst" }
		},
		new string[][]
		{
			new string[] { "{verb}_up_pre", "{verb}_up_loop", "{verb}_up_pst" },
			new string[] { "ladder_{verb}_up_pre", "ladder_{verb}_up_loop", "ladder_{verb}_up_pst" },
			new string[] { "pole_{verb}_up_pre", "pole_{verb}_up_loop", "pole_{verb}_up_pst" },
			new string[] { "jetpack_{verb}_up_pre", "jetpack_{verb}_up_loop", "jetpack_{verb}_up_pst" }
		}
	};

	private static Dictionary<string, string[][][]> TOOL_ANIM_SETS = new Dictionary<string, string[][][]>();

	public new class Instance : GameStateMachine<MultitoolController, MultitoolController.Instance, Worker, object>.GameInstance
	{
		public Instance(Workable workable, Worker worker, HashedString context, GameObject hit_effect)
			: base(worker)
		{
			this.hitEffectPrefab = hit_effect;
			worker.GetComponent<AnimEventHandler>().SetContext(context);
			base.sm.worker.Set(worker, base.smi);
			base.sm.workable.Set(workable, base.smi);
			this.anims = MultitoolController.GetAnimationStrings(workable, worker, "dig");
		}

		public void PlayPre()
		{
			base.sm.worker.Get<KAnimControllerBase>(base.smi).Play(this.anims[0], KAnim.PlayMode.Once, 1f, 0f);
		}

		public void PlayLoop()
		{
			KAnimControllerBase kanimControllerBase = base.sm.worker.Get<KAnimControllerBase>(base.smi);
			if (kanimControllerBase.currentAnim != this.anims[1])
			{
				base.sm.worker.Get<KAnimControllerBase>(base.smi).Play(this.anims[1], KAnim.PlayMode.Loop, 1f, 0f);
			}
		}

		public void PlayPost()
		{
			KAnimControllerBase kanimControllerBase = base.sm.worker.Get<KAnimControllerBase>(base.smi);
			if (kanimControllerBase.currentAnim != this.anims[2])
			{
				base.sm.worker.Get<KAnimControllerBase>(base.smi).Play(this.anims[2], KAnim.PlayMode.Once, 1f, 0f);
			}
		}

		public void UpdateHitEffectTarget()
		{
			if (this.hitEffect == null)
			{
				return;
			}
			Workable workable = base.sm.workable.Get<Workable>(base.smi);
			Worker worker = base.sm.worker.Get<Worker>(base.smi);
			AnimEventHandler component = worker.GetComponent<AnimEventHandler>();
			Vector3 targetPoint = workable.GetTargetPoint();
			worker.GetComponent<Facing>().Face(workable.transform.GetPosition());
			this.anims = MultitoolController.GetAnimationStrings(workable, worker, "dig");
			this.PlayLoop();
			component.SetTargetPos(targetPoint);
			component.UpdateWorkTarget(workable.GetTargetPoint());
			this.hitEffect.transform.SetPosition(targetPoint);
		}

		public void CreateHitEffect()
		{
			Worker worker = base.sm.worker.Get<Worker>(base.smi);
			Workable workable = base.sm.workable.Get<Workable>(base.smi);
			if (worker == null || workable == null)
			{
				return;
			}
			if (Grid.PosToCell(workable) != Grid.PosToCell(worker))
			{
				worker.Trigger(-673283254, null);
			}
			Diggable diggable = workable as Diggable;
			if (diggable)
			{
				Element targetElement = diggable.GetTargetElement();
				worker.Trigger(-1762453998, targetElement);
			}
			if (this.hitEffectPrefab == null)
			{
				return;
			}
			if (this.hitEffect != null)
			{
				this.DestroyHitEffect();
			}
			AnimEventHandler component = worker.GetComponent<AnimEventHandler>();
			Vector3 targetPoint = workable.GetTargetPoint();
			component.SetTargetPos(targetPoint);
			this.hitEffect = GameUtil.KInstantiate(this.hitEffectPrefab, targetPoint, Grid.SceneLayer.FXFront2, null, 0);
			KBatchedAnimController component2 = this.hitEffect.GetComponent<KBatchedAnimController>();
			this.hitEffect.SetActive(true);
			component2.sceneLayer = Grid.SceneLayer.FXFront2;
			component2.enabled = false;
			component2.enabled = true;
			component.UpdateWorkTarget(workable.GetTargetPoint());
		}

		public void DestroyHitEffect()
		{
			Worker worker = base.sm.worker.Get<Worker>(base.smi);
			if (worker != null)
			{
				worker.Trigger(-1559999068, null);
				worker.Trigger(939543986, null);
			}
			if (this.hitEffectPrefab == null)
			{
				return;
			}
			if (this.hitEffect == null)
			{
				return;
			}
			this.hitEffect.DeleteObject();
		}

		private GameObject hitEffectPrefab;

		private GameObject hitEffect;

		private string[] anims;

		private bool inPlace;
	}

	private enum DigDirection
	{
		dig_down,
		dig_up
	}
}
