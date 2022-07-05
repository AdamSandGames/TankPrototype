using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;



namespace Tankgame1
{

    public class Tank : Microsoft.Xna.Framework.DrawableGameComponent
    {
        VertexPositionNormalTexture[] pnverts;
        Texture2D texture;
        Effect PointDiffSpecTextureEffect;

        Vector3[] pts;
        public Matrix worldTrans { get; set; } = Matrix.Identity;
        public Matrix worldRot { get; set; } = Matrix.Identity;
        Game1 myGame;
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        readonly float speed = 160f;
        float accel = 0f;
        float gravity = 50f;
        public Vector3 Velocity { get; set; } = new Vector3(0, 0, 0);
        //Matrix view = Matrix.Identity;
        //Matrix proj = Matrix.Identity;
        //Vector4 lPos = Vector4.Zero;
        Vector2[] tcs;
        //Vector4 CamPos;
        //Vector4 Ambient;
        //Vector4 LightColor;

        // My Vars
        //Camera camera;
        KeyboardState keyboardState;
        //BasicEffect teffect;
        float yRotation;
        Matrix WorldM;

        //Turret
        public TankTurret myTurret { get; set; }
        public TankBarrel myBarrel { get; set; }

        //Rotation Book
        //Vector3 rotRight;//Matrix first row
        public Vector3 rotUp { get; set; }//Matrix second row
        //Vector3 rotForward;//Matrix third row
        //Vector3 posNorm;
        Matrix normalRotation;
        bool touchingGround;
        float jumpVelocity = 50f;

        public Tank(Game game, Vector3 pos)
            : base(game)
        {

            myGame = (Game1)game;
            pnverts = new VertexPositionNormalTexture[36];

            //camera = cam;
            Position = pos;
            Direction = new Vector3(0, 0, 1);
            yRotation = 0f;

            int iv = 0;
            pts = new Vector3[8];
            Vector3[] nms = new Vector3[8];
            tcs = new Vector2[8];


            pts[0].X = -1.0f;
            pts[0].Y = -1.0f;
            pts[0].Z = -1.0f;
            pts[1].X = -1.0f;
            pts[1].Y = 1.0f;
            pts[1].Z = -1.0f;
            pts[2].X = -1.0f;
            pts[2].Y = -1.0f;
            pts[2].Z = 1.0f;
            pts[3].X = -1.0f;
            pts[3].Y = 1.0f;
            pts[3].Z = 1.0f;
            pts[4].X = 1.0f;
            pts[4].Y = -1.0f;
            pts[4].Z = -1.0f;
            pts[5].X = 1.0f;
            pts[5].Y = 1.0f;
            pts[5].Z = -1.0f;
            pts[6].X = 1.0f;
            pts[6].Y = -1.0f;
            pts[6].Z = 1.0f;
            pts[7].X = 1.0f;
            pts[7].Y = 1.0f;
            pts[7].Z = 1.0f;

            for (int i = 0; i < 8; i++)
            {
                pts[i].Z *= 3.0f;
                pts[i].X *= 2.0f;
            }

            nms[0].X = 0.0f;
            nms[0].Y = 1.0f;
            nms[0].Z = 0.0f;
            nms[1].X = 0.0f;
            nms[1].Y = -1.0f;
            nms[1].Z = 0.0f;
            nms[2].X = 1.0f;
            nms[2].Y = 0.0f;
            nms[2].Z = 0.0f;
            nms[3].X = -1.0f;
            nms[3].Y = 0.0f;
            nms[3].Z = 0.0f;
            nms[4].X = 0.0f;
            nms[4].Y = 0.0f;
            nms[4].Z = 1.0f;
            nms[5].X = 0.0f;
            nms[5].Y = 0.0f;
            nms[5].Z = -1.0f;

            tcs[0].X = 0.0f;
            tcs[0].Y = 0.0f;
            tcs[1].X = 1.0f;
            tcs[1].Y = 0.0f;
            tcs[2].X = 0.0f;
            tcs[2].Y = 1.0f;
            tcs[3].X = 1.0f;
            tcs[3].Y = 1.0f;

            pnverts[iv++] = new VertexPositionNormalTexture(pts[0], nms[3], tcs[1]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[1], nms[3], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[2], nms[3], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[2], nms[3], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[1], nms[3], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[3], nms[3], tcs[2]);

            pnverts[iv++] = new VertexPositionNormalTexture(pts[4], nms[1], tcs[1]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[0], nms[1], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[6], nms[1], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[6], nms[1], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[0], nms[1], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[2], nms[1], tcs[2]);

            pnverts[iv++] = new VertexPositionNormalTexture(pts[7], nms[0], tcs[1]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[3], nms[0], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[5], nms[0], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[5], nms[0], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[3], nms[0], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[1], nms[0], tcs[2]);

            pnverts[iv++] = new VertexPositionNormalTexture(pts[4], nms[5], tcs[1]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[0], nms[5], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[5], nms[5], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[5], nms[5], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[0], nms[5], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[1], nms[5], tcs[2]);

            pnverts[iv++] = new VertexPositionNormalTexture(pts[6], nms[4], tcs[1]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[7], nms[4], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[2], nms[4], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[2], nms[4], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[7], nms[4], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[3], nms[4], tcs[2]);

            pnverts[iv++] = new VertexPositionNormalTexture(pts[4], nms[2], tcs[1]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[6], nms[2], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[5], nms[2], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[5], nms[2], tcs[0]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[6], nms[2], tcs[3]);
            pnverts[iv++] = new VertexPositionNormalTexture(pts[7], nms[2], tcs[2]);

        }

        public override void Initialize()
        {
            myTurret = new TankTurret(myGame, this);
            myGame.Components.Add(myTurret);
            PointDiffSpecTextureEffect = Game.Content.Load<Effect>("Effects/DiffuseTexture"); //("Effects/DiffSpecPointTexture");
            WorldM = worldRot * worldTrans;
            touchingGround = false;
            //teffect = new BasicEffect(GraphicsDevice);
            //teffect.VertexColorEnabled = true;
            //teffect.World = worldRot * worldTrans;
            //CamPos = camera.getCamPos();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("Textures/brick2_b");
            //PointDiffSpecTextureEffect = Game.Content.Load<Effect>("Effects/DiffuseTexture"); //("Effects/DiffSpecPointTexture");
            if (PointDiffSpecTextureEffect != null)
            {
                Console.WriteLine("PointDiff Check Pass");
            }
            else
                Console.WriteLine("PointDiff Check Fail");
            //Ambient.X = 0.2f; // ambient light
            //Ambient.Y = 10.0f; // specular exponent
            //Ambient.Z = 1.0f; // specular intensity
            //Ambient.W = 1.0f;

            //LightColor.X = 0.6f;
            //LightColor.Y = 0.4f;
            //LightColor.Z = 0.8f;
            //LightColor.W = 0.0f;
            //lPos.Y = 50f;

            base.LoadContent();
        }


        public override void Update(GameTime gameTime)
        {
            if (myGame.GameOn)
            {
                //CamPos = camera.getCamPos();
                if (myGame.camMode == 1)
                    keyInput(gameTime);

                rotationCompiler();
                //Gravity
                if (Position.Y > (myGame.getElevation(Position) + 1.1f))
                {
                    touchingGround = false;
                    Velocity += Vector3.Down * gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    touchingGround = true;
                    //Position = new Vector3(Position.X, (myGame.getElevation(Position) + 1f), Position.Z);

                    //Acceleration
                    Velocity += Direction * accel * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    //Drag
                    if (Velocity.Length() > 0.05f)
                        Velocity *= 1f - ((float)gameTime.ElapsedGameTime.TotalSeconds) * 5f;// * myGame.floatLerp(1f / 3f, 12f, Velocity.Length() / 12f) );
                    else
                        Velocity *= 0f;
                }

                //Fences
                if (Math.Abs(Position.X + Velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds) > myGame.fenceDistance)// || Math.Abs(Position.Z) > myGame.fenceZ)
                {
                    Velocity = new Vector3(0f, Velocity.Y, Velocity.Z);
                }
                if (Math.Abs(Position.Z + Velocity.Z * (float)gameTime.ElapsedGameTime.TotalSeconds) > myGame.fenceDistance)// || Math.Abs(Position.Z) > myGame.fenceZ)
                {
                    Velocity = new Vector3(Velocity.X, Velocity.Y, 0f);
                }
                //Movement
                Position += 1f * Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (Position.Y > (myGame.getElevation(Position) + 1.1f))
                {
                    touchingGround = false;
                }
                else
                {
                    touchingGround = true;
                    Position = new Vector3(Position.X, (myGame.getElevation(Position) + 1f), Position.Z);
                }

                SetTranslation();
            }
            
            
            base.Update(gameTime);
        }

        public void SetTranslation()
        {
            //Position = new Vector3(Position.X, (myGame.getElevation(Position) + 1f), Position.Z);
            //Position.Y = (myGame.getElevation(Position) + 1f);
            worldTrans = Matrix.CreateTranslation(Position);
            WorldM = normalRotation * worldTrans;
            //teffect.World = WorldM;
        }
        void keyInput(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();

            float n = 0f;
            // Turning
            if (keyboardState.IsKeyDown(Keys.A))
            {
                n += 1f; // Left
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                n += -1f; // Right
            }
            if (keyboardState.IsKeyDown(Keys.LeftControl))
            {
                n *= 1f / 3f; // Precision Controls
            }
            yRotation += 1f * n * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (yRotation > MathHelper.Pi)
            {
                yRotation = -MathHelper.TwoPi + yRotation;
            }
            else if (yRotation < -MathHelper.Pi)
            {
                yRotation = MathHelper.TwoPi - yRotation;
            }
            //rotRight = new Vector3((float)Math.Cos(yRotation), 0, -(float)Math.Sin(yRotation));
            //rotForward = new Vector3((float)Math.Sin(yRotation), 0, (float)Math.Cos(yRotation));


            worldRot = new Matrix(new Vector4((float)Math.Cos(yRotation), 0, -(float)Math.Sin(yRotation), 0),
                                   new Vector4(0, 1, 0, 0),
                                   new Vector4((float)Math.Sin(yRotation), 0, (float)Math.Cos(yRotation), 0),
                                   new Vector4(0, 0, 0, 1));

            Direction = new Vector3((float)Math.Cos(yRotation), 0, -(float)Math.Sin(yRotation));
            //Direction = new Vector3((float)Math.Sin(yRotation), 0, (float)Math.Cos(yRotation));
            //Direction = Vector3.Transform(Vector3.Forward, worldRot);
            Direction.Normalize();

            // Moving
            int a = 0;
            accel = speed;
            if (keyboardState.IsKeyDown(Keys.W))
            {
                a += 1; // Forwards
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                a -= 1; // Backwards
            }
            if (keyboardState.IsKeyDown(Keys.LeftShift))
            {
                a *= 2; // Speed multiplier
            }
            accel = a * speed;

            if (touchingGround == true && keyboardState.IsKeyDown(Keys.Space))
            {
                Velocity += Vector3.Up * jumpVelocity;
            }
        }

        void rotationCompiler()
        {
            //posNorm = myGame.getNormal(Position);
            rotUp = touchingGround ? myGame.getNormal(Position) : rotUp; // new Vector3(posNorm.X, posNorm.Y, posNorm.Z);
            rotUp.Normalize();
            Vector3 normLeft = Vector3.Cross(rotUp, Direction);
            normLeft.Normalize();
            Vector3 newDir = Vector3.Cross(normLeft, rotUp);
            newDir.Normalize();

            normalRotation = Matrix.Identity;
            normalRotation.Left = normLeft;
            normalRotation.Up = rotUp;
            normalRotation.Forward = newDir;
            //Direction = normalRotation.Forward;

        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            //myGame.setCamera(teffect);

            if (myGame.worldShader != null)
            {
                // Console.WriteLine("PointDiff Draw Pass");
                myGame.shaderAssignment(WorldM, texture);

                foreach (EffectPass pass in myGame.worldShader.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, pnverts, 0, 12);
                }
            }
            else
            {
                LoadContent();
            }
            


            base.Draw(gameTime);
        }

        public Vector3 getPosition()
        {
            return Position;
        }
        public Vector3 getDirection()
        {
            return Direction;
        }

        public Matrix getWorldMatrix()
        {
            return WorldM;
        }
        public bool isTouchingGround()
        {
            return touchingGround;
        }
        
        //void refreshNormals()
        //{
        //    pts[0].X = -1.0f;
        //    pts[0].Y = -1.0f;
        //    pts[0].Z = -1.0f;
        //    pts[1].X = -1.0f;
        //    pts[1].Y = 1.0f;
        //    pts[1].Z = -1.0f;
        //    pts[2].X = -1.0f;
        //    pts[2].Y = -1.0f;
        //    pts[2].Z = 1.0f;
        //    pts[3].X = -1.0f;
        //    pts[3].Y = 1.0f;
        //    pts[3].Z = 1.0f;
        //    pts[4].X = 1.0f;
        //    pts[4].Y = -1.0f;
        //    pts[4].Z = -1.0f;
        //    pts[5].X = 1.0f;
        //    pts[5].Y = 1.0f;
        //    pts[5].Z = -1.0f;
        //    pts[6].X = 1.0f;
        //    pts[6].Y = -1.0f;
        //    pts[6].Z = 1.0f;
        //    pts[7].X = 1.0f;
        //    pts[7].Y = 1.0f;
        //    pts[7].Z = 1.0f;


        //    Vector3[] nms = new Vector3[8];
        //    nms[0].X = 0.0f;
        //    nms[0].Y = 1.0f;
        //    nms[0].Z = 0.0f;
        //    nms[0] = Vector3.Cross();
        //    nms[1].X = 0.0f;
        //    nms[1].Y = -1.0f;
        //    nms[1].Z = 0.0f;
        //    nms[2].X = 1.0f;
        //    nms[2].Y = 0.0f;
        //    nms[2].Z = 0.0f;
        //    nms[3].X = -1.0f;
        //    nms[3].Y = 0.0f;
        //    nms[3].Z = 0.0f;
        //    nms[4].X = 0.0f;
        //    nms[4].Y = 0.0f;
        //    nms[4].Z = 1.0f;
        //    nms[5].X = 0.0f;
        //    nms[5].Y = 0.0f;
        //    nms[5].Z = -1.0f;

        //    int iv = 0;
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[0], nms[3], tcs[1]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[1], nms[3], tcs[3]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[2], nms[3], tcs[0]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[2], nms[3], tcs[0]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[1], nms[3], tcs[3]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[3], nms[3], tcs[2]);

        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[4], nms[1], tcs[1]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[0], nms[1], tcs[3]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[6], nms[1], tcs[0]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[6], nms[1], tcs[0]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[0], nms[1], tcs[3]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[2], nms[1], tcs[2]);

        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[7], nms[0], tcs[1]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[3], nms[0], tcs[3]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[5], nms[0], tcs[0]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[5], nms[0], tcs[0]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[3], nms[0], tcs[3]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[1], nms[0], tcs[2]);

        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[4], nms[5], tcs[1]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[0], nms[5], tcs[3]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[5], nms[5], tcs[0]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[5], nms[5], tcs[0]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[0], nms[5], tcs[3]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[1], nms[5], tcs[2]);

        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[6], nms[4], tcs[1]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[7], nms[4], tcs[3]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[2], nms[4], tcs[0]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[2], nms[4], tcs[0]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[7], nms[4], tcs[3]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[3], nms[4], tcs[2]);

        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[4], nms[2], tcs[1]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[6], nms[2], tcs[3]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[5], nms[2], tcs[0]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[5], nms[2], tcs[0]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[6], nms[2], tcs[3]);
        //    pnverts[iv++] = new VertexPositionNormalTexture(pts[7], nms[2], tcs[2]);
        //}
        
        
    }
}
