
IMPORTANT: CREATE THE FOLDERS: Assets/Resources/Levels/LastPlayMode/

HIERARCHY:

'Tilemap3D Editor' is the GameObject that has the two important scripts

Script1: TileMapEditor3D : Brush, PlacingObj, RemovingObj, Saving, CameraController.
Script2: RecreateTilemapInEditMode: Reload last playmode interactions in EditMode.

TAGS: Block, Prop, BrushTile
LAYER: BrushTile

INSPECTOR:

The only Inspector window that exists is located on the 'Canvas / Inventory' object. Place the mouse over all the variables shown to get tips and guidelines (tooltips)

SCRIPT:

Script1: TileMapEditor3D: If you want to modify  all the matrix size (1x1x1 to 0.5fx 0.5fx 0.5f)...see
the lines: 38-39-40-272-330


HOW TO USE:

1 - Add your Prefabs inside varialve TileList on TileMapEditor3D script on Inspector(Tilemap3DEditor GameObject)

2 - Enter PlayMode

3 - Create you Level. (LeftMouseClick > Place | RightMouseClick > Remove)
Moving Camera: W S A D + Shift > Move or Rotate Camera View

4 - Click on SAVE button BEFORE EXIT PLAY MODE

5 - Click on button Reload Level in RecreateTilemapInEditMode script on Inspector(Tilemap3DEditor GameObject)

6 - DONE!

7 - CTRL + S !


A tip: You can save all .save files for backup purposes.

#END