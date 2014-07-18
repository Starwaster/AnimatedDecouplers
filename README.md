AnimatedDecouplers
==================

Extensions of KSP's decouplers that play animations


Configure the same way as you would standard decoupler modules but it now accepts animationName, which is the name of an animation on the model.

Requires a model with an animation.

For example:

	MODULE
	{
	    name = ModuleAnimatedDecoupler
	    ejectionForce = 200
	    explosiveNodeID = top
	    staged = false
      animationName = SDHI_Umbilical
	}
