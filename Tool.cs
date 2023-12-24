using System;
using System.Collections.Generic;
using System.Drawing;

namespace Paint
{
    internal class Tool
    {
        public Graphics MyGraphics { get; private set; }
        public bool IsDrawing { get; private set; } = false;
        public Point StartPoint { get; private set; }
        public Point EndPoint { get; private set; }

        public Tool(Graphics graphics)
        {
            MyGraphics = graphics;
        }

        public virtual void Draw(Pen pen, Point endPoint)
        {
            if (IsDrawing == false) return;
            MyGraphics.DrawLine(pen, StartPoint, endPoint);
            StartPoint = endPoint;
        }

        public void ChangeLastPoint(Point endPoint)
        {
            if (IsDrawing == false) return;
            EndPoint = endPoint;
        }

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

    internal class LineTool : Tool
    {
        public LineTool(Graphics graphics) : base(graphics) { }

        public override void Draw(Pen pen, Point endPoint)
        {
            MyGraphics.DrawLine(pen, StartPoint, endPoint);
        }
    }

    internal class CircleTool : Tool
    {
        public CircleTool(Graphics graphics) : base(graphics) { }

        public override void Draw(Pen pen, Point endPoint)
        {
            Rectangle circleRect = new Rectangle(StartPoint.X, StartPoint.Y, endPoint.X - StartPoint.X, endPoint.Y - StartPoint.Y);
            MyGraphics.DrawEllipse(pen, circleRect);
        }
    }
    internal class RectangleTool : Tool
    {
        public RectangleTool(Graphics graphics) : base(graphics) { }

        public override void Draw(Pen pen, Point endPoint)
        {
            MyGraphics.DrawRectangle(pen, GetRectangle());
        }
        public Rectangle GetRectangle() 
        {
            int x = Math.Min(StartPoint.X, EndPoint.X);
            int y = Math.Min(StartPoint.Y, EndPoint.Y);
            int width = Math.Abs(EndPoint.X - StartPoint.X);
            int height = Math.Abs(EndPoint.Y - StartPoint.Y);

            return new Rectangle(x, y, width, height);
        }
    }
    internal class TriangleTool : Tool
    {
        public TriangleTool(Graphics graphics) : base(graphics) { }

        public List<Point> GetPoints()
        {
            List<Point> points = new List<Point>(3)
            {
                EndPoint,
                new Point(StartPoint.X, EndPoint.Y),
                new Point((EndPoint.X - StartPoint.X) / 2 + StartPoint.X, StartPoint.Y)
            };            
            return points;
        }

        public override void Draw(Pen pen, Point endPoint)
        {
            List<Point> points = GetPoints();
            MyGraphics.DrawLine(pen, points[0], points[1]);
            MyGraphics.DrawLine(pen, points[0], points[2]);
            MyGraphics.DrawLine(pen, points[1], points[2]);
            points.Clear();
        }
    }
}
