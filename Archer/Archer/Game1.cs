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

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region fields
        const int SCREEN_WIDTH = 1024;
        const int SCREEN_HEIGHT = 768;
        GraphicsDeviceManager graphics;
        Texture2D crossHair;
        Texture2D gameOverBear;
        SpriteBatch spriteBatch;
        Texture2D background;
        KeyboardState keyboardstate;
        KeyboardState lastkeystate;
        MouseState mousestate;
        GameState gameState;
        Menu menu;
        LoginForm loginForm;
        Archer player;
        Bow bow;
        Nyan nyancat;
        SpriteFont font;
        CollidableObject nyanCollide;
        List<CollidableObject> menuObjects;
        ArcheryTarget archeryTarget;
        BearMode bearMode;
        AudioEngine audioengine;
        WaveBank wavebank;
        SoundBank soundbank;
        Cue mainTheme;
        int targetModeScore;
        bool mainThemePlaying;
        bool LoggedIn;
        bool firstGameOverpass;
        bool isHighScore;
        string username;
        #endregion

        enum GameState
        {
            Menu,
            Target,
            Bear,
            GameOver,
            Login
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
        } // Game1 Constructor

        protected override void Initialize()
        {
            gameState = GameState.Menu;
            base.Initialize();
        } // Initialize

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            mainThemePlaying = false;
            targetModeScore = 0;
            LoggedIn = false;
            firstGameOverpass = true;
            isHighScore = false;
            username = "";

            player = new Archer(Content);
            bow = new Bow(Content);
            menu = new Menu(Content);
            loginForm = new LoginForm(Content);
            nyancat = new Nyan(Content);
            bearMode = new BearMode(Content);

            font = Content.Load<SpriteFont>("Menu\\VisitorBrk");
            crossHair = Content.Load<Texture2D>("Player\\Crosshairs");
            gameOverBear = Content.Load<Texture2D>("Menu\\AB-Bear");
            archeryTarget = new ArcheryTarget(Content);

            background = Content.Load<Texture2D>("Menu\\background");
            menuObjects = new List<CollidableObject>();
            menuObjects.Add(menu.getArcherCollide());
            menuObjects.Add(menu.getTargetCollide());
            menuObjects.Add(menu.getbearCollide());
            menuObjects.Add(menu.getExitCollide());

            audioengine = new AudioEngine("Content\\archerXact.xgs");
            wavebank = new WaveBank(audioengine, "Content\\Wave Bank.xwb");
            soundbank = new SoundBank(audioengine, "Content\\Sound Bank.xsb");
            mainTheme = soundbank.GetCue("mainTheme");

        } // LoadContent

        protected override void Update(GameTime gameTime)
        {
            keyboardstate = Keyboard.GetState();
            mousestate = Mouse.GetState();

            // Allows the game to exit
            if (keyboardstate.IsKeyDown(Keys.Escape))
                this.Exit();

            if (keyboardstate.IsKeyDown(Keys.F5) && lastkeystate.IsKeyUp(Keys.F5))
            {
                graphics.ToggleFullScreen();
                graphics.ApplyChanges();
            }
            if (!mainThemePlaying && !nyancat.IsMoving())
            {
                mainTheme = soundbank.GetCue("mainTheme");
                mainTheme.Play();
                mainThemePlaying = true;
            }

            if (nyancat.IsMoving())
            {
                mainTheme.Stop(AudioStopOptions.AsAuthored);
                mainThemePlaying = false;
            }

            switch (gameState)
            {
                case GameState.Menu:
                    menu.MoveOfforOnScreen(true);
                    bow.update(gameTime, Content);
                    HandleCollisions();
                    nyancat.update(gameTime, soundbank);
                    break;
                case GameState.Target:
                    archeryTarget.update();
                    menu.MoveOfforOnScreen(false);
                    bow.update(gameTime, Content);
                    HandleCollisions();
                    nyancat.update(gameTime, soundbank);
                    break;
                case GameState.Bear:
                    menu.MoveOfforOnScreen(false);
                    bearMode.update(gameTime, Content);
                    bow.update(gameTime, Content);
                    HandleCollisions();
                    nyancat.update(gameTime, soundbank);
                    break;
                case GameState.GameOver:

                    if (firstGameOverpass)
                    {
                        if ( LoginForm.IsNewHighScore(username, bearMode.bearsKilled) )
                        {
                            LoginForm.UpdateHighScore(username, bearMode.bearsKilled);
                            isHighScore = true;
                        }
                        firstGameOverpass = false;
                    }
                    if ( (mousestate.LeftButton == ButtonState.Pressed) && (mousestate.X < 100 && mousestate.Y < 10) )
                    {
                        firstGameOverpass = true;
                        isHighScore = false;
                        bearMode.reset();
                        gameState = GameState.Menu;
                        bow.resetArrows(Content);
                    }
                    break;
                case GameState.Login:
                    loginForm.Update(gameTime.ElapsedGameTime.Ticks);
                    LoggedIn = loginForm.Is_Logged_In();
                    if ( loginForm.IsDone() )
                    {
                        LoggedIn = loginForm.Is_Logged_In();
                        if (LoggedIn)
                            username = loginForm.GetUsername();
                        gameState = GameState.Menu;
                        loginForm = new LoginForm(Content);
                    }
                    break;
            }

            lastkeystate = keyboardstate;

            base.Update(gameTime);
        } // Update

        protected override void Draw(GameTime gameTime)
        {   
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            switch (gameState)
            {
                case GameState.Menu:
                    spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
                    nyancat.draw(spriteBatch);
                    player.draw(spriteBatch);
                    bow.draw(spriteBatch);
                    menu.draw(spriteBatch);

                    if (!LoggedIn)
                        spriteBatch.DrawString(font, "Login", new Vector2(0, 0), Color.Black, 0f, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                    else
                        spriteBatch.DrawString(font, "Logged in as " + username, new Vector2(0, 0), Color.Black, 0f, new Vector2(0, 0), 0.25f, SpriteEffects.None, 0);

                    break;

                case GameState.Target:
                    spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
                    archeryTarget.draw(spriteBatch);
                    nyancat.draw(spriteBatch);
                    spriteBatch.DrawString(font, "Menu", new Vector2(0, 0), Color.Black, 0f,
                    new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, "Score: " + targetModeScore, new Vector2(450, 0), Color.Black, 0f, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                    player.draw(spriteBatch);
                    bow.draw(spriteBatch);
                    menu.draw(spriteBatch);

                    if (LoggedIn)
                        spriteBatch.DrawString(font, "Logged in as " + username, new Vector2(128, 8), Color.Black, 0f, new Vector2(0, 0), 0.25f, SpriteEffects.None, 0);
                    break;
                case GameState.Bear:
                    spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
                    nyancat.draw(spriteBatch);
                    player.draw(spriteBatch);
                    bow.draw(spriteBatch);
                    menu.draw(spriteBatch);
                    bearMode.draw(spriteBatch);

                    if (LoggedIn)
                        spriteBatch.DrawString(font, "Logged in as " + username, new Vector2(0, 0), Color.Black, 0f, new Vector2(0, 0), 0.25f, SpriteEffects.None, 0);
                    break;

                case GameState.GameOver:

                    spriteBatch.DrawString(font, "Menu", new Vector2(0, 0), Color.Black, 0f,
                    new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);

                    spriteBatch.DrawString(font, "Game Over!", new Vector2(345, 0), Color.Black);

                    spriteBatch.DrawString(font, bearMode.bearsKilled.ToString(),
                    new Vector2(470, 80), Color.Red, 0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);

                    spriteBatch.DrawString(font, "bear(s) brutally murdered.",
                    new Vector2(260, 160), Color.Black, 0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0);

                    if (nyancat.getIsDead())
                        spriteBatch.DrawString(font, "(You also somehow managed to kill Nyan Cat, good job.)",
                        new Vector2(255, 280), Color.Black, 0f, new Vector2(0, 0), 0.3f, SpriteEffects.None, 0);

                    spriteBatch.Draw(gameOverBear, new Vector2(0, 300), Color.White);

                    if (isHighScore)
                    {
                        spriteBatch.DrawString(font, "New High Score!",
                            new Vector2(410, 220), Color.Red, 0f, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0);
                    }

                    break;
                case GameState.Login:
                    spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
                    nyancat.draw(spriteBatch);
                    player.draw(spriteBatch);
                    bow.draw(spriteBatch);
                    menu.draw(spriteBatch);
                    spriteBatch.DrawString(font, "Login", new Vector2(0, 0), Color.Black, 0f,
                    new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                    break;
            }

            loginForm.Draw(spriteBatch, font);

            spriteBatch.Draw(crossHair, new Vector2(mousestate.X, mousestate.Y), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        } // Draw

        public void HandleCollisions()
        {
            nyanCollide = nyancat.getCollidableObject();
            bool resetarrows = false;

            foreach (Arrow arrow in bow.getArrows() )
            {
                if (!arrow.isinAir && arrow.isonTarget())
                {
                    arrow.changeArcheryTargetPosY(archeryTarget.getPosition().Y);
                }

                if (arrow.isinAir == true)
                {
                    if (arrow.getCollidableObject().IsColliding(nyanCollide))
                    {
                        nyancat.isdead();
                    }

                    switch (gameState)
                    {
                        case GameState.Target:

                                if (arrow.getCollidableObject().IsColliding(archeryTarget.getCollidable()) )
                                {
                                    arrow.isinAir = false;
                                    arrow.velocity = new Vector2(0, 0);
                                    arrow.onTarget(Math.Abs(arrow.position.Y - archeryTarget.getPosition().Y), archeryTarget, Content);
                                    targetModeScore += 50;
                                }

                                if (arrow.position.Y < 10 && arrow.position.X < 100)
                                {
                                    resetarrows = true;
                                    gameState = GameState.Menu;
                                }
                            break;
                        case GameState.Bear:
                            foreach (Bear bear in bearMode.getBears())
                            {
                                if (!bear.isDead)
                                {
                                    if ( !arrow.isDocked() && arrow.getCollidableObject().IsColliding(bear.getCollidable()) )
                                    {       
                                            bear.isDead = true;
                                            bearMode.bearsKilled += 1;
                                            arrow.stopXVeloc();
                                    }

                                    if (player.getCollidable().IsColliding(bear.getCollidable()))
                                    {
                                        resetarrows = true;
                                        gameState = GameState.GameOver;
                                    }
                                }
                            }
                            break;
                        case GameState.Menu:

                            if (arrow.getCollidableObject().IsColliding(menuObjects[0])) // "Archer" logo
                            {
                                arrow.isinAir = false;
                            }
                            if (arrow.getCollidableObject().IsColliding(menuObjects[1])) // "Target" logo
                            {  
                                arrow.isinAir = false;
                                resetarrows = true;
                                gameState = GameState.Target;
                                break;
                            }
                            if (arrow.getCollidableObject().IsColliding(menuObjects[2])) // "Bear" logo
                            {
                                arrow.isinAir = false;
                                resetarrows = true;
                                gameState = GameState.Bear;
                                bearMode.reset();
                                break;
                            }
                            if (arrow.getCollidableObject().IsColliding(menuObjects[3]) ) // "Exit" logo
                            {
                                this.Exit();
                            }

                            if (arrow.position.Y < 10 && arrow.position.X < 100 && !LoggedIn)          // "Login" logo
                            {
                                resetarrows = true;
                                gameState = GameState.Login;
                            }
                            
                            break;
                    } // switch statement

                } // if isInAir
            } // foreach arrow

            if (resetarrows)
            {
                bow.resetArrows(Content);
            }

        } // HandleCollisions
    } //class Game1
} // namespace Archer
