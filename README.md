# NasaSUITS-EV-UIUXPrototype
Project created for competing in the NASA SUITS competition. 

The goal was to create an Augmented Reality UI/UX Interface for the EV (SpaceSuit) to reduce stress during space walks and convey crutial information in a consise manner. 

This project needed to be deployed with its intended build target being the HoloLens 2. 
Since we did not have and were not willing to spend 1.5 months worth of rent in costs, We deployed (by remoting) to the emulator.

Our goal was to create the solution, and **have it run on a powerful computer**, thus, *mostly eliminating performance issues due to the hardware constraints* of the Hololens device.

This project is not finished due to complications with the team, and is an early prototype for which the purpose is to demonstrate the following
# > Speech command functionality. 
    Because space suits are bulky, we wanted to create a hands-free, UI Navigation approach, which involved the use of voice commands

    For showing and hiding UI elements, phrases like "Show [element]" or "Hide [element]", and "[element]" serving as some kind of toggle, would need to be hardcoded for the Microsoft MRTK to detect these things. Since the default implementation from the API is serialized as a .asset file, these could not be created at runtime.

    Given this, the simplest option was to create a custom event router that would detect keywords and map them to events so that each element can listen to its own event instead of different MRTK events.

    To combat this, and if necessary, future plans (which would be executed by the software lead with much more experience than me) included the use of a context engine, which would infer which keywords are being said and bind them to these hardcoded input actions (Which is in essense a larger scale implementation of the my event router system solution), or create some custom implementation.

# > Gaze and controller functionality
    This is further demonstrated by the use buttons and UI Prompts

    I coded a prompt queue, which can send prompt events and messages/types of prompts at runtime. I coded in two that trigger when oxygen variables get low. These prompts can be dismissed by using the hand pointer, by tapping on the physical button, or by saying "Dismiss".

    If it is an emergency prompt, the method to dismiss is to say "Override"
    
# > Spacial Tracking
    The hololens device has 6 degrees of freedom tracking. To demonstrate this, I coded and implemented a sample minimap that appears beside your hand when you look down, as if looking down to check the time on a watch. This is also a main UI element with its own toggle (show/hide/etc.) functionalities.

    You are able to see where you are on this map as denoted by a blue arrow indicator, and its orientation along the map's plane the z rotation of the headset in real life.
