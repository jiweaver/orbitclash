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
 * Description: Holds constants for configuration of the game.
 */

#endregion Header Comments

using System;
using System.Drawing;
using System.IO;
using SdlDotNet.Input;

namespace OrbitClash
{
    internal static class Configuration
    {
        #region General

        // Game title.
        public const string Title = "OrbitClash";

        // A hard cap on the maximum speed of every SolidEntity in the universe.
        public const float UniversalSpeedLimit = 3f;  // Pixels per Frame.

        public const int Fps = 30;

        public const int MaxSoundChannels = 500;

        public static int MaxLiveBulletsPerCannon = 10;

        public const int SoundVolume = 30; // (0-128)

        #endregion General

        #region Screen Layout

        // Video screen info.
        public static Size DisplaySize = new Size(640, 480);

        #region Main Title

        public static string MainTitleFontFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Fonts\Orbitron\Orbitron-bold.otf");
        public const int MainTitleFontSize = 80;
        public static Color MainTitleFontColor = Color.Blue;
        public static Point MainTitleCenterOffset = new Point(0, -100);

        public static string CreditMessageFontFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Fonts\Orbitron\Orbitron-light.otf");
        public static int CreditMessageFontSize = 20;
        public static Color CreditMessageFontColor = Color.SeaGreen;
        public static string CreditMessage = "by Justin Weaver";
        public static Point CreditMessageCenterOffset = new Point(0, -60);

        public static string StartMessageFontFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Fonts\Orbitron\Orbitron-light.otf");
        public const int StartMessageFontSize = 24;
        public static Color StartMessageFontColor = Color.Red;
        public const string StartMessage = "Press the spacebar to begin.";
        public static Point StartMessageCenterOffset = new Point(0, 100);

        #endregion Main Title

        #region InfoBar

        public static Size InfoBarSize = new Size(DisplaySize.Width, 40);
        public static Point InfoBarPosition = new Point(0, DisplaySize.Height - InfoBarSize.Height);

        public static class InfoBar
        {
            public static string ImageFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Images\InfoBar.png");

            public static string PlayerStatusDisplayImageFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Images\ScoreCardBackground.png");

            public static Size PlayerStatusDisplaySize = new Size(156, 34);
            public static Point Player1ScoreCardDisplayPosition = new Point(3, 3);
            public static Point Player2ScoreCardDisplayPosition = new Point(Configuration.DisplaySize.Width - PlayerStatusDisplaySize.Width - 3, 3);

            // Indent within the scorecard space.
            public const int XBuffer = 3;
            public const int YBuffer = 2;

            public static string PlayerStatusDisplayFontFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Fonts\Orbitron\Orbitron-light.otf");
            public const int PlayerStatusDisplayFontSize = 10;

            public static int FirstColumn_PixelsToIndent = 6;
            public static int SecondColumn_PixelsToIndent = 70;
            public static Color CounterTextColor = Color.LightBlue;
        }

        #endregion InfoBar

        #region Play Area

        public static Rectangle PlayArea = new Rectangle(0, 0, DisplaySize.Width, DisplaySize.Height - InfoBarSize.Height);
        public static string PlayAreaBackgroundImageFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Images\Background.png");

        #endregion Play Area

        #endregion Screen Layout

        #region Player Controls

        public static class Controls
        {
            public static class Player1
            {
                public const Key Up = Key.UpArrow;
                public const Key Down = Key.DownArrow;
                public const Key Left = Key.LeftArrow;
                public const Key Right = Key.RightArrow;
                public const Key Fire = Key.RightAlt;
            }

            public static class Player2
            {
                public const Key Up = Key.W;
                public const Key Down = Key.S;
                public const Key Left = Key.A;
                public const Key Right = Key.D;
                public const Key Fire = Key.LeftControl;
            }
        }

        #endregion Player Controls

        #region Entities

        #region Planet

        public static class Planet
        {
            public static string ImageFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Images\Planet1.png");

            public static Color ImageTransparentColor = Color.FromArgb(0, 0, 0, 0);

            public const float ImageScale = 0.2f;

            /* The gravity strength multiplier.  The higher this is, the
             * stronger the gravity will be.  This number is the exact force
             * applied to a particle, if it were at the center of the well.
             * The outer edge of the well starts at 0 force.
             */
            public const float GravityPower = 105f;

            public static short GravityWellRadius = Convert.ToInt16(Configuration.PlayArea.Height / 2.5f);

            public static bool ShowPlanetaryHalo = true;
            public static Color PlanetaryHaloFillColor = Color.FromArgb(30, 255, 255, 255);
            public static Color PlanetaryRimFillColor = Color.FromArgb(45, 255, 255, 255);
            public static byte PlanetaryHaloAlpha = 150;
        }

        #endregion Planet

        #region Ships

        public static class Ships
        {
            public const float TopSpeed = 2.5f;

            /* Amount of damage a ship takes when it collides with another
             * ship.
             */
            public const float ShipShipCollisionDamage = 1;

            public static string ShipShipImpactSoundFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Sounds\ShipShipImpact.wav");

            #region Spawning

            public static Point Player1StartingScreenPosition = new Point(10, 80);
            public static Point Player2StartingScreenPosition = new Point(DisplaySize.Width - 40, DisplaySize.Height - 130);

            public static TimeSpan RespawnDelay = new TimeSpan(0, 0, 3);

            public static class Creation
            {
                public static string SoundFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Sounds\Warp.wav");
                // The life of the explosion.
                public const int Life = 10;
                // Particles per 1000 updates.
                public const double Frequency = 25000d;
                // Explosion individual particle duration.
                public const int LifeMin = 7;
                public const int LifeMax = 10;
                /* Used to decide when the particle should start dying out with alpha
                 * transparency.
                 */
                public const int LifeFullMin = 10;
                public const int LifeFullMax = 10;
                // Min/Max speed of emitted particles.
                public const float SpeedMin = 0.5f;
                public const float SpeedMax = 5.0f;
                public static Color MinColor = Color.DarkBlue;
                public static Color MaxColor = Color.LightBlue;
                public const short RadiusMin = 2;
                public const short RadiusMax = 4;
            }

            #endregion Spawning

            #region Shields

            public static class Shields
            {
                public const float Power = 5f;

                public static Color InfoDisplayStrongColor = Color.LawnGreen;
                public static Color InfoDisplayWeakColor = Color.Orange;
                public static Color InfoDisplayCriticalColor = Color.Red;

                public static Color InfoDisplayPlayerFontColor = Color.LimeGreen;
            }

            #endregion Shields

            #region Cannon

            public static class Cannon
            {
                public static string FiringSoundFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Sounds\CannonFire.wav");
                public const float Power = 1f;
                public static TimeSpan Cooldown = new TimeSpan(0, 0, 0, 0, 150);
                public const float MuzzleSpeed = 3f;
                public static Color InfoDisplayStrongBulletCountColor = Color.LawnGreen;
                public static Color InfoDisplayWeakBulletCountColor = Color.Orange;
                public static Color InfoDisplayCriticalBulletCountColor = Color.Red;

                public static string DryFireSoundFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Sounds\DryFire.wav");
            }

            #endregion Cannon

            #region Thrusters

            public static class Thrusters
            {
                public const int LifeMin = 1;
                public const int LifeMax = 6;

                public const float SpeedMin = 1;
                public const float SpeedMax = 8;

                // Particles per 1000 updates...
                public const double Frequency = 25000;

                #region Forward

                public static class Forward
                {
                    public static string SoundFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Sounds\ShipForwardThrusters.wav");

                    public const int ExhaustConeDeg = 30;

                    public static Color ParticleMinColor = Color.Red;
                    public static Color ParticleMaxColor = Color.White;

                    public const float Power = 0.0250f;
                }

                #endregion Forward

                #region Reverse

                public static class Reverse
                {
                    public static string SoundFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Sounds\ShipReverseThrusters.wav");

                    public const int ExhaustConeDeg = 60;

                    public static Color ParticleMinColor = Color.Blue;
                    public static Color ParticleMaxColor = Color.White;

                    public const float Power = 0.0125f;
                }

                #endregion Reverse
            }

            #endregion Thrusters

            #region Explosion

            public static class Explosion
            {
                // Death Explosion,
                public static string SoundFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Sounds\ShipExplosion.wav");
                // The life of the explosion.
                public const int Life = 13;
                // Particles per 1000 updates.
                public const double Frequency = 20000d;
                // Explosion individual particle duration.
                public const int LifeMin = 5;
                public const int LifeMax = 15;
                /* Used to decide when the particle should start dying out with alpha
                 * transparency.
                 */
                public const int LifeFullMin = 10;
                public const int LifeFullMax = 10;
                // Min/Max speed of emitted particles.
                public const float SpeedMin = 0.5f;
                public const float SpeedMax = 5.0f;
                public static Color MinColor = Color.Red;
                public static Color MaxColor = Color.Orange;
                public const short RadiusMin = 1;
                public const short RadiusMax = 2;
            }

            #endregion Explosion

            #region Models

            #region Model1

            public static class Model1
            {
                public static string InfoBarShipImageFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Images\Ship1.png");

                public static string SpriteSheetFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Images\Ship1 Sprite Sheet.png");

                public static SpriteSheet SpriteSheet = new SpriteSheet(SpriteSheetFilename, Color.FromArgb(255, 0, 0, 0), new Size(28, 28), 10, 270);

                public const int RotationAnimationDelay = 50;

                public const int CannonBarrelLength = 17;
                public const int ForwardThrusterEngineLength = 11;
                public const int ReverseThrusterEngineLength = 11;

                public static string BulletImageFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Images\Bullet1.png");
                public static Color BulletImageTransparentColor = Color.FromArgb(255, 0, 0, 0);
                public static double BulletImageScale = 0.25d;
                public static Color InfoBarShipImageTransparentColor = Color.FromArgb(255, 0, 0, 0);
            }

            #endregion Model1

            #region Model2

            public static class Model2
            {
                public static string InfoBarShipImageFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Images\Ship2.png");

                public static string SpriteSheetFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Images\Ship2 Sprite Sheet.png");

                public static SpriteSheet SpriteSheet = new SpriteSheet(SpriteSheetFilename, Color.FromArgb(255, 0, 0, 0), new Size(26, 26), 10, 270);

                public const int RotationAnimationDelay = 50;

                public const int CannonBarrelLength = 17;
                public const int ForwardThrusterEngineLength = 11;
                public const int ReverseThrusterEngineLength = 11;

                public static string BulletImageFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Images\Bullet2.png");
                public static Color BulletImageTransparentColor = Color.FromArgb(255, 0, 0, 0);
                public static double BulletImageScale = 0.25d;
                public static Color InfoBarShipImageTransparentColor = Color.FromArgb(255, 0, 0, 0);
            }

            #endregion Model2

            #endregion Models
        }

        #endregion Ships

        #region Bullets

        public static class Bullets
        {
            public static string BulletShipImpactBounceSoundFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Sounds\Bounce.wav");
            public static string BulletShipImpactSoundFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Sounds\BulletShipImpact.wav");
            public static string BulletPlanetImpactSoundFilename = Path.Combine(Directory.GetCurrentDirectory(), @"Sounds\BulletPlanetImpact.wav");

            public const int Life = 160;
        }

        #endregion Bullets

        #endregion Entities
    }
}