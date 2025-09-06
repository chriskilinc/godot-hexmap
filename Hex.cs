using Godot;
using System;
using System.Linq;

public partial class Hex : Node2D
{
  public bool debugMode = false;
  private const float V = 32.0f;

  public readonly int Q;
  public readonly int R;
  public readonly int S;
  private readonly HexMap _hexMap;
  readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;
  public static float radius = V;

  private Sprite2D hexSprite;
  private Control labelContainer;
  private Label coordLabel;
  public Color fillColor = Colors.White;

  public Texture2D hexTexture;
  public Texture2D grassTexture;
  public Texture2D grassTexture2;

  public Hex(int q, int r, HexMap hexMap)
  {
    Q = q;
    R = r;
    S = -Q - R;
    _hexMap = hexMap;

    float x = HexHorizontalSpacing() * (q + r * 0.5f);
    float y = HexVerticalSpacing() * r;
    Position = new Vector2(x, y);

    Name = $"Hex_{Q}_{R}";
  }

  public override void _Ready()
  {
    // The texture should be a square image with a hexagon centered and transparent corners.
    grassTexture = GD.Load<Texture2D>("res://Assets/Tiles/Terrain/Grass/grass_05.png");
    grassTexture2 = GD.Load<Texture2D>("res://Assets/Tiles/Terrain/Grass/grass_12.png");

    // Randomly assign one of the grass textures
    hexTexture = GD.Randf() < 0.5f ? grassTexture : grassTexture2;
    // hexTexture = GD.Load<Texture2D>("res://Assets/Tiles/Terrain/Grass/grass_05.png");

    if (debugMode)
    {
      AddChild(CreateCenterTextControl($"{Q},{R}"));  // Debug: Show hex coordinates - TODO: Toggle via debug setting?
    }
    QueueRedraw();
  }

  private Control CreateCenterTextControl(string text = "")
  {
    var labelContainer = new Control();
    labelContainer.Size = new Vector2(radius * 2, radius * 2);
    labelContainer.SetAnchorsPreset(Control.LayoutPreset.Center);
    labelContainer.Position = new Vector2(-radius, -radius);

    var coordLabel = new Label();
    coordLabel.Text = text;
    coordLabel.ZIndex = 1;
    coordLabel.SetAnchorsPreset(Control.LayoutPreset.Center, true);
    coordLabel.HorizontalAlignment = HorizontalAlignment.Center;
    coordLabel.VerticalAlignment = VerticalAlignment.Center;
    coordLabel.SizeFlagsHorizontal = Control.SizeFlags.Expand;
    coordLabel.SizeFlagsVertical = Control.SizeFlags.Expand;
    coordLabel.AnchorLeft = 0;
    coordLabel.AnchorTop = 0;
    coordLabel.AnchorRight = 1;
    coordLabel.AnchorBottom = 1;
    coordLabel.AddThemeFontSizeOverride("font_size", 16);
    coordLabel.AddThemeColorOverride("font_color", Colors.Black);
    labelContainer.AddChild(coordLabel);

    return labelContainer;
  }

  public override void _Draw()
  {
    //  Create hexagon points
    Vector2[] points = new Vector2[6];
    Vector2[] uvs = new Vector2[6];
    float angle_offset = Mathf.Pi / 6; // Pointy top

    float uvScale = 1f; // Shrink texture coverage
    for (int i = 0; i < 6; i++)
    {
      float angle = angle_offset + i * Mathf.Pi / 3;
      points[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
      // Map hex points to [0,1] UV space (centered), then scale toward center
      // The UV mapping assumes the hex fits perfectly inside the texture.
      uvs[i] = (points[i] * uvScale / (radius * 2)) + new Vector2(0.5f, 0.5f);
    }

    if (hexTexture != null)
    {
      DrawPolygon(points, null, uvs, hexTexture);
    }
    else
    {
      DrawPolygon(points, Enumerable.Repeat(fillColor, 6).ToArray());
    }

    DrawPolyline(points.Append(points[0]).ToArray(), Colors.Black, 1.0f); // Outline
  }

  public static float HexHeight()
  {
    return radius * 2;
  }

  public float HexWidth()
  {
    return WIDTH_MULTIPLIER * HexHeight();
  }

  public static float HexVerticalSpacing()
  {
    return HexHeight() * 0.75f;
  }

  public float HexHorizontalSpacing()
  {
    return HexWidth();
  }

  public Vector2 PositionFromCamera(Vector2 cameraPosition, float numRows, float numCols)
  {
    Vector2 position = Position;
    float mapWidth = numCols * HexHorizontalSpacing();

    if (_hexMap.allowWrapAroundEastWest)
    {
      float wfc = (position.X - cameraPosition.X) / mapWidth;

      if (wfc > 0)
      {
        wfc += 0.5f;
      }
      else
      {
        wfc -= 0.5f;
      }

      int wtf = (int)wfc;
      position.X -= wtf * mapWidth;
    }

    return position;
  }

  public void UpdatePosition(Vector2 cameraPosition)
  {
    if (_hexMap == null)
    {
      GD.Print("HexMap is not initialized.");
      return;
    }

    Vector2 position = PositionFromCamera(cameraPosition, _hexMap.numRows, _hexMap.numCols);
    Position = position;
  }

  public void SetFillColor(Color color)
  {
    fillColor = color;
    QueueRedraw();
  }
}
