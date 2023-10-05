# HoloNav_Test
Seamlessly visualizing Augmented Reality (AR) cues in the real world in HoloLens 2 by giving GNSS (Lattitude, Longitude)

## Hardware
HoloLens 2
HoloNav

## Software
Developed in Unity 2020.3.14f1

## Functions
1. Receive GPS and IMU data from HoloNav via Bluetooth
2. Coordinate conversion - convert GPS to Unity Cartesian coordinates automatically (When read in GPS data)
3. North calibration by audio input - say "Calibrate" when the circle marker on the compass turns red

- Accuracy: 5m
- Test Area: 500m as a crow flies

### Initialization and Tracking Implementation
<img src="https://github.com/zy0531/HoloNav/blob/main/Figures/Tracking.PNG?raw=true" width="90%" height="90%">

### Visual AR Cues
<img src="https://github.com/zy0531/HoloNav/blob/main/Figures/ARCue.PNG?raw=true" width="90%" height="90%">

### Input and Interaction
<img src="https://github.com/zy0531/HoloNav/blob/main/Figures/InputInteraction.PNG?raw=true" width="90%" height="90%">
