AnimatedDecouplers
==================

Extensions of KSP's decouplers that play animations


Configure the same way as you would standard decoupler modules but it now accepts animationName, which is the name of an animation on the model.

If the model has no animation or no animation name is configured then these function exactly as stock decouplers.

For example:

	MODULE
	{
	    name = ModuleAnimatedDecoupler
	    ejectionForce = 200
	    explosiveNodeID = top
	    staged = false
	    animationName = SDHI_Umbilical
	}
	
Additionally, if ModuleCargoBay is configured on the part and DeployModuleIndex has the module index for the decoupler provided (0 = first) then the part will shield enclosed parts in KSP 1.0 and beyond. (against aerothermodynamic forces)
