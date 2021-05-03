## How It Works
The sirens will get attached to every cockpit module of the car the player looks at. When pressing the button inside the cockpit the sirens will iterate through the states off, on and lights only. If sound is configured as disabled or a siren without tones was chosen, then the on state will be skipped. Sirens can also be attached and detached globally.

## Chat Commands
### AttachSirens
This will attach the given siren (or the default) to the modular car the player looks at. Already existing sirens will be overridden.
```
/attachsirens [optional:sirenname]
Example: /attachsirens police-germany
```

#### Syntax - Options
 - **sirenname** - The name of the siren melody which will be used for this car (default: the first in the configlist)

### DetachSirens
This will remove the siren from the modular car the player looks at.
```
/detachsirens
Example: /detachsirens
```

### AttachAllSirens
This will attach the given siren (or the default) to all existing modular cars on the map. Already existing sirens will be overridden.
```
/attachallsirens [optional:sirenname]
Example: /attachallsirens police-germany
```

#### Syntax - Options
 - **sirenname** - The name of the siren melody which will be used for this car (default: the first in the configlist)

### DetachAllSirens
This will remove all sirens from all modular cars on the map.
```
/detachallsirens
Example: /detachallsirens
```

### ListSirens
This will post a list of all available sirens to the players chat.
```
/listsirens
Example: /listsirens
```

## Permissions
 - sirens.attachsirens
 - sirens.detachsirens
 - sirens.attachallsirens
 - sirens.detachallsirens

## Configuration
```
{
  // Sets if a player needs to be mounted on the car to press the button
  "MountNeeded": true,
  // Global config for enabling the usage of sounds for sirens
  "SoundEnabled": true,
  // Sets the probability of newly spawning cars (or existing cars when loading the plugin) will have a siren attached to [0.0; 1.0]
  "SirenSpawnProbability": 0.0,
  // Sets the default state (0 = off, 1 = on, 2 = lights_only) which a newly attached siren will have
  "DefaultState": 0,
  
  // These are the siren melodies usable ingame, the german police siren is preconfigured
  "Sirens": [
    {
      // Mapping name to identify the siren while attaching
      "Name": "police-germany",
      // the tone order
      "Tones": [
        {
          // The notes from A = 0 to G = 6
          "Note": 0,
          // The note type with regular = 0 and sharp = 1
          "NoteType": 0,
          // The octave
          "Octave": 4,
          // The time till this tone will stop and the next one plays
          "Duration": 1.0
        },
        {
          "Note": 3,
          "NoteType": 0,
          "Octave": 5,
          "Duration": 1.0
        }
      ]
    }
  ],
  
  // These are the spawn positions and angles for the siren parts and implictly the supported module list
  // If there is no position entry for a prefab then no siren will be spawned on that module
  "LeftSirenPositions": {
    "assets/content/vehicles/modularcar/module_entities/1module_cockpit.prefab": {
      "x": -0.4,
      "y": 1.4,
      "z": -0.9
    },
    "assets/content/vehicles/modularcar/module_entities/1module_cockpit_armored.prefab": {
      "x": -0.4,
      "y": 1.4,
      "z": -0.9
    },
    "assets/content/vehicles/modularcar/module_entities/1module_cockpit_with_engine.prefab": {
      "x": -0.4,
      "y": 1.4,
      "z": -0.9
    }
  },
  "LeftSirenAngles": {},
  "RightSirenPositions": {
    "assets/content/vehicles/modularcar/module_entities/1module_cockpit.prefab": {
      "x": 0.4,
      "y": 1.4,
      "z": -0.9
    },
    "assets/content/vehicles/modularcar/module_entities/1module_cockpit_armored.prefab": {
      "x": 0.4,
      "y": 1.4,
      "z": -0.9
    },
    "assets/content/vehicles/modularcar/module_entities/1module_cockpit_with_engine.prefab": {
      "x": 0.4,
      "y": 1.4,
      "z": -0.9
    }
  },
  "RightSirenAngles": {},
  "TrumpetPositions": {
    "assets/content/vehicles/modularcar/module_entities/1module_cockpit.prefab": {
      "x": -0.08,
      "y": 1.4,
      "z": -0.9
    },
    "assets/content/vehicles/modularcar/module_entities/1module_cockpit_armored.prefab": {
      "x": -0.08,
      "y": 1.4,
      "z": -0.9
    },
    "assets/content/vehicles/modularcar/module_entities/1module_cockpit_with_engine.prefab": {
      "x": -0.08,
      "y": 1.4,
      "z": -0.9
    }
  },
  "TrumpetAngles": {
    "assets/content/vehicles/modularcar/module_entities/1module_cockpit.prefab": {
      "x": 148.0,
      "y": 150.0,
      "z": 30.0
    },
    "assets/content/vehicles/modularcar/module_entities/1module_cockpit_armored.prefab": {
      "x": 148.0,
      "y": 150.0,
      "z": 30.0
    },
    "assets/content/vehicles/modularcar/module_entities/1module_cockpit_with_engine.prefab": {
      "x": 148.0,
      "y": 150.0,
      "z": 30.0
    }
  },
  "ButtonPositions": {
    "assets/content/vehicles/modularcar/module_entities/1module_cockpit.prefab": {
      "x": 0.05,
      "y": 1.7,
      "z": 0.78
    },
    "assets/content/vehicles/modularcar/module_entities/1module_cockpit_armored.prefab": {
      "x": 0.05,
      "y": 1.7,
      "z": 0.78
    },
    "assets/content/vehicles/modularcar/module_entities/1module_cockpit_with_engine.prefab": {
      "x": 0.05,
      "y": 1.7,
      "z": 0.78
    }
  },
  "ButtonAngles": {
    "assets/content/vehicles/modularcar/module_entities/1module_cockpit.prefab": {
      "x": 210.0,
      "y": 0.0,
      "z": 0.0
    },
    "assets/content/vehicles/modularcar/module_entities/1module_cockpit_armored.prefab": {
      "x": 210.0,
      "y": 0.0,
      "z": 0.0
    },
    "assets/content/vehicles/modularcar/module_entities/1module_cockpit_with_engine.prefab": {
      "x": 210.0,
      "y": 0.0,
      "z": 0.0
    }
  },
  
  // These are the used assets from the game and should not be changed unless not working anymore
  "PrefabFlasherLight": "assets/prefabs/deployable/playerioents/lights/flasherlight/electric.flasherlight.deployed.prefab",
  "PrefabTrumpet": "assets/prefabs/instruments/trumpet/trumpet.weapon.prefab",
  "PrefabButton": "assets/prefabs/deployable/playerioents/button/button.prefab"
```
