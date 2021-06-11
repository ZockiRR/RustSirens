## How It Works
The sirens will get attached to the vehicle or every configured module of the modular car the player looks at, if the siren supports that vehicle. What and at what position the entities will spawn is dependend on the configuration of each siren. When pressing the button inside the vehicle (or whereever the button is configured to be) the sirens will iterate through the states off, on and lights only. If sound is configured as disabled or a siren without tones was chosen, then the on state will be skipped. Sirens can also be attached and detached globally.

## Commands
### AttachSirens
This will attach the given siren (or the default) to the vehicle the player looks at. Already existing sirens will be overridden.
```
/attachsirens [optional:sirenname]
Example: /attachsirens police-germany
```

#### Syntax - Options
 - **sirenname** - The name of the siren which will be used for this vehicle (default: the first in the configlist)

### DetachSirens
This will remove the siren from the vehicle the player looks at.
```
/detachsirens
Example: /detachsirens
```

### AttachAllSirens
This will attach the given siren (or the default) to all existing vehicles on the map, if supported by the siren. Already existing sirens will be overridden.
```
/attachallsirens [optional:sirenname]
Example: /attachallsirens police-germany
```

#### Syntax - Options
 - **sirenname** - The name of the siren which will be used for this vehicle (default: the first in the configlist)

### DetachAllSirens
This will remove all sirens from all vehicles on the map.
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
  // Sets the probability of newly spawning vehicles (or existing ones when loading the plugin) having a siren attached to [0.0; 1.0]
  // MODULAR_CAR is a special key for the modular car vehicle type.
  "SirenSpawnProbability": {
    "MODULAR_CAR": 0.0,
    "assets/content/vehicles/sedan_a/sedantest.entity.prefab": 0.0,
    "assets/content/vehicles/minicopter/minicopter.entity.prefab": 0.0,
    "assets/content/vehicles/scrap heli carrier/scraptransporthelicopter.prefab": 0.0,
    "assets/content/vehicles/boats/rhib/rhib.prefab": 0.0,
    "assets/content/vehicles/boats/rowboat/rowboat.prefab": 0.0,
    "assets/content/vehicles/workcart/workcart.entity.prefab": 0.0,
    "assets/content/vehicles/crane_magnet/magnetcrane.entity.prefab": 0.0
  },
  // Sets the default state (OFF, ON, LIGHTS_ONLY) which a newly attached siren will have
  "DefaultState": "OFF"
}
```

## Sirens
The folder '/data/sirens/' contains the available sirens. Existing ones can be edited or new ones can be created.
As an example the police-germany.json file with some additional comments:
```
{
  // Mapping name to identify the siren while attaching
  "Name": "police-germany",
  // the tone order
  "Tones": [
    {
      // The notes [A; G]
      "Note": "A",
      // The note type (Regular, Sharp)
      "NoteType": "Regular",
      // The octave
      "Octave": 4,
      // The time till this tone will stop and the next one plays (in seconds)
      "Duration": 1.0
    },
    {
      "Note": "D",
      "NoteType": "Regular",
      "Octave": 5,
      "Duration": 1.0
    }
  ],
  // These are the configs for each module of modular cars relevant for this siren,
  // if a module is not set here there won't be anything attached to it
  "Modules": {
    // The module which is described
    "assets/content/vehicles/modularcar/module_entities/1module_cockpit.prefab": [
      {
        // the prefab that will spawn
        "Prefab": "assets/prefabs/io/electric/switches/pressbutton/pressbutton.prefab",
        // the local position of that entity
        "Position": {
          "x": 0.05,
          "y": 1.7,
          "z": 0.78
        },
        // the local angles of that entity
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
  },
  / These are the configs for all other vehicles (BaseVehicle entity) relevant for this siren,
  // if a vehicle is not set here it will not be supported by the siren andthere won't be anything attached to it
  "Vehicles": {
    // The module which is described
    "assets/content/vehicles/sedan_a/sedantest.entity.prefab": [
      {
        // the prefab that will spawn
        "Prefab": "assets/prefabs/io/electric/switches/pressbutton/pressbutton.prefab",
        // the local position of that entity
        "Position": {
          "x": 0.0,
          "y": 2.05,
          "z": 1.9
        },
        // the local angles of that entity
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
          "y": 1.65,
          "z": 0.2
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
          "y": 1.65,
          "z": 0.2
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
          "y": 1.68,
          "z": 0.2
        },
        "Angle": {
          "x": 148.0,
          "y": 150.0,
          "z": 30.0
        }
      }
    ],
    "assets/content/vehicles/minicopter/minicopter.entity.prefab": [
      {
        "Prefab": "assets/prefabs/io/electric/switches/pressbutton/pressbutton.prefab",
        "Position": {
          "x": -0.1,
          "y": 2.0,
          "z": 1.0
        },
        "Angle": {
          "x": 180.0,
          "y": 0.0,
          "z": 0.0
        }
      },
      {
        "Prefab": "assets/prefabs/deployable/playerioents/lights/flasherlight/electric.flasherlight.deployed.prefab",
        "Position": {
          "x": 0.0,
          "y": 2.235,
          "z": -0.025
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
          "x": 0.0,
          "y": 0.832,
          "z": -2.7
        },
        "Angle": {
          "x": 272.0,
          "y": 0.0,
          "z": 0.0
        }
      },
      {
        "Prefab": "assets/prefabs/instruments/trumpet/trumpet.weapon.prefab",
        "Position": {
          "x": 0.09,
          "y": 0.78,
          "z": -0.9
        },
        "Angle": {
          "x": 148.0,
          "y": 330.0,
          "z": 30.0
        }
      }
    ],
    "assets/content/vehicles/scrap heli carrier/scraptransporthelicopter.prefab": [
      {
        "Prefab": "assets/prefabs/io/electric/switches/pressbutton/pressbutton.prefab",
        "Position": {
          "x": -0.1,
          "y": 2.68,
          "z": 3.865
        },
        "Angle": {
          "x": 205.0,
          "y": 0.0,
          "z": 0.0
        }
      },
      {
        "Prefab": "assets/prefabs/deployable/playerioents/lights/flasherlight/electric.flasherlight.deployed.prefab",
        "Position": {
          "x": -0.3,
          "y": 3.15,
          "z": -7.65
        },
        "Angle": {
          "x": 0.0,
          "y": 0.0,
          "z": 90.0
        }
      },
      {
        "Prefab": "assets/prefabs/deployable/playerioents/lights/flasherlight/electric.flasherlight.deployed.prefab",
        "Position": {
          "x": 0.1,
          "y": 3.15,
          "z": -7.65
        },
        "Angle": {
          "x": 0.0,
          "y": 0.0,
          "z": 270.0
        }
      },
      {
        "Prefab": "assets/prefabs/deployable/playerioents/lights/flasherlight/electric.flasherlight.deployed.prefab",
        "Position": {
          "x": 0.0,
          "y": 0.55,
          "z": 3.6
        },
        "Angle": {
          "x": 180.0,
          "y": 0.0,
          "z": 0.0
        }
      },
      {
        "Prefab": "assets/prefabs/instruments/trumpet/trumpet.weapon.prefab",
        "Position": {
          "x": 0.0,
          "y": 0.58,
          "z": 2.5
        },
        "Angle": {
          "x": 328.0,
          "y": 30.0,
          "z": 30.0
        }
      }
    ],
    "assets/content/vehicles/boats/rhib/rhib.prefab": [
      {
        "Prefab": "assets/prefabs/io/electric/switches/pressbutton/pressbutton.prefab",
        "Position": {
          "x": -0.4,
          "y": 3.0,
          "z": 0.25
        },
        "Angle": {
          "x": 180.0,
          "y": 0.0,
          "z": 0.0
        }
      },
      {
        "Prefab": "assets/prefabs/deployable/playerioents/lights/flasherlight/electric.flasherlight.deployed.prefab",
        "Position": {
          "x": -0.55,
          "y": 2.83,
          "z": 0.62
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
          "x": 0.55,
          "y": 2.83,
          "z": 0.62
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
          "x": 0.8,
          "y": 1.16,
          "z": 0.5
        },
        "Angle": {
          "x": 148.0,
          "y": 150.0,
          "z": 30.0
        }
      }
    ],
    "assets/content/vehicles/boats/rowboat/rowboat.prefab": [
      {
        "Prefab": "assets/prefabs/io/electric/switches/pressbutton/pressbutton.prefab",
        "Position": {
          "x": -1.7,
          "y": 0.5,
          "z": -1.8
        },
        "Angle": {
          "x": 270.0,
          "y": 270.0,
          "z": 0.0
        }
      },
      {
        "Prefab": "assets/prefabs/deployable/playerioents/lights/flasherlight/electric.flasherlight.deployed.prefab",
        "Position": {
          "x": 0.0,
          "y": 0.8,
          "z": 2.18
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
          "x": 0.5,
          "y": 0.1,
          "z": -0.4
        },
        "Angle": {
          "x": 148.0,
          "y": 150.0,
          "z": 30.0
        }
      }
    ],
    "assets/content/vehicles/workcart/workcart.entity.prefab": [
      {
        "Prefab": "assets/prefabs/io/electric/switches/pressbutton/pressbutton.prefab",
        "Position": {
          "x": 0.19,
          "y": 3.13,
          "z": 4.95
        },
        "Angle": {
          "x": 235.0,
          "y": 0.0,
          "z": 0.0
        }
      },
      {
        "Prefab": "assets/prefabs/deployable/playerioents/lights/flasherlight/electric.flasherlight.deployed.prefab",
        "Position": {
          "x": 0.1,
          "y": 3.95,
          "z": 4.13
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
          "x": 0.25,
          "y": 3.95,
          "z": 4.13
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
          "x": -1.49,
          "y": 2.52,
          "z": -4.58
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
          "x": 1.51,
          "y": 2.52,
          "z": -4.55
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
          "x": 0.59,
          "y": 3.85,
          "z": 4.13
        },
        "Angle": {
          "x": 148.0,
          "y": 150.0,
          "z": 30.0
        }
      }
    ],
    "assets/content/vehicles/crane_magnet/magnetcrane.entity.prefab": [
      {
        "Prefab": "assets/prefabs/io/electric/switches/pressbutton/pressbutton.prefab",
        "Position": {
          "x": -0.61,
          "y": 4.0,
          "z": 2.3
        },
        "Angle": {
          "x": 230.0,
          "y": 0.0,
          "z": 0.0
        }
      },
      {
        "Prefab": "assets/prefabs/deployable/playerioents/lights/flasherlight/electric.flasherlight.deployed.prefab",
        "Position": {
          "x": -0.95,
          "y": 4.25,
          "z": 0.5
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
          "z": 0.0
        },
        "Angle": {
          "x": 148.0,
          "y": 150.0,
          "z": 30.0
        }
      }
    ]
  }
}
```
