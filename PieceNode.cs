using Godot;
using System;

internal enum PieceType // Interface Terrain?
{
    Landmine,
    Spy,
    Scout,
    Engineer,
    Sergeant,
    Lieutenant,
    Captain,
    Commandant,
    Colonel,
    BrigadierGeneral,
    CommanderInChief,
    Flag
}

internal enum Team
{
    Red,
    Blue
}

public partial class PieceNode : Node2D
{
    private Team _team;

    // consider changing team to class and exposing in godot ui?
    private static Color red1 = new Color((float)0.6745, (float)0.1961, (float)0.1961, (float)1);
    private static Color red2 = new Color((float)0.3945, (float)0.0997, 0, 1);
    private static Color red3 = new Color((float)0.1086, (float)0.0107, (float)0.0109, 1);

    private static Color blue1 = new Color((float)0.2020, (float)0.1940, (float)0.67, (float)1);
    private static Color blue2 = new Color((float)0.0060, 0, (float)0.39, 1);
    private static Color blue3 = new Color((float)0.013, (float)0.011, (float)0.11, 1);

    [Export]
    internal Team Team {
        get 
        {
            return _team;
        }

        set
        {
            _team = value;

            if (_team == Team.Red)
            {
                //var propertyList = _material.GetPropertyList();
                //var color1 = _material.Get("shader_parameter/color1");

                _material.Set("shader_parameter/color1", red1);
                _material.Set("shader_parameter/color2", red2);
                _material.Set("shader_parameter/color3", red3);
            }
            else
            {
                _material.Set("shader_parameter/color1", blue1);
                _material.Set("shader_parameter/color2", blue2);
                _material.Set("shader_parameter/color3", blue3);
            }
        }
    }

    private PieceType _pieceType;

    private Sprite2D _sprite;

    ShaderMaterial _material;

    [Export]
    internal PieceType PieceType { 
        get 
        { 
            return _pieceType; 
        }
        
        set 
        { 
            _pieceType = value; 
            _control.TooltipText = $"{value} ({(int)value})"; // TODO only show tooltip if piece is 'exposed'

            switch (_pieceType) 
            {
                case PieceType.Flag:
                case PieceType.Landmine:
                    Range = 0;
                    break;

                case PieceType.Scout: 
                    Range = 10;
                    break;

                default: 
                    Range = 1;
                    break;
            }
        }
    }

    [Export]
    public int Range;

    private Control _control;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _control = GetNode<Control>("Control");

        _sprite = GetNode<Sprite2D>("Sprite2D");

        // Create an instance of the shader unique to this node.
        // This prevents changes to the shader being applied to all pieces at once.
        _material = _sprite.Material.Duplicate() as ShaderMaterial;
        _sprite.Material = _material;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    // Todo set shader colors?
}
