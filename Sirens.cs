using Newtonsoft.Json;
using Rust.Instruments;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static InstrumentKeyController;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using Newtonsoft.Json.Converters;
using System;

namespace Oxide.Plugins
{
    [Info("Sirens", "ZockiRR", "2.0.0")]
    [Description("Gives players the ability to attach sirens to vehicles")]
    class Sirens : CovalencePlugin
    {
        #region variables
        private const string PERMISSION_ATTACHSIRENS = "sirens.attachsirens";
        private const string PERMISSION_DETACHSIRENS = "sirens.detachsirens";
        private const string PERMISSION_ATTACHSIRENS_GLOBAL = "sirens.attachallsirens";
        private const string PERMISSION_DETACHSIRENS_GLOBAL = "sirens.detachallsirens";

        private const string I18N_MISSING_SIREN = "NoSirenForName";
        private const string I18N_COULD_NOT_ATTACH = "CouldNotAttach";
        private const string I18N_NOT_SUPPORTED = "NotSupported";
        private const string I18N_ATTACHED = "Attached";
        private const string I18N_ATTACHED_GLOBAL = "AttachedGlobal";
        private const string I18N_DETACHED = "Detached";
        private const string I18N_DETACHED_GLOBAL = "DetachedGlobal";
        private const string I18N_NOT_A_VEHICLE = "NotAVehicle";
        private const string I18N_SIRENS = "Sirens";
        private const string I18N_PLAYERS_ONLY = "PlayersOnly";

        // Initial prefabs
        private const string PREFAB_COCKPIT = "assets/content/vehicles/modularcar/module_entities/1module_cockpit.prefab";
        private const string PREFAB_COCKPIT_ARMORED = "assets/content/vehicles/modularcar/module_entities/1module_cockpit_armored.prefab";
        private const string PREFAB_COCKPIT_WITH_ENGINE = "assets/content/vehicles/modularcar/module_entities/1module_cockpit_with_engine.prefab";
        private const string PREFAB_BUTTON = "assets/prefabs/io/electric/switches/pressbutton/pressbutton.prefab";
        private const string PREFAB_FLASHERLIGHT = "assets/prefabs/deployable/playerioents/lights/flasherlight/electric.flasherlight.deployed.prefab";
        private const string PREFAB_SIRENLIGHT = "assets/prefabs/deployable/playerioents/lights/sirenlight/electric.sirenlight.deployed.prefab";
        private const string PREFAB_TRUMPET = "assets/prefabs/instruments/trumpet/trumpet.weapon.prefab";

        private const string PREFAB_SEDAN = "assets/content/vehicles/sedan_a/sedantest.entity.prefab";
        private const string PREFAB_MINICOPTER = "assets/content/vehicles/minicopter/minicopter.entity.prefab";
        private const string PREFAB_TRANSPORTHELI = "assets/content/vehicles/scrap heli carrier/scraptransporthelicopter.prefab";
        private const string PREFAB_RHIB = "assets/content/vehicles/boats/rhib/rhib.prefab";
        private const string PREFAB_ROWBOAT = "assets/content/vehicles/boats/rowboat/rowboat.prefab";
        private const string PREFAB_WORKCART = "assets/content/vehicles/workcart/workcart.entity.prefab";
        private const string PREFAB_MAGNETCRANE = "assets/content/vehicles/crane_magnet/magnetcrane.entity.prefab";

        private const string KEY_MODULAR_CAR = "KEY_MODULAR_CAR";

        // Preconfigured sirens
        private static readonly Siren SIREN_DEFAULT = new Siren("police-germany",
            new Dictionary<string, Attachment[]>
            {
                [PREFAB_COCKPIT] = new Attachment[] {
                    new Attachment(PREFAB_BUTTON, new Vector3(0.05f, 1.7f, 0.78f), new Vector3(210f, 0f, 0f)),
                    new Attachment(PREFAB_FLASHERLIGHT, new Vector3(-0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_FLASHERLIGHT, new Vector3(0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_TRUMPET, new Vector3(-0.08f, 1.4f, -0.9f), new Vector3(148f, 150f, 30f))
                },
                [PREFAB_COCKPIT_ARMORED] = new Attachment[] {
                    new Attachment(PREFAB_BUTTON, new Vector3(0.05f, 1.7f, 0.78f), new Vector3(210f, 0f, 0f)),
                    new Attachment(PREFAB_FLASHERLIGHT, new Vector3(-0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_FLASHERLIGHT, new Vector3(0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_TRUMPET, new Vector3(-0.08f, 1.4f, -0.9f), new Vector3(148f, 150f, 30f))
                },
                [PREFAB_COCKPIT_WITH_ENGINE] = new Attachment[] {
                    new Attachment(PREFAB_BUTTON, new Vector3(0.05f, 1.7f, 0.78f), new Vector3(210f, 0f, 0f)),
                    new Attachment(PREFAB_FLASHERLIGHT, new Vector3(-0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_FLASHERLIGHT, new Vector3(0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_TRUMPET, new Vector3(-0.08f, 1.4f, -0.9f), new Vector3(148f, 150f, 30f))
                }
            },
            new Dictionary<string, Attachment[]>
            {
                [PREFAB_SEDAN] = new Attachment[] {
                    new Attachment(PREFAB_BUTTON, new Vector3(0.05f, 1.7f, 0.78f), new Vector3(210f, 0f, 0f)),
                    new Attachment(PREFAB_FLASHERLIGHT, new Vector3(-0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_FLASHERLIGHT, new Vector3(0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_TRUMPET, new Vector3(-0.08f, 1.4f, -0.9f), new Vector3(148f, 150f, 30f))
                },
                [PREFAB_MINICOPTER] = new Attachment[] {
                    new Attachment(PREFAB_BUTTON, new Vector3(0.05f, 1.7f, 0.78f), new Vector3(210f, 0f, 0f)),
                    new Attachment(PREFAB_FLASHERLIGHT, new Vector3(-0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_FLASHERLIGHT, new Vector3(0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_TRUMPET, new Vector3(-0.08f, 1.4f, -0.9f), new Vector3(148f, 150f, 30f))
                },
                [PREFAB_TRANSPORTHELI] = new Attachment[] {
                    new Attachment(PREFAB_BUTTON, new Vector3(0.05f, 1.7f, 0.78f), new Vector3(210f, 0f, 0f)),
                    new Attachment(PREFAB_FLASHERLIGHT, new Vector3(-0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_FLASHERLIGHT, new Vector3(0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_TRUMPET, new Vector3(-0.08f, 1.4f, -0.9f), new Vector3(148f, 150f, 30f))
                },
                [PREFAB_RHIB] = new Attachment[] {
                    new Attachment(PREFAB_BUTTON, new Vector3(0.05f, 1.7f, 0.78f), new Vector3(210f, 0f, 0f)),
                    new Attachment(PREFAB_FLASHERLIGHT, new Vector3(-0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_FLASHERLIGHT, new Vector3(0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_TRUMPET, new Vector3(-0.08f, 1.4f, -0.9f), new Vector3(148f, 150f, 30f))
                },
                [PREFAB_ROWBOAT] = new Attachment[] {
                    new Attachment(PREFAB_BUTTON, new Vector3(0.05f, 1.7f, 0.78f), new Vector3(210f, 0f, 0f)),
                    new Attachment(PREFAB_FLASHERLIGHT, new Vector3(-0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_FLASHERLIGHT, new Vector3(0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_TRUMPET, new Vector3(-0.08f, 1.4f, -0.9f), new Vector3(148f, 150f, 30f))
                },
                [PREFAB_WORKCART] = new Attachment[] {
                    new Attachment(PREFAB_BUTTON, new Vector3(0.05f, 1.7f, 0.78f), new Vector3(210f, 0f, 0f)),
                    new Attachment(PREFAB_FLASHERLIGHT, new Vector3(-0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_FLASHERLIGHT, new Vector3(0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_TRUMPET, new Vector3(-0.08f, 1.4f, -0.9f), new Vector3(148f, 150f, 30f))
                },
                [PREFAB_MAGNETCRANE] = new Attachment[] {
                    new Attachment(PREFAB_BUTTON, new Vector3(0.05f, 1.7f, 0.78f), new Vector3(210f, 0f, 0f)),
                    new Attachment(PREFAB_FLASHERLIGHT, new Vector3(-0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_FLASHERLIGHT, new Vector3(0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_TRUMPET, new Vector3(-0.08f, 1.4f, -0.9f), new Vector3(148f, 150f, 30f))
                }
            }, new Tone(Notes.A, NoteType.Regular, 4, 1f), new Tone(Notes.D, NoteType.Regular, 5, 1f));
        private static readonly Siren SIREN_SILENT = new Siren("warning-lights",
            new Dictionary<string, Attachment[]>
            {
                [PREFAB_COCKPIT] = new Attachment[] {
                    new Attachment(PREFAB_BUTTON, new Vector3(0.05f, 1.7f, 0.78f), new Vector3(210f, 0f, 0f)),
                    new Attachment(PREFAB_SIRENLIGHT, new Vector3(-0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_SIRENLIGHT, new Vector3(0.4f, 1.4f, -0.9f))
                },
                [PREFAB_COCKPIT_ARMORED] = new Attachment[] {
                    new Attachment(PREFAB_BUTTON, new Vector3(0.05f, 1.7f, 0.78f), new Vector3(210f, 0f, 0f)),
                    new Attachment(PREFAB_SIRENLIGHT, new Vector3(-0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_SIRENLIGHT, new Vector3(0.4f, 1.4f, -0.9f))
                },
                [PREFAB_COCKPIT_WITH_ENGINE] = new Attachment[] {
                    new Attachment(PREFAB_BUTTON, new Vector3(0.05f, 1.7f, 0.78f), new Vector3(210f, 0f, 0f)),
                    new Attachment(PREFAB_SIRENLIGHT, new Vector3(-0.4f, 1.4f, -0.9f)),
                    new Attachment(PREFAB_SIRENLIGHT, new Vector3(0.4f, 1.4f, -0.9f))
                }
            },
            new Dictionary<string, Attachment[]>
            { });
        #endregion variables

        #region data
        private class DataContainer
        {
            // Map BaseVehicle.net.ID -> SirenInfos
            public Dictionary<uint, VehicleContainer> VehicleSirenMap = new Dictionary<uint, VehicleContainer>();
        }

        private class VehicleContainer
        {
            public string SirenName = SIREN_DEFAULT.Name;
            public SirenController.States State = SirenController.States.OFF;
            public ISet<uint> NetIDs = new HashSet<uint>();

            public VehicleContainer(string aSirenName, SirenController.States aState, IEnumerable<uint> someNetIDs)
            {
                SirenName = aSirenName;
                State = aState;
                NetIDs.UnionWith(someNetIDs);
            }
        }
        #endregion data

        #region configuration

        private Configuration config;
        private IDictionary<string, Siren> SirenDictionary { get; } = new Dictionary<string, Siren>();

        private class Configuration
        {
            [JsonProperty("MountNeeded")]
            public bool MountNeeded = true;

            [JsonProperty("SoundEnabled")]
            public bool SoundEnabled = true;

            [JsonProperty("SirenSpawnProbability")]
            public Dictionary<string, float> SirenSpawnProbability = new Dictionary<string, float>
            {
                [KEY_MODULAR_CAR] = 0f,
                [PREFAB_SEDAN] = 0f,
                [PREFAB_MINICOPTER] = 0f,
                [PREFAB_TRANSPORTHELI] = 0f,
                [PREFAB_RHIB] = 0f,
                [PREFAB_ROWBOAT] = 0f,
                [PREFAB_WORKCART] = 0f,
                [PREFAB_MAGNETCRANE] = 0f
            };

            [JsonConverter(typeof(StringEnumConverter))]
            [JsonProperty("DefaultState")]
            public SirenController.States DefaultState = SirenController.States.OFF;

            public string ToJson() => JsonConvert.SerializeObject(this);

            public Dictionary<string, object> ToDictionary() => JsonConvert.DeserializeObject<Dictionary<string, object>>(ToJson());
        }

        private class Tone
        {
            public Tone(Notes aNote = Notes.A, NoteType aNoteType = NoteType.Regular, int anOctave = 4, float aDuration = 1f)
            {
                Note = aNote;
                NoteType = aNoteType;
                Octave = anOctave;
                Duration = aDuration;
            }

            [JsonConverter(typeof(StringEnumConverter))]
            [JsonProperty("Note")]
            public Notes Note;

            [JsonConverter(typeof(StringEnumConverter))]
            [JsonProperty("NoteType")]
            public NoteType NoteType;

            [JsonProperty("Octave")]
            public int Octave;

            [JsonProperty("Duration")]
            public float Duration;

            public string ToJson() => JsonConvert.SerializeObject(this);

            public Dictionary<string, object> ToDictionary() => JsonConvert.DeserializeObject<Dictionary<string, object>>(ToJson());
        }

        private class Siren
        {
            public Siren(string aName, Dictionary<string, Attachment[]> someModules, Dictionary<string, Attachment[]> someVehicles, params Tone[] someTones)
            {
                Name = aName;
                Modules = someModules;
                Vehicles = someVehicles;
                Tones = someTones;
            }

            [JsonProperty("Name")]
            public string Name;

            [JsonProperty("Tones")]
            public Tone[] Tones;

            [JsonProperty("Modules")]
            public Dictionary<string, Attachment[]> Modules;

            [JsonProperty("Vehicles")]
            public Dictionary<string, Attachment[]> Vehicles;

            public string ToJson() => JsonConvert.SerializeObject(this);

            public Dictionary<string, object> ToDictionary() => JsonConvert.DeserializeObject<Dictionary<string, object>>(ToJson());
        }

        private class Attachment
        {
            public Attachment(string aPrefab, Vector3 aPosition, Vector3 anAngle = new Vector3())
            {
                Prefab = aPrefab;
                Position = aPosition;
                Angle = anAngle;
            }

            [JsonProperty("Prefab")]
            public string Prefab;

            [JsonProperty("Position")]
            public Vector3 Position;

            [JsonProperty("Angle")]
            public Vector3 Angle;

            public string ToJson() => JsonConvert.SerializeObject(this);

            public Dictionary<string, object> ToDictionary() => JsonConvert.DeserializeObject<Dictionary<string, object>>(ToJson());
        }

        protected override void LoadDefaultConfig()
        {
            config = new Configuration();
            SirenDictionary.Clear();
            SirenDictionary.Add(SIREN_DEFAULT.Name, SIREN_DEFAULT);
            SirenDictionary.Add(SIREN_SILENT.Name, SIREN_SILENT);
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                config = Config.ReadObject<Configuration>();
                if (config == null)
                {
                    throw new JsonException();
                }

                foreach (string eachSirenFile in Interface.Oxide.DataFileSystem.GetFiles("sirens", "*.json"))
                {
                    Siren theSiren = Interface.Oxide.DataFileSystem.ReadObject<Siren>(eachSirenFile);
                    SirenDictionary.Add(theSiren.Name, theSiren);
                }

                if (SirenDictionary.IsEmpty())
                {
                    PrintWarning("Configuration appears to be missing sirens; using defaults");
                    SirenDictionary.Add(SIREN_DEFAULT.Name, SIREN_DEFAULT);
                    SirenDictionary.Add(SIREN_SILENT.Name, SIREN_SILENT);
                }

                if (!config.ToDictionary().Keys.SequenceEqual(Config.ToDictionary(x => x.Key, x => x.Value).Keys))
                {
                    PrintWarning("Configuration appears to be outdated; updating and saving");
                    SaveConfig();
                }

            }
            catch
            {
                PrintWarning($"Configuration file {Name}.json is invalid; using defaults");
                LoadDefaultConfig();
            }
        }

        protected override void SaveConfig()
        {
            PrintWarning($"Configuration changes saved to {Name}.json");
            Config.WriteObject(config, true);

            foreach (Siren eachSiren in SirenDictionary.Values)
            {
                Interface.Oxide.DataFileSystem.WriteObject("sirens/" + eachSiren.Name, eachSiren);
            }
        }
        #endregion configuration

        #region localization
        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                [I18N_MISSING_SIREN] = "No siren was found for the given name (using {0} instead)",
                [I18N_COULD_NOT_ATTACH] = "Could not attach '{0}'",
                [I18N_ATTACHED] = "Attached siren '{0}'",
                [I18N_ATTACHED_GLOBAL] = "Attached siren '{0}' to all existing cars",
                [I18N_DETACHED] = "Detached siren",
                [I18N_DETACHED_GLOBAL] = "Detached all existing sirens",
                [I18N_NOT_A_VEHICLE] = "This entity is not a (supported) vehicle",
                [I18N_SIRENS] = "Available sirens: {0}",
                [I18N_PLAYERS_ONLY] = "Command '{0}' can only be used by a player",
                [I18N_NOT_SUPPORTED] = "The siren '{0}' has no configuration for '{1}'"
            }, this);
        }
        #endregion localization

        #region commands
        [Command("attachsirens"), Permission(PERMISSION_ATTACHSIRENS)]
        private void AttachCarSirens(IPlayer aPlayer, string aCommand, string[] someArgs)
        {
            if (aPlayer.IsServer)
            {
                Message(aPlayer, I18N_PLAYERS_ONLY, aCommand);
                return;
            }

            BaseVehicle theVehicle = RaycastVehicle(aPlayer);
            if (theVehicle)
            {
                Siren theSiren = someArgs.Length > 0 ? FindSirenForName(someArgs[0], aPlayer) : SirenDictionary.Values.First();
                AttachSirens(theVehicle, theSiren, config.DefaultState, aPlayer);
                Message(aPlayer, I18N_ATTACHED, theSiren.Name);
            }
        }

        [Command("detachsirens"), Permission(PERMISSION_DETACHSIRENS)]
        private void DetachCarSirens(IPlayer aPlayer, string aCommand, string[] someArgs)
        {
            if (aPlayer.IsServer)
            {
                Message(aPlayer, I18N_PLAYERS_ONLY, aCommand);
                return;
            }

            BaseVehicle theVehicle = RaycastVehicle(aPlayer);
            if (theVehicle && DetachSirens(theVehicle))
            {
                Message(aPlayer, I18N_DETACHED);
            }
        }

        [Command("attachallsirens"), Permission(PERMISSION_ATTACHSIRENS_GLOBAL)]
        private void AttachAllCarSirens(IPlayer aPlayer, string aCommand, string[] someArgs)
        {
            Siren theSiren = someArgs.Length > 0 ? FindSirenForName(someArgs[0], aPlayer) : SirenDictionary.Values.First();
            foreach (BaseVehicle eachVehicle in BaseNetworkable.serverEntities.OfType<BaseVehicle>())
            {
                AttachSirens(eachVehicle, theSiren, config.DefaultState, aPlayer);
            }
            Message(aPlayer, I18N_ATTACHED_GLOBAL, theSiren.Name);
        }

        [Command("detachallsirens"), Permission(PERMISSION_DETACHSIRENS_GLOBAL)]
        private void DetachAllCarSirens(IPlayer aPlayer, string aCommand, string[] someArgs)
        {
            foreach (BaseVehicle eachVehicle in BaseNetworkable.serverEntities.OfType<BaseVehicle>())
            {
                DetachSirens(eachVehicle);
            }
            Message(aPlayer, I18N_DETACHED_GLOBAL);
        }

        [Command("listsirens")]
        private void ListSirens(IPlayer aPlayer, string aCommand, string[] someArgs)
        {
            Message(aPlayer, I18N_SIRENS, string.Join(", ", SirenDictionary.Keys));
        }
        #endregion commands

        #region hooks
        private void Unload()
        {
            OnServerSave();

            foreach (BaseVehicle eachVehicle in BaseNetworkable.serverEntities.OfType<BaseVehicle>())
            {
                DetachSirens(eachVehicle);
            }
        }

        private void OnServerSave()
        {
            DataContainer thePersistentData = new DataContainer();
            foreach (BaseVehicle eachCar in BaseNetworkable.serverEntities.OfType<BaseVehicle>())
            {
                SirenController theController = eachCar.GetComponent<SirenController>();
                thePersistentData.VehicleSirenMap.Add(eachCar.net.ID, theController ? new VehicleContainer(theController.Siren.Name, theController.State, theController.NetIDs) : null);
            }
            Interface.Oxide.DataFileSystem.WriteObject(Name, thePersistentData);
        }

        private void OnServerInitialized(bool anInitialFlag)
        {
            bool theSpawnRandomlyFlag = config.SirenSpawnProbability.Any(entry => entry.Value > 0f);
            if (!theSpawnRandomlyFlag)
            {
                Unsubscribe("OnEntitySpawned");
            }

            // Reattach on server restart
            DataContainer thePersistentData = Interface.Oxide.DataFileSystem.ReadObject<DataContainer>(Name);
            foreach (BaseVehicle eachVehicle in BaseNetworkable.serverEntities.OfType<BaseVehicle>())
            {
                VehicleContainer theContainer;
                if (thePersistentData.VehicleSirenMap.TryGetValue(eachVehicle.net.ID, out theContainer))
                {
                    if (theContainer != null)
                    {
                        Siren theSiren;
                        if (SirenDictionary.TryGetValue(theContainer.SirenName, out theSiren))
                        {
                            CreateSirenController(eachVehicle, theSiren, theContainer.NetIDs);
                            AttachSirens(eachVehicle, theSiren, theContainer.State);
                        } else
                        {
                            CreateSirenController(eachVehicle, null, theContainer.NetIDs);
                            DetachSirens(eachVehicle);
                            Puts($"Missing siren for name \"{theContainer.SirenName}\". Ignoring...");
                        }
                    }
                } else if (theSpawnRandomlyFlag)
                {
                    SirenController theController = eachVehicle.GetComponent<SirenController>();
                    if (!theController)
                    {
                        float theProbability;
                        if (config.SirenSpawnProbability.TryGetValue(eachVehicle is BaseVehicle ? KEY_MODULAR_CAR : eachVehicle.PrefabName, out theProbability) && Core.Random.Range(0f, 1f) < theProbability)
                        {
                            AttachSirens(eachVehicle, SirenDictionary.Values.First(), config.DefaultState);
                        }
                    }
                }
            }
        }

        private object OnButtonPress(PressButton aButton, BasePlayer aPlayer)
        {
            BaseVehicle theVehicle = aButton.GetComponentInParent<BaseVehicle>();
            if (theVehicle)
            {
                SirenController theController = theVehicle.GetComponent<SirenController>();
                if (theController)
                {
                    if ((config.MountNeeded && aPlayer.GetMountedVehicle() != theVehicle) || !theController.NetIDs.Contains(aButton.net.ID))
                    {
                        return false;
                    }
                    theController.ChangeState();
                }
            }
            return null;
        }

        private void OnEntitySpawned(BaseNetworkable anEntity)
        {
            BaseVehicle theVehicle = anEntity as BaseVehicle;
            if (theVehicle)
            {
                SirenController theController = theVehicle.GetComponent<SirenController>();
                if (!theController)
                {
                    float theProbability;
                    if (config.SirenSpawnProbability.TryGetValue(theVehicle is BaseVehicle ? KEY_MODULAR_CAR : theVehicle.PrefabName, out theProbability) && Core.Random.Range(0f, 1f) < theProbability)
                    {
                        AttachSirens(theVehicle, SirenDictionary.Values.First(), config.DefaultState);
                    }
                }
            }
        }
        #endregion hooks

        #region methods
        /// <summary>
        /// Tries to attach the given siren to the given vehicle.
        /// </summary>
        /// <param name="aVehicle">The vehicle.</param>
        /// <param name="aSiren">The siren.</param>
        /// <param name="anInitialState">The initial siren state.</param>
        /// <param name="aPlayer">The calling player.</param>
        private void AttachSirens(BaseVehicle aVehicle, Siren aSiren, SirenController.States anInitialState, IPlayer aPlayer = null)
        {
            DetachSirens(aVehicle);
            SirenController theController = CreateSirenController(aVehicle, aSiren);
            if (aVehicle as ModularCar)
            {
                foreach (BaseVehicleModule eachModule in aVehicle.GetComponentsInChildren<BaseVehicleModule>())
                {
                    SpawnAttachments(aSiren.Modules, aPlayer, theController, eachModule);
                }
            } else if (!SpawnAttachments(aSiren.Vehicles, aPlayer, theController, aVehicle))
            {
                Message(aPlayer, I18N_NOT_SUPPORTED, aSiren.Name, aVehicle.PrefabName);
                DetachSirens(aVehicle);
                return;
            }
            theController.SetState(anInitialState);
        }

        /// <summary>
        /// Spawns the attachments for the given dictionary for the given parent entity.
        /// </summary>
        /// <param name="someAttachments">The dictionary.</param>
        /// <param name="aPlayer">The calling player.</param>
        /// <param name="theController">The SirenController of the Parent.</param>
        /// <param name="aParent">The Parent.</param>
        /// <returns>True, if the parent has an entry in the dictionary with at least one Attachment.</returns>
        private bool SpawnAttachments(IDictionary<string, Attachment[]> someAttachments, IPlayer aPlayer, SirenController theController, BaseEntity aParent)
        {
            Attachment[] theAttachments;
            if (someAttachments.TryGetValue(aParent.PrefabName, out theAttachments))
            {
                foreach (Attachment eachAttachment in theAttachments)
                {
                    BaseEntity theNewEntity = AttachEntity(aParent, eachAttachment.Prefab, eachAttachment.Position, eachAttachment.Angle);
                    if (!theNewEntity)
                    {
                        theController.NetIDs.Add(theNewEntity.net.ID);
                        if (aPlayer != null)
                        {
                            Message(aPlayer, I18N_COULD_NOT_ATTACH, eachAttachment.Prefab);
                        }
                    }
                }
                return !theAttachments.IsEmpty();
            }
            return false;
        }

        private SirenController CreateSirenController(BaseVehicle aVehicle, Siren aSiren, IEnumerable<uint> someNetIDs = null)
        {
            SirenController theController = aVehicle.GetComponent<SirenController>();
            if (theController)
            {
                UnityEngine.Object.DestroyImmediate(theController);
            }
            theController = aVehicle.gameObject.AddComponent<SirenController>();
            theController.Config = config;
            theController.Siren = aSiren;
            if (someNetIDs != null)
            {
                theController.NetIDs.UnionWith(someNetIDs);
            }
            return theController;
        }

        private bool DetachSirens(BaseVehicle aVehicle)
        {
            SirenController theController = aVehicle.GetComponent<SirenController>();
            if (theController)
            {
                foreach (BaseEntity eachEntity in aVehicle.GetComponentsInChildren<BaseEntity>())
                {
                    if (theController.NetIDs.Contains(eachEntity.net.ID))
                    {
                        Destroy(eachEntity);
                    }
                }
                UnityEngine.Object.DestroyImmediate(theController);
                return true;
            }
            return false;
        }

        private static void Destroy(BaseEntity anEntity)
        {
            if (!anEntity.IsDestroyed)
            {
                anEntity.Kill();
            }
        }

        private BaseEntity AttachEntity(BaseEntity aParent, string aPrefab, Vector3 aPosition, Vector3 anAngle = new Vector3())
        {
            BaseEntity theNewEntity = GameManager.server.CreateEntity(aPrefab, aParent.transform.position);
            if (!theNewEntity)
            {
                return null;
            }

            theNewEntity.Spawn();
            theNewEntity.SetParent(aParent);
            theNewEntity.transform.localEulerAngles = anAngle;
            theNewEntity.transform.localPosition = aPosition;
            UnityEngine.Object.DestroyImmediate(theNewEntity.GetComponent<DestroyOnGroundMissing>());
            UnityEngine.Object.DestroyImmediate(theNewEntity.GetComponent<GroundWatch>());
            UnityEngine.Object.DestroyImmediate(theNewEntity.GetComponent<BoxCollider>());
            UnityEngine.Object.DestroyImmediate(theNewEntity.GetComponent<InstrumentKeyController>());
            theNewEntity.OwnerID = 0;
            BaseCombatEntity theCombatEntity = theNewEntity as BaseCombatEntity;
            if (theCombatEntity)
            {
                theCombatEntity.pickup.enabled = false;
                theCombatEntity.diesAtZeroHealth = false;
            }
            PressButton theButton = theNewEntity as PressButton;
            if (theButton)
            {
                theButton.pressDuration = 0.2f;
            }

            theNewEntity.EnableSaving(true);
            theNewEntity.SendNetworkUpdateImmediate();
            return theNewEntity;
        }

        private static void ToogleSirens(IOEntity anIOEntity, bool theEnabledFlag)
        {
            anIOEntity.UpdateHasPower(theEnabledFlag ? anIOEntity.ConsumptionAmount() : 0, 0);
            anIOEntity.SetFlag(BaseEntity.Flags.On, theEnabledFlag);
        }
        #endregion methods

        #region helpers
        private BaseVehicle RaycastVehicle(IPlayer aPlayer)
        {
            RaycastHit theHit;
            if (!Physics.Raycast((aPlayer.Object as BasePlayer).eyes.HeadRay(), out theHit, 5f))
            {
                return null;
            }

            BaseVehicle theVehicle = theHit.GetEntity()?.GetComponentInParent<BaseVehicle>();
            if (!theVehicle)
            {
                Message(aPlayer, I18N_NOT_A_VEHICLE);
            }
            return theVehicle;
        }

        private Siren FindSirenForName(string aName, IPlayer aPlayer)
        {
            Siren theSiren;
            if (!SirenDictionary.TryGetValue(aName, out theSiren))
            {
                theSiren = SirenDictionary.Values.First();
                Message(aPlayer, I18N_MISSING_SIREN, theSiren.Name);
            }
            return theSiren;
        }

        private string GetText(string aKey, string aPlayerId = null, params object[] someArgs) => string.Format(lang.GetMessage(aKey, this, aPlayerId), someArgs);

        private void Message(IPlayer aPlayer, string anI18nKey, params object[] someArgs)
        {
            if (aPlayer.IsConnected)
            {
                string theText = GetText(anI18nKey, aPlayer.Id, someArgs);
                aPlayer.Reply(theText != anI18nKey ? theText : anI18nKey);
            }
        }

        private void Message(BasePlayer aPlayer, string anI18nKey, params object[] someArgs)
        {
            if (aPlayer.IsConnected)
            {
                string theText = GetText(anI18nKey, aPlayer.UserIDString, someArgs);
                aPlayer.ChatMessage(theText != anI18nKey ? theText : anI18nKey);
            }
        }
        #endregion helpers

        #region controllers
        private class SirenController : FacepunchBehaviour
        {
            public enum States {
                OFF,
                ON,
                LIGHTS_ONLY
            }

            private BaseVehicle vehicle;
            private InstrumentTool trumpet;
            public Configuration Config { get; set; }
            public States State { get; private set; }
            public Siren Siren { get; set; }
            public ISet<uint> NetIDs { get; } = new HashSet<uint>();

            public States ChangeState()
            {
                SetState(State >= States.LIGHTS_ONLY ? States.OFF : State + 1);
                return State;
            }

            public void SetState(States aState)
            {
                State = aState;
                if ((!Config.SoundEnabled || Siren?.Tones?.Length < 1 || !GetTrumpet()) && State == States.ON)
                {
                    State++;
                }
                RefreshSirenState();
            }

            public void RefreshSirenState()
            {
                if (State == States.ON) {
                    PlayTone(0);
                }
                bool theLightsOnFlag = State > States.OFF;
                foreach (IOEntity eachEntity in GetVehicle().GetComponentsInChildren<IOEntity>())
                {
                    if (NetIDs.Contains(eachEntity.net.ID) && !(eachEntity is PressButton))
                    {
                        ToogleSirens(eachEntity, theLightsOnFlag);
                    }
                }
            }

            private InstrumentTool GetTrumpet()
            {
                if (trumpet == null || trumpet.IsDestroyed)
                {
                    trumpet = GetVehicle().GetComponentInChildren<InstrumentTool>();
                }
                return trumpet;
            }

            private BaseVehicle GetVehicle()
            {
                if (vehicle == null)
                {
                    vehicle = GetComponentInParent<BaseVehicle>();
                }
                return vehicle;
            }

            private void PlayTone(int anIndex)
            {
                if (State != States.ON || !GetTrumpet())
                {
                    return;
                }
                if (anIndex >= Siren.Tones.Length)
                {
                    anIndex = 0;
                }
                Tone theTone = Siren.Tones[anIndex];
                GetTrumpet().ClientRPC(null, "Client_PlayNote", (int) theTone.Note, (int) theTone.NoteType, theTone.Octave, 1f);
                Invoke(() => GetTrumpet().ClientRPC(null, "Client_StopNote", (int)theTone.Note, (int)theTone.NoteType, theTone.Octave), theTone.Duration);
                Invoke(() => PlayTone(++anIndex), theTone.Duration);
            }
        }
        #endregion controllers
    }
}
