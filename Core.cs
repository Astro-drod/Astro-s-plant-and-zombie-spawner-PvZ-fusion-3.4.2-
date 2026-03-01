using Il2Cpp;
using MelonLoader;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;


[assembly: MelonInfo(typeof(Plant_and_Zombie_Spawner.Core), "Astro Spawner", "23.0.0", "Astro", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]

namespace Plant_and_Zombie_Spawner
{
    public class Core : MelonMod
    {
        public static List<int> validPlants = new List<int>();
        public static List<int> validZombies = new List<int>();
        public static int currentIndex = 0;
        public static bool isPlantMode = true;
        public static bool isInitialized = false;
        
        public static string notification = "";
        public static float notifTimer = 0f;

        public override void OnUpdate()
        {
            if (Board.Instance == null) return;

            if (!isInitialized)
            {
                for (int i = 0; i < 10000; i++) 
                {
                    if (((PlantType)i).ToString() != i.ToString()) validPlants.Add(i);
                    if (((ZombieType)i).ToString() != i.ToString()) validZombies.Add(i);
                }
                isInitialized = true;
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                isPlantMode = !isPlantMode;
                currentIndex = 0;
            }

            List<int> currentList = isPlantMode ? validPlants : validZombies;

            for (KeyCode key = KeyCode.A; key <= KeyCode.Z; key++)
            {
                if (Input.GetKeyDown(key))
                {
                    string targetLetter = key.ToString(); 
                    bool found = false;

                    for (int i = 1; i <= currentList.Count; i++)
                    {
                        int checkIndex = (currentIndex + i) % currentList.Count; 
                        int id = currentList[checkIndex];
                        string rawName = isPlantMode ? ((PlantType)id).ToString() : ((ZombieType)id).ToString();
                        string cleanName = CleanName(rawName);

                        if (cleanName.StartsWith(targetLetter, StringComparison.OrdinalIgnoreCase))
                        {
                            currentIndex = checkIndex;
                            found = true;
                            notification = ">>> " + cleanName + " BULUNDU! <<<";
                            notifTimer = 2f;
                            break; 
                        }
                    }

                    if (!found)
                    {
                        notification = targetLetter + " ile baslayan kayit yok!";
                        notifTimer = 2f;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentIndex++;
                if (currentIndex >= currentList.Count) currentIndex = 0; 
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentIndex--;
                if (currentIndex < 0) currentIndex = currentList.Count - 1; 
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                int currentId = currentList[currentIndex];
                if (isPlantMode)
                {
                    try { CreatePlant.Instance.SetPlant(Mouse.Instance.theMouseColumn, Mouse.Instance.theMouseRow, (PlantType)currentId, null, default(Vector2), false, false); } catch {}
                }
                else
                {
                    try { CreateZombie.Instance.SetZombie(Mouse.Instance.theMouseRow, (ZombieType)currentId, (float)Mouse.Instance.theMouseColumn, false); } catch {}
                }
            }
            
            if (notifTimer > 0) notifTimer -= Time.deltaTime;
        }

        public static string CleanName(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            text = text.Replace("_", " ");
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                {
                    newText.Append(' ');
                }
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        public override void OnGUI()
        {
            if (Board.Instance == null || !isInitialized) return;

            List<int> currentList = isPlantMode ? validPlants : validZombies;
            string modeName = isPlantMode ? "BİTKİ MODU" : "ZOMBİ MODU";

            GUI.color = Color.black;
            GUI.Label(new Rect(21, 21, 400, 30), "<size=24>[" + modeName + "] (TAB)</size>");
            GUI.color = Color.yellow;
            GUI.Label(new Rect(20, 20, 400, 30), "<size=24>[" + modeName + "] (TAB)</size>");

            int yPos = 60;
            for (int i = -2; i <= 2; i++)
            {
                int showIndex = currentIndex + i;
                if (showIndex < 0) showIndex += currentList.Count;
                if (showIndex >= currentList.Count) showIndex -= currentList.Count;

                int showId = currentList[showIndex];
                string rawName = isPlantMode ? ((PlantType)showId).ToString() : ((ZombieType)showId).ToString();
                string showName = CleanName(rawName);

                if (i == 0)
                {
                    GUI.color = Color.black;
                    GUI.Label(new Rect(21, yPos+1, 600, 40), "<size=30>► " + showName + "</size>");
                    GUI.color = Color.green;
                    GUI.Label(new Rect(20, yPos, 600, 40), "<size=30>► " + showName + "</size>");
                    yPos += 40;
                }
                else
                {
                    GUI.color = Color.black;
                    GUI.Label(new Rect(21, yPos+1, 600, 30), "<size=20>  " + showName + "</size>");
                    GUI.color = Color.white;
                    GUI.Label(new Rect(20, yPos, 600, 30), "<size=20>  " + showName + "</size>");
                    yPos += 25;
                }
            }

            GUI.color = Color.black;
            GUI.Label(new Rect(21, yPos + 11, 800, 30), "<size=18>Harf Tuslari: Ara | Oklar: Kaydir | SHIFT: Dik</size>");
            GUI.color = Color.cyan;
            GUI.Label(new Rect(20, yPos + 10, 800, 30), "<size=18>Harf Tuslari: Ara | Oklar: Kaydir | SHIFT: Dik</size>");

            if (notifTimer > 0)
            {
                GUI.color = Color.black;
                GUI.Label(new Rect(21, yPos + 41, 600, 30), "<size=22>" + notification + "</size>");
                GUI.color = Color.magenta;
                GUI.Label(new Rect(20, yPos + 40, 600, 30), "<size=22>" + notification + "</size>");
            }
        }
    }
}
