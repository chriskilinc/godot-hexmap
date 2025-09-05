using Godot;

public partial class GameManager : Node2D
{
  private Hex[,] hexes;
  private Hex lastSelectedHex;
  private HexMap hexMap;

  // Create a hex map and camera controls
  public override void _Ready()
  {
    hexMap = new HexMap();
    hexes = hexMap.hexes;

    var camera = new CameraControls(GetHexMapCenter());


    camera.Connect("WorldClicked", new Callable(this, nameof(OnWorldClickedEventListener)));
    camera.Connect("CameraMoved", new Callable(this, nameof(OnCameraMovedEventListener)));

    AddChild(hexMap);
    AddChild(camera);
  }

  // TODO: Should this be in HexMap?
  private void OnWorldClickedEventListener(Vector2 worldPos)
  {
    var clickedHex = GetHexByPosition(worldPos);

    GD.Print(clickedHex != null ? $"Clicked hex: Q={clickedHex.Q}, R={clickedHex.R}" : "Clicked outside of hex map bounds");
    if (clickedHex != null)
    {
      lastSelectedHex?.SetFillColor(Colors.White); // Reset previous
      clickedHex.SetFillColor(Colors.Yellow);      // Highlight new
      lastSelectedHex = clickedHex;
    }
  }

  // TODO: Should this be in HexMap?
  private void OnCameraMovedEventListener(Vector2 newPosition)
  {
    GD.Print($"Camera moved to: {newPosition}");
    hexMap.UpdateHexPositions(newPosition);
  }

  public Hex GetHexByPosition(Vector2 position)
  {
    // var clickedHex = hexes.Cast<Hex>().FirstOrDefault(hex => hex.Position.DistanceTo(worldPos) < Hex.radius);

    Hex foundHex = null;
    foreach (var hex in hexes)
    {
      if (hex.Position.DistanceTo(position) < Hex.radius)
      {
        foundHex = hex;
        break;
      }
    }
    return foundHex;
  }

  private Vector2 GetHexMapCenter()
  {
    var centerPos = Vector2.Zero;
    if (hexMap != null)
    {
      var centerHex = hexMap.hexes[hexMap.numCols / 2, hexMap.numRows / 2];
      centerPos = centerHex != null ? centerHex.Position : Vector2.Zero;
    }
    return centerPos;
  }
}
