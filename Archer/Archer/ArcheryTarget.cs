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
    class ArcheryTarget
    {
        const int SCREEN_WIDTH = 1024;
        const int SCREEN_HEIGHT = 768;
        const int MOVE_SPEED = 5;

        Texture2D texture;
        int moveVelocity;
        CollidableObject collidable;
        Vector2 position;

        public ArcheryTarget(ContentManager content)
        {
            texture = content.Load<Texture2D>("bullsEye");
            position = new Vector2(SCREEN_WIDTH - texture.Width, 0);
            collidable = new CollidableObject(texture, position, 0.0f);
            collidable.Origin = new Vector2(0, 0);
            moveVelocity = MOVE_SPEED;
        } // constructor

        public void update()
        {
            position.Y += moveVelocity;

            if ((position.Y <= 0) || (position.Y >= SCREEN_HEIGHT - texture.Height))
            {
                moveVelocity = -moveVelocity;
            }

            collidable.Position = this.position;
        } // update

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);

        } // draw

        public CollidableObject getCollidable()
        {
            return this.collidable;
        } //getCollidable

        public Vector2 getPosition()
        {
            return this.position;
        } // getPosition

        public int getMoveSpeed()
        {
            return moveVelocity;
        } //getMoveSpeed

        public void changePosY(float val)
        {
            this.position.Y = val;
        }  // changePosY

    }
}
