OrbitClash ReadMe/User-Manual

OrbitClash Homepage: http://orbitclash.googlecode.com
Author: Justin Weaver <piranther@gmail.com>
Date: April 2011

-------------------------------------------------------------------------------
1. License:

/* Copyright 2011 Justin Weaver
 *
 * This file is part of OrbitClash.
 *
 * OrbitClash is free software: you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free
 * Software Foundation, either version 3 of the License, or (at your option)
 * any later version.
 *
 * OrbitClash is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
 * more details.
 *
 * You should have received a copy of the GNU General Public License along with
 * OrbitClash.  If not, see <http://www.gnu.org/licenses/>.
 */

 
-------------------------------------------------------------------------------
2. Introduction:

	Facts:
		- OrbitClash is my very first video game!
		- It was inspired by the classic game Spacewar! 

	Specs:
		- Open source (GPLv3 License)
		- Written in C#
		- Uses SDL via the SDL.NET wrapper

	Features:
		- Sights, sounds, and spectacles: the likes of which test the 
		very limits of the imagination of even the most exceptional fruit bat.


-------------------------------------------------------------------------------
3. Player Controls:

Player One Controls (The Yellow Ship):
	Rotate-left: Left Arrow
	Rotate-right: Right Arrow
	Forward-thruster: Up Arrow
	Reverse-thruster: Down Arrow
	Fire-cannon: Right Alt
	
Player Two Controls (The Red Ship):
	Rotate-left: A
	Rotate-right: D
	Forward-thruster: W
	Reverse-thruster: S
	Fire-cannon: Left Control

	
-------------------------------------------------------------------------------
4. Configuration:

	Compile time only; for now.  See the 'Configuration.cs' file.

	
-------------------------------------------------------------------------------
5.Compiling from Source:

Use Microsoft Visual Studio 2010 Professional on Microsoft .NET 4.0 framework.


-------------------------------------------------------------------------------
6. Wish List:

- Graphic-effects when bullet hits ship
- Graphic-effects when ship hit ship
- Graphic-effects of ship damage level
- Configuration
- Joystick support
- Framerate-independent movement & predictive collision detection
- Ability to pause/unpause the game
- "Press 'H' for instructions' on main title screen
- Give SolidEntity objects "mass" to make physics more realistic
- Support higher resolutions
- Network play
- Power-ups
- Player profiles
- Choice of ships /w various specs
- AI to play against
- Installer for binary distribution


-------------------------------------------------------------------------------
7. References and Credits:

- SDL <http://www.libsdl.org/>
- SDL.NET <http://cs-sdl.sourceforge.net/>
- See "Images/Image Attribution.txt"
- See "Sounds/Sound Attribution.txt"
