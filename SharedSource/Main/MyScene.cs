#region Using Statements
using System;
using System.Diagnostics;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Common.Media;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Gestures;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Sound;
#endregion

namespace Mad_Head_Puzzle
{
    public class MyScene : Scene
    {
        const int WIDTH = 50;

        public static Entity[][] slots;
        Piece[] pieces;
        public static bool[][] slotAvailable;
        public static int filledSlots;
        Entity resetButton, instructions, intro, fullscreen;
        public static Entity congratulations;
        public static Entity exit;
        SoundBank soundBank;
        public static SoundInfo congratulationsSound;

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.MyScene);

            this.InitializeSprites();

            //background music
            WaveServices.MusicPlayer.IsRepeat = true;
            WaveServices.MusicPlayer.Play(new MusicInfo(WaveContent.Assets.Audio.background_mp3));

            //sound effect init
            soundBank = new SoundBank(Assets);
            congratulationsSound = new SoundInfo(WaveContent.Assets.Audio.congratulations_wav);
            soundBank.Add(congratulationsSound);
            WaveServices.SoundPlayer.RegisterSoundBank(soundBank);
        }

        protected void InitializeSprites()
        {
            /*
             * slots init
             * no filled slots
             * all slots are available
            */
            filledSlots = 0;
            slots = new Entity[6][];
            slotAvailable = new bool[6][];
            for (int i = 0; i < 6; i++)
            {
                slots[i] = new Entity[6];
                slotAvailable[i] = new bool[6];
                for (int j = 0; j < 6; j++)
                {
                    slotAvailable[i][j] = true;
                    slots[i][j] = new Entity()
                        .AddComponent(new Transform2D()
                        {
                            Position = new Vector2((i - 3) * WIDTH, (j - 3) * WIDTH),
                            DrawOrder = 0.9f
                        })
                        .AddComponent(new Sprite(WaveContent.Assets.Images.slot_png))
                        .AddComponent(new SpriteRenderer())
                        .AddComponent(new RectangleCollider2D());
                    this.EntityManager.Add(slots[i][j]);
                }
            }

            //pieces init
            pieces = new Piece[8];
            /*
             * piece0 set up
             * pos = starting positon on screen
             * pat = pattern coordinates (from top/left to bottom/right) using vectors as points for pieces appearance
             * InitEntity - set piece texture and image
             * add entity on screen
             * set entity position on screen
            */
            Vector2 pos = new Vector2(-550, -350);
            Vector2[] pat = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 2) };
            pieces[0] = new Piece(pos, pat);
            pieces[0].InitEntity(WaveContent.Assets.Images.piece0_png);
            this.EntityManager.Add(pieces[0].entity);
            pieces[0].SetStartingPosition();
            //piece4 set up
            pos.X = 400; pos.Y = -150;
            pat = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };
            pieces[4] = new Piece(pos, pat);
            pieces[4].InitEntity(WaveContent.Assets.Images.piece4_png);
            this.EntityManager.Add(pieces[4].entity);
            pieces[4].SetStartingPosition();
            //piece1 set up
            pos.X = -550; pos.Y = 0;
            pat = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 2), new Vector2(1, 3) };
            pieces[1] = new Piece(pos, pat);
            pieces[1].InitEntity(WaveContent.Assets.Images.piece1_png);
            this.EntityManager.Add(pieces[1].entity);
            pieces[1].SetStartingPosition();
            //piece5 set up
            pos.X = -550; pos.Y = 250;
            pat = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
            pieces[5] = new Piece(pos, pat);
            pieces[5].InitEntity(WaveContent.Assets.Images.piece5_png);
            this.EntityManager.Add(pieces[5].entity);
            pieces[5].SetStartingPosition();
            //piece2 set up
            pos.X = 400; pos.Y = -350;
            pat = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), new Vector2(1, 2) };
            pieces[2] = new Piece(pos, pat);
            pieces[2].InitEntity(WaveContent.Assets.Images.piece2_png);
            this.EntityManager.Add(pieces[2].entity);
            pieces[2].SetStartingPosition();
            //piece6 set up
            pos.X = -550; pos.Y = -150;
            pat = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0), new Vector2(0, 1) };
            pieces[6] = new Piece(pos, pat);
            pieces[6].InitEntity(WaveContent.Assets.Images.piece6_png);
            this.EntityManager.Add(pieces[6].entity);
            pieces[6].SetStartingPosition();
            //piece3 set up
            pos.X = 400; pos.Y = 0;
            pat = new Vector2[] { new Vector2(2, 0), new Vector2(1, 1), new Vector2(2, 1), new Vector2(0, 2), new Vector2(1, 2), new Vector2(2, 2) };
            pieces[3] = new Piece(pos, pat);
            pieces[3].InitEntity(WaveContent.Assets.Images.piece3_png);
            this.EntityManager.Add(pieces[3].entity);
            pieces[3].SetStartingPosition();
            //piece7 set up
            pos.X = 400; pos.Y = 200;
            pat = new Vector2[] { new Vector2(2, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1) };
            pieces[7] = new Piece(pos, pat);
            pieces[7].InitEntity(WaveContent.Assets.Images.piece7_png);
            this.EntityManager.Add(pieces[7].entity);
            pieces[7].SetStartingPosition();
            for (int i = 0; i < 8; i++)
            {
                pieces[i].AddTouchEvents();
            }

            //reset button init
            resetButton = new Entity()
                        .AddComponent(new Transform2D()
                        {
                            Position = new Vector2(-225, 180),
                            DrawOrder = 0.9f
                        })
                        .AddComponent(new Sprite(WaveContent.Assets.Images.resetbutton_png))
                        .AddComponent(new SpriteRenderer())
                        .AddComponent(new RectangleCollider2D())
                        .AddComponent(new TouchGestures());
            //reset button pressed
            resetButton.FindComponent<TouchGestures>().TouchTap += (s, e) =>
            {
                ResetGame();
            };
            this.EntityManager.Add(resetButton);

            //fullscreen button init
            fullscreen = new Entity()
                        .AddComponent(new Transform2D()
                        {
                            Position = new Vector2(-20, 180),
                            DrawOrder = 0.9f
                        })
                        .AddComponent(new Sprite(WaveContent.Assets.Images.fullscreenoff_png))
                        .AddComponent(new SpriteRenderer())
                        .AddComponent(new RectangleCollider2D())
                        .AddComponent(new TouchGestures());
            //switch fullscreen
            fullscreen.FindComponent<TouchGestures>().TouchTap += (s, e) =>
            {
                Program.game.FullScreen = !Program.game.FullScreen;
                if (Program.game.FullScreen) fullscreen.FindComponent<Sprite>().TexturePath = WaveContent.Assets.Images.fullscreen_png;
                else fullscreen.FindComponent<Sprite>().TexturePath = WaveContent.Assets.Images.fullscreenoff_png;
            };
            this.EntityManager.Add(fullscreen);

            //congratulations message
            congratulations = new Entity()
                        .AddComponent(new Transform2D()
                        {
                            Origin = new Vector2(0.5f, 0.5f),
                            Position = new Vector2(0, 0),
                            DrawOrder = 0,
                        })
                        .AddComponent(new Sprite(WaveContent.Assets.Images.congratulations_png))
                        .AddComponent(new SpriteRenderer())
                        .AddComponent(new RectangleCollider2D())
                        .AddComponent(new TouchGestures());
            //not visible until all slots are filled
            congratulations.IsVisible = false;
            //after it is shown, reset game and hide it
            congratulations.FindComponent<TouchGestures>().TouchTap += (s, e) =>
            {
                ResetGame();
                congratulations.IsVisible = false;
            };
            this.EntityManager.Add(congratulations);

            //instructions screen
            intro = new Entity()
                        .AddComponent(new Transform2D()
                        {
                            Origin = new Vector2(0.5f, 0.5f),
                            Position = new Vector2(0, 0),
                            DrawOrder = 0,
                        })
                        .AddComponent(new Sprite(WaveContent.Assets.Images.intro_png))
                        .AddComponent(new SpriteRenderer())
                        .AddComponent(new RectangleCollider2D())
                        .AddComponent(new TouchGestures());
            //hide instructions when pressed
            intro.FindComponent<TouchGestures>().TouchTap += (s, e) =>
            {
                intro.IsVisible = false;
            };
            this.EntityManager.Add(intro);

            //instructions button
            instructions = new Entity()
                        .AddComponent(new Transform2D()
                        {
                            Position = new Vector2(-225, 250),
                            DrawOrder = 0.9f
                        })
                        .AddComponent(new Sprite(WaveContent.Assets.Images.instructions_png))
                        .AddComponent(new SpriteRenderer())
                        .AddComponent(new RectangleCollider2D())
                        .AddComponent(new TouchGestures());
            //show instructions when pressed
            instructions.FindComponent<TouchGestures>().TouchTap += (s, e) =>
            {
                intro.IsVisible = true;
            };
            this.EntityManager.Add(instructions);

            //exit button
            exit = new Entity()
                        .AddComponent(new Transform2D()
                        {
                            Position = new Vector2(83, 250),
                            DrawOrder = 0.9f
                        })
                        .AddComponent(new Sprite(WaveContent.Assets.Images.exit_png))
                        .AddComponent(new SpriteRenderer())
                        .AddComponent(new RectangleCollider2D())
                        .AddComponent(new TouchGestures());
            //exit when pressed
            exit.FindComponent<TouchGestures>().TouchTap += (s, e) =>
            {
                Program.game.shouldExit = true;
            };
            this.EntityManager.Add(exit);
        }

        protected void ResetGame()
        {
            //set all pieces on starting positions
            for (int i = 0; i < 8; i++)
            {
                pieces[i].SetStartingPosition();
            }
            //make all slots available
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    slotAvailable[i][j] = true;
                }
            }
            //no filled slots
            filledSlots = 0;
        }
    }
}
