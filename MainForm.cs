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
        private Tool _penTool;
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
        private MouseEventHandler _mouseClickEvent;
        private PaintEventHandler _paintEvent;
        private Pen _pen
        {
            get
            {
                Pen pen = new Pen(_lineColor, trackBar1.Value);
                pen.DashStyle = DashStyle.Solid;
                pen.StartCap = LineCap.Round;
                return pen;
            }
        }
        public MainForm()   
        {
            InitializeComponent();
            InitColors();
            tabControl1.SelectedIndex = 1;
            _lineColor = Color.Black;
            currentColor.BackColor = _lineColor;        
            _bitmap = new Bitmap(canvas.Width, canvas.Height);
            _graphics = Graphics.FromImage(_bitmap);
            _penTool = new Tool(_graphics);
            _lineTool = new LineTool(_graphics);
            _circleTool = new CircleTool(_graphics);
            _rectTool = new RectangleTool(_graphics);
            _triangleTool = new TriangleTool(_graphics);
        }
        private void InitColors()
        {
            foreach (Control control in colorsPanel.Controls)
            {
                control.Click += (sender, e) =>
                {
                    _lineColor = ((Control)sender).BackColor;
                    ((Control)sender).Focus();
                    currentColor.BackColor = _lineColor;
                };
            }
        }          

        private void palette_Click(object sender, EventArgs e)
        {

            ColorDialog colorDialog = new ColorDialog();
            canvas.Paint -= _paintEvent;
            if (colorDialog.ShowDialog() == DialogResult.OK)
                _lineColor = colorDialog.Color;

            currentColor.BackColor = _lineColor;            
            canvas.Paint += _paintEvent;
        }

        private void ClearEvents()
        {
            canvas.MouseDown -= _mouseDownEvent;
            canvas.MouseMove -= _mouseMoveEvent;
            canvas.MouseUp -= _mouseUpEvent;
            canvas.MouseClick -= _mouseClickEvent;
        }
        private void SubEvents()
        {
            canvas.MouseDown += _mouseDownEvent;
            canvas.MouseMove += _mouseMoveEvent;
            canvas.MouseUp += _mouseUpEvent;
        }

        private void penButton_Click(object sender, EventArgs e)
        {
            ClearEvents();
            _mouseDownEvent = (mySender, myE) => { _penTool.StartDrawing(myE.Location); };
            _mouseMoveEvent = (mySender, myE) =>
            {
                _penTool.Draw(_pen, myE.Location);
                canvas.Invalidate();
            };
            _paintEvent = (mySender, myE) => { canvas.Image = _bitmap; };
            _mouseUpEvent = (mySender, myE) => { _penTool.StopDrawing(); };
            SubEvents();
        }

        private void lineButton_Click(object sender, EventArgs e)
        {
            ClearEvents();
            _mouseDownEvent = (mySender, myE) =>
            {
                canvas.Paint += _paintEvent;
                _lineTool.StartDrawing(myE.Location);
            };
            _mouseMoveEvent = (mySender, myE) =>
            {
                _lineTool.ChangeLastPoint(myE.Location); 
                canvas.Invalidate();
            };
            _paintEvent = (mySender, myE) => { myE.Graphics.DrawLine(_pen, _lineTool.StartPoint, _lineTool.EndPoint); };
                
            _mouseUpEvent = (mySender, myE) => 
            {
                canvas.Paint -= _paintEvent;
                _lineTool.StopDrawing();
                _lineTool.Draw(_pen, myE.Location);
                canvas.Image = _bitmap;
            };
            SubEvents();
        }

        private void circleButton_Click(object sender, EventArgs e)
        {
            ClearEvents();
            _mouseDownEvent = (mySender, myE) =>
            {
                canvas.Paint += _paintEvent; 
                _circleTool.StartDrawing(myE.Location);
            };
            _mouseMoveEvent = (mySender, myE) =>
            {
                _circleTool.ChangeLastPoint(myE.Location);
                canvas.Invalidate();
            };
            _paintEvent = (mySender, myE) => 
            {
                Rectangle circleRect = new Rectangle(_circleTool.StartPoint.X, _circleTool.StartPoint.Y,
                    _circleTool.EndPoint.X - _circleTool.StartPoint.X, _circleTool.EndPoint.Y - _circleTool.StartPoint.Y);
                myE.Graphics.DrawEllipse(_pen, circleRect);
            };
            _mouseUpEvent = (mySender, myE) =>
            {
                canvas.Paint -= _paintEvent;
                _circleTool.StopDrawing();
                _circleTool.Draw(_pen,myE.Location);
                canvas.Image = _bitmap;
            };
            SubEvents();
        }


        private void rectangleButton_Click(object sender, EventArgs e)
        {

            ClearEvents();
            _mouseDownEvent = (mySender, myE) =>
            {
                canvas.Paint += _paintEvent; 
                _rectTool.StartDrawing(myE.Location);
            };
            _mouseMoveEvent = (mySender, myE) =>
            {
                _rectTool.ChangeLastPoint(myE.Location);
                canvas.Invalidate();
            };
            _paintEvent = (mySender, myE) =>
            {
                myE.Graphics.DrawRectangle(_pen, _rectTool.GetRectangle());
            };
            _mouseUpEvent = (mySender, myE) =>
            {
                canvas.Paint -= _paintEvent;
                _rectTool.StopDrawing();
                _rectTool.Draw(_pen, myE.Location);
                canvas.Image = _bitmap;
            };
            SubEvents();
        }

        private void triangleButton_Click(object sender, EventArgs e)
        {
            {

                ClearEvents();
                _mouseDownEvent = (mySender, myE) =>
                {
                    canvas.Paint += _paintEvent;
                    _triangleTool.StartDrawing(myE.Location);
                };
                _mouseMoveEvent = (mySender, myE) =>
                {
                    _triangleTool.ChangeLastPoint(myE.Location);
                    canvas.Invalidate();
                    coordinates.Text = myE.Location.ToString();
                };
                _paintEvent = (mySender, myE) =>
                {
                    List<Point> points = _triangleTool.GetPoints();
                    myE.Graphics.DrawLine(_pen, points[0], points[1]);
                    myE.Graphics.DrawLine(_pen, points[0], points[2]);
                    myE.Graphics.DrawLine(_pen, points[1], points[2]);
                    coord.Text = $"start: {_triangleTool.StartPoint}, end: {_triangleTool.EndPoint}";
                };
                _mouseUpEvent = (mySender, myE) =>
                {
                    canvas.Paint -= _paintEvent;
                    _triangleTool.StopDrawing();
                    _triangleTool.Draw(_pen, myE.Location);
                    canvas.Image = _bitmap;
                };
                SubEvents();
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            ClearEvents();
            _graphics.Clear(canvas.BackColor);
            canvas.Invalidate();    
            SubEvents();
        }

        private void eraserButton_Click(object sender, EventArgs e)
        {
            ClearEvents();
            _mouseDownEvent = (mySender, myE) => { _penTool.StartDrawing(myE.Location); };
            _mouseMoveEvent = (mySender, myE) =>
            {
                _penTool.Draw(new Pen(canvas.BackColor, trackBar1.Value), myE.Location);
                canvas.Invalidate();
            };
            _paintEvent = (mySender, myE) => { canvas.Image = _bitmap; };
            _mouseUpEvent = (mySender, myE) => { _penTool.StopDrawing(); };
            SubEvents();
        }
        private void DrawPoint(int x, int y, Color fillColor)
        {
            if (x >= 0 && x < _bitmap.Width && y >= 0 && y < _bitmap.Height)
                _bitmap.SetPixel(x, y, fillColor);
        }
        private bool Equals(Color color1, Color color2)
        {
            return (color1.R == color2.R) && (color1.G == color2.G)
                && (color1.B == color2.B) && (color1.A == color2.A);
        }
        public void FillByKoroyed(int x, int y, Color fillColor)
        {
            Color targetColor = _bitmap.GetPixel(x, y);
            Stack<(int, int)> stack = new Stack<(int, int)>();
            stack.Push((x, y));
            int currentX, currentY;

            while (stack.Count > 0)
            {
                (currentX, currentY) = stack.Pop();
                if (currentX >= 0 && currentX < _bitmap.Width && currentY >= 0 && currentY < _bitmap.Height)
                {
                    Color currentColor = _bitmap.GetPixel(currentX, currentY);
                    if (!Equals(currentColor, fillColor) && Equals(currentColor, targetColor))
                    {
                        DrawPoint(currentX, currentY, fillColor);
                        stack.Push((currentX - 1, currentY));
                        stack.Push((currentX + 1, currentY));
                        stack.Push((currentX, currentY - 1));
                        stack.Push((currentX, currentY + 1));
                    }
                }
            }
        }

        private void fillButton_Click(object sender, EventArgs e)
        {
            ClearEvents();
            _mouseClickEvent = (mySender, myE) => 
            { 
                FillByKoroyed(myE.X, myE.Y, _lineColor);
                canvas.Invalidate();
            };
            canvas.MouseClick += _mouseClickEvent;
            
        }

       
    }
}
