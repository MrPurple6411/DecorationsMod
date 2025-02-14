﻿using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace DecorationsMod.Controllers
{
    public class SeamothDollController : HandTarget, IHandTarget, IProtoEventListener
    {
        public void OnHandClick(GUIHand hand)
        {
            if (!enabled)
                return;

            GameObject model = this.gameObject.FindChild("Model");
            if (model == null)
                return;
            
            GameObject extras = model.FindChild("Submersible_SeaMoth_extras");
            GameObject extra = extras.FindChild("Submersible_seaMoth_geo 1").FindChild("seaMoth_storage_01_L_geo");
            Renderer rend = extra.GetComponent<Renderer>();

            Renderer[] renderers = extras.GetAllComponentsInChildren<Renderer>();

            if (rend.enabled)
            {
                foreach (Renderer renderer in renderers)
                    renderer.enabled = false;
            }
            else
            {
                foreach (Renderer renderer in renderers)
                    renderer.enabled = true;
            }
        }

        public void OnHandHover(GUIHand hand)
        {
            if (!enabled)
                return;

            var reticle = HandReticle.main;
            reticle.SetIcon(HandReticle.IconType.Hand, 1f);
            reticle.SetTextRaw(HandReticle.TextType.Hand, LanguageHelper.GetFriendlyWord("SwitchSeamothModel"));
        }
        
        // Save seamoth doll state
        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            PrefabIdentifier id = GetComponentInParent<PrefabIdentifier>();
            if (id == null)
                if ((id = GetComponent<PrefabIdentifier>()) == null)
                    return;

            string saveFolder = FilesHelper.GetSaveFolderPath();
            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            GameObject model = this.gameObject.FindChild("Model").FindChild("Submersible_SeaMoth_extras").FindChild("Submersible_seaMoth_geo 1").FindChild("seaMoth_storage_01_L_geo");
            Renderer rend = model.GetComponent<Renderer>();
            
            File.WriteAllText(FilesHelper.Combine(saveFolder, "seamothdoll_" + id.Id + ".txt"), rend.enabled ? "1" : "0", Encoding.UTF8);
        }

        // Load seamoth doll state
        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            PrefabIdentifier id = GetComponentInParent<PrefabIdentifier>();
            if (id == null)
                if ((id = GetComponent<PrefabIdentifier>()) == null)
                    return;

            string filePath = FilesHelper.Combine(FilesHelper.GetSaveFolderPath(), "seamothdoll_" + id.Id + ".txt");
            if (File.Exists(filePath))
            {
                string state = File.ReadAllText(filePath, Encoding.UTF8).Trim();

                GameObject extras = this.gameObject.FindChild("Model").FindChild("Submersible_SeaMoth_extras");
                Renderer[] renderers = extras.GetAllComponentsInChildren<Renderer>();

                foreach (Renderer renderer in renderers)
                {
                    renderer.enabled = (string.Compare(state, "1", false, CultureInfo.InvariantCulture) == 0);
                }
            }
        }
    }
}
