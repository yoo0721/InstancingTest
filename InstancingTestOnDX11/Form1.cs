using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InstancingTestOnDX11
{
    public partial class Form1 : Form
    {
        Engine3D engine = null;
        Boxcell boxcell = null;
        public Form1()
        {
            InitializeComponent();

            engine = new Engine3D();

            boxcell = new Boxcell();
            engine.SetRenderObject(boxcell);
            engine.OnInitialize(this);
        }

        private void OnLoad_form1(object sender, EventArgs e)
        {
            engine.Run();
        }

        private void OnKeyDown_Form1(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.D:
                    engine.camera.MoveLR(-1.0f);
                    break;
                case Keys.A:
                    engine.camera.MoveLR(1.0f);
                    break;
                case Keys.W:
                    engine.camera.MoveFB(1.0f);
                    break;
                case Keys.S:
                    engine.camera.MoveFB(-1.0f);
                    break;
                case Keys.Q:
                    engine.camera.MoveUD(1.0f);
                    break;
                case Keys.E:
                    engine.camera.MoveUD(-1.0f);
                    break;
                case Keys.Left:
                    engine.camera.RotationAxis(0.1f);
                    break;
                case Keys.Right:
                    engine.camera.RotationAxis(-0.1f);
                    break;
                case Keys.Up:
                    engine.camera.RotationZX(-0.1f);
                    break;  
                case Keys.Down:
                    engine.camera.RotationZX(0.1f);
                    break;
                case Keys.D1:
                    engine.camera.Speed += 1.0f;
                    break;
                case Keys.D2:
                    engine.camera.Speed -= 1.0f;
                    break;


            }
        }

        private void OnMouseDown_Form1(object sender, MouseEventArgs e)
        {

        }
    }
}
