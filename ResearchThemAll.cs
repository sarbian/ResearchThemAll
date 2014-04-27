/*
 * Author: Sébastien GAGGINI AKA Sarbian, France
 * License: Attribution 4.0 International (CC BY 4.0): http://creativecommons.org/licenses/by/4.0/
 *  
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ResearchThemAll
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class ResearchThemAll : MonoBehaviour
    {

        bool isRnDOpen = false;

        private void Start()
        {
            GameEvents.onGUIRnDComplexSpawn.Add(new EventVoid.OnEvent(this.OnGUIRnDComplexSpawn));
            GameEvents.onGUIRnDComplexDespawn.Add(new EventVoid.OnEvent(this.onGUIRnDComplexDespawn));
        }

        private void OnDestroy()
        {
            GameEvents.onGUIRnDComplexSpawn.Remove(new EventVoid.OnEvent(this.OnGUIRnDComplexSpawn));
            GameEvents.onGUIRnDComplexDespawn.Remove(new EventVoid.OnEvent(this.onGUIRnDComplexDespawn));
        }

        private void OnGUIRnDComplexSpawn()
        {
            isRnDOpen = (ResearchAndDevelopment.Instance != null);
        }

        private void onGUIRnDComplexDespawn()
        {
            isRnDOpen = false;
        }

        private void OnGUI()
        {
            if (!isRnDOpen)
                return;

            int unlockable = 0;

            foreach (RDNode node in UnityEngine.Object.FindObjectsOfType(typeof(RDNode)))
            {
                if (node.IsResearched)
                    unlockable += node.PartsNotUnlocked();
            }

            if (unlockable > 0)
                PopupDialog.SpawnPopupDialog(new MultiOptionDialog("There are " + unlockable + " part to research. Do you want to research them all ?", "Research Them All", HighLogic.Skin,
                    new DialogOption[] { new DialogOption("Research Them All", new Callback(this.OnResearchAllConfirm)), new DialogOption("Cancel", new Callback(this.OnResearchAllCancel)) }), false, HighLogic.Skin);
        }


        private void OnResearchAllConfirm()
        {

            foreach (RDNode node in UnityEngine.Object.FindObjectsOfType(typeof(RDNode)))
            {
                if (node.IsResearched)
                {
                    for (int i = 0; i < node.tech.partsAssigned.Count; i++)
                    {
                        AvailablePart item = node.tech.partsAssigned[i];
                        if (!node.tech.partsPurchased.Contains(item))
                        {
                            node.tech.partsPurchased.Add(item);
                        }
                    }
                    node.graphics.SetAvailablePartsCircle(node.PartsNotUnlocked());
                }
            }
        }

        private void OnResearchAllCancel()
        {
            isRnDOpen = false;
        }


        public new static void print(object message)
        {
            MonoBehaviour.print("[ResearchThemAll] " + message);
        }

    }
}
