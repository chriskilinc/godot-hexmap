using Godot;

public partial class CameraControls : Camera2D
{
  [Signal]
  public delegate void WorldClickedEventHandler(Vector2 worldPos);
  [Signal]
  public delegate void CameraMovedEventHandler(Vector2 newPosition);

  [Export]
  public float Speed = 500f;
  [Export]
  public float ZoomStep = 0.1f;
  [Export]
  public float MinZoom = 0.5f;
  [Export]
  public float MaxZoom = 2.0f;
  [Export]
  public Vector2 oldPosition;

  [Export]
  public Vector2 StartPosition = Vector2.Zero;

  public CameraControls(Vector2 startPosition = new Vector2())
  {
    StartPosition = startPosition;
  }

  public override void _Ready()
  {
    Position = StartPosition;
  }


  public override void _Process(double delta)
  {
    Vector2 direction = Vector2.Zero;
    if (Input.IsActionPressed("camera_up") || Input.IsKeyPressed(Key.W))
      direction.Y -= 1;
    if (Input.IsActionPressed("camera_down") || Input.IsKeyPressed(Key.S))
      direction.Y += 1;
    if (Input.IsActionPressed("camera_left") || Input.IsKeyPressed(Key.A))
      direction.X -= 1;
    if (Input.IsActionPressed("camera_right") || Input.IsKeyPressed(Key.D))
      direction.X += 1;

    if (direction != Vector2.Zero)
    {
      direction = direction.Normalized();
      Position += direction * Speed * (float)delta;
    }

    CheckIfCameraMoved();
  }

  public override void _Input(InputEvent @event)
  {
    if (@event is InputEventKey keyEvent && keyEvent.Pressed)
    {
      if (keyEvent.Keycode == Key.Q)
      {
        ApplyZoom(Zoom + new Vector2(ZoomStep, ZoomStep));
      }
      else if (keyEvent.Keycode == Key.E)
      {
        ApplyZoom(Zoom - new Vector2(ZoomStep, ZoomStep));
      }
    }
    else if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
    {
      if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
      {
        ApplyZoom(Zoom + new Vector2(ZoomStep, ZoomStep));
      }
      else if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
      {
        ApplyZoom(Zoom - new Vector2(ZoomStep, ZoomStep));
      }
      else if (Input.IsActionPressed("mouse_left"))
      {
        Vector2 worldPos = GetGlobalMousePosition();
        EmitSignal(nameof(WorldClicked), worldPos);
      }
    }
  }

  private void ApplyZoom(Vector2 newZoom)
  {
    float clamped = Mathf.Clamp(newZoom.X, MinZoom, MaxZoom);
    Zoom = new Vector2(clamped, clamped);
  }

  private void CheckIfCameraMoved()
  {
    if (oldPosition == Position)
      return;

    oldPosition = Position;
    EmitSignal(nameof(CameraMoved), Position);
  }
}
