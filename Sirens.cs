using Newtonsoft.Json;
using Rust.Instruments;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static InstrumentKeyController;
using Oxide.Core;

namespace Oxide.Plugins
{
    [Info("Sirens", "ZockiRR", "1.1.1")]
    [Description("Gives players the ability to attach sirens to modular cars")]
    class Sirens : RustPlugin
    {
        #region variables
        private const string PERMISSION_ATTACHSIRENS = "sirens.attachsirens";
        private const string PERMISSION_DETACHSIRENS = "sirens.detachsirens";

        private const string I18N_MISSING_PERMISSION_ATTACHSIRENS = "NoPermissionAttachSirens";
        private const string I18N_MISSING_PERMISSION_DETACHSIRENS = "NoPermissionDetachSirens";
        private const string I18N_MISSING_SIREN = "NoSirenForName";
        private const string I18N_COULD_NOT_ATTACH = "CouldNotAttach";
        private const string I18N_ATTACHED = "Attached";
        private const string I18N_NOT_A_CAR = "NotACar";

        // Initial prefabs
        private const string PREFAB_COCKPIT = "assets/content/vehicles/modularcar/module_entities/1module_cockpit.prefab";
        private const string PREFAB_COCKPIT_ARMORED = "assets/content/vehicles/modularcar/module_entities/1module_cockpit_armored.prefab";
        private const string PREFAB_COCKPIT_WITH_ENGINE = "assets/content/vehicles/modularcar/module_entities/1module_cockpit_with_engine.prefab";
        private const string PREFAB_BUTTON = "assets/prefabs/io/electric/switches/pressbutton/pressbutton.prefab";
        private const string PREFAB_FLASHERLIGHT = "assets/prefabs/deployable/playerioents/lights/flasherlight/electric.flasherlight.deployed.prefab";
        private const string PREFAB_SIRENLIGHT = "assets/prefabs/deployable/playerioents/lights/sirenlight/electric.sirenlight.deployed.prefab";
        private const string PREFAB_TRUMPET = "assets/prefabs/instruments/trumpet/trumpet.weapon.prefab";

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
            });

        // Siren-Name-Mapping
        private readonly Dictionary<string, Siren> SirenMapping = new Dictionary<string, Siren>();
        #endregion variables

        #region Data
        private class DataContainer
        {
            // Map ModularCar.net.ID -> Siren
            public Dictionary<uint, CarContainer> CarSirenMap = new Dictionary<uint, CarContainer>();
        }

        private class CarContainer
        {
            public Siren Siren;

            public CarContainer(Siren aSiren)
            {
                Siren = aSiren;
            }
        }
        #endregion Data

        #region Configuration

        private Configuration config;

        private class Configuration
        {
            [JsonProperty("MountNeeded")]
            public bool MountNeeded = true;

            [JsonProperty("SoundEnabled")]
            public bool SoundEnabled = true;

            [JsonProperty("Sirens")]
            public Siren[] Sirens = { SIREN_DEFAULT, SIREN_SILENT };

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

            [JsonProperty("Note")]
            public Notes Note;

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
            public Siren(string aName, Dictionary<string, Attachment[]> someModules, params Tone[] someTones)
            {
                Name = aName;
                Modules = someModules;
                Tones = someTones;
            }

            [JsonProperty("Name")]
            public string Name;

            [JsonProperty("Tones")]
            public Tone[] Tones;

            [JsonProperty("Modules")]
            public Dictionary<string, Attachment[]> Modules;

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

        protected override void LoadDefaultConfig() => config = new Configuration();

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

                if (!config.ToDictionary().Keys.SequenceEqual(Config.ToDictionary(x => x.Key, x => x.Value).Keys))
                {
                    PrintWarning("Configuration appears to be outdated; updating and saving");
                    SaveConfig();
                }

                foreach (Siren eachSiren in config.Sirens)
                {
                    if (!SirenMapping.ContainsKey(eachSiren.Name))
                    {
                        SirenMapping.Add(eachSiren.Name, eachSiren);
                    }
                }
                if (SirenMapping.IsEmpty())
                {
                    SirenMapping.Add(SIREN_DEFAULT.Name, SIREN_DEFAULT);
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
        }

        #endregion Configuration

        #region localization
        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                [I18N_MISSING_PERMISSION_ATTACHSIRENS] = "You are not allowed to attach car sirens",
                [I18N_MISSING_PERMISSION_DETACHSIRENS] = "You are not allowed to detach car sirens",
                [I18N_MISSING_SIREN] = "No siren was found for the given name (using {0} instead)",
                [I18N_COULD_NOT_ATTACH] = "Could not attach \"{0}\"",
                [I18N_ATTACHED] = "Attached siren \"{0}\"",
                [I18N_NOT_A_CAR] = "This entity is not a car"
            }, this);
        }
        #endregion localization

        #region chatommands
        [ChatCommand("attachsirens")]
        private void AttachCarSirens(BasePlayer aPlayer, string aCommand, string[] someArgs)
        {
            if (!permission.UserHasPermission(aPlayer.UserIDString, PERMISSION_ATTACHSIRENS))
            {
                aPlayer.ChatMessage(Lang(I18N_MISSING_PERMISSION_ATTACHSIRENS, aPlayer.UserIDString));
                return;
            }

            ModularCar aCar = RaycastVehicleModule(aPlayer);
            if (aCar)
            {
                Siren theSiren = someArgs.Length > 0 ? FindSirenForName(someArgs[0], aPlayer) : SirenMapping.Values.First();
                AttachCarSirens(aCar, theSiren, aPlayer);
            }
        }

        [ChatCommand("detachsirens")]
        private void DetachCarSirens(BasePlayer aPlayer, string aCommand, string[] someArgs)
        {
            if (!permission.UserHasPermission(aPlayer.UserIDString, PERMISSION_DETACHSIRENS))
            {
                aPlayer.ChatMessage(Lang(I18N_MISSING_PERMISSION_DETACHSIRENS, aPlayer.UserIDString));
                return;
            }

            ModularCar aCar = RaycastVehicleModule(aPlayer);
            if (aCar)
            {
                DetachCarSirens(aCar);
            }
        }
        #endregion chatommands

        #region hooks
        private void Unload()
        {
            OnServerSave();

            foreach (ModularCar eachCar in BaseNetworkable.serverEntities.OfType<ModularCar>())
            {
                DetachCarSirens(eachCar);
            }
        }

        private void OnServerSave()
        {
            DataContainer thePersistentData = new DataContainer();
            foreach (ModularCar eachCar in BaseNetworkable.serverEntities.OfType<ModularCar>())
            {
                SirenController theController = eachCar.GetComponent<SirenController>();
                if (theController)
                {
                    thePersistentData.CarSirenMap.Add(eachCar.net.ID, new CarContainer(theController.Siren));
                }
            }
            Interface.Oxide.DataFileSystem.WriteObject(Name, thePersistentData);
        }

        private void OnServerInitialized(bool anInitialFlag)
        {
            permission.RegisterPermission(PERMISSION_ATTACHSIRENS, this);
            permission.RegisterPermission(PERMISSION_DETACHSIRENS, this);

            // Reattach on server restart
            DataContainer thePersistentData = Interface.Oxide.DataFileSystem.ReadObject<DataContainer>(Name);
            foreach (uint eachCarID in thePersistentData.CarSirenMap.Keys)
            {
                ModularCar theCar = BaseNetworkable.serverEntities.Find(eachCarID).GetComponent<ModularCar>();
                if (theCar)
                {
                    CarContainer theCarContainer = thePersistentData.CarSirenMap[eachCarID];
                    // Create SirenController first, so that eventually existings siren entites can be detached
                    CreateSirenController(theCar, theCarContainer.Siren);
                    AttachCarSirens(theCar, theCarContainer.Siren);
                }
            }
        }

        private object OnButtonPress(PressButton aButton, BasePlayer aPlayer)
        {
            ModularCar theCar = aButton.GetComponentInParent<ModularCar>();
            if (theCar)
            {
                if (config.MountNeeded && aPlayer.GetMountedVehicle() != theCar)
                {
                    return false;
                }
                SirenController theController = theCar.GetComponent<SirenController>();
                if (theController)
                {
                    theController.ChangeState();
                }
            }
            return null;
        }
        #endregion hooks

        #region methods
        private void AttachCarSirens(ModularCar aCar, Siren aSiren, BasePlayer aPlayer = null)
        {
            DetachCarSirens(aCar);
            CreateSirenController(aCar, aSiren);
            foreach (BaseVehicleModule eachModule in aCar.GetComponentsInChildren<BaseVehicleModule>())
            {
                Attachment[] theAttachments;
                if (aSiren.Modules.TryGetValue(eachModule.PrefabName, out theAttachments))
                {
                    foreach (Attachment eachAttachment in theAttachments)
                    {
                        BaseEntity theNewEntity = AttachEntityToModule(eachModule, eachAttachment.Prefab, eachAttachment.Position, eachAttachment.Angle);
                        if (!theNewEntity)
                        {
                            aPlayer?.ChatMessage(Lang(I18N_COULD_NOT_ATTACH, aPlayer.UserIDString, eachAttachment.Prefab));
                        }
                    }
                }
            }
            aPlayer?.ChatMessage(Lang(I18N_ATTACHED, aPlayer.UserIDString, aSiren.Name));
        }

        private SirenController CreateSirenController(ModularCar aCar, Siren aSiren)
        {
            SirenController theController = aCar.GetComponent<SirenController>();
            if (theController)
            {
                UnityEngine.Object.DestroyImmediate(theController);
            }
            theController = aCar.gameObject.AddComponent<SirenController>();
            theController.Config = config;
            theController.Siren = aSiren;
            return theController;
        }

        private void DetachCarSirens(ModularCar aCar)
        {
            SirenController theController = aCar.GetComponent<SirenController>();
            if (theController)
            {
                foreach (BaseEntity eachEntity in aCar.GetComponentsInChildren<BaseEntity>())
                {
                    if (theController.EntityPrefabs.Contains(eachEntity.PrefabName))
                    {
                        Destroy(eachEntity);
                    }
                }
                UnityEngine.Object.Destroy(theController);
            }
        }

        private static void Destroy(BaseEntity anEntity)
        {
            if (!anEntity.IsDestroyed)
            {
                anEntity.Kill();
            }
        }

        private BaseEntity AttachEntityToModule(BaseVehicleModule aModule, string aPrefab, Vector3 aPosition, Vector3 anAngle = new Vector3())
        {
            BaseEntity theNewEntity = GameManager.server.CreateEntity(aPrefab, aModule.transform.position);
            if (!theNewEntity)
            {
                return null;
            }

            theNewEntity.Spawn();
            theNewEntity.SetParent(aModule);
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
        private ModularCar RaycastVehicleModule(BasePlayer aPlayer)
        {
            RaycastHit theHit;
            if (!Physics.Raycast(aPlayer.eyes.HeadRay(), out theHit, 5f))
            {
                return null;
            }

            ModularCar theCar = theHit.GetEntity()?.GetComponentInParent<ModularCar>();
            if (!theCar)
            {
                aPlayer.ChatMessage(Lang(I18N_NOT_A_CAR, aPlayer.UserIDString));
            }
            return theCar;
        }

        private Siren FindSirenForName(string aName, BasePlayer aPlayer)
        {
            Siren theSiren;
            if (!SirenMapping.TryGetValue(aName, out theSiren))
            {
                theSiren = SirenMapping.Values.First();
                aPlayer.ChatMessage(Lang(I18N_MISSING_SIREN, aPlayer.UserIDString, theSiren.Name));
            }
            return theSiren;
        }

        private string Lang(string aKey, string aUserID = null, params object[] someArgs) => string.Format(lang.GetMessage(aKey, this, aUserID), someArgs);
        #endregion helpers

        #region controllers
        private class SirenController : FacepunchBehaviour
        {
            private enum State {
                OFF,
                ON,
                LIGHTS_ONLY
            }

            private State state = State.OFF;
            private ModularCar car;
            private InstrumentTool trumpet;
            private HashSet<string> entityPrefabs;
            private Siren siren;
            public Configuration Config { get; set; }
            public Siren Siren {
                get { return siren; }
                set { entityPrefabs = null; siren = value; }
            }
            public HashSet<string> EntityPrefabs {
                get
                {
                    if (entityPrefabs == null)
                    {
                        entityPrefabs = new HashSet<string>(Siren.Modules.Values.SelectMany(x => x).Select(x => x.Prefab));
                    }
                    return entityPrefabs;
                }
            }

            public void ChangeState()
            {
                state = state >= State.LIGHTS_ONLY ? State.OFF : state + 1;
                if ((!Config.SoundEnabled || Siren?.Tones?.Length < 1 || !GetTrumpet()) && state == State.ON)
                {
                    state++;
                }
                RefreshSirenState();
            }

            public void Off()
            {
                state = State.OFF;
                RefreshSirenState();
            }

            public void RefreshSirenState()
            {
                if (state == State.ON) {
                    PlayTone(0);
                }
                bool theLightsOnFlag = state > State.OFF;
                foreach (IOEntity eachEntity in GetCar().GetComponentsInChildren<IOEntity>())
                {
                    if (EntityPrefabs.Contains(eachEntity.PrefabName) && !(eachEntity is PressButton))
                    {
                        ToogleSirens(eachEntity, theLightsOnFlag);
                    }
                }
            }

            private InstrumentTool GetTrumpet()
            {
                if (trumpet == null || trumpet.IsDestroyed)
                {
                    trumpet = GetCar().GetComponentInChildren<InstrumentTool>();
                }
                return trumpet;
            }

            private ModularCar GetCar()
            {
                if (car == null)
                {
                    car = GetComponentInParent<ModularCar>();
                }
                return car;
            }

            private void PlayTone(int anIndex)
            {
                if (state != State.ON)
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
