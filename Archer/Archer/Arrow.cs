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
    class Arrow
    {
        #region fields
        const int SCREEN_WIDTH = 1024;
        const int SCREEN_HEIGHT = 768;

        Texture2D texture;
        CollidableObject Collidable;
        Vector2 Position;
        Vector2 Velocity;
        Vector2 initVeloc;
        Vector2 Origin;
        bool isOnTarget;
        float elapsed;
        bool beenshot;
        float rotation;
        bool docked;
        bool inAir;
        bool stopxveloc;
        float changeY;
        ArcheryTarget archerytarget;
        #endregion

        public Arrow(ContentManager content, float x, float y)
        {
            changeY = 0;
            docked = true;
            inAir = true;
            isOnTarget = false;
            texture = content.Load<Texture2D>("Player\\arrow");
            Position.X = x;
            Position.Y = y;
            Origin.X = 2;
            Origin.Y = 2;
            rotation = 0;
            Collidable = new CollidableObject(texture, Position, rotation);
            Collidable.LoadTexture(texture, Origin);
            beenshot = false;
            stopxveloc = false;
            archerytarget = new ArcheryTarget(content);
        }

        public void update(float angle, GameTime gametime)
        {
            if (!inAir && isOnTarget)
            {
                archerytarget.update();
                this.Position.Y = archerytarget.getPosition().Y + changeY;
            }

            if (inAir)
            {
                if (docked & !beenshot)
                {
                    rotation = angle;
                    Position.Y = 768 - 45 + (float)Math.Sin(angle) * 18;
                    Position.X = 20 + (float)Math.Cos(angle) * 16;
                }
                else if (!docked && !beenshot)
                {
                    elapsed += (float)gametime.ElapsedGameTime.TotalSeconds;

                    if (stopxveloc)
                    {
                        Velocity.X = 0;
                        Velocity.Y = 10f;
                    }
                    else if (!isOnTarget)
                    {
                        Velocity.X = initVeloc.X;
                        Velocity.Y = (float)(initVeloc.Y + (9.8 * 2) * elapsed);
                    }

                    Position.X += Velocity.X;
                    Position.Y += Velocity.Y;

                    rotation = (float)Math.Atan(Velocity.Y / Velocity.X);
                    Collidable.Rotation = this.rotation;
                    Collidable.Position = this.Position;

                    if (Position.Y > SCREEN_HEIGHT - texture.Height * 3)
                    {
                        Position.Y = SCREEN_HEIGHT - texture.Height * 3.5f;
                        docked = true;
                        beenshot = true;
                        inAir = false;
                    }

                }
            } //ifinAir

        } // update

        public void shoot(float velocity)
        {
            docked = false;

            initVeloc.Y = ( (float)Math.Sin(rotation) * velocity ) * 2f;
            initVeloc.X = ( (float)Math.Cos(rotation) * velocity ) * 2;

            elapsed = 0;

        } // shoot

        public void draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(texture, Position, null, Color.White, rotation, Origin, 1.0f, SpriteEffects.None, 0f);
        } // draw

        public CollidableObject getCollidableObject()
        {
            return Collidable;
        } //getCollidableObject

        public bool isinAir
        {
            get
            {
                return this.inAir;
            }
            set
            {
                this.inAir = value;
            }
        }

        public Vector2 velocity
        {
            get
            {
                return this.Velocity;
            }
            set
            {
                this.Velocity = value;
            }
        } // Vector2 velocity

        public void stopXVeloc()
        {
            stopxveloc = true;
        } //stopXveloc

        public bool isDocked()
        {
            return docked;
        } //isDocked

        public void onTarget(float deltaY, ArcheryTarget target, ContentManager content)
        {
            isOnTarget = true;
            archerytarget = new ArcheryTarget(content);
            archerytarget.changePosY(target.getPosition().Y);
            changeY = deltaY;

        } //onTarget

        public bool isonTarget()
        {
            return isOnTarget;
        } // isonTarget

        public Vector2 position 
        {
            get
            {
                return this.Position;
            }
            set
            {
                this.Position = value;
            }
        } //Vector2 position

        public void changeArcheryTargetPosY( float newY)
        {
            archerytarget.changePosY(newY);
        } // changeArcheryTargetPosY

    } // class arrow

} // namespace Archer
