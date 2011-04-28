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
 * Description:  Holds info about the planet.
 */

#endregion Header Comments

using System;
using System.Drawing;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Primitives;
using SdlDotNet.Graphics.Sprites;
using SdlDotNet.Particles;

namespace OrbitClash
{
    internal class Planet : SolidEntity, IDisposable
    {
        #region Fields

        private Surface haloSurface;

        #endregion Fields

        #region Properties

        public Surface HaloSurface
        {
            get
            {
                return haloSurface;
            }
        }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Create a new Planet instance.
        /// </summary>
        /// <param name="imageFilename">The filename of the planet image to load.</param>
        /// <param name="bitmap_TransparentColor">The color in the image to treat as transparent.</param>
        /// <param name="position">The screen location of the center of the planet.</param>
        /// <param name="scale">The scale (size multiplier) of the image.</param>
        public Planet(string imageFilename, Color bitmap_TransparentColor, Point position, float scale)
            : base(GetPlanetSprite(imageFilename, bitmap_TransparentColor, scale))
        {
            //Surface surface = this.Sprite.Surface;
            this.X = position.X - this.Width / 2;
            this.Y = position.Y - this.Height / 2;
            this.Velocity = new Vector();
            this.Life = -1;
            this.Static = true;

            if (Configuration.Planet.ShowPlanetaryHalo)
            {
                // Prepare the planetary "halo" surface.
                Circle circle = new Circle(Configuration.Planet.GravityWellRadius, Configuration.Planet.GravityWellRadius, Configuration.Planet.GravityWellRadius);

                this.haloSurface = new Surface(Configuration.Planet.GravityWellRadius * 2, Configuration.Planet.GravityWellRadius * 2);
                this.haloSurface.Alpha = Configuration.Planet.PlanetaryHaloAlpha;
                this.haloSurface.AlphaBlending = true;
                this.haloSurface.Draw(circle, Configuration.Planet.PlanetaryRimFillColor, true, false);
                this.haloSurface.Draw(circle, Configuration.Planet.PlanetaryHaloFillColor, true, true);

                // Convert it for display.
                this.haloSurface = this.haloSurface.Convert(Video.Screen, true, false);
            }
            else
            {
                this.haloSurface = null;
            }
        }

        private static Sprite GetPlanetSprite(string imageFilename, Color bitmap_TransparentColor, float scale)
        {
            Bitmap image = new Bitmap(imageFilename);
            Surface surface = new Surface(image);

            // Convert it for display.
            surface.Convert(Video.Screen, true, false);

            // Scale it.
            surface = surface.CreateScaledSurface(scale);

            // Set the transparent color.
            surface.Transparent = true;
            surface.TransparentColor = bitmap_TransparentColor;

            Sprite sprite = new Sprite(surface);

            return sprite;
        }

        #endregion Constructor

        #region IDisposable

        private bool disposed = false;

        ~Planet()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                // We haven't been disposed yet.

                if (disposing)
                {
                    /* The method has been called directly or indirectly by a
                     * user's code.  Dispose of managed resources here.
                     */
                    if (this.haloSurface != null)
                    {
                        this.haloSurface.Dispose();
                        this.haloSurface = null;
                    }
                }

                // Dispose of unmanaged resources _only_ out here.

                this.disposed = true;
            }
        }

        #endregion IDisposable
    }
}