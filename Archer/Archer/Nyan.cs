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
    class Nyan
    {
        const int SCREEN_WIDTH = 1024;
        const int SCREEN_HEIGHT = 768;
        const float MOVE_SPEED = 8.0f;
        const float ANGLE = (float)Math.PI / 20;
        const int NYAN_INTERVAL = 30;
        const int DEAD_INTERVAL = 1;
        bool ismoving;
        bool isDead;
        float angle;
        float elapsed;
        float randlapsed;
        CollidableObject Collidable;
        Texture2D texture;
        Texture2D nyan1;
        Texture2D nyan2;
        Texture2D nyanDead;
        Random rand;
        Cue NyanSong;
        bool nyanSongIsPlaying;
        bool meowHasPlayed;
        bool thudHasPlayed;
        Vector2 position;
        float deadlapsed;

        public Nyan(ContentManager content)
        {
            rand = new Random();
            elapsed = 0;
            randlapsed = 0f;
            deadlapsed = 0f;
            isDead = false;
            nyan1 = content.Load<Texture2D>("Nyan\\nyan1");
            nyan2 = content.Load<Texture2D>("Nyan\\nyan2");
            nyanDead = content.Load<Texture2D>("Nyan\\nyandead");
            texture = nyan1;
            position.X = -300;
            position.Y = 3;
            angle = 0;
            Collidable = new CollidableObject(texture, position);
            nyanSongIsPlaying = false;
            meowHasPlayed = false;
            thudHasPlayed = false;
        }

        public void update(GameTime gametime, SoundBank soundbank)
        {
            angle += ANGLE;

            elapsed += (float)gametime.ElapsedGameTime.TotalSeconds;
            randlapsed += (float)gametime.ElapsedGameTime.TotalSeconds;

            if (!nyanSongIsPlaying && ismoving)
            {
                NyanSong = soundbank.GetCue("Nyan Cat");
                NyanSong.Play();
                nyanSongIsPlaying = true;
            }

            // handle animation
            if (!isDead)
            {
                if (elapsed > .25f && texture == nyan1)
                {
                    texture = nyan2;
                    Collidable.LoadTexture(texture);
                }
                else if (elapsed > .5f && texture == nyan2)
                {
                    texture = nyan1;
                    Collidable.LoadTexture(texture);
                    elapsed = 0;
                }
            }
            else
            {
                texture = nyanDead;
                elapsed = 0;
            }

            // end animation handling

            // if angle is greater than pi * 2, reset to zero. 2PI == 0 DUHH NOOOB
            if (angle >= Math.PI * 2)
            {
                angle = 0;
            }

            if (!isDead) // if nyancat is dead, dont bother handling his random appearance. he can't appear if HES DEAD
            {
                handleWhenonScreen();
                position.Y += (float)Math.Sin(angle) * 3;
            }
            else // handles when falling after shot.
            {
                NyanSong.Stop(AudioStopOptions.AsAuthored);

                if (deadlapsed < 2.0f)
                {
                    deadlapsed += (float)gametime.ElapsedGameTime.TotalSeconds;
                }
                if (!meowHasPlayed)
                {
                    soundbank.PlayCue("meow");
                    meowHasPlayed = true;
                }

                ismoving = false;
                if (position.Y < SCREEN_HEIGHT - (texture.Height))
                {
                    position.Y += 30f;
                }
                else
                {
                    position.Y = SCREEN_HEIGHT - texture.Height;
                    if (!thudHasPlayed)
                    {
                        soundbank.PlayCue("thud");
                        thudHasPlayed = true;
                    }
                }
            }

            Collidable.Position = this.position; // sets bounding box to same position as where nyancat is being drawn.

        } // update

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        } // draw

        private void handleWhenonScreen()
        {
            if (randlapsed >= NYAN_INTERVAL)
            {
                randlapsed = 0f;

                if (rand.Next() % 2 == 0)
                {
                    ismoving = true;
                    position.X = SCREEN_WIDTH + texture.Width * 4;
                }
            }

            if (ismoving)
            {
                position.X -= MOVE_SPEED;
                randlapsed = 0f;

                if (position.X <= -300)
                {
                    position.X = -300;
                    ismoving = false;
                    nyanSongIsPlaying = false;
                }
            }

        } // handleWhenOnScreen

        public void isdead()
        {
            isDead = true;
        } // isdead()

        public bool getIsDead()
        {
            return isDead;
        }

        public CollidableObject getCollidableObject()
        {
            return Collidable;
        } //getCollidableObject

        public bool IsMoving()
        {
            if (!isDead)
            {
                return ismoving;
            }
            else
            {
                if (deadlapsed > 2.0f)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        } //IsMoving
    } // class Nyan
} // namespace Archer
