using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paint
{
    public partial class MainForm : Form
    {
        private PenTool _penTool;
        private Graphics _graphics;
        private Bitmap _bitmap;
        private Color _lineColor;
        public MainForm()   
        {
            InitializeComponent();
            InitColors();
            Pen pen = ReturnCurrentPen();
            _bitmap = new Bitmap(canvas.Width, canvas.Height);
            _graphics = Graphics.FromImage(_bitmap);
            _penTool = new PenTool(_graphics, pen);
        }
        private void InitColors()
        {
            foreach (Control control in colorsPanel.Controls)
            {
                control.Click += (sender, e) =>
                {
                    _lineColor = ((Control)sender).BackColor;
                    ((Control)sender).Focus();
                    _penTool.ChangePen(ReturnCurrentPen());
                    currentColor.BackColor = _lineColor;
                };
            }
        }
        private Pen ReturnCurrentPen()
        {
            Pen pen = new Pen(_lineColor, 2f);
            pen.DashStyle = DashStyle.Solid;
            pen.StartCap = LineCap.Round;
            return pen;
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            _penTool.StartDrawing(e.Location);
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            _penTool.Drawing(e.Location);
            canvas.Invalidate();
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            _penTool.StopDrawing();
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            canvas.Image = _bitmap;
        }

        private void palette_Click(object sender, EventArgs e)
        {

            ColorDialog colorDialog = new ColorDialog();
            canvas.Paint -= canvas_Paint;
            if (colorDialog.ShowDialog() == DialogResult.OK)
                _lineColor = colorDialog.Color;

            currentColor.BackColor = _lineColor;
            _penTool.ChangePen(ReturnCurrentPen());
            
            canvas.Paint += canvas_Paint;
        }
    }
}
