﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace TextAdventureGame
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Game game = new Game();
            game.Start();
        }
    }

    public class Game
    {
        private Player player;
        private Map map;
        private bool isRunning;
        private const int PAUSE_DURATION = 1500;

        public void Start()
        {
            Console.Title = "The Forest Mystery";

            while (true)
            {
                DrawTitle();
                string choice = GetMenuChoice();

                switch (choice)
                {
                    case "1":
                        StartNewGame();
                        break;
                    case "2":
                        ShowCredits();
                        break;
                    case "3":
                        Console.Clear();
                        Console.WriteLine("\nThanks for playing! Goodbye!");
                        Thread.Sleep(PAUSE_DURATION);
                        return;
                    default:
                        Console.WriteLine("\nInvalid choice. Please try again.");
                        Thread.Sleep(PAUSE_DURATION);
                        break;
                }
            }
        }

        private void DrawTitle()
        {
            Console.Clear();
            Console.WriteLine(@"
╔══════════════════════════════════════╗
║          The Forest Mystery          ║
╚══════════════════════════════════════╝");
            Console.WriteLine("\n1. New Game");
            Console.WriteLine("2. Credits");
            Console.WriteLine("3. Exit");
        }

        private string GetMenuChoice()
        {
            Console.Write("\nEnter your choice (1-3): ");
            return Console.ReadLine().Trim();
        }

        private void StartNewGame()
        {
            player = new Player();
            map = new Map();
            isRunning = true;

            ShowIntroduction();
            RunGame();
        }

        private void ShowIntroduction()
        {
            Console.Clear();
            Console.WriteLine("═══ Welcome to The Forest Mystery ═══\n");
            Console.WriteLine("You find yourself at the entrance of an ancient forest,");
            Console.WriteLine("where legends speak of a mysterious door guarding untold secrets...\n");

            Console.WriteLine("Available Commands:");
            Console.WriteLine("━━━━━━━━━━━━━━━━━");
            Console.WriteLine("• Movement: n (North), s (South), e (East), w (West)");
            Console.WriteLine("• look - Examine your surroundings");
            Console.WriteLine("• inventory - Check your items");
            Console.WriteLine("• talk - Speak with characters");
            Console.WriteLine("• take - Pick up items");
            Console.WriteLine("• use [item] - Use an item");
            Console.WriteLine("• help - Show commands");
            Console.WriteLine("• quit - Exit to main menu\n");

            Console.WriteLine("Press any key to begin your adventure...");
            Console.ReadKey(true);
        }

        private void RunGame()
        {
            while (isRunning)
            {
                Console.Clear();
                ShowGameStatus();
                ProcessCommand(GetCommand());
            }
        }

        private void ShowGameStatus()
        {
            map.ShowCurrentLocation(player.X, player.Y);
            Console.WriteLine("\nExits: " + map.GetAvailableExits(player.X, player.Y));
            if (player.Inventory.Any())
            {
                Console.WriteLine($"Inventory: {string.Join(", ", player.Inventory)}");
            }
        }

        private string GetCommand()
        {
            Console.Write("\nWhat would you like to do? > ");
            return Console.ReadLine().ToLower().Trim();
        }

        private void ProcessCommand(string command)
        {
            switch (command)
            {
                case "n" or "s" or "e" or "w":
                    player.Move(command, map);
                    break;

                case "look":
                    map.ExamineSurroundings(player.X, player.Y);
                    WaitForKey();
                    break;

                case "inventory":
                    ShowInventory();
                    break;

                case "talk":
                    map.TalkAtLocation(player.X, player.Y);
                    WaitForKey();
                    break;

                case "take":
                    map.TakeItemAtLocation(player.X, player.Y, player);
                    WaitForKey();
                    break;

                case "use key":
                    if (map.UseKeyAtLocation(player.X, player.Y, player))
                    {
                        ShowWinScreen();
                        isRunning = false;
                    }
                    else
                    {
                        WaitForKey();
                    }
                    break;

                case "help":
                    ShowHelp();
                    break;

                case "quit":
                    if (ConfirmQuit())
                    {
                        isRunning = false;
                    }
                    break;

                default:
                    Console.WriteLine("\nI don't understand that command. Type 'help' for available commands.");
                    WaitForKey();
                    break;
            }
        }

        private void ShowInventory()
        {
            Console.WriteLine("\n═══ Inventory ═══");
            if (player.Inventory.Any())
            {
                foreach (string item in player.Inventory)
                {
                    Console.WriteLine($"• {item}");
                }
            }
            else
            {
                Console.WriteLine("Your inventory is empty.");
            }
            WaitForKey();
        }

        private void ShowHelp()
        {
            Console.Clear();
            Console.WriteLine("═══ Available Commands ═══\n");
            Console.WriteLine("Movement: n (North), s (South), e (East), w (West)");
            Console.WriteLine("look - Examine your surroundings");
            Console.WriteLine("inventory - Check your items");
            Console.WriteLine("talk - Speak with characters");
            Console.WriteLine("take - Pick up items");
            Console.WriteLine("use [item] - Use an item");
            Console.WriteLine("help - Show this help message");
            Console.WriteLine("quit - Exit to main menu");
            WaitForKey();
        }

        private bool ConfirmQuit()
        {
            Console.Write("\nAre you sure you want to quit? (y/n): ");
            return Console.ReadLine().Trim().ToLower() == "y";
        }

        private void ShowWinScreen()
        {
            Console.Clear();
            Console.WriteLine(@"
╔══════════════════════════════════════╗
║         Congratulations!!!           ║
║                                      ║
║    You've solved the forest mystery  ║
║        and opened the ancient door!  ║
╚══════════════════════════════════════╝");
            WaitForKey();
        }

        private void ShowCredits()
        {
            Console.Clear();
            Console.WriteLine(@"
╔══════════════════════════════════════╗
║             Credits                  ║
╚══════════════════════════════════════╝

Game Design & Development:
------------------------
[SaeedAbuSaleh]

Special Thanks:
-------------
To all adventure game enthusiasts!");
            WaitForKey();
        }

        private void WaitForKey()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }
    }

    public class Player
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public List<string> Inventory { get; } = new List<string>();

        public void Move(string direction, Map map)
        {
            int newX = X;
            int newY = Y;

            switch (direction)
            {
                case "n": newY--; break;
                case "s": newY++; break;
                case "e": newX++; break;
                case "w": newX--; break;
            }

            if (map.IsValidLocation(newX, newY))
            {
                X = newX;
                Y = newY;
                Console.WriteLine($"\nYou move {GetDirectionName(direction)}.");
                Thread.Sleep(1000);
            }
            else
            {
                Console.WriteLine("\nYou can't go that way! A dense thicket blocks your path.");
                Thread.Sleep(1500);
            }
        }

        private string GetDirectionName(string dir) => dir switch
        {
            "n" => "North",
            "s" => "South",
            "e" => "East",
            "w" => "West",
            _ => string.Empty
        };
    }

    public class Map
    {
        private readonly Location[,] locations;

        public Map()
        {
            locations = new Location[2, 2];

            // Starting location
            locations[0, 0] = new Location(
                "Forest Entrance",
                "You stand at the entrance to an ancient forest. Tall trees loom overhead, their branches swaying gently in the breeze. A narrow path leads north and east.",
                "The ground is covered in leaves and twigs, but nothing of interest catches your eye.",
                "The rustling leaves seem to whisper ancient secrets, but there's no one here to talk to.");

            // Location with NPC
            locations[0, 1] = new Location(
                "Mystic Clearing",
                "Sunlight filters through the canopy into a peaceful clearing. An elderly sage in flowing robes stands here, deep in meditation.",
                "The clearing is well-kept, with nothing to take.",
                "The sage opens his eyes and speaks: 'Seeker, the ancient door to the east guards great mysteries. But only those who possess the sacred key may pass. I sense such a key lies within an abandoned dwelling to the south...'");

            // Location with key
            locations[1, 0] = new Location(
                "Abandoned Hut",
                "A weathered wooden hut stands in a shadowy grove. Its door hangs slightly ajar, and something catches the light within.",
                "You discover an ornate key with strange markings! Its craftsmanship suggests great importance.",
                "The hut is long abandoned, with only echoes of its former inhabitants.");

            // Location with door (end game)
            locations[1, 1] = new Location(
                "Ancient Door",
                "An imposing stone door dominates this area. Strange symbols are carved into its surface, and a peculiar keyhole gleams at its center.",
                "The door is firmly sealed - there's nothing to take.",
                "The ancient door stands silent, waiting for its key.");
        }

        public bool IsValidLocation(int x, int y) => x >= 0 && x < 2 && y >= 0 && y < 2;

        public void ShowCurrentLocation(int x, int y)
        {
            if (IsValidLocation(x, y))
            {
                Console.WriteLine($"\n═══ {locations[x, y].Name} ═══");
                Console.WriteLine(locations[x, y].Description);
            }
        }

        public string GetAvailableExits(int x, int y)
        {
            List<string> exits = new List<string>();
            if (IsValidLocation(x, y - 1)) exits.Add("North");
            if (IsValidLocation(x, y + 1)) exits.Add("South");
            if (IsValidLocation(x + 1, y)) exits.Add("East");
            if (IsValidLocation(x - 1, y)) exits.Add("West");

            return string.Join(", ", exits);
        }

        public void ExamineSurroundings(int x, int y)
        {
            if (IsValidLocation(x, y))
            {
                Console.WriteLine($"\n{locations[x, y].Description}");
                Console.WriteLine($"\nAs you look around more carefully: {locations[x, y].ExamineMessage}");
            }
        }

        public void TalkAtLocation(int x, int y)
        {
            if (IsValidLocation(x, y))
            {
                Console.WriteLine($"\n{locations[x, y].TalkMessage}");
            }
        }

        public void TakeItemAtLocation(int x, int y, Player player)
        {
            if (IsValidLocation(x, y))
            {
                if (x == 1 && y == 0 && !player.Inventory.Contains("Ornate Key"))
                {
                    player.Inventory.Add("Ornate Key");
                    Console.WriteLine("\nYou carefully pick up the ornate key.");
                }
                Console.WriteLine($"\n{locations[x, y].TakeMessage}");
            }
        }

        public bool UseKeyAtLocation(int x, int y, Player player)
        {
            if (IsValidLocation(x, y) && x == 1 && y == 1)
            {
                if (player.Inventory.Contains("Ornate Key"))
                {
                    Console.WriteLine("\nThe key fits perfectly! With a deep rumbling sound, the ancient door begins to open...");
                    return true;
                }
                else
                {
                    Console.WriteLine("\nThe door remains firmly shut. You'll need to find the right key to open it.");
                }
            }
            else
            {
                Console.WriteLine("\nThere's nothing here that requires a key.");
            }
            return false;
        }
    }

    public class Location
    {
        public string Name { get; }
        public string Description { get; }
        public string ExamineMessage { get; }
        public string TakeMessage { get; }
        public string TalkMessage { get; }

        public Location(string name, string description, string takeMessage, string talkMessage)
        {
            Name = name;
            Description = description;
            ExamineMessage = takeMessage;
            TakeMessage = takeMessage;
            TalkMessage = talkMessage;
        }
    }
}