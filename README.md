## How It Works
The sirens will get attached to every configured module of the car the player looks at. What and at what position the entities will spawn is dependend on the configuration of each siren. When pressing the button inside the car the sirens will iterate through the states off, on and lights only. If sound is configured as disabled or a siren without tones was chosen, then the on state will be skipped.

## Chat Commands
### AttachSirens

```
/attachsirens [optional:sirenname]
Example: /attachsirens police-germany
```

#### Syntax - Options
 - **sirenname** - The name of the siren which will be used for this car (default: the first in the configlist)

### DetachSirens

```
/detachsirens
Example: /detachsirens
```

## Permissions
 - sirens.attachsirens
 - sirens.detachsirens

## Configuration
```
{
  // Sets if a player needs to be mounted on the car to press the button
  "MountNeeded": true,
  // Global config for enabling the usage of sounds for sirens
  "SoundEnabled": true,
  // These are the sirens usable ingame, the german police siren and some silent warning lights are preconfigured
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
      ],
      // These are the configs for each module relevant for this siren, if a module is not set here there won't be anything attached to it
      "Modules": {
        "assets/content/vehicles/modularcar/module_entities/1module_cockpit.prefab": [
          {
            // the prefab that will spawn
            "Prefab": "assets/prefabs/io/electric/switches/pressbutton/pressbutton.prefab",
            // the position of that entity
            "Position": {
              "x": 0.05,
              "y": 1.7,
              "z": 0.78
            },
            // the angles of that entity
            "Angle": {
              "x": 210.0,
              "y": 0.0,
              "z": 0.0
            }
          },
          {
            "Prefab": "assets/prefabs/deployable/playerioents/lights/flasherlight/electric.flasherlight.deployed.prefab",
            "Position": {
              "x": -0.4,
              "y": 1.4,
              "z": -0.9
            },
            "Angle": {
              "x": 0.0,
              "y": 0.0,
              "z": 0.0
            }
          },
          {
            "Prefab": "assets/prefabs/deployable/playerioents/lights/flasherlight/electric.flasherlight.deployed.prefab",
            "Position": {
              "x": 0.4,
              "y": 1.4,
              "z": -0.9
            },
            "Angle": {
              "x": 0.0,
              "y": 0.0,
              "z": 0.0
            }
          },
          {
            "Prefab": "assets/prefabs/instruments/trumpet/trumpet.weapon.prefab",
            "Position": {
              "x": -0.08,
              "y": 1.4,
              "z": -0.9
            },
            "Angle": {
              "x": 148.0,
              "y": 150.0,
              "z": 30.0
            }
          }
        ],
        "assets/content/vehicles/modularcar/module_entities/1module_cockpit_armored.prefab": [
          {
            "Prefab": "assets/prefabs/io/electric/switches/pressbutton/pressbutton.prefab",
            "Position": {
              "x": 0.05,
              "y": 1.7,
              "z": 0.78
            },
            "Angle": {
              "x": 210.0,
              "y": 0.0,
              "z": 0.0
            }
          },
          {
            "Prefab": "assets/prefabs/deployable/playerioents/lights/flasherlight/electric.flasherlight.deployed.prefab",
            "Position": {
              "x": -0.4,
              "y": 1.4,
              "z": -0.9
            },
            "Angle": {
              "x": 0.0,
              "y": 0.0,
              "z": 0.0
            }
          },
          {
            "Prefab": "assets/prefabs/deployable/playerioents/lights/flasherlight/electric.flasherlight.deployed.prefab",
            "Position": {
              "x": 0.4,
              "y": 1.4,
              "z": -0.9
            },
            "Angle": {
              "x": 0.0,
              "y": 0.0,
              "z": 0.0
            }
          },
          {
            "Prefab": "assets/prefabs/instruments/trumpet/trumpet.weapon.prefab",
            "Position": {
              "x": -0.08,
              "y": 1.4,
              "z": -0.9
            },
            "Angle": {
              "x": 148.0,
              "y": 150.0,
              "z": 30.0
            }
          }
        ],
        "assets/content/vehicles/modularcar/module_entities/1module_cockpit_with_engine.prefab": [
          {
            "Prefab": "assets/prefabs/io/electric/switches/pressbutton/pressbutton.prefab",
            "Position": {
              "x": 0.05,
              "y": 1.7,
              "z": 0.78
            },
            "Angle": {
              "x": 210.0,
              "y": 0.0,
              "z": 0.0
            }
          },
          {
            "Prefab": "assets/prefabs/deployable/playerioents/lights/flasherlight/electric.flasherlight.deployed.prefab",
            "Position": {
              "x": -0.4,
              "y": 1.4,
              "z": -0.9
            },
            "Angle": {
              "x": 0.0,
              "y": 0.0,
              "z": 0.0
            }
          },
          {
            "Prefab": "assets/prefabs/deployable/playerioents/lights/flasherlight/electric.flasherlight.deployed.prefab",
            "Position": {
              "x": 0.4,
              "y": 1.4,
              "z": -0.9
            },
            "Angle": {
              "x": 0.0,
              "y": 0.0,
              "z": 0.0
            }
          },
          {
            "Prefab": "assets/prefabs/instruments/trumpet/trumpet.weapon.prefab",
            "Position": {
              "x": -0.08,
              "y": 1.4,
              "z": -0.9
            },
            "Angle": {
              "x": 148.0,
              "y": 150.0,
              "z": 30.0
            }
          }
        ]
      }
    },
    {
      "Name": "warning-lights",
      "Tones": [],
      "Modules": {
        "assets/content/vehicles/modularcar/module_entities/1module_cockpit.prefab": [
          {
            "Prefab": "assets/prefabs/io/electric/switches/pressbutton/pressbutton.prefab",
            "Position": {
              "x": 0.05,
              "y": 1.7,
              "z": 0.78
            },
            "Angle": {
              "x": 210.0,
              "y": 0.0,
              "z": 0.0
            }
          },
          {
            "Prefab": "assets/prefabs/deployable/playerioents/lights/sirenlight/electric.sirenlight.deployed.prefab",
            "Position": {
              "x": -0.4,
              "y": 1.4,
              "z": -0.9
            },
            "Angle": {
              "x": 0.0,
              "y": 0.0,
              "z": 0.0
            }
          },
          {
            "Prefab": "assets/prefabs/deployable/playerioents/lights/sirenlight/electric.sirenlight.deployed.prefab",
            "Position": {
              "x": 0.4,
              "y": 1.4,
              "z": -0.9
            },
            "Angle": {
              "x": 0.0,
              "y": 0.0,
              "z": 0.0
            }
          }
        ],
        "assets/content/vehicles/modularcar/module_entities/1module_cockpit_armored.prefab": [
          {
            "Prefab": "assets/prefabs/io/electric/switches/pressbutton/pressbutton.prefab",
            "Position": {
              "x": 0.05,
              "y": 1.7,
              "z": 0.78
            },
            "Angle": {
              "x": 210.0,
              "y": 0.0,
              "z": 0.0
            }
          },
          {
            "Prefab": "assets/prefabs/deployable/playerioents/lights/sirenlight/electric.sirenlight.deployed.prefab",
            "Position": {
              "x": -0.4,
              "y": 1.4,
              "z": -0.9
            },
            "Angle": {
              "x": 0.0,
              "y": 0.0,
              "z": 0.0
            }
          },
          {
            "Prefab": "assets/prefabs/deployable/playerioents/lights/sirenlight/electric.sirenlight.deployed.prefab",
            "Position": {
              "x": 0.4,
              "y": 1.4,
              "z": -0.9
            },
            "Angle": {
              "x": 0.0,
              "y": 0.0,
              "z": 0.0
            }
          }
        ],
        "assets/content/vehicles/modularcar/module_entities/1module_cockpit_with_engine.prefab": [
          {
            "Prefab": "assets/prefabs/io/electric/switches/pressbutton/pressbutton.prefab",
            "Position": {
              "x": 0.05,
              "y": 1.7,
              "z": 0.78
            },
            "Angle": {
              "x": 210.0,
              "y": 0.0,
              "z": 0.0
            }
          },
          {
            "Prefab": "assets/prefabs/deployable/playerioents/lights/sirenlight/electric.sirenlight.deployed.prefab",
            "Position": {
              "x": -0.4,
              "y": 1.4,
              "z": -0.9
            },
            "Angle": {
              "x": 0.0,
              "y": 0.0,
              "z": 0.0
            }
          },
          {
            "Prefab": "assets/prefabs/deployable/playerioents/lights/sirenlight/electric.sirenlight.deployed.prefab",
            "Position": {
              "x": 0.4,
              "y": 1.4,
              "z": -0.9
            },
            "Angle": {
              "x": 0.0,
              "y": 0.0,
              "z": 0.0
            }
          }
        ]
      }
    }
  ]
}
```
