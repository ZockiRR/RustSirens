using System.Collections.Generic;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Sirens", "ZockiRR", "1.1.0")]
    [Description("Gives players the ability to put sirens on modular cars and other vehicles")]
    class Sirens : RustPlugin
    {

        #region variables
        private const string flasherLightPrefab = "assets/prefabs/deployable/playerioents/lights/flasherlight/electric.flasherlight.deployed.prefab";
        private const string buttonPrefab = "assets/prefabs/deployable/playerioents/button/button.prefab";
        private const string trumpetPrefab = "assets/prefabs/instruments/trumpet/trumpet.weapon.prefab";

        private const string cockpitPrefab = "assets/content/vehicles/modularcar/module_entities/1module_cockpit.prefab";
        private const string cockpitArmoredPrefab = "assets/content/vehicles/modularcar/module_entities/1module_cockpit_armored.prefab";
        private const string cockpitWithEnginePrefab = "assets/content/vehicles/modularcar/module_entities/1module_cockpit_with_engine.prefab";

        private const string attachsirenspermissionName = "sirens.attachsirens";

        private readonly Dictionary<string, Vector3> leftSirenPosition = new Dictionary<string, Vector3>
        {
            [cockpitPrefab] = new Vector3(-0.4f, 1.4f, -0.9f),
            [cockpitArmoredPrefab] = new Vector3(-0.4f, 1.4f, -0.9f),
            [cockpitWithEnginePrefab] = new Vector3(-0.4f, 1.4f, -0.9f)
        };
        private readonly Dictionary<string, Vector3> rightSirenPosition = new Dictionary<string, Vector3>
        {
            [cockpitPrefab] = new Vector3(0.4f, 1.4f, -0.9f),
            [cockpitArmoredPrefab] = new Vector3(0.4f, 1.4f, -0.9f),
            [cockpitWithEnginePrefab] = new Vector3(0.4f, 1.4f, -0.9f)
        };
        private readonly Dictionary<string, Vector3> trumpetPosition = new Dictionary<string, Vector3>
        {
            [cockpitPrefab] = new Vector3(-0.08f, 1.4f, -0.9f),
            [cockpitArmoredPrefab] = new Vector3(-0.08f, 1.4f, -0.9f),
            [cockpitWithEnginePrefab] = new Vector3(-0.08f, 1.4f, -0.9f)
        };
        private readonly Dictionary<string, Vector3> trumpetAngles = new Dictionary<string, Vector3>
        {
            [cockpitPrefab] = new Vector3(148f, 150f, 30f),
            [cockpitArmoredPrefab] = new Vector3(148f, 150f, 30f),
            [cockpitWithEnginePrefab] = new Vector3(148f, 150f, 30f)
        };
        private readonly Dictionary<string, Vector3> buttonPosition = new Dictionary<string, Vector3>
        {
            [cockpitPrefab] = new Vector3(0.05f, 1.7f, 0.78f),
            [cockpitArmoredPrefab] = new Vector3(0.05f, 1.7f, 0.78f),
            [cockpitWithEnginePrefab] = new Vector3(0.05f, 1.7f, 0.78f)
        };
        private readonly Dictionary<string, Vector3> buttonAngles = new Dictionary<string, Vector3>
        {
            [cockpitPrefab] = new Vector3(210f, 0f, 0f),
            [cockpitArmoredPrefab] = new Vector3(210f, 0f, 0f),
            [cockpitWithEnginePrefab] = new Vector3(210f, 0f, 0f)
        };
        #endregion variables

        #region localization
        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["No Permission AttachSirens"] = "You are not allowed to attach car sirens",
                ["Not A Car"] = "This entity is not a car"
            }, this);
        }
        #endregion localization

        #region chatommands
        [ChatCommand("attachsirens")]
        private void AttachCarSirens(BasePlayer aPlayer, string aCommand, string[] someArgs)
        {
            ModularCar aCar = RaycastVehicleModule(aPlayer);
            if (aCar)
            {
                AttachCarSirens(aCar, aPlayer);
            }
        }

        [ChatCommand("detachsirens")]
        private void DetachCarSirens(BasePlayer aPlayer, string aCommand, string[] someArgs)
        {
            ModularCar aCar = RaycastVehicleModule(aPlayer);
            if (aCar)
            {
                DetachCarSirens(aCar, aPlayer);
            }
        }
        #endregion chatommands

        #region hooks
        private void Init()
        {
            permission.RegisterPermission(attachsirenspermissionName, this);
        }

        private object OnButtonPress(PressButton aButton, BasePlayer aPlayer)
        {
            ModularCar theCar = aButton.GetComponentInParent<ModularCar>();
            if (theCar)
            {
                if (aPlayer.GetMountedVehicle() != theCar)
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
        private void AttachCarSirens(ModularCar aCar, BasePlayer aPlayer = null)
        {
            if (aPlayer != null && !permission.UserHasPermission(aPlayer.UserIDString, attachsirenspermissionName))
            {
                aPlayer.ChatMessage(Lang("No Permission AttachSirens", aPlayer.UserIDString));
                return;
            }

            if (!aCar.GetComponent<SirenController>())
            {
                aCar.gameObject.AddComponent<SirenController>();
            }
            foreach (BaseVehicleModule eachModule in aCar.GetComponentsInChildren<BaseVehicleModule>())
            {
                if (!eachModule.GetComponentInChildren<FlasherLight>()) {
                    AttachEntityToModule<FlasherLight>(eachModule, flasherLightPrefab, leftSirenPosition);
                    AttachEntityToModule<FlasherLight>(eachModule, flasherLightPrefab, rightSirenPosition);
                }
                if (!eachModule.GetComponentInChildren<PressButton>()) {
                    PressButton theButton = AttachEntityToModule<PressButton>(eachModule, buttonPrefab, buttonPosition, buttonAngles);
                    if (theButton) {
                        theButton.pressDuration = 0.2f;
                    }
                }
                if (!eachModule.GetComponentInChildren<InstrumentTool>()) {
                    AttachEntityToModule<InstrumentTool>(eachModule, trumpetPrefab, trumpetPosition, trumpetAngles);
                }
            }
            aCar.GetComponent<SirenController>().RefreshSirenState();
        }

        private void DetachCarSirens(ModularCar aCar, BasePlayer aPlayer = null)
        {
            if (aPlayer != null && !permission.UserHasPermission(aPlayer.UserIDString, attachsirenspermissionName))
            {
                aPlayer.ChatMessage(Lang("No Permission AttachSirens", aPlayer.UserIDString));
                return;
            }

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
                aPlayer.ChatMessage(Lang("Not A Car", aPlayer.UserIDString));
            }
            return theCar;
        }

        private string Lang(string aKey, string aUserID = null, params object[] someArgs) => string.Format(lang.GetMessage(aKey, this, aUserID), someArgs);
        #endregion helpers

        #region controllers
        public class SirenController : FacepunchBehaviour
        {
            private enum State {
                OFF,
                ON,
                LIGHTS_ONLY
            }

            private State state = State.OFF;
            private ModularCar car;
            private InstrumentTool trumpet;
            private bool soundActive;

            public void ChangeState()
            {
                state = state >= State.LIGHTS_ONLY ? State.OFF : state + 1;
                RefreshSirenState();
            }

            public void Off()
            {
                state = State.OFF;
                RefreshSirenState();
            }

            public void RefreshSirenState()
            {
                if (state == State.ON)
                {
                    PlayFirstTone();
                }
                bool theLightsOnFlag = state > State.OFF;
                foreach (FlasherLight eachFlasherLight in GetCar().GetComponentsInChildren<FlasherLight>())
                {
                    ToogleSirens(eachFlasherLight, theLightsOnFlag);
                }
            }

            public void PlayFirstTone()
            {
                soundActive = true;
                GetTrumpet().ClientRPC(null, "Client_PlayNote", 0, 0, 4, 1f);
                Invoke(StopFirstTone, 1f);
            }

            public void PlaySecondTone()
            {
                GetTrumpet().ClientRPC(null, "Client_PlayNote", 3, 0, 5, 1f);
                Invoke(StopSecondTone, 1f);
            }

            public void StopFirstTone()
            {
                GetTrumpet().ClientRPC(null, "Client_StopNote", 0, 0, 4, 1f);
                if (state == State.ON)
                {
                    PlaySecondTone();
                }
                else
                {
                    soundActive = false;
                }
            }

            public void StopSecondTone()
            {
                GetTrumpet().ClientRPC(null, "Client_StopNote", 3, 0, 5, 1f);
                PlayFirstTone();
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
        }
        #endregion controllers
    }
}
