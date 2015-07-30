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
using MySql.Data.MySqlClient;


namespace Archer
{
    class LoginForm
    {
        #region fields
        const float sWIDTH = 1024;
        const float sHEIGHT = 768;
        const float OFFSET = 5;
        const float XPOS = (sWIDTH / 3) + OFFSET;
        const float MOVESPEED = 20;
        const float TICK_INTERV = 2000000;
        float yStartPos;
        float yEndPos;
        const string SERVER = "localhost";
        const string DB = "archer";
        const string UID = "root";
        const string DB_PASSWORD = "";
        String email;
        String password;
        String username;
        Texture2D background;
        Texture2D xButton;
        Texture2D cursor;
        bool isOnScreen;
        bool isMovingOffScreen;
        bool isdone;
        Vector2 Position;
        Vector2 xButtonPos;
        Vector2 CursorPos;
        Vector2 emailPos;
        Vector2 passwordPos;
        KeyboardState keystate;
        KeyboardState lastkeystate;
        bool cursortoggle;
        bool cursorEnabled;
        bool IsLoggedIn;
        bool invalidLogin;
        float timer;
        #endregion


        public LoginForm(ContentManager content)
        {
            background = content.Load<Texture2D>("Menu\\LoginMenu");
            xButton = content.Load<Texture2D>("Menu\\xButton");
            cursor = content.Load<Texture2D>("Menu\\Cursor");
            CursorPos.X = XPOS + 16;
            emailPos.X = XPOS + 32;
            passwordPos.X = XPOS + 32;
            yStartPos = -background.Height;
            yEndPos = sHEIGHT / 3;
            Position = new Vector2(XPOS, yStartPos);
            xButtonPos = new Vector2(XPOS + (background.Width - xButton.Width) - 5, yStartPos);
            isOnScreen = false;
            isMovingOffScreen = false;
            isdone = false;
            cursortoggle = false;
            email = "";
            password = "";
            cursorEnabled = false;
            IsLoggedIn = false;
            invalidLogin = false;
            timer = 0;
        } //LoginForm

        public void Update(float elapsed)
        {

            timer += elapsed;

            if (timer > TICK_INTERV)
            {
                timer = 0;
                cursorEnabled = !cursorEnabled;
            }

            keystate = Keyboard.GetState();

            if (!isOnScreen)                //Move onto screen
            {
                MoveDown();
                if (Position.Y >= yEndPos)
                {   
                    isOnScreen = true;
                    Position.Y = yEndPos;
                    xButtonPos.Y = yEndPos + 5;
                    CursorPos.Y = Position.Y + 84;
                }
            }

            MouseState mouse = Mouse.GetState();

            if ( (mouse.LeftButton == ButtonState.Pressed) &&
                 ( (mouse.X > xButtonPos.X - (xButton.Width / 2)) && (mouse.X < xButtonPos.X + (xButton.Width / 2)) ) &&
                 ( (mouse.Y > xButtonPos.Y - (xButton.Height / 2) ) && (mouse.Y < xButtonPos.Y + (xButton.Height / 2)) ) )      //check if mouse button is pressed and is intersecting exit button in order to close login form
            {
                isMovingOffScreen = true;
            }

            if (isMovingOffScreen)
            {
                MoveUp();
                if (Position.Y <= yStartPos)
                {
                    isOnScreen = false;
                    isMovingOffScreen = false;
                    Position.Y = yStartPos;
                    xButtonPos.Y = yStartPos;
                    isdone = true;          
                }
            }

            if (keystate.IsKeyDown(Keys.Tab) && lastkeystate.IsKeyUp(Keys.Tab))
            {
                ToggleCursor();
            }

            if ( keystate.IsKeyDown(Keys.Enter) && lastkeystate.IsKeyUp(Keys.Enter) )
            {
                if (ValidateEmailandPassword())
                {
                    if (Login(email, password))
                    {
                        username = GetUsernameFromDB(email);
                        IsLoggedIn = true;
                    }
                    else
                        invalidLogin = true;
                }
                else
                    invalidLogin = true;
            }

            UpdateCursor(keystate, lastkeystate);
            lastkeystate = keystate;
        } //Update

        public void Draw(SpriteBatch spritebatch, SpriteFont font)
        {
            spritebatch.Draw(background, Position, Color.White);
            spritebatch.Draw(xButton, xButtonPos, Color.White);

            if (isOnScreen && cursorEnabled)
                spritebatch.Draw(cursor, CursorPos, Color.White);

            if (isOnScreen && !isMovingOffScreen)
            {
                spritebatch.DrawString(font, email, emailPos, Color.Black, 0f, new Vector2(0, 0), 0.2f, SpriteEffects.None, 0);

                string password_encrypted = "";

                for (int i = 0; i < password.Length; i++)
                {
                    password_encrypted += "*";
                }

                spritebatch.DrawString(font, password_encrypted, passwordPos, Color.Black, 0f, new Vector2(0, 0), 0.2f, SpriteEffects.None, 0);
            }

            if (isOnScreen && invalidLogin && !isMovingOffScreen)
            {
                spritebatch.DrawString(font, "Login Failed.", new Vector2(456, 490), Color.Red, 0f, new Vector2(0, 0), 0.2f, SpriteEffects.None, 0);
            }
        } //Draw

        public bool IsDone()
        {
            return isdone;
        } //IsDone

        private void MoveUp()
        {
            Position.Y = Position.Y - MOVESPEED;
            xButtonPos.Y = Position.Y + 5;
        } //MoveUp

        private void MoveDown()
        {
            Position.Y = Position.Y + MOVESPEED;
            xButtonPos.Y = Position.Y + 5;
        } //MoveDown

        private void ToggleCursor()
        {
            cursortoggle = !cursortoggle;
        } //ToggleCursor

        private void UpdateCursor(KeyboardState keystate, KeyboardState lastkeystate)
        {
            Keys[] lastpressedkeys = keystate.GetPressedKeys();
            Keys[] prev_lastpressedkeys = lastkeystate.GetPressedKeys();
            bool isCaps = (keystate.IsKeyDown(Keys.LeftShift) || keystate.IsKeyDown(Keys.RightShift));
            string prev_key = "";

            if (prev_lastpressedkeys.Length > 0)
            {
                prev_key = prev_lastpressedkeys.Last().ToString();
                prev_key = makeNumeric(prev_key, isCaps);
            }
               
            if (lastpressedkeys.Length > 0)
            {   
                string new_key = lastpressedkeys.Last().ToString();
                bool can_type = true;

                if (isCaps)
                    new_key = lastpressedkeys[0].ToString();

                new_key = makeNumeric(new_key, isCaps);

                if (!cursortoggle)
                {
                    CursorPos.Y = Position.Y + 84;
                    emailPos.Y = Position.Y + 84 + 8;

                    if (new_key != prev_key && isAlphaNumeric(new_key) )
                    {
                        foreach (Keys key in prev_lastpressedkeys)
                        {
                            foreach (Keys key_new in lastpressedkeys)
                            {
                                if (key_new == key && key.ToString() != "LeftShift" && key.ToString() != "RightShift")
                                {   
                                    can_type = false;
                                }
                            }
                        }

                        if (can_type)
                            email +=  isCaps ? new_key.ToUpper() :  new_key.ToLower();
                    }

                    if ( keystate.IsKeyDown(Keys.Back) && lastkeystate.IsKeyUp(Keys.Back) && email.Length > 0 )
                    {
                        email = email.Substring(0, email.Length - 1);
                    }
                }
                else
                {
                    CursorPos.Y = Position.Y + 188;
                    passwordPos.Y = Position.Y + 188 + 8;

                    if (new_key != prev_key && isAlphaNumeric(new_key))
                    {
                        foreach (Keys key in prev_lastpressedkeys)
                        {
                            foreach (Keys key_new in lastpressedkeys)
                            {
                                if (key_new == key && key.ToString() != "LeftShift" && key.ToString() != "RightShift")
                                {
                                    can_type = false;
                                }
                            }
                        }

                        if (can_type)
                            password += isCaps ? new_key.ToUpper() : new_key.ToLower();
                    }

                    if (keystate.IsKeyDown(Keys.Back) && lastkeystate.IsKeyUp(Keys.Back) && password.Length > 0)
                    {
                        password = password.Substring(0, password.Length - 1);
                    }
                }

            }

        } //UpdateCursor

        private bool isAlphaNumeric(string c)
        {

            if (c.Length > 1)
                return false;

            if (c == "!" || c == "#" || c == "$" || c == "%" || c == "^" || c == "&" || c == "*" || c == "." ||
                c == "(" || c == ")" || c == "_" || c == "+" || c == "@" || c == "-" || c == "=" || c == " ")
                return true;

            if ( c.All(char.IsLetterOrDigit) )
            {
                return true;
            }

            return false;
        }

        private string makeNumeric(string c, bool caps)
        {
            caps = !caps;

            switch (c)
            {
                case "D1": c = caps ? "1" : "!";
                    break;
                case "D2": c = caps ? "2" : "@";
                    break;
                case "D3": c = caps ? "3" : "#";
                    break;
                case "D4": c = caps ? "4" : "$";
                    break;
                case "D5": c = caps ? "5" : "%";
                    break;
                case "D6": c = caps ? "6" : "^";
                    break;
                case "D7": c = caps ? "7" : "&";
                    break;
                case "D8": c = caps ? "8" : "*";
                    break;
                case "D9": c = caps ? "9" : "(";
                    break;
                case "D0": c = caps ? "0" : ")";
                    break;
                case "Oem Minus": c = caps ? "-" : "_";
                    break;
                case "Oem Plus": c = caps ? "=" : "+";
                    break;
                case "OemPeriod": c = ".";
                    break;
                case "Space": return " ";
            }

            return c;
        }

        public bool Login(string email, string password)
        {
            String str = "server=" + SERVER + ";database=" + DB + ";userid=" + UID + ";password=" + DB_PASSWORD + ";";
            MySqlConnection con = null;
            MySqlDataReader rdr = null;
            email = email.ToLower();

            try
            {
                con = new MySqlConnection(str);
                con.Open();
                string query = "SELECT EXISTS (SELECT 1 FROM users WHERE email = '" + email + "' AND password = '" + password + "')";
                MySqlCommand cmd = new MySqlCommand(query, con);
                rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    if (rdr.GetBoolean(0))
                    {
                        isMovingOffScreen = true;
                        return true;
                    }
                    else
                    {
                        invalidLogin = true;
                        return false;
                    }
                }
                else
                    return false;

            }
            catch (Exception e)
            {
                System.Console.WriteLine( "FATAL ERROR: {0}", e.ToString() );
                invalidLogin = true;
                return false;
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        } //Login

        private string GetUsernameFromDB(string email)
        {
            String str = "server=" + SERVER + ";database=" + DB + ";userid=" + UID + ";password=" + DB_PASSWORD + ";";
            MySqlConnection con = null;
            MySqlDataReader rdr = null;
            email = email.ToLower();

            try
            {
                con = new MySqlConnection(str);
                con.Open();
                string query = "SELECT username FROM users WHERE email = '" + email + "'";
                MySqlCommand cmd = new MySqlCommand(query, con);
                rdr = cmd.ExecuteReader();

                if ( rdr.Read() )
                    return rdr.GetString(0);
            }
            catch (Exception e)
            {
                System.Console.WriteLine( "FATAL ERROR: {0}", e.ToString() );
                con.Close();
                return null;
            }
            finally
            {
                if (con != null)
                    con.Close();
            }

            return null;
        } //GetUserNameFromDB()

        public string GetEmail()
        {
            return email;
        }

        public string GetPassword()
        {
            return password;
        }

        public string GetUsername()
        {
            return username;
        }

        public bool Is_Logged_In()
        {
            return IsLoggedIn;
        }

        private bool ValidateEmailandPassword()
        {
            if (email == "" || password == "")
                return false;

            if (email.Contains("@") && email.Contains("."))
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return (addr.Address == email);
            }
            else
            {
                return false;
            }
        } //ValidateEmailandPassword()

        public static bool IsNewHighScore(string username, int score)
        {
            String str = "server=" + SERVER + ";database=" + DB + ";userid=" + UID + ";password=" + DB_PASSWORD + ";";
            MySqlConnection con = null;
            MySqlDataReader rdr = null;

            try
            {
                con = new MySqlConnection(str);
                con.Open();
                string query = "SELECT archer FROM high_scores WHERE username = '" + username + "'";
                MySqlCommand cmd = new MySqlCommand(query, con);
                rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {
                    int highscore = rdr.GetInt32(0);

                    if (score > highscore)
                        return true;
                    else
                        return false;
                }
                else
                    return false;

            }
            catch (Exception e)
            {
                System.Console.WriteLine("FATAL ERROR: {0}", e.ToString());
                return false;
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        } //IsNewHighScore(string username, int score)

        public static void UpdateHighScore(string username, int score)
        {
            String str = "server=" + SERVER + ";database=" + DB + ";userid=" + UID + ";password=" + DB_PASSWORD + ";";
            MySqlConnection con = null;

            try
            {
                con = new MySqlConnection(str);
                con.Open();
                string query = "UPDATE high_scores SET archer = " + score + " WHERE username = '" + username + "'";
                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("FATAL ERROR: {0}", e.ToString());
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        } //public static void UpdateHighScore(string unsername, int score)
    } //class LoginForm
} //namespace Archer
