# ScootAR: E-Scooter AR Navigation Simulator

ScootAR is an open-source Unity-based e-scooter simulator designed to evaluate Augmented Reality (AR) visualizations for safer e-scooter navigation in urban environments. This project was developed for HCI research on micromobility safety and builds upon the [Coupled-Sim](https://github.com/bazilinskyy/coupled-sim) framework.


## Overview

With the increasing popularity of e-scooters and a corresponding rise in accidents, this simulator provides a controlled environment to study and develop safety interventions. ScootAR allows researchers to:

- Test AR navigation visualizations in a safe VR environment
- Evaluate pedestrian warning systems
- Study e-scooter rider behavior under varying urban traffic conditions
- Compare traditional navigation methods with AR-enhanced alternatives

## Research Background

This simulator was developed to address key research questions:
1. How effectively can AR visualizations support e-scooter riders in navigating urban environments?
2. Do AR-based pedestrian warnings influence e-scooter riders' driving behavior in terms of safety?
3. How do AR visualizations affect secondary factors such as mental workload, system acceptance, usability, and presence?

Our research found that AR-based navigation was rated significantly higher in usability, usefulness, and satisfaction compared to smartphone navigation.

## Hardware Setup

The simulator uses a modified real e-scooter with:
- Original thumb throttle controls rerouted to an Arduino UNO
- VIVE controller attached to the handlebar for steering tracking
- Support bars for stability during operation
- VR headset for immersion


## Software Features

### Visualization Systems
- **Navigation Line**: World-fixed blue arrows showing the route
- **Speedometer**: Color-coded display showing current speed (blue for safe, red for excessive)
- **Pedestrian Warning**: Barrier visualization that appears when approaching pedestrians (3.7m initial warning, 1.2m full effect)

### Urban Environment
- Procedurally generated city based on real OpenStreetMap data
- Realistic pedestrian behaviors and traffic flows
- Two city center areas with varying pedestrian density

### Traffic Simulation
- Realistic vehicle behavior
- Pedestrian path-following with obstacle avoidance
- Varying traffic density conditions

## Installation

1. The Unity project requires Unity 6.0 or 6.1
2. Download the repository or just the "scooter6" folder
3. In Unity Hub, select "Open from disk" and navigate to the "scooter6" folder
4. No additional installations are required

## Usage

### Running the Simulator
Start the scene at: `scooter6/Assets/ARide/ARide/_Scenes/BigCity/Ulm neue Mitte.unity`

### Setting Up Hardware Connection
1. Add the `ArduinoReader` script to your scene and configure ports and pins
2. Add the `EScooterNew` script to your e-scooter object
3. Connect the appropriate objects in the inspector

### Adding AR Visualizations
1. Add the `DangerSignaling` script to your e-scooter
2. Add the `Warnings.prefab` to pedestrians in your hierarchy

## Included Assets

The project incorporates numerous assets to create a realistic urban environment:
- City People FREE Samples
- Electric Scooter Prop
- Free SpeedTrees Package
- Engine Sound packs
- Realistic Hands for VR
- SteamVR Plugin
- City props pack
- Various textures and optimization tools

All assets used are either open-source or have permissive licenses allowing redistribution.

## Extending the Project

This simulator is designed to be modified and extended for different research purposes:
- Add new AR visualization concepts
- Implement different warning systems
- Modify traffic patterns and pedestrian density
- Create new urban environments

## Citation

If you use this simulator in your research, please cite:

```
TODO
```

## Contact

For questions or collaboration requests, please contact:
- Marcel Giss - MGiss@gmx.net
- Mark Colley - [Personal Website](https://m-colley.github.io/) - [GitHub](https://github.com/M-Colley)
