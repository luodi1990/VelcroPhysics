﻿using System.Collections.Generic;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.TestBed.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FarseerPhysics.TestBed.Tests
{
    public class CuttingTest : Test
    {
        private const float moveAmount = 0.1f;

        private const int Count = 20;
        private Vector2 _end = new Vector2(6, 5);
        private Vector2 _start = new Vector2(-6, 5);
        private bool switched;

        private CuttingTest()
        {
            //Ground
            FixtureFactory.CreateEdge(World, new Vector2(-40.0f, 0.0f), new Vector2(40.0f, 0.0f), 0);

            Vertices box = PolygonTools.CreateRectangle(0.5f, 0.5f);
            PolygonShape shape = new PolygonShape(box);

            Vector2 x = new Vector2(-7.0f, 0.75f);
            Vector2 deltaX = new Vector2(0.5625f, 1.25f);
            Vector2 deltaY = new Vector2(1.125f, 0.0f);

            for (int i = 0; i < Count; ++i)
            {
                Vector2 y = x;

                for (int j = i; j < Count; ++j)
                {
                    Body body = BodyFactory.CreateBody(World);
                    body.BodyType = BodyType.Dynamic;
                    body.Position = y;
                    body.CreateFixture(shape, 5);

                    y += deltaY;
                }

                x += deltaX;
            }
        }

        public override void Update(GameSettings settings, GameTime gameTime)
        {
            DebugView.DrawSegment(_start, _end, Color.Red);

            List<Fixture> fixtures = new List<Fixture>();
            List<Vector2> entryPoints = new List<Vector2>();
            List<Vector2> exitPoints = new List<Vector2>();

            //Get the entry points
            World.RayCast((f, p, n, fr) =>
                              {
                                  fixtures.Add(f);
                                  entryPoints.Add(p);
                                  return 1;
                              }, _start, _end);

            //Reverse the ray to get the exitpoints
            World.RayCast((f, p, n, fr) =>
                              {
                                  exitPoints.Add(p);
                                  return 1;
                              }, _end, _start);

            DebugView.DrawString(50, TextLine, "Fixtures: " + fixtures.Count);

            foreach (Vector2 entryPoint in entryPoints)
            {
                DebugView.DrawPoint(entryPoint, 0.5f, Color.Yellow);
            }

            foreach (Vector2 exitPoint in exitPoints)
            {
                DebugView.DrawPoint(exitPoint, 0.5f, Color.PowderBlue);
            }

            base.Update(settings, gameTime);
        }

        public override void Keyboard(KeyboardState state, KeyboardState oldState)
        {
            if (state.IsKeyDown(Keys.Tab) && oldState.IsKeyUp(Keys.Tab))
                switched = !switched;

            if (state.IsKeyDown(Keys.Enter) && oldState.IsKeyUp(Keys.Enter))
                CuttingTools.Cut(World, _start, _end, 0.001f);

            if (switched)
            {
                if (state.IsKeyDown(Keys.A))
                    _start.X -= moveAmount;

                if (state.IsKeyDown(Keys.S))
                    _start.Y -= moveAmount;

                if (state.IsKeyDown(Keys.W))
                    _start.Y += moveAmount;

                if (state.IsKeyDown(Keys.D))
                    _start.X += moveAmount;
            }
            else
            {
                if (state.IsKeyDown(Keys.A))
                    _end.X -= moveAmount;

                if (state.IsKeyDown(Keys.S))
                    _end.Y -= moveAmount;

                if (state.IsKeyDown(Keys.W))
                    _end.Y += moveAmount;

                if (state.IsKeyDown(Keys.D))
                    _end.X += moveAmount;
            }

            base.Keyboard(state, oldState);
        }

        public override void Gamepad(GamePadState state, GamePadState oldState)
        {
            _start.X += state.ThumbSticks.Left.X / 5;
            _start.Y += state.ThumbSticks.Left.Y / 5;

            _end.X += state.ThumbSticks.Right.X / 5;
            _end.Y += state.ThumbSticks.Right.Y / 5;

            if (state.Buttons.A == ButtonState.Pressed && oldState.Buttons.A == ButtonState.Released)
                CuttingTools.Cut(World, _start, _end, 0.001f);

            base.Gamepad(state, oldState);
        }

        public static CuttingTest Create()
        {
            return new CuttingTest();
        }
    }
}