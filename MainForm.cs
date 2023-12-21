﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Tracing;
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
        private LineTool _lineTool;
        private CircleTool _circleTool;
        private RectangleTool _rectTool;
        private TriangleTool _triangleTool;

        private Graphics _graphics;
        private Stack <Bitmap> _bitmaps;
        private Bitmap _bitmap;
       
        private Color _lineColor;

        private MouseEventHandler _mouseDownEvent;
        private MouseEventHandler _mouseMoveEvent;
        private MouseEventHandler _mouseUpEvent;
        private MouseEventHandler _mouseClickEvent;
        private KeyPressEventHandler _textBoxKeyPressEvent;
        private PaintEventHandler _paintEvent;
        private MouseEventHandler _selectedAreaMouseDown;
        private MouseEventHandler _selectedAreaMouseMove;
        private MouseEventHandler _selectedAreaMouseUp;

        private Label _currentTextLabel;
        private Font _font;
        private bool _isWriting;
        
        private PictureBox _selectedArea;
        private bool _isDragging;
        private Point _startAllocatePoint;
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
            _bitmaps = new Stack<Bitmap>();
            _bitmap = new Bitmap(canvas.Width, canvas.Height);
            _bitmaps.Push(new Bitmap(_bitmap));
            InitTools();
            _font = new Font("Arial", 16, FontStyle.Regular);
            _currentTextLabel = new Label();
        }

        private void InitTools()
        {
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
            canvas.Paint -= canvas_Paint;
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
            _currentTextLabel.KeyPress -= _textBoxKeyPressEvent;
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
            _mouseDownEvent = (mySender, myE) =>
            {
                _penTool.StartDrawing(myE.Location);
            };
            _mouseMoveEvent = (mySender, myE) =>
            {
                _penTool.Draw(_pen, myE.Location);
                canvas.Invalidate();
            };
            _mouseUpEvent = (mySender, myE) =>
            {
                _penTool.StopDrawing();
                _bitmaps.Push(new Bitmap(_bitmap));
            };
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
                if (_lineTool.IsDrawing == false) return;
                _lineTool.ChangeLastPoint(myE.Location);
                canvas.Invalidate();
            };
            _paintEvent = (mySender, myE) => 
            { 
                if(_lineTool.IsDrawing == false) return;
                myE.Graphics.DrawLine(_pen, _lineTool.StartPoint, _lineTool.EndPoint);
            };

            _mouseUpEvent = (mySender, myE) =>
            {
                canvas.Paint -= _paintEvent;
                _lineTool.StopDrawing();
                _lineTool.Draw(_pen, myE.Location);
                canvas.Invalidate();
                _bitmaps.Push(new Bitmap(_bitmap));
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
                _circleTool.Draw(_pen, myE.Location);
                canvas.Invalidate();
                _bitmaps.Push(new Bitmap(_bitmap));
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
                canvas.Invalidate();
                _bitmaps.Push(new Bitmap(_bitmap));
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
                };
                _mouseUpEvent = (mySender, myE) =>
                {
                    canvas.Paint -= _paintEvent;
                    _triangleTool.StopDrawing();
                    _triangleTool.Draw(_pen, myE.Location);
                    canvas.Invalidate();
                    _bitmaps.Push(new Bitmap(_bitmap));
                };
                SubEvents();
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            _graphics.Clear(canvas.BackColor);
            _bitmaps.Clear();
            _bitmap = new Bitmap(canvas.Width, canvas.Height);
            _bitmaps.Push(new Bitmap(_bitmap));
            InitTools();
            canvas.Invalidate();
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
            _mouseUpEvent = (mySender, myE) => 
            { 
                _penTool.StopDrawing();
                _bitmaps.Push(new Bitmap(_bitmap));
            };
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
                _bitmaps.Push(new Bitmap(_bitmap));
            };
            canvas.MouseClick += _mouseClickEvent;

        }

        private void pipetteButton_Click(object sender, EventArgs e)
        {
            ClearEvents();
            _mouseClickEvent = (mySender, myE) =>
            {
                Color colorPipette = _bitmap.GetPixel(myE.X, myE.Y);
                currentColor.BackColor = colorPipette;
                _lineColor = colorPipette;
            };
            canvas.MouseClick += _mouseClickEvent;

        }
        
        private void saveFileButton_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "PNG(*.PNG)|*.PNG";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (canvas.Image != null)
                {
                    canvas.Image.Save(saveFileDialog1.FileName);
                }
            }
        }

        private void openFileButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "PNG(*.PNG)|*.PNG";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    canvas.Image = new Bitmap(openFileDialog1.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке изображения: " + ex.Message);
                }
            }
            canvas.Invalidate();
        }

        private void backgroundButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Изображения (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|Все файлы (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Устанавливаем изображение как фон формы
                    canvas.BackgroundImage = new Bitmap(Image.FromFile(openFileDialog1.FileName));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке изображения: " + ex.Message);
                }
            }
            canvas.Invalidate();
        }
        private void InitTextLabel()
        {
            _currentTextLabel = new Label();
            
            _currentTextLabel.Visible = false;
            _currentTextLabel.Font = _font;
            _currentTextLabel.AutoSize = true;
            _currentTextLabel.BackColor = canvas.BackColor;            
            splitContainer1.Panel2.Controls.Add(_currentTextLabel);
            _currentTextLabel.BringToFront();
        }

        private void textButton_Click(object sender, EventArgs e)
        {
            ClearEvents();
            _textBoxKeyPressEvent = (mySender, myE) =>
            {
                _isWriting = true;
                if (myE.KeyChar == (char)Keys.Enter)
                {
                    _isWriting = false;
                    _currentTextLabel.BorderStyle = BorderStyle.None;
                    _graphics.DrawString(_currentTextLabel.Text, _font, _pen.Brush, _currentTextLabel.Location);
                    canvas.Invalidate();
                    _bitmaps.Push(new Bitmap(_bitmap));
                    splitContainer1.Panel2.Controls.Remove(_currentTextLabel);
                }
                else
                {
                    _currentTextLabel.ForeColor = _lineColor;
                    _currentTextLabel.BorderStyle = BorderStyle.FixedSingle;
                    _currentTextLabel.Visible = true;
                    _currentTextLabel.Text += myE.KeyChar;
                }
            };

            _mouseClickEvent = (mySender, myE) =>
            {
                if (_isWriting)
                {
                    _isWriting = false;
                    _currentTextLabel.BorderStyle = BorderStyle.None;
                    _graphics.DrawString(_currentTextLabel.Text, _font, _pen.Brush, _currentTextLabel.Location);
                    canvas.Invalidate();
                    _bitmaps.Push(new Bitmap(_bitmap));
                    splitContainer1.Panel2.Controls.Remove(_currentTextLabel);
                }
                else
                {
                    InitTextLabel();
                    _currentTextLabel.Location = myE.Location;
                    _currentTextLabel.Visible = true;
                    _currentTextLabel.Focus();
                    _currentTextLabel.KeyPress += _textBoxKeyPressEvent;
                }              

            };
            canvas.MouseClick += _mouseClickEvent;
        }
        private void stileTextButton_Click(object sender, EventArgs e)
        {
            canvas.Paint -= canvas_Paint;
            FontDialog fontDialog = new FontDialog();
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                _font = fontDialog.Font;    
            }
            canvas.Paint += canvas_Paint;
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            canvas.Image = _bitmap;
        }
        private void PlaceSelectedArea()
        {
            _isDragging = false;
            _selectedArea.MouseDown -= _selectedAreaMouseDown;
            _selectedArea.MouseMove -= _selectedAreaMouseMove;
            _selectedArea.MouseUp -= _selectedAreaMouseUp;

            _graphics.DrawImage(_selectedArea.Image, _selectedArea.Location);
            _bitmaps.Push(new Bitmap(_bitmap));
            splitContainer1.Panel2.Controls.Remove(_selectedArea);
            _selectedArea = null;
            canvas.Invalidate();
        }

        private void InitDragSelectedAreaIvents()
        {
            _selectedAreaMouseDown = (mySender, myE) =>
            {
                if (myE.Button == MouseButtons.Left)
                {
                    _startAllocatePoint = myE.Location;
                    _isDragging = true;
                }
            };
            _selectedAreaMouseMove = (mySender, myE) =>
            {
                if (_isDragging)
                {
                    int deltaX = myE.X - _startAllocatePoint.X;
                    int deltaY = myE.Y - _startAllocatePoint.Y;
                    _selectedArea.Location = new Point(_selectedArea.Location.X + deltaX, _selectedArea.Location.Y + deltaY);
                }

            };
            _selectedAreaMouseUp = (mySender, myE) =>
            {
                PlaceSelectedArea();
                InitAllocateIvents(new Pen(Color.Blue, 3f));
            };
            _selectedArea.MouseDown += _selectedAreaMouseDown;
            _selectedArea.MouseMove += _selectedAreaMouseMove;
            _selectedArea.MouseUp += _selectedAreaMouseUp;
        }
        private void InitSelectedArea()
        {           
            _selectedArea = new PictureBox();
            _selectedArea.Location = _rectTool.GetRectangle().Location;
            _selectedArea.Size = _rectTool.GetRectangle().Size;
            _selectedArea.BackColor = canvas.BackColor;
            _selectedArea.BorderStyle = BorderStyle.FixedSingle;
        }
        private void CopyImageOnSelectedArea()
        {
            InitSelectedArea();
            Bitmap selectedBitmap = new Bitmap(_selectedArea.Width, _selectedArea.Height);

            using (Graphics g = Graphics.FromImage(selectedBitmap))
            {
                g.DrawImage(_bitmap, 0, 0, _rectTool.GetRectangle(), GraphicsUnit.Pixel);
            }
            _selectedArea.Image = selectedBitmap;
            splitContainer1.Panel2.Controls.Add(_selectedArea);
            _selectedArea.BringToFront();
        }
        private void InitAllocateIvents(Pen pen)
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
                myE.Graphics.DrawRectangle(pen, _rectTool.GetRectangle());
            };
            _mouseUpEvent = (mySender, myE) =>
            {
                if (_selectedArea != null)
                    PlaceSelectedArea();
                CopyImageOnSelectedArea();
                canvas.Paint -= _paintEvent;
                _rectTool.StopDrawing();
                canvas.Invalidate();
                InitDragSelectedAreaIvents();
            };
            SubEvents();
        }
        private void allocateButton_Click(object sender, EventArgs e)
        {
            Pen pen = new Pen(Color.Blue, 3f);
            pen.DashStyle = DashStyle.Dash;
            InitAllocateIvents(pen);
        }


        private void canselButton_Click(object sender, EventArgs e)
        {
            if (_bitmaps.Count <= 1)
                return;

            _bitmaps.Pop();
            _bitmap = _bitmaps.Peek();
            _graphics.Clear(canvas.BackColor);
            InitTools();

            canvas.Invalidate();
        }

        private void canvas_Click(object sender, EventArgs e)
        {

        }
    }

}
