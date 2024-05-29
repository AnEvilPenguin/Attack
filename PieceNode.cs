using Godot;
using System;

internal enum PieceType // Interface Terrain?
{
    Landmine,
    Spy,
    Scout,
    Engineer,
    Sergeant,
    Liutenant,
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
    [Export]
    internal Team Team;

    private PieceType _pieceType;

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
        }
    }

    [Export]
    public int Range;

    private Control _control;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _control = GetNode<Control>("Control");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
