# galaga-the-game

A basic spaceship shooting game implemented **without** using any third party tools, assets, and codes.

Created with **Unity 2019.2.18f1 Personal, DirectX11**

Development build designed to work on Windows 10 x64

## Goals

Survive and gain as much point as possible with 3 available lives

## Basic Features

- Player starts with 3 lives, if the player is **killed while having 0 life**, the game ends.
- Each time the player clears a whole enemy fleet, the new fleet will fly in with even **greater speed**.
- There are 3 types of enemies; blue, red, and green.
- **Blue** enemies can **chase player**, **red** enemies can **shoot rockets**, and **green** enemies can shoot **continuous laser beams**.
- Each enemies has **chances** to spawn **a power up** that boosts player's shooting speed.
- After the game ends, player can press space bar to restart the game.

## Controls

- Press **A/D** or **left/right** arrow to steer the player's ship
- Tap or hold **space bar** to shoot projectiles

## Adjustable Variables

1. Clone this project to your machine and open it in the Unity Editor (preferably 2019.2.18f1 or later version).
2. Navigate to the scene windows, left click to select a GameObject `Game Manager`. You'll see an Inspector window shown up.
3. Adjust the value in the `Game Manager` as you wish.

### Adjust the Number of Rockets

- Type in the new value in `Number Of Rockets` text field under the script `Manager Script` in the Inspector window.

### Adjust the Number of Player Life

- Type in the new value in `Lives` text field under the script `Player Life` in the Inspector window.

### Adjust the Overall Speed of Enemies

- Type in the new value in `Enemy Speed Mult` text field under the script `Manager Script` in the Inspector window.
- **Please note** that the maximum speed multiplier handled is `2.5`; increasing the speed more than 2.5 might cause unexpected behaviors.