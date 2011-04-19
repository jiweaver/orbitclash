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

/* $Id$
 *
 * Author: Justin Weaver
 * Date: Mar 2011
 * Description: The main game code.
 */

#endregion Header Comments

using System;
using System.Drawing;
using SdlDotNet.Audio;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using SdlDotNet.Particles;
using SdlDotNet.Particles.Manipulators;

namespace OrbitClash
{
    internal class OrbitClash : IDisposable
    {
        #region Fields

        // True after someone hits the spacebar to initially begin the game.
        private bool theGameHasBegun;

        // The background surface.
        private Surface background;

        // The InfoBar surface.
        private Surface infoBar;

        // The main title screen.
        private MainTitle mainTitle;

        // The two players.
        private Player player1;
        private Player player2;

        // The planet.
        private Planet planet;

        // The main particle system.
        private ParticleSystem particleSystem;

        // Particle manipulators.
        private ParticleBoundary boundaryManipulator;
        private SpeedLimit speedLimitManipulator;
        private GravityWell gravityManipulator;

        // Collision sounds.
        private Sound bulletShipImpactSound;
        private Sound bulletShipImpactBounceSound;
        private Sound bulletPlanetImpactSound;
        private Sound shipShipImpactSound;

        #endregion Fields

        #region Properties

        public static string Title
        {
            get
            {
                return Configuration.Title;
            }
        }

        #endregion Properties

        #region Constructors

        public OrbitClash()
        {
            Video.WindowIcon();

            Video.SetVideoMode(Configuration.DisplaySize.Width, Configuration.DisplaySize.Height, false);

            Video.WindowCaption = Configuration.Title;

            Mixer.ChannelsAllocated = Configuration.MaxSoundChannels;

            this.theGameHasBegun = false;

            this.particleSystem = new ParticleSystem();

            this.mainTitle = new MainTitle();

            Surface infoBarSurface = new Surface(Configuration.InfoBar.ImageFilename);
            this.infoBar = infoBarSurface.Convert(Video.Screen, true, false);

            // Player 1.
            this.player1 = new Player(1);
            this.player1.LeftKey = Configuration.Controls.Player1.Left;
            this.player1.RightKey = Configuration.Controls.Player1.Right;
            this.player1.UpKey = Configuration.Controls.Player1.Up;
            this.player1.DownKey = Configuration.Controls.Player1.Down;
            this.player1.FireKey = Configuration.Controls.Player1.Fire;

            // Player 2.
            this.player2 = new Player(2);
            this.player2.LeftKey = Configuration.Controls.Player2.Left;
            this.player2.RightKey = Configuration.Controls.Player2.Right;
            this.player2.UpKey = Configuration.Controls.Player2.Up;
            this.player2.DownKey = Configuration.Controls.Player2.Down;
            this.player2.FireKey = Configuration.Controls.Player2.Fire;

            this.bulletShipImpactSound = new Sound(Configuration.Bullets.BulletShipImpactSoundFilename);
            this.bulletShipImpactSound.Volume = Configuration.SoundVolume;

            this.bulletShipImpactBounceSound = new Sound(Configuration.Bullets.BulletShipImpactBounceSoundFilename);
            this.bulletShipImpactBounceSound.Volume = Configuration.SoundVolume;

            this.bulletPlanetImpactSound = new Sound(Configuration.Bullets.BulletPlanetImpactSoundFilename);
            this.bulletPlanetImpactSound.Volume = Configuration.SoundVolume;

            this.shipShipImpactSound = new Sound(Configuration.Ships.ShipShipImpactSoundFilename);
            this.shipShipImpactSound.Volume = Configuration.SoundVolume;

            // Setup screen bounce-boundary manipulator.
            this.boundaryManipulator = new ParticleBoundary(new Rectangle(Configuration.PlayArea.Location, Configuration.PlayArea.Size));
            this.particleSystem.Manipulators.Add(this.boundaryManipulator);

            // Set up star-background.
            Surface backgroundSurface = new Surface(Configuration.PlayAreaBackgroundImageFilename);
            this.background = backgroundSurface.Convert(Video.Screen, true, false);

            // Setup the planet.
            Point planetLocation = new Point(Configuration.PlayArea.Width / 2, Configuration.PlayArea.Height / 2);
            this.planet = new Planet(Configuration.Planet.ImageFilename, Configuration.Planet.ImageTransparentColor, planetLocation, Configuration.Planet.ImageScale);
            this.particleSystem.Add(this.planet);

            // Draw the planetary halo onto the background.
            this.planet.DrawHalo(this.background);

            // Setup the gravity manipulator.
            this.gravityManipulator = new GravityWell(this.planet.Center, Configuration.Planet.GravityWellRadius, Configuration.Planet.GravityPower);
            this.particleSystem.Manipulators.Add(this.gravityManipulator);

            // Setup speed limit manipulator.
            this.speedLimitManipulator = new SpeedLimit(Configuration.UniversalSpeedLimit);
            this.particleSystem.Manipulators.Add(this.speedLimitManipulator);

            Events.KeyboardDown += new EventHandler<KeyboardEventArgs>(this.KeyboardDown);
            Events.KeyboardUp += new EventHandler<KeyboardEventArgs>(this.KeyboardUp);

            Events.Fps = Configuration.Fps;

            Events.Tick += new EventHandler<TickEventArgs>(this.Tick);
            Events.Quit += new EventHandler<QuitEventArgs>(this.Quit);
        }

        #endregion Constructors

        #region Operations

        public void Go()
        {
            Events.Run();
        }

        #endregion Operations

        #region Tick

        private void Tick(object sender, TickEventArgs e)
        {
            DateTime now = DateTime.Now;

            // Read the "game has begun" flag.
            bool gameHasBegun = this.theGameHasBegun;

            // Fill in the background.
            Video.Screen.Fill(Color.Black);
            Video.Screen.Blit(this.background);

            // Update all the particles in the universe.
            this.particleSystem.Update();

            if (gameHasBegun)
            {
                // The game has begun.

                // Spawn player 1.
                if (this.player1.Ship == null
                    || !this.player1.Ship.Alive && this.player1.Ship.RespawnTime < now)
                    SpawnShip(1);

                // Spawn player 2.
                if (this.player2.Ship == null
                    || !this.player2.Ship.Alive && this.player2.Ship.RespawnTime < now)
                    SpawnShip(2);

                if (this.player1.Ship != null && this.player2.Ship != null)
                {
                    // Perform user control (input) related updates.

                    // Player 1.
                    Bullet bullet = this.player1.ProcessUserInput();
                    if (bullet != null)
                        this.particleSystem.Particles.Add(bullet);

                    // Player 2.
                    bullet = this.player2.ProcessUserInput();
                    if (bullet != null)
                        this.particleSystem.Particles.Add(bullet);

                    // Check for collisions.
                    EnforceCollisions();
                }
            }

            // Render all the particles in the universe.
            this.particleSystem.Render(Video.Screen);

            if (!gameHasBegun)
            {
                // The game hasn't actually started yet.

                // Display the title screen.
                this.mainTitle.Refresh();
            }
            else
            {
                // Display the plain InfoBar graphic.
                Video.Screen.Blit(this.infoBar, Configuration.InfoBarPosition);

                // Display the scoreCards on the InfoBar.
                this.player1.ScoreCard.Refresh(Video.Screen);
                this.player2.ScoreCard.Refresh(Video.Screen);
            }

            // Display the back-buffer onto the screen surface (double-buffering).
            Video.Screen.Update();
        }

        #region Ship Spawning

        private static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        private static double Distance(Point p1, Point p2)
        {
            return Distance(p1.X, p1.Y, p2.X, p2.Y);
        }

        private Point GetSafeSpawnPosition(Ship ship)
        {
            // Choose the corner furthest from the other ship.

            if (this.player1.Ship == null)
                return Configuration.Ships.Player1StartingScreenPosition;
            else if (this.player2.Ship == null)
                return Configuration.Ships.Player2StartingScreenPosition;

            Ship otherShip = (ship.Player.Number == 1) ? this.player2.Ship : this.player1.Ship;

            // Determine spawn point based on current enemy location.
            double otherShipDistanceFromStartPosition1 = Distance(otherShip.Center, Configuration.Ships.Player1StartingScreenPosition);
            double otherShipDistanceFromStartPosition2 = Distance(otherShip.Center, Configuration.Ships.Player2StartingScreenPosition);

            Point startPos = (otherShipDistanceFromStartPosition1 < otherShipDistanceFromStartPosition2) ? Configuration.Ships.Player2StartingScreenPosition : Configuration.Ships.Player1StartingScreenPosition;

            return startPos;
        }

        private void SpawnShip(int number)
        {
            Player player;
            SpriteSheet spriteSheet;
            string bulletImageFilename;
            double bulletImageScale;
            Color bulletImageTransparentColor;
            string infoBarShipImageFilename;
            Color infoBarShipImageTransparentColor;
            int cannonBarrelLength;
            int forwardThrusterEngineLength;
            int reverseThrusterEngineLength;
            int rotationAnimationDelay;
            Point scoreCardDisplayPosition;
            if (number == 1)
            {
                player = this.player1;
                spriteSheet = Configuration.Ships.Model1.SpriteSheet;
                bulletImageFilename = Configuration.Ships.Model1.BulletImageFilename;
                bulletImageScale = Configuration.Ships.Model1.BulletImageScale;
                bulletImageTransparentColor = Configuration.Ships.Model1.BulletImageTransparentColor;
                infoBarShipImageFilename = Configuration.Ships.Model1.InfoBarShipImageFilename;
                infoBarShipImageTransparentColor = Configuration.Ships.Model1.InfoBarShipImageTransparentColor;
                cannonBarrelLength = Configuration.Ships.Model1.CannonBarrelLength;
                forwardThrusterEngineLength = Configuration.Ships.Model1.ForwardThrusterEngineLength;
                reverseThrusterEngineLength = Configuration.Ships.Model1.ReverseThrusterEngineLength;
                rotationAnimationDelay = Configuration.Ships.Model1.RotationAnimationDelay;
                scoreCardDisplayPosition = new Point(Configuration.InfoBarPosition.X + Configuration.InfoBar.Player1ScoreCardDisplayPosition.X, Configuration.InfoBarPosition.Y + Configuration.InfoBar.Player1ScoreCardDisplayPosition.Y);
            }
            else
            {
                player = this.player2;
                spriteSheet = Configuration.Ships.Model2.SpriteSheet;
                bulletImageFilename = Configuration.Ships.Model2.BulletImageFilename;
                bulletImageScale = Configuration.Ships.Model2.BulletImageScale;
                bulletImageTransparentColor = Configuration.Ships.Model2.BulletImageTransparentColor;
                infoBarShipImageFilename = Configuration.Ships.Model2.InfoBarShipImageFilename;
                infoBarShipImageTransparentColor = Configuration.Ships.Model2.InfoBarShipImageTransparentColor;
                cannonBarrelLength = Configuration.Ships.Model2.CannonBarrelLength;
                forwardThrusterEngineLength = Configuration.Ships.Model2.ForwardThrusterEngineLength;
                reverseThrusterEngineLength = Configuration.Ships.Model2.ReverseThrusterEngineLength;
                rotationAnimationDelay = Configuration.Ships.Model2.RotationAnimationDelay;
                scoreCardDisplayPosition = new Point(Configuration.InfoBarPosition.X + Configuration.InfoBar.Player2ScoreCardDisplayPosition.X, Configuration.InfoBarPosition.Y + Configuration.InfoBar.Player2ScoreCardDisplayPosition.Y);
            }

            Point spawnPosition = GetSafeSpawnPosition(player.Ship);

            // Start the creation special effects.
            this.particleSystem.Particles.Add(player.CreationEffect.Create(new Point(spawnPosition.X + spriteSheet.FrameSize.Width / 2, spawnPosition.Y + spriteSheet.FrameSize.Height / 2)));

            Bitmap bulletImage = new Bitmap(bulletImageFilename);
            Surface bulletSurface = new Surface(bulletImage).Convert(Video.Screen, true, false);
            bulletSurface = bulletSurface.CreateScaledSurface(bulletImageScale);
            bulletSurface.Transparent = true;
            bulletSurface.TransparentColor = bulletImageTransparentColor;

            Bitmap infoBarShipImage = new Bitmap(infoBarShipImageFilename);
            Surface infoBarShipSurface = new Surface(infoBarShipImage).Convert(Video.Screen, true, false);
            infoBarShipSurface.Transparent = true;
            infoBarShipSurface.TransparentColor = infoBarShipImageTransparentColor;

            if (player.Ship != null)
                player.Ship.Dispose();

            player.Ship = new Ship(player, spriteSheet, spawnPosition, bulletSurface, cannonBarrelLength, forwardThrusterEngineLength, reverseThrusterEngineLength, rotationAnimationDelay, infoBarShipSurface);

            if (player.ScoreCard == null)
                player.ScoreCard = new ScoreCard(player, scoreCardDisplayPosition);

            this.particleSystem.Add(player.Ship);
        }

        #endregion Ship Spawning

        #region Collision Detection and Handling

        private void DamageShip(Ship ship, float points)
        {
            ship.Shields -= points;

            if (ship.Shields <= 0)
            {
                ship.Die();
                if (ship.Player.Number == 1)
                {
                    this.player1.ScoreCard.Defeats++;
                    this.player2.ScoreCard.Kills++;
                    this.particleSystem.Particles.Add(this.player1.ExplosionEffect.Explode(this.player1.Ship.Center));
                }
                else
                {
                    this.player2.ScoreCard.Defeats++;
                    this.player1.ScoreCard.Kills++;
                    this.particleSystem.Particles.Add(this.player2.ExplosionEffect.Explode(this.player2.Ship.Center));
                }
            }
        }

        private void Impact(Ship ship, Bullet bullet)
        {
            if (bullet.Owner == ship.Player)
            {
                // I own this particle, thus it won't hurt me.

                // Make it bounce.
                bullet.Velocity *= -1;

                try
                {
                    this.bulletShipImpactBounceSound.Play();
                }
                catch
                {
                    // Must be out of sound channels.
                }
            }
            else
            {
                bullet.Life = 0;

                try
                {
                    this.bulletShipImpactSound.Play();
                }
                catch
                {
                    // Must be out of sound channels.
                }

                DamageShip(ship, bullet.Power);
            }
        }

        private void Impact(Ship ship, Planet planet)
        {
            Player player = (ship.Player.Number == 1) ? this.player1 : this.player2;
            this.particleSystem.Particles.Add(player.ExplosionEffect.Explode(ship.Center));
            ship.Die();
            player.ScoreCard.Suicides++;
        }

        private void Impact(Ship ship1, Ship ship2)
        {
            Vector oldShip1Velocity = ship1.Velocity;
            Vector oldShip2Velocity = ship2.Velocity;

            Vector newVelocity1 = (oldShip1Velocity * -0.5f) + (oldShip2Velocity * 0.5f);
            Vector newVelocity2 = (oldShip2Velocity * -0.5f) + (oldShip1Velocity * 0.5f);

            ship1.Velocity = newVelocity1;
            ship2.Velocity = newVelocity2;

            DamageShip(ship1, Configuration.Ships.ShipShipCollisionDamage);
            DamageShip(ship2, Configuration.Ships.ShipShipCollisionDamage);

            try
            {
                this.shipShipImpactSound.Play();
            }
            catch
            {
                // Must be out of sound channels.
            }
        }

        private void Impact(Planet planet, Bullet bullet)
        {
            bullet.Life = 0;

            try
            {
                this.bulletPlanetImpactSound.Play();
            }
            catch
            {
                // Must be out of sound channels.
            }
        }

        private void EnforceCollisions()
        {
            // Enforce Ship-Planet Impact.
            if (this.player1.Ship.Alive && this.player1.Ship.Collision(this.planet))
                Impact(this.player1.Ship, this.planet);
            if (this.player2.Ship.Alive && this.player2.Ship.Collision(this.planet))
                Impact(this.player2.Ship, this.planet);

            // Enforce Ship-Ship Impact.
            if (this.player1.Ship.Alive && this.player2.Ship.Alive && this.player1.Ship.Collision(this.player2.Ship))
                Impact(this.player1.Ship, this.player2.Ship);

            /* Create a shallow copy of the bullet particles to work with;
             * otherwise things go all wonky when we add particles during
             * iteration (i.e. explosions).
             */
            ParticleCollection bulletParticles = new ParticleCollection();
            foreach (BaseParticle p in this.particleSystem.Particles)
                if ((Type)p.GetType() == typeof(Bullet))
                    bulletParticles.Add(p);

            foreach (Bullet b in bulletParticles)
            {
                // Enforce Ship-Bullet impact.
                if (this.player1.Ship.Collision(b))
                    Impact(this.player1.Ship, b);
                if (this.player2.Ship.Collision(b))
                    Impact(this.player2.Ship, b);

                // Enforce Planet-Bullet impact.
                if (this.planet.Collision(b))
                    Impact(this.planet, b);
            }
        }

        #endregion Collision Detection and Handling

        #endregion Tick

        #region Keypresses

        private void KeyboardDown(object sender, KeyboardEventArgs e)
        {
            if (!this.theGameHasBegun)
            {
                if (e.Key == Key.Space)
                    this.theGameHasBegun = true;

                return;
            }

            if (this.player1.CheckKeyPresses(e.Key))
                return;

            this.player2.CheckKeyPresses(e.Key);
        }

        private void KeyboardUp(object sender, KeyboardEventArgs e)
        {
            if (this.player1.CheckKeyReleases(e.Key))
                return;

            this.player2.CheckKeyReleases(e.Key);
        }

        #endregion Keypresses

        #region Quit

        private void Quit(object sender, QuitEventArgs e)
        {
            Events.QuitApplication();
        }

        #endregion Quit

        #region IDisposable

        private bool disposed;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Close()
        {
            Dispose();
        }

        ~OrbitClash()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.player1.Ship != null)
                    {
                        this.player1.Ship.Dispose();
                        this.player1.Ship = null;
                    }

                    if (this.player2.Ship != null)
                    {
                        this.player2.Ship.Dispose();
                        this.player2.Ship = null;
                    }

                    if (this.bulletShipImpactBounceSound != null)
                    {
                        this.bulletShipImpactBounceSound.Dispose();
                        this.bulletShipImpactBounceSound = null;
                    }

                    if (this.bulletPlanetImpactSound != null)
                    {
                        this.bulletPlanetImpactSound.Dispose();
                        this.bulletPlanetImpactSound = null;
                    }

                    if (this.bulletShipImpactSound != null)
                    {
                        this.bulletShipImpactSound.Dispose();
                        this.bulletShipImpactSound = null;
                    }

                    if (this.shipShipImpactSound != null)
                    {
                        this.shipShipImpactSound.Dispose();
                        this.shipShipImpactSound = null;
                    }
                }
                this.disposed = true;
            }
        }

        #endregion IDisposable
    }
}