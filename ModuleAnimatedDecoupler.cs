using System;
using System.Linq; 
using KSP;
using UnityEngine;

namespace AnimatedDecoupler
{
	public class ModuleAnimatedDecoupler : ModuleDecouple, IScalarModule
	{
		[KSPField]
		public string animationName = "";

		protected Animation anim;

		protected bool isDecoupling;

		protected bool isResetting;

		protected ModuleCargoBay cargoBay;

		[KSPField(isPersistant = true)]
		public bool animationComplete = false;

		public ModuleAnimatedDecoupler ():
		base()
		{
		}

		public override void OnAwake()
		{
			this.OnMovingEvent = new EventData<float, float>("ModuleAnimatedDecoupler.OnMovingEvent");
			this.OnStoppedEvent = new EventData<float>("ModuleAnimateDecoupler.OnStoppedEvent");
			base.OnAwake ();
		}

		public override void OnStart (StartState state)
		{
			GameEvents.onStageSeparation.Add (checkForDecoupling);
			GameEvents.onVesselWasModified.Add (OnVesselWasModified);
			cargoBay = part.FindModuleImplementing<ModuleCargoBay> ();
			base.OnStart (state);
			Debug.Log ("ModuleAnimatedDecoupler.OnStart(), isDecoupled = " + this.isDecoupled.ToString ());
			if (animationName != "")
			{
				anim = part.FindModelAnimators(animationName).FirstOrDefault();
				if ((object)this.anim == null)
				{
					Debug.Log ("ModuleAnimatedDecoupler: Animations not found");
				}
				else
				{
					Debug.Log ("ModuleAnimatedDecoupler.OnStart() - Animation found named " + animationName);
					// If Decoupled or animation already played then set animation to end.
					if (this.animationComplete || this.isDecoupled)
					{
						this.anim[animationName].normalizedTime = 1f;
					}
				}
			}
		}

		private void OnVesselWasModified (Vessel v)
		{
			if ((object)v != null && v == vessel)
			{
				if (!(isDecoupling || isDecoupled))
				{
					if((object)part.findAttachNode (this.explosiveNodeID).attachedPart == null)
					{
						isDecoupling = true;
						OnMoving.Fire (0f, 1f);
						OnStop.Fire (1f);
					}
				}
			}
		}

		private void checkForDecoupling(EventReport separationData)
		{
			if (separationData.eventType == FlightEvents.STAGESEPARATION && separationData.origin == this.part)
			{
				// PROBABLY got called because we decoupled, but no way to know because ModuleDecouple doesn't SET isDecoupled until after the event fires. 
				OnMoving.Fire (0f, 1f);
				if (animationName != "" && (object)anim != null && (!this.animationComplete || !this.anim.IsPlaying (animationName)))
				{
					this.anim.Play (animationName);
					this.OnMoving.Fire (0f, 1f);
					this.animationComplete = true;
					Debug.Log ("ModuleAnimatedDecoupler.onStageSeparation() triggered animation " + this.animationName);
				}
				this.isDecoupling = true;
				this.OnStop.Fire (1f);
			}
		}

		private void OnDestroy()
		{
			GameEvents.onStageSeparation.Remove (checkForDecoupling);
			GameEvents.onVesselWasModified.Remove (OnVesselWasModified);
		}

		//
		// Properties
		//
		private EventData <float, float> OnMovingEvent;
		private EventData <float> OnStoppedEvent;

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
