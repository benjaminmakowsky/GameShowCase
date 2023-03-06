using System;
using Data_Classes;
using Farming;
using Items;
using UnityEngine;
using Random = UnityEngine.Random;

namespace General
{
    public class WorldGeneration : MonoBehaviour
    {
        private FarmingIcon iconPool;
        
        /// <summary>
        /// /Grid/FarmingSprites/Soil/
        /// </summary>
        private Transform soilContainer;
        
        /// <summary>
        /// /Grid/FarmingSprites/Crops/
        /// </summary>
        private Transform cropContainer;
        
        /// <summary>
        /// /Grid/FarmingSprites/Fencing/
        /// </summary>
        private Transform wallContainer;
        
        /// <summary>
        /// /Grid/FarmingSprites/Decorations/
        /// </summary>
        private Transform decorContainer;
        
        
        // Start is called before the first frame update
        void Start(){
            
            iconPool = GameObject.Find("/Grid/FarmingSprites/").GetComponent<FarmingIcon>();
            soilContainer = GameObject.Find("/Grid/FarmingSprites/Soil/").transform;
            cropContainer = GameObject.Find("/Grid/FarmingSprites/Crops/").transform;
            wallContainer = GameObject.Find("/Grid/FarmingSprites/Fencing/").transform;
            decorContainer = GameObject.Find("/Grid/FarmingSprites/Decorations/").transform;

            Debug.Log("Attempting to load map");
            MapData mapData = SaveSystem.loadMap();
            if (mapData != null) {
                Debug.Log("Loading map from save: " + Application.persistentDataPath);
                createMapDataFromSave(mapData);
            } else {
                Debug.Log("Map save data not found, creating new map");
                for (int x = -53; x < 0; x++) {
                    for (int y = 28; y > -25; y--) {

                        Vector2 plantPos = new Vector2(x, y);

                        //Add .5 to check center of square
                        plantPos = plantPos + 0.5f * Vector2.up + 0.5f * Vector2.right;

                        //Check all layers for a hit except the camera confiner
                        LayerMask confiner = LayerMask.GetMask("Confiner", "Ignore Raycast");
                        confiner = ~confiner;


                        //Check if there is anything is already down
                        RaycastHit2D[] obstructionCheck = Physics2D.RaycastAll(
                            plantPos,
                            Vector2.up,
                            0f,
                            confiner);
                        // Debug.Log(obstructionCheck.collider.name);

                        bool canCreate = true;

                        //If you hit anything begin a check to see if you hit anything besides a PolygonCollider
                        if (obstructionCheck.Length > 0) {
                            foreach (RaycastHit2D hit2D in obstructionCheck) {
                                if (hit2D.collider is not PolygonCollider2D) {
                                    canCreate = false;
                                }
                            }
                        }

                        if (canCreate){

                            Transform spriteContainerRoot = GameObject.Find("/Prefabs").transform;
                            GameObject cropPrefab = null;
                            int random = Random.Range(-2, 12);
                            switch (random) {
                                case -2:
                                case -1:
                                case 0:
                                case 1:
                                    break;

                                case 2:
                                case 3:
                                case 11:
                                    cropPrefab = spriteContainerRoot.gameObject.GetComponent<Prefabs>().ROCKS;
                                    break;

                                case 4:
                                case 5:
                                case 10:
                                    cropPrefab = spriteContainerRoot.gameObject.GetComponent<Prefabs>().THORNS;
                                    break;

                                case 6:
                                case 7:
                                    cropPrefab = spriteContainerRoot.gameObject.GetComponent<Prefabs>().LEAVES;
                                    break;

                                case 8:
                                    cropPrefab = spriteContainerRoot.gameObject.GetComponent<Prefabs>().PINE_TREE;
                                    cropPrefab.GetComponent<GameTree>().spawnAtRandomStage();
                                    break;

                                case 9:
                                    cropPrefab = spriteContainerRoot.gameObject.GetComponent<Prefabs>().BUSHY_TREE;
                                    cropPrefab.GetComponent<GameTree>().spawnAtRandomStage();
                                    break;
                            }

                            if (cropPrefab != null) {
                                Transform spriteContainer = GameObject.Find("/Grid/Forage/").transform;
                                Instantiate(cropPrefab, new Vector3(x, y), Quaternion.identity, spriteContainer);
                            }
                        } 
                    }
                }
                SaveSystem.SaveMap();
            }
        }


        private void createMapDataFromSave(MapData mapData){
            String[] names = mapData.names;
            float[] xCoords = mapData.xCoords;
            float[] yCoords = mapData.yCoords;
            int[] currentStage = mapData.currentStage;
            int[] growDay = mapData.growthDay;
            float[] growingTime = mapData.timeGrowing;
            
            foreach (Transform child in cropContainer) {
                Destroy(child.gameObject);
            }
            foreach (Transform child in soilContainer) {
                Destroy(child.gameObject);
            }
            foreach (Transform child in wallContainer) {
                Destroy(child.gameObject);
            }
            foreach (Transform child in decorContainer) {
                Destroy(child.gameObject);
            }
            
            //Clear all the objects from the forage set-up container
            Transform forageContainer = GameObject.Find("/Grid/Forage").transform;
            foreach (Transform child in forageContainer) {
                Destroy(child.gameObject);
            }
            
            // Transform roadContainer = GameObject.Find("/Grid/Roads").transform;
            // int iy = 0;
            // foreach (Transform child in roadContainer) {
            //     Destroy(child.gameObject);
            //     iy++;
            // }
            // Debug.Log($"Destroyed {iy}");
            
            GameObject cropPrefab = null;
            for (int i = 0; i < mapData.firstIndex; i++) {
               // Debug.Log("Creating: " + names[i] +" at (" + xCoords[i] +"," + yCoords[i] + ")" );
                Transform spriteContainerRoot = GameObject.Find("/Prefabs").transform;
                cropPrefab = null;
                switch (names[i]) {
                    case "Stones(Clone)":
                        cropPrefab = spriteContainerRoot.gameObject.GetComponent<Prefabs>().ROCKS;
                        break;

                    
                    case "Thorns(Clone)":
                        cropPrefab = spriteContainerRoot.gameObject.GetComponent<Prefabs>().THORNS;
                        break;

                    case "Leaves(Clone)":
                        cropPrefab = spriteContainerRoot.gameObject.GetComponent<Prefabs>().LEAVES;
                        break;

                    case "Pine(Clone)":
                        cropPrefab = spriteContainerRoot.gameObject.GetComponent<Prefabs>().PINE_TREE;
                        cropPrefab.GetComponent<GameTree>().setGrowth(growDay[i], currentStage[i]);
                        break;

                    case "BushyTree(Clone)":
                        cropPrefab = spriteContainerRoot.gameObject.GetComponent<Prefabs>().BUSHY_TREE;
                        cropPrefab.GetComponent<GameTree>().setGrowth(growDay[i], currentStage[i]);
                        break;
                    
                    default:
                        // Debug.Log(names[i]);
                        break;
                }

                if (cropPrefab != null) {
                    Transform spriteContainer = GameObject.Find("/Grid/Forage/").transform;
                    Instantiate(cropPrefab, new Vector3(xCoords[i], yCoords[i]), Quaternion.identity, spriteContainer);
                }
                
            }
            
            for (int i = mapData.firstIndex; i < mapData.secondIndex; i++) {
                dig(xCoords[i], yCoords[i]);
            }

            for (int i = mapData.secondIndex; i < mapData.thirdIndex; i++) {
                // Debug.Log("Making Crop" + names[i] +" at: (" + xCoords[i] + ", "+ yCoords[i] + ") -> " + growingTime[i]);
                plant(names[i], xCoords[i], yCoords[i], growingTime[i]);
            }
            
            for (int i = mapData.thirdIndex; i < mapData.fourthIndex; i++) {
                placeFencing(names[i], xCoords[i], yCoords[i]);
            }

            for (int i = mapData.fourthIndex; i < names.Length; i++) {
                placeDecor(names[i], xCoords[i], yCoords[i]);
            }
        }
        
        private void plant(string cropName, float xCoord, float yCoord, float timeAlive){
            GameObject prefab = null;
            //Create a farming sprite fab to instantiate to make life easier
            switch (cropName) {
                case "Witch Hazel":
                    prefab =  iconPool.witchHazelSeeds;
                    break;
                
                case "Monkshood":
                    prefab =  iconPool.monkshoodSeeds;
                    break;
            }
            GameObject crop = Instantiate(prefab, new Vector3(xCoord, yCoord), Quaternion.identity, cropContainer);
            crop.GetComponent<Crop>().timeAlive = timeAlive;

        }

        private void dig(float xCoord, float yCoord){
            //Create a farming sprite fab to instantiate to make life easier
            GameObject prefab = iconPool.farmingSprite;
            Instantiate(prefab, new Vector3(xCoord, yCoord), Quaternion.identity, soilContainer);
        }
        
        private void placeFencing(string fenceType, float xCoord, float yCoord){
            GameObject prefab = null;
            //Create a farming sprite fab to instantiate to make life easier
            switch (fenceType) {
                case "Fencing":
                    prefab =  iconPool.fencingSprite;
                    break;
                
                case "Camp Fire":
                    // prefab = iconPool.campfire;
                    // Instantiate(prefab, new Vector3(xCoord, yCoord), Quaternion.identity, decorContainer);
                    Debug.LogError("Should not be camp fire");
                    return;
                    
                default:
                    Debug.Log(fenceType);
                    prefab = null;
                    return;
            }
            Instantiate(prefab, new Vector3(xCoord, yCoord), Quaternion.identity, wallContainer);
        }
        
        private void placeDecor(string decor, float xCoord, float yCoord){
            GameObject prefab = null;
            // Debug.Log(decor);
            //Create a farming sprite fab to instantiate to make life easier
            switch (decor) {
                case "Camp Fire":
                    prefab =  iconPool.campfire;
                    break;
                
                case "Fencing":
                    placeFencing(decor, xCoord, yCoord);
                    break;
                
                case "Pebbles":
                    prefab = iconPool.pebbles;
                    break;
                
                case "Hole":
                    prefab = iconPool.hole;
                    break;
                
                case "Water Hole":
                    prefab = iconPool.waterHole;
                    break;
                
                default:
                    prefab = InventoryLoader.loadItemGameObject(decor, 1);
                    break;
            }

            if (prefab) {
                Instantiate(prefab, new Vector3(xCoord, yCoord), Quaternion.identity, decorContainer);
            }
        }
    }
}
