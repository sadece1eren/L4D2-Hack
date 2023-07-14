using ezOverLay;
using swed32;
using System.Threading;
using static System.Windows.Forms.DataFormats;

namespace Left_4_Dead_2_External
{
    public partial class Form2 : Form
    {
        const int localplayer = 0x0724B58;
        const int entitylist = 0x0748524;
        const int viewmatrix = 0x0601FEC;
        const int viewoffset = 0x2E4;
        const int zomb_alive = 0x158;
        const int team = 0xE4;
        const int health = 0xEC;
        const int zomb_xyz = 0x11C;
        const int zomb_team = 0xDC;
        const int zomb_hp = 0xE4;
        const int zomb_id = 0x0;
        Pen health100 = new Pen(Color.FromArgb(16, 255, 0), 3);
        Pen health60 = new Pen(Color.FromArgb(64, 204, 0), 3);
        Pen health40 = new Pen(Color.FromArgb(112, 153, 0), 3);
        Pen health20 = new Pen(Color.FromArgb(159, 102, 0), 3);
        Pen health1 = new Pen(Color.FromArgb(207, 51, 0), 3);
        Pen teamPen = new Pen(Color.Blue, 3);
        Pen EnemyPen = new Pen(Color.Red, 3);
        swed jhrox = new swed();
        ez ez = new ez();
        entity player = new entity();
        public List<entity> list = new List<entity>();
        IntPtr client;
        IntPtr engine;
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            jhrox.GetProcess("left4dead2");
            client = jhrox.GetModuleBase("client.dll");
            engine = jhrox.GetModuleBase("engine.dll");
            ez.SetInvi(this);
            ez.DoStuff("Left 4 Dead 2 - Direct3D 9", this);

            Thread thread = new Thread(main) { IsBackground = true };
            thread.Start();
        }

        void main()
        {
            while (true)
            {
                updatelocal();
                updateentities();
                panel1.Refresh();
                Thread.Sleep(13);
            }
        }


        void updatelocal()
        {
            var buffer = jhrox.ReadPointer(client, localplayer);
            player.team = BitConverter.ToInt32(jhrox.ReadBytes(buffer, team, 4));
        }

        void updateentities()
        {
            list.Clear();

            for (int i = 0; i < 850; i++)
            {
                var buffer = jhrox.ReadPointer(client, entitylist + i * 0x8);
                var isalive = BitConverter.ToInt32(jhrox.ReadBytes(buffer, zomb_alive, 4), 0);
                var entityteam = BitConverter.ToInt32(jhrox.ReadBytes(buffer, zomb_team, 4), 0);
                var entityhealth = BitConverter.ToInt32(jhrox.ReadBytes(buffer, zomb_hp, 4), 0);
                var ID = BitConverter.ToInt32(jhrox.ReadBytes(buffer, zomb_id, 4), 0);
                if ( (byte)ID != 124 && (byte)ID != 116 && entityhealth < 2)
                    continue;

                var coords = jhrox.ReadBytes(buffer, zomb_xyz, 12);

                var ent = new entity
                {
                    x = BitConverter.ToSingle(coords, 0),
                    y = BitConverter.ToSingle(coords, 4),
                    z = BitConverter.ToSingle(coords, 8),
                    team = entityteam,
                    health = entityhealth
                };

                ent.bot = WorldToScreen(readmatrix(), ent.x, ent.y, ent.z, Width, Height);

                ent.top = WorldToScreen(readmatrix(), ent.x, ent.y, ent.z + 58, Width, Height);


                list.Add(ent);
                var a = 31;
            }
        }

        viewmatrix readmatrix()
        {
            var matrix = new viewmatrix();

            var buffer = new byte[16 * 4];
            var test = jhrox.ReadPointer(engine, viewmatrix);
            buffer = jhrox.ReadBytes(test, viewoffset, buffer.Length);
            matrix.m11 = BitConverter.ToSingle(buffer, 0 * 4);
            matrix.m12 = BitConverter.ToSingle(buffer, 1 * 4);
            matrix.m13 = BitConverter.ToSingle(buffer, 2 * 4);
            matrix.m14 = BitConverter.ToSingle(buffer, 3 * 4);
            matrix.m21 = BitConverter.ToSingle(buffer, 4 * 4);
            matrix.m22 = BitConverter.ToSingle(buffer, 5 * 4);
            matrix.m23 = BitConverter.ToSingle(buffer, 6 * 4);
            matrix.m24 = BitConverter.ToSingle(buffer, 7 * 4);
            matrix.m31 = BitConverter.ToSingle(buffer, 8 * 4);
            matrix.m32 = BitConverter.ToSingle(buffer, 9 * 4);
            matrix.m33 = BitConverter.ToSingle(buffer, 10 * 4);
            matrix.m34 = BitConverter.ToSingle(buffer, 11 * 4);
            matrix.m41 = BitConverter.ToSingle(buffer, 12 * 4);
            matrix.m42 = BitConverter.ToSingle(buffer, 13 * 4);
            matrix.m43 = BitConverter.ToSingle(buffer, 14 * 4);
            matrix.m44 = BitConverter.ToSingle(buffer, 15 * 4);
            return matrix;

        }

        Point WorldToScreen(viewmatrix mtx, float x, float y, float z, int width, int height)
        {
            var twoD = new Point();

            float screenW = (mtx.m41 * x) + (mtx.m42 * y) + (mtx.m43 * z) + mtx.m44;
            if (screenW > 0.001f)
            {
                float screenX = (mtx.m11 * x) + (mtx.m12 * y) + (mtx.m13 * z) + mtx.m14;
                float screenY = (mtx.m21 * x) + (mtx.m22 * y) + (mtx.m23 * z) + mtx.m24;

                float camX = width / 2f;
                float camY = height / 2f;


                float X = camX + (camX * screenX / screenW);

                float Y = camY - (camY * screenY / screenW);

                twoD.X = (int)X;
                twoD.Y = (int)Y;
            }
            else
            {
                return new Point(-99, -99);
            }


            return twoD;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            list.Clear();

            for (int i = 0; i < 850; i++)
            {
                var buffer = jhrox.ReadPointer(client, entitylist + i * 0x8);
                var entityteam = BitConverter.ToInt32(jhrox.ReadBytes(buffer, zomb_team, 4), 0);
                var entityhealth = BitConverter.ToInt32(jhrox.ReadBytes(buffer, zomb_hp, 4), 0);
                var isalive = BitConverter.ToInt32(jhrox.ReadBytes(buffer, zomb_alive, 4), 0);
                var ID = BitConverter.ToInt32(jhrox.ReadBytes(buffer, zomb_id, 4), 0);
                if ((byte)ID != 124 && (byte)ID != 116 && entityhealth < 2)
                    continue;
                var coords = jhrox.ReadBytes(buffer, zomb_xyz, 12);
                var ent = new entity
                {
                    x = BitConverter.ToSingle(coords, 0),
                    y = BitConverter.ToSingle(coords, 4),
                    z = BitConverter.ToSingle(coords, 8),
                    team = entityteam,
                    health = entityhealth
                };
                ent.bot = WorldToScreen(readmatrix(), ent.x, ent.y, ent.z, Width, Height);
                ent.top = WorldToScreen(readmatrix(), ent.x, ent.y, ent.z + 58, Width, Height);
                list.Add(ent);
                var sex = e.Graphics;
                Font font = new Font("Roboto", 12, FontStyle.Regular);
                Brush brush = Brushes.DarkRed;
                string healthtext = Convert.ToString(entityhealth);
                if (list.Count > 0)
                {
                    try
                    {
                        if (ent.team == player.team && ent.bot.X > 0 && ent.bot.X < Width && ent.bot.Y > 0 && ent.bot.Y < Height)
                        {
                            if (entityhealth >= 100)
                            {
                                sex.DrawRectangle(health100, ent.rect());
                                sex.DrawLine(teamPen, Width / 2, Height, ent.bot.X, ent.bot.Y);
                            }
                            else if (entityhealth >= 60)
                            {
                                sex.DrawRectangle(health60, ent.rect());
                                sex.DrawLine(teamPen, Width / 2, Height, ent.bot.X, ent.bot.Y);
                            }
                            else if (entityhealth >= 40)
                            {
                                sex.DrawRectangle(health40, ent.rect());
                                sex.DrawLine(teamPen, Width / 2, Height, ent.bot.X, ent.bot.Y);
                            }
                            else if (entityhealth >= 20)
                            {
                                sex.DrawRectangle(health20, ent.rect());
                                sex.DrawLine(teamPen, Width / 2, Height, ent.bot.X, ent.bot.Y);
                            }
                            else if (entityhealth >= 1)
                            {
                                sex.DrawRectangle(health1, ent.rect());
                                sex.DrawLine(teamPen, Width / 2, Height, ent.bot.X, ent.bot.Y);
                            }
                        }
                        else if (ent.team != player.team && ent.bot.X > 0 && ent.bot.X < Width && ent.bot.Y > 0 && ent.bot.Y < Height)
                        {
                            sex.DrawRectangle(EnemyPen, ent.rect());
                            sex.DrawLine(EnemyPen, Width / 2, Height, ent.bot.X, ent.bot.Y);
                        }

                    }
                    catch { }
                }
            }
        }
    }
}