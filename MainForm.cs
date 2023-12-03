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
        private LineTool _lineTool;
        private MouseEventHandler _mouseDownEvent;
        private MouseEventHandler _mouseMoveEvent;
        private MouseEventHandler _mouseUpEvent;
        private PaintEventHandler _paintEvent;
        public MainForm()   
        {
            InitializeComponent();
            InitColors();
            tabControl1.SelectedIndex = 1;
            _lineColor = Color.Black;
            currentColor.BackColor = _lineColor;
            Pen pen = ReturnCurrentPen();            
            _bitmap = new Bitmap(canvas.Width, canvas.Height);
            _graphics = Graphics.FromImage(_bitmap);
            _penTool = new PenTool(_graphics, pen);
            _lineTool = new LineTool(_graphics, ReturnCurrentPen());
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
                    _lineTool.ChangePen(ReturnCurrentPen());    
                    currentColor.BackColor = _lineColor;
                };
            }
        }
        private Pen ReturnCurrentPen()
        {
            Pen pen = new Pen(_lineColor, trackBar1.Value);
            pen.DashStyle = DashStyle.Solid;
            pen.StartCap = LineCap.Round;
            return pen;
        }              

        private void palette_Click(object sender, EventArgs e)
        {

            ColorDialog colorDialog = new ColorDialog();
            canvas.Paint -= _paintEvent;
            if (colorDialog.ShowDialog() == DialogResult.OK)
                _lineColor = colorDialog.Color;

            currentColor.BackColor = _lineColor;
            _penTool.ChangePen(ReturnCurrentPen());
            
            canvas.Paint += _paintEvent;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            _penTool.ChangePen(ReturnCurrentPen());
        }


        private void ClearEvents()
        {
            canvas.MouseDown -= _mouseDownEvent;
            canvas.MouseMove -= _mouseMoveEvent;
            canvas.MouseUp -= _mouseUpEvent;
            canvas.Paint -= _paintEvent;
        }
        private void SubEvents()
        {
            canvas.MouseDown += _mouseDownEvent;
            canvas.MouseMove += _mouseMoveEvent;
            canvas.MouseUp += _mouseUpEvent;
            canvas.Paint += _paintEvent;
        }

        private void penButton_Click(object sender, EventArgs e)
        {
            ClearEvents();
            _mouseDownEvent = (mySender, myE) => { _penTool.StartDrawing(myE.Location); };
            _mouseMoveEvent = (mySender, myE) =>
            {
                _penTool.Drawing(myE.Location);
                canvas.Invalidate();
            };
            _paintEvent = (mySender, myE) => { canvas.Image = _bitmap; };
            _mouseUpEvent = (mySender, myE) => { _penTool.StopDrawing(); };
            SubEvents();
        }

        private void lineButton_Click(object sender, EventArgs e)
        {
            ClearEvents();
            _mouseDownEvent = (mySender, myE) => { _lineTool.StartDrawing(myE.Location); };
            _mouseMoveEvent = (mySender, myE) =>
            {
                _lineTool.Paint(myE.Location); 
                canvas.Invalidate();
            };
            _paintEvent = (mySender, myE) => {
                myE.Graphics.DrawLine(ReturnCurrentPen(), _lineTool.StartPoint, _lineTool.EndPoint); };
            _mouseUpEvent = (mySender, myE) => 
            {
                _lineTool.StopDrawing();
                _lineTool.Draw(myE.Location);
                canvas.Image = _bitmap;                
            };
            SubEvents();
        }
    }
}
