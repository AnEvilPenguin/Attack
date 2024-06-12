using Godot;
using Serilog;
using System;

internal enum PieceType // Interface Terrain?
{
    Landmine,
    Spy,
    Scout,
    Engineer,
    Private,
    LanceCorporal,
    Corporal,
    Sergeant,
    Lieutenant,
    Captain,
    Commandant,
    Colonel,
    General
}

internal enum Team
{
    Red,
    Blue
}

internal enum AttackResult
{
    Victory,
    Defeat,
    Stalemate
}

public partial class PieceNode : Node2D
{
    private Team _team;

    // consider changing team to class and exposing in godot ui?
    private static Color _red1 = new Color(1, 0, 0, 1);
    private static Color red2 = new Color((float)0.3945, (float)0.0997, 0, 1);
    private static Color red3 = new Color((float)0.1086, (float)0.0107, (float)0.0109, 1);

    private static Color _blue1 = new Color(0, 0, 1, 1);

    [Export]
    internal Team Team {
        get 
        {
            return _team;
        }

        set
        {
            _team = value;

            setShaderColor();
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

            setTooltipText();

            switch (_pieceType) 
            {
                case PieceType.General:
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

    public bool Spotted = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _control = GetNode<Control>("Control");

        _sprite = GetNode<Sprite2D>("Sprite2D");

        // Create an instance of the shader unique to this node.
        // This prevents changes to the shader being applied to all pieces at once.
        _material = _sprite.Material.Duplicate() as ShaderMaterial;
        _sprite.Material = _material;

        setTooltipText();
        setShaderColor();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
    }

    private void setTooltipText()
    {
        if (_control == null)
            return;

        _control.TooltipText = $"{_pieceType} ({(int)_pieceType})"; // TODO only show tooltip if piece is 'exposed'
    }

    private void setShaderColor()
    {
        if(_material == null)
            return;

        if (_team == Team.Red)
        {
            //var propertyList = _material.GetPropertyList();
            //var color1 = _material.Get("shader_parameter/color1");

            _material.Set("shader_parameter/color1", _red1);
        }
        else
        {
            _material.Set("shader_parameter/color1", _blue1);
        }
    }

    internal AttackResult Attacks(PieceNode defender)
    {
        // TODO logic for showing icons and tooltips and the like (not here)
        Spotted = true;
        defender.Spotted = true;

        Log.Debug($"{PieceType} attacks {defender.PieceType}");

        switch (defender.PieceType)
        {
            case PieceType.Landmine:
                return PieceType == PieceType.Engineer ?
                    AttackResult.Victory :
                    AttackResult.Defeat;

            case PieceType.Colonel:
                return PieceType == PieceType.Spy ?
                    AttackResult.Victory :
                    AttackResult.Defeat;

            case PieceType.Spy:
                return PieceType == PieceType.Colonel ?
                    AttackResult.Defeat :
                    AttackResult.Victory;

            case PieceType.General:
                return AttackResult.Victory;

            default:
                var attackerValue = (int)PieceType;
                var defenderValue = (int)defender.PieceType;

                if (attackerValue == defenderValue)
                    return AttackResult.Stalemate;
                else if (attackerValue > defenderValue)
                    return AttackResult.Victory;

               return AttackResult.Defeat;
        }
    }
}
