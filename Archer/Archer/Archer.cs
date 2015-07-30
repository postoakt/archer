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
    class Archer
    {
        const int SCREEN_WIDTH = 1024;
        const int SCREEN_HEIGHT = 768;

        Vector2 position;
        Texture2D texture;
        CollidableObject collidable;

        public Archer(ContentManager content)
        {
            texture = content.Load<Texture2D>("Player\\player");
            position.X = 0;
            position.Y = SCREEN_HEIGHT - texture.Height;
            collidable = new CollidableObject(texture, position, 0.0f);
            collidable.Origin = new Vector2(0, 0);

        } // Archer constructor

        public void draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(texture, position, Color.White);
        } // draw

        public CollidableObject getCollidable()
        {
            return this.collidable;
        } // getCollidable

    } //class Archer

} // namespace Archer
