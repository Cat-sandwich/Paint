using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paint
{
    internal abstract class FigureTool
    {
        public bool IsDrawing { get; private set; } = false;
        public Pen MyPen {get; private set;}
        public Point StartPoint { get; private set; }
        public Point EndPoint;
        public FigureTool(Pen pen)
        {
            MyPen = pen;
        }

        public void ChangePen(Pen pen) => MyPen = pen;

        public abstract void Draw(Point endPoint);

        public abstract void Paint(Point endPoint);
        public void StartDrawing(Point startPoint)
        {
            IsDrawing = true;
            StartPoint = startPoint;
        }
        public void StopDrawing()
        {
            IsDrawing = false;
        }

    }
    internal class LineTool: FigureTool
    {
        private Graphics _graphics;
      
        public LineTool(Graphics graphics, Pen pen) : base(pen) { _graphics = graphics; }

        public override void Draw(Point endPoint)
        {
            _graphics.DrawLine(MyPen, StartPoint, endPoint);
        }
        public override void Paint(Point endPoint)
        {
            if (IsDrawing == false) return;
            EndPoint = endPoint;
            
        }

    }

    internal class CircleTool : FigureTool
    {
        private Graphics _graphics;

        public CircleTool(Graphics graphics, Pen pen) : base(pen) { _graphics = graphics; }

        public override void Draw(Point endPoint)
        {
            Rectangle circleRect = new Rectangle(StartPoint.X, StartPoint.Y, endPoint.X - StartPoint.X, endPoint.Y - StartPoint.Y);
            _graphics.DrawEllipse(MyPen,circleRect);
        }
        public override void Paint(Point endPoint)
        {
            if (IsDrawing == false) return;
            EndPoint = endPoint;

        }

    }
    internal class RectangleTool : FigureTool
    {
        private Graphics _graphics;

        public RectangleTool(Graphics graphics, Pen pen) : base(pen) { _graphics = graphics; }

        public override void Draw(Point endPoint)
        {
            Rectangle Rect = new Rectangle(StartPoint.X, StartPoint.Y, endPoint.X - StartPoint.X, endPoint.Y - StartPoint.Y);
            _graphics.DrawRectangle(MyPen, Rect);
        }
        public override void Paint(Point endPoint)
        {
            if (IsDrawing == false) return;
            EndPoint = endPoint;

        }

    }
    internal class TriangleTool : FigureTool
    {
        private Graphics _graphics;
        public TriangleTool(Graphics graphics, Pen pen) : base(pen) 
        {
            _graphics = graphics;
        }

        public List<Point> GetPoints()
        {
            List<Point> points = new List<Point>(3);
            points.Add(EndPoint);
            points.Add(new Point(StartPoint.X, EndPoint.Y));
            points.Add(new Point((EndPoint.X - StartPoint.X) / 2 + StartPoint.X, StartPoint.Y));
            return points;
        }
        public override void Draw(Point endPoint)
        {
            List<Point> points = GetPoints();
            _graphics.DrawLine(MyPen, points[0], points[1]);
            _graphics.DrawLine(MyPen, points[0], points[2]);
            _graphics.DrawLine(MyPen, points[1], points[2]);
            points.Clear();
        }
        public override void Paint(Point endPoint)
        {
            if (IsDrawing == false) return;
            EndPoint = endPoint;

        }

    }
}
