using Godot;
using System;

public partial class Hog : CharacterBody3D
{
	private Vector3 _axis = Vector3.Up.Normalized();
	// How fast the player moves in meters per second.
	[Export]
	public int Speed { get; set; } = 14;
	[Export]
	public float RotationSpeed { get; set; } = 0.01f;
	// The downward acceleration when in the air, in meters per second squared.
	[Export]
	public int FallAcceleration { get; set; } = 75;
	
	[Export]
	public float HorizontalAcceleration { get; set; } = 5.0f;
	
	[Export]
	public int DashTime { get; set; } = 10;
	
	[Export]
	public int DashDelay { get; set; } = 20;
	
	private Vector3 _targetVelocity = Vector3.Zero;
	
	public override void _PhysicsProcess(double delta)
	{
		float acceleration = 1.0f;

		var cam_basis = this.Basis;
		var movement_vec = Input.GetVector("move_left", "move_right", "move_forward", "move_back");

		var direction = cam_basis * new Vector3(movement_vec.X, 0, movement_vec.Y);

		direction.Y = 0;

		direction = direction.Normalized();

		if (direction != Vector3.Zero)
		{
			// Setting the basis property will affect the rotation of the node.
			GetNode<Node3D>("Pivot").Basis = Basis.LookingAt(direction);
		}
		if (Input.IsActionPressed("dash") && DashTime > 0)
		{
			acceleration = HorizontalAcceleration;
			DashTime -= 1;
			DashDelay = 0;
		}
		else
		{
			if(DashDelay == 20){
				DashTime = 10;
			}
			DashDelay += 1;
		}
		// Ground velocity
		_targetVelocity.X = direction.X * Speed * acceleration;
		_targetVelocity.Z = direction.Z * Speed * acceleration;
		// Vertical velocity
		if (!IsOnFloor()) // If in the air, fall towards the floor. Literally gravity
		{
			_targetVelocity.Y -= FallAcceleration * (float)delta;
		}
		// Moving the character
		Velocity = _targetVelocity;
		MoveAndSlide();
	}
}
