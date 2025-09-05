using Godot;
using System;
using System.Linq;

public partial class Hex : Node2D
{
  private const float V = 32.0f;

  public readonly int Q;
  public readonly int R;
  public readonly int S;
  private readonly HexMap _hexMap;
  readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;
  public static float radius = V;

  public Texture2D HexTexture;
  private Sprite2D hexSprite;
  private Control labelContainer;
  private Label coordLabel;
  public Color fillColor = Colors.White;

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
    AddChild(CreateCenterTextControl($"{Q},{R}"));  // Debug: Show hex coordinates - TODO: Toggle via debug setting?
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
    float angle_offset = Mathf.Pi / 6; // Pointy top
    for (int i = 0; i < 6; i++)
    {
      float angle = angle_offset + i * Mathf.Pi / 3;
      points[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
    }
    DrawPolygon(points, Enumerable.Repeat(fillColor, 6).ToArray());

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
