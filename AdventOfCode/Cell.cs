using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Cell
{
    public bool isStaticObstacle = false;
    public int x, y;
    public char symbol;
    public (int,int) pos => (x,y);

    public Cell(int x, int y, char symbol)
    {
        this.x = x;
        this.y = y;
        this.symbol = symbol;
    }

    public override string ToString()
    {
        return $"{symbol} ({x}, {y})";
    }

    public virtual void Reset() { }


}