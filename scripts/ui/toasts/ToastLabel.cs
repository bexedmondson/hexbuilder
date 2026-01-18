using System;
using Godot;
using Godot.Collections;

public partial class ToastLabel : Label
{
	public Action<ToastLabel> RemoveLabelAction;

	public Vector2 Resolution = Vector2.Zero;


	// Text margin with box parent
	public Dictionary<string, float> Margins = new(){{"left", 12},{"top", 7},{"right", 12},{"bottom", 7},};

	// margin between buttons
	public const int MarginBetween = 23;
	
	// offset position box with screen position
	public Vector2 OffsetPosition = new Vector2(10, 10);

	public Vector2 ButtonSize;
	protected Tween _TweenIn;
	
	public ToastDirection Direction = ToastDirection.TopCenter;
	public int timerToDestroy = 2;


	// seconds by default
	public override void _Ready()
	{
		_SetResolution();
		ButtonSize = this.Size;


		// start position
		_TweenDestroyLabelTimer();
	}

	public void Init(ToastConfig config)
	{
		UpdateText(config.text);
		this.LabelSettings.FontColor = config.color;
		this.AddThemeColorOverride("bg_color", config.bgColor);
		this.LabelSettings.FontSize = config.textSize;

		Direction = config.direction;

		Position = new Vector2(Position.X, GetYPos( - 100, config.direction));

		_SetMargins();
		_SetShadowDirection();
	}
	
	public void UpdateText(string text)
	{
		this.Text = text;
		ButtonSize = this.Size;
		UpdateXPosition();
	}
	
	public void MoveTo(int index)
	{
		UpdateXPosition();

		float offset_y = (MarginBetween + ButtonSize.Y) * index;
		float _y = GetYPos(offset_y, Direction);
		
		// bottom
		if (index == 0)
		{
			_TweenIn = GetTree().CreateTween();
			_TweenIn.SetPauseMode(Tween.TweenPauseMode.Stop);
			// pause mode
			_TweenIn.Stop();

			_TweenIn.TweenProperty(this, "position", new Vector2(Position.X, _y), 0.3)
				.SetTrans(Tween.TransitionType.Quint)
				.SetEase(Tween.EaseType.In)
				.SetDelay(0.03);
			_TweenIn.Play();
		}
		else
		{
			
			_TweenIn = GetTree().CreateTween();
			_TweenIn.SetPauseMode(Tween.TweenPauseMode.Stop);
			// pause mode
			_TweenIn.Stop();

			_TweenIn.TweenProperty(this, "position", new Vector2(Position.X, _y), 0.3)
				.SetTrans(Tween.TransitionType.Elastic)
				.SetEase(Tween.EaseType.InOut);
			_TweenIn.Play();
		}
	}
	
	protected void _TweenDestroyLabelComplete()
	{
		// Send event complete
		RemoveLabelAction?.Invoke(this);
		QueueFree();
	}


	protected void _TweenDestroyLabelTimer()
	{

		// tween alpha to 0
		var tween_alpha = GetTree().CreateTween();
		tween_alpha.SetPauseMode(Tween.TweenPauseMode.Stop);
		// pause mode
		tween_alpha.TweenProperty(this, "modulate:a", 0, 0.8).SetDelay(timerToDestroy);
		tween_alpha.TweenCallback(Callable.From(_TweenDestroyLabelComplete));
	}


	public float GetYPos(float offset, ToastDirection direction)
	{

		// left position.x = margins.left + offset_position.x
		float _y_pos = 0;
		if(direction == ToastDirection.TopLeft || direction == ToastDirection.TopCenter || direction == ToastDirection.TopRight)
		{
			_y_pos = Margins["top"] + OffsetPosition.Y + offset;
		}
		else
		{
			_y_pos = Resolution.Y - Margins["top"] - ButtonSize.Y - OffsetPosition.Y - offset;
		}
		return _y_pos;
	}


	public void UpdateXPosition()
	{
		_SetResolution();

		if(Direction == ToastDirection.TopLeft || Direction == ToastDirection.BottomLeft)
		{
			Position = new Vector2(Margins["left"] + OffsetPosition.X, Position.Y);
		}
		else if(Direction == ToastDirection.TopCenter || Direction == ToastDirection.BottomCenter)
		{
			Position = new Vector2((Resolution.X / 2) - (Size.X / 2), Position.Y);
		}
		else
		{
			Position = new Vector2(Resolution.X - Margins["left"] - Size.X - OffsetPosition.X, Position.Y);
		}
	}

	protected void _SetMargins()
	{
		// set margins
		var theme_override = this.Get("theme_override_styles/normal");
		AddThemeConstantOverride("expand_margin_left", (int)Margins["left"]);
		/*theme_override.Set("expand_margin_left", Margins.Left);
		theme_override.Set("expand_margin_top", Margins.Top);
		theme_override.Set("expand_margin_right", Margins.Right);
		theme_override.Set("expand_margin_bottom", Margins.Bottom);*/
	}
	
	protected void _SetShadowDirection()
	{
		// set shadow direction
		var shadowOffsetAbs = 2;
		this.LabelSettings.ShadowOffset = Direction switch
		{
			ToastDirection.TopLeft => new Vector2(-shadowOffsetAbs, shadowOffsetAbs),
			ToastDirection.BottomLeft => new Vector2(-shadowOffsetAbs, shadowOffsetAbs),
			ToastDirection.TopCenter => new Vector2(0, shadowOffsetAbs),
			ToastDirection.BottomCenter => new Vector2(0, shadowOffsetAbs),
			ToastDirection.TopRight => new Vector2(shadowOffsetAbs, shadowOffsetAbs),
			ToastDirection.BottomRight => new Vector2(-shadowOffsetAbs, shadowOffsetAbs),
		};
	}


	protected void _SetResolution()
	{
		Resolution.X = GetViewport().GetVisibleRect().Size.X;
		Resolution.Y = GetViewport().GetVisibleRect().Size.Y;
	}
}