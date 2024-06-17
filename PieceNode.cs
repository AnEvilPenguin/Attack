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

    private static Color _blue1 = new Color(0, 0, 1, 1);

    private static Texture2D _landmine = GD.Load<Texture2D>("res://Art/Chip01 - Mine.png");
    private static Texture2D _spy = GD.Load<Texture2D>("res://Art/Chip02 - Spy.png");
    private static Texture2D _scout = GD.Load<Texture2D>("res://Art/Chip03 - Scout.png");
    private static Texture2D _engineer = GD.Load<Texture2D>("res://Art/Chip04 - Engineer.png");
    private static Texture2D _private = GD.Load<Texture2D>("res://Art/Chip05 - Private.png");
    private static Texture2D _lanceCorporal = GD.Load<Texture2D>("res://Art/Chip06 - LanceCorporal.png");
    private static Texture2D _corporal = GD.Load<Texture2D>("res://Art/Chip07 - Corporal.png");
    private static Texture2D _sergeant = GD.Load<Texture2D>("res://Art/Chip08 - Sergeant.png");
    private static Texture2D _lieutenant = GD.Load<Texture2D>("res://Art/Chip09 - Lieutenant.png");
    private static Texture2D _captain = GD.Load<Texture2D>("res://Art/Chip10 - Captain.png");
    private static Texture2D _colonel = GD.Load<Texture2D>("res://Art/Chip11 - Colonel.png");
    private static Texture2D _general = GD.Load<Texture2D>("res://Art/Chip12 - General.png");

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

    private bool _spotted = false;
    public bool Spotted { 
        get => _spotted; 
        set 
        { 
            _spotted = value;

            if (_spotted)
            {
                setTooltipText();
                SetSprite();
                _material.Set("shader_parameter/exposed", true);
            }
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _control = GetNode<Control>("Control");

        _sprite = GetNode<Sprite2D>("Sprite2D");

        // Create an instance of the shader unique to this node.
        // This prevents changes to the shader being applied to all pieces at once.
        _material = _sprite.Material.Duplicate() as ShaderMaterial;
        _sprite.Material = _material;

        setShaderColor();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
    }

    internal void ConfigurePiece(Team team, PieceType pieceType)
    {
        Team = team;
        PieceType = pieceType;

        if (Team == Team.Blue) 
        { 
            SetSprite();
            setTooltipText();
        }
    }

    private void SetSprite()
    {
        if (_sprite == null)
            return;

        // In future I could probably just load/set the texture using piecetype if I rename the files
        switch (_pieceType)
        {
            case PieceType.Landmine:
                _sprite.Texture = _landmine;
                return;

            case PieceType.Scout:
                _sprite.Texture = _scout;
                return;

            case PieceType.Spy:
                _sprite.Texture = _spy;
                return;

            case PieceType.Engineer:
                _sprite.Texture = _engineer;
                return;

            case PieceType.Private:
                _sprite.Texture = _private;
                return;

            case PieceType.LanceCorporal:
                _sprite.Texture = _lanceCorporal;
                return;

            case PieceType.Corporal:
                _sprite.Texture = _corporal;
                return;

            case PieceType.Sergeant:
                _sprite.Texture = _sergeant;
                return;

            case PieceType.Lieutenant:
                _sprite.Texture = _lieutenant;
                return;

            case PieceType.Captain:
                _sprite.Texture = _captain;
                return;

            case PieceType.Colonel:
                _sprite.Texture = _colonel;
                return;

            case PieceType.General:
                _sprite.Texture = _general;
                return;
        }
    }

    private void setTooltipText()
    {
        if (_control == null)
            return;

        string text = _spotted ?
            $"{_pieceType} (Rank: {(int)_pieceType}) - Spotted" :
            $"{_pieceType} (Rank: {(int)_pieceType})";

        _control.TooltipText = text; // TODO only show tooltip if piece is 'exposed'
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

    internal AttackResult Attacks(PieceNode defender, bool testRun = false)
    {
        // TODO logic for showing icons and tooltips and the like (not here)
        if (!testRun)
        {
            Spotted = true;
            defender.Spotted = true;

            Log.Debug($"{PieceType} attacks {defender.PieceType}");
        }

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
