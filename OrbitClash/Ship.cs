#region License

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

#endregion License

#region Header Comments

/* $Id: Ship.cs 1995 2011-04-18 18:38:21Z weaver $
 *
 * Author: Justin Weaver
 * Date: Mar 2011
 * Description:  Holds info about a ship.
 */

#endregion Header Comments

using System;
using System.Drawing;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;
using SdlDotNet.Particles;

namespace OrbitClash
{
    internal class Ship : SolidEntity, IDisposable
    {
        #region Fields

        // The ship rotation sprite sheet.
        private SpriteSheet spriteSheet;

        // The ship is an animated sprite (for rotations).
        private AnimatedSprite animatedSprite;

        // The forward and reverse thrusters.
        private Thruster forwardThruster;
        private Thruster reverseThruster;

        // The ship's cannon.
        private Cannon cannon;

        private float shields;

        // The time the ship was spawned.
        private DateTime spawnTime;

        // If we are currently dead, we must wait to respawn until this time.
        private DateTime respawnTime;

        // The ship "photo" for the InfoBar.
        private Surface shipPhotoSurface;

        // The player that owns this ship.
        private Player player;

        // The ship's maximum speed.
        private float topSpeed;
        private SpeedLimit speedLimiter;

        #endregion Fields

        #region Properties

        public Player Player
        {
            get
            {
                return this.player;
            }
            set
            {
                this.player = value;
            }
        }

        public SpriteSheet SpriteSheet
        {
            get
            {
                return this.spriteSheet;
            }
            set
            {
                this.spriteSheet = value;
            }
        }

        public float Shields
        {
            get
            {
                return this.shields;
            }
            set
            {
                this.shields = value;
            }
        }

        public DateTime SpawnTime
        {
            get
            {
                return this.spawnTime;
            }
        }

        public DateTime RespawnTime
        {
            get
            {
                return this.respawnTime;
            }
            set
            {
                this.respawnTime = value;
            }
        }

        public Surface ShipPhotoSurface
        {
            get
            {
                return this.shipPhotoSurface;
            }
            set
            {
                this.shipPhotoSurface = value;
            }
        }

        public int AmmoCount
        {
            get
            {
                return this.cannon.AmmoCount;
            }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates a new Ship instance.
        /// </summary>
        /// <param name="player">The player who owns the ship.</param>
        /// <param name="spriteSheet">
        /// The Ship's rotational sprite sheet.
        /// </param>
        /// <param name="startingScreenPosition">
        /// The Ship's starting screen position.
        /// </param>
        /// <param name="bulletSurface">
        /// The surface for bullets fired by this ship.
        /// </param>
        /// <param name="cannonBarrelLength">
        /// The length (in pixels; from the pivot point) of the Ship's cannon
        /// barrel.  This determines where bullets will emerge from.
        /// </param>
        /// <param name="forwardThrusterEngineLength">
        /// The length (in pixels; from the pivot point) of the Ship's forward
        /// facing engine tube.  This determines where the exhaust cone will
        /// begin.
        /// </param>
        /// <param name="reverseThrusterEngineLength">
        /// The length (in pixels; from the pivot point) of the Ship's reverse
        /// facing engine tube.  This determines where the exhaust cone will
        /// begin.
        /// </param>
        /// <param name="rotationAnimationDelay">
        /// The delay between frames of the Ship's sprite sheet animation.
        /// </param>
        /// <param name="infoBarShipSurface">
        /// The surface that is displayed as the identifying icon in the
        /// InfoBar.
        /// </param>
        public Ship(Player player, SpriteSheet spriteSheet, Point startingScreenPosition, Surface bulletSurface, int cannonBarrelLength, int forwardThrusterEngineLength, int reverseThrusterEngineLength, double rotationAnimationDelay, Surface infoBarShipSurface)
            : base(GetAnimatedSprite(spriteSheet, rotationAnimationDelay))
        {
            this.player = player;

            // This is for the ship picture in the InfoBar.
            this.shipPhotoSurface = infoBarShipSurface;

            this.topSpeed = Configuration.Ships.TopSpeed;
            this.shields = Configuration.Ships.Shields.Power;

            this.X = startingScreenPosition.X;
            this.Y = startingScreenPosition.Y;
            this.Velocity = new Vector();
            this.Life = -1; // Infinite life span.

            // Our next scheduled respawn time (i.e. never).
            this.respawnTime = DateTime.MinValue;

            this.spriteSheet = spriteSheet;
            this.animatedSprite = (AnimatedSprite)this.Sprite;

            this.speedLimiter = new SpeedLimit(this.topSpeed);

            this.forwardThruster = new Thruster(this, false, Configuration.Ships.Thrusters.Forward.SoundFilename, Configuration.Ships.Thrusters.Forward.Power, forwardThrusterEngineLength, Configuration.Ships.Thrusters.Forward.ExhaustConeDeg, Configuration.Ships.Thrusters.Forward.ParticleMinColor, Configuration.Ships.Thrusters.Forward.ParticleMaxColor);

            this.reverseThruster = new Thruster(this, true, Configuration.Ships.Thrusters.Reverse.SoundFilename, Configuration.Ships.Thrusters.Reverse.Power, reverseThrusterEngineLength, Configuration.Ships.Thrusters.Reverse.ExhaustConeDeg, Configuration.Ships.Thrusters.Reverse.ParticleMinColor, Configuration.Ships.Thrusters.Reverse.ParticleMaxColor);

            this.cannon = new Cannon(this, cannonBarrelLength, bulletSurface, Configuration.Ships.Cannon.Power, Configuration.Ships.Cannon.Cooldown, Configuration.Ships.Cannon.MuzzleSpeed, Configuration.Bullets.Life);

            this.spawnTime = DateTime.Now;
        }

        private static AnimatedSprite GetAnimatedSprite(SpriteSheet spriteSheet, double rotationAnimationDelay)
        {
            SurfaceCollection spriteSheet_SurfaceCollection = new SurfaceCollection();
            Surface spriteSheet_Surface = new Surface(spriteSheet.Bitmap).Convert(Video.Screen, true, false);

            spriteSheet_SurfaceCollection.Add(spriteSheet_Surface, spriteSheet.FrameSize);

            AnimationCollection animationCollection = new AnimationCollection();
            animationCollection.Add(spriteSheet_SurfaceCollection);
            animationCollection.Delay = rotationAnimationDelay;

            AnimatedSprite animatedSprite = new AnimatedSprite(animationCollection);

            animatedSprite.TransparentColor = spriteSheet.TransparentColor;
            animatedSprite.Transparent = true;

            return animatedSprite;
        }

        #endregion Constructors

        #region Particle Operations

        public override bool Update()
        {
            this.forwardThruster.Update();
            this.reverseThruster.Update();

            // Enforce the Ship's speed limit.
            ParticleCollection particleCollection = new ParticleCollection();
            particleCollection.Add(this);
            this.speedLimiter.Manipulate(particleCollection);

            return base.Update();
        }

        public override void Render(Surface surface)
        {
            this.forwardThruster.Render(surface);
            this.reverseThruster.Render(surface);

            base.Render(surface);
        }

        #endregion Particle Operations

        #region Operations

        #region Thrusters

        public void BeginForwardThruster()
        {
            if (this.Alive)
                // Turn on the thruster graphic.
                this.forwardThruster.FireThruster();
        }

        public void EndForwardThruster()
        {
            // Turn off the forward thruster graphic.
            this.forwardThruster.EndThruster();
        }

        public void BeginReverseThruster()
        {
            if (this.Alive)
                // Turn on the thruster graphic.
                this.reverseThruster.FireThruster();
        }

        public void EndReverseThruster()
        {
            // Turn off the reverse thruster graphic.
            this.reverseThruster.EndThruster();
        }

        #endregion Thrusters

        #region Rotation

        public void BeginRotateRight()
        {
            this.animatedSprite.AnimateForward = true;
            this.animatedSprite.Animate = true;
        }

        public void BeginRotateLeft()
        {
            this.animatedSprite.AnimateForward = false;
            this.animatedSprite.Animate = true;
        }

        public void EndRotate()
        {
            this.animatedSprite.Animate = false;
        }

        #endregion Rotation

        #region Cannon

        public Bullet FireCannon()
        {
            if (this.Alive)
                return this.cannon.Fire();

            return null;
        }

        #endregion Cannon

        #region Death

        public void Die()
        {
            this.Life = 0;

            // Set shields to zero for the InfoBar display.
            this.shields = 0;

            this.forwardThruster.EndThruster();
            this.reverseThruster.EndThruster();

            this.respawnTime = DateTime.Now + Configuration.Ships.RespawnDelay;
        }

        #endregion Death

        #endregion Operations

        #region IDisposable

        private bool disposed;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Ship()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.cannon != null)
                    {
                        this.cannon.Dispose();
                        this.cannon = null;
                    }

                    if (this.forwardThruster != null)
                    {
                        this.forwardThruster.Dispose();
                        this.forwardThruster = null;
                    }

                    if (this.reverseThruster != null)
                    {
                        this.reverseThruster.Dispose();
                        this.reverseThruster = null;
                    }
                }

                this.disposed = true;
            }
        }

        #endregion IDisposable
    }
}