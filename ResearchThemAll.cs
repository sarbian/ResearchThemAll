/*
 * Copyright (c) <2014>, Sébastien GAGGINI AKA Sarbian, France
 * License: Attribution 4.0 International (CC BY 4.0): http://creativecommons.org/licenses/by/4.0/
 *
 */

using UnityEngine;

namespace ResearchThemAll
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class ResearchThemAll : MonoBehaviour
    {
        private bool isRnDOpen = false;
        private int unlockable = 0;
        private int cost = 0;

        private void Start()
        {
            GameEvents.onGUIRnDComplexSpawn.Add(OnGUIRnDComplexSpawn);
            GameEvents.onGUIRnDComplexDespawn.Add(OnGUIRnDComplexDespawn);
            GameEvents.OnTechnologyResearched.Add(OnTechnologyResearched);
        }

        private void OnDestroy()
        {
            GameEvents.onGUIRnDComplexSpawn.Remove(OnGUIRnDComplexSpawn);
            GameEvents.onGUIRnDComplexDespawn.Remove(OnGUIRnDComplexDespawn);
            GameEvents.OnTechnologyResearched.Remove(OnTechnologyResearched);
        }

        private void OnTechnologyResearched(GameEvents.HostTargetAction<RDTech, RDTech.OperationResult> data)
        {
            OnGUIRnDComplexSpawn();
        }

        private void OnGUIRnDComplexSpawn()
        {
            isRnDOpen = (ResearchAndDevelopment.Instance != null);

            if (!isRnDOpen)
            {
                return;
            }

            foreach (RDNode node in FindObjectsOfType(typeof(RDNode)))
            {
                if (node.tech != null && node.IsResearched)
                {
                    for (int i = 0; i < node.tech.partsAssigned.Count; i++)
                    {
                        AvailablePart item = node.tech.partsAssigned[i];
                        if (item != null && !node.tech.partsPurchased.Contains(item))
                        {
                            unlockable++;
                            cost += item.entryCost;
                        }
                    }
                }
            }
        }

        private void OnGUIRnDComplexDespawn()
        {
            isRnDOpen = false;
        }

        private void OnGUI()
        {
            if (!isRnDOpen)
            {
                return;
            }

            bool enoughFunds = Funding.Instance == null || Funding.Instance.Funds >= cost;

            if (unlockable > 0)
            {
                if (enoughFunds)
                {
                    string question1 = "There are " + unlockable +
                                       " part to research. Do you want to research them all";
                    string question2 = Funding.Instance != null
                        ? " for " + cost.ToString("N0") + " Funds ?"
                        : " ?";

                    OnGUIRnDComplexDespawn();
                    PopupDialog.SpawnPopupDialog(
                        new MultiOptionDialog(question1 + question2
                            , "Research Them All", HighLogic.Skin,
                            new[]
                            {
                                new DialogOption("Research Them All", OnResearchAllConfirm),
                                new DialogOption("Cancel", OnGUIRnDComplexDespawn)
                            }),
                        false,
                        HighLogic.Skin);
                }
                else
                {
                    OnGUIRnDComplexDespawn();
                    PopupDialog.SpawnPopupDialog("Research Them All",
                        "Not enough Funds to unlock the " + unlockable + " part to research.", "Cancel", false,
                        HighLogic.Skin);
                }
            }
        }

        private void OnResearchAllConfirm()
        {
            foreach (RDNode node in FindObjectsOfType(typeof(RDNode)))
            {
                if (node.tech != null && node.IsResearched)
                {
                    node.tech.AutoPurchaseAllParts();
                    node.graphics.SetAvailablePartsCircle(node.PartsNotUnlocked());
                }
            }
            OnGUIRnDComplexDespawn();
        }

        public new static void print(object message)
        {
            MonoBehaviour.print("[ResearchThemAll] " + message);
        }
    }
}