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
    class Animation
    {
        Texture2D texture;
        Rectangle sourceRect;
        Vector2 position;
        float elapsed;
        float interval;
        int currframe;
        int frameCount;

        public Animation(ContentManager content, String path, int framecount, int framesPerSecond)
        {
            texture = content.Load<Texture2D>(path);
            frameCount = framecount;
            sourceRect = new Rectangle(0, 0, (texture.Width / frameCount), texture.Height);
            currframe = 0;
            interval = 1f / framesPerSecond;

        } // constructor

        public void update(GameTime gameTime)
        {
            elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (elapsed > interval)
            {
                currframe++;
                if (currframe > frameCount - 1)
                {
                    currframe = 0;
                }
                sourceRect.X = sourceRect.Width * (currframe);
                elapsed = 0;

            }

        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, sourceRect, Color.White);
        } // draw

        public Vector2 Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
            }
        } // get/set position

        public Rectangle SourceRect
        {
            get
            {
                return this.sourceRect;
            }
            set
            {
                this.sourceRect = value;
            }
        } // get/set SourceRect

        public int currFrame
        {
            get
            {
                return this.currframe;
            }
            set
            {
                this.currframe = value;
            }
        } //currFrame

        public int framecount
        {
            get
            {
                return this.frameCount;
            }
            set
            {
                this.framecount = value;
            }
        } //framecount

        public Texture2D getTexture()
        {
            return this.texture;
        } //getTexture

    }

}
