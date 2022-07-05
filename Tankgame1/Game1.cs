using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
// Adam Sanders
// Idea: Mining under a red sun
//          While specular brightness is > x, destroyed terrain grants score.
//          Must manage ammo that generates slowly over time and consumes minerals
// Idea: Tower of Babel
//          Staying above a certain altitude for x seconds wins the game.
namespace Tankgame1
{
    struct Fog
    {
        public Fog(Color col, float dens, float dist)
        {
            Color4 = col.ToVector4();
            Density = dens;
            Distance = dist;
        }
        public Vector4 Color4 { get; set; } // Fog Color
        public float Density { get; set; } // Number of Distance lengths before fog fully obscures
        public float Distance { get; set; } // Distance
    } // Contains color and thickness data
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // Variables/Properties
        // Sphere
        // declare sphere variable
        Sphere aSphere;
        Sphere[] spheres;
        bool makespheres = false;
        VertexPositionColor[] terrainVPColor;
        // Triangle
        bool maketriangle = false;
        VertexPositionColor[] verts;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont Font1;
        SpriteFont uiFont;

        Song bgmusic;
        public SoundEffect gunFire { get; set; }

        Tank tank;
        Camera camera;
        Matrix worldTrans = Matrix.Identity;
        Matrix worldRot = Matrix.Identity;

        BasicEffect ceffect;


        //Mouse Input
        float CamXangle;
        float CamYangle;
        MouseState mouseState;
        MouseState prevMouseState;
        float mx, my, dmx, dmy, mScale;
        KeyboardState keyboardState;
        KeyboardState prevKeyboardState;
        //float keyX;
        //float keyY;
        //float keyZ;
        Vector3 moveDir;

        // Draw
        public int camMode { get; set; } = 0;
        float tankCamDist { get; set; } = 20f;
        Vector3 tankViewPoint { get; set; }
        public int fishEye { get; set; } = 0;
        public Effect shaderLightNormal { get; set; }
        public Effect shaderLightFisheye { get; set; }
        public Effect worldShader { get; set; }
        VertexBuffer vb;
        Texture2D backupTex;
        BasicEffect effect;

        // Height Field
        public static float[,] mHeights;
        Random rng;

        // Normals
        public static Vector3[,] normals;

        // Map Texture
        VertexPositionNormalTexture[] terrainVPNTex;
        Texture2D groundTexture;
        Texture2D preGroundTexture;
        Effect groundTexEffect;
        public float fenceDistance { get; set; }
        //public float fenceZ { get; set; }
        Color fogColor = new Color(255, 110, 76, 255); // Red Sand

        // Lighting
        LightSource light1;

        Texture2D tiles;

        // Game Elements
        public bool GameOn { get; set; }
        bool GameWin;
        public float Timer { get; set; }
        public int Score { get; set; }
        public float fScore { get; set; }
        public float Minerals { get; set; }
        public int Ammunition { get; set; }
        public int LiveShots { get; set; }
        public int shotCost = 100;
        float mineralConsume = 0;
        float usedMinerals = 0;
        float productionTime = 0;
        float heightSum = 0;
        float initialHSum;
        float currentHeighest;
        float altAccumulator;
        public string gunMode { get; set; } = "Craters";

        int screenWidth;
        int screenHeight;

        //Special Handles
        public float gameVolume { get; set; } = 0.05f;
        float MaxAlt = 30f;
        public static int gDim = 400; // 100? 300? 3000?
        float terrainScale = 3f; // Not great above 3
        float texScale = 6f;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;

            this.IsMouseVisible = false;
            Window.IsBorderless = true;
            screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth = screenWidth;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = screenHeight;   // set this value to the desired height of your window
            this.Window.Position = new Point(0, 0);  // set this point to the desired upper left corner of your window
            graphics.ApplyChanges();

            //Mouse.SetPosition(Window.ClientBounds.X / 2, Window.ClientBounds.Y / 2);
            Mouse.SetPosition(screenWidth / 2, screenHeight / 2);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            GameOn = true;
            GameWin = false;
            altAccumulator = 0;
            Timer = 5f * 60f; // 5 minutes
            Score = 0;
            fScore = 0;
            Minerals = 0;
            Ammunition = 10;

            camera = new Camera(this, new Vector3(20, 15, 0), Vector3.Zero, Vector3.Up);
            Components.Add(camera);

            tank = new Tank(this, Vector3.Zero);
            Components.Add(tank);

            // Lighting
            DiffuseLight diff = new DiffuseLight(Color.White, 0.7f);
            AmbientLight ambi = new AmbientLight(Color.White, 0.2f);
            SpecularLight spec = new SpecularLight(5f, Color.BlueViolet, 1f);
            Fog frog = new Fog(fogColor, 0.08f, 35.0f); // Red Sand

            light1 = new LightSource(this, new Vector3(0, 0, 0), Vector3.One, Color.Aqua, diff, ambi, spec, frog, new Vector3(500, 500, 0));
            Components.Add(light1);
            //light1 = new LightSource(this, new Vector3(0, 40, 0), Vector3.One, Color.Aqua, diff, ambi, spec, frog, new Vector3(20, 0, 20));
            //Components.Add(light1);

            camMode = 0;
            if (Components.Contains(tank))
                camMode = 1; // 0 for free camera, 1 for tank camera
            fishEye = 0;
            //camera.toggleLens();

            Debug.AutoFlush = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            rng = new Random(37);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Font1 = Content.Load<SpriteFont>("Font1");
            uiFont = Content.Load<SpriteFont>("uiFont1");

            gunFire = Content.Load<SoundEffect>("Sounds/Gun+Silencer");
            bgmusic = Content.Load<Song>("Sounds/05. Imitation");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = gameVolume;
            if (MediaPlayer.State != MediaState.Playing)
                MediaPlayer.Play(bgmusic);

            if (makespheres) // SPHERES
            {
                // aSphere = new Sphere(this);
                //Components.Add(aSphere);
                //aSphere.SetLightDir(new Vector3(0.57735f, 0.57735f, 0.57735f));
                spheres = new Sphere[4];

                for (int i = 0; i < spheres.Length; i++)
                {
                    spheres[i] = new Sphere(this);
                    Components.Add(spheres[i]);
                    spheres[i].SetLightDir(new Vector3(0.57735f, 0.57735f, 0.57735f));
                }

                // aSphere.SetTranslation(new Vector3(15, 5, -20));
                foreach (Sphere tSphere in spheres)
                {
                    tSphere.SetTranslation(new Vector3((float)(rng.NextDouble() * 50 - 25), 5, (float)(rng.NextDouble() * 50 - 25)));
                }
            }
            if (maketriangle) // TRIANGLE
            {
                // Initialize vertices
                verts = new VertexPositionColor[3];
                verts[0] = new VertexPositionColor(new Vector3(0, 41, 0), Color.Blue);
                verts[1] = new VertexPositionColor(new Vector3(1, 40, 0), Color.Red);
                verts[2] = new VertexPositionColor(new Vector3(-1, 40, 0), Color.Green);
            }

            //texture = Content.Load<Texture2D>(@"Textures\trees");
            preGroundTexture = Content.Load<Texture2D>(@"Textures/rockytexture");
            float txs = texScale;
            groundTexture = textureTiler(preGroundTexture, txs, txs); //Content.Load<Texture2D>(@"Textures/rockytexture");
            backupTex = Content.Load<Texture2D>("Textures/brick2_b");
            groundTexEffect = Content.Load<Effect>(@"Effects/DiffuseTexture");
            shaderLightNormal = Content.Load<Effect>(@"Effects/DiffuseTexture");
            shaderLightFisheye = Content.Load<Effect>(@"Effects/FisheyeTexture");
            worldShader = shaderLightNormal;
            Texture2D hMapTex = Content.Load<Texture2D>(@"Textures\MtnMap");// can be .bmp, .dds, .dib, .hdr, .jpg, .pfm, .png, .ppm, .tga

            gDim = (int)((hMapTex.Width > hMapTex.Height ? hMapTex.Width : hMapTex.Height) + 0);
            fenceDistance = (terrainScale * gDim / 2) - 2f;// / 10; // TODO
            // Height Field
            mHeights = new float[gDim + 1, gDim + 1];

            bool hmap = true; // either loads heightmap from a file or randomly generates one
            if (hmap)
            {
                otherCalcHeights(hMapTex);
            }
            else
            {

                for (int i = 0; i <= gDim; i++)
                {
                    for (int j = 0; j <= gDim; j++)
                    {
                        mHeights[i, j] = (float)rng.NextDouble() - 4.0f;
                    }
                }
            }
            heightSum = getHeightSum();
            initialHSum = heightSum;
            currentHeighest = getHeighestPoint();

            terrainVPNTex = new VertexPositionNormalTexture[gDim * gDim * 6];
            terrainVPColor = new VertexPositionColor[gDim * gDim * 6];
            ceffect = new BasicEffect(GraphicsDevice);
            ceffect.VertexColorEnabled = true;

            // Normals
            normals = new Vector3[gDim + 1, gDim + 1];

            recalculateVertices(gDim, gDim);
            //Vertices assignment
            //for (int i = 0; i < gDim; i++)
            //{
            //    for (int j = 0; j < gDim; j++)
            //    {
            //        normals[i, j] = Vector3.Up;
            //        float x = (float)(i - (gDim / 2));
            //        float y = mHeights[i, j];
            //        float z = (float)(j - (gDim / 2));
            //        Color vco;
            //        vco = Color.Pink;
            //        terrainVPColor[i * gDim * 6 + j * 6 + 0] = new VertexPositionColor(new Vector3(x, mHeights[i, j], z), vco);
            //        terrainVPColor[i * gDim * 6 + j * 6 + 1] = new VertexPositionColor(new Vector3(x + 1, mHeights[i + 1, j], z), vco);
            //        terrainVPColor[i * gDim * 6 + j * 6 + 2] = new VertexPositionColor(new Vector3(x + 1, mHeights[i + 1, j + 1], z + 1), vco);
            //        terrainVPColor[i * gDim * 6 + j * 6 + 3] = new VertexPositionColor(new Vector3(x, mHeights[i, j], z), vco);
            //        terrainVPColor[i * gDim * 6 + j * 6 + 4] = new VertexPositionColor(new Vector3(x, mHeights[i, j + 1], z + 1), vco);
            //        terrainVPColor[i * gDim * 6 + j * 6 + 5] = new VertexPositionColor(new Vector3(x + 1, mHeights[i + 1, j + 1], z + 1), vco);
            //        // Quick Normals
            //        if (terrainVPColor[i * gDim * 6 + j * 6 + 0].Color != Color.Transparent) // sanity check
            //        {
            //            float k = 0;
            //            Vector3 nNorth = new Vector3(0, 1, 0);
            //            Vector3 nSouth = new Vector3(0, 1, 0);
            //            Vector3 nEast = new Vector3(0, 1, 0);
            //            Vector3 nWest = new Vector3(0, 1, 0);
            //            if (j + 1 <= gDim * 2 + 1)
            //            {
            //                k = mHeights[i, j] - mHeights[i, j + 1];
            //                nNorth = new Vector3(0, 1, k);
            //            }
            //            if (j - 1 >= 0)
            //            {
            //                k = mHeights[i, j] - mHeights[i, j - 1];
            //                nSouth = new Vector3(0, 1, -k);
            //            }
            //            if (i + 1 <= gDim * 2 + 1)
            //            {
            //                k = mHeights[i, j] - mHeights[i + 1, j];
            //                nEast = new Vector3(k, 1, 0);
            //            }
            //            if (i - 1 >= 0)
            //            {
            //                k = mHeights[i, j] - mHeights[i - 1, j];
            //                nWest = new Vector3(-k, 1, 0);
            //            }
            //            Vector3 nOne = nNorth + nSouth + nEast + nWest;
            //            nOne.Normalize();
            //            normals[i, j] = nOne;
            //            //float nNorth = 0;
            //            //float nSouth = 0;
            //            //float nEast = 0;
            //            //float nWest = 0;
            //            //if (i + 1 <= gDim * 2 + 1)
            //            //{
            //            //    nNorth = mHeights[i, j] - mHeights[i + 1, j];
            //            //}
            //            //if (i - 1 >= 0)
            //            //{
            //            //    nSouth = mHeights[i, j] - mHeights[i - 1, j];
            //            //}
            //            //if (j + 1 <= gDim * 2 + 1)
            //            //{
            //            //    nEast = mHeights[i, j] - mHeights[i, j + 1];
            //            //}
            //            //if (j - 1 >= 0)
            //            //{
            //            //    nWest = mHeights[i, j] - mHeights[i, j - 1];
            //            //}

            //            //Vector3 nOne = new Vector3((nEast + nWest) / 2, 1, (nNorth + nSouth) / 2);
            //            //nOne.Normalize();
            //            //normals[i, j] = nOne;

            //            //Vector3 lega = cverts[i * gDim * 6 + j * 6 + 1].Position - cverts[i * gDim * 6 + j * 6 + 0].Position;
            //            //Vector3 legb = cverts[i * gDim * 6 + j * 6 + 2].Position - cverts[i * gDim * 6 + j * 6 + 1].Position;
            //            //normals[i * 2 + 0, j] = Vector3.Cross(lega, legb);
            //            //if (normals[i * 2 + 0, j].Length() > 0)
            //            //{
            //            //    if (normals[i * 2 + 0, j].Y < 0)
            //            //        normals[i * 2 + 0, j].Y *= -1;
            //            //    normals[i * 2 + 0, j].Normalize();
            //            //}
            //            //vco = new Color(normals[i * 2 + 0, j]);
            //            //cverts[i * gDim * 6 + j * 6 + 0].Color = vco;
            //            //cverts[i * gDim * 6 + j * 6 + 1].Color = vco;
            //            //cverts[i * gDim * 6 + j * 6 + 2].Color = vco;

            //            ////Vector3 ldir = new Vector3(0.57735f, 0.57735f, 0.57735f);
            //            ////float red = Vector3.Dot(normals[i, j], ldir);
            //            ////vco = new Color(red, red, red);

            //            //Vector3 legc = cverts[i * gDim * 6 + j * 6 + 4].Position - cverts[i * gDim * 6 + j * 6 + 3].Position;
            //            //Vector3 legd = cverts[i * gDim * 6 + j * 6 + 5].Position - cverts[i * gDim * 6 + j * 6 + 4].Position;
            //            //normals[i * 2 + 1, j] = Vector3.Cross(legd, legc);
            //            //if (normals[i * 2 + 1, j].Length() > 0)
            //            //{
            //            //    if (normals[i * 2 + 1, j].Y < 0)
            //            //        normals[i * 2 + 1, j].Y *= -1;
            //            //    normals[i * 2 + 1, j].Normalize();
            //            //}
            //            //vco = new Color(normals[i * 2 + 1, j]);
            //            //cverts[i * gDim * 6 + j * 6 + 3].Color = vco;
            //            //cverts[i * gDim * 6 + j * 6 + 4].Color = vco;
            //            //cverts[i * gDim * 6 + j * 6 + 5].Color = vco;

            //            //red = Vector3.Dot(normals[i, j], ldir);
            //            //vco = new Color(red, red, red);

            //        }
            //        //float red = Vector3.Dot(normals[i, j], ldir);
            //        //vco = new Color(red, red, red);

            //        //vco = new Color(normals[i, j].X * 100, normals[i, j].Y * 100, normals[i, j].Z * 100);

            //        //cverts[i * gDim * 6 + j * 6 + 0].Color = vco;
            //        //cverts[i * gDim * 6 + j * 6 + 1].Color = vco;
            //        //cverts[i * gDim * 6 + j * 6 + 2].Color = vco;
            //        //cverts[i * gDim * 6 + j * 6 + 3].Color = vco;
            //        //cverts[i * gDim * 6 + j * 6 + 4].Color = vco;
            //        //cverts[i * gDim * 6 + j * 6 + 5].Color = vco;
            //    }
            //}
            //// Vertices assignmentbeta
            //for (int i = 0; i < gDim; i++)
            //{

            //    for (int j = 0; j < gDim; j++)
            //    {
            //        float x = (float)(i - (gDim / 2));
            //        float y = mHeights[i, j];
            //        float z = (float)(j - (gDim / 2));


            //        terrainVPNTex[i * gDim * 6 + j * 6 + 0] = new VertexPositionNormalTexture(new Vector3(x, mHeights[i, j], z),
            //                                                                                    normals[i, j],
            //                                                                                    new Vector2((float)i / (float)gDim, (float)j / (float)gDim));
            //        terrainVPNTex[i * gDim * 6 + j * 6 + 1] = new VertexPositionNormalTexture(new Vector3(x + 1, mHeights[i + 1, j], z),
            //                                                                                    normals[i + 1, j],
            //                                                                                    new Vector2((float)(i + 1) / (float)gDim, (float)j / (float)gDim));
            //        terrainVPNTex[i * gDim * 6 + j * 6 + 2] = new VertexPositionNormalTexture(new Vector3(x + 1, mHeights[i + 1, j + 1], z + 1),
            //                                                                                    normals[i + 1, j + 1],
            //                                                                                    new Vector2((float)(i + 1) / (float)gDim, (float)(j + 1) / (float)gDim));
            //        terrainVPNTex[i * gDim * 6 + j * 6 + 3] = new VertexPositionNormalTexture(new Vector3(x, mHeights[i, j], z),
            //                                                                                    normals[i, j],
            //                                                                                    new Vector2((float)i / (float)gDim, (float)j / (float)gDim));
            //        terrainVPNTex[i * gDim * 6 + j * 6 + 4] = new VertexPositionNormalTexture(new Vector3(x, mHeights[i, j + 1], z + 1),
            //                                                                                    normals[i, j + 1],
            //                                                                                    new Vector2((float)i / (float)gDim, (float)(j + 1) / (float)gDim));
            //        terrainVPNTex[i * gDim * 6 + j * 6 + 5] = new VertexPositionNormalTexture(new Vector3(x + 1, mHeights[i + 1, j + 1], z + 1),
            //                                                                                    normals[i + 1, j + 1],
            //                                                                                    new Vector2((float)(i + 1) / (float)gDim, (float)(j + 1) / (float)gDim));


            //    }
            //}
            //// Assign Colors
            //for (int i = 0; i < gDim; i++)
            //{
            //    for (int j = 0; j < gDim; j++)
            //    {
            //        Color vco = new Color(normals[i, j]);
            //        terrainVPColor[i * gDim * 6 + j * 6 + 0].Color = vco;
            //        vco = new Color(normals[i + 1, j]);
            //        terrainVPColor[i * gDim * 6 + j * 6 + 1].Color = vco;
            //        vco = new Color(normals[i + 1, j + 1]);
            //        terrainVPColor[i * gDim * 6 + j * 6 + 2].Color = vco;
            //        vco = new Color(normals[i, j]);
            //        terrainVPColor[i * gDim * 6 + j * 6 + 3].Color = vco;
            //        vco = new Color(normals[i, j + 1]);
            //        terrainVPColor[i * gDim * 6 + j * 6 + 4].Color = vco;
            //        vco = new Color(normals[i + 1, j + 1]);
            //        terrainVPColor[i * gDim * 6 + j * 6 + 5].Color = vco;
            //    }
            //}// Old Vertex Assignments

            // Set cullmode to none
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rs;

            CamXangle = 0;
            CamYangle = MathHelper.Pi;
            dmx = dmy = 0;
            mScale = 0.01f;

            //keyX = 0f;
            //keyY = 0f;
            //keyZ = 0f;
            moveDir = Vector3.Zero;


            base.LoadContent();
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            base.UnloadContent();
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            prevKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();
            prevMouseState = mouseState;
            mouseState = Mouse.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();

            switch (camMode)
            {
                case 0:
                    //Look Control
                    freeCam();
                    camera.Position += 100f * moveDir * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    MouseInput();
                    camera.setView();
                    break;
                case 1:
                    //Camera on Tank
                    tankCam();
                    camera.setView();
                    break;
            }
            KeyInput();
            tankCamDist *= 1f - 0.05f * ((float)mouseState.ScrollWheelValue - (float)prevMouseState.ScrollWheelValue) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            tankCamDist = Math.Clamp(tankCamDist, 10f, 100f);

            if (GameOn)
            {
                //Game Stuff
                Timer -= 1f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                fScore += 1f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                productionTime += 1f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                mineralConsume += 1f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Score = (int)fScore;
                //Ammunition += 1; // testing
                //Minerals += 50f * (float)gameTime.ElapsedGameTime.TotalSeconds; // Testing passive minerals
                float mineralsPerSecond = -5f;
                if (mineralConsume > 1f && Minerals + mineralsPerSecond > 0) // upkeep/income
                {
                    Minerals += mineralsPerSecond;
                    if (mineralsPerSecond < 0)
                        usedMinerals -= mineralsPerSecond;
                    mineralConsume = 0f;
                }
                if (Minerals > shotCost && productionTime > 0.6f && Ammunition < 10) // ammo generation
                {
                    Minerals -= shotCost;
                    usedMinerals += shotCost;
                    Ammunition += 1;
                    productionTime = 0;
                }
                int mAmm = (int)Math.Floor(Minerals / shotCost);
                if ((Ammunition + LiveShots + mAmm <= 0 ) || Timer <= 0f) // lose condition
                {
                    //gameOver();
                    GameOn = false;
                }

                if (tank.isTouchingGround() && tank.Position.Y > MaxAlt * terrainScale * 1.5f) // checks win condition and starts timer
                {
                    altAccumulator += 1f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (altAccumulator > 10f)
                    {
                        GameWin = true;
                        GameOn = false;
                    }
                }
                else // reset wincondition
                    altAccumulator = 0f;
            }
            else
            {
                if (keyboardState.IsKeyDown(Keys.Enter) && !GameWin) // resume after loss
                {
                    Timer = 120f;
                    GameOn = true;
                    Ammunition = 10;
                }
            }
            
            base.Update(gameTime);
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(fogColor);
            
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;
            GraphicsDevice.VertexSamplerStates[0] = SamplerState.AnisotropicWrap;

            if (maketriangle)
            {
                GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
                vb = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), verts.Length, BufferUsage.None);

                vb.SetData(verts.ToArray());
                GraphicsDevice.SetVertexBuffer(vb);

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    GraphicsDevice.DrawUserPrimitives<VertexPositionColor>
                        (PrimitiveType.TriangleStrip, verts, 0, 1);
                    //pass.End();

                }
                effect = new BasicEffect(GraphicsDevice);
                effect.World = worldRot * worldTrans;
                effect.View = camera.View;
                effect.Projection = camera.Projection;
                effect.VertexColorEnabled = true;
                // effect.Texture = texture;
                // effect.TextureEnabled = true;
                // effect.Begin();
                // effect.End();
            }
            if (makespheres)
            {
                ceffect.World = Matrix.Identity;
                ceffect.View = camera.View;
                ceffect.Projection = camera.Projection;

                foreach (EffectPass pass in ceffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    GraphicsDevice.DrawUserPrimitives<VertexPositionColor>
                        (PrimitiveType.TriangleList, terrainVPColor, 0, gDim * gDim * 2);
                }
            }

            //vb = new VertexBuffer(GraphicsDevice, typeof(VertexPositionNormalTexture), gDim * gDim * 6, BufferUsage.None);
            //vb.SetData(terrainVPNTex.ToArray());
            //GraphicsDevice.SetVertexBuffer(vb);

            this.shaderAssignment(Matrix.Identity, groundTexture); // groundTexture
            foreach (EffectPass pass in worldShader.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, terrainVPNTex, 0, gDim * gDim * 2);
                //GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, terrainVPNTex, gDim * gDim * 6 * 0 / 4, gDim * gDim * 2 / 4);
                //GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, terrainVPNTex, gDim * gDim * 6 * 1 / 4 - 1, gDim * gDim * 2 / 4);
                //GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, terrainVPNTex, gDim * gDim * 6 * 2 / 4, gDim * gDim * 2 / 4);
                //GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, terrainVPNTex, gDim * gDim * 6 * 3 / 4 - 1, gDim * gDim * 2 / 4);
            }

            base.Draw(gameTime);

            Vector2 uiAnchor = new Vector2(screenWidth * 1 / 10, screenHeight * 1 / 10);
            Vector2 newLine = new Vector2(0, 24);
            int tMin = (int)(Timer / 60);
            int tSec = (int)Timer % 60;

            spriteBatch.Begin();
            if (GameOn)
            {
                spriteBatch.DrawString(uiFont, "Time Remaining: " + tMin + ":" + tSec,  uiAnchor + newLine * -1f, Color.White);
                spriteBatch.DrawString(uiFont, "Score: " + Score,                       uiAnchor + newLine * 0f, Color.White);
                spriteBatch.DrawString(uiFont, "Minerals: " + (float)((int)(Minerals * 10f)) / 10f, uiAnchor + newLine * 1f, Color.White);
                spriteBatch.DrawString(uiFont, "Ammunition: " + Ammunition,             uiAnchor + newLine * 2f, Color.White);
                spriteBatch.DrawString(uiFont, "Firing Mode: " + gunMode,               uiAnchor + newLine * 3f, Color.White);
                spriteBatch.DrawString(uiFont, "Tank Height: " + tank.Position.Y,       uiAnchor + newLine * 4f, Color.White);
                spriteBatch.DrawString(uiFont, "Goal Height: " + (MaxAlt * terrainScale * 1.5f), uiAnchor + newLine * 5f, Color.White);

                //spriteBatch.DrawString(uiFont, "heights Sum: " + heightSum + " x1000", uiAnchor + newLine * 6f, Color.White);
                //spriteBatch.DrawString(uiFont, "Diff Sum: " + (heightSum - initialHSum) * 1000f, uiAnchor + newLine * 7f, Color.White);

                //spriteBatch.DrawString(uiFont, "Highest Height: " + getHeighestPoint(), uiAnchor + newLine * 7f, Color.White);
            }
            else if (GameWin)
            {
                spriteBatch.DrawString(uiFont, "GAME WIN : YOU HAVE BESTED GOD",        4f * uiAnchor + newLine * 0f, Color.Blue);
                spriteBatch.DrawString(uiFont, "Press Escape to Quit",                  4f * uiAnchor + newLine * 2f, Color.Blue);

                spriteBatch.DrawString(uiFont, "Score: " + Score,                       4f * uiAnchor + newLine * 4f, Color.White);
                spriteBatch.DrawString(uiFont, "Time Remaining: " + tMin + ":" + tSec,  4f * uiAnchor + newLine * 5f, Color.White);
                spriteBatch.DrawString(uiFont, "Minerals: " + (float)((int)(Minerals * 10f)) / 10f, 4f * uiAnchor + newLine * 6f, Color.White);
                spriteBatch.DrawString(uiFont, "Ammunition: " + Ammunition,             4f * uiAnchor + newLine * 7f, Color.White);
            }
            else
            {
                spriteBatch.DrawString(uiFont, "GAME OVER",                             4f * uiAnchor + newLine * 0f, Color.Blue);
                spriteBatch.DrawString(uiFont, "Press Enter to resume",                 4f * uiAnchor + newLine * 2f, Color.Blue);

                spriteBatch.DrawString(uiFont, "Score: " + Score,                       4f * uiAnchor + newLine * 4f, Color.White);
                spriteBatch.DrawString(uiFont, "Time Remaining: " + tMin + ":" + tSec,  4f * uiAnchor + newLine * 5f, Color.White);
                spriteBatch.DrawString(uiFont, "Minerals: " + (float)((int)(Minerals * 10f)) / 10f, 4f * uiAnchor + newLine * 6f, Color.White);
                spriteBatch.DrawString(uiFont, "Ammunition: " + Ammunition,             4f * uiAnchor + newLine * 7f, Color.White);
            }
            spriteBatch.End();
        }
        float getHeightSum()
        {
            float r = 0f;
            foreach(float f in mHeights)
            {
                r += f * 1f;
            }
            return r;
        }
        float getHeighestPoint()
        {
            float r = 0f;
            foreach (float f in mHeights)
            {
                r = f > r ? f : r;
            }
            return r;
        }
        void MouseInput()
        {

            //if (mouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton != ButtonState.Pressed)
            {

            } //empty

            if (mouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Pressed)
            {
                dmx = mouseState.X - mx;
                dmy = mouseState.Y - my;
                CamXangle -= dmy * mScale;
                CamYangle -= dmx * mScale;
                if (CamXangle > 1.5f)
                    CamXangle = 1.5f;
                if (CamXangle < -1.5f)
                    CamXangle = -1.5f;
                // Debug.WriteLine(" x " + CamXangle + "    y " + CamYangle);

                camera.Direction = new Vector3(
                    (float)Math.Cos(CamXangle) * (float)Math.Sin(CamYangle),
                    (float)Math.Sin(CamXangle),
                    (float)Math.Cos(CamXangle) * (float)Math.Cos(CamYangle));
                mx = mouseState.X;
                my = mouseState.Y;
            }
            if (mouseState.RightButton == ButtonState.Pressed)
            {
                //if (prevMouseState.RightButton == ButtonState.Pressed)
                //{
                //    dmx = mouseState.X - mx;
                //    dmy = mouseState.Y - my;
                //    CamXangle -= dmy * mScale;
                //    CamYangle -= dmx * mScale;
                //    if (CamXangle > 1.5f)
                //        CamXangle = 1.5f;
                //    if (CamXangle < -1.5f)
                //        CamXangle = -1.5f;
                //    // Debug.WriteLine(" x " + CamXangle + "    y " + CamYangle);

                //    camera.Direction = new Vector3(
                //        (float)Math.Cos(CamXangle) * (float)Math.Sin(CamYangle),
                //        (float)Math.Sin(CamXangle),
                //        (float)Math.Cos(CamXangle) * (float)Math.Cos(CamYangle));

                //}
                //mx = mouseState.X;
                //my = mouseState.Y;

            }
            // Mouse.SetPosition(Window.ClientBounds.X / 2, Window.ClientBounds.Y / 2);


        }
        void KeyInput()
        {
            // Camera Mode Switch
            if (keyboardState.IsKeyDown(Keys.Y) && !prevKeyboardState.IsKeyDown(Keys.Y))
            {
                if (camMode == 0)
                    camMode = 1;
                else
                    camMode = 0;
            }
            // Lens Switch
            if (keyboardState.IsKeyDown(Keys.N) && !prevKeyboardState.IsKeyDown(Keys.N))
            {
                camera.toggleLens();
                if (fishEye == 1)
                {
                    //fishEye = 0;
                    ////camera.FieldOfView = MathHelper.PiOver4;
                    //camera.renewCameraProj();
                    worldShader = shaderLightNormal;
                    //worldShader = shaderLightFisheye;
                }
                else
                {
                    //fishEye = 1;
                    ////camera.FieldOfView = MathHelper.PiOver4; //MathHelper.Pi;
                    //camera.renewCameraProj();
                    worldShader = shaderLightFisheye;
                }
            }
            // FOV controls
            if (keyboardState.IsKeyDown(Keys.OemOpenBrackets) && !prevKeyboardState.IsKeyDown(Keys.OemOpenBrackets)) // far/narrow
            {
                camera.FocalLength = Math.Clamp(camera.FocalLength * 1.15f, 0.2f, 5f);
                camera.renewCameraProj();
            }
            if (keyboardState.IsKeyDown(Keys.OemCloseBrackets) && !prevKeyboardState.IsKeyDown(Keys.OemCloseBrackets)) // close/wide
            {
                camera.FocalLength = Math.Clamp(camera.FocalLength / 1.15f, 0.2f, 5f);
                camera.renewCameraProj();
            }
            if (keyboardState.IsKeyDown(Keys.OemMinus) && !prevKeyboardState.IsKeyDown(Keys.OemMinus)) // wide
            {
                camera.FieldOfView = Math.Clamp(camera.FieldOfView * 1.15f, MathHelper.Pi / 8, MathHelper.Pi / 2);
                camera.renewCameraProj();
            }
            if (keyboardState.IsKeyDown(Keys.OemPlus) && !prevKeyboardState.IsKeyDown(Keys.OemPlus)) // narrow
            {
                camera.FieldOfView = Math.Clamp(camera.FieldOfView / 1.15f, MathHelper.Pi / 8, MathHelper.Pi / 2);
                camera.renewCameraProj();
            }
            //if (keyboardState.IsKeyDown(Keys.OemSemicolon) && !prevKeyboardState.IsKeyDown(Keys.OemSemicolon))
            //{
            //    camera.FisheyFactor = Math.Clamp(camera.FisheyFactor - 0.5f, -1, 1);
            //    camera.renewCameraProj();
            //}
            //if (keyboardState.IsKeyDown(Keys.OemQuotes) && !prevKeyboardState.IsKeyDown(Keys.OemQuotes))
            //{
            //    camera.FisheyFactor = Math.Clamp(camera.FisheyFactor + 0.5f, -1, 1);
            //    camera.renewCameraProj();
            //}
            if (fishEye == 1)
            {
                if (keyboardState.IsKeyDown(Keys.OemSemicolon) && !prevKeyboardState.IsKeyDown(Keys.OemSemicolon))
                {
                    camera.FisheyFactor = Math.Clamp(camera.FisheyFactor - 0.5f, -1, 1);
                    camera.renewCameraProj();
                }
                if (keyboardState.IsKeyDown(Keys.OemQuotes) && !prevKeyboardState.IsKeyDown(Keys.OemQuotes))
                {
                    camera.FisheyFactor = Math.Clamp(camera.FisheyFactor + 0.5f, -1, 1);
                    camera.renewCameraProj();
                }
            }
            if (keyboardState.IsKeyDown(Keys.M) && !prevKeyboardState.IsKeyDown(Keys.M))
            {
                camera.resetVals();
            }

        }
        void freeCam()
        {
            moveDir = Vector3.Zero;
            if (keyboardState.IsKeyDown(Keys.Left))
                moveDir += new Vector3(camera.Direction.Z, camera.Direction.Y, -camera.Direction.X) * new Vector3(1, 0, 1);
            if (keyboardState.IsKeyDown(Keys.Right))
                moveDir -= new Vector3(camera.Direction.Z, camera.Direction.Y, -camera.Direction.X) * new Vector3(1, 0, 1);
            if (keyboardState.IsKeyDown(Keys.Up))
                moveDir += camera.Direction * new Vector3(1, 0, 1);
            if (keyboardState.IsKeyDown(Keys.Down))
                moveDir -= camera.Direction * new Vector3(1, 0, 1);
            if (keyboardState.IsKeyDown(Keys.U))
                moveDir += camera.Up * new Vector3(0, 1, 0);
            if (keyboardState.IsKeyDown(Keys.J))
                moveDir -= camera.Up * new Vector3(0, 1, 0);
            if (moveDir.Length() > 0f)
                moveDir.Normalize();

        }
        void tankCam()
        {
            float v = -5f * tank.myBarrel.vRotation;
            camera.Position = new Vector3(tank.getPosition().X - tank.myTurret.getDirection().X * tankCamDist,
                                            tank.getPosition().Y + tankCamDist * 0.5f,
                                            tank.getPosition().Z - tank.myTurret.getDirection().Z * tankCamDist);

            float trl = -4f * (tankCamDist - 4f) / 96f + 4f;
            tankViewPoint = tank.getPosition() + new Vector3(0, trl + v, 0);
            camera.Direction = Vector3.Normalize(tankViewPoint - camera.Position);
            //float y = tank.myTurret.yRotation;
            //float v = tank.myBarrel.vRotation;
            //Vector3 cr = Vector3.Cross(tank.getDirection(), tank.rotUp);
            //cr.Normalize();
            //cr *= -1f * y;

            //camera.Position = new Vector3(  tank.getPosition().X - (tank.getDirection().X * (1 + cr.X) + cr.X) * tankCamDist,
            //                                tank.getPosition().Y + tankCamDist * 0.5f,
            //                                tank.getPosition().Z - (tank.getDirection().Z * (1 + cr.Z) + cr.Z) * tankCamDist);

            //float trl = -4f * (tankCamDist-4f) / 96f + 4f;
            //tankViewPoint = tank.getPosition() + new Vector3(4f * cr.X, trl + v * 2f, 4f * cr.Z);
            //camera.Direction = Vector3.Normalize(tankViewPoint - camera.Position);
        }

       

        public void otherCalcHeights(Texture2D heightMap)
        {
            //heightMap = Content.Load<Texture2D>(@"Textures\MtnMap");
            // can be .bmp, .dds, .dib, .hdr, .jpg, .pfm, .png, .ppm, .tga

            int terrainWidth = heightMap.Width;
            int terrainHeight = heightMap.Height;

            Color[] heightMapColors = new Color[terrainWidth * terrainHeight];
            heightMap.GetData(heightMapColors);

            for (int j = 0; j <= gDim; j++) 
            {
                for (int i = 0; i <= gDim; i++)
                {
                    if ( i == 0 || j == 0)
                    {
                        mHeights[i, j] = MaxAlt * terrainScale * 1.1f;
                    }
                    else if ((i < terrainWidth) && (j < terrainHeight))
                    {
                        int index = (int)(i + j * terrainWidth);
                        mHeights[i, j] = (heightMapColors[index].R / 255f) * MaxAlt * terrainScale + 0.0f;
                    }
                    else
                    {
                        mHeights[i, j] = MaxAlt * terrainScale * 1.1f;
                    }
                }
            }
        }
        public void setCamera(BasicEffect effect)
        {
            effect.View = camera.View;
            effect.Projection = camera.Projection;
        }
        public float getElevation(Vector3 position)
        {
            float r;
            float x = position.X / terrainScale;
            float z = position.Z / terrainScale;
            int i = (int)Math.Floor(x + (gDim / 2));
            int j = (int)Math.Floor(z + (gDim / 2));
            float xr = x - (float)Math.Floor(x);
            float zr = z - (float)Math.Floor(z);

            try
            {
                r = floatLerp(
                        floatLerp(mHeights[i, j], mHeights[i + 1, j], xr),
                        floatLerp(mHeights[i, j + 1], mHeights[i + 1, j + 1], xr),
                        zr);

                return r;
            }
            catch (IndexOutOfRangeException e)
            {
                Console.Error.WriteLine(e.ToString());
                return 0f;
            }
        }
        public Vector3 getNormal(Vector3 position)
        {
            Vector3 r;
            float x = position.X / terrainScale;
            float z = position.Z / terrainScale;
            int i = (int)Math.Floor(x + (gDim / 2));
            int j = (int)Math.Floor(z + (gDim / 2));
            float xr = x - (float)Math.Floor(x);
            float zr = z - (float)Math.Floor(z);

            try
            {
                r = vectorLerp(
                        vectorLerp(normals[i, j], normals[i + 1, j], xr),
                        vectorLerp(normals[i, j + 1], normals[i + 1, j + 1], xr),
                        zr);

                return r;
            }
            catch (IndexOutOfRangeException e)
            {
                Console.Error.WriteLine(e.ToString());
                return Vector3.Up;
            }
        }
        public void terrainImpactDeform(Vector3 position, float radius, float power, int type) // TODO
        {
            float hsBefore = getHeightSum();
            Vector3 pv;
            float rv;
            //radius /= terrainScale;
            //power /= terrainScale;
            //float x = position.X; //(int)Math.Floor(position.position.X / terrainScale) + gDim / 2; //position.X / terrainScale;
            //float z = position.Z; //(int)Math.Floor(position.Z / terrainScale) + gDim / 2; //position.Z / terrainScale;
            int a = (int)((position.X / terrainScale) - radius * 1.5f) + gDim / 2;
            int b = (int)((position.Z / terrainScale) - radius * 1.5f) + gDim / 2;
            int c = (int)((position.X / terrainScale) + radius * 1.5f) + gDim / 2;
            int d = (int)((position.Z / terrainScale) + radius * 1.5f) + gDim / 2;
            int ox = (int)(position.X / terrainScale) + gDim / 2;
            int oz = (int)(position.Z / terrainScale) + gDim / 2;
            
            for (int i = (a < 0 ? 0 : a); i < (c > gDim ? gDim : c); i++)
            {
                for (int j = (b < 0 ? 0 : b); j < (d > gDim ? gDim : d); j++)
                {
                    try
                    {
                        if (i != 0 && j != 0 && i != gDim && j != gDim)
                        {
                            pv = new Vector3((float)(i - (gDim / 2)) * terrainScale, position.Y, (float)(j - (gDim / 2)) * terrainScale);
                            rv = (position - pv).Length();
                            switch (type)
                            {
                                case 0:  // Crater Mode
                                    if (rv <= radius * 0.5f)
                                    {
                                        mHeights[i, j] -= power * (float)Math.Sqrt(radius - rv);
                                    }
                                    else if (rv <= radius * 0.75f)
                                    {
                                        mHeights[i, j] += 0.2f * power * (rv - radius);
                                    }
                                    else if (rv <= radius * 1f)
                                    {
                                        mHeights[i, j] += 0.2f * power * (radius * 2f - rv);
                                    }
                                    break;
                                case 1: // Excavate Mode
                                    if (rv <= radius * 1f)
                                    {
                                        mHeights[i, j] -= power * (float)Math.Sqrt(radius * 1f - rv);
                                    }
                                    break;
                                case 2: // Flatten Mode
                                    if (rv <= radius * 1f)
                                    {
                                        mHeights[i, j] = mHeights[ox, oz] + (1f/(1f + power)) * (mHeights[ox, oz] - mHeights[i, j]);
                                    }
                                    break;
                                case 3: // Build Mode
                                    if (rv <= radius * 1f)
                                    {
                                        mHeights[i, j] += power * (float)Math.Sqrt(radius * 1f - rv);
                                    }
                                    break;
                            }
                            mHeights[i, j] = mHeights[i, j] > 0f ? mHeights[i, j] : 0f; // minimum altitude. Limits available minerals in game.

                        }
                    }
                    catch (IndexOutOfRangeException ie)
                    {
                        Console.Error.WriteLine(ie.ToString());
                    }
                    
                }
            }
            float multi = tank.isTouchingGround() ? 1f : 2f;
            heightSum = getHeightSum();
            if (hsBefore > heightSum)
            {
                Minerals += (hsBefore - heightSum) * 0.5f;
                fScore += (hsBefore - heightSum) * 0.1f * multi;
            }
            float high = getHeighestPoint();
            if (high > currentHeighest)
            {
                fScore += (high - currentHeighest) * 100f * multi;
                currentHeighest = high;
            }

            try
            {
                recalculateVertices(
                (int)(position.X / terrainScale + radius * 4f) + gDim / 2,
                (int)(position.Z / terrainScale + radius * 4f) + gDim / 2,
                (int)(position.X / terrainScale - radius * 4f) + gDim / 2,
                (int)(position.Z / terrainScale - radius * 4f) + gDim / 2); // ((int)(position.X - radius - 1) + gDim / 2, (int)(position.X + radius + 1) + gDim / 2, (int)(position.Z - radius - 1) + gDim / 2, (int)(position.Z + radius + 1) + gDim / 2)

            }
            catch (IndexOutOfRangeException ie)
            {
                Console.Error.WriteLine(ie.ToString());
                recalculateVertices(gDim, gDim);
            }

        }
        void recalculateVertices(int o, int p, int a = 0, int b = 0) // int i, int o, int j, int p
        {
            a = a < 0 ? 0 : a;
            o = o > gDim ? gDim : o;
            b = b < 0 ? 0 : b;
            p = p > gDim ? gDim : p;
            // Vertices assignment COLOR/NORMAL
            for (int i = a; i < o; i++) // Is this legal?????
            {
                for (int j = b; j < p; j++)
                {
                    normals[i, j] = Vector3.Up;

                    float x = (float)(i - (gDim / 2)) * terrainScale;
                    float y = mHeights[i, j];
                    float z = (float)(j - (gDim / 2)) * terrainScale;
                    


                    Color vco;
                    vco = Color.Pink;

                    terrainVPColor[i * gDim * 6 + j * 6 + 0] = new VertexPositionColor(new Vector3(x    , mHeights[i, j]        , z)    , vco);
                    terrainVPColor[i * gDim * 6 + j * 6 + 1] = new VertexPositionColor(new Vector3(x + terrainScale, mHeights[i + 1, j]    , z)    , vco);
                    terrainVPColor[i * gDim * 6 + j * 6 + 2] = new VertexPositionColor(new Vector3(x + terrainScale, mHeights[i + 1, j + 1], z + terrainScale), vco);
                    terrainVPColor[i * gDim * 6 + j * 6 + 3] = new VertexPositionColor(new Vector3(x    , mHeights[i, j]        , z)    , vco);
                    terrainVPColor[i * gDim * 6 + j * 6 + 4] = new VertexPositionColor(new Vector3(x    , mHeights[i, j + 1]    , z + terrainScale), vco);
                    terrainVPColor[i * gDim * 6 + j * 6 + 5] = new VertexPositionColor(new Vector3(x + terrainScale, mHeights[i + 1, j + 1], z + terrainScale), vco);


                    // Quick Normals
                    float k;
                    Vector3 nNorth = new Vector3(0, 1, 0);
                    Vector3 nSouth = new Vector3(0, 1, 0);
                    Vector3 nEast = new Vector3(0, 1, 0);
                    Vector3 nWest = new Vector3(0, 1, 0);
                    if (j + 1 <= gDim * 2 + 1)
                    {
                        k = mHeights[i, j] - mHeights[i, j + 1];
                        nNorth = new Vector3(0, 1, k);
                    }
                    if (j - 1 >= 0)
                    {
                        k = mHeights[i, j] - mHeights[i, j - 1];
                        nSouth = new Vector3(0, 1, -k);
                    }
                    if (i + 1 <= gDim * 2 + 1)
                    {
                        k = mHeights[i, j] - mHeights[i + 1, j];
                        nEast = new Vector3(k, 1, 0);
                    }
                    if (i - 1 >= 0)
                    {
                        k = mHeights[i, j] - mHeights[i - 1, j];
                        nWest = new Vector3(-k, 1, 0);
                    }

                    Vector3 nOne = nNorth + nSouth + nEast + nWest;
                    nOne.Normalize();
                    normals[i, j] = nOne;
                }
            }
            // Assign Colors based on normals
            for (int i = a; i < o; i++)
            {
                for (int j = b; j < p; j++)
                {
                    Color vco;
                    vco = new Color(normals[i, j]);
                    terrainVPColor[i * gDim * 6 + j * 6 + 0].Color = vco;
                    vco = new Color(normals[i + 1, j]);
                    terrainVPColor[i * gDim * 6 + j * 6 + 1].Color = vco;
                    vco = new Color(normals[i + 1, j + 1]);
                    terrainVPColor[i * gDim * 6 + j * 6 + 2].Color = vco;
                    vco = new Color(normals[i, j]);
                    terrainVPColor[i * gDim * 6 + j * 6 + 3].Color = vco;
                    vco = new Color(normals[i, j + 1]);
                    terrainVPColor[i * gDim * 6 + j * 6 + 4].Color = vco;
                    vco = new Color(normals[i + 1, j + 1]);
                    terrainVPColor[i * gDim * 6 + j * 6 + 5].Color = vco;
                }
            }
            // Terrain Texture creation
            for (int i = a; i < o; i++)
            {
                for (int j = b; j < p; j++)
                {
                    float x = (float)(i - (gDim / 2)) * terrainScale;
                    float y = mHeights[i, j];
                    float z = (float)(j - (gDim / 2)) * terrainScale;


                    terrainVPNTex[i * gDim * 6 + j * 6 + 0] = new VertexPositionNormalTexture(new Vector3(x, mHeights[i, j], z),
                                                                                                normals[i, j],
                                                                                                new Vector2((float)i / (float)gDim, (float)j / (float)gDim));
                    terrainVPNTex[i * gDim * 6 + j * 6 + 1] = new VertexPositionNormalTexture(new Vector3(x + terrainScale, mHeights[i + 1, j], z),
                                                                                                normals[i + 1, j],
                                                                                                new Vector2((float)(i + 1) / (float)gDim, (float)j / (float)gDim));
                    terrainVPNTex[i * gDim * 6 + j * 6 + 2] = new VertexPositionNormalTexture(new Vector3(x + terrainScale, mHeights[i + 1, j + 1], z + terrainScale),
                                                                                                normals[i + 1, j + 1],
                                                                                                new Vector2((float)(i + 1) / (float)gDim, (float)(j + 1) / (float)gDim));
                    terrainVPNTex[i * gDim * 6 + j * 6 + 3] = new VertexPositionNormalTexture(new Vector3(x, mHeights[i, j], z),
                                                                                                normals[i, j],
                                                                                                new Vector2((float)i / (float)gDim, (float)j / (float)gDim));
                    terrainVPNTex[i * gDim * 6 + j * 6 + 4] = new VertexPositionNormalTexture(new Vector3(x, mHeights[i, j + 1], z + terrainScale),
                                                                                                normals[i, j + 1],
                                                                                                new Vector2((float)i / (float)gDim, (float)(j + 1) / (float)gDim));
                    terrainVPNTex[i * gDim * 6 + j * 6 + 5] = new VertexPositionNormalTexture(new Vector3(x + terrainScale, mHeights[i + 1, j + 1], z + terrainScale),
                                                                                                normals[i + 1, j + 1],
                                                                                                new Vector2((float)(i + 1) / (float)gDim, (float)(j + 1) / (float)gDim));


                }
            }
        }
        public void shaderAssignment(Matrix myWorld, Texture2D myTex)
        {
            if (worldShader != null)
            {
                // arguments
                try{
                    if (myWorld != null)
                    {
                        worldShader.Parameters["World"].SetValue(myWorld);
                        worldShader.Parameters["WorldInverseTranspose"].SetValue(Matrix.Invert(Matrix.Transpose(myWorld)));
                    }
                    if (myTex != null)
                    {
                        worldShader.Parameters["ModelTexture"].SetValue(myTex);
                    }
                }
                catch (NullReferenceException nre)
                {
                    Console.WriteLine("NullException" + nre.ToString());
                }
                // Generics
                shaderGeneric(worldShader);
            }
            else
            {
                Console.WriteLine("Effect Null");
            }
        }
        void shaderGeneric(Effect myEffect)
        {
            // Camera
            myEffect.Parameters["View"].SetValue(camera.View);
            myEffect.Parameters["Projection"].SetValue(camera.Projection);
            myEffect.Parameters["CameraPosition"].SetValue(camera.getCamPos4());
            myEffect.Parameters["CameraView"].SetValue(camera.getViewVector4());
            myEffect.Parameters["CameraDir"].SetValue(camera.getCamDir4());
            myEffect.Parameters["FishFactor"].SetValue(CustomNormalize(camera.FisheyFactor, -1, 1));

            // Lights
            myEffect.Parameters["LightPos"].SetValue(light1.getLightPos4());
            myEffect.Parameters["LightDirection"].SetValue(light1.getLightDir4());
            myEffect.Parameters["LightColor"].SetValue(light1.getLightColor4());
            // Ambient
            myEffect.Parameters["AmbientColor"].SetValue(light1.Ambient.Color4);
            myEffect.Parameters["AmbientIntensity"].SetValue(light1.Ambient.Intensity);
            // Diffuse
            myEffect.Parameters["DiffuseColor"].SetValue(light1.Diffuse.Color4);
            myEffect.Parameters["DiffuseIntensity"].SetValue(light1.Diffuse.Intensity);
            // Specular
            myEffect.Parameters["Shininess"].SetValue(light1.Specular.Shine);
            myEffect.Parameters["SpecularColor"].SetValue(light1.Specular.Color4);
            myEffect.Parameters["SpecularIntensity"].SetValue(light1.Specular.Intensity);
            // Fog
            myEffect.Parameters["FogDistance"].SetValue(light1.Fog.Distance);
            myEffect.Parameters["FogDensity"].SetValue(light1.Fog.Density);
            myEffect.Parameters["FogColor"].SetValue(light1.Fog.Color4);
        }
        public float floatLerp(float zero, float one, float scalar)
        {
            float r = scalar * (one - zero) + zero;
            return r;
        }
        public Vector3 vectorLerp(Vector3 zero, Vector3 one, float scalar)
        {
            Vector3 r = scalar * (one - zero) + zero;
            return r;
        }
        Texture2D textureTiler(Texture2D texIn, float width = 1, float height = 1)
        {
            if(texIn == null)
            {
                Console.Error.WriteLine("Texture Null");
            }
            int buffsize = 4096;
            if((texIn.Width * width) * (texIn.Height * height) > buffsize * buffsize)
            {
                Console.Error.WriteLine("Texture Generator Size Overload");
                width = (float)buffsize / (float)texIn.Width;
                height = (float)buffsize / (float)texIn.Height;
            }
            Texture2D texOut = new Texture2D(GraphicsDevice, (int)(texIn.Width * width), (int)(texIn.Height * height));
            Color[] colorsIn = new Color[texIn.Width * texIn.Height + 0];
            texIn.GetData(colorsIn);
            Color[] colorsOut = new Color[texOut.Width * texOut.Height + 0];
            //int idRow = texIn.Width - 1;
            //int idCol = texIn.Height - 1;

            for (int j = 0; j < texIn.Height; j++)
            {
                for (int i = 0; i < texIn.Width; i++)
                {
                    for (int jh = 0; jh < height; jh++) 
                    {
                        for (int iw = 0; iw < width; iw++)
                        {
                            int odex = i + iw * texIn.Width + (j + jh * texIn.Height) * texOut.Width;
                            int idex = i + j * texIn.Width;
                            if (odex < colorsOut.Length && idex < colorsIn.Length)
                            {
                                colorsOut[odex] = colorsIn[idex] * 1f;
                                //colorsOut[odex].A = (byte)1f;
                                if (colorsOut[odex].R < 1)
                                    colorsOut[odex] = Color.Purple;
                            }
                        }
                    }
                }
            }
            try
            {
                texOut.SetData<Color>(colorsOut);
            }
            catch (SharpDX.SharpDXException sdxe)
            {
                Console.Error.WriteLine(sdxe.ToString());
                //texOut = textureTiler(texOut, 0.9f, 0.9f);
            }
            return texOut;
        }
        public float CustomNormalize(float input, float min = 0, float max = 1)
        {
            try
            {
                if (min != max)
                {
                    if (min > max)
                    {
                        float n = max;
                        max = min;
                        min = n;
                    }
                    float r = input < min ? 0 : (input > max ? 1 : ((input - min) / (max - min)));
                    return r;
                }
                else
                {
                    return float.NaN;
                }
            }
            catch (DivideByZeroException de)
            {
                Console.Error.WriteLine(de.ToString());
            }
            return float.NaN;
        }
    }
}
