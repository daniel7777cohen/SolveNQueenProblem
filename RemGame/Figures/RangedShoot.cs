using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using FarseerPhysics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Contacts;

namespace RemGame
{
    class RangedShoot
    {
        public int PathSegments { get; private set; }
        public float Thickness { get; }

        private Game game;
        private World world;

        // Rope elements
        Path path;
        PhysicsObject element;

        public RangedShoot(Game game, World world, float thickness = 1.0f)
        {
            this.game = game;
            this.world = world;

            this.PathSegments = -1;
            this.Thickness = thickness;
        }

        public bool Create(Vector2 start, Vector2 end)
        {
            if (path != null)
                Dispose();

            // Define a path
            path = new Path();

            path.Add(start);
            path.Add(end);

            float ropeLength = (start - end).Length();

            /*
                        foreach (Body body in bodies)
                        {
                            foreach (Fixture fixture in body.FixtureList)
                            {
                                fixture.CollisionCategories = Category.Cat3;
                                fixture.CollidesWith = Category.None;
                            }
                        }

                        Fixture endFixture = bodies[bodies.Count - 1].FixtureList[0];
                        endFixture.Shape.Density = 5.0f;
                        endFixture.Restitution = 1.0f;
                        endFixture.CollidesWith = Category.Cat1;
                        endFixture.OnCollision += OnCollisionWithSphere;

                        // Attach bodies together with revolute joints
                        PathManager.AttachBodiesWithRevoluteJoint(world, bodies, new Vector2(0.0f, -linkRadius), new Vector2(0.0f, linkRadius), false, false);
                        */ 
            return true;
        }
        

        /*
        public bool OnCollisionWithSphere(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            fixtureB.Body.ApplyLinearImpulse(fixtureA.Body.LinearVelocity * 1000.0f);
            fixtureA.OnCollision -= OnCollisionWithSphere;
            return true;
        }
        */
        #region IDisposable Members

        /// <summary>
        /// Removes the rope from the world.
        /// </summary>
        public void Dispose()
        {
            
        }

        #endregion

        /// <summary>
        /// Applies an impulse force to the end of the rope.
        /// </summary>
        /// <param name="impulse">The impulse force to be applied</param>
        public void ApplyLinearImpulse(ref Vector2 impulse)
        {
            
        }
    }
}
