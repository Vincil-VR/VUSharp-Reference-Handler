using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using Vincil.VUSharp.Extensions;

namespace Vincil.VUSharp.ReferenceHandler
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VUSharpReferenceHandlerImplementation : VUSharpReferenceHandler
    {
        DataDictionary singletons = new DataDictionary();
        DataDictionary playerObjectsByID = new DataDictionary();

        #region Singletons
        public override void AddSingleton(UdonSharpBehaviour singletonBehaviour)
        {
            if (singletonBehaviour == null)
            {
                Debug.LogError("[ReferenceHandler] Cannot add singleton because the provided UdonSharpBehaviour is null.");
                return;
            }
            string name = singletonBehaviour.GetUdonTypeName();
            if (!singletons.ContainsKey(name))
            {
                singletons.Add(name, singletonBehaviour);
            }
            else
            {
                Debug.LogWarning($"[ReferenceHandler] Singleton of type {name} is already registered. Ignoring duplicate registration.");
            }
        }
        public override UdonSharpBehaviour GetSingleton(string udonTypeName)
        {
            if (udonTypeName == null)
            {
                Debug.LogError("[ReferenceHandler] Cannot get singleton because the provided Udon type name is null.");
                return null;
            }
            if (singletons.TryGetValue(udonTypeName, TokenType.Reference, out DataToken token))
            {
                return (UdonSharpBehaviour)token.Reference;
            }
            else
            {
                Debug.LogError($"[ReferenceHandler] Singleton of type {udonTypeName} not found: {token.Error}");
                return null;
            }
        }
        public override UdonSharpBehaviour GetSingleton(string UdonTypeName, string backupFindTarget)
        {
            if (UdonTypeName == null)
            {
                Debug.LogError("[ReferenceHandler] Cannot get singleton because the provided Udon type name is null.");
                return null;
            }
            if (backupFindTarget == null)
            {
                Debug.LogError("[ReferenceHandler] Cannot get singleton because the provided backup find target is null.");
                return null;
            }
            if (singletons.TryGetValue(UdonTypeName, TokenType.Reference, out DataToken token))
            {
                return (UdonSharpBehaviour)token.Reference;
            }
            else
            {
                GameObject go = GameObject.Find(backupFindTarget);
                if (go != null)
                {
                    UdonSharpBehaviour[] foundBehaviours = go.GetComponents<UdonSharpBehaviour>();
                    foreach (UdonSharpBehaviour behaviour in foundBehaviours)
                    {
                        if (behaviour.GetUdonTypeName() == UdonTypeName)
                        {
                            return behaviour;
                        }
                    }
                }
                Debug.LogError($"[ReferenceHandler] Singleton of type {UdonTypeName} not found and backup failed: {token.Error}");
                return null;
            }
        }
        #endregion

        #region Player Objects
        public override void AddPlayerObject(UdonSharpBehaviour obj)
        {
            if (obj == null)
            {
                Debug.LogError("[ReferenceHandler] Cannot add PlayerObject because the provided UdonSharpBehaviour is null.");
                return;
            }
            string name = obj.GetUdonTypeName();
            VRCPlayerApi owner = Networking.GetOwner(obj.gameObject);
            if (Utilities.IsValid(owner))
            {
                int playerID = owner.playerId;
                playerObjectsByID.AddDataTokenToContainedDataDictionary(playerID, name, obj);
            }
            else
            {
                Debug.LogError($"[ReferenceHandler] Cannot add Player Object of type {name} because the owner is invalid.");
            }

        }

        public override UdonSharpBehaviour GetPlayerObject(VRCPlayerApi player, string className)
        {
            if (!Utilities.IsValid(player))
            {
                Debug.LogError($"[ReferenceHandler] Cannot get Player Object of type {className} because the player is invalid.");
                return null;
            }
            if (className == null)
            {
                Debug.LogError("[ReferenceHandler] Cannot get Player Object because the provided class name is null.");
                return null;
            }

            int playerID = player.playerId;
            if (playerObjectsByID.TryGetValue(playerID, TokenType.DataDictionary, out DataToken token))
            {
                DataDictionary playerObjects = token.DataDictionary;
                if (playerObjects.TryGetValue(className, TokenType.Reference, out token))
                {
                    return (UdonSharpBehaviour)token.Reference;
                }
                else
                {
                    Debug.LogError($"[ReferenceHandler] Player Objects of type {className} not found: {token.Error}");
                    return null;
                }
            }
            else
            {
                Debug.LogError($"[ReferenceHandler] Player Objects for player {playerID} not found: {token.Error}");
                return null;
            }
        }
        #endregion
    }
}
