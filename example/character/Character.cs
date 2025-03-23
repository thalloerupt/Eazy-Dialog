using Godot;
using System;

public partial class Character : CharacterBody2D
{
	public const float Speed = 150.0f;
	public const float JumpVelocity = -400.0f;




	public override void _PhysicsProcess(double delta)
	{
		 Vector2 velocity = new Vector2();
		 AnimatedSprite2D animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        if (Input.IsActionPressed("ui_right"))
        {
			animatedSprite2D.Play("walk_right");

            velocity.X += 1;
			//animatedSprite2D.FlipH = false;

        }
        else if (Input.IsActionPressed("ui_left"))
        {
			animatedSprite2D.Play("walk_left");

            velocity.X -= 1;
			//animatedSprite2D.FlipH = true;

        }
        else if (Input.IsActionPressed("ui_down"))
        {
            velocity.Y += 1;
			animatedSprite2D.Play("walk");
        }
        else if (Input.IsActionPressed("ui_up"))
        {
            velocity.Y -= 1;
			animatedSprite2D.Play("walk_back");
        }
		else animatedSprite2D.Stop();

        // 归一化速度向量，防止斜向移动速度过快
        velocity = velocity.Normalized() * Speed;

		Velocity = velocity;
		MoveAndSlide();
	}
}
