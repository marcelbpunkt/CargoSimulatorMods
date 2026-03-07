# Cargo Simulator Mods

Behind this super creative title hide the mods that I wrote for the game Cargo Simulator 25 (Steam/PC).
All mods are using the latest BepInEx 5 version and are configurable, but sadly not via the ConfigurationManager since
the UI won't show in this game.

## Installation

Every mod has its own subfolder in which the .dll file with the same name as the folder is located. After
[installing BepInEx](https://docs.bepinex.dev/articles/user_guide/installation/index.html), these .dll files
need to be put in the game's subfolder `BepInEx/plugins`.

## Source Code

Available at [github](https://github.com/marcelbpunkt/CargoSimulatorMods).

## License

All projects are under the [GNU GPL v3](https://www.gnu.org/licenses/gpl-3.0.en.html) license.

## Projects

### MakeItWork

This is currently the only project in this repository. It is still WIP and contains the following QoL tweaks (all of
which are configurable with at least an enable/disable flag):

**Implemented features:**

* Lights in the store and warehouse turn on automatically at a configurable in-game hour between 8am and 9pm.
* The vehicle camera does not "snap back" after 2 seconds when mouse-looking while driving.
* "Low supply" notifications only appear once the respective supply is empty (boxes, labels) or too low to use (tape).
This also causes the cashiers to fill up their supplies later and hence the "x2" leftover boxes to not appear at all.
* Customers switch to shorter queues when available.
* Tutorial messages and markers are completely disabled.
* Make chase cam follow vehicle turns even when the look direction is not the same as the driving direction
(similar to the SnowRunner chase cam)
* Zoom in and out in chase cam mode via scroll wheel.

**Planned:**

* Some bugfixes
	* Actually automatically continue partnerships
	* Fix weird UI behaviour when a partnership is active (e.g. displaying "vehicle x needed for this partnership"
	even though it's already active)
	* Fix carrier bug where they only load one vehicle in the warehouse but not the other two.
	* Fix another carrier bug where they do not even move to the warehouse unless you switch their role back and forth.
	* Fix player turning with open tablet while using a trolley/pallet truck
* Make cashiers choose the closest supply shelf for restocking their desk.
* Make vehicle requirements for partnerships "at least this vehicle" instead of "exactly this vehicle". No one
keeps the stupid cargo bike anymore once they upgrade to bigger ones.
* Make trolleys, pallets and pallet trucks loadable/unloadable on the truck through extra key binds.
* Make delivery areas interactable with unload key bind (including the aforementioned whole pallets).
	* If a whole pallet is unloaded, an empty pallet will be there ready for pick-up the next day.
* Rebalance repair costs so they scale with vehicle type and damage percentage so it actually incentivises the player
to keep the vehicles in one piece.
* Have employees make mistakes on lower levels. This could mean mistaping fragile packages, misplacing them in the
wrong vehicle or shelf, or cause driving accidents.
* Either rebalance employee wages or make a lot more customers come to the shop on higher reputation ratings so it is
actually worth maxing them out.
* Apply shelf filtering when unloading pallets/trolleys or putting in boxes manually.
* Make vehicles break down at a random low percentage instead of 0%. Maybe implement partial malfunctions like the
engine overheating until blowing completely, the vehicle pulling to the side etc.
* Introduce fines for driving offences when a police car is around.
* Make driving accidents damage packages as well (especially fragile ones)
* Make customers leave when they have to wait longer than a specified amount of time, randomised inside a range.
Maybe take queue length into account, e.g. lower the probability of leaving when they are closer to the front of the
queue.

**Maybe sometime in the distant future:**

* Anything visual or asset-related in general is absolute lowest priority because visuals are my dump skill and I only
have the pure code to change them and haven't added any new resources into any game so far.
	* "Unblock" NPC vehicles that somehow get stuck in front of intersections
	* Turn truck doors into a ramp or tail lift that you can push pallets over.
	* Make trolley/pallet truck movements more realistic (i.e. set the pivoting point to the other end)
	* Add nicer engine and transmission sounds for higher tier vehicles. The current sounds are ok for the cargo bike but
	not for the other vehicles. The truck needs big manly Diesel engine and pneumatic shift sounds!
	* Fix stereo ouch in truck cockpit view
	* Add a reversing cam into the cockpit
	* In cockpit view, improve visibility through side mirrors at night.
	* Big truck: move them out from behind the A column if possible
	* Make pedestrians go around a vehicle that is parked on the sideway as well as active delivery areas so they don't
	* Make empty pallets stackable (max. 10 high), preferrably through the load/unload keybinds from the pallet truck
	* Make carriers use pallets and trolleys wherever possible (within their area, i.e. either the store or the warehouse)
	* Apply physics to packages on vehicles, pallets and trolleys so they can always tip over, get pushed off etc.
	* Increase trolley and pallet hitboxes so they collide at their edges and get stuck faster so you have to keep that in
	mess up the packages.
	* Make NPC vehicles avoid crashing into the player.
	* Prevent employee vehicles from colliding when driving off simultaneously
* Generally improve traffic AI
	* Make them use both lanes. Nowhere is safe hrhr.
	* Make them use the correct lane when turning into another street.
	* Make traffic lights actually mean something
	* Make vehicles free up intersections faster
* Maybe don't stop the clock at 9pm but let it keep running 24/7 (maybe slower at night)
mind when setting up your delivery shelves in the store.
* Make cashiers halt their work if there are too many packages behind them.
* Add trash duty to carriers' work tasks
* Add controller support

**NOT planned:**

* Add Steam Cloud functionality because it cannot be activated from inside the game, it has to be done by the publisher
on the Steam UI.
