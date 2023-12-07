using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Reflection;
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
        private CircleTool _circleTool;
        private RectangleTool _rectTool;
        private TriangleTool _triangleTool;
        private MouseEventHandler _mouseDownEvent;
        private MouseEventHandler _mouseMoveEvent;
        private MouseEventHandler _mouseUpEvent;
        private PaintEventHandler _paintEvent;
        private Graphics _canvasGraphics;

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
            _circleTool = new CircleTool(_graphics, ReturnCurrentPen());
            _rectTool = new RectangleTool(_graphics, ReturnCurrentPen());
            _triangleTool = new TriangleTool(_graphics, ReturnCurrentPen());
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
                    _circleTool.ChangePen(ReturnCurrentPen());
                    _rectTool.ChangePen(ReturnCurrentPen());
                    _triangleTool.ChangePen(ReturnCurrentPen());
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
            _paintEvent = (mySender, myE) => 
            {
                myE.Graphics.DrawLine(ReturnCurrentPen(), _lineTool.StartPoint, _lineTool.EndPoint);
                _canvasGraphics = myE.Graphics;
            };
                
            _mouseUpEvent = (mySender, myE) => 
            {
                //_canvasGraphics.Clear(canvas.BackColor);
                _lineTool.StopDrawing();
                _lineTool.Draw(myE.Location);
                canvas.Image = _bitmap;                
            };
            SubEvents();
        }

        private void circleButton_Click(object sender, EventArgs e)
        {
            ClearEvents();
            _mouseDownEvent = (mySender, myE) => { _circleTool.StartDrawing(myE.Location); };
            _mouseMoveEvent = (mySender, myE) =>
            {
                _circleTool.Paint(myE.Location);
                canvas.Invalidate();
            };
            _paintEvent = (mySender, myE) => 
            {
                Rectangle circleRect = new Rectangle(_circleTool.StartPoint.X, _circleTool.StartPoint.Y,
                    _circleTool.EndPoint.X - _circleTool.StartPoint.X, _circleTool.EndPoint.Y - _circleTool.StartPoint.Y);
                myE.Graphics.DrawEllipse(ReturnCurrentPen(), circleRect);
                _canvasGraphics = myE.Graphics;
            };
            _mouseUpEvent = (mySender, myE) =>
            {
                //_canvasGraphics.Clear(Color.White);
                _circleTool.StopDrawing();
                _circleTool.Draw(myE.Location);
                canvas.Image = _bitmap;
            };
            SubEvents();
        }

        private void rectangleButton_Click(object sender, EventArgs e)
        {

            ClearEvents();
            _mouseDownEvent = (mySender, myE) => { _rectTool.StartDrawing(myE.Location); };
            _mouseMoveEvent = (mySender, myE) =>
            {
                _rectTool.Paint(myE.Location);
                canvas.Invalidate();
            };
            _paintEvent = (mySender, myE) =>
            {
                Rectangle Rect = new Rectangle(_rectTool.StartPoint.X, _rectTool.StartPoint.Y,
                    _rectTool.EndPoint.X - _rectTool.StartPoint.X, _rectTool.EndPoint.Y - _rectTool.StartPoint.Y);
                myE.Graphics.DrawRectangle(ReturnCurrentPen(), Rect);
                _canvasGraphics = myE.Graphics;
            };
            _mouseUpEvent = (mySender, myE) =>
            {
                //_canvasGraphics.Clear(canvas.BackColor);
                _rectTool.StopDrawing();
                _rectTool.Draw(myE.Location);
                canvas.Image = _bitmap;
            };
            SubEvents();
        }

        private void triangleButton_Click(object sender, EventArgs e)
        {
            {

                ClearEvents();
                _mouseDownEvent = (mySender, myE) => { _triangleTool.StartDrawing(myE.Location); };
                _mouseMoveEvent = (mySender, myE) =>
                {
                    _triangleTool.Paint(myE.Location);
                    canvas.Invalidate();
                    coordinates.Text = myE.Location.ToString();
                };
                _paintEvent = (mySender, myE) =>
                {
                    List<Point> points = _triangleTool.GetPoints();
                    myE.Graphics.DrawLine(ReturnCurrentPen(), points[0], points[1]);
                    myE.Graphics.DrawLine(ReturnCurrentPen(), points[0], points[2]);
                    myE.Graphics.DrawLine(ReturnCurrentPen(), points[1], points[2]);
                    coord.Text = $"start: {_triangleTool.StartPoint}, end: {_triangleTool.EndPoint}";
                    _canvasGraphics = myE.Graphics;
                };
                _mouseUpEvent = (mySender, myE) =>
                {
                    //_canvasGraphics.Clear(canvas.BackColor);
                    _triangleTool.StopDrawing();
                    _triangleTool.Draw(myE.Location);
                    canvas.Image = _bitmap;
                };
                SubEvents();
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            _graphics.Clear(canvas.BackColor);
        }
    }
}
