using System;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Archer
{
    public class CollidableObject
    {
        #region Fields

        private Texture2D texture;
        private Vector2 position;
        private float rotation;
        private Vector2 origin;
        private Color[] textureData;

        #endregion

        #region Properties

        public Vector2 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public Texture2D Texture
        {
            get 
            { 
                return this.texture;
            }
            set
            {
                this.texture = value;
            }
 
        }

        public Color[] TextureData
        {
            get { return this.textureData; }
        }

        public Vector2 Origin
        {
            get { return this.origin; }
            set { this.origin = value; }
        }

        public Rectangle Rect
        {
            get { return new Rectangle(0, 0, this.Texture.Width, this.Texture.Height); }
        }

        public Matrix Transform
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-this.Origin, 0.0f)) *
                                        Matrix.CreateRotationZ(this.Rotation) *
                                        Matrix.CreateTranslation(new Vector3(this.Position, 0.0f));
            }
        }

        public Rectangle BoundingRectangle
        {
            get { return CalculateBoundingRectangle(this.Rect, this.Transform); }
        }

        #endregion

        #region Constructors

        public CollidableObject(Texture2D texture, Vector2 position)
            : this(texture, position, 0.0f)
        {
        }

        public CollidableObject(Texture2D texture, Vector2 position, float rotation)
        {
            this.LoadTexture(texture);
            this.position = position;
            this.rotation = rotation;
        }

        #endregion

        #region Instance Methods

        public void MoveLeft(float moveBy)
        {
            this.position.X -= moveBy;
        }

        public void MoveRight(float moveBy)
        {
            this.position.X += moveBy;
        }

        public void MoveUp(float moveBy)
        {
            this.position.Y -= moveBy;
        }

        public void MoveDown(float moveBy)
        {
            this.position.Y += moveBy;
        }

        public void Rotate(float rotateBy)
        {
            if (rotateBy < 0)
            {
                this.rotation -= rotateBy;
            }
            else
            {
                this.rotation += rotateBy;
            }
        }

        public float Rotation
        {
            get
            {
                return this.rotation;
            }
            set
            {
                this.rotation = value;
            }
        }

        public bool IsColliding(CollidableObject collidable)
        {
            bool retval = false;

            if (this.BoundingRectangle.Intersects(collidable.BoundingRectangle))
            {
                if (IntersectPixels(this.Transform, this.Texture.Width, this.Texture.Height, this.TextureData, collidable.Transform, collidable.Texture.Width, collidable.Texture.Height, collidable.TextureData))
                {
                    retval = true;
                }
            }

            return retval;
        }

        public void LoadTexture(Texture2D texture)
        {
            this.texture = texture;
            this.origin = new Vector2(texture.Width / 2, texture.Height / 2);
            this.textureData = new Color[texture.Width * texture.Height];
            this.texture.GetData(this.textureData);
        }

        public void LoadTexture(Texture2D texture, Vector2 origin)
        {
            this.LoadTexture(texture);
            this.origin = origin;
        }

        #endregion

        #region Static Methods

        public static bool IntersectPixels(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB)
        {
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }

        public static bool IntersectPixels(Matrix transformA, int widthA, int heightA, Color[] dataA, Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            for (int yA = 0; yA < heightA; yA++)
            {
                Vector2 posInB = yPosInB;

                for (int xA = 0; xA < widthA; xA++)
                {
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            return true;
                        }
                    }

                    posInB += stepX;
                }

                yPosInB += stepY;
            }

            return false;
        }

        public static Rectangle CalculateBoundingRectangle(Rectangle rectangle, Matrix transform)
        {
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        #endregion
    }
}
