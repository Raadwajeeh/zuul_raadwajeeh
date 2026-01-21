using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
class Game
{
	// Private fields
	private Parser parser;
	private Player player;
	private Room startRoom;

	private Room WinRoom;

	//private Room HealRoom;

	private bool finished;

	// Constructor
	public Game()
{
    parser = new Parser();
    CreateRooms();
    player = new Player(startRoom);
}


	// Initialise the Rooms (and the Items)
	private void CreateRooms()
	{
		// Create the rooms
		Room outside = new Room("outside the main entrance of the university");
		Room theatre = new Room("in a lecture theatre");
		Room pub = new Room("in the campus pub");
		Room lab = new Room("in a computing lab");
		Room office = new Room("in the computing admin office");

		// Initialise room exits
		outside.AddExit("east", theatre);
		outside.AddExit("south", lab);
		outside.AddExit("west", pub);
		outside.AddExit("up", theatre); 

		theatre.AddExit("west", outside);
		theatre.AddExit("down", outside);

		pub.AddExit("east", outside);

		lab.AddExit("north", outside);
		lab.AddExit("east", office);

		office.AddExit("west", lab);

		// Create your Items here
		// ...
		
		// And add them to the Rooms
		// ...

		// Start game outside
		startRoom = outside;
		WinRoom = office;
		//HealRoom = lab;
	}

	//  Main play routine. Loops until end of play.
	public void Play()
	{
		PrintWelcome();

		// Enter the main command loop. Here we repeatedly read commands and
		// execute them until the player wants to quit.
		finished = false;
		while (!finished)
		{
			Command command = parser.GetCommand();
			ProcessCommand(command);
		}

		if (!player.IsAlive())
		{
			return;
		}

		Console.WriteLine("Thank you for playing.");
		Console.WriteLine("Press [Enter] to continue.");
		Console.ReadLine();
	}


	// Print out the opening message for the player.
	private void PrintWelcome()
	{
		Console.WriteLine();
		Console.WriteLine("Welcome to Zuul!");
		Console.WriteLine("Zuul is a new, incredibly boring adventure game.");
		Console.WriteLine("Type 'help' if you need help.");
		Console.WriteLine();
		Console.WriteLine("How to play: Use commands like 'go east', 'go up', 'look', and 'quit' to navigate through the rooms.");
		parser.PrintValidCommands();
		Console.WriteLine("To win the game, you have to reach the other side of the university.");
		Console.WriteLine("It's your adventure. Good luck!");
		Console.WriteLine();
		Console.WriteLine(player.CurrentRoom.GetLongDescription());
	}

	// Given a command, process (that is: execute) the command.
	// If this command ends the game, it returns true.
	// Otherwise false is returned.
	private void ProcessCommand(Command command)
	{
		
		if (!player.IsAlive())
		{
    		finished = true;
			return;
		}

		if(command.IsUnknown())
		{
			Console.WriteLine("I don't know what you mean...");
			return;
		}

		switch (command.CommandWord)
		{
			case "help":
				PrintHelp();
				break;
			case "go":
				GoRoom(command);
				break;
			case "look":
			    Look();
			 	break;
			case "up":
			    GoRoom(new Command("go", "up"));
			    break;
			case "down":
			    GoRoom(new Command("go", "down"));
			    break;
			case "status":
			    PrintStatus();
				break;
			case "heal":
				HealPlayer();
				break;
			case "quit":
				finished = true;
				break;
			
		}

		return;
	}

	// ######################################
	// implementations of user commands:
	// ######################################
	private void Look()
	{
		Console.WriteLine(player.CurrentRoom.GetLongDescription());

	}
	// Print out some help information.
	// Here we print the mission and a list of the command words.
	private void PrintStatus()
	{
		Console.WriteLine("You have " + player.GetHealth() + " health points.");
	}
	private void PrintHelp()
	{
		Console.WriteLine("You are lost. You are alone.");
		Console.WriteLine("You wander around at the university.");
		Console.WriteLine();
		// let the parser print the commands
		parser.PrintValidCommands();
	}
	private void HealPlayer()
	{
		player.Heal(10);
		Console.WriteLine("You healed 10 points.");
    	Console.WriteLine("Health: " + player.GetHealth());
	}

	// Try to go to one direction. If there is an exit, enter the new
	// room, otherwise print an error message.
	private void GoRoom(Command command)
	{
		if(!command.HasSecondWord())
		{
			// if there is no second word, we don't know where to go...
			Console.WriteLine("Go where?");
			return;
		}

		string direction = command.SecondWord;

		
		// Try to go to the next room.
		Room nextRoom = player.CurrentRoom.GetExit(direction);

		if (nextRoom == null)
		{
			Console.WriteLine("There is no door to "+direction+"!");
			return;
		}
		
		player.CurrentRoom = nextRoom;

		// won
		if (player.CurrentRoom == WinRoom)
		{
			Console.WriteLine("You have Won!");
			finished = true;
			return;
		}
		
		//          من راسي تعديل للهيل بحيث يمكن ان ترجع الصحه 20 اذا راح الى (lab)         if (player.CurrentRoom == HealRoom)
		//{
		//	player.Heal (20);
		//	Console.WriteLine("You have healed 20 points!");
		//	return;
		//}

		player.Damage (5);
		if (!player.IsAlive())
		{
			Console.WriteLine("You have died. Game over!");
    		finished = true;
    		return;
		}

		Console.WriteLine(player.CurrentRoom.GetLongDescription());

		
	}
}

