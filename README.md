# SIT771-Tutorial-GameDevelopment
In this tutorial, we will create a simple version of the "Doodle Jump" game using the SplashKit SDK in C#. This game features a player character that jumps between platforms, avoids enemies, and collects power-ups while aiming for a high score.
*Created br Thai Ha NGUYEN, Deakin University.*

### Game Overview

**Game Objective:** The player controls a character that jumps between platforms, collects power-ups, and avoids falling off the screen. The goal is to achieve the highest score possible.

### Concepts Covered

- **Game Structure**: Understanding how game components interact.
- **Classes and Inheritance**: Using object-oriented programming to create reusable game object classes.
- **Collision Detection**: Implementing methods to check for interactions between objects.
- **Game Loop**: Managing the continuous flow of the game.
- **User Input**: Handling keyboard input to control the player character.

## Prerequisites

- **C# Knowledge**: Basic understanding of C# programming.
- **Game Development Concepts**: Familiarity with game loops, classes, and object-oriented programming.
- **SplashKit SDK**: Ensure you have the SplashKit SDK installed and set up in your C# development environment. Visit the [SplashKit website](https://splashkit.io/) for installation instructions.

## Game Setup

### Walkthrough for Creating Doodle Jump in C#
![image](https://github.com/user-attachments/assets/a09cd455-31cd-4afd-a4ff-6d64392c5d22)
---

### Step 1: Set Up Your Development Environment

1. **Install Visual Studio**:
   - Download and install [Visual Studio](https://visualstudio.microsoft.com/downloads/). Choose the Community edition, which is free.

2. **Install SplashKit SDK**:
   - Download the SplashKit SDK from [SplashKit's website](https://splashkit.net/downloads).
   - Follow the installation instructions specific to your operating system.

3. **Create a New Project**:
   - Open Visual Studio and create a new Console Application project.
   - Name your project (e.g., `DoodleJump`).

4. **Add SplashKit Reference**:
   - Right-click on the project in Solution Explorer and select "Manage NuGet Packages."
   - Search for `SplashKit` and install it.

---

### 1. Create the Game Classes

We will create multiple classes to represent the various objects in the game.

#### GameObject Class

This is the base class for all game objects. It handles the common properties and behaviors of game entities.

```csharp
using SplashKitSDK;

public abstract class GameObject
{
    protected Sprite _sprite; // The sprite representing this game object

    // Constructor to load the bitmap and initialize the sprite
    public GameObject(string bitmapName, string bitmapFile)
    {
        SplashKit.LoadBitmap(bitmapName, bitmapFile);
        _sprite = new Sprite(bitmapName, bitmapFile);
    }

    // Draw the sprite on the screen
    public virtual void Draw()
    {
        _sprite.Draw();
    }

    // Update method to be overridden in derived classes
    public virtual void Update() { }

    // Check for collision with another game object
    public virtual bool CollidesWith(GameObject other)
    {
        return SplashKit.SpriteCollision(this._sprite, other._sprite);
    }

    // Properties for accessing the sprite's position and dimensions
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
```

#### Explanation

- **Sprite**: Represents the visual element of the game object. We load a bitmap image that will be displayed on the screen.
- **Collision Detection**: The `CollidesWith` method uses the SplashKit functionality to check if this game object collides with another.

#### Player Class

This class inherits from `GameObject` and manages the player's actions, including movement and jumping.

```csharp
public class Player : GameObject
{
    private const float Gravity = 0.5f; // The force of gravity affecting the player
    public const float JumpStrength = -15f; // The strength of the player's jump
    private float _yVelocity; // The current vertical velocity of the player

    // Constructor initializes the player position and sprite
    public Player() : base("doodle", "kangaroo.png")
    {
        X = DoodleJumpGame.WindowWidth / 2; // Start in the middle of the screen
        Y = DoodleJumpGame.WindowHeight / 2; // Start at the center vertically
        _yVelocity = 0; // Initial vertical velocity
    }

    // Apply gravity to the player
    public void ApplyGravity()
    {
        _yVelocity += Gravity; // Increase vertical velocity
        Y += _yVelocity; // Update position based on velocity
    }

    // Jump method to give the player an upward velocity
    public void Jump(float strength = JumpStrength)
    {
        _yVelocity = strength; // Set the upward velocity
    }

    // Move the player to the left
    public void MoveLeft()
    {
        X -= 5; // Move 5 units left
    }

    // Move the player to the right
    public void MoveRight()
    {
        X += 5; // Move 5 units right
    }

    // Check if the player is falling
    public bool IsFalling => _yVelocity > 0;

    // Update the player position and handle screen boundaries
    public override void Update()
    {
        if (X < 0) X = 0; // Prevent moving off the left edge
        if (X > DoodleJumpGame.WindowWidth - Width) X = DoodleJumpGame.WindowWidth - Width; // Prevent moving off the right edge
    }

    // Check if the player has fallen off the screen
    public bool IsOffScreen()
    {
        return Y > DoodleJumpGame.WindowHeight; // Check if the Y position exceeds the window height
    }
}
```

#### Explanation

- **Gravity**: The `ApplyGravity` method simulates the downward force affecting the player, increasing their downward velocity over time.
- **Movement**: The `MoveLeft` and `MoveRight` methods allow the player to move horizontally across the screen.
- **Jumping**: The `Jump` method sets an upward velocity to simulate jumping.

#### Platform Class

This class represents the platforms the player can jump on.

```csharp
public class Platform : GameObject
{
    // Constructor initializes the platform's position
    public Platform(float x, float y) : base("platform", "platform.png")
    {
        X = x;
        Y = y;
    }

    // Check if the player can bounce off this platform
    public bool CanBounce(Player player)
    {
        return player.Y < Y && player.CollidesWith(this) && player.IsFalling;
    }

    // Update the platform's position
    public override void Update()
    {
        if (Y > DoodleJumpGame.WindowHeight) // If the platform goes off-screen
        {
            X = SplashKit.Rnd(DoodleJumpGame.WindowWidth - Width); // Randomize the X position
            Y = -Height; // Reset to the top of the screen
        }
    }
}
```

#### Explanation

- **Bounce Detection**: The `CanBounce` method checks if the player is falling onto the platform, allowing them to bounce off.
- **Platform Reset**: The `Update` method randomizes the position of the platform when it moves off-screen, creating a continuous gameplay experience.

#### Enemy Class

Represents the enemies in the game.

```csharp
public class Enemy : GameObject
{
    // Constructor initializes the enemy's position
    public Enemy(float x, float y) : base("enemy", "enemy.png")
    {
        X = x;
        Y = y;
    }

    // Update the enemy's position
    public override void Update()
    {
        if (Y > DoodleJumpGame.WindowHeight) // If the enemy goes off-screen
        {
            X = SplashKit.Rnd(DoodleJumpGame.WindowWidth - Width); // Randomize the X position
            Y = -Height; // Reset to the top of the screen
        }
    }
}
```

#### Explanation

- Similar to the `Platform` class, the `Enemy` class resets its position when it moves off-screen, allowing for a continuous challenge for the player.

#### PowerUp Class

Represents power-ups that boost the player's jump strength.

```csharp
public class PowerUp : GameObject
{
    // Constructor initializes the power-up's position
    public PowerUp(float x, float y) : base("spring", "spring.png")
    {
        X = x;
        Y = y;
    }

    // Apply the power-up to the player
    public void ApplyPowerUp(Player player)
    {
        player.Jump(Player.JumpStrength * 2); // Boost the player's jump strength
    }

    // Update the power-up's position
    public override void Update()
    {
        if (Y > DoodleJumpGame.WindowHeight) // If the power-up goes off-screen
        {
            X = SplashKit.Rnd(DoodleJumpGame.WindowWidth - Width); // Randomize the X position
            Y = -Height; // Reset to the top of the screen
        }
    }
}
```

#### Explanation

- The `ApplyPowerUp` method doubles the jump strength of the player when they collect the power-up, providing an advantage during gameplay.

### 2. Create the Game Manager Class

This class manages the game loop, updates, and rendering. It also initializes game objects and handles game state.

```csharp
public class DoodleJumpGame
{
    public const int WindowWidth = 400; // Width of the game window
    public const int WindowHeight = 600; // Height of the game window

    private Player _player; // Player instance
    private List<Platform> _platforms; // List of platforms
    private List<Enemy> _enemies; // List of enemies
    private List<PowerUp> _powerUps; // List of power-ups

    private float _viewOffset; // Offset for scrolling
    private int _score; // Player's score
    private bool _game

Over; // Game over flag

    public DoodleJumpGame()
    {
        SplashKit.OpenWindow("Doodle Jump", WindowWidth, WindowHeight);
        _player = new Player();
        _platforms = new List<Platform>();
        _enemies = new List<Enemy>();
        _powerUps = new List<PowerUp>();
        InitializePlatforms(); // Create initial platforms
        InitializeEnemies(); // Create initial enemies
        InitializePowerUps(); // Create initial power-ups
    }

    private void InitializePlatforms()
    {
        for (int i = 0; i < 10; i++) // Create 10 platforms
        {
            float x = SplashKit.Rnd(WindowWidth - 100); // Random X position
            float y = SplashKit.Rnd(WindowHeight - 100); // Random Y position
            _platforms.Add(new Platform(x, y)); // Add platform to the list
        }
    }

    private void InitializeEnemies()
    {
        for (int i = 0; i < 5; i++) // Create 5 enemies
        {
            float x = SplashKit.Rnd(WindowWidth - 50); // Random X position
            float y = SplashKit.Rnd(WindowHeight - 200); // Random Y position
            _enemies.Add(new Enemy(x, y)); // Add enemy to the list
        }
    }

    private void InitializePowerUps()
    {
        for (int i = 0; i < 3; i++) // Create 3 power-ups
        {
            float x = SplashKit.Rnd(WindowWidth - 50); // Random X position
            float y = SplashKit.Rnd(WindowHeight - 200); // Random Y position
            _powerUps.Add(new PowerUp(x, y)); // Add power-up to the list
        }
    }

    public void Run()
    {
        while (!SplashKit.WindowCloseRequested("Doodle Jump")) // Game loop
        {
            SplashKit.ProcessEvents(); // Handle user input
            Update(); // Update game state
            Draw(); // Render game objects
            SplashKit.RefreshScreen(60); // Refresh the screen at 60 FPS
        }
    }

    private void Update()
    {
        if (_gameOver) return; // If the game is over, skip updates

        _player.ApplyGravity(); // Apply gravity to the player

        // Handle user input for movement and jumping
        if (SplashKit.KeyDown(KeyCode.LeftKey)) _player.MoveLeft();
        if (SplashKit.KeyDown(KeyCode.RightKey)) _player.MoveRight();
        if (SplashKit.KeyDown(KeyCode.Space)) _player.Jump();

        foreach (var platform in _platforms)
        {
            if (platform.CanBounce(_player)) // Check if the player can bounce
            {
                _player.Jump();
            }
        }

        // Check for collisions with enemies
        foreach (var enemy in _enemies)
        {
            if (enemy.CollidesWith(_player))
            {
                GameOver(); // End the game if the player collides with an enemy
            }
        }

        // Check for collisions with power-ups
        foreach (var powerUp in _powerUps)
        {
            if (powerUp.CollidesWith(_player))
            {
                powerUp.ApplyPowerUp(_player); // Apply the power-up effect
                // Remove power-up after collection
                _powerUps.Remove(powerUp);
                break; // Exit loop to avoid modifying the list while iterating
            }
        }

        // Update all game objects
        foreach (var platform in _platforms) platform.Update();
        foreach (var enemy in _enemies) enemy.Update();
        foreach (var powerUp in _powerUps) powerUp.Update();

        // Check if the player has fallen off the screen
        if (_player.IsOffScreen())
        {
            GameOver(); // End the game if the player falls
        }
    }

    private void Draw()
    {
        SplashKit.ClearScreen(); // Clear the screen

        // Draw all game objects
        _player.Draw();
        foreach (var platform in _platforms) platform.Draw();
        foreach (var enemy in _enemies) enemy.Draw();
        foreach (var powerUp in _powerUps) powerUp.Draw();

        // Draw the score
        SplashKit.DrawText("Score: " + _score, Color.White, 10, 10); // Display the score
    }

    private void GameOver()
    {
        _gameOver = true; // Set game over flag
        SplashKit.DrawText("Game Over!", Color.Red, WindowWidth / 2 - 50, WindowHeight / 2); // Display game over message
        SplashKit.DrawText("Press R to Restart", Color.White, WindowWidth / 2 - 75, WindowHeight / 2 + 30); // Display restart message
    }

    private void Restart()
    {
        // Reset game state
        _player = new Player();
        _platforms.Clear();
        _enemies.Clear();
        _powerUps.Clear();
        InitializePlatforms();
        InitializeEnemies();
        InitializePowerUps();
        _score = 0;
        _gameOver = false;
    }
}
```

#### Explanation

- **Game Initialization**: The constructor initializes the game window and creates instances of the player, platforms, enemies, and power-ups.
- **Game Loop**: The `Run` method contains the main game loop that processes events, updates game states, and draws objects.
- **Updating Game State**: The `Update` method manages player movement, gravity, collision detection, and handles game logic such as scoring and game-over conditions.
- **Drawing Game Objects**: The `Draw` method clears the screen and renders all game objects along with the score.
- **Game Over Logic**: The `GameOver` method displays the game over message and handles the restart logic.

### 3. Create the Main Method

The `Main` method starts the game.

```csharp
public static class Program
{
    public static void Main()
    {
        DoodleJumpGame game = new DoodleJumpGame(); // Create an instance of the game
        game.Run();
    }
}
```

---

### 4: Run Your Game

1. **Build the Project**:
   - Click on "Build" in the menu and select "Build Solution."

2. **Run the Game**:
   - Press `Ctrl + F5` or select "Start Without Debugging" to run the game.

---

### Additional Features to Consider

- **Scoring System**: Implement a scoring system that increases as the player jumps on platforms or collects power-ups.
- **Levels and Progression**: Create different levels with varying difficulty, platforms, enemies, and power-ups.
- **Sound Effects**: Add sound effects for jumping, collecting power-ups, and game over.

---

### Congratulations! 

You've created a simple version of Doodle Jump using C# and SplashKit. You can expand on this base by adding more features and polishing the gameplay. Enjoy coding and have fun!
