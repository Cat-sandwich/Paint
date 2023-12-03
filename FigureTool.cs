using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
}
