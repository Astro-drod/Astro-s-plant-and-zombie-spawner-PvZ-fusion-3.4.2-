using Il2Cpp;
using MelonLoader;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: MelonInfo(typeof(Plant_and_Zombie_Spawner.Core), "Astro Spawner", "100.0.2", "Astro", null)]
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

        public static Dictionary<string, string> specialNames = new Dictionary<string, string>()
        {
            {"SunFlower", "Sunflower"}, {"CherryBomb", "Cherry Bomb"}, {"WallNut", "Wall-nut"}, 
            {"PotatoMine", "Potato Mine"}, {"SmallPuff", "Puff-shroom"}, {"FumeShroom", "Fume-shroom"},
            {"HypnoShroom", "Hypno-shroom"}, {"ScaredyShroom", "Scaredy-shroom"}, {"IceShroom", "Ice-shroom"},
            {"DoomShroom", "Doom-shroom"}, {"LilyPad", "Lily Pad"}, {"ThreePeater", "Threepeater"},
            {"Tanglekelp", "Tangle Kelp"}, {"TorchWood", "Torchwood"}, {"SeaShroom", "Sea-shroom"},
            {"StarFruit", "Starfruit"}, {"Magnetshroom", "Magnet-shroom"}, {"Cabbagepult", "Cabbage-pult"},
            {"Cornpult", "Kernel-pult"}, {"Melonpult", "Melon-pult"}, {"PeaShooterZombie", "Peashooter Zombie"}
        };

        public override void OnUpdate()
        {
            if (Board.Instance == null) return;

            if (!isInitialized)
            {
                AddIds(validPlants, 0, 39);
                AddIds(validPlants, 222, 267);
                AddIds(validPlants, 300, 305);
                AddIds(validPlants, 350, 368);
                AddIds(validPlants, 900, 979);
                AddIds(validPlants, 1000, 1378);

                AddIds(validZombies, 0, 79);
                AddIds(validZombies, 100, 129);
                AddIds(validZombies, 200, 244);
                AddIds(validZombies, 300, 335);

                isInitialized = true;
            }

            if (Input.GetKeyDown(KeyCode.Tab)) { isPlantMode = !isPlantMode; currentIndex = 0; }

            List<int> currentList = isPlantMode ? validPlants : validZombies;

            for (KeyCode key = KeyCode.A; key <= KeyCode.Z; key++)
            {
                if (Input.GetKeyDown(key))
                {
                    string target = key.ToString();
                    for (int i = 1; i <= currentList.Count; i++)
                    {
                        int idx = (currentIndex + i) % currentList.Count;
                        string raw = isPlantMode ? ((PlantType)currentList[idx]).ToString() : ((ZombieType)currentList[idx]).ToString();
                        string name = GetName(raw);
                        if (name.StartsWith(target, StringComparison.OrdinalIgnoreCase))
                        {
                            currentIndex = idx;
                            notification = ">>> " + name + " <<<";
                            notifTimer = 2f;
                            break;
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentIndex = (currentIndex + 1) % currentList.Count;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentIndex = (currentIndex - 1 + currentList.Count) % currentList.Count;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                int id = currentList[currentIndex];
                if (isPlantMode) try { CreatePlant.Instance.SetPlant(Mouse.Instance.theMouseColumn, Mouse.Instance.theMouseRow, (PlantType)id, null, default(Vector2), false, false); } catch {}
                else try { CreateZombie.Instance.SetZombie(Mouse.Instance.theMouseRow, (ZombieType)id, (float)Mouse.Instance.theMouseColumn, false); } catch {}
            }

            if (notifTimer > 0) notifTimer -= Time.deltaTime;
        }

        private void AddIds(List<int> list, int s, int e) { for (int i = s; i <= e; i++) list.Add(i); }

        public static string GetName(string raw) { if (specialNames.ContainsKey(raw)) return specialNames[raw]; return Clean(raw); }

        public static string Clean(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            s = s.Replace("_", " ");
            StringBuilder sb = new StringBuilder();
            sb.Append(s[0]);
            for (int i = 1; i < s.Length; i++) { if (char.IsUpper(s[i]) && s[i - 1] != ' ') sb.Append(' '); sb.Append(s[i]); }
            return sb.ToString();
        }

        public override void OnGUI()
        {
            if (Board.Instance == null || !isInitialized) return;
            List<int> list = isPlantMode ? validPlants : validZombies;
            string m = isPlantMode ? "PLANT" : "ZOMBIE";

            DrawTxt(20f, 20f, "[" + m + "] (TAB)", Color.yellow, 24);

            for (int i = -2; i <= 2; i++)
            {
                int idx = (currentIndex + i + list.Count) % list.Count;
                string name = GetName(isPlantMode ? ((PlantType)list[idx]).ToString() : ((ZombieType)list[idx]).ToString());
                if (i == 0) { DrawTxt(20f, 60f + (i + 2f) * 30f, "► " + name, Color.green, 30); }
                else { DrawTxt(20f, 60f + (i + 2f) * 30f, "  " + name, Color.white, 20); }
            }

            DrawTxt(20f, 220f, "A-Z: Ara | Oklar: Kaydir | SHIFT: Dik", Color.cyan, 18);
            if (notifTimer > 0) DrawTxt(20f, 250f, notification, Color.magenta, 22);
        }

        private void DrawTxt(float x, float y, string t, Color c, int s)
        {
            // Metin formatını ve kutuları tamamen ayırdık (Derleyici hatasını önlemek için)
            string f = string.Format("<size={0}><b>{1}</b></size>", s, t);
            Rect golge = new Rect(x + 1f, y + 1f, 800f, 50f);
            Rect asil = new Rect(x, y, 800f, 50f);

            GUI.color = Color.black;
            GUI.Label(golge, f);
            GUI.color = c;
            GUI.Label(asil, f);
        }
    }
}
