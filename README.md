Unity Hex Grid System

A hex grid is different from a square grid because each tile has six neighbors and cannot be aligned in simple rows and columns. For this reason, hex grids require special coordinate systems and mathematical handling.

This system uses two coordinate representations together: offset coordinates for storage and cube coordinates for calculations.

Offset coordinates use a simple (x, y) format and are ideal for storing the grid inside a 2D array. This makes indexing and iteration easy and keeps the data structure simple.

Cube coordinates represent each hex tile with three values (x, y, z), with the rule:

x + y + z = 0

This representation makes distance calculation easy, rounding reliable, and conversions stable.

When placing hex tiles in the world, grid coordinates are converted into world positions using hex geometry. Each hex has a width based on the square root of three and a vertical spacing of seventy five percent of its height. Every second row is horizontally offset, which creates the classic pointy top hex layout.

The grid to world conversion follows this logic:

xPos = x * HexWidth + (y % 2 == 1 ? HexWidth * 0.5 : 0)
yPos = y * VertSpacing

To detect which hex tile the mouse is pointing at, the system performs the inverse operation. The mouse position in world space is converted into axial coordinates, then into cube coordinates. The cube values are rounded, and finally converted back into offset coordinates.

The world to grid conversion is based on these formulas:

q = (sqrt(3) / 3 * localX - 1 / 3 * localY) / cellSize
r = (2 / 3 * localY) / cellSize

Centering the grid is done by calculating the total width and height of the grid and applying half of that value as an offset. This ensures that the grid always spawns around the center of the GameObject instead of starting from a corner.

All interactions in the system are based purely on coordinate math. No colliders are used for tile detection. The grid logic works entirely through grid to world and world to grid conversions.

This project is a minimal implementation created to demonstrate how hex grids work internally in Unity, focusing on mathematical clarity rather than visual features.
