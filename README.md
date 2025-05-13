# ScootAR

An open-source Unity project based on the Coupled-Sim project and other assets.


## Licensing and Assets

All assets used in this project are either open source or have permissive licenses that allow redistribution.

This ensures the project remains fully distributable and reusable by other researchers.

## Research Use

This simulator was used in a VR user study evaluating:

- The effectiveness of AR navigation lines
- Pedestrian proximity warning systems
- The influence of pedestrian density on driving behavior
- Comparison of smartphone-based vs AR-based navigation

The environment and scripts can be reused or modified to suit other experimental setups.

## Installation


The Unity project is using Unity6.1 but was also tested on Unity 6.0.
The Unity project can be found under the name scooter6 wither download this file or the whole reposatory.
Open Unity Hub -> open from disc -> select the folder scooter6.
No further Installations are needed.

## Contained Assets:
- ardity 
- City People FREE Samples
- DLSS
- Electic Scooter Prop
- Free SpeedTrees Package
- i6 German - Free Engine Sound pack
- (Ian's Fire Pack)
- Impostors - runtime optimization
- Lemonity
- Logitech Gaming SDK
- Realistic Fences Pack
- Realistics Hands - animated for VR
- Rewired
- Scene Optimizer
- SteamVR Plugin
- Textures Free
- VR Panorama
- City props pack 
- Plastic Chair and Table Set
- Anime Girl Idle Animation
- HD Low Poly Racing Car No.1201
- VR Tunneling Pro
- Free General Ambient Sounds 
- 18 High Resolution Wall Textures
- This project is based on the coupled-Sim project that already incorporated even more assets

## General Connection and E-Scooter functionality
E-Scooter functionality can be found in scooter6/Assets/ARide/ARide
E-Scooter functionality:
- Use the script ArduinoReader, place it anywhere in your scene and select your used ports and pins to connect arduino to Unity
- Use the script EScooterNew on your E-Scooter and drag the object with this script into the public fields EScooterWheel and EScooterWheelOld in the ArduinoReader
AR-Warnings:
- Add the script DangerSignaling to your E-scooter.
- Add the prefab Warnings.prefab to your pedestrians in the scene hierarchy.

## Test Scene
start scooter6/Assets/ARide/ARide/_Scenes/BigCity/Ulm neue Mitte.unity
Either run Play to directly start into the scene (without loading traffic lights.


## Contact

For questions or collaboration requests, please contact:

Marcel Giss  
MGiss@gmx.net 

or 

Mark Colley  
https://m-colley.github.io/  
https://github.com/M-Colley
