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

### ToggleSirens
This will toggle the siren state of the vehicle the player is mounted to. If the player is not mounted it toggles the siren state of the vehicle the player is looking at, if the MountNeeded configuration is set to false. This is especially useful for horses, because buttons are quite difficult to fit on them.
```
/togglesirens
Example: /togglesirens
```

Paste `bind j togglesirens` into the ingame console to bind that command to the H key as an example.

## Permissions
 - `sirens.attachsirens` -- Allows the player to attach sirens
 - `sirens.detachsirens` -- Allows the player to detach sirens
 - `sirens.attachallsirens` -- Allows the player to attach sirens globally
 - `sirens.detachallsirens` -- Allows the player to detach all sirens globally

## Configuration
```
{
  // Sets if a player needs to be mounted on the car to press the button
  "MountNeeded": true,
  // Global config for enabling the usage of sounds for sirens
  "SoundEnabled": true,
  // Sets the probability of newly spawning vehicles (or existing ones when loading the plugin)
  // having a siren attached to [0.0; 1.0]
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
To find correct positions I suggest to edit the siren file directly on the server (via winscp, filezilla or whatever you use). You can attach the siren to the vehicle you plan to configure, change the config file as you want and type `oxide.reload Sirens` in the console to reload and directly apply the changes. Repeat the last two steps until it fits your needs. You don't need to reattach the siren. This is the way I used to preconfigure the default sirens. As an example the police-germany.json file with some additional comments:
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
          // Left/Right
          "x": 0.05,
          // Up/Down
          "y": 1.7,
          // Forward/Backward
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
    ...
  },
  / These are the configs for all other vehicles (BaseVehicle entity) relevant for this siren,
  // if a vehicle is not set here it will not be supported by the siren and there won't be anything attached to it
  "Vehicles": {
    // The vehicle which is described
    "assets/content/vehicles/sedan_a/sedantest.entity.prefab": [
      {
        // the prefab that will spawn
        "Prefab": "assets/prefabs/io/electric/switches/pressbutton/pressbutton.prefab",
        // the local position of that entity
        "Position": {
          // Left/Right
          "x": 0.0,
          // Up/Down
          "y": 2.05,
          // Forward/Backward
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
    // the horse that is described (acutally this is for all ridable horses)
    "assets/rust.ai/nextai/testridablehorse.prefab": [
      {
        // the prefab that will be spawned
        "Prefab": "assets/prefabs/deployable/playerioents/lights/flasherlight/electric.flasherlight.deployed.prefab",
        // the local position of that entity
        "Position": {
          "x": 0.0,
          "y": 1.7,
          "z": 1.2
        },
        // the local angles of that entity
        "Angle": {
          "x": 45.0,
          "y": 0.0,
          "z": 0.0
        },
        // the bone that entity will be parented to, so that it moves with that bone
        "Bone": "head"
      },
      {
        "Prefab": "assets/prefabs/instruments/trumpet/trumpet.weapon.prefab",
        "Position": {
          "x": -0.06,
          "y": 1.15,
          "z": 1.3
        },
        "Angle": {
          "x": 90.0,
          "y": 150.0,
          "z": 90.0
        },
        "Bone": "lip_lower"
      }
    ],
    ...
  }
}
```

## Available Horse Bones
This are all currently available horse bones, which can be used to parent entites:
 - `Horse_RootBone`
 - `L_Hip`
 - `L_Rear_Thigh`
 - `L_Rear_Shin`
 - `L_Rear_Foot`
 - `L_Rear_Foot_END`
 - `R_Hip`
 - `R_Rear_Thigh`
 - `R_Rear_Shin`
 - `R_Rear_Foot`
 - `R_Rear_Foot_END`
 - `spine_2`
 - `spine_3`
 - `spine_4`
 - `spine_END`
 - `L_mane_1A`
 - `L_mane_1B`
 - `L_mane_1C`
 - `L_Shoulder`
 - `L_Fore_Thigh`
 - `L_Fore_Shin`
 - `L_Fore_Foot`
 - `L_Fore_Foot_END`
 - `neck_1`
 - `L_mane_2a`
 - `L_mane_2b`
 - `L_mane_2c`
 - `neck_2`
 - `L_mane_3a`
 - `L_mane_3b`
 - `L_mane_3c`
 - `neck_3`
 - `L_mane_4a`
 - `L_mane_4b`
 - `L_mane_4c`
 - `neck_END`
 - `head`
 - `L_ear`
 - `L_ear_END`
 - `L_eyelid`
 - `L_eyelid_END`
 - `lip_upper`
 - `joint32`
 - `L_nostril`
 - `joint34`
 - `R_nostril`
 - `joint34_`
 - `lower_jaw`
 - `lip_lower`
 - `lip_lower_END`
 - `tongue_1`
 - `tongue_2`
 - `tongue_END`
 - `mane_5a`
 - `mane_5b`
 - `mane_5c`
 - `R_ear`
 - `R_ear_END`
 - `R_eyelid`
 - `R_eyelid_END`
 - `R_mane_4a`
 - `R_mane_4b`
 - `R_mane_4c`
 - `R_mane_3a`
 - `R_mane_3b`
 - `R_mane_3c`
 - `neck_expander_1`
 - `joint40`
 - `R_mane_2a`
 - `R_mane_2b`
 - `R_mane_2c`
 - `R_mane_1A`
 - `R_mane_1B`
 - `R_mane_1C`
 - `R_Shoulder`
 - `R_Fore_Thigh`
 - `R_Fore_Shin`
 - `R_Fore_Foot`
 - `R_Fore_Foot_END`
 - `stomache_expander_1`
 - `joint36`
 - `stomache_expander_2`
 - `joint38`
 - `tail_1`
 - `tail_2`
 - `tail_3`
 - `tail_4`
 - `tail_END`
