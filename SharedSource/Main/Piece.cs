using System;
using WaveEngine.Common.Math;
using WaveEngine.Components.Gestures;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;

namespace Mad_Head_Puzzle
{
    public class Piece
    {
        Vector2 startingPos; //starting positon on screen
        Vector2 previousPos; //previous positon on screen
        Vector2[] pattern; //pattern coordinates (from top/left to bottom/right) using vectors as points for pieces appearance
        public Entity entity; //piece entity on screen

        public Piece()
        {
            startingPos = new Vector2(0,0);
            pattern = new Vector2 [] { new Vector2(0,0) };
            previousPos = startingPos;
        }

        public Piece(Vector2 pos,Vector2[] pat)
        {
            startingPos = pos;
            previousPos = startingPos;
            pattern = pat;
        }

        public void SetStartingPosition()
        {
            entity.FindComponent<Transform2D>().Position = startingPos;
        }

        public void InitEntity(string s)
        {
            entity = new Entity()
                .AddComponent(new Transform2D()
                {
                    DrawOrder = 0.1f
                })
                .AddComponent(new Sprite(s))
                .AddComponent(new SpriteRenderer())
                .AddComponent(new PolygonCollider2D()
                {
                    TexturePath = s
                })
                .AddComponent(new TouchGestures()
                {
                    EnabledGestures = SupportedGesture.Translation
                });
        }

        public void SetPosition(Vector2 pos)
        {
            entity.FindComponent<Transform2D>().Position = pos;
        }

        public bool PieceInteresctsWithGrid()
        {
            foreach (Entity[] slotArray in MyScene.slots)
            {
                foreach (Entity slot in slotArray)
                {
                    if (slot.FindComponent<RectangleCollider2D>().Intersects(entity.FindComponent<PolygonCollider2D>()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool PieceOutOfScreen()
        {
            var entt = entity.FindComponent<Transform2D>();
            int x = (int)entt.Position.X;
            int y = (int)entt.Position.Y;
            int xOut = x + (int)entt.Rectangle.Width;
            int yOut = y + (int)entt.Rectangle.Height;
            //Console.WriteLine(x + " x<=w " + -WaveServices.Platform.ScreenWidth / 2);
            //Console.WriteLine(y + " y<=h " + -WaveServices.Platform.ScreenHeight / 2);
            if ((x <= -640) || (y <= -360)) return true;
            if ((xOut >= 640) || (yOut >= 360)) return true;
            return false;
        }

        public void AddTouchEvents()
        {
            //check if piece is out of border when moved and keep it inside screen
            /*entity.FindComponent<TouchGestures>().TouchMoved += (s, e) =>
            {
                int x = (int)entity.FindComponent<Transform2D>().Position.X;
                int y = (int)entity.FindComponent<Transform2D>().Position.Y;
                if (x <= -640) SetPosition(new Vector2(-600, y));
                if (y <= -360) SetPosition(new Vector2(x, -300));
                if (x >= 490) SetPosition(new Vector2(490, y));
                if (y >= 160) SetPosition(new Vector2(x, 160));
            };*/

            //when piece picked up
            entity.FindComponent<TouchGestures>().TouchPressed += (s, e) =>
            {
                previousPos = entity.FindComponent<Transform2D>().Position;
                //set on top
                entity.FindComponent<Transform2D>().DrawOrder = 0f;

                int i = 0; //row in slots
                int j = 0; //column in slots
                int x = (int)entity.FindComponent<Transform2D>().Position.X + 25; //piece x coordinate on screen
                int y = (int)entity.FindComponent<Transform2D>().Position.Y + 25; //piece y coordinate on screen

                //if piece was picked from inside of slots(grid)
                if (PieceInteresctsWithGrid())
                {
                    i = (int)((x + 150) / 50); //find row
                    j = (int)((y + 150) / 50); //find column
                    //from piece pattern set slots available
                    foreach (Vector2 place in pattern)
                    {
                        MyScene.slotAvailable[i + (int)place.X][j + (int)place.Y] = true;
                        MyScene.filledSlots--;
                    }
                }
            };

            //when piece is dropped
            entity.FindComponent<TouchGestures>().TouchReleased += (s, e) =>
            {
                int i = 0; //row in slots
                int j = 0; //column in slots
                int x = (int)entity.FindComponent<Transform2D>().Position.X + 25; //piece x coordinate on screen
                int y = (int)entity.FindComponent<Transform2D>().Position.Y + 25; //piece y coordinate on screen
                
                //if piece was released on slots(grid)
                if (PieceInteresctsWithGrid())
                {
                    i = (int)((x + 150) / 50); //find row
                    j = (int)((y + 150) / 50); //find column

                    bool canPlace = true; //piece can be dropped
                    //from pattern, see if piece is dropped over another piece
                    foreach (Vector2 place in pattern)
                    {
                        //if half of piece is exiting the slots(grid)
                        if ((i + (int)place.X > 5) || (j + (int)place.Y > 5) || (i + (int)place.X < 0) || (j + (int)place.Y < 0))
                        {
                            canPlace = false;
                            break;
                        }
                        canPlace = canPlace && (MyScene.slotAvailable[i + (int)place.X][j + (int)place.Y]);
                    }
                    //if piece can be dropped
                    if (canPlace)
                    {
                        //place piece on slots(grid)
                        SetPosition(new Vector2(i * 50 - 150, j * 50 - 150)); // -150 is starting position of slots(grid)
                        //set all slots unavailable
                        foreach (Vector2 place in pattern)
                        {
                            MyScene.slotAvailable[i + (int)place.X][j + (int)place.Y] = false;
                            //increase number of filled slots
                            MyScene.filledSlots++;
                        }
                        //if number of filled slots matches number of all slots then show congratulations screen (number of slots 6x6=36)
                        if (MyScene.filledSlots == 36)
                        {
                            MyScene.congratulations.IsVisible = true;
                            WaveServices.SoundPlayer.Play(MyScene.congratulationsSound);
                        }
                    }
                    //if piece is over another piece return piece to previous position
                    else
                    {
                        SetPosition(previousPos);
                        
                        x = (int)entity.FindComponent<Transform2D>().Position.X + 25; //piece x coordinate on screen
                        y = (int)entity.FindComponent<Transform2D>().Position.Y + 25; //piece y coordinate on screen

                        //if piece previous position was in slots(grid)
                        if (PieceInteresctsWithGrid())
                        {
                            i = (int)((x + 150) / 50); //find row
                            j = (int)((y + 150) / 50); //find column
                            foreach (Vector2 place in pattern)
                            {
                                MyScene.slotAvailable[i + (int)place.X][j + (int)place.Y] = false;
                                //increase number of filled slots
                                MyScene.filledSlots++;
                            }
                        }
                    }
                }
                else
                {
                    if (PieceInteresctsWithGrid() || PieceOutOfScreen())
                    {
                        SetPosition(previousPos);

                        x = (int)entity.FindComponent<Transform2D>().Position.X + 25; //piece x coordinate on screen
                        y = (int)entity.FindComponent<Transform2D>().Position.Y + 25; //piece y coordinate on screen

                        //if piece previous position was in slots(grid)
                        if (PieceInteresctsWithGrid())
                        {
                            i = (int)((x + 150) / 50); //find row
                            j = (int)((y + 150) / 50); //find column
                            foreach (Vector2 place in pattern)
                            {
                                MyScene.slotAvailable[i + (int)place.X][j + (int)place.Y] = false;
                                //increase number of filled slots
                                MyScene.filledSlots++;
                            }
                        }
                    }
                }

                //move to back
                entity.FindComponent<Transform2D>().DrawOrder = 0.1f;
            };
        }
    }
}
