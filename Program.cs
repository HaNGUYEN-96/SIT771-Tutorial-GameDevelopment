using SplashKitSDK;
using System;
using System.Collections.Generic;

// Base class for all game objects
public abstract class GameObject
{
    protected Sprite _sprite;

    public GameObject(string bitmapName, string bitmapFile)
    {
        SplashKit.LoadBitmap(bitmapName, bitmapFile);
        _sprite = new Sprite(bitmapName, bitmapFile);
    }

    public virtual void Draw()
    {
        _sprite.Draw();
    }

    public virtual void Update() { }

    public virtual bool CollidesWith(GameObject other)
    {
        return SplashKit.SpriteCollision(this._sprite, other._sprite);
    }

    public float X
    {
        get { return _sprite.X; }
        set { _sprite.X = value; }
    }

    public float Y
    {
        get { return _sprite.Y; }
        set { _sprite.Y = value; }
    }

    public int Width => _sprite.Width;
    public int Height => _sprite.Height;
}

// Player class inherits from GameObject
public class Player : GameObject
{
    private const float Gravity = 0.5f;
    public const float JumpStrength = -15f;
    private float _yVelocity;

    public Player() : base("doodle", "kangaroo.png")
    {
        X = DoodleJumpGame.WindowWidth / 2;
        Y = DoodleJumpGame.WindowHeight / 2;
        _yVelocity = 0;
    }

    public void ApplyGravity()
    {
        _yVelocity += Gravity;
        Y += _yVelocity;
    }

    public void Jump(float strength = JumpStrength)
    {
        _yVelocity = strength;
    }

    public void MoveLeft()
    {
        X -= 5;
    }

    public void MoveRight()
    {
        X += 5;
    }

    public bool IsFalling => _yVelocity > 0;

    public override void Update()
    {
        // Apply player bounds to stay inside the window
        if (X < 0) X = 0;
        if (X > DoodleJumpGame.WindowWidth - Width) X = DoodleJumpGame.WindowWidth - Width;
    }

    public bool IsOffScreen()
    {
        return Y > DoodleJumpGame.WindowHeight;
    }
}

// Platform class inherits from GameObject
public class Platform : GameObject
{
    public Platform(float x, float y) : base("platform", "platform.png")
    {
        X = x;
        Y = y;
    }

    public bool CanBounce(Player player)
    {
        return player.Y < Y && player.CollidesWith(this) && player.IsFalling;
    }

    public override void Update()
    {
        if (Y > DoodleJumpGame.WindowHeight)
        {
            X = SplashKit.Rnd(DoodleJumpGame.WindowWidth - Width);
            Y = -Height;
        }
    }
}

// Enemy class inherits from GameObject
public class Enemy : GameObject
{
    public Enemy(float x, float y) : base("enemy", "enemy.png")
    {
        X = x;
        Y = y;
    }

    public override void Update()
    {
        if (Y > DoodleJumpGame.WindowHeight)
        {
            X = SplashKit.Rnd(DoodleJumpGame.WindowWidth - Width);
            Y = -Height;
        }
    }
}

// Power-up class inherits from GameObject
public class PowerUp : GameObject
{
    public PowerUp(float x, float y) : base("spring", "spring.png")
    {
        X = x;
        Y = y;
    }

    public void ApplyPowerUp(Player player)
    {
        player.Jump(Player.JumpStrength * 2); // Boost the player's jump strength
    }

    public override void Update()
    {
        if (Y > DoodleJumpGame.WindowHeight)
        {
            X = SplashKit.Rnd(DoodleJumpGame.WindowWidth - Width);
            Y = -Height;
        }
    }
}

// Game Manager class
public class DoodleJumpGame
{
    public const int WindowWidth = 400;
    public const int WindowHeight = 600;

    private Player _player;
    private List<Platform> _platforms;
    private List<Enemy> _enemies;
    private List<PowerUp> _powerUps;

    private float _viewOffset;
    private int _score;
    private bool _gameOver;

    public DoodleJumpGame()
    {
        // Create the window
        new Window("Doodle Jump", WindowWidth, WindowHeight);

        // Initialize player, platforms, enemies, and power-ups
        _player = new Player();
        _platforms = new List<Platform>();
        _enemies = new List<Enemy>();
        _powerUps = new List<PowerUp>();

        InitializePlatforms();
        InitializeEnemies();
        InitializePowerUps();

        // Initialize game variables
        _viewOffset = 0;
        _score = 0;
        _gameOver = false;
    }

    private void InitializePlatforms()
    {
        for (int i = 0; i < 5; i++)
        {
            var platform = new Platform(SplashKit.Rnd(WindowWidth - 80), WindowHeight - (i * 120));
            _platforms.Add(platform);
        }
    }

    private void InitializeEnemies()
    {
        if (SplashKit.Rnd() < 0.5)
        {
            // i is the number of enemies
            for (int i = 0; i < 1; i++)
            {
                var enemy = new Enemy(SplashKit.Rnd(WindowWidth - 80), SplashKit.Rnd(WindowHeight / 2));
                _enemies.Add(enemy);
            }
        }   
    }

    private void InitializePowerUps()
    {
        for (int i = 0; i < 2; i++)
        {
            var powerUp = new PowerUp(SplashKit.Rnd(WindowWidth - 80), SplashKit.Rnd(WindowHeight / 2));
            _powerUps.Add(powerUp);
        }
    }

    public void Run()
    {
        while (!SplashKit.QuitRequested())
        {
            SplashKit.ProcessEvents();
            if (!_gameOver)
            {
                Update();
            }
            Draw();
        }
    }

    private void Update()
    {
        _player.ApplyGravity();
        _player.Update();

        // Player movement
        if (SplashKit.KeyDown(KeyCode.LeftKey)) _player.MoveLeft();
        if (SplashKit.KeyDown(KeyCode.RightKey)) _player.MoveRight();

        // Check platform collisions
        foreach (var platform in _platforms)
        {
            platform.Update();
            if (platform.CanBounce(_player))
            {
                _player.Jump();
                _score += 10;
            }
        }

        // Check enemy collisions
        foreach (var enemy in _enemies)
        {
            enemy.Update();
            if (_player.CollidesWith(enemy))
            {
                GameOver();
            }
        }

        // Check power-up collisions
        foreach (var powerUp in _powerUps)
        {
            powerUp.Update();
            if (_player.CollidesWith(powerUp))
            {
                powerUp.ApplyPowerUp(_player);
                _powerUps.Remove(powerUp);
                break;
            }
        }

        // Scroll the view when the player moves up
        if (_player.Y < WindowHeight / 2)
        {
            _viewOffset = WindowHeight / 2 - _player.Y;
            foreach (var platform in _platforms) platform.Y += _viewOffset;
            foreach (var enemy in _enemies) enemy.Y += _viewOffset;
            foreach (var powerUp in _powerUps) powerUp.Y += _viewOffset;
            _player.Y = WindowHeight / 2;
        }

        // Check if player is off-screen (falling below)
        if (_player.IsOffScreen())
        {
            GameOver();
        }
    }

    private void Draw()
    {
        SplashKit.ClearScreen(Color.White);

        if (_gameOver)
        {
            SplashKit.DrawText("Game Over", Color.Red, WindowWidth / 2 - 50, WindowHeight / 2);
            SplashKit.DrawText("Press 'R' to Restart", Color.Black, WindowWidth / 2 - 70, WindowHeight / 2 + 40);
        }
        else
        {
            _player.Draw();

            foreach (var platform in _platforms)
            {
                platform.Draw();
            }

            foreach (var enemy in _enemies)
            {
                enemy.Draw();
            }

            foreach (var powerUp in _powerUps)
            {
                powerUp.Draw();
            }

            SplashKit.DrawText($"Score: {_score}", Color.Black, 10, 10);
        }

        SplashKit.RefreshScreen(60);

        // Restart the game if 'R' is pressed
        if (_gameOver && SplashKit.KeyTyped(KeyCode.RKey))
        {
            RestartGame();
        }
    }

    private void GameOver()
    {
        _gameOver = true;
    }

    private void RestartGame()
    {
        _player = new Player();
        _platforms.Clear();
        _enemies.Clear();
        _powerUps.Clear();
        InitializePlatforms();
        InitializeEnemies();
        InitializePowerUps();
        _viewOffset = 0;
        _score = 0;
        _gameOver = false;
    }
}

// Entry point
public static class Program
{
    public static void Main()
    {
        DoodleJumpGame game = new DoodleJumpGame();
        game.Run();
    }
}
