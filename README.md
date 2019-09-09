# Unity-Controller-Input-Handler
Cross controller support for the Unity Engine

Just a small project, trying to make use of Unity only, to create a way to support multiple controllers in the same game, regardless of their type.

It uses Unity's built in tools, along with a unified way of mapping controllers, to create the cross-controller compatability.

# Why though?

Since GamePad manufacturers don't have a standardized way to make controllers, computers interpret their input differently. For example is the X button on a PS4, button 1, while on an Xbox, the A button is button 0. This causes all kinds of annoyances, when making a game that supports multiple controllers. And since I didn't want to buy a plugin, which probably does all kinds of things that I don't need, I decided to make my own.

# How to implement!

- When first starting, either download the package and pull it into your Unity project, or download the source code directly. 

- After that you will notice a Input Handler tab in the top, with a few options. First thing you want to do is run the setup, which creates all the entries in the Input Manager, which are needed for the system. Here you have the choice to create a backup of your old input settings, in case you want to revert later. Just copy the content of the backup, into the InputManager.asset in the ProjectSettings folder, and you are back to where you started.

- After that, you will want to setup the scene. This will create an empty game object, with the InputHandler object added. This is a singleton object, and DontDestroyOnLoad, so you can just throw it into the first scene in your game, or any scene you are testing on, and it will exist forever. 


# Usage
- Since the InputHandler is a singleton, you can just call InputHandler.Instance, and assuming it's on the scene, and has run it's Awake/Start code, you should be good to go.

- In the class, you find a lot of functions, which can be used to get input from controllers. I have made my own fictive gamepad enum, which will be used to "Unify" the different controllers (and even the keyboard/mouse!). So whenever you want get whether a button has been pressed this frame, just call: 
  - `InputHandler.Instance.GetButtonDown(GamepadButton.ActionSouth, 1);` 

- and that will get you the "South" action (X on PS4, A on Xbox and so on). 1 is the PlayerNumber. 0 is by default the keyboard, and every number from then on, corresponds to a controller, connected to your computer. Therefore, if you have a field, which is PlayerNumber, and you pass that into the function, and you have a controller plugged in, you can instantly switch from keyboard to controller, for example.

- It is possible to make Axis act like buttons as well, in case you want the triggers on controllers to be either pressed, or not pressed, by using the `GetAxisAsButton` function.

# Modification
This system is made so it's fairly simple to modify. Adding new controllers when new comes out, should also be as easy as copying one of the existing classes, and changing the buttons. I've mapped all the controllers I have access to (and Xbox One X controller from what I could see on the internet), as well as shown how to make the keyboard seem like a controller, so you don't have to handle that edge case. The keyboard controls are mapped to some random things, so you might want to change that, so it fits to your game. It should be easy enough though! Just go into the KeyboardMapping.cs class, and change the different GamePad keys to whatever key on your keyboard you would like. 

# Restrictions
Since I use some file copying for the Input manager setup, I highly doubt this works on Apple products, apologies.

At this moment, only 10 controllers are supported, but theoretically, assuming Unity doesn't block it at some point, it should be doable to add more entries in the Input Manager.

At this point I have only run testing with up to 4 players, without seeing any performance degredation, but will update this, when I test with more.

This code is tested with Unity 2017.4.27 and 2019.2.1, but I would assume that it will work with basically any version. Will update this after more testing at some point.
