using Godot;

public partial class HexMap : Node2D
{
  public readonly int numRows;
  public readonly int numCols;
  public bool allowWrapAroundEastWest = true;

  public Hex[,] hexes;

  public HexMap(int numRows = 20, int numCols = 40)
  {
    this.numRows = numRows;
    this.numCols = numCols;
    
    Name = "HexMap";
    UniqueNameInOwner = true;

    GenerateHexMap();
    // TODO: Implement map terrain generation
  }

  private void GenerateHexMap()
  {
    hexes = new Hex[numCols, numRows];
    for (int q = 0; q < numCols; q++)
    {
      for (int r = 0; r < numRows; r++)
      {
        var hex = new Hex(q, r, this);
        hexes[q, r] = hex;
        AddChild(hex);
      }
    }
  }

  public void UpdateHexPositions(Vector2 cameraPosition)
  {
    if (hexes == null || hexes.Length == 0)
      return;

    foreach (Hex hex in hexes)
    {
      hex?.UpdatePosition(cameraPosition);
    }
  }
}
