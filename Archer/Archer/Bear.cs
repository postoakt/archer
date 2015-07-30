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
    class Bear
    {
        #region data members
        const int SCREEN_WIDTH = 1024;
        const int SCREEN_HEIGHT = 768;
        const float WALK_SPEED = 1.75f;
        const float ROLL_SPEED = 8.0f;
        const float INTERVAL = 1f;
        const float JUMP_VELOCITY = - 10.0f;
        const float GRAVITY = 10.0f;
        bool isJumping;
        bool walkjumppass;
        bool dead;
        bool firstdeadpass;
        Vector2 velocity;
        Animation walkingAnime;
        Animation rollingAnime;
        Animation jumpingAnime;
        Animation deadAnime;
        Texture2D bearJump1;
        Texture2D bearJump2;
        Texture2D bearWalk1;
        Texture2D bearWalk2;
        Texture2D bearWalk3;
        Texture2D bearWalk4;
        Texture2D bearRoll1;
        Texture2D bearRoll2;
        Texture2D bearRoll3;
        Texture2D bearRoll4;
        Vector2 position;
        Random rand;
        float timer;
        float jumpLapsed;
        float timersincedead;
        BearState bearState;
        #endregion

        enum BearState
        {
            Walking,
            Rolling,
            Jumping,
            Dead
        }

        public Bear(ContentManager Content)
        {
            timer = 0f;
            jumpLapsed = 0f;
            rand = new Random();
            isJumping = false;
            walkjumppass = false;
            dead = false;
            firstdeadpass = true;
            walkingAnime = new Animation(Content, "Bear\\bearwalk", 4, 4);
            rollingAnime = new Animation(Content, "Bear\\bearroll", 4, 12);
            jumpingAnime = new Animation(Content, "Bear\\bearjump", 2, 1);
            deadAnime = new Animation(Content, "Bear\\beardead", 2, 1);

            switch (rand.Next(1, 3))
            {
                case 1: bearState = BearState.Walking;
                    position.X = SCREEN_WIDTH;
                    position.Y = SCREEN_HEIGHT - walkingAnime.SourceRect.Height;
                    velocity = new Vector2(-WALK_SPEED, 0);
                    break;
                case 2: bearState = BearState.Rolling;
                    position.X = SCREEN_WIDTH;
                    position.Y = SCREEN_HEIGHT - rollingAnime.SourceRect.Height;
                    velocity = new Vector2(-ROLL_SPEED, 0);
                    break;
            }
            #region beartexturesload
            bearJump1 = Content.Load<Texture2D>("BearFrames\\bearJump1");
            bearJump2 = Content.Load<Texture2D>("BearFrames\\bearJump2");
            bearWalk1 = Content.Load<Texture2D>("BearFrames\\bearWalk1");
            bearWalk2 = Content.Load<Texture2D>("BearFrames\\bearWalk2");
            bearWalk3 = Content.Load<Texture2D>("BearFrames\\bearWalk3");
            bearWalk4 = Content.Load<Texture2D>("BearFrames\\bearWalk4");
            bearRoll1 = Content.Load<Texture2D>("BearFrames\\bearRoll1");
            bearRoll2 = Content.Load<Texture2D>("BearFrames\\bearRoll2");
            bearRoll3 = Content.Load<Texture2D>("BearFrames\\bearRoll3");
            bearRoll4 = Content.Load<Texture2D>("BearFrames\\bearRoll4");
            #endregion

        } // constructor

        public void update(GameTime gameTime)
        {
            handleIfDead();

            if (dead)
            {
                timersincedead += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            position += velocity;

            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!dead)
            {
                handleRandJump(gameTime);
            }

            if (position.Y > (SCREEN_HEIGHT - 148))
            {
                position.Y = SCREEN_HEIGHT - 148;
            }

            switch (bearState)
            {
                case BearState.Walking:
                    walkingAnime.Position = this.position;
                    walkingAnime.update(gameTime);
                    break;
                case BearState.Rolling:
                    rollingAnime.Position = this.position;
                    rollingAnime.update(gameTime);
                    break;
                case BearState.Jumping:
                    jumpingAnime.Position = this.position;
                    break;

                case BearState.Dead:
                    deadAnime.Position = this.position;
                    break;
            }

            if (position.X < -200) // if off of screen, then bear is dead.
            {
                isDead = true;
            }

        } // update

        public void draw(SpriteBatch spriteBatch)
        {
            switch (bearState)
            {
                case BearState.Walking:
                    walkingAnime.draw(spriteBatch);
                    break;
                case BearState.Rolling:
                    rollingAnime.draw(spriteBatch);
                    break;
                case BearState.Jumping:
                    jumpingAnime.draw(spriteBatch);
                    break;
                case BearState.Dead:
                    deadAnime.draw(spriteBatch);
                    break;
            }
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
        } // Position

        private void Jump(GameTime gameTime)
        {   
            jumpLapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            velocity.Y = JUMP_VELOCITY + (GRAVITY * jumpLapsed);

            timer = 0f;

            // this snipped handles if the bear was initially walking, if so play jump animation. otherwise rolling animation is fine.
            if (bearState == BearState.Walking)
            {
                bearState = BearState.Jumping;
            }

            if (bearState == BearState.Jumping)
            {
                if (jumpLapsed < 1.0f && !walkjumppass)
                {
                    jumpingAnime.currFrame = 0;
                    velocity.X = 0f;
                    velocity.Y = 0f;
                    timer = 0f;
                }
                else
                {
                    if (!walkjumppass)
                    {
                        jumpLapsed = 0f;
                        walkjumppass = true;
                    }
                    velocity.X = -WALK_SPEED * 2.0f;
                    jumpingAnime.currFrame = 1;
                    jumpingAnime.SourceRect = new Rectangle(jumpingAnime.SourceRect.Width, 0, jumpingAnime.SourceRect.Width, jumpingAnime.SourceRect.Height);
                }
            }
            // walkingState jump

            if (position.Y > SCREEN_HEIGHT - 148)
            {
                if (bearState == BearState.Jumping)
                {
                    bearState = BearState.Walking;
                    jumpingAnime.SourceRect = new Rectangle(0, 0, jumpingAnime.SourceRect.Width, jumpingAnime.SourceRect.Height);
                    jumpingAnime.currFrame = 0;
                }

                walkjumppass = false;
                position.Y = SCREEN_HEIGHT - 148;
                velocity.Y = 0;
                isJumping = false;
                jumpLapsed = 0f;
                timer = 0f;
            }
        } // Jump

        public bool isDead
        {
            get
            {
                return this.dead;
            }
            set
            {
                this.dead = value;
                timer = 0;
            }
        }

        private void handleIfDead()
        {
            if (dead)
            {   
                bearState = BearState.Dead;
                if (firstdeadpass && timer < 0.5f)
                {
                    deadAnime.SourceRect = new Rectangle(0, 0, deadAnime.SourceRect.Width, deadAnime.SourceRect.Height);
                }
                else
                {
                    firstdeadpass = false;
                    deadAnime.SourceRect = new Rectangle(deadAnime.SourceRect.Width, 0, deadAnime.SourceRect.Width, deadAnime.SourceRect.Height);
                }
                velocity.X = 0f;
                velocity.Y = GRAVITY;
            }
        } // void handleIfDead

        private void handleRandJump(GameTime gameTime)
        {
            if (((!isJumping) && (timer > INTERVAL)))
            {
                int temp = rand.Next() % 2;
                if (temp == 0)
                {
                    if (bearState == BearState.Walking)
                    {
                        temp = rand.Next() % 2;
                    }

                    if (temp == 0)
                    {
                        isJumping = true;
                    }
                }
                timer = 0f;
            }

            if (isJumping)
            {
                Jump(gameTime);
            }
        } // handleRandJump

        public CollidableObject getCollidable()
        {
            CollidableObject collidable;
            Texture2D theTexture;

            #region assignTexture
            switch (bearState)
            {
                case BearState.Walking:

                    switch (walkingAnime.currFrame)
                    {
                        case 0:
                            theTexture = bearWalk1;
                            break;
                        case 1:
                            theTexture = bearWalk2;
                            break;
                        case 2:
                            theTexture = bearWalk3;
                            break;
                        case 3:
                            theTexture = bearWalk4;
                            break;
                        default: theTexture = bearWalk1;
                            break;
                    }
                    break;
                case BearState.Rolling:
                    switch (rollingAnime.currFrame)
                    {
                        case 0:
                            theTexture = bearRoll1;
                            break;
                        case 1:
                            theTexture = bearRoll2;
                            break;
                        case 2:
                            theTexture = bearRoll3;
                            break;
                        case 3:
                            theTexture = bearRoll4;
                            break;
                        default:
                            theTexture = bearRoll1;
                            break;
                    }
                    break;
                case BearState.Jumping:
                    switch (jumpingAnime.currFrame)
                    {
                        case 0:
                            theTexture = bearJump1;
                            break;
                        case 1:
                            theTexture = bearJump2;
                            break;
                        default: theTexture = bearJump1;
                            break;
                    }
                   break;
                default: theTexture = bearWalk1;
                   break;
            }
            #endregion

            collidable = new CollidableObject(theTexture, position);
            collidable.LoadTexture(theTexture);
            collidable.Origin = new Vector2(0, 0);
            return collidable;
        }

        public float timeSinceDead()
        {
            return timersincedead;
        } // timeSinceDead

    } // class Bear
}
