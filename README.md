# Leviathan_original

A simulated farming society in which settlements evolve
according to mutable rules in paradigms that spread in power-law patterns.
Most of this code dates from 2020, as a project for relearning programming
and implementing some ideas from research into societal collapse
and complex systems. Not well optimized,
but makes for a sometimes interesting simulation.

Serves as the base for a game version with procedurally generated terrain
(using FBM noisemaps and Voronoi tessellation) and go powers,
availible free here: https://acellular.itch.io/leviathan

###LEVIATHAN-0.18d###############################################

---Michael Tuck, 2022-----------------------
---contact@michaeltuck.com-----------------------


###DESC#########################################################

A model of a farming society made up of settlements (NOTE: called
leviathans in code) that organize themselves according to the rules
and assumptions spread by paradigms, based on a combination of
systems theory (specifially self-organized criticallity) and a grab
bag of ideas from collapse theory, social science and philosophy.

Leviathans create their own paradigms through mutation, but also adopt 
paradigms created by other leviathans, allowing the spread of paradigms 
according word-of-mouth expectations set by other leviathans.
The spread of paradigms from leviathan to leviathan forms power-law 
distributions, like those found in many natural and humans systems.


###RUN####################################################################

Tested in Unity Editor 2020.3.27f1
Unity download (free for noncommercial use): https://unity.com/products/unity-platform

TO RUN: Download this folder and add as a project in the Unity Hub after installing Unity.

Sample test scenes are included in the Scenes folder inside the Assets folder.

There are two general setups for the sample scenes. One uses a standard grid for
leviathans, the other splits leviathans into distinct areas with bottlenecks
in between (like mountain passes or land bridges) through which paradigms 
often have trouble spreading, sometimes helping prevent collapses 
from spreading from one region to another.

Tests A and B use basic mutation in which only land use rules are added
or removed during the creation of new paradigms.

Tests C and D add trait mutation to paradigms for 11 other parameters:
	influenceRadius: how far knowledge of paradigms spreads from leviathans
	sensitivity: how sensitive leviathans are to expectations not being met
	deathCom: the effect of deaths on leviathan comfort
	birthCom: the effect of births on leviathan comfort
	emmiCom: the effect of emmigration on leviathan comfort
	adoptThresh: how much better a competing paradigm must be relative to the
	             current paradigm to consider adopting it, in combination 
	             with comfort and expectations.
	mutationRate: used to calculate probability of creating a new paradigm 
	baseMimesis: used in probability calculation of memesis and mutation
	baseEmProb: the base probability of emmigration
	storageWant: desired food storage
	birthRate: do I need to explain this one?
(To set the start values of these parameters, modify the Paradigm object in the editor.)

Test E shows how the spread of paradigms remains
power-law even when mutation is fully random (instead of increasing in
likelihood as settlements become more uncomfortable)

Test F shows how the spread of paradigms again
remains power-law when using deterministic instead of stochastic
thresholds for paradigm adoption and mutation

Add yield multiplier fluctuations using Fractional brownian motion (FBM) in any test
using the checkbox and modifier in the Main Camera object.

HOT TIP: to keep the model running without the Unity window selected
turn on "Run in Background" in Project Settings


###PLT#############################################################

While running the model, output test results to this folder (a mess of .csv files) by 
pressing SPACE while in the game window in Unity or by adding a preset test and output
time in the Main Camera and Control object (0 lets the model run indefinitely.)

Use the included python script to chart output data, including the
history of the system and the distributions of paradigm spread. 

Plotting script tested in Python 3.9
Python libraries required: matplotlib, numpy, scipy

