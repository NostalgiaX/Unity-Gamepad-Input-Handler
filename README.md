# Unity-Gamepad-Input-Handler
Cross gamepad support for the Unity Engine

Just a small project, trying to make use of Unity only, to create a way to support multiple gamepads in the same game, regardless of their type.

It uses Unity's built in tools, along with a unified way of mapping gamepads, to create the cross-gamepad compatibility.

# Why though?

Since gamepad manufacturers don't have a standardized way to make gamepads, computers interpret their input differently. For example, is the X button on a PS4, button 1, while on an Xbox, the A button is button 0. This causes all kinds of annoyances, when making a game that supports multiple gamepads. And since I didn't want to buy a plugin, which probably does all kinds of things that I don't need, I decided to make my own.

# How to implement!

- When first starting, either download the package and pull it into your Unity project or download the source code directly. 

- After that you will notice a Input Handler tab in the top, with a few options. First thing you want to do is run the setup, which creates all the entries in the Input Manager, which are needed for the system. Here you have the choice to create a backup of your old input settings, in case you want to revert later. Just copy the content of the backup, into the InputManager.asset in the ProjectSettings folder, and you are back to where you started.

- After that, you will want to setup the scene. This will create an empty game object, with the InputHandler object added. This is a singleton object, and DontDestroyOnLoad, so you can just throw it into the first scene in your game, or any scene you are testing on, and it will exist forever. 


# Usage
- Since the InputHandler is a singleton, you can just call InputHandler.Instance, and assuming it's on the scene, and has run its Awake/Start code, you should be good to go.

- In the class, you find a lot of functions, which can be used to get input from gamepads. I have made my own fictive gamepad enum, which will be used to "Unify" the different gamepads (and even the keyboard/mouse!). So, whenever you want get whether a button has been pressed this frame, just call: 
  - `InputHandler.Instance.GetButtonDown(GamepadButton.ActionSouth, 1);` 

- and that will get you the "South" action (X on PS4, A on Xbox and so on). 1 is the PlayerNumber. 0 is by default the keyboard, and every number from then on, corresponds to a gamepad, connected to your computer. Therefore, if you have a field, which is PlayerNumber, and you pass that into the function, and you have a gamepad plugged in, you can instantly switch from keyboard to gamepad, for example.

- It is possible to make Axis act like buttons as well, in case you want the triggers on gamepads to be either pressed, or not pressed, by using the `GetAxisAsButton` function.

- The InputHandler automatically starts a coroutine, which looks for new controllers every so often. The time between each check can be tweaked with the field inside of the code. Personally, I decrease the time between checks, at scenes where new players join, and then increase it at places, where new controllers wouldn't have a relevance (like while in a match).
- There are events on controller disconnect and connect. Though, if a controller is disconnected, you can continue to poll input from that gamepad, without any issues, and as soon as it's connected again, the polls will start returning actual responses again. There is no reconnect event, but it's on my to-do list!

# Modification
This system is made so it's fairly simple to modify. Adding new gamepads when new comes out, should also be as easy as copying one of the existing classes, and changing the buttons. I've mapped all the gamepads I have access to (and Xbox One X gamepad from what I could see on the internet), as well as shown how to make the keyboard seem like a gamepad, so you don't have to handle that edge case. The keyboard controls are mapped to some random things, so you might want to change that, so it fits to your game. It should be easy enough though! Just go into the KeyboardMapping.cs class and change the different Gamepad keys to whatever key on your keyboard you would like. 

# Restrictions
Since I use some file copying for the Input manager setup, I highly doubt this works on Apple products, apologies.

At this moment, only 10 gamepads are supported, but theoretically, assuming Unity doesn't block it at some point, it should be doable to add more entries in the Input Manager.

At this point I have only run testing with up to 4 players, without seeing any performance degredation, but will update this, when I test with more.

This code is tested with Unity 2017.4.27 and 2019.2.1, but I would assume that it will work with basically any version. Will update this after more testing at some point.

Note I am unable to provide much help with controllers I do not own myself, so if bugs occur, the help may be limited.

# Future additions
Since this was made to work in my own games, I usually only add something, when I need it for the system. There is, however, a few things that I want to add in the future
- Some tool to remap buttons.
  - At the moment, you would have to do this in game code.
- A small tool, which helps quickly find out what buttons/axis corresponds to what button/axis number in code. This would primarily be for when new controllers are added.
- An event you can subscribe to, which fires when a gamepad, which has previously connected, and disconnected, is reconnected again.
- Probably the biggest addition I would like to make: A feature, where you can add your own images, and "connect" them with controller mappings, so you would be able to do something like PS4Mapping.GetImage(GamepadButton.ActionSouth) which would return whatever image you have assigned. This could then be used to make specific tooltips, based on what controller is used.
