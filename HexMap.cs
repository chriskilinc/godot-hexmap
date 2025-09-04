using Godot;
using System;

public partial class HexMap : Node2D
{
  public readonly int numRows = 20;
  public readonly int numCols = 40;
  public bool allowWrapAroundEastWest = true;

  public Hex[,] Hexes;

  public HexMap()
  {
    Hexes = new Hex[numCols, numRows];
    for (int q = 0; q < numCols; q++)
    {
      for (int r = 0; r < numRows; r++)
      {
        var hex = new Hex(q, r, this);
        // Calculate position for pointy topped hexes
        float x = hex.HexHorizontalSpacing() * (q + r * 0.5f);
        float y = Hex.HexVerticalSpacing() * r;
        hex.Position = new Vector2(x, y);
        AddChild(hex);
        Hexes[q, r] = hex;
      }
    }
  }
}
