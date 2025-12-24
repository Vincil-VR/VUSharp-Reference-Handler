using System.Collections;
using System.Collections.Generic;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Vincil.VUSharp.ReferenceHandler
{
    public abstract class VUSharpReferenceHandler : UdonSharpBehaviour
    {
        public static VUSharpReferenceHandler Instance()
        {
            VUSharpReferenceHandler referenceHandler = GameObject.Find("/ReferenceHandler").GetComponent<VUSharpReferenceHandler>();
            if (referenceHandler == null)
            {
                Debug.LogError("Failed to find VUSharpReferenceHandler!");
            }
            return referenceHandler;
        }

        #region Singletons
        /// <summary>
        /// Registers the specified <see cref="UdonSharpBehaviour"/> as a singleton.
        /// </summary>
        /// <remarks>If a <see cref="UdonSharpBehaviour"/> of the same type name is already registered, the method logs a
        /// warning  and ignores the duplicate registration. The method does not replace the existing singleton.</remarks>
        /// <param name="singletonBehaviour">The singleton instance to register. The instance must have a unique Udon type name.</param>
        public abstract void AddSingleton(UdonSharpBehaviour singletonBehaviour);
        /// <summary>
        /// Retrieves the singleton instance of the specified class.
        /// </summary>
        /// <remarks>This method attempts to locate the singleton instance of the specified class by its Udon type
        /// name.  If the singleton is not found, an error message is logged, and <see langword="null"/> is
        /// returned.</remarks>
        /// <param name="udonTypeName">The Udon type name of the <see cref="UdonSharpBehaviour"/> whose singleton instance is to be retrieved.</param>
        /// <returns>The singleton instance of the specified class as an <see cref="UdonSharpBehaviour"/>,  or <see
        /// langword="null"/> if the singleton cannot be found.</returns>
        public abstract UdonSharpBehaviour GetSingleton(string udonTypeName);
        /// <summary>
        /// Retrieves the singleton instance of the specified class, using a backup method of finding the singleton if it has not been registered yet.
        /// </summary>
        /// <remarks>Use this method in cases were the singleton might be retrieved before it has had time to register itself, for example during world start.</remarks>
        /// <param name="udonTypeName">The Udon type name of the <see cref="UdonSharpBehaviour"/> whose singleton instance is to be retrieved.</param>
        /// <param name="backupFindTarget">A backup Unity Find target in case the desired singleton hasn't been registered yet.</param>
        /// <returns>The singleton instance of the specified class as an <see cref="UdonSharpBehaviour"/>,  or <see
        /// langword="null"/> if the singleton cannot be found.</returns>
        public abstract UdonSharpBehaviour GetSingleton(string UdonTypeName, string backupLocation);
        #endregion

        #region Player Objects
        /// <summary>
        /// Adds a PlayerObject to the system.
        /// </summary>
        /// <remarks>This method automatically associates the provided object with its owner.  If a PlayerObject of the same Udon type name has already been added, it is overridden.</remarks>
        /// <param name="obj">The <see cref="UdonSharpBehaviour"/> instance representing the object to add. Cannot be <see
        /// langword="null"/>.</param>
        public abstract void AddPlayerObject(UdonSharpBehaviour obj);
        /// <summary>
        /// Retrieves the UdonSharpBehaviour instance associated with the specified player and Udon type name.
        /// </summary>
        /// <param name="player">The <see cref="VRCPlayerApi"/> of the player whose object is being retrieved.</param>
        /// <param name="udonTypeName">The Udon type name of the desired UdonSharpBehaviour.</param>
        /// <returns>The <see cref="UdonSharpBehaviour"/> instance corresponding to the specified player and Udon type name, or <see
        /// langword="null"/> if no matching object is found.</returns>
        public abstract UdonSharpBehaviour GetPlayerObject(VRCPlayerApi player, string udonTypeName);
        #endregion
    }
}
