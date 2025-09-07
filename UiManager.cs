using Godot;
using System;

public partial class UiManager : Control
{
  private Camera2D camera;
  private ColorRect spawnedBox;

  // TODO: remake everything
  public UiManager(Camera2D camera)
  {
    this.camera = camera;
    Name = "UiManager";
    UniqueNameInOwner = true;
    AnchorLeft = 0;
    AnchorTop = 0;
    AnchorRight = 1;
    AnchorBottom = 1;
    Position = Vector2.Zero;
  }

  public void DisplayHoverBox(Vector2 position, string text = "")
  {
    if (spawnedBox != null)
      FreeHoverBox();

    var box = new ColorRect();
    box.Color = new Color(0, 0, 0, 0.7f); // Black, semi-transparent
    box.Size = new Vector2(128, 64);
    box.Position = position;

    // Add gold border
    var border = new BorderControl();
    border.Size = box.Size;
    border.Position = Vector2.Zero;
    box.AddChild(border);

    // Add label
    var label = new Label();
    label.Text = text;
    label.HorizontalAlignment = HorizontalAlignment.Center;
    label.VerticalAlignment = VerticalAlignment.Center;
    label.SizeFlagsHorizontal = Control.SizeFlags.Expand;
    label.SizeFlagsVertical = Control.SizeFlags.Expand;
    label.AnchorLeft = 0;
    label.AnchorTop = 0;
    label.AnchorRight = 1;
    label.AnchorBottom = 1;
    label.AutowrapMode = TextServer.AutowrapMode.Word; // Enable word wrap
    box.AddChild(label);

    AddChild(box);
    spawnedBox = box;
  }

  public void FreeHoverBox()
  {
    if (spawnedBox != null && spawnedBox.IsInsideTree())
    {
      spawnedBox.QueueFree();
      spawnedBox = null;
    }
  }

  public void SpawnBoxInCameraCorner(string text = "")
  {
    var viewport = GetViewport();
    Vector2 boxSize = new Vector2(128, 64); // Match box size in SpawnBox
    Vector2 screenSize = viewport.GetVisibleRect().Size;
    Vector2 screenPos = new Vector2(16, screenSize.Y - boxSize.Y - 16); // 16px margin from bottom/left
    FreeHoverBox(); // Remove any existing box
    SpawnBox(screenPos, text); // Use screen coordinates for UI
  }

  private void SpawnBox(Vector2 position, string text)
  {
    var box = new ColorRect();
    box.Color = new Color(0, 0, 0, 0.7f); // Black, semi-transparent
    box.Size = new Vector2(128, 64);
    box.AnchorLeft = 0;
    box.AnchorTop = 0;
    box.AnchorRight = 0;
    box.AnchorBottom = 0;
    box.Position = position;

    // Add gold border
    var border = new BorderControl();
    border.Size = box.Size;
    border.Position = Vector2.Zero;
    box.AddChild(border);

    // Add label
    var label = new Label();
    label.Text = text;
    label.HorizontalAlignment = HorizontalAlignment.Center;
    label.VerticalAlignment = VerticalAlignment.Center;
    label.SizeFlagsHorizontal = Control.SizeFlags.Expand;
    label.SizeFlagsVertical = Control.SizeFlags.Expand;
    label.AnchorLeft = 0;
    label.AnchorTop = 0;
    label.AnchorRight = 1;
    label.AnchorBottom = 1;
    label.AutowrapMode = TextServer.AutowrapMode.Word; // Enable word wrap
    box.AddChild(label);

    AddChild(box);
    spawnedBox = box;
  }
}

// Custom control for gold border
public partial class BorderControl : Control
{
  public override void _Draw()
  {
    DrawRect(new Rect2(Vector2.Zero, Size), new Color(1.0f, 0.84f, 0.0f), false, 3.0f);
  }
}
