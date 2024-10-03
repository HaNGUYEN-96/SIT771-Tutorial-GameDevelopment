# SIT771-Tutorial-GameDevelopment
This is a tutorial to create a game, using C# and Splashkit
### Walkthrough for Creating Doodle Jump in C#

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

### Step 2: Create Game Classes

You will need several classes to represent different game elements: `Player`, `Platform`, `Enemy`, `PowerUp`, and the main `DoodleJumpGame` class. Here’s how to set them up:

#### 1. Create the Player Class

Create a new class file named `Player.cs`.

```csharp
using SplashKitSDK;

public class Player
{
    private const float Gravity = 0.5f; // Gravity effect
    private const float JumpForce = 15.0f; // Jump force
    private const float Speed = 5.0f; // Player movement speed
    private float _x, _y; // Player position
    private float _velocityY; // Player vertical velocity

    public Player()
    {
        _x = 400; // Initial X position
        _y = 300; // Initial Y position
        _velocityY = 0; // Initial vertical velocity
    }

    public void ApplyGravity()
    {
        _velocityY += Gravity; // Increase downward velocity
        _y += _velocityY; // Update position based on velocity
    }

    public void MoveLeft() => _x -= Speed; // Move left
    public void MoveRight() => _x += Speed; // Move right

    public void Jump()
    {
        _velocityY = -JumpForce; // Apply jump force
    }

    public void Draw() => SplashKit.DrawCircle(Color.Blue, _x, _y, 20); // Draw player

    public bool IsOffScreen() => _y > 600; // Check if player is off the screen
}
```

#### 2. Create the Platform Class

Create another class file named `Platform.cs`.

```csharp
using SplashKitSDK;

public class Platform
{
    private float _x, _y; // Platform position
    private const float Width = 100; // Platform width
    private const float Height = 10; // Platform height

    public Platform(float x, float y)
    {
        _x = x;
        _y = y;
    }

    public void Draw() => SplashKit.FillRectangle(Color.Green, _x, _y, Width, Height); // Draw platform

    public bool CanBounce(Player player)
    {
        // Check if the player collides with the platform
        return (player.IsOffScreen() == false && player.X > _x && player.X < _x + Width && player.Y + 20 >= _y && player.Y + 20 <= _y + Height);
    }

    public void Update() { /* Add logic for platform behavior if needed */ }
}
```

#### 3. Create the Enemy Class

Create a class file named `Enemy.cs`.

```csharp
using SplashKitSDK;

public class Enemy
{
    private float _x, _y; // Enemy position

    public Enemy(float x, float y)
    {
        _x = x;
        _y = y;
    }

    public void Draw() => SplashKit.FillCircle(Color.Red, _x, _y, 20); // Draw enemy

    public bool CollidesWith(Player player)
    {
        // Check for collision with player
        return (player.X > _x - 20 && player.X < _x + 20 && player.Y > _y - 20 && player.Y < _y + 20);
    }

    public void Update() { /* Add logic for enemy behavior if needed */ }
}
```

#### 4. Create the PowerUp Class

Create a class file named `PowerUp.cs`.

```csharp
using SplashKitSDK;

public class PowerUp
{
    private float _x, _y; // Power-up position

    public PowerUp(float x, float y)
    {
        _x = x;
        _y = y;
    }

    public void Draw() => SplashKit.FillRectangle(Color.Yellow, _x, _y, 20, 20); // Draw power-up

    public bool CollidesWith(Player player)
    {
        // Check for collision with player
        return (player.X > _x - 10 && player.X < _x + 30 && player.Y > _y - 10 && player.Y < _y + 30);
    }

    public void ApplyPowerUp(Player player)
    {
        // Implement power-up effect (e.g., increase score, boost player, etc.)
    }

    public void Update() { /* Add logic for power-up behavior if needed */ }
}
```

---

### Step 3: Create the Game Logic

Create a new class file named `DoodleJumpGame.cs`.

```csharp
using System.Collections.Generic;
using SplashKitSDK;

public class DoodleJumpGame
{
    private const int WindowWidth = 800; // Window width
    private const int WindowHeight = 600; // Window height
    private Player _player; // Player instance
    private List<Platform> _platforms; // List of platforms
    private List<Enemy> _enemies; // List of enemies
    private List<PowerUp> _powerUps; // List of power-ups
    private bool _gameOver; // Game over flag
    private int _score; // Player score

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

        if (_player.IsOffScreen()) // Check if the player falls off the screen
        {
            GameOver(); // End the game if the player falls
        }
    }

    private void Draw()
    {
        SplashKit.ClearScreen(); // Clear the screen
        _player.Draw(); // Draw the player
        foreach (var platform in _platforms) platform.Draw(); // Draw all platforms
        foreach (var enemy in _enemies) enemy.Draw(); // Draw all enemies
        foreach (var powerUp in _powerUps) powerUp.Draw(); // Draw all power-ups
        // Display score and game over message if necessary
        SplashKit.DrawText($"Score: {_score}", Color.White, 10, 10); // Display the score
    }

    private void GameOver()
    {
        _gameOver = true; // Set game over flag
        SplashKit.DrawText("Game Over", Color.Red, WindowWidth / 2 - 50, WindowHeight / 2); // Display game over message
        SplashKit.RefreshScreen(); // Refresh the screen
        SplashKit.Delay(2000); // Wait for 2 seconds
    }
}
```

---

### Step 4: Create the Main Entry Point

Finally, create the main entry point for your game in `Program.cs`.

```csharp
class Program
{
    static void Main(string[] args)
    {
        DoodleJumpGame game = new DoodleJumpGame(); // Create an instance of the game
        game.Run(); // Start the game
    }
}
```

---

### Step 5: Run Your Game

1. **Build the Project**:
   - Click on "Build" in the menu and select "Build Solution."

2. **Run the Game**:
   - Press `Ctrl + F5` or select "Start Without Debugging" to run the game.

---

### Additional Features to Consider

- **Scoring System**: Implement a scoring system that increases as the player jumps on platforms or collects power-ups.
- **Levels and Progression**: Create different levels with varying difficulty, platforms, enemies, and power-ups.
- **Sound Effects**: Add sound effects for jumping, collecting power-ups, and game over.
- **Mobile Support**: If you're interested in mobile development, consider using a framework like Xamarin or Unity to create a mobile version of the game.

---

### Conclusion

Congratulations! You've created a simple version of Doodle Jump using C# and SplashKit. You can expand on this base by adding more features and polishing the gameplay. Enjoy coding and have fun!
