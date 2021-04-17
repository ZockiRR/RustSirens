using Newtonsoft.Json;
using Rust.Instruments;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static InstrumentKeyController;

namespace Oxide.Plugins
{
    [Info("Sirens", "ZockiRR", "1.0.0")]
    [Description("Gives players the ability to attach sirens to modular cars")]
    class Sirens : RustPlugin
    {

        #region variables
        private const string PERMISSION_ATTACHSIRENS = "sirens.attachsirens";
        private const string PERMISSION_DETACHSIRENS = "sirens.detachsirens";

        private const string I18N_MISSING_PERMISSION_ATTACHSIRENS = "NoPermissionAttachSirens";
        private const string I18N_MISSING_PERMISSION_DETACHSIRENS = "NoPermissionDetachSirens";
        private const string I18N_MISSING_SIREN = "NoSirenForName";
        private const string I18N_NOT_A_CAR = "NotACar";

        private static readonly Siren SIREN_DEFAULT = new Siren("police-germany", new Tone(Notes.A, NoteType.Regular, 4, 1f), new Tone(Notes.D, NoteType.Regular, 5, 1f));

        // Initially possible modules
        private const string PREFAB_COCKPIT = "assets/content/vehicles/modularcar/module_entities/1module_cockpit.prefab";
        private const string PREFAB_COCKPIT_ARMORED = "assets/content/vehicles/modularcar/module_entities/1module_cockpit_armored.prefab";
        private const string PREFAB_COCKPIT_WITH_ENGINE = "assets/content/vehicles/modularcar/module_entities/1module_cockpit_with_engine.prefab";

        private readonly Dictionary<string, Siren> SirenMapping = new Dictionary<string, Siren>();
        #endregion variables

        #region Configuration

        private Configuration config;

        private class Configuration
        {
            [JsonProperty("MountNeeded")]
            public bool MountNeeded = true;

            [JsonProperty("SoundEnabled")]
            public bool SoundEnabled = true;
            [JsonProperty("Sirens")]
            public Siren[] Sirens = { SIREN_DEFAULT };

            [JsonProperty("LeftSirenPositions")]
            public Dictionary<string, Vector3> LeftSirenPositions = new Dictionary<string, Vector3>
            {
                [PREFAB_COCKPIT] = new Vector3(-0.4f, 1.4f, -0.9f),
                [PREFAB_COCKPIT_ARMORED] = new Vector3(-0.4f, 1.4f, -0.9f),
                [PREFAB_COCKPIT_WITH_ENGINE] = new Vector3(-0.4f, 1.4f, -0.9f)
            };

            [JsonProperty("LeftSirenAngles")]
            public Dictionary<string, Vector3> LeftSirenAngles = new Dictionary<string, Vector3>();

            [JsonProperty("RightSirenPositions")]
            public Dictionary<string, Vector3> RightSirenPositions = new Dictionary<string, Vector3>
            {
                [PREFAB_COCKPIT] = new Vector3(0.4f, 1.4f, -0.9f),
                [PREFAB_COCKPIT_ARMORED] = new Vector3(0.4f, 1.4f, -0.9f),
                [PREFAB_COCKPIT_WITH_ENGINE] = new Vector3(0.4f, 1.4f, -0.9f)
            };

            [JsonProperty("RightSirenAngles")]
            public Dictionary<string, Vector3> RightSirenAngles = new Dictionary<string, Vector3>();

            [JsonProperty("TrumpetPositions")]
            public Dictionary<string, Vector3> TrumpetPositions = new Dictionary<string, Vector3>
            {
                [PREFAB_COCKPIT] = new Vector3(-0.08f, 1.4f, -0.9f),
                [PREFAB_COCKPIT_ARMORED] = new Vector3(-0.08f, 1.4f, -0.9f),
                [PREFAB_COCKPIT_WITH_ENGINE] = new Vector3(-0.08f, 1.4f, -0.9f)
            };

            [JsonProperty("TrumpetAngles")]
            public Dictionary<string, Vector3> TrumpetAngles = new Dictionary<string, Vector3>
            {
                [PREFAB_COCKPIT] = new Vector3(148f, 150f, 30f),
                [PREFAB_COCKPIT_ARMORED] = new Vector3(148f, 150f, 30f),
                [PREFAB_COCKPIT_WITH_ENGINE] = new Vector3(148f, 150f, 30f)
            };

            [JsonProperty("ButtonPositions")]
            public Dictionary<string, Vector3> ButtonPositions = new Dictionary<string, Vector3>
            {
                [PREFAB_COCKPIT] = new Vector3(0.05f, 1.7f, 0.78f),
                [PREFAB_COCKPIT_ARMORED] = new Vector3(0.05f, 1.7f, 0.78f),
                [PREFAB_COCKPIT_WITH_ENGINE] = new Vector3(0.05f, 1.7f, 0.78f)
            };

            [JsonProperty("ButtonAngles")]
            public Dictionary<string, Vector3> ButtonAngles = new Dictionary<string, Vector3>
            {
                [PREFAB_COCKPIT] = new Vector3(210f, 0f, 0f),
                [PREFAB_COCKPIT_ARMORED] = new Vector3(210f, 0f, 0f),
                [PREFAB_COCKPIT_WITH_ENGINE] = new Vector3(210f, 0f, 0f)
            };

            [JsonProperty("PrefabFlasherLight")]
            public string PrefabFlasherLight = "assets/prefabs/deployable/playerioents/lights/flasherlight/electric.flasherlight.deployed.prefab";

            [JsonProperty("PrefabTrumpet")]
            public string PrefabTrumpet = "assets/prefabs/instruments/trumpet/trumpet.weapon.prefab";

            [JsonProperty("PrefabButton")]
            public string PrefabButton = "assets/prefabs/deployable/playerioents/button/button.prefab";

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
            public Siren(string aName = "new-siren", params Tone[] someTones)
            {
                Name = aName;
                Tones = someTones;
            }

            [JsonProperty("Name")]
            public string Name;

            [JsonProperty("Tones")]
            public Tone[] Tones;

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
                AttachCarSirens(aCar, theSiren);
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
        private void Init()
        {
            permission.RegisterPermission(PERMISSION_ATTACHSIRENS, this);
            permission.RegisterPermission(PERMISSION_DETACHSIRENS, this);
        }

        private object OnButtonPress(PressButton aButton, BasePlayer aPlayer)
        {
            ModularCar theCar = aButton.GetComponentInParent<ModularCar>();
            if (theCar)
            {
                if (config.MountNeeded && aPlayer.GetMountedVehicle() != theCar)
                {
                    return aButton;
                }
                SirenController theController = theCar.GetComponent<SirenController>();
                if (theController)
                {
                    theController.ChangeState();
                }
            }
            return null;
        }

        private void Unload()
        {
            foreach (ModularCar eachCar in UnityEngine.Object.FindObjectsOfType<ModularCar>())
            {
                DetachCarSirens(eachCar);
            }
        }

        #endregion hooks

        #region methods
        private void AttachCarSirens(ModularCar aCar, Siren aSiren)
        {
            if (!aCar.GetComponent<SirenController>())
            {
                aCar.gameObject.AddComponent<SirenController>().Config = config;
            }
            foreach (BaseVehicleModule eachModule in aCar.GetComponentsInChildren<BaseVehicleModule>())
            {
                if (!eachModule.GetComponentInChildren<FlasherLight>()) {
                    AttachEntityToModule<FlasherLight>(eachModule, config.PrefabFlasherLight, config.LeftSirenPositions, config.LeftSirenAngles);
                    AttachEntityToModule<FlasherLight>(eachModule, config.PrefabFlasherLight, config.RightSirenPositions, config.RightSirenAngles);
                }
                if (!eachModule.GetComponentInChildren<PressButton>()) {
                    PressButton theButton = AttachEntityToModule<PressButton>(eachModule, config.PrefabButton, config.ButtonPositions, config.ButtonAngles);
                    if (theButton) {
                        theButton.pressDuration = 0.2f;
                    }
                }
                if (!eachModule.GetComponentInChildren<InstrumentTool>()) {
                    AttachEntityToModule<InstrumentTool>(eachModule, config.PrefabTrumpet, config.TrumpetPositions, config.TrumpetAngles);
                }
            }

            SirenController theController = aCar.GetComponent<SirenController>();
            theController.Siren = aSiren;
            theController.RefreshSirenState();
        }

        private void DetachCarSirens(ModularCar aCar)
        {
            SirenController theComponent = aCar.GetComponent<SirenController>();
            UnityEngine.Object.Destroy(theComponent);

            foreach (FlasherLight eachFlasherLight in aCar.GetComponentsInChildren<FlasherLight>())
            {
                Destroy(eachFlasherLight);
            }

            foreach (PressButton eachButton in aCar.GetComponentsInChildren<PressButton>())
            {
                Destroy(eachButton);
            }

            foreach (InstrumentTool eachTrumpet in aCar.GetComponentsInChildren<InstrumentTool>())
            {
                Destroy(eachTrumpet);
            }
        }

        private static void Destroy(BaseEntity anEntity)
        {
            if (!anEntity.IsDestroyed)
            {
                anEntity.Kill();
            }
        }

        private T AttachEntityToModule<T>(BaseVehicleModule aModule, string aPrefab, Dictionary<string, Vector3> aPositionMapping, Dictionary<string, Vector3> anAnglesMapping = null) where T : BaseEntity
        {
            Vector3 thePosition;
            if (!aPositionMapping.TryGetValue(aModule.PrefabName, out thePosition))
            {
                return null;
            }

            T theNewEntity = GameManager.server.CreateEntity(aPrefab, aModule.transform.position)?.GetComponent<T>();
            if (!theNewEntity)
            {
                return null;
            }

            theNewEntity.Spawn();
            theNewEntity.SetParent(aModule);
            if (anAnglesMapping != null)
            {
                Vector3 theAngles;
                if (anAnglesMapping.TryGetValue(aModule.PrefabName, out theAngles))
                {
                    theNewEntity.transform.localEulerAngles = theAngles;
                }
            }
            theNewEntity.transform.localPosition = thePosition;
            UnityEngine.Object.DestroyImmediate(theNewEntity.GetComponent<DestroyOnGroundMissing>());
            UnityEngine.Object.DestroyImmediate(theNewEntity.GetComponent<GroundWatch>());
            theNewEntity.OwnerID = 0;
            BaseCombatEntity theCombatEntity = theNewEntity as BaseCombatEntity;
            if (theCombatEntity)
            {
                theCombatEntity.pickup.enabled = false;
                theCombatEntity.diesAtZeroHealth = false;
            }

            theNewEntity.EnableSaving(true);
            theNewEntity.SendNetworkUpdateImmediate();
            return theNewEntity;
        }

        private static void ToogleSirens(FlasherLight aFlasherLight, bool theEnabledFlag)
        {
            aFlasherLight.UpdateHasPower(theEnabledFlag ? aFlasherLight.ConsumptionAmount() : 0, 0);
            aFlasherLight.SetFlag(BaseEntity.Flags.On, theEnabledFlag);
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
            public Configuration Config { get; set; }
            public Siren Siren { get; set; }

            public void ChangeState()
            {
                state = state >= State.LIGHTS_ONLY ? State.OFF : state + 1;
                if ((!Config.SoundEnabled || Siren?.Tones?.Length < 1) && state == State.ON)
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
                if (state == State.ON && GetTrumpet() != null) {
                    PlayTone(0);
                }
                bool theLightsOnFlag = state > State.OFF;
                foreach (FlasherLight eachFlasherLight in GetCar().GetComponentsInChildren<FlasherLight>())
                {
                    ToogleSirens(eachFlasherLight, theLightsOnFlag);
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
