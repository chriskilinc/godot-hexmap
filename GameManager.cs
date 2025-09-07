using Godot;

public partial class GameManager : Node2D
{
  private Hex[,] hexes;
  private Hex lastSelectedHex;
  private Hex lastHoveredHex;
  private HexMap hexMap;
  // private UiManager uiManager;
  private CameraControls camera;

  // Create a hex map and camera controls
  public override void _Ready()
  {
    hexMap = new HexMap();
    hexes = hexMap.hexes;

    var camera = new CameraControls(GetHexMapCenter());
    this.camera = camera;

    camera.Connect("WorldClicked", new Callable(this, nameof(OnWorldClickedEventListener)));
    camera.Connect("CameraMoved", new Callable(this, nameof(OnCameraMovedEventListener)));
    camera.Connect("MouseHovered", new Callable(this, nameof(OnMouseHoveredEventListener)));

    // uiManager = new UiManager(camera);

    AddChild(hexMap);
    AddChild(camera);
    // AddChild(uiManager);
  }

  // TODO: Should this be in HexMap?
  private void OnWorldClickedEventListener(Vector2 worldPos)
  {
    var clickedHex = GetHexByPosition(worldPos);

    GD.Print(clickedHex != null ? $"Clicked hex: Q={clickedHex.Q}, R={clickedHex.R}" : "Clicked outside of hex map bounds");
    if (clickedHex != null)
    {
      clickedHex.SelectTile();
      lastSelectedHex?.DeselectTile();
      lastSelectedHex = clickedHex;
    }
  }

  // TODO: Should this be in HexMap?
  private void OnCameraMovedEventListener(Vector2 newPosition)
  {
    GD.Print($"Camera moved to: {newPosition}");
    hexMap.UpdateHexPositions(newPosition);
  }

  private void OnMouseHoveredEventListener(Vector2 worldPos)
  {
    var hoveredHex = GetHexByPosition(worldPos);
    if (hoveredHex != null && hoveredHex != lastHoveredHex)
    {
      GD.Print($"Hovering over hex: Q={hoveredHex.Q}, R={hoveredHex.R}");
      hoveredHex.HoverTile();
      // uiManager.DisplayHoverBox(hoveredHex.Position, $"Q={hoveredHex.Q}, R={hoveredHex.R} / Q={hoveredHex.Q}, R={hoveredHex.R}");
      camera.uiManager.SpawnBoxInCameraCorner($"Q={hoveredHex.Q}, R={hoveredHex.R} / Q={hoveredHex.Q}, R={hoveredHex.R}");
      lastHoveredHex?.UnhoverTile();
      lastHoveredHex = hoveredHex;
    }
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
