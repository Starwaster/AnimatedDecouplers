using System;
using System.Linq;
using System.Collections;
using KSP;
using UnityEngine;

namespace AnimatedDecouplers
{
	public class ModuleAnimatedAnchoredDecoupler : ModuleAnchoredDecoupler, IScalarModule
	{
		[KSPField]
		public string animationName = "";
		
		protected Animation anim;
		
		protected bool isDecoupling;

		protected bool isResetting;

		ModuleCargoBay cargoBay;

		[KSPField()]
		public bool waitForAnimation = false;
		
		[KSPField(isPersistant = true)]
		public bool animationComplete = false;
		
		[KSPField]
		public int layer = 0;

        [KSPField]
        public string moduleID = "animatedAnchoredDecoupler";
		
		public ModuleAnimatedAnchoredDecoupler ():
		base()
		{
		}

		public new void DecoupleAction(KSPActionParam param)
		{
			if (waitForAnimation && anim != null)
			{
				anim.Play(animationName);
				isDecoupling = true;
				OnMoving.Fire (0f, 1f);
				StartCoroutine(DelayedDecouple());
			}
			else
				OnDecouple ();
		}
		
		public new void Decouple()
		{
			if (waitForAnimation && anim != null)
			{
				anim.Play(animationName);
				isDecoupling = true;
				OnMoving.Fire (0f, 1f);
				StartCoroutine(DelayedDecouple());
			}
			else
				OnDecouple ();
		}
		

		public override void OnAwake()
		{
			this.OnMovingEvent = new EventData<float, float>("ModuleAnimatedAnchoredDecoupler.OnMovingEvent");
			this.OnStoppedEvent = new EventData<float>("ModuleAnimatedAnchoredDecoupler.OnStoppedEvent");
			base.OnAwake ();
		}
		
		public override void OnStart (StartState state)
		{
			// TODO Consider deprecating checkForDecoupling; it should no longer be necessary
			GameEvents.onStageSeparation.Add (CheckForDecoupling);
			GameEvents.onVesselWasModified.Add (OnVesselWasModified);
			cargoBay = part.FindModuleImplementing<ModuleCargoBay> ();
			base.OnStart (state);
			Debug.Log ("ModuleAnimatedAnchoredDecoupler.OnStart(), isDecoupled = " + isDecoupled.ToString ());
			if (animationName != "")
			{
				anim = part.FindModelAnimators(animationName).FirstOrDefault ();
				if (anim == null)
				{
					Debug.Log ("ModuleAnimatedAnchoredDecoupler: Animations not found");
				}
				else
				{
					Debug.Log ("ModuleAnimatedAnchoredDecoupler.OnStart() - Animation found named " + animationName);
					if (this.animationComplete || this.isDecoupled)
					// If Decoupled or animation already played then set animation to end.
					{
						this.anim[animationName].normalizedTime = 1f;
					}
				}
			}
		}

		public override void OnActive()
		{
			if (staged)
			{
				if (waitForAnimation && anim != null)
				{
					anim.Play(animationName);
					isDecoupling = true;
					OnMoving.Fire (0f, 1f);
					StartCoroutine(DelayedDecouple());
				}
				else
					OnDecouple ();
			}
		}
		
		private void OnVesselWasModified (Vessel v)
		{
			if (v != null && v == vessel)
			{
				if (!(isDecoupling || isDecoupled))
				{
					
					Part p;
                    p = this.ExplosiveNode.attachedPart;
					if (p = null)
					{
						isDecoupling = true;
						OnMoving.Fire (0f, 1f);
						OnStop.Fire (1f);
					}
				}
			}
		}

		// TODO Consider deprecating checkForDecoupling; it should no longer be necessary
		private void CheckForDecoupling(EventReport separationData)
		{
			if (separationData.eventType == FlightEvents.STAGESEPARATION && separationData.origin == this.part)
			{
				// PROBABLY got called because we decoupled, but no way to know because ModuleAnchoredDecoupler doesn't SET isDecoupled until after the event fires. 
				OnMoving.Fire (0f, 1f);
				if (animationName != "" && anim != null && (!this.animationComplete || !this.anim.IsPlaying (animationName)))
				{
					this.anim.Play (animationName);
					this.animationComplete = true;
					Debug.Log ("ModuleAnimatedAnchoredDecoupler.onStageSeparation() triggered animation " + this.animationName);
				}
				this.isDecoupling = true;
				this.OnStop.Fire (1f);
			}
		}

		IEnumerator DelayedDecouple()
		{
			yield return new WaitForSeconds(EventTime);
			this.animationComplete = true;
			this.OnStop.Fire (1f);
			OnDecouple ();
		}
		
		public void OnDestroy()
		{
			GameEvents.onStageSeparation.Remove (CheckForDecoupling);
			GameEvents.onVesselWasModified.Remove (OnVesselWasModified);
		}
		
		//
		// Properties
		//
		private EventData <float, float> OnMovingEvent;
		private EventData <float> OnStoppedEvent;
		
		float EventTime
		{
			get
			{
				return anim[animationName].length / anim[animationName].speed;
			}
		}

		public bool CanMove
		{
			get
			{
				//return part.ShieldedFromAirstream;
				return true;
			}
		}
		
		public float GetScalar
		{
			get
			{
				return (isResetting || isDecoupling) ? 1f : 0f;
			}
		}

        public string ScalarModuleID
        {
            get
            {
                return this.moduleID;
            }
        }
      
        public EventData<float, float> OnMoving
		{
			get
			{
				return OnMovingEvent;
			}
		}
		
		public EventData<float> OnStop
		{
			get
			{
				return OnStoppedEvent;
			}
		}
		
		//
		// Methods
		//
		public bool IsMoving ()
		{
			return false;
		}
		
		public void SetScalar (float t)
		{
		}
		
		public void SetUIRead (bool state)
		{
		}
		
		public void SetUIWrite (bool state)
		{
		}
	}
}
