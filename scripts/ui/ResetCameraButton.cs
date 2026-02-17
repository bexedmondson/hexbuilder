using Godot;

public partial class ResetCameraButton : Button
{
    public void OnResetButton()
    {
        var cameraController = InjectionManager.Get<MapCameraController>();
        cameraController.FlyToCell(Vector2I.Zero, 0.5f);
        cameraController.ResetZoom(0.5f);
    }
}
