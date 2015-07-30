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
    class Bow
    {   
        const int SCREEN_WIDTH = 1024;
        const int SCREEN_HEIGHT = 768;

        Vector2 position;
        Vector2 origin;
        Texture2D texture;
        float rotation;
        MouseHandler mouse;
        List<Arrow> arrows;
        int currentarrow;

        public Bow(ContentManager content)
        {   
            texture = content.Load<Texture2D>("Player\\bow");
            mouse = new MouseHandler(content);
            rotation = 0;
            position.X = 20;
            position.Y = SCREEN_HEIGHT - texture.Height + 46;
            origin.X = 20;
            origin.Y = 46;
            arrows = new List<Arrow>();
            arrows.Add(new Arrow(content, 36, SCREEN_HEIGHT - texture.Height + 52) );
            arrows.Add(new Arrow(content, 36, SCREEN_HEIGHT - texture.Height + 52));
            currentarrow = 0;

        } // bow constructor

        public void update(GameTime gametime, ContentManager content)
        {
            mouse.update();
            rotation = (float)Math.Atan( (mouse.mouseY() - (SCREEN_HEIGHT - 46)) / (mouse.mouseX() - 25));

            foreach (Arrow arrow in arrows)
            {
                arrow.update(rotation, gametime);
            }

            if (mouse.readyshoot())
            {
                arrows[currentarrow].shoot( mouse.getVelocity() );
                currentarrow++;
                arrows.Add(new Arrow(content, 36, SCREEN_HEIGHT - texture.Height + 52));
                mouse.changereadyshoot(false);
                mouse.changeVelocity(0);
            }

        } // update

        public void draw(SpriteBatch theSpriteBatch)
        {

            theSpriteBatch.Draw(texture, position, null, Color.White, rotation, origin, 1.0f, SpriteEffects.None, 0f);

            foreach (Arrow arrow in arrows)
            {
                arrow.draw(theSpriteBatch);
            }

        } // draw

        public Arrow getCurrArrow()
        {
            return arrows[currentarrow];
        } // getCcurrArrow

        public List<Arrow> getArrows()
        {
            return arrows;
        } // Arrows in Air

        public void resetArrows(ContentManager content)
        {
            arrows.Clear();
            arrows.Add(new Arrow(content, 36, SCREEN_HEIGHT - texture.Height + 52));
            arrows.Add(new Arrow(content, 36, SCREEN_HEIGHT - texture.Height + 52));
            currentarrow = 0;
        }

    } // class Bow

} // namespace Archer
