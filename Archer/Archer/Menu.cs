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
    class Menu
    {
        const int MOVESPEED = 30;
        const int ORIGARCHPOS = 120;
        Texture2D archer;
        Texture2D target;
        Texture2D bear;
        Texture2D exit;
        int xPos;
        CollidableObject archerCollide;
        CollidableObject targetCollide;
        CollidableObject bearCollide;
        CollidableObject exitCollide;
        Rectangle archerbox;
        Rectangle targetbox;
        Rectangle bearbox;
        Rectangle exitbox;

        public Menu(ContentManager content)
        {   
            xPos = 410;
            archer = content.Load<Texture2D>("Menu\\archer");
            target = content.Load<Texture2D>("Menu\\target");
            bear = content.Load<Texture2D>("Menu\\bear");
            exit = content.Load<Texture2D>("Menu\\exit");

            archerbox = new Rectangle();
            targetbox = new Rectangle();
            bearbox = new Rectangle();
            exitbox = new Rectangle();

            archerbox.X = xPos;
            targetbox.X = xPos;
            bearbox.X = xPos;
            exitbox.X = xPos;

            archerbox.Y = ORIGARCHPOS;
            targetbox.Y = ORIGARCHPOS + 180;
            bearbox.Y = ORIGARCHPOS + 180 + 80;
            exitbox.Y = ORIGARCHPOS + 180 + 80 + 80;

            archerbox.Width = archer.Width;
            archerbox.Height = archer.Height;

            targetbox.Width = target.Width;
            targetbox.Height = target.Height;

            bearbox.Width = bear.Width;
            bearbox.Height = bear.Height;

            exitbox.Width = exit.Width;
            exitbox.Height = exit.Height;
            archerCollide = new CollidableObject(archer, new Vector2(archerbox.X, archerbox.Y));
            targetCollide = new CollidableObject(target, new Vector2(targetbox.X, targetbox.Y));
            bearCollide = new CollidableObject(bear, new Vector2(bearbox.X, bearbox.Y));
            exitCollide = new CollidableObject(exit, new Vector2(exitbox.X, exitbox.Y));
            archerCollide.Origin = new Vector2(0, 0);
            targetCollide.Origin = new Vector2(0, 0);
            bearCollide.Origin = new Vector2(0, 0);
            exitCollide.Origin = new Vector2(0, 0);
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(archer, archerbox, Color.White);
            spriteBatch.Draw(target, targetbox, Color.White);
            spriteBatch.Draw(bear, bearbox, Color.White);
            spriteBatch.Draw(exit, exitbox, Color.White);
        }

        public void MoveOfforOnScreen(bool On)
        {
            if (On)
            {
                archerbox.Y -= MOVESPEED;
                targetbox.Y -= MOVESPEED;
                bearbox.Y -= MOVESPEED;
                exitbox.Y -= MOVESPEED;

                if (archerbox.Y <= ORIGARCHPOS)
                {
                    archerbox.Y = ORIGARCHPOS;
                    targetbox.Y = ORIGARCHPOS + 180;
                    bearbox.Y = ORIGARCHPOS + 180 + 80;
                    exitbox.Y = ORIGARCHPOS + 180 + 80 + 80;
                }

            }
            else
            {
                archerbox.Y += MOVESPEED;
                targetbox.Y += MOVESPEED;
                bearbox.Y += MOVESPEED;
                exitbox.Y += MOVESPEED;

                if (archerbox.Y >= 768 + archerbox.Height * 2)
                {
                    archerbox.Y = 768 + archerbox.Height * 2;
                    targetbox.Y = archerbox.Y + 180;
                    bearbox.Y = archerbox.Y + 180 + 80;
                    exitbox.Y = archerbox.Y + 180 + 80 + 80;
                }
            }

        }

        public CollidableObject getArcherCollide()
        {
            return archerCollide;
        } //getArcherCollide

        public CollidableObject getTargetCollide()
        {
            return targetCollide;
        } //getTargetCollide

        public CollidableObject getbearCollide()
        {
            return bearCollide;
        } //getbearCollide

        public CollidableObject getExitCollide()
        {
            return exitCollide;
        } //getExitCollide

    }
}
