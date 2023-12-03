using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paint
{
    internal class PenTool
    {
        private bool _isDrawing = false;
        private Graphics _graphics;
        private Point _previousPoint;
        private Pen _pen;
        public PenTool(Graphics graphics, Pen pen)
        {
            _graphics = graphics;
            _pen = pen;
        }

        public void ChangePen(Pen pen) => _pen = pen;

        public void StartDrawing(Point previousPoint)
        {
            _isDrawing = true;
            _previousPoint = previousPoint;
        }
        public void Drawing(Point currentPoint)
        {
            if (_isDrawing == false) return;
            _graphics.DrawLine(_pen, _previousPoint, currentPoint);
            _previousPoint = currentPoint;
        }
        public void StopDrawing() => _isDrawing = false;
        
    }
}
