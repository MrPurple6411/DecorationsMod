﻿using DecorationsMod.Controllers;
using System.Collections.Generic;
using UnityEngine;

namespace DecorationsMod.NewItems
{
    public class BarFood1 : DecorationItem
    {
        public BarFood1() // Feeds abstract class
        {
            this.ClassID = "BarFood1";
            this.PrefabFileName = DecorationItem.DefaultResourcePath + this.ClassID;

            //this.GameObject = AssetsHelper.Assets.LoadAsset<GameObject>("barfood01");
            this.GameObject = new GameObject(this.ClassID);

            this.TechType = SMLHelper.V2.Handlers.TechTypeHandler.AddTechType(this.ClassID,
                                                        LanguageHelper.GetFriendlyWord("BarFood1Name"),
                                                        LanguageHelper.GetFriendlyWord("BarFood1Description"),
                                                        true);

#if SUBNAUTICA
            this.Recipe = new SMLHelper.V2.Crafting.TechData()
            {
                craftAmount = 1,
                Ingredients = new List<SMLHelper.V2.Crafting.Ingredient>(new SMLHelper.V2.Crafting.Ingredient[3]
                    {
                        new SMLHelper.V2.Crafting.Ingredient(TechType.Titanium, 1),
                        new SMLHelper.V2.Crafting.Ingredient(TechType.CuredPeeper, 1),
                        new SMLHelper.V2.Crafting.Ingredient(TechType.HangingFruit, 1)
                    }),
            };
#else
            this.Recipe = new SMLHelper.V2.Crafting.RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[3]
                    {
                        new Ingredient(TechType.Titanium, 1),
                        new Ingredient(TechType.CuredPeeper, 1),
                        new Ingredient(TechType.HangingFruit, 1)
                    }),
            };
#endif
        }

        private static GameObject _barFood1 = null;

        public override void RegisterItem()
        {
            if (this.IsRegistered == false)
            {
                if (_barFood1 == null)
                    _barFood1 = AssetsHelper.Assets.LoadAsset<GameObject>("barfood01");

                GameObject model = _barFood1.FindChild("docking_food_01_food2").FindChild("docking_food_01_food2");

                // Scale model
                model.transform.localScale *= 22f;

                // Set tech tag
                var techTag = _barFood1.AddComponent<TechTag>();
                techTag.type = this.TechType;

                // Set prefab identifier
                var prefabId = _barFood1.AddComponent<PrefabIdentifier>();
                prefabId.ClassId = this.ClassID;

                // Set collider
                var collider = _barFood1.AddComponent<BoxCollider>();
                collider.size = new Vector3(0.1f, 0.05f, 0.07f);

                // Set large world entity
                var lwe = _barFood1.AddComponent<LargeWorldEntity>();
                lwe.cellLevel = LargeWorldEntity.CellLevel.Near;

                // Set proper shaders (for crafting animation)
                Shader marmosetUber = Shader.Find("MarmosetUBER");
                Texture normal = AssetsHelper.Assets.LoadAsset<Texture>("docking_food_01_normal");
                var renderer = _barFood1.GetComponentInChildren<Renderer>();
                renderer.material.shader = marmosetUber;
                renderer.material.SetTexture("_BumpMap", normal);
                renderer.material.EnableKeyword("MARMO_NORMALMAP");
                renderer.material.EnableKeyword("_ZWRITE_ON"); // Enable Z write

                // Update sky applier
                var applier = _barFood1.GetComponent<SkyApplier>();
                if (applier == null)
                    applier = _barFood1.AddComponent<SkyApplier>();
                applier.renderers = new Renderer[] { renderer };
                applier.anchorSky = Skies.Auto;

                // We can pick this item
                var pickupable = _barFood1.AddComponent<Pickupable>();
                pickupable.isPickupable = true;
                pickupable.randomizeRotationWhenDropped = true;

                // We can place this item
                _barFood1.AddComponent<CustomPlaceToolController>();
                var placeTool = _barFood1.AddComponent<GenericPlaceTool>();
                placeTool.allowedInBase = true;
                placeTool.allowedOnBase = false;
                placeTool.allowedOnCeiling = false;
                placeTool.allowedOnConstructable = true;
                placeTool.allowedOnGround = true;
                placeTool.allowedOnRigidBody = true;
                placeTool.allowedOnWalls = false;
                placeTool.allowedOutside = ConfigSwitcher.AllowPlaceOutside;
                placeTool.rotationEnabled = true;
                placeTool.enabled = true;
                placeTool.hasAnimations = false;
                placeTool.hasBashAnimation = false;
                placeTool.hasFirstUseAnimation = false;
                placeTool.mainCollider = collider;
                placeTool.pickupable = pickupable;
                placeTool.drawTime = 0.5f;
                placeTool.dropTime = 1;
                placeTool.holsterTime = 0.35f;

                // We can eat this item
                var eatable = _barFood1.AddComponent<Eatable>();
                eatable.foodValue = ConfigSwitcher.BarFood1FoodValue;
                eatable.waterValue = ConfigSwitcher.BarFood1WaterValue;
#if SUBNAUTICA
                eatable.stomachVolume = 10.0f;
                eatable.allowOverfill = false;
#endif
                eatable.decomposes = false;
                eatable.despawns = false;
                eatable.kDecayRate = 0.0f;
                eatable.despawnDelay = 0.0f;

                // Associate recipe to the new TechType
                SMLHelper.V2.Handlers.CraftDataHandler.SetTechData(this.TechType, this.Recipe);

                // Add the new TechType to Hand Equipment type.
                SMLHelper.V2.Handlers.CraftDataHandler.SetEquipmentType(this.TechType, EquipmentType.Hand);

                // Set quick slot type.
                SMLHelper.V2.Handlers.CraftDataHandler.SetQuickSlotType(this.TechType, QuickSlotType.Selectable);

                // Set the buildable prefab
                SMLHelper.V2.Handlers.PrefabHandler.RegisterPrefab(this);

                // Set the custom sprite
                SMLHelper.V2.Handlers.SpriteHandler.RegisterSprite(this.TechType, AssetsHelper.Assets.LoadAsset<Sprite>("barfood01icon"));

                this.IsRegistered = true;
            }
        }

        public override GameObject GetGameObject()
        {
            GameObject prefab = GameObject.Instantiate(_barFood1);

            prefab.name = this.ClassID;

            // Add fabricating animation
            var fabricatingA = prefab.AddComponent<VFXFabricating>();
            fabricatingA.localMinY = -0.1f;
            fabricatingA.localMaxY = 0.6f;
            fabricatingA.posOffset = new Vector3(0f, 0f, 0f);
            fabricatingA.eulerOffset = new Vector3(0f, 0f, 0f);
            fabricatingA.scaleFactor = 1.0f;

            return prefab;
        }
    }
}
