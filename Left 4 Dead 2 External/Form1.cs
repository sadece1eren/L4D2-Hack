using Swed32;
using Memory;
using System.Runtime.InteropServices;
using System;
using System.Threading;
using swed32;

namespace Left_4_Dead_2_External
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(Keys vKey);
        const int entityBase = 0x07384F4;
        const int localplayer = 0x0724B58;
        const int viewY = 0x04268EC;
        const int viewX = 0x4AAC;
        const int hp = 0xEC;
        const int xyz = 0x94;
        const int team = 0xE4;
        const int m_flag = 0xF0;
        const int forceJump = 0x0757DF0;
        entity player = new entity();
        List<entity> entityList = new List<entity>();
        Mem jhrox = new Mem();
        public static IntPtr client;
        public static IntPtr engine;
        public static Swed eren;
        public Form1()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            int PID = jhrox.GetProcIdFromName("left4dead2");
            if (PID > 0)
            {
                if (checkBox1.Checked)
                {
                    MessageBox.Show("You Can Activate Aimbot by Holding the Mouse X2 Button UwU", "UwU", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("And This Aimbot Works Only Special Zombs :3", "UwU", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                if (checkBox1.Checked)
                {
                    MessageBox.Show("you need the open game before cheating >:3", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    checkBox1.Checked = false;
                }
            }
        }
        float calcMag(entity entity)
        {
            return (float)Math.Sqrt(Math.Pow(entity.x - player.x, 2) + Math.Pow(entity.y - player.y, 2) + Math.Pow(entity.z - player.z, 2));
        }

        void upadateLocal()
        {
            var buffer = eren.ReadPointer(client, localplayer);
            var teamnum = eren.ReadBytes(buffer, team, 4);
            var pos = eren.ReadBytes(buffer, xyz, 12);
            player.x = BitConverter.ToSingle(pos, 0);
            player.y = BitConverter.ToSingle(pos, 4);
            player.z = BitConverter.ToSingle(pos, 8);
            player.team = BitConverter.ToInt32(teamnum, 0);
        }

        void updateEntities()
        {
            entityList.Clear();
            for (int i = 0; i < 32; i++)
            {
                var buffer = eren.ReadPointer(client, entityBase + i * 0x10);
                var health = BitConverter.ToInt32(eren.ReadBytes(buffer, hp, 4), 0);
                var teamnum = BitConverter.ToInt32(eren.ReadBytes(buffer, team, 4), 0);
                if (health < 2 || teamnum == player.team)
                    continue;
                var pos = eren.ReadBytes(buffer, xyz, 12);
                var ent = new entity
                {
                    x = BitConverter.ToSingle(pos, 0),
                    y = BitConverter.ToSingle(pos, 4),
                    z = BitConverter.ToSingle(pos, 8),
                    team = teamnum,
                    health = health
                };
                ent.mag = calcMag(ent);
                entityList.Add(ent);
            }
        }

        void aimbot(entity entity)
        {
            var buffer = eren.ReadPointer(engine, viewY);
            float deltaX = entity.x - player.x;
            float deltaY = entity.y - player.y;
            float X = (float)(Math.Atan2(deltaY, deltaX) * 180 / Math.PI);
            float deltaZ = entity.z - player.z;
            double dist = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
            float Y = -(float)(Math.Atan2(deltaZ, dist) * 180 / Math.PI);
            eren.WriteBytes(buffer, viewX + 0x4, BitConverter.GetBytes(X));
            eren.WriteBytes(buffer, viewX, BitConverter.GetBytes(Y));
        }

        void aimbot2()
        {

            while (true)
            {
                if (checkBox1.Checked)
                {
                    if (player != null)
                    {
                        if (GetAsyncKeyState(Keys.XButton2) < 0)
                        {
                            upadateLocal();
                            updateEntities();
                            entityList = entityList.OrderBy(o => o.mag).ToList();
                            if (entityList.Count > 0)
                                aimbot(entityList[0]);
                        }
                    }
                }
                Thread.Sleep(1);
            }

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Thread thread = new Thread(bhop) { IsBackground = true };
            int PID = jhrox.GetProcIdFromName("left4dead2");
            if (PID > 0)
            {
                if (checkBox2.Checked)
                {
                    MessageBox.Show("You Can Activate Bhop By Holding Space Bar Rawrrr x3", "x3", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CheckForIllegalCrossThreadCalls = false;
                    thread.Start();
                }
            }
            else
            {
                if (checkBox2.Checked)
                {
                    MessageBox.Show("you need the open game before cheating >:3", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    checkBox2.Checked = false;
                }
            }
        }

        void bhop()
        {
            client = eren.GetModuleBase("client.dll");
            while (true)
            {
                if (checkBox2.Checked)
                {
                    if (GetAsyncKeyState(Keys.Space) < 0)
                    {
                        var buffer = eren.ReadPointer(client, localplayer);
                        var flag = BitConverter.ToInt32(eren.ReadBytes(buffer, m_flag, 4), 0);
                        if (flag == 129 || flag == 131 || flag == 130)
                        {
                            eren.WriteBytes(client, forceJump, BitConverter.GetBytes(5));
                        }
                        else
                        {
                            eren.WriteBytes(client, forceJump, BitConverter.GetBytes(4));
                        }
                    }
                    Thread.Sleep(3);
                }
                else
                {
                    Thread.Sleep(999999999);
                }
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            int PID = jhrox.GetProcIdFromName("left4dead2");
            if (PID > 0)
            {
                if (checkBox3.Checked)
                {
                    Form2 sex = new Form2();
                    sex.Show();
                }
                else
                {
                    Form2 obj = (Form2)Application.OpenForms["Form2"];
                    obj.Close();
                }
            }
            else
            {
                if (checkBox3.Checked)
                {
                    MessageBox.Show("you need the open game before cheating >:3", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    checkBox3.Checked = false;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int PID = jhrox.GetProcIdFromName("left4dead2");
            if (PID > 0)
            {
                Swed jhrox = new Swed("left4dead2");
                var client = jhrox.GetModuleBase("client.dll");
                var buffer = jhrox.ReadPointer(client, localplayer);
                var pos = jhrox.ReadBytes(buffer, xyz, 12);
                this.Text = "Left 4 Dead 2 External | Game is open";
                float x = player.x = BitConverter.ToSingle(pos, 0);
                float y = player.y = BitConverter.ToSingle(pos, 4);
                float z = player.z = BitConverter.ToSingle(pos, 8);
                label4.Text = Convert.ToString(x);
                label5.Text = Convert.ToString(y);
                label6.Text = Convert.ToString(z);
                label8.Text = "left4dead2.exe";
                label9.Visible = true;
                label9.Text = Convert.ToString(PID);
            }
            else
            {
                this.Text = "Left 4 Dead 2 External | Game is not open";
                label4.Text = "Null";
                label5.Text = "Null";
                label6.Text = "Null";
                label8.Text = "Null";
                label9.Visible = false;
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int PID = jhrox.GetProcIdFromName("left4dead2");
            if (PID > 0)
            {
                eren = new Swed("left4dead2");
                client = eren.GetModuleBase("client.dll");
                engine = eren.GetModuleBase("engine.dll");
                CheckForIllegalCrossThreadCalls = false;
                Thread thread = new Thread(aimbot2) { IsBackground = true };
                thread.Start();
            }
            else
            {
                this.Refresh();
            }
        }
    }
}