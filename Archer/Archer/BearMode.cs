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
    class BearMode
    {
        #region data members
        const int SCREEN_WIDTH = 1024;
        const int SCREEN_HEIGHT = 768;
        const int STRINGMOVE_SPEED = 50;
        const int STRINGMIDDLE = (SCREEN_WIDTH / 2) - 125;
        List<Bear> bears;
        SpriteFont Visitor;
        int roundCount;
        Vector2 stringPos;
        bool beginRound;
        bool stringidle;
        float timer;
        int bearsPerRound;
        int howManyDead;
        int tempCount;
        int killCount;
        Random rand1;
        #endregion 

        public BearMode(ContentManager content)
        {
            Visitor = content.Load<SpriteFont>("Menu\\VisitorBrk");
            roundCount = 1;
            bears = new List<Bear>();
            beginRound = true;
            stringidle = false;
            timer = 0f;
            stringPos = new Vector2(SCREEN_WIDTH, (SCREEN_HEIGHT / 2) - 50);
            bearsPerRound = 1;
            howManyDead = 1;
            rand1 = new Random();
            tempCount = 0;
            killCount = 0;
        }

        public void update(GameTime gameTime, ContentManager content)
        {
            handleTextMove(gameTime);

            if (!beginRound)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if ( (bearsPerRound > 0 && timer > 1.5f) && (rand1.Next() % 8 == 0) )
                {          
                    bears.Add(new Bear(content));
                    bearsPerRound--;
                    timer = 0f;
                }

                foreach (Bear bear in bears)
                {
                    bear.update(gameTime);

                    if (bear.isDead == true)
                    {
                        tempCount++;
                    }

                    if (tempCount == howManyDead)
                    {
                        newRound();
                    }
                }

                tempCount = 0;

                for (int i = 0; i < bears.Count; i++)
                {
                    if (bears[i].isDead == true)
                    {
                        if (bears[i].timeSinceDead() >= 30f)
                        {
                            bears.RemoveAt(i);
                            howManyDead--;
                        }
                    }
                }
            }
        }

        public void draw(SpriteBatch spriteBatch)
        {
            foreach (Bear bear in bears)
            {
                bear.draw(spriteBatch);
            }

            spriteBatch.DrawString(Visitor, "ROUND " + roundCount, stringPos, Color.Black);
        }

        public void newRound()
        {
            beginRound = true;
            timer = 0f;
            tempCount = 0;
            roundCount++;
            bearsPerRound = (roundCount * roundCount);
            howManyDead += bearsPerRound;
        }

        public void reset()
        {
            bears.Clear();
            roundCount = 1;
            bearsPerRound = 1;
            howManyDead = 1;
            beginRound = true;
            stringidle = false;
            killCount = 0;
            timer = 0f;
            tempCount = 0;
        }

        public void handleTextMove(GameTime gameTime)
        {
            if (beginRound)
            {
                stringPos.X -= STRINGMOVE_SPEED;

                if (stringPos.X <= STRINGMIDDLE && !stringidle)
                {
                    stringPos.X = STRINGMIDDLE;
                    timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (timer > 1.0f)
                    {
                        stringidle = true;
                        timer = 0f;
                    }
                }

                if (stringPos.X < -100)
                {
                    beginRound = false;
                    stringPos.X = SCREEN_WIDTH;
                    stringidle = false;
                    timer = 0f;
                }

            } // handleTextMove
        }

        public List<Bear> getBears()
        {
            return bears;
        } //getBears

        public int bearsKilled
        {
            get
            {
                return this.killCount;
            }
            set
            {
                this.killCount = value;
            }
        }

    } //class BearMode
}
