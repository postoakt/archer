using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Archer
{
    class MouseHandler
    {
        Texture2D crosshair;
        MouseState mousestate;
        float velocity;
        GameTime gametime;
        bool readytoshoot;
        bool charging;


        public MouseHandler(ContentManager content)
        {
            charging = false;
            readytoshoot = false;
            gametime = new GameTime();
            velocity = 0;
            crosshair = content.Load<Texture2D>("Player\\Crosshairs");
        } // MouseHandler constructor

        public void update()
        {   
            mousestate = Mouse.GetState();

            if (mousestate.LeftButton == ButtonState.Pressed)
            {
                charging = true;

                velocity += .481f;

                if (velocity > 15)
                {
                    velocity = 15;
                }

            }

            if (mousestate.LeftButton == ButtonState.Released && charging)
            {
                charging = false;
                readytoshoot = true;
            }

        } // update

        public float mouseY()
        {
            return mousestate.Y;
        } // mouseY


        public float mouseX()
        {
            return mousestate.X;
        } // mouseX

        public float getVelocity()
        {
            return velocity;
        } // getVelocity

        public void changeVelocity(float val)
        {
            velocity = val;
        } // changeVelocity

        public bool readyshoot()
        {
            return readytoshoot;
        } // readyshoot

        public void changereadyshoot(bool state)
        {
            readytoshoot = state;
        } // changereadyshoot

    } // class MouseHandler

} // namespace Archer
