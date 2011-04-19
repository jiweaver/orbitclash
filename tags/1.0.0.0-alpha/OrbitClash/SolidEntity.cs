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
 * Description: Extendable abstract class to facilitate Collision detection
 * between particles.  This class is extended by several classes in the game
 * (e.g. Planet, Ship, and Bullet classes).
 *
 * --------------------------------
 * Notes About Collision Detection:
 *
 * Framerate independent movement, would accommodate slower machines, by moving
 * particles according to the elapsed time since the last screen-refresh
 * (Tick). Rather than moving them a static distance per screen-refresh.
 * However, such a system would also require some form of predictive detection
 * in order to prevent missed collisions.  Since I haven't done this yet, I set
 * a universal speed limit (enforced via ParticleManipulator, which ensures
 * that no particle can ever travel further than the limit in one Tick.  Thus,
 * missed collisions are very unlikely.
 *
 * Since I currently employ only after-the-fact detection, collided entities
 * will be overlapping when the collision is detected.  If the objects are
 * still overlapping at the next Tick, they will react as if a new collision
 * has occured.  To prevent this, I keep a live list of collisions that are
 * currently taking place.  Thus, those that have been handled once are ignored
 * subsequent times.  The live list is pruned each Tick as-well.
 */

#endregion Header Comments

using System;
using System.Collections.Generic;
using System.Drawing;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Graphics.Sprites;
using SdlDotNet.Particles;

namespace OrbitClash
{
    // Notice that this class extends SDL.NET's ParticleSprite class.
    internal abstract class SolidEntity : ParticleSprite
    {
        #region Fields

        // List of collisions currently in progress.
        protected List<SolidEntity> currentCollisions;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Get the current position of the center of the particle image
        /// </summary>
        public Point Center
        {
            get
            {
                /* Locate the center using the position of the current particle
                 * image's top-left corner, and its Size.  Use Truncate rather
                 * than Floor, Ceiling, or Round in order to avoid the
                 * possibility of breaching the boundaries of the image.
                 */
                Size size = this.Sprite.Size;
                int x = Convert.ToInt32(Math.Truncate(this.X)) + size.Width / 2;
                int y = Convert.ToInt32(Math.Truncate(this.Y)) + size.Height / 2;
                Point center = new Point(x, y);

                return center;
            }
        }

        /// <summary>
        /// True if this particle is alive (exists); false otherwise.
        /// </summary>
        public bool Alive
        {
            get
            {
                // Non-zero particle Life means: alive.
                return (this.Life != 0);
            }
        }

        #endregion Properties

        #region Constructors

        public SolidEntity(Sprite sprite)
            : base(sprite, 0, 0, new Vector(), 0)
        {
            this.currentCollisions = new List<SolidEntity>();
        }

        public SolidEntity(Sprite sprite, float x, float y, Vector vector, int life)
            : base(sprite, x, y, vector, life)
        {
            this.currentCollisions = new List<SolidEntity>();
        }

        #endregion Constructors

        #region Collision Detection

        /// <summary>
        /// Detect a collision between this entity and another.
        /// </summary>
        /// <param name="otherSolidEntity">
        /// The other entity to check for collision with this one.
        /// </param>
        /// <returns>
        /// True if the entities are colliding; false otherwise.
        /// </returns>
        public bool Collision(SolidEntity otherSolidEntity)
        {
            if (!this.Alive)
                // Dead entities don't collide, because they don't exist.
                return false;

            bool collisionStatus = SolidEntity.Collision(this, otherSolidEntity);
            bool alreadyCollided = this.currentCollisions.Contains(otherSolidEntity);

            if (!collisionStatus)
            {
                // No Collision detected with the other object.

                if (alreadyCollided)
                    /* Last we checked, we were in a Collision with this other
                     * object, so lets update our Collision list to reflect the
                     * fact that we are no-longer colliding with it.
                     */
                    this.currentCollisions.Remove(otherSolidEntity);

                return false;
            }

            // Collision detected

            if (!alreadyCollided)
            {
                // A brand new Collision!

                this.currentCollisions.Add(otherSolidEntity);
                return true;
            }

            /* A previous Collision with this other object hasn't ended
             * yet.
             */

            if (this.currentCollisions[this.currentCollisions.Count - 1] == otherSolidEntity)
            {
                /* This was the exact last object we collided with (in our
                 * list of currently occurring collisions),  so we treat
                 * this like it never happened.  You saw nothing!
                 */

                return false;
            }
            else
            {
                /* Update the list to reflect that we last collided with this
                 * object.  In other words, move it to the back of the list.
                 */
                this.currentCollisions.Remove(otherSolidEntity);
                this.currentCollisions.Add(otherSolidEntity);

                return true;
            }
        }

        /* Returns true if the two specified entities are colliding.  This
         * method has pixel-level accuracy.
         */
        private static bool Collision(SolidEntity entity1, SolidEntity entity2)
        {
            /* Immediately grab a reference to each particle sprite image
             * (Surface), so that any sprite animation will not trip us up by
             * changing surfaces under us.
             */
            Surface surface1 = entity1.Sprite.Surface;
            Surface surface2 = entity2.Sprite.Surface;

            // Find the position of each particle image.
            Point surface1Position = new Point(Convert.ToInt32(Math.Truncate(entity1.X)), Convert.ToInt32(Math.Truncate(entity1.Y)));
            Point surface2Position = new Point(Convert.ToInt32(Math.Truncate(entity2.X)), Convert.ToInt32(Math.Truncate(entity2.Y)));

            // Find the size of each particle image.
            Size surface1Size = new Size(Convert.ToInt32(Math.Truncate(entity1.Width)), Convert.ToInt32(Math.Truncate(entity1.Height)));
            Size surface2Size = new Size(Convert.ToInt32(Math.Truncate(entity2.Width)), Convert.ToInt32(Math.Truncate(entity2.Height)));

            // Put 'em together to make rectangles.
            Rectangle surface1Rec = new Rectangle(surface1Position, surface1Size);
            Rectangle surface2Rec = new Rectangle(surface2Position, surface2Size);

            if (!surface1Rec.IntersectsWith(surface2Rec))
                /* The two entity's bounding rectangles don't even intersect.
                 * They obviously aren't colliding.
                 */
                // False: entity1 and entity2 are not colliding.
                return false;

            /* Their rectangles do intersect, so now we need to check each
             * pixel in the overlapping area.
             */

            // Find the intersecting rectangle of the two entities.
            Rectangle intersectingRectangle = new Rectangle(surface1Rec.Location, surface1Rec.Size);
            intersectingRectangle.Intersect(surface2Rec);

            Point surface1PixelPosition = new Point();
            Point surface2PixelPosition = new Point();

            /* Lock the two entity's surfaces into memory to allow direct
             * pixel manipulation with high performance.
             */
            surface1.Lock();
            surface2.Lock();

            // Check each overlapping pixel.
            for (int x = 0; x < intersectingRectangle.Width; x++)
                for (int y = 0; y < intersectingRectangle.Height; y++)
                {
                    /* Get the position within each surface of the overlapping
                     * pixel.
                     */
                    surface1PixelPosition.X = intersectingRectangle.X - surface1Rec.X + x;
                    surface1PixelPosition.Y = intersectingRectangle.Y - surface1Rec.Y + y;
                    surface2PixelPosition.X = intersectingRectangle.X - surface2Rec.X + x;
                    surface2PixelPosition.Y = intersectingRectangle.Y - surface2Rec.Y + y;

                    /* Get the color from each surface at the overlapping
                     * pixel.  Unlike the GetPixel and SetPixel operations of
                     * the .NET Image class, the GetPixel and SetPixel
                     * operations of the C# SDL Surface class allow direct
                     * pixel manipulation, and do not create their own locks
                     * (hence the locks happen outside the loop).
                     */
                    Color surface1PixelColor = surface1.GetPixel(surface1PixelPosition);
                    Color surface2PixelColor = surface2.GetPixel(surface2PixelPosition);

                    /* True if the pixel at the overlapping point in the
                     * associated surface is transparent.
                     */
                    bool surface1PixelIsTransparent = surface1.Transparent && surface1PixelColor == surface1.TransparentColor;
                    bool surface2PixelIsTransparent = surface2.Transparent && surface2PixelColor == surface2.TransparentColor;

                    /* If both pixels are non-transparent, then we have a
                     * Collision!
                     */
                    if (!surface1PixelIsTransparent && !surface2PixelIsTransparent)
                    {
                        // Unlock the two surfaces before returning.
                        surface1.Unlock();
                        surface2.Unlock();

                        // True: entity1 and entity2 are colliding.
                        return true;
                    }
                }

            // Unlock the two surfaces before returning.
            surface1.Unlock();
            surface2.Unlock();

            // False: entity1 and entity2 are not colliding.
            return false;
        }

        #endregion Collision Detection

        #region Static Helper Operations

        /* Note: SDL.NET provides the Vector struct which is composed of an
         * origin point, a direction, and a magnitude (length). Vectors are
         * used to specify particle velocity, but they also make handy
         * abstractions for a few generally useful tasks (see below).
         */

        /// <summary>
        /// Calculate a screen position given various information.
        /// </summary>
        /// <param name="startingPosition">The origin position.</param>
        /// <param name="angleDeg">The angle in degrees.</param>
        /// <param name="distancePixels">The distance in pixels.</param>
        /// <returns>
        /// The position that lies the specified distance from the specified
        /// position, at the specified angle.
        /// </returns>
        public static Point GetPosition(Point startingPosition, int angleDeg, int distancePixels)
        {
            // Create a vector with the specified angle and length.
            Vector offsetVector = Vector.FromDirection(angleDeg, distancePixels);

            // Add the two vectors.
            Vector resultVector = new Vector(startingPosition.X, startingPosition.Y, 0) + offsetVector;

            // Return the resulting position.
            return resultVector.Point;
        }

        /// <summary>
        /// Find the distance between two SolidEntity objects.
        /// </summary>
        /// <param name="entity1">The first entity.</param>
        /// <param name="entity2">The second entity.</param>
        /// <returns>
        /// The distance (in pixels) between the centers of the two objects.
        /// </returns>
        public static double GetDistance(SolidEntity entity1, SolidEntity entity2)
        {
            // Create a vector between the two entities to find the distance.
            Vector v = new Vector(entity1.Center, entity2.Center);
            return v.Length;
        }

        /// <summary>
        /// Find the angle between two SolidEntity objects.
        /// </summary>
        /// <param name="entity1">The first (origin) entity.</param>
        /// <param name="entity2">The second (destination) entity.</param>
        /// <returns>
        /// The degree of the angle between the centers of the two entities
        /// (with entity1's center as the origin).
        /// </returns>
        public static int GetDirectionDeg(SolidEntity entity1, SolidEntity entity2)
        {
            // Create a vector between the two entities to find the angle.
            Vector v = new Vector(entity1.Center, entity2.Center);
            return Convert.ToInt32(Math.Truncate(v.DirectionDeg));
        }

        #endregion Static Helper Operations
    }
}