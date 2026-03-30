namespace LR1TrackEditor
{
    using LibLR1.Utils;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using System;
    using System.Collections.Generic;

    public static class InputHandler
    {
        public static int cullmode = 1;
        public static int fillmode = 0;
        private static int lastscrollvalue = 0;
        private static bool holdingright = false;
        private static bool dragging = false;
        private static readonly Dictionary<Keys, bool> pressed = new Dictionary<Keys, bool>();
        private static bool clicked = false;
        private static bool justplaced = false;
        private static readonly float[] speedSteps = { 0.25f, 0.5f, 1f, 2f, 3f, 5f, 10f, 15f, 20f, 30f };
        private static int speedIndex = 3; // default = 2
        public static float flyspeed = speedSteps[3];
        public static FormEditor form;
        private static Point dragstart = new Point();
        private static int dragaxis;

        private static Vector3? CalculatePlaceLocation(GameView game, int MouseX, int MouseY)
        {
            Vector3? nullable2;
            Vector3? nullable4;
            Vector3 position = game.GraphicsDevice.Viewport.Unproject(new Vector3((float)MouseX, (float)MouseY, 0f), game.basicEffect.Projection, game.basicEffect.View, Matrix.Identity);
            Vector3 direction = game.GraphicsDevice.Viewport.Unproject(new Vector3((float)MouseX, (float)MouseY, 1f), game.basicEffect.Projection, game.basicEffect.View, Matrix.Identity) - position;
            direction.Normalize();
            Ray input = new Ray(position, direction);
            float? nullable = Utils.distanceToTriangle(input);
            if (nullable == null)
            {
                nullable4 = null;
                nullable2 = nullable4;
            }
            else
            {
                Vector3? nullable5;
                Vector3? nullable1;
                Vector3? nullable6;
                Vector3? nullable7;
                Vector3 vector4 = position;
                Vector3 vector5 = direction;
                float? nullable3 = nullable;
                if (nullable3 != null)
                {
                    nullable1 = new Vector3?(vector5 * nullable3.GetValueOrDefault());
                }
                else
                {
                    nullable4 = null;
                    nullable1 = nullable4;
                }
                nullable4 = nullable1;
                if (nullable4 != null)
                {
                    nullable6 = new Vector3?(vector4 + nullable4.GetValueOrDefault());
                }
                else
                {
                    nullable5 = null;
                    nullable6 = nullable5;
                }
                nullable4 = nullable6;
                vector4 = new Vector3(0f, 0f, 5f);
                if (nullable4 != null)
                {
                    nullable7 = new Vector3?(nullable4.GetValueOrDefault() + vector4);
                }
                else
                {
                    nullable5 = null;
                    nullable7 = nullable5;
                }
                nullable2 = nullable7;
            }
            return nullable2;
        }

        private static void Drag(GameView game, float MouseX, float MouseY)
        {
            Plane plane;
            Vector3 vector5;
            bool flag1;
            int num = game.SelectedBrickIndices.Count - 1;
            Vector3 vector = game.GraphicsDevice.Viewport.Unproject(new Vector3(MouseX, MouseY, 0f), game.basicEffect.Projection, game.basicEffect.View, Matrix.Identity);
            Vector3 vector2 = game.GraphicsDevice.Viewport.Unproject(new Vector3(MouseX, MouseY, 1f), game.basicEffect.Projection, game.basicEffect.View, Matrix.Identity);
            Vector3 vector3 = game.GraphicsDevice.Viewport.Unproject(new Vector3(((float)game.width) / 2f, ((float)game.height) / 2f, 0f), game.basicEffect.Projection, game.basicEffect.View, Matrix.Identity);
            if ((dragaxis == 1) || (dragaxis == 2))
            {
                plane = new Plane(vector, vector2, vector + new Vector3(0f, 0f, 1f));
            }
            else
            {
                plane = new Plane(vector, vector2, vector + new Vector3(1f, 0f, 0f));
            }
            LRVector3 input = !game.SelectedBricksColored[num] ? game.pwb.WhiteBricks[game.SelectedBrickIndices[num]].Position : game.pwb.ColorBricks[game.SelectedBrickIndices[num]].Position;
            Vector3 vector6 = input.toXNAVector();
            float num2 = MathHelper.Min((vector6 - vector3).Length(), 250f) * 0.2f;
            if (dragaxis == 1)
            {
                vector5 = new Vector3(1f, 0f, 0f);
            }
            else if (dragaxis == 2)
            {
                vector5 = new Vector3(0f, 1f, 0f);
            }
            else
            {
                vector5 = new Vector3(0f, 0f, 1f);
            }
            float? nullable = new Ray(vector6 + (vector5 * num2), vector5).Intersects(plane, false);
            if (nullable == null)
            {
                flag1 = true;
            }
            else
            {
                float? nullable2 = nullable;
                flag1 = (nullable2.GetValueOrDefault() == 0f) && (nullable2 != null);
            }
            if (!flag1)
            {
                int num3 = 0;
                while (true)
                {
                    if (num3 >= game.SelectedBrickIndices.Count)
                    {
                        break;
                    }
                    input = !game.SelectedBricksColored[num3] ? game.pwb.WhiteBricks[game.SelectedBrickIndices[num3]].Position : game.pwb.ColorBricks[game.SelectedBrickIndices[num3]].Position;
                    if (dragaxis == 1)
                    {
                        input.X += nullable.Value;
                    }
                    else if (dragaxis == 2)
                    {
                        input.Y += nullable.Value;
                    }
                    else if (dragaxis == 3)
                    {
                        input.Z += nullable.Value;
                    }
                    if (game.SelectedBricksColored[num3])
                    {
                        game.pwb.ColorBricks[game.SelectedBrickIndices[num3]].Position = input;
                    }
                    else
                    {
                        game.pwb.WhiteBricks[game.SelectedBrickIndices[num3]].Position = input;
                    }
                    num3++;
                }
            }
        }

        public static void handleinput(GameView game)
        {
            if (!form.ContainsFocus && !game.gameFormFocused)
            {
                if (game.mouselock)
                {
                    game.mouselock = false;
                    Console.WriteLine("Mouse unlocked");
                }
            }
            else
            {
                if (!form.TabControlFocused)
                {
                    RasterizerState state3;
                    KeyboardState state = KeyboardHelper.GetState();
                    MouseState state2 = MouseHelper.GetState(game.drawsurface);
                    if (game.mouselock)
                    {
                        game.Pitch += ((game.height / 2) - state2.Y) * 0.001f;
                        game.Yaw -= ((game.width / 2) - state2.X) * 0.001f;
                        if (game.Pitch >= 0.45f)
                        {
                            game.Pitch = 0.45f;
                        }
                        if (game.Pitch <= -0.45f)
                        {
                            game.Pitch = -0.45f;
                        }
                        if (game.Yaw >= 2f)
                        {
                            game.Yaw -= 2f;
                        }
                        if (game.Yaw <= -2f)
                        {
                            game.Yaw += 2f;
                        }
                        MouseHelper.SetPosition(game.drawsurface, game.width / 2, game.height / 2);
                        if (!(!state.IsKeyDown(Keys.Escape) || holdingright))
                        {
                            if (game.fullscreen)
                            {
                                game.ToggleFullscreen();
                            }
                            else
                            {
                                game.mouselock = false;
                                game.IsMouseVisible = true;
                                Console.WriteLine("Mouse unlocked");
                            }
                        }
                        if ((state2.RightButton == ButtonState.Released) && holdingright)
                        {
                            holdingright = false;
                            game.mouselock = false;
                            game.IsMouseVisible = true;
                        }
                    }
                    else if (game.GraphicsDevice.Viewport.Bounds.Contains(state2.X, state2.Y))
                    {
                        if (state2.RightButton == ButtonState.Pressed)
                        {
                            holdingright = true;
                            MouseHelper.SetPosition(game.drawsurface, game.width / 2, game.height / 2);
                            game.IsMouseVisible = false;
                            game.mouselock = true;
                        }
                        if (state2.LeftButton == ButtonState.Pressed)
                        {
                            if (!clicked)
                            {
                                dragaxis = 0;
                                dragstart = new Point(state2.X, state2.Y);
                                if (game.placing)
                                {
                                    game.placingposition = CalculatePlaceLocation(game, state2.X, state2.Y);
                                    game.Place(state.IsKeyDown(Keys.LeftControl) || state.IsKeyDown(Keys.RightControl));
                                    justplaced = true;
                                }
                                else
                                {
                                    Vector3 position = game.GraphicsDevice.Viewport.Unproject(new Vector3((float)state2.X, (float)state2.Y, 0f), game.basicEffect.Projection, game.basicEffect.View, Matrix.Identity);
                                    Vector3 direction = game.GraphicsDevice.Viewport.Unproject(new Vector3((float)state2.X, (float)state2.Y, 1f), game.basicEffect.Projection, game.basicEffect.View, Matrix.Identity) - position;
                                    direction.Normalize();
                                    Ray ray = new Ray(position, direction);
                                    float maxValue = float.MaxValue;
                                    float? nullable = ray.Intersects(game.dragarrowhitboxes[0]);
                                    if (nullable != null)
                                    {
                                        maxValue = nullable.Value;
                                        dragaxis = 1;
                                    }
                                    if (((nullable = ray.Intersects(game.dragarrowhitboxes[1])) != null) && (nullable.Value < maxValue))
                                    {
                                        maxValue = nullable.Value;
                                        dragaxis = 2;
                                    }
                                    if (((nullable = ray.Intersects(game.dragarrowhitboxes[2])) != null) && (nullable.Value < maxValue))
                                    {
                                        //maxValue = nullable.Value;
                                        dragaxis = 3;
                                    }
                                }
                                clicked = true;
                            }
                            if ((dragstart != new Point(state2.X, state2.Y)) && (dragaxis != 0))
                            {
                                dragging = true;
                            }
                            if (dragging && ((game.editmode == 1) && (game.SelectedBrickIndices.Count > 0)))
                            {
                                Drag(game, (float)state2.X, (float)state2.Y);
                            }
                        }
                        if ((state2.LeftButton == ButtonState.Released) && clicked)
                        {
                            if (!(game.placing || justplaced))
                            {
                                if (!dragging)
                                {
                                    game.Select(state2.X, state2.Y, state.IsKeyDown(Keys.LeftControl) || state.IsKeyDown(Keys.RightControl));
                                }
                                else
                                {
                                    form.refreshPWB(true);
                                }
                            }
                            justplaced = false;
                            dragging = false;
                            clicked = false;
                        }
                        if (game.placing && LR1TrackEditor.Settings.Default.GhostPlacing)
                        {
                            game.placingposition = CalculatePlaceLocation(game, state2.X, state2.Y);
                        }
                    }
                    if (state.IsKeyDown(Keys.W))
                    {
                        game.cameraPosition += new Vector3(((float)Math.Sin(game.Yaw * 3.1415926535897931)) * flyspeed, ((float)Math.Cos(game.Yaw * 3.1415926535897931)) * flyspeed, 0f);
                    }
                    if (state.IsKeyDown(Keys.S))
                    {
                        game.cameraPosition -= new Vector3(((float)Math.Sin(game.Yaw * 3.1415926535897931)) * flyspeed, ((float)Math.Cos(game.Yaw * 3.1415926535897931)) * flyspeed, 0f);
                    }
                    if (state.IsKeyDown(Keys.A))
                    {
                        game.cameraPosition += new Vector3(((float)Math.Sin((game.Yaw * 3.1415926535897931) - 1.5707963705062866)) * flyspeed, ((float)Math.Cos((game.Yaw * 3.1415926535897931) - 1.5707963705062866)) * flyspeed, 0f);
                    }
                    if (state.IsKeyDown(Keys.D))
                    {
                        game.cameraPosition += new Vector3(((float)Math.Sin((game.Yaw * 3.1415926535897931) + 1.5707963705062866)) * flyspeed, ((float)Math.Cos((game.Yaw * 3.1415926535897931) + 1.5707963705062866)) * flyspeed, 0f);
                    }
                    if (state.IsKeyDown(Keys.Space) || state.IsKeyDown(Keys.E))
                    {
                        game.cameraPosition += new Vector3(0f, 0f, 1f * flyspeed);
                    }
                    if (state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.Q))
                    {
                        game.cameraPosition -= new Vector3(0f, 0f, 1f * flyspeed);
                    }
                    if (!(!state.IsKeyDown(Keys.LeftAlt) && !state.IsKeyDown(Keys.RightAlt) || !isKeyPressed(Keys.Enter)))
                    {
                        game.ToggleFullscreen();
                    }
                    if ((!state.IsKeyDown(Keys.LeftControl) && !state.IsKeyDown(Keys.RightControl)) && isKeyPressed(Keys.R))
                    {
                        game.cameraPosition = Vector3.Zero;
                        game.Yaw = game.Pitch = 0f;
                        Console.WriteLine("View reset");
                    }
                    if (isKeyPressed(Keys.T))
                    {
                        game.doTextures = !game.doTextures;
                        form.doTexturesChanged(game.doTextures);
                    }
                    if (isKeyPressed(Keys.V))
                    {
                        game.doVertexColors = !game.doVertexColors;
                        form.doVertexColorsChanged(game.doVertexColors);
                    }
                    if (!(!state.IsKeyDown(Keys.LeftControl) && !state.IsKeyDown(Keys.RightControl) || !isKeyPressed(Keys.O)) && form.OpenWarning())
                    {
                        Utils.OpenFileDialog(1);
                    }
                    if (!(!state.IsKeyDown(Keys.LeftControl) && !state.IsKeyDown(Keys.RightControl) || !isKeyPressed(Keys.R)))
                    {
                        game.Reload();
                    }
                    if (isKeyPressed(Keys.F2))
                    {
                        cullmode++;
                        if (cullmode == 3)
                        {
                            cullmode = 0;
                        }
                        state3 = new RasterizerState
                        {
                            CullMode = (CullMode)cullmode,
                            FillMode = (FillMode)fillmode
                        };
                        game.rasterizerstate = state3;
                        Console.WriteLine("Cullmode=" + state3.CullMode.ToString());
                    }
                    if (isKeyPressed(Keys.F1))
                    {
                        fillmode++;
                        if (fillmode == 2)
                        {
                            fillmode = 0;
                        }
                        state3 = new RasterizerState
                        {
                            CullMode = (CullMode)cullmode,
                            FillMode = (FillMode)fillmode
                        };
                        game.rasterizerstate = state3;
                        Console.WriteLine("Fillmode=" + state3.FillMode.ToString());
                    }
                    if (state2.ScrollWheelValue != lastscrollvalue)
                    {
                        int num = state2.ScrollWheelValue - lastscrollvalue;
                        if (num > 0 && speedIndex < speedSteps.Length - 1)
                        {
                            speedIndex++;
                        }
                        else if (num < 0 && speedIndex > 0)
                        {
                            speedIndex--;
                        }
                        flyspeed = speedSteps[speedIndex];
                        Console.WriteLine("FlySpeed=" + flyspeed);
                    }
                    lastscrollvalue = state2.ScrollWheelValue;
                }
                if (isKeyPressed(Keys.Delete) && ((game.editmode == 1) && (game.SelectedBrickIndices.Count > 0)))
                {
                    form.DeleteSelectedBricks();
                }
            }
        }

        private static bool isKeyPressed(Keys key)
        {
            bool flag;
            if (!(!KeyboardHelper.GetState().IsKeyDown(key) || (pressed.ContainsKey(key) && pressed[key])))
            {
                pressed[key] = true;
                flag = true;
            }
            else
            {
                if (KeyboardHelper.GetState().IsKeyUp(key))
                {
                    pressed[key] = false;
                }
                flag = false;
            }
            return flag;
        }

        public static void overridePressed(Keys key, bool value)
        {
            pressed[key] = value;
        }
    }
}

